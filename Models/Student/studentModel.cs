using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Digital_Resource_Occupancy_System.Models.Student
{
    [Table("Tbl_Student")]
    public class studentModel
    {
        [Key]
        [Required]
        public string StudentID { get; set; }

        [Required(ErrorMessage = "student Name is required")]
        [StringLength(70)]
        public string StudentName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [StringLength(150)]
        public string StudentEmail { get; set; }

        [Required(ErrorMessage = "Div is required")]
        public int Div { get; set; }

        [Required(ErrorMessage = "Program is required")]
        public string Program { get; set; }

        [Required(ErrorMessage = "sem is required")]
        public int Sem { get; set; }
        [Required(ErrorMessage = "collage year is required")]
        public int CollageYear { get; set; }

        [Required(ErrorMessage = "Password is required")]
        //[RegularExpression(@"^.{6}$", ErrorMessage = "Password must be exactly 6 characters.")]
        //[StringLength(100, MinimumLength = 6, ErrorMessage = "Password at least 6 characters long")]
        public string Password { get; set; }

        [Required(ErrorMessage = "signature is required")]
        public string Signature { get; set; }
    }
}