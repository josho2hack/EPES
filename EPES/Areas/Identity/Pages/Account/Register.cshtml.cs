using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using EPES.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Localization;

namespace EPES.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        //private readonly IStringLocalizer<RegisterModel> _localizer;

        private readonly SignInManager<EPESUser> _signInManager;
        private readonly UserManager<EPESUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterModel(
            //IStringLocalizer<RegisterModel> localizer,
            UserManager<EPESUser> userManager,
            SignInManager<EPESUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            //_localizer = localizer;

            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "{0} จำเป็นต้องกรอกข้อมูล")]
            [Display(Name = "รหัสผู้ใช้งาน (EOffice)")]
            public string EOffice { get; set; }

            [Required(ErrorMessage ="{0} จำเป็นต้องกรอกข้อมูล")]
            [EmailAddress]
            [Display(Name = "อีเมล์ (Email)")]
            public string Email { get; set; }

            [Required(ErrorMessage = "{0} จำเป็นต้องกรอกข้อมูล")]
            [StringLength(100, ErrorMessage = "{0} ต้องไม่น้อยกว่า {2} ตัวอักษรและยาวได้สูงสุด {1} ตัวอักษร", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "พาสเวิร์ด (Password)")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "ยืนยันพาสเวิร์ด (Confirm password)")]
            [Compare("Password", ErrorMessage = "พาสเวิร์ดและการยืนยันพาสเวิร์ดไม่ตรงกัน")]
            public string ConfirmPassword { get; set; }
        }

        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {
                var user = new EPESUser { UserName = Input.EOffice, Email = Input.Email };
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { userId = user.Id, code = code },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
