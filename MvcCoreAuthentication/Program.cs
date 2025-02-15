using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using StackExchange.Redis;

namespace MvcCoreAuthentication
{
    public class Program
    {
        const string APPLICATION_NAME = "SharedAuthCookieApp";
        const string COOKIE_NAME = ".AspNet.ApplicationAuthCookie";
        const string COOKIE_PATH = "/";
        const string LOGIN_PATH = "/Account/Login";
        const string SHARING_FOLDER_PATH = @"C:\Sources\Tests\DotNet\SharedAuthCookie";
        const string DATA_PROTECTION_KEY_CACHE_NAME = "MY-SHARED-DATA-PROTECTION-KEY";

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();


            #region Cookie Configuration

            //builder.Services.AddDataProtection()
            //    .PersistKeysToFileSystem(new DirectoryInfo(SHARING_FOLDER_PATH))
            //    .SetApplicationName(APPLICATION_NAME);

            builder.Services.AddDataProtection()
                .PersistKeysToStackExchangeRedis(ConnectionMultiplexer.Connect("localhost:6379"), DATA_PROTECTION_KEY_CACHE_NAME)
                .SetApplicationName(APPLICATION_NAME);

            var chunkingCookieManager = new ChunkingCookieManager();

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.Cookie.HttpOnly = true;
                    options.Cookie.Name = COOKIE_NAME;
                    options.Cookie.Path = COOKIE_PATH;
                    options.Cookie.SameSite = SameSiteMode.Lax;
                    options.CookieManager = chunkingCookieManager;
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
                    options.LoginPath = LOGIN_PATH;
                    options.SlidingExpiration = true;
                });

            #endregion


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
