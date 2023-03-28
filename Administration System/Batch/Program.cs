using System.Configuration;
using System.Data.Common;
using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Model;
using NakedFramework.DependencyInjection.Extensions;
using NakedFramework.Menu;
using NakedFramework.Persistor.EFCore.Extensions;
using NakedFramework.Rest.Extensions;
using NakedFunctions.Reflector.Extensions;
using Server;

DbProviderFactories.RegisterFactory("System.Data.SqlClient", SqlClientFactory.Instance);

var builder = Host.CreateDefaultBuilder(args);

// Add services to the container.

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
        frameworkOptions.AddRestfulObjects(options => options.AcceptHeaderStrict = false);
    });

    services.AddDbContext<DbContext, AdminDbContext>(options => { options.UseSqlServer(cs); });
});

var app = builder.Build();

app.Run();