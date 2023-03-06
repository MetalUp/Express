using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Model;
using User = Model.Types.User;

namespace Test;

public class TestAdminDbContext : AdminDbContext {
    public static string AppveyorServer => @"(local)\SQL2017";
    public static string LocalServer => @"(localdb)\MSSQLLocalDB;";

#if APPVEYOR
        public static string Server => AppveyorServer;
#else
    public static string Server => LocalServer;
#endif

    public static readonly string CsAdmin = @$"Data Source={Server};Initial Catalog={"AdminTests"};Integrated Security=True;Encrypt=False;";
    

    private readonly string cs;

    private TestAdminDbContext(string cs) => this.cs = cs;

    public TestAdminDbContext() : this(CsAdmin) { }
  

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseSqlServer(cs);
        optionsBuilder.UseLazyLoadingProxies();
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);

        Seed(modelBuilder);
    }
    private static string Hash(string userName) =>
        SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(userName)).Aggregate("", (s, b) => s + b.ToString("x2"));

    private static void Seed(ModelBuilder modelBuilder) {
        modelBuilder.Entity<Organisation>().HasData(new Model.Types.Organisation() { Id = 1, Name  = "" });
        modelBuilder.Entity<Model.Types.User>().HasData(new Model.Types.User() { Id = 1, UserName  = Hash("Richard"), Role = Role.Teacher, OrganisationId = 1, Status = UserStatus.Active});
        modelBuilder.Entity<Language>().HasData(new Model.Types.Language() { LanguageID = "Python", Name  = "Python", AlphaName = "python"});
    }

    public void Create() => Database.EnsureCreated();
    public void Delete() => Database.EnsureDeleted();
}