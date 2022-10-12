using Microsoft.EntityFrameworkCore;

namespace Model
{
    public class AdminDbContext : DbContext
    {
        public AdminDbContext(DbContextOptions<AdminDbContext> options): base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<User> Students { get; set; }
        public DbSet<Group> Group { get; set; }
        public DbSet<User> Teachers { get; set; }
        public DbSet<Organisation> Organisations { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<File> Files { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<Activity> Activities { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.LogTo(l => Debug.WriteLine(l));
            optionsBuilder.UseLazyLoadingProxies();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Assignment>().HasOne(e => e.AssignedBy).WithMany().OnDelete(DeleteBehavior.NoAction); //Because cascading delete would be confused by the two FKs to User
            modelBuilder.Entity<Assignment>().HasOne(e => e.AssignedTo).WithMany().OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Task>().Property(e => e.Name).HasColumnName("Title");
            modelBuilder.Entity<Task>().HasOne(e => e.NextTask).WithMany().OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Task>().HasOne(e => e.PreviousTask).WithMany().OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Task>().HasOne(e => e.DescriptionFile).WithMany().OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Task>().HasOne(e => e.HiddenFunctionsFile).WithMany().OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Task>().HasOne(e => e.TestsFile).WithMany().OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Task>().HasMany(e => e.Hints).WithMany(h => h.Tasks);

            modelBuilder.Entity<Hint>().Property(e => e.Name).HasColumnName("Title");
            modelBuilder.Entity<Hint>().Property(e => e.Content).HasColumnName("FileContent");

            modelBuilder.Entity<Organisation>().HasMany(e => e.Teachers).WithOne(e => e.Organisation).OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<User>().HasMany(e => e.Groups).WithMany(e => e.Students).UsingEntity(j => j.ToTable("StudentGroups"));


             }
    }
}
