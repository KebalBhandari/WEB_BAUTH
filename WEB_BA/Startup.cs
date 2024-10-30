using Microsoft.AspNetCore.Authentication.Cookies;

namespace WEB_BA
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDistributedMemoryCache();
            services.AddRazorPages();
            services.AddControllersWithViews();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(10);
            });
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
               .AddCookie(options =>
               {
                   options.Cookie.Expiration = TimeSpan.FromMinutes(10);
               });
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/SessionExpired";
                options.SlidingExpiration = true;
            });
        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseExceptionHandler("/ExceptionError");
            }
            else if (env.IsProduction())
            {
                app.UseExceptionHandler("/ExceptionError");
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.Use(async (context, next) =>
            {
                await next();
                if (context.Response.StatusCode == 404)
                {
                    context.Request.Path = "/PageNotFound";
                    await next();
                }
            });

            app.UseStaticFiles();
            app.UseSession();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                        name: "default",
                        pattern: "{controller=Login}/{action=Index}/{id?}");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                        name: "default",
                        pattern: "{controller=PageNotFound}/{action=index}");
            });
        }
    }
}
