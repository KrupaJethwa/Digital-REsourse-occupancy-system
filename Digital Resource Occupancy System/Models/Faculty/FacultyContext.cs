using System.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Digital_Resource_Occupancy_System.Models.Faculty
{
    public class FacultyContext : DbContext
    {
        public FacultyContext() : base("name=DefaultConnection")
        {
        }

        public DbSet<FacultyModel> Faculties { get; set; }
    
     
        

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
