using EPES.Areas.Identity.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace EPES.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly SignInManager<EPESUser> _signInManager;
        private readonly UserManager<EPESUser> _userManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly IEmailSender _emailSender;

        public LoginModel(
            SignInManager<EPESUser> signInManager,
            UserManager<EPESUser> userManager,
            ILogger<LoginModel> logger,
            IEmailSender emailSender)

        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "{0} จำเป็นต้องกรอกข้อมูล")]
            [Display(Name = "รหัสผู้ใช้งาน (EOffice)")]
            public string EOffice { get; set; }

            [Required(ErrorMessage = "{0} จำเป็นต้องกรอกข้อมูล")]
            [Display(Name = "พาสเวิร์ด (Password)")]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "จำรหัสผู้ใช้ (Remember me?)")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl = returnUrl ?? Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {
                WSEOffice.AuthenUserEoffice1SoapClient soapClient = new WSEOffice.AuthenUserEoffice1SoapClient(WSEOffice.AuthenUserEoffice1SoapClient.EndpointConfiguration.AuthenUserEoffice1Soap);
                WSEOffice.AuthenUserResponse udata = await soapClient.AuthenUserAsync("InternetUser", "InternetPass", Input.EOffice.ToUpper(), Input.Password);

                if (udata.DataUser.Authen)
                {
                    var user = new EPESUser
                    {
                        UserName = Input.EOffice,
                        Title = udata.DataUser.TITLE,
                        Email = udata.DataUser.EMAIL,
                        PIN = udata.DataUser.PIN,
                        FName = udata.DataUser.FNAME,
                        LName = udata.DataUser.LNAME,
                        PosName = udata.DataUser.POSITION_M,
                        Class = udata.DataUser.CLASS_NEW,
                        OfficeId = udata.DataUser.OFFICEID,
                        OfficeName = udata.DataUser.OFFICENAME,
                        GroupName = udata.DataUser.GROUPNAME
                    };
                    var resultCreate = await _userManager.CreateAsync(user, "P@ssw0rd");
                    if (resultCreate.Succeeded)
                    {
                        _logger.LogInformation("User created a new account with password.");

                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        var callbackUrl = Url.Page(
                            "/Account/ConfirmEmail",
                            pageHandler: null,
                            values: new { userId = user.Id, code = code },
                            protocol: Request.Scheme);

                        await _emailSender.SendEmailAsync(user.Email, "Confirm your email",
                            $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                        //await _signInManager.SignInAsync(user, isPersistent: false);
                        //return LocalRedirect(returnUrl);
                    }
                    foreach (var error in resultCreate.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login eoffice");
                    return Page();
                }

                var result = await _signInManager.PasswordSignInAsync(Input.EOffice, "P@ssw0rd", Input.RememberMe, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    return LocalRedirect(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
