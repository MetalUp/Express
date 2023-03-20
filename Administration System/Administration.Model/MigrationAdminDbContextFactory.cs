using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Model; 

public class MigrationAdminDbContextFactory : IDesignTimeDbContextFactory<AdminDbContext> {

    public AdminDbContext CreateDbContext(string[] args) {
        var cs = Environment.GetEnvironmentVariable("connection_string") ?? throw new Exception("Missing connection_string");
        var optionsBuilder = new DbContextOptionsBuilder<AdminDbContext>();
        optionsBuilder.UseSqlServer(cs);

        return new AdminDbContext(optionsBuilder.Options);
    }
}