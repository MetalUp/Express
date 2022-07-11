using System;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Model.Types;

namespace Model
{
    public class AdminDbContext : DbContext
    {

        private readonly string cs;

        public AdminDbContext(string cs) => this.cs = cs;

        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
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
            modelBuilder.Entity<Student>().HasData(new Student { Id = 1, FullName = "Alie Algol" });
            modelBuilder.Entity<Student>().HasData(new Student { Id = 2, FullName = "Forrest Fortran" });
            modelBuilder.Entity<Student>().HasData(new Student { Id = 3, FullName = "James Java" });
        }
       
            public void Delete() => Database.EnsureDeleted();

            public void Create() => Database.EnsureCreated();
    }
}
