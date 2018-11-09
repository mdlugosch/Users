using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Users.Infrastructure;
using Users.Models;

namespace Users
{
    /*
     * Schritt 1:   ASP.Net Identity muss konfiguriert werden damit dessen Datenzugriffsdienste
     *              von MVC genutzt werden können.
     */
    public class Startup
    {
        public Startup(IConfiguration configuration) => Configuration = configuration;

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IPasswordValidator<AppUser>, Infrastructure.CustomPasswordValidator>();
            services.AddTransient<IUserValidator<AppUser>, CustomUserValidator>();

            // Für das ClaimBeispiel eine neue Claimsquelle registrieren
            services.AddSingleton<IClaimsTransformation, LocationClaimsProvider>();
            // AddDbContext:    Fügt Dienste des Entity Framework hinzu. Fügt die Contextklasse der SQL-Datenbank hinzu.
            //                  Die SQL-Datenbank wird über die appsettings und deren Contextstring verbunden.
            // UseSqlServer:    Fügt die Unterstützung von Microsoft SQL Server zum speichern von Daten hinzu.
            services.AddDbContext<AppIdentityDbContext>(options => options.UseSqlServer(Configuration["Data:SportStoreIdentity:ConnectionString"]));

            // AddIdentity:     Konfiguriert die für den Identitydienst nötigen Parameter. Klassen für User und Rollen.
            //                  Es wird festgelegt das EF Core zum speichern und lesen der Daten genutzt wird. (AddEntityFrameworkStores<Kontextklasse>)
            //                  AddDefaultTokenProviders fügt eine Tokenfunktionalität hinzu die z.B. für das ändern des Passwortes benötigt wird.
            //                  opts.Password.RequiredLength Passwortoptionen/anforderungen z.B. Passwortlänge
            services.AddIdentity<AppUser, IdentityRole>(opts =>
            {
                opts.User.RequireUniqueEmail = true;
                //opts.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyz";
                opts.Password.RequiredLength = 6;
                opts.Password.RequireNonAlphanumeric = false;
                opts.Password.RequireLowercase = false;
                opts.Password.RequireUppercase = false;
                opts.Password.RequireDigit = false;
            }).AddEntityFrameworkStores<AppIdentityDbContext>().AddDefaultTokenProviders();
            //services.ConfigureApplicationCookie(opts => opts.LoginPath = "/Users/Login");

            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseStatusCodePages();
            app.UseDeveloperExceptionPage();
            app.UseStaticFiles();
            // Mit UseAuthentication sind Bentuzerdaten nicht mehr im Http-Request enthalten.
            app.UseAuthentication();
            app.UseMvcWithDefaultRoute();
            // DataSeeding for AdminAccount
            //AppIdentityDbContext.CreateAdminAccount(app.ApplicationServices, Configuration).Wait();
        }
    }
}
