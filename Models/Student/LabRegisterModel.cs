using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Digital_Resource_Occupancy_System.Models.Lab;

namespace Digital_Resource_Occupancy_System.Models.Student
{
    [Table("Tbl_LabRegister")]
    public class LabRegisterModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LabRegisterID { get; set; }

        [Required]
        [StringLength(15)]
        public string StudentID { get; set; }

      
        [Required]
        public int LabScheuleid { get; set; }

        [Required]
        public DateTime RegistrationTime { get; set; }

        [Required]
        [StringLength(100)]
        public string PCIPAddress { get; set; }

        [Required]
        [StringLength(255)]
        public string UploadedSignature { get; set; }

        // Foreign key relationships
        [ForeignKey("LabScheuleid")]

        public virtual FacultyModel Faculty { get; set; }
        //public virtual LabDetailsModel LabDetails { get; set; }

        [ForeignKey("StudentID")]
        public virtual studentModel Student { get; set; }


    }
}