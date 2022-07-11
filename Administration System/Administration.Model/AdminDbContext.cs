using Microsoft.EntityFrameworkCore;

namespace Model
{
    public class AdminDbContext : DbContext
    {

        private readonly string cs;

        public AdminDbContext(string cs) => this.cs = cs;

        public DbSet<User> Users { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Organisation> Organisations { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<AssignmentActivity> AssignmentActivities { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.LogTo(l => Debug.WriteLine(l));
            optionsBuilder.UseSqlServer(cs);
            optionsBuilder.UseLazyLoadingProxies();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(new User { Id = 1, FriendlyName = "Alie Algol" });
            modelBuilder.Entity<User>().HasData(new User { Id = 2, FriendlyName = "Forrest Fortran" });
            modelBuilder.Entity<User>().HasData(new User { Id = 3, FriendlyName = "James Java" });
        }
       
            public void Delete() => Database.EnsureDeleted();

            public void Create() => Database.EnsureCreated();
    }
}
