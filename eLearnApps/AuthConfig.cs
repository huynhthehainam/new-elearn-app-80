using Microsoft.AspNetCore.Authentication.Cookies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearnApps
{
    public static class AuthConfig
    {
        public static void ConfigureAuth(this WebApplicationBuilder builder)
        {
            var url = builder.Configuration.GetValue<string>("FrameAncestorsUrl");

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    var constants = new Constants(builder.Configuration);
                    options.LoginPath = "/Account/LtiView";
                    options.LogoutPath = "/Account/LogOff";
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(constants.Timeout);
                    options.SlidingExpiration = true;

                    options.Events = new CookieAuthenticationEvents
                    {
                        OnRedirectToLogin = context =>
                        {
                            var redirectUri = context.RedirectUri.ToLower();
                            if (redirectUri.Contains(constants.ToolIdPeerFeedback.ToLower()))
                            {
                                context.Response.Redirect(redirectUri.Replace("ltiview", "pflti"));
                            }
                            else
                            {
                                context.Response.Redirect(context.RedirectUri);
                            }
                            return Task.CompletedTask;
                        }
                    };
                });
            builder.Services.AddAuthorization();
        }

        public static void UseAuth(this WebApplication app)
        {
            app.Use(async (context, next) =>
            {
                var configuration = app.Services.GetRequiredService<IConfiguration>();
                context.Response.Headers["Content-Security-Policy"] = $"frame-ancestors 'self' {configuration.GetValue<string>("FrameAncestorsUrl")};";
                await next();
            });

            app.UseAuthentication();
            app.UseAuthorization();
        }
    }
}
