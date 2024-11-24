using Digital_Resource_Occupancy_System.Models.Faculty;
using Digital_Resource_Occupancy_System.Models.Student;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Digital_Resource_Occupancy_System.Models.Lab
{
    public class LabContext : DbContext
    {

        public LabContext() : base("name=DefaultConnection")
        {
        }

        public DbSet<LabRegisterModel> labRegisters { get; set; }
        public DbSet<LabModel> Labs { get; set; }

        public DbSet<EventModel> Events { get; set; }

        public DbSet<subjectmodel> Subjects { get; set; }

        public DbSet<LabDetailsModel> LabDetails { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            modelBuilder.Entity<LabModel>()
                .ToTable("Tbl_Lab")  // Ensure this matches the actual table name
                .HasKey(l => l.LabNo)  // Define primary key for lab schedule
                 .Property(l => l.LabNo)
                 .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            modelBuilder.Entity<EventModel>()
          .ToTable("Tbl_Events")  // Ensure this matches the actual table name
          .HasKey(l => l.LabNo)  // Define primary key for lab schedule
           .Property(l => l.LabNo)
           .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            modelBuilder.Entity<subjectmodel>()
               .ToTable("Tbl_Subject")  // Ensure this matches the actual table name
               .HasKey(s => s.SubCode); // Define primary key for subjects

            modelBuilder.Entity<LabDetailsModel>()
              .ToTable("Tbl_Labdetails")  // Ensure this matches the actual table name
              .HasKey(s => s.LabNo);

            base.OnModelCreating(modelBuilder);
        }
    }
}