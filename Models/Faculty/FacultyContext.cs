using System.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using Digital_Resource_Occupancy_System.Models.Lab;

namespace Digital_Resource_Occupancy_System.Models.Faculty
{
    public class FacultyContext : DbContext
    {
        public FacultyContext() : base("name=DefaultConnection")
        {
        }

        public DbSet<FacultyModel> Faculties { get; set; }
        public DbSet<ForgotpassFacluty> forgotpass { get; set; }
        // public DbSet<studentLabRegViewModel> studentLabregdetails { get; set; }

        public DbSet<LabModel> Labs { get; set; }

 
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FacultyModel>()
            .ToTable("Tbl_Faculty")
            .HasKey(f => f.FacultyId)
            .Property(f => f.FacultyId)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

          
            base.OnModelCreating(modelBuilder);
        }
    }
}
