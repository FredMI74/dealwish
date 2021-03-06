using dw_webservice.Models;
using dw_webservice.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;


namespace dw_webservice
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
            services.AddTransient<CidadesRepository, CidadesRepository>();
            services.AddTransient<PlanosRepository, PlanosRepository>();
            services.AddTransient<ConfigRepository, ConfigRepository>();
            services.AddTransient<PermissoesRepository, PermissoesRepository>();
            services.AddTransient<GrpPermissoesRepository, GrpPermissoesRepository>();
            services.AddTransient<SituacoesRepository, SituacoesRepository>();
            services.AddTransient<GrpProdutosRepository, GrpProdutosRepository>();
            services.AddTransient<TpProdutosRepository, TpProdutosRepository>();
            services.AddTransient<UsuariosRepository, UsuariosRepository>();
            services.AddTransient<DesejosRepository, DesejosRepository>();
            services.AddTransient<EmpresasRepository, EmpresasRepository>();
            services.AddTransient<ContratosRepository, ContratosRepository>();
            services.AddTransient<OfertasRepository, OfertasRepository>();
            services.AddTransient<FaturasRepository, FaturasRepository>();

            services.AddSingleton<IConfiguration>(Configuration);
            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Dealwish API",
                    Description = "Dealwish API"
                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description =
                        "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

               c.AddSecurityRequirement(new OpenApiSecurityRequirement {
               {
                 new OpenApiSecurityScheme
                 {
                   Reference = new OpenApiReference
                   {
                     Type = ReferenceType.SecurityScheme,
                     Id = "Bearer"
                   }
                  },
                  new string[] { }
                }
              });


            });

            Constantes.string_conexao = Configuration.GetSection("ConnectionStrings").GetSection("DealwishConnection").Value;

            if (!Directory.Exists(@"temp"))
            {
                Directory.CreateDirectory(@"temp");
            }

            var key = Encoding.ASCII.GetBytes(Configuration.GetSection("Chaves").GetSection("ChaveJWT").Value);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            //if (env.IsDevelopment())
            //{
                app.UseDeveloperExceptionPage();

                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Dealwish API V1");
                });

            //}

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }
    }
}
