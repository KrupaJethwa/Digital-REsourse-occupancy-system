using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Digital_Resource_Occupancy_System.Models
{
    [Table("Tbl_Faculty")]
    public class FacultyModel
    {
        [Key]
        [Required]
        public int FacultyId { get; set; }

        [Required(ErrorMessage = "Faculty Name is required")]
        [StringLength(70)]
        public string FacultyName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [StringLength(100)]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Department is required")]
        [StringLength(50)]
        public string Department { get; set; }

        [Required(ErrorMessage = "Password is required")]
        //[RegularExpression(@"^.{6}$", ErrorMessage = "Password must be exactly 6 characters.")]
        //[StringLength(100, MinimumLength = 6, ErrorMessage = "Password at least 6 characters long")]
        public string Password { get; set; }
    }
}