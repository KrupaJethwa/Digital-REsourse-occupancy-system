using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Digital_Resource_Occupancy_System.Models
{
   
    public class StudentAttendanceModel
    {
        public string StudentID { get; set; }
        public string StudentName { get; set; }
        public string LabNo { get; set; }
        public DateTime RegistrationTime { get; set; }
        public string AttendanceStatus { get; set; } // "Present" or "Absent"
    }
}