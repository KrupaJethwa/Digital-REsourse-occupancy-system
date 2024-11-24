using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Digital_Resource_Occupancy_System.Models.Lab
{
    [Table("Tbl_Labdetails")]
    public class LabDetailsModel
    {
        [Key]
        [Required]
        public int LabNo { get; set; }

        [Required]
        public int Lab_floor { get; set; }

        [Required]
        [ForeignKey("Faculty")]
        public int facltyIncharg_id { get; set; }

        // Navigation property
        public virtual FacultyModel Faculty { get; set; }
    }
}