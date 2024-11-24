﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Digital_Resource_Occupancy_System.Models.Student
{
    public class ForgotPassStudent
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string ResetToken { get; set; }

        public DateTime? TokenExpiration { get; set; }
    }
}