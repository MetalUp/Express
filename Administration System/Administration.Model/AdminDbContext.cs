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
        public DbSet<Language> Languages { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<File> Files { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<Activity> Activities { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.LogTo(l => Debug.WriteLine(l));
            optionsBuilder.UseLazyLoadingProxies();
        }

        protected override void OnModelCreating(ModelBuilder mb)
        {
            mb.Entity<Assignment>().HasOne(e => e.AssignedBy).WithMany().OnDelete(DeleteBehavior.NoAction); //Because cascading delete would be confused by the two FKs to User
            mb.Entity<Assignment>().HasOne(e => e.AssignedTo).WithMany().OnDelete(DeleteBehavior.NoAction);

            mb.Entity<Task>().Property(e => e.Number).HasColumnName("Number");
            mb.Entity<Task>().HasOne(e => e.NextTask).WithMany().OnDelete(DeleteBehavior.NoAction);
            mb.Entity<Task>().HasOne(e => e.PreviousTask).WithMany().OnDelete(DeleteBehavior.NoAction);
            mb.Entity<Task>().HasOne(e => e.DescriptionFile).WithMany().OnDelete(DeleteBehavior.NoAction);
            mb.Entity<Task>().HasOne(e => e.HiddenCodeFile).WithMany().OnDelete(DeleteBehavior.NoAction);
            mb.Entity<Task>().HasOne(e => e.TestsFile).WithMany().OnDelete(DeleteBehavior.NoAction);
            mb.Entity<Task>().HasMany(e => e.Hints).WithMany(h => h.Tasks);

            mb.Entity<Hint>().Property(e => e.Name).HasColumnName("Title");

            mb.Entity<Language>().HasOne(e => e.WrapperFile).WithMany().OnDelete(DeleteBehavior.NoAction);
            mb.Entity<Language>().HasOne(e => e.HelpersFile).WithMany().OnDelete(DeleteBehavior.NoAction);
            mb.Entity<Language>().HasOne(e => e.RegExRulesFile).WithMany().OnDelete(DeleteBehavior.NoAction);

            mb.Entity<File>().HasOne(e => e.Language).WithMany().OnDelete(DeleteBehavior.NoAction);

            mb.Entity<Project>().HasOne(e => e.Language).WithMany().OnDelete(DeleteBehavior.NoAction);
            mb.Entity<Project>().HasOne(e => e.Language).WithMany().OnDelete(DeleteBehavior.NoAction);

            mb.Entity<Organisation>().HasMany(e => e.Teachers).WithOne(e => e.Organisation).OnDelete(DeleteBehavior.NoAction);

            mb.Entity<User>().HasMany(e => e.Groups).WithMany(e => e.Students).UsingEntity(j => j.ToTable("StudentGroups"));


             }
    }
}
