using IdentityModel;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TCECPortal.Models;
using TCECPortal.Services;
using TCECPortal.Services.FireBase;

namespace TCECPortal
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddAuthentication(options => {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
                .AddCookie(options =>
                {
                    options.LoginPath = "/~";
                })
                .AddFacebook(facbookOption => { 
                    facbookOption.AppId = Configuration.GetSection("AppID").Value;
                    facbookOption.AppSecret = Configuration.GetSection("AppSecret").Value;
                    facbookOption.Scope.Add("user_birthday");
                    facbookOption.Scope.Add("user_gender");
                    facbookOption.Fields.Add("picture");
                    facbookOption.Fields.Add("birthday");
                    facbookOption.Fields.Add("gender");

                    facbookOption.Events = new Microsoft.AspNetCore.Authentication.OAuth.OAuthEvents
                    {
                        OnCreatingTicket = (context) =>
                        {
                            ClaimsIdentity identity = (ClaimsIdentity)context.Principal.Identity;
                            string profileImg = context.User.GetProperty("picture").GetProperty("data").GetProperty("url").ToString();
                            identity.AddClaim(new Claim(JwtClaimTypes.Picture, profileImg));
                            return Task.CompletedTask;
                        }
                    };
                });

            services.AddSession();

            services.AddTransient<IFirebaseService, FirebaseService>();
            services.AddTransient<IRequestService, RequestService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
             
            loggerFactory.AddLog4Net("log4net.config");

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseSession();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Login}/{action=Index}/{id?}");
            });
        }
    }
}
