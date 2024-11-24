using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Digital_Resource_Occupancy_System.Models.Student
{
    public class studentLoginViewModel
    {
        [Required]
        public string StudentID { get; set; }

        [Required]
        //[StringLength(100, MinimumLength = 6)]
        [RegularExpression(@"^.{6}$", ErrorMessage = "Password must be exactly 6 characters.")]
        public string Password { get; set; }
    }
}