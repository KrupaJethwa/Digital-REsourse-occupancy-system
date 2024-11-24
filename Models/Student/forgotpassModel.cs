using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Digital_Resource_Occupancy_System.Models.Student
{
    public class forgotpassModel
    {
        public string Email { get; set; }
        public string Password { get; set; }

        public string ResetPasswordToken { get; set; } // For storing token
        public DateTime? ResetPasswordExpiry { get; set; } // Token expiry time
    }
}