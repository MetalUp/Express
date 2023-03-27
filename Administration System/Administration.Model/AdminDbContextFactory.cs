using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Model; 

public class AdminDbContextFactory : IDesignTimeDbContextFactory<AdminDbContext> {
    public AdminDbContext CreateDbContext(string[] args) {
        var configuration = new ConfigurationBuilder()
                            .AddUserSecrets<AdminDbContext>()
                            .Build();

        var cs = Environment.GetEnvironmentVariable("connection_string") ?? configuration["ConnectionString"];
        var optionsBuilder = new DbContextOptionsBuilder<AdminDbContext>();
        optionsBuilder.UseSqlServer(cs);

        return new AdminDbContext(optionsBuilder.Options);
    }
}