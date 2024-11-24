using Digital_Resource_Occupancy_System.Models.Student;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Digital_Resource_Occupancy_System.Models.Faculty
{

    public class FacultyLoginViewModel
    {
    
        [Required]
        public int FacultyId { get; set; }

        [Required]
        //[StringLength(100, MinimumLength = 6)]
        [RegularExpression(@"^.{6}$", ErrorMessage = "Password must be exactly 6 characters.")]
        public string Password { get; set; }

        public string Email { get; set; }

        public ForgotpassFacluty ForgotpassFacluty { get; set; }

    }
}