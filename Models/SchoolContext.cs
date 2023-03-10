using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.IO;

namespace SchoolAppV2.Models
{
    public class SchoolContext : DbContext
    {
        public DbSet<Course> Courses { get; set; }

        public DbSet<Grade> Grades { get; set; }

        public DbSet<CourseGrade> CoursesGrades { get; set; }

        public DbSet<Student> Students { get; set; }

        public DbSet<Archivo> Archivos { get; set; }


        public SchoolContext() : base()
        {

        }


        public SchoolContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.UseSqlServer("name=Conexion");

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Student>()
                .Property(e => e.BirthDate).HasColumnType("date");

            modelBuilder.Entity<Student>()
                .HasMany(s => s.Archivos)
                .WithOne(a => a.Student).HasForeignKey(a => a.StudentId);
                
        }

    }
}
