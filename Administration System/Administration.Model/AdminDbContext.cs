using Microsoft.EntityFrameworkCore;

namespace Model
{
    public class AdminDbContext : DbContext
    {
        public AdminDbContext(DbContextOptions<AdminDbContext> options): base(options) { }


        private readonly string cs;

        public AdminDbContext(string cs) => this.cs = cs;

        public DbSet<User> Users { get; set; }
        public DbSet<User> Students { get; set; }
        public DbSet<Group> Group { get; set; }
        public DbSet<StudentGroup> StudentGroups { get; set; }
        public DbSet<User> Teachers { get; set; }
        public DbSet<Organisation> Organisations { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<Activity> Activities { get; set; }

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

            modelBuilder.Entity<Task>().HasOne(e => e.NextTask).WithMany().OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Task>().HasOne(e => e.PreviousTask).WithMany().OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Organisation>().HasMany(e => e.Teachers).WithOne(e => e.Organisation).OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<StudentGroup>().HasKey(e => new { e.StudentId, e.GroupId });
            modelBuilder.Entity<StudentGroup>().HasOne(e => e.Student).WithMany().HasForeignKey(e => e.StudentId).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<StudentGroup>().HasOne(e => e.Group).WithMany().HasForeignKey(e => e.GroupId).OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Invitation>().Property(e => e.Id).ValueGeneratedNever();
        }
    }
}
