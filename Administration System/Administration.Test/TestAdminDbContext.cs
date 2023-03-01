using Microsoft.EntityFrameworkCore;
using Model;

namespace Test;

public class TestAdminDbContext : AdminDbContext {
    public TestAdminDbContext(DbContextOptions<AdminDbContext> options) : base(options) { }
}