using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Digital_Resource_Occupancy_System.Models.Lab
{
    [Table("Tbl_Events")]
    public class EventModel
    {




        [Key]

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LabeventScheduleId { get; set; }


        [Required(ErrorMessage = "Lab number is required")]
        public int LabNo { get; set; }

        [ForeignKey("Faculty")]
        [Required(ErrorMessage = "Faculty Name is required")]
        public int FacultyId { get; set; }

        [Required(ErrorMessage = "Event Name is required")]
        [StringLength(200)]
        public string EventName { get; set; }

        [Required(ErrorMessage = "Date is required")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "start time is required")]
        [DataType(DataType.Time)]
        public TimeSpan StartTime { get; set; }

        [Required(ErrorMessage = "start time is required")]
        [DataType(DataType.Time)]
        public TimeSpan EndTime { get; set; }

        // Navigation property for Faculty (if needed)
        [ForeignKey("FacultyId")]
        public virtual FacultyModel Faculty { get; set; }
    }
}