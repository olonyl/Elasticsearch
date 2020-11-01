using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Auth0API.Options;
using Elasticsearch.API.Middlewares;
using Elasticsearch.Infrastructure.Adapter;
using Elasticsearch.Infrastructure.Extension;
using Elasticsearch.Infrastructure.Repositories;
using Elasticserach.Domain.Interfaces;
using Elasticserach.Service.Interfaces;
using Elasticserach.Service.Services;
using Elastticsearch.API.Helper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Nest;

namespace Elastticsearch.API
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        private SwaggerOptions swaggerOptions = new SwaggerOptions();
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            Configuration.GetSection(nameof(SwaggerOptions)).Bind(swaggerOptions);
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var domain = $"https://{Configuration["Auth0:Domain"]}/";
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = domain;
                    options.Audience = Configuration["Auth0:Audience"];
                });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("read:properties", policy => policy.Requirements.Add(new HasScopeRequirement("read:properties", domain)));
                options.AddPolicy("write:properties", policy => policy.Requirements.Add(new HasScopeRequirement("write:properties", domain)));
            });
            services.AddSwaggerGen(x =>
            {
                x.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = swaggerOptions.Title,
                    Version = swaggerOptions.Version,
                    Description = swaggerOptions.Description,
                    Contact = new OpenApiContact
                    {
                        Name = swaggerOptions.ContactName,
                        Email = swaggerOptions.ContactEmail,
                        Url = new Uri(swaggerOptions.ContactProfile)
                    }
                });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                x.IncludeXmlComments(xmlPath);
            });

            services.AddElasticsearch(Configuration);
            services.AddScoped<IElasticsearchService, ElasticsearchService>();
            services.AddScoped<IBuildingRepository, BuildingRepository>();
            services.AddSingleton<ITypeAdapterFactory, AutomapperTypeAdapterFactory>();
            services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<HttpRequestBodyMiddleware>();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseMiddleware<UnhandledExceptionMiddleware>();
            app.UseSwagger(option => option.RouteTemplate = swaggerOptions.JsonRoute);
            app.UseSwaggerUI(option => option.SwaggerEndpoint(swaggerOptions.UIEndpoint, swaggerOptions.Description));

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            var typeAdapterFactory = app.ApplicationServices.GetService<ITypeAdapterFactory>();
            if (typeAdapterFactory == null) throw new Exception("TypeAdapterFactory has not been added");
            TypeAdapterFactory.SetCurrent(typeAdapterFactory);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
