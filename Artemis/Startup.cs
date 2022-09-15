using Artemis.Data;
using Artemis.Helpers;
using Artemis.Interfaces;
using Artemis.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Artemis
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add service and create Policy with options
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.WithOrigins("http://localhost:4200")
                                .WithMethods("GET", "POST", "PUT", "PATCH", "DELETE", "HEAD")
                                .AllowAnyHeader()
                                .AllowCredentials()
                    );
            });

            // Add framework services.
            services.AddMvc().AddJsonOptions(options => {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

            //// Add SignalR.
            //services
            //    .AddSignalR();

            // Add authentication.
            string domain = $"https://{Configuration["Auth0_Domain"]}/";
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(options =>
            {
                options.Authority = domain;
                options.Audience = Configuration["Auth0_ApiIdentifier"];

                //options.Events = new JwtBearerEvents
                //{
                //    OnMessageReceived = context =>
                //    {
                //        var accessToken = context.Request.Query["access_token"];

                //        // If the request is for our hub...
                //        var path = context.HttpContext.Request.Path;
                //        if (!string.IsNullOrEmpty(accessToken) &&
                //            (path.StartsWithSegments("/chatHub")))
                //        {
                //            // Read the token out of the query string
                //            context.Token = accessToken;
                //        }
                //        return Task.CompletedTask;
                //    }
                //};
            });

            // register the scope authorization handler
            services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();

            // Add our repository type(s)
            services.AddSingleton<ICurrentUserRepository, CurrentUserRepository>();

            // Add our helper method(s)
            services.AddSingleton<IHelperMethods, HelperMethods>();
            services.AddSingleton<IAzureBlobStorage, AzureBlobStorage>();
            services.AddSingleton<IImageUtil, ImageUtil>();

            // Register the Swagger generator, defining one or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Artemis API",
                    Description = "A simple example Artemis API",
                    //TermsOfService = "None",
                    //Contact = new Contact { Name = "Peter Rose", Email = "", Url = "http://Artemis.com/" },
                    //License = new Swashbuckle.AspNetCore.Swagger.License { Name = "Use under LICX", Url = "http://Artemis.com" }
                });

                // Define the ApiKey scheme that's in use (i.e. Implicit Flow)
                c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.ApiKey,
                    Flows = new OpenApiOAuthFlows
                    {
                        Implicit = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri("/auth-server/connect/authorize", UriKind.Relative),
                            Scopes = new Dictionary<string, string>
                            {
                                { "readAccess", "Access read operations" },
                                { "writeAccess", "Access write operations" }
                            }
                        }
                    }
                });
            });

            services.Configure<Settings>(options =>
            {
                options.ConnectionString = Configuration.GetSection("Mongo_ConnectionString").Value;
                options.Database = Configuration.GetSection("Mongo_Database").Value;
                options.Auth0Id = Configuration.GetSection("Auth0_Claims_nameidentifier").Value;
            });

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Enable routing
            app.UseRouting();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Shows UseCors with CorsPolicyBuilder.
            app.UseCors("CorsPolicy");

            app.UseHttpsRedirection();

            // Enable Authentication
            app.UseAuthentication();

            // Enable Authorization
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Artemis V1");
            });
        }
    }
}
