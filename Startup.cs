using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotnetCoreAPI.Data;
using dotnetCoreAPI.Repository;
using dotnetCoreAPI.Repository.IRepository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using AutoMapper;
using dotnetCoreAPI.ParkyMapper;
using System.Reflection;
using System.IO;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace dotnetCoreAPI
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
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<INationalParkRepository, NationalParkRepository>();
            services.AddScoped<ITrailRepository, TrailRepository>();
            services.AddAutoMapper(typeof(ParkyMappings));
            services.AddApiVersioning(options => 
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ReportApiVersions = true;
            });
            //below does services.AddSwaggerGen dynamically
            services.AddVersionedApiExplorer(options => options.GroupNameFormat = "'v'VVV");
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
            services.AddSwaggerGen();

            // services.AddSwaggerGen(options => {
            //     options.SwaggerDoc("ParkyOpenAPISpec",
            //         new Microsoft.OpenApi.Models.OpenApiInfo() {
            //             Title = "Parky API",
            //             Version = "1",
            //             Description = "Udemy Parky API",
            //             Contact = new Microsoft.OpenApi.Models.OpenApiContact()
            //             {
            //                 Email = "m_arch@outlook.com.au",
            //                 Name = "Mark Janssen",
            //                 Url = new Uri("https://github.com/Mark-Cooper-Janssen-Vooles")
            //             },
            //             License = new Microsoft.OpenApi.Models.OpenApiLicense()
            //             {
            //                 Name = "MIT License",
            //                 Url = new Uri("https://en.wikipedia.org/wiki/MIT_License")
            //             }
            //         });

            //         // options.SwaggerDoc("ParkyOpenAPISpecT",
            //         // new Microsoft.OpenApi.Models.OpenApiInfo() {
            //         //     Title = "Parky API (Trails)",
            //         //     Version = "1",
            //         //     Description = "Udemy Parky API Trails",
            //         //     Contact = new Microsoft.OpenApi.Models.OpenApiContact()
            //         //     {
            //         //         Email = "m_arch@outlook.com.au",
            //         //         Name = "Mark Janssen",
            //         //         Url = new Uri("https://github.com/Mark-Cooper-Janssen-Vooles")
            //         //     },
            //         //     License = new Microsoft.OpenApi.Models.OpenApiLicense()
            //         //     {
            //         //         Name = "MIT License",
            //         //         Url = new Uri("https://en.wikipedia.org/wiki/MIT_License")
            //         //     }
            //         // });

            //     var xmlCommentFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            //     var cmlCommentsFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentFile);
            //     options.IncludeXmlComments(cmlCommentsFullPath);
            // });
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseSwagger();
            app.UseSwaggerUI(options => {
                foreach(var desc in provider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint($"/swagger/{desc.GroupName}/swagger.json", desc.GroupName.ToUpperInvariant());
                }
                options.RoutePrefix = "";
            });


            // app.UseSwaggerUI(options => {
            //     options.SwaggerEndpoint("/swagger/ParkyOpenAPISpec/swagger.json", "Parky API National Parks");
            //     // options.SwaggerEndpoint("/swagger/ParkyOpenAPISpecT/swagger.json", "Parky API Trails");
            //     options.RoutePrefix = "";
            // });
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
