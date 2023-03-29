using System.Data.Common;
using System.Data.SqlClient;
using System.Security.Claims;
using System.Security.Principal;
using Batch;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Model;
using NakedFramework.Architecture.Component;
using NakedFramework.DependencyInjection.Extensions;
using NakedFramework.Menu;
using NakedFramework.Persistor.EFCore.Extensions;
using NakedFunctions.Reflector.Extensions;
using Server;

DbProviderFactories.RegisterFactory("System.Data.SqlClient", SqlClientFactory.Instance);

var builder = Host.CreateDefaultBuilder(args);

// Add services to the container.

builder.ConfigureAppConfiguration((hostBuilderContext, configBuilder) => { configBuilder.AddUserSecrets<Handler>(); });

builder.ConfigureServices((hostBuilderContext, services) => {
    var cs = Environment.GetEnvironmentVariable("connection_string") ?? hostBuilderContext.Configuration["ConnectionString"];

    services.AddNakedFramework(frameworkOptions => {
        frameworkOptions.MainMenus = MenuHelper.GenerateMenus(ModelConfig.MainMenus());
        frameworkOptions.AddEFCorePersistor();
        frameworkOptions.AuthorizationConfiguration = AuthorizationHelpers.AdminAuthConfig();
        frameworkOptions.AddNakedFunctions(appOptions => {
            appOptions.DomainTypes = ModelConfig.DomainTypes();
            appOptions.DomainFunctions = ModelConfig.TypesDefiningDomainFunctions();
            appOptions.DomainServices = ModelConfig.DomainServices();
        });
    });

    services.AddDbContext<DbContext, AdminDbContext>(options => { options.UseSqlServer(cs); });

    services.AddScoped<Handler>();
    services.AddScoped<IPrincipal>(s => new ClaimsPrincipal(new ClaimsIdentity()));
});

var app = builder.Build();

app.Services.GetService<IModelBuilder>()!.Build();
app.Services.GetService<Handler>()!.Run();