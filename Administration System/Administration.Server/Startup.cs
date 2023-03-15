// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Component;
using NakedFramework.DependencyInjection.Extensions;
using NakedFramework.Menu;
using NakedFramework.Persistor.EFCore.Extensions;
using NakedFramework.Rest.Extensions;
using NakedFunctions.Reflector.Extensions;
using Newtonsoft.Json;
using Model;
using Server;
using Microsoft.Extensions.Options;
using static System.Net.WebRequestMethods;

namespace NakedFunctions.Rest.App.Demo
{
    public class Startup
    {

        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        private string Audience => Environment.GetEnvironmentVariable("Audience") ?? Configuration["Auth0:Audience"];

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options => {
                options.Authority = $"https://{Configuration["Auth0:Domain"]}/";
                options.Audience = Audience;
                options.TokenValidationParameters.NameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";
            });
            services.AddControllers()
                .AddNewtonsoftJson(options => options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc);
            services.AddMvc(options => options.EnableEndpointRouting = false);
            services.AddHttpContextAccessor();
            services.AddNakedFramework(frameworkOptions =>
            {
                frameworkOptions.MainMenus = MenuHelper.GenerateMenus(ModelConfig.MainMenus());
                frameworkOptions.AddEFCorePersistor();
                frameworkOptions.AuthorizationConfiguration = AuthorizationHelpers.AdminAuthConfig();
                frameworkOptions.AddNakedFunctions(appOptions =>
                {
                    appOptions.DomainTypes = ModelConfig.DomainTypes();
                    appOptions.DomainFunctions = ModelConfig.TypesDefiningDomainFunctions();
                    appOptions.DomainServices = ModelConfig.DomainServices();
                });
                frameworkOptions.AddRestfulObjects(options => options.AcceptHeaderStrict = false);
            });
            services.AddCors(corsOptions =>
            {
                corsOptions.AddPolicy(MyAllowSpecificOrigins, policyBuilder =>
                {
                    policyBuilder
                        .WithOrigins(
                            "http://localhost:5001",
                            "http://localhost:49998",
                            "https://express.metalup.org",
                            "https://development.metalup.org",
                            "https://test.metalup.org")
                        .AllowAnyHeader()
                        .WithExposedHeaders("Warning", "ETag", "Set-Cookie")
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });
            services.AddDbContext<DbContext, AdminDbContext>(options => {
                options.UseSqlServer(Configuration.GetConnectionString("ILEAdmin"));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IModelBuilder builder, ILoggerFactory loggerFactory)
        {

            // for Demo use Log4Net. Configured in log4net.config  
            loggerFactory.AddLog4Net();

            builder.Build();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();

            app.UseCors(MyAllowSpecificOrigins);
            app.UseRouting();
            app.UseRestfulObjects();
        }
    }
}