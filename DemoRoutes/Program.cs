using DemoRoutes.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Localization.Routing;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Globalization;

namespace DemoRoutes
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configuration de la base de données et de l'identité.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();
            builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ApplicationDbContext>();

            // ------------------------------- Configuration de la localisation. -------------------------------
            builder.Services.AddPortableObjectLocalization(options => options.ResourcesPath = "Resources");
            var supportedCultures = new[] { "en-CA", "fr-CA" };
            var localizationOptions = new RequestLocalizationOptions()
                .SetDefaultCulture(supportedCultures[0])
                .AddSupportedCultures(supportedCultures)
                .AddSupportedUICultures(supportedCultures);

            //localizationOptions.RequestCultureProviders.Insert(0, new RouteDataRequestCultureProvider { Options = localizationOptions });

            localizationOptions.RequestCultureProviders.Insert(0, new CustomRequestCultureProvider(async context =>
            {
                var path = context.Request.Path;
                var segments = path.Value.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                if (segments.Length > 0)
                {
                    var cultureSegment = segments[0];
                    if (supportedCultures.Contains(cultureSegment))
                        return new ProviderCultureResult(cultureSegment);
                }
                return null; // Fallback to default culture if none specified
            }));


            builder.Services.Configure<RequestLocalizationOptions>(options =>
            {
                options = localizationOptions;
            });

            builder.Services.AddControllersWithViews()
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix) //LanguageViewLocationExpanderFormat.Suffix
                .AddDataAnnotationsLocalization();

            var app = builder.Build();

            // Configuration de la pipeline HTTP.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            // ------------------------------- Configuration du middleware de localisation. -------------------------------
            //app.UseRequestLocalization(localizationOptions);

            app.UseRequestLocalizationWithSettings(localizationOptions);
            //app.UseRequestLocalization();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            // Configuration des routes avec support de localisation dans l'URL.
            app.MapControllerRoute(
                name: "default",
                pattern: "{culture}/{controller=Home}/{action=Index}/{id?}");

            app.MapRazorPages();

            app.Run();
        }
    }
}