using System.Data.Common;
using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Model;

namespace Migration;

internal class Program {
    private static IHostBuilder CreateHostBuilder(string cs) =>
        Host.CreateDefaultBuilder(Array.Empty<string>())
            .ConfigureServices((hostContext, services) =>
                                   services.AddDbContext<DbContext, AdminDbContext>(options => { options.UseSqlServer(cs); }));

    private static void Main() {
        var cs = Environment.GetEnvironmentVariable("connection_string") ?? throw new Exception("Missing connection_string");
        var host = CreateHostBuilder(cs).Build();
        using var scope = host.Services.CreateScope();

        DbProviderFactories.RegisterFactory("System.Data.SqlClient", SqlClientFactory.Instance);

        var db = scope.ServiceProvider.GetRequiredService<AdminDbContext>();
        db.Database.Migrate();
    }
}