using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Digital_Resource_Occupancy_System.Models.Faculty
{
    public class FacultyViewModel
    {
        [Required]
        public int FacultyId { get; set; }

        [Required]
        [StringLength(70)]
        public string FacultyName { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        [StringLength(50)]
        public string Department { get; set; }

        [Required]
        // [StringLength(100, MinimumLength = 6)]
        [RegularExpression(@"^.{6}$", ErrorMessage = "Password must be exactly 6 characters.")]
        public string Password { get; set; }

        [Required]
        //[StringLength(100, MinimumLength = 6)]
        [RegularExpression(@"^.{6}$", ErrorMessage = "Password must be exactly 6 characters.")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}