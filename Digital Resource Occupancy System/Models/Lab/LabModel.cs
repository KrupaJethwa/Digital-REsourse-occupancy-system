using Digital_Resource_Occupancy_System.Models.Faculty;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Digital_Resource_Occupancy_System.Models.Lab
{
    [Table("Tbl_Lab")]
    public class LabModel
    {
        [Key]

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LabScheuleid { get; set; }



        [Required(ErrorMessage = "Lab number is required")]
        public int LabNo { get; set; }

        [ForeignKey("Faculty")]
        [Required(ErrorMessage = "Faculty Name is required")]
        public int FacultyId { get; set; }
        public FacultyModel Faculty { get; set; }

        [Required(ErrorMessage = "Subject Name is required")]
        [ForeignKey("Subject")]
        [StringLength(50)]
        public string SubCode { get; set; }
        public subjectmodel Subject { get; set; }

        [Required(ErrorMessage = "Div is required")]
        public int DIV { get; set; }

        [Required(ErrorMessage = "Day of week is required")]
        [StringLength(10)]
        public string DayOfWeek { get; set; }

        [Required(ErrorMessage = "start time is required")]
        public TimeSpan StartTime { get; set; }

        [Required(ErrorMessage = "End time is required")]
        public TimeSpan EndTime { get; set; }
    }
}

