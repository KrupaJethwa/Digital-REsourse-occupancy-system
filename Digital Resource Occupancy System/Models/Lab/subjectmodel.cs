using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Digital_Resource_Occupancy_System.Models.Lab
{
    [Table("Tbl_Subject")]
    public class subjectmodel
    {
        [Key]
        [Required]
        [StringLength(50)]
        public string SubCode { get; set; }

        [Required]
        [StringLength(100)]
        public string Subject { get; set; }

        [Required]
        [StringLength(6)]
        public string Propgram { get; set; }

        [Required]
        public int Sem { get; set; }

        // Navigation property
        public virtual ICollection<LabScheduleModel> Labs { get; set; }
    }
}