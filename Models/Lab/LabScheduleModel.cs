using Digital_Resource_Occupancy_System.Models.Faculty;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Digital_Resource_Occupancy_System.Models.Lab
{
    public class LabScheduleModel
    {
        [Key]
        public int LabScheuleid { get; set; }
     
        [Required]
        public int LabNo { get; set; }

        [Required]
        public int FacultyId { get; set; }

        [Required]
        public int DIV { get; set; }

        [Required]
        [StringLength(50)]
        public string SubCode { get; set; }

        [Required]
        [StringLength(10)]
        public string DayOfWeek { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }

        // Navigation properties (optional)
        [ForeignKey("FacultyId")]
        public virtual FacultyModel Faculty { get; set; }

        [ForeignKey("SubCode")]
        public virtual subjectmodel Subject { get; set; }
    }
}