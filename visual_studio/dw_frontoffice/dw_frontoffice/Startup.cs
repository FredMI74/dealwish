using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dw_frontoffice.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace dw_frontoffice
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
            services.AddAuthentication(Constantes.DW_FRONTOFFICE)
                .AddCookie(Constantes.DW_FRONTOFFICE,
           options =>
           {
               options.LoginPath = new PathString("/auth/login");
               options.AccessDeniedPath = new PathString("/auth/denied");
           });

            services.AddMvc();
            services.AddDistributedMemoryCache();

            services.AddSession();
            services.AddHttpContextAccessor();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            DwClienteHttp httpClient = DwClienteHttp.Instance;
            httpClient.client.BaseAddress = new Uri(Configuration.GetSection("Uri").GetSection("Uri").Value);
            httpClient.token_anonimo = Configuration.GetSection("Anonimo").GetSection("Token").Value;
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            string _basepath = "/dw_frontoffice";
            app.UsePathBase(new PathString(_basepath));
            app.Use((context, next) =>
            {
                context.Request.PathBase = _basepath;
                return next();
            });

            app.UseAuthentication();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            
            app.UseSession();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Auth}/{action=Login}/{id?}");
            });
        }
    }
}
