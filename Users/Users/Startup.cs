using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
            // AddDbContext:    Fügt Dienste des Entity Framework hinzu. Fügt die Contextklasse der SQL-Datenbank hinzu.
            //                  Die SQL-Datenbank wird über die appsettings und deren Contextstring verbunden.
            // UseSqlServer:    Fügt die Unterstützung von Microsoft SQL Server zum speichern von Daten hinzu.
            services.AddDbContext<AppIdentityDbContext>(options => options.UseSqlServer(Configuration["Data:SportStoreIdentity:ConnectionString"]));

            // AddIdentity:     Konfiguriert die für den Identitydienst nötigen Parameter. Klassen für User und Rollen.
            //                  Es wird festgelegt das EF Core zum speichern und lesen der Daten genutzt wird. (AddEntityFrameworkStores<Kontextklasse>)
            //                  AddDefaultTokenProviders fügt eine Tokenfunktionalität hinzu die z.B. für das ändern des Passwortes benötigt wird.
            services.AddIdentity<AppUser, IdentityRole>().AddEntityFrameworkStores<AppIdentityDbContext>().AddDefaultTokenProviders();

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
        }
    }
}
