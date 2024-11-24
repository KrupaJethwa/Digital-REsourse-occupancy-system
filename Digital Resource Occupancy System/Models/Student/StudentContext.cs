using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using Digital_Resource_Occupancy_System.Models.Lab;

namespace Digital_Resource_Occupancy_System.Models.Student
{
    public class StudentContext : DbContext
    {
        public StudentContext() : base("name=DefaultConnection")
        {
        }
        public DbSet<studentModel> Students { get; set; }

        public DbSet<Lab.LabModel> Labs { get; set; }

        public DbSet<Lab.EventModel> Events { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<studentModel>()
                .ToTable("Tbl_Student"); // Ensure it maps to your table name

            modelBuilder.Entity<studentModel>()
                .HasKey(f => f.StudentID)
                .Property(f => f.StudentID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None); // Ensure FacultyId is not auto-generated

            base.OnModelCreating(modelBuilder);
        }

    }
}