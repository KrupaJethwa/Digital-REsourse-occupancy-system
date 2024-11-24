using Digital_Resource_Occupancy_System.Models.Faculty;
using Digital_Resource_Occupancy_System.Models.Student;
using System;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;

namespace Digital_Resource_Occupancy_System.Controllers
{
    public class AccountController : Controller
    {
        private readonly StudentContext db = new StudentContext();
        private readonly FacultyContext dbfact = new FacultyContext();
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(studentLoginViewModel model)
        {

            var Emailfortxt = model.StudentEmail; // Trim any leading or trailing spaces
            var user = db.Students.Where(x => x.StudentEmail == model.StudentEmail).FirstOrDefault();

            if (user != null)
            {

                string resetToken = Guid.NewGuid().ToString();


            DateTime tokenExpiration = DateTime.Now.AddHours(1);

                TempData["ResetEmail"] = Emailfortxt;
                TempData["ResetToken"] = resetToken;
                TempData["TokenExpiration"] = tokenExpiration;

                // Create reset link with the token
                var resetLink = Url.Action("ResetPassword", "Account", new { token = resetToken }, protocol: Request.Url.Scheme);


                //Session["ResetEmail"] = Emailfortxt;
                //Session["resetToken"] = resetToken;
                //Session["TokenExpiration"] = tokenExpiration;

                //var resetLink = Url.Action("ResetPassword", "Account", new { token = resetToken }, protocol: Request.Url.Scheme);


                var Subject = "Password Reset Request";
            var Body = $"Please reset your password by clicking:  {resetLink} ";

            SendMail(Emailfortxt, Subject, Body);

            TempData["ErrorMessage"] = "Password reset link has been sent to your email.";
            return RedirectToAction("StudentLogin", "Student");
        }
            else
            {
                TempData["ErrorMessage"] = "Email not found.";
                ModelState.AddModelError("", "Email not found. Please try again!");
                return RedirectToAction("StudentLogin", "Student");
    }

}
        public static void SendMail(string toEmail, string strSubject, string strBody)
        {
            try
            {
                MailMessage objMailMessage = new MailMessage("thakkartulsi.2513@gmail.com", toEmail);
                objMailMessage.Subject = strSubject;
                objMailMessage.Body = strBody;
                //objMailMessage.Body = "Yaa, I'm nervious that day also, and today also..!";

                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);
                smtpClient.Credentials = new System.Net.NetworkCredential()
                {
                    UserName = "thakkartulsi.2513@gmail.com",      
                    Password = "bmzs nmdw ccmj lvwx"        // Google account app password
                };

                smtpClient.EnableSsl = true;
                smtpClient.Send(objMailMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in email send");
            }
        }
        [HttpGet]
        public ActionResult ResetPassword(string token)
        {
            var sessionToken = TempData["ResetToken"] as string;
            var tokenExpiration = TempData["TokenExpiration"] as DateTime?;


            TempData.Keep("ResetToken");
            TempData.Keep("TokenExpiration");
            TempData.Keep("ResetEmail");
            if (sessionToken == null || tokenExpiration == null || tokenExpiration <= DateTime.Now)
            {
               
                return RedirectToAction("InvalidToken");
            }

            if (!string.Equals(sessionToken, token, StringComparison.OrdinalIgnoreCase))
            {
   
                return RedirectToAction("InvalidToken");
            }

            ViewBag.Token = token;
            return View();
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        
        public ActionResult ResetPasswordstudent(string token, string newPassword, string ConfirmPassword)
        {
            // Check if token verification has already been completed
            if (TempData["TokenVerified"] == null)
            {
               
                var storedToken = TempData["ResetToken"] as string;
                var tokenExpiration = TempData["TokenExpiration"] as DateTime?;

               
                if (storedToken == null || tokenExpiration == null || tokenExpiration <= DateTime.Now)
                {
                    ModelState.AddModelError("", "Invalid or expired token.");
                    return View("ResetPassword");
                }

              
                if (storedToken != token)
                {
                    ModelState.AddModelError("", "Token mismatch. Please request a new reset link.");
                    return View("ResetPassword");
                }

                TempData["TokenVerified"] = true;
            }

            
            if (newPassword != ConfirmPassword)
            {
                ModelState.AddModelError("", "Passwords do not match.");
                return View("ResetPassword");
            }

     
            var email = TempData["ResetEmail"] as string;
            if (email == null)
            {
                ModelState.AddModelError("", "Invalid token. Email not found.");
                return View("ResetPassword");
            }

            var student = db.Students.SingleOrDefault(x => x.StudentEmail == email);
            if (student != null)
            {
                student.Password = HashPassword(newPassword);

  
                TempData.Remove("ResetToken");
                TempData.Remove("TokenExpiration");
                TempData.Remove("TokenVerified");

                db.SaveChanges();
                return RedirectToAction("StudentLogin", "Student");
            }
            else
            {
                ModelState.AddModelError("", "Student not found.");
                return View("ResetPassword");
            }
        }

        /////////////////facluty reset password

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FacultyForgotPassword(FacultyLoginViewModel model)
        {

            var Emailfortxt = model.Email; // Trim any leading or trailing spaces
            var user = dbfact.Faculties.Where(x => x.Email == model.Email).FirstOrDefault();

            if (user != null)
            {

                string resetToken = Guid.NewGuid().ToString();


                DateTime tokenExpiration = DateTime.Now.AddHours(1);

                TempData["factResetEmail"] = Emailfortxt;
                TempData["factResetToken"] = resetToken;
                TempData["factTokenExpiration"] = tokenExpiration;

                // Create reset link with the token
                var resetLink = Url.Action("FacultyResetPassword", "Account", new { token = resetToken }, protocol: Request.Url.Scheme);


                //Session["ResetEmail"] = Emailfortxt;
                //Session["resetToken"] = resetToken;
                //Session["TokenExpiration"] = tokenExpiration;

                //var resetLink = Url.Action("ResetPassword", "Account", new { token = resetToken }, protocol: Request.Url.Scheme);


                var Subject = "Password Reset Request";
                var Body = $"Please reset your password by clicking:  {resetLink} ";

                SendMail(Emailfortxt, Subject, Body);

                TempData["ErrorMessage"] = "Password reset link has been sent to your email.";
                return RedirectToAction("FacultyLogin", "Faculty");
            }
            else
            {
                TempData["ErrorMessage"] = "Email not found.";
                ModelState.AddModelError("", "Email not found. Please try again!");
                return RedirectToAction("StudentLogin", "Student");
            }

        }


        public ActionResult FacultyResetPassword(string token)
        {
            var sessionToken = TempData["factResetToken"] as string;
            var tokenExpiration = TempData["factTokenExpiration"] as DateTime?;


            TempData.Keep("factResetToken");
            TempData.Keep("factTokenExpiration");
            TempData.Keep("factResetEmail");
            if (sessionToken == null || tokenExpiration == null || tokenExpiration <= DateTime.Now)
            {

                return RedirectToAction("InvalidToken");
            }

            if (!string.Equals(sessionToken, token, StringComparison.OrdinalIgnoreCase))
            {

                return RedirectToAction("InvalidToken");
            }

            ViewBag.Token = token;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]


        public ActionResult ResetPasswordfact(string token, string newPassword, string ConfirmPassword)
        {
            // Check if token verification has already been completed
            if (TempData["TokenVerified"] == null)
            {

                var storedToken = TempData["factResetToken"] as string;
                var tokenExpiration = TempData["factTokenExpiration"] as DateTime?;


                if (storedToken == null || tokenExpiration == null || tokenExpiration <= DateTime.Now)
                {
                    ModelState.AddModelError("", "Invalid or expired token.");
                    return View("FacultyResetPassword");
                }


                if (storedToken != token)
                {
                    ModelState.AddModelError("", "Token mismatch. Please request a new reset link.");
                    return View("FacultyResetPassword");
                }

                TempData["TokenVerified"] = true;
            }


            if (newPassword != ConfirmPassword)
            {
                ModelState.AddModelError("", "Passwords do not match.");
                return View("ResetPassword");
            }


            var email = TempData["factResetEmail"] as string;
            if (email == null)
            {
                ModelState.AddModelError("", "Invalid token. Email not found.");
                return View("FacultyResetPassword");
            }

            var fact = dbfact.Faculties.SingleOrDefault(x => x.Email == email);
            if (fact != null)
            {
                fact.Password = HashPassword(newPassword);


                TempData.Remove("factResetToken");
                TempData.Remove("factTokenExpiration");
                TempData.Remove("factTokenVerified");

                dbfact.SaveChanges();
                return RedirectToAction("FacultyLogin", "Faculty");
            }
            else
            {
                ModelState.AddModelError("", "Student not found.");
                return View("FacultyResetPassword");
            }
        }

    }
}
