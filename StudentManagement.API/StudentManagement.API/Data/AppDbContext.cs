using Microsoft.EntityFrameworkCore;
using StudentApi.Model;
using StudentManagement.Model;

namespace StudentManagement.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Student UNIQUE
            modelBuilder.Entity<Student>()
                .HasIndex(s => s.Email)
                .IsUnique();

            modelBuilder.Entity<Student>()
                .HasIndex(s => s.StudentCode)
                .IsUnique();

            // User UNIQUE
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            // Seed Courses
            modelBuilder.Entity<Course>().HasData(
                new Course { Id = 1, Title = "Computer Science" },
                new Course { Id = 2, Title = "Mathematics" }
            );

            // Seed Students
            modelBuilder.Entity<Student>().HasData(
                new Student { Id = 1, Name = "Alisa", Email = "alisa@example.com", Age = 22, CourseId = 1, StudentCode = "CS-001" },
                new Student { Id = 2, Name = "Adam", Email = "adam@example.com", Age = 20, CourseId = 2, StudentCode = "MA-002" },
                new Student { Id = 3, Name = "Bloom", Email = "bloom@example.com", Age = 23, CourseId = 1, StudentCode = "CS-003" }
            );

            // Admin user seeding is done at runtime in Program.cs with a hashed password
        }
    }
}
