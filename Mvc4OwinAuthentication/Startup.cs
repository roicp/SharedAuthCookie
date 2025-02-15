using Microsoft.AspNetCore.DataProtection;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Interop;
using Owin;
using StackExchange.Redis;
using System;
using System.IO;

[assembly: OwinStartup(typeof(Mvc4OwinAuthentication.Startup))]

namespace Mvc4OwinAuthentication
{
    public class Startup
    {
        const string APPLICATION_NAME = "SharedAuthCookieApp";
        const string COOKIE_NAME = ".AspNet.ApplicationAuthCookie";
        const string COOKIE_PATH = "/";
        const string LOGIN_PATH = "/Account/Login";
        const string SHARING_FOLDER_PATH = @"C:\Sources\Tests\DotNet\SharedAuthCookie";
        const string DATA_PROTECTION_KEY_CACHE_NAME = "MY-SHARED-DATA-PROTECTION-KEY";

        public void Configuration(IAppBuilder app)
        {
            var loginPath = new PathString(LOGIN_PATH);

            var chunkingCookieManager = new ChunkingCookieManager();

            //var dataProtectionProvider = DataProtectionProvider.Create(
            //    new DirectoryInfo(SHARING_FOLDER_PATH),
            //    builder =>
            //    {
            //        builder.SetApplicationName(APPLICATION_NAME);
            //    }
            //).CreateProtector(
            //    "Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationMiddleware",
            //    "Cookies",
            //    "v2"
            //);

            var dataProtectionProvider = DataProtectionProvider.Create(
                new DirectoryInfo(SHARING_FOLDER_PATH),
                builder =>
                {
                    builder.SetApplicationName(APPLICATION_NAME);
                    builder.PersistKeysToStackExchangeRedis(ConnectionMultiplexer.Connect("localhost:6379"), DATA_PROTECTION_KEY_CACHE_NAME);
                }
            ).CreateProtector(
                "Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationMiddleware",
                "Cookies",
                "v2"
            );

            var dataProtectorShim = new DataProtectorShim(dataProtectionProvider);

            var aspNetTicketDataFormat = new AspNetTicketDataFormat(dataProtectorShim);

            var cookieAuthenticationOptions = new CookieAuthenticationOptions
            {
                // This should match the AuthenticationType you use when creating the ClaimsIdentity ("Cookies")
                AuthenticationType = CookieAuthenticationDefaults.AuthenticationType,
                CookieHttpOnly = true,
                CookieManager = chunkingCookieManager,
                CookieName = COOKIE_NAME,
                CookiePath = COOKIE_PATH,
                CookieSameSite = SameSiteMode.Lax,
                ExpireTimeSpan = TimeSpan.FromMinutes(20),
                LoginPath = loginPath,
                SlidingExpiration = true,
                TicketDataFormat = aspNetTicketDataFormat
            };

            app.UseCookieAuthentication(cookieAuthenticationOptions);
        }
    }
}