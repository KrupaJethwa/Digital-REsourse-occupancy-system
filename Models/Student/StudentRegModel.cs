using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

namespace Digital_Resource_Occupancy_System.Models.Student
{

    public class StudentRegModel
    {


        [Key]
        [Required]
        [StringLength(15)]
        public string StudentID { get; set; }

        [Required(ErrorMessage = "Student Name is required")]
        public string StudentName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string StudentEmail { get; set; }

        [Required(ErrorMessage = "Program is required")]
        public string Program { get; set; }
      

        [Required(ErrorMessage = "Div is required")]
        public int Div { get; set; }

        [Required(ErrorMessage = "Semester is required")]
        public int Sem { get; set; }

        [Required(ErrorMessage = "College Year is required")]
        public int CollageYear { get; set; }

        [Required]
        // [StringLength(100, MinimumLength = 6)]
        [RegularExpression(@"^.{6}$", ErrorMessage = "Password must be exactly 6 characters.")]
        public string Password { get; set; }

        [Required]
        //[StringLength(100, MinimumLength = 6)]
        [RegularExpression(@"^.{6}$", ErrorMessage = "Password must be exactly 6 characters.")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Signature { get; set; }

        //[NotMapped]
        //[Required(ErrorMessage = "Signature is required")]
        //public HttpPostedFileBase SignatureFile { get; set; }
    }

}
