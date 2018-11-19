using System;
using EPES.Areas.Identity.Data;
using EPES.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(EPES.Areas.Identity.IdentityHostingStartup))]
namespace EPES.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<EPESContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("EPESContextConnection")));

                services.AddIdentity<IdentityUser, IdentityRole>()
                    //.AddDefaultIdentity<EPESUser>()
                    .AddEntityFrameworkStores<EPESContext>()
                    .AddDefaultUI()
                    .AddDefaultTokenProviders();
            });
        }
    }
}