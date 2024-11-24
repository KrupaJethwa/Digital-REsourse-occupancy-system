using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;



namespace Digital_Resource_Occupancy_System.Models.Faculty
{
    [Table("Tbl_Faculty")]
    public class FacultyLoginModel
    {
        [Key]
        [Required]
        public int FacultyId { get; set; }

       
        [Required(ErrorMessage = "Password is required")]
        //[StringLength(100, MinimumLength = 6, ErrorMessage = "Password at least 6 characters long")]
        [RegularExpression(@"^.{6}$", ErrorMessage = "Password must be exactly 6 characters.")]
        public string Password { get; set; }
    }
}

