using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Digital_Resource_Occupancy_System.Models
{
   
    public class LabReportModel
    {
        public int LabNo { get; set; }
        public int LabSchudleid { get; set; }
        public string StudentID { get; set; }
        public string StudentEmail { get; set; }
        public string StudentName { get; set; }

        public int CollageYear { get; set; }
        public string Program { get; set; }
        public int Div { get; set; }
        public int Sem { get; set; }
        public string SubCode { get; set; }
        public string DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public DateTime RegistrationTime { get; set; }
        public string PCIPAddress { get; set; }

        public string AttendanceStatus { get; set; }
    }
}