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
        public DbSet<Invitation> Invitations { get; set; }
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
            modelBuilder.Entity<Assignment>().HasOne(e => e.AssignedBy).WithMany().OnDelete(DeleteBehavior.NoAction); //Because cascading delete would be confused by the two FKs to User
            modelBuilder.Entity<Assignment>().HasOne(e => e.AssignedTo).WithMany().OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Invitation>().HasOne(e => e.FromUser).WithMany().OnDelete(DeleteBehavior.NoAction);
            AddSeedData(modelBuilder);
        }

        private void AddSeedData(ModelBuilder modelBuilder)
        {
            //Organisations
            modelBuilder.Entity<Organisation>().HasData(new Organisation { Id = 1, Name = "Not afiliated" });
            modelBuilder.Entity<Organisation>().HasData(new Organisation { Id = 2, Name = "Woodlark Academy" });
            modelBuilder.Entity<Organisation>().HasData(new Organisation { Id = 3, Name = "Upavon Technical College" });


            //Groups
            modelBuilder.Entity<Group>().HasData(new Group { Id = 1, GroupName = "Lower 6th A", OrganisationId = 2 });
            modelBuilder.Entity<Group>().HasData(new Group { Id = 2, GroupName = "Lower 6th B", OrganisationId = 2 });
            modelBuilder.Entity<Group>().HasData(new Group { Id = 3, GroupName = "Upper 6th A", OrganisationId = 2 });
            modelBuilder.Entity<Group>().HasData(new Group { Id = 4, GroupName = "Lower 6th B", OrganisationId = 2 });

            //Users
            modelBuilder.Entity<User>().HasData(new User { Id = 1, FriendlyName = "Alie Algol", UserName="aa@metalup.org", Role = Role.Student, OrganisationId = 2 });
            modelBuilder.Entity<User>().HasData(new User { Id = 2, FriendlyName = "Forrest Fortran", UserName = "ff@metalup.org", Role = Role.Student, OrganisationId = 2 });
            modelBuilder.Entity<User>().HasData(new User { Id = 3, FriendlyName = "James Java", UserName = "jj@metalup.org", Role = Role.Teacher, OrganisationId = 2 });
            modelBuilder.Entity<User>().HasData(new User { Id = 4, FriendlyName = "Harry Haskell", UserName = "hh@metalup.org", Role = Role.Teacher, OrganisationId = 3 });
            modelBuilder.Entity<User>().HasData(new User { Id = 5, FriendlyName = "Sally Smalltalk", UserName = "ss@metalup.org", Role = Role.Student, OrganisationId = 3 });
            modelBuilder.Entity<User>().HasData(new User { Id = 6, FriendlyName = "Peter Python", UserName = "pp@metalup.org", Role = Role.User, OrganisationId = 1});
            modelBuilder.Entity<User>().HasData(new User { Id = 7, FriendlyName = "Lindy Lisp", UserName = "ll@metalup.org", Role = Role.User , OrganisationId = 1 });


            //Tasks
            modelBuilder.Entity<Task>().HasData(new Task { Id = 1, Title = "Task1" });
            modelBuilder.Entity<Task>().HasData(new Task { Id = 2, Title = "Task2" });
            modelBuilder.Entity<Task>().HasData(new Task { Id = 3, Title = "Task3" });

            //Assignments
            modelBuilder.Entity<Assignment>().HasData(new Assignment { Id = 1, TaskId = 1, GroupId = 1, AssignedToId = 1, AssignedById = 3, DueBy = DateTime.Today.AddDays(-1) });
            modelBuilder.Entity<Assignment>().HasData(new Assignment { Id = 2, TaskId = 1, GroupId = 1, AssignedToId = 2, AssignedById = 3, DueBy = DateTime.Today.AddDays(-1) });
            modelBuilder.Entity<Assignment>().HasData(new Assignment { Id = 3, TaskId = 1, GroupId = 1, AssignedToId = 3, AssignedById = 3, DueBy = DateTime.Today.AddDays(-1) });

        }

        public void Delete() => Database.EnsureDeleted();

        public void Create() => Database.EnsureCreated();
    }
}
