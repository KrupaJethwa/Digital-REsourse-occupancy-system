using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Digital_Resource_Occupancy_System.Models.Lab
{
    public class EventScheduleModel
    {
        [Key]
        [Required]
        public int LabNo { get; set; }

        [Required]
        public int FacultyId { get; set; }

        [Required]
        [StringLength(200)]
        public string EventName { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public TimeSpan StartTime { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public TimeSpan EndTime { get; set; }

        // Navigation property for Faculty (if needed)
        [ForeignKey("FacultyId")]
        public virtual FacultyModel Faculty { get; set; }
    }
}