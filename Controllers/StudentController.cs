using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Digital_Resource_Occupancy_System.Models;
using Digital_Resource_Occupancy_System.Models.Faculty;
using Digital_Resource_Occupancy_System.Models.Lab;
using Digital_Resource_Occupancy_System.Models.Student;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Net;


namespace Digital_Resource_Occupancy_System.Controllers
{
    public class StudentController : Controller
    {

        public void MakeImageTransparent(string inputPath, string outputPath)
        {
            using (Bitmap bitmap = new Bitmap(inputPath))
            {
                Bitmap transparentBitmap = new Bitmap(bitmap.Width, bitmap.Height, PixelFormat.Format32bppArgb);

                using (Graphics g = Graphics.FromImage(transparentBitmap))
                {
                    g.Clear(Color.Transparent); // Start with a transparent background


                    Color backgroundColor = DetectBackgroundColor(bitmap);

                    for (int y = 0; y < bitmap.Height; y++)
                    {
                        for (int x = 0; x < bitmap.Width; x++)
                        {
                            Color color = bitmap.GetPixel(x, y);

                            if (IsSimilarColor(color, backgroundColor))
                            {
                                // Set pixel to transparent
                                transparentBitmap.SetPixel(x, y, Color.Transparent);
                            }
                            else
                            {
                                // Copy original pixel
                                transparentBitmap.SetPixel(x, y, color);
                            }
                        }
                    }

                    transparentBitmap.Save(outputPath, ImageFormat.Png);
                }
            }
        }
        private Color DetectBackgroundColor(Bitmap bitmap)
        {
            Dictionary<Color, int> colorCounts = new Dictionary<Color, int>();

            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    Color color = bitmap.GetPixel(x, y);

                    if (color.A > 0) // Skip fully transparent pixels
                    {
                        if (colorCounts.ContainsKey(color))
                        {
                            colorCounts[color]++;
                        }
                        else
                        {
                            colorCounts[color] = 1;
                        }
                    }
                }
            }

            // Find the most frequent color
            var mostFrequentColor = colorCounts.OrderByDescending(c => c.Value).FirstOrDefault().Key;

            return mostFrequentColor;
        }

        // Determines if two colors are similar
        private bool IsSimilarColor(Color color1, Color color2)
        {
            const int tolerance = 30;
            return Math.Abs(color1.R - color2.R) < tolerance &&
                   Math.Abs(color1.G - color2.G) < tolerance &&
                   Math.Abs(color1.B - color2.B) < tolerance;
        }

        private readonly StudentContext db = new StudentContext();
        private LabContext dblab = new LabContext();
        private FacultyContext db1 = new FacultyContext();

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

        // GET: Student/Register
        [HttpGet]
        public ActionResult StudentRegister()
        {
            return View("StudentRegister");
        }


        public ActionResult StudentLogin()
        {
            ViewBag.UserType = "Student";
            var model = new studentLoginViewModel();
            return View(model);
            //return View("StudentLogin");
        }


        public ActionResult ResourseRegister()
        {
            if (Session["StudentID"] == null)
            {
                return RedirectToAction("StudentLogin");
            }
            var labdetails = dblab.Labs.ToList();
            ViewBag.LabDetails = labdetails;
            if (labdetails.Count > 0)
            {
                Console.WriteLine("Lab details fetched successfully.");
            }
            else
            {
                Console.WriteLine("No lab details found.");
            }
             // This fetches all the lab details from the database          
            return View();

            

        }
        public ActionResult StudentDashboard()
        {
            if (Session["StudentID"] == null)
            {
                return RedirectToAction("StudentLogin");
            }
            return View();
        }


        public ActionResult logout()
        {
            Session.Clear();
            return RedirectToAction("StudentLogin");
        }


        // POST: Student Register
        public ActionResult Student_DBRegister(StudentRegModel model, HttpPostedFileBase signature)
        {
           

            // Set the StudentID in the model
           

            if (ModelState.IsValid)
            {
                if (signature != null && signature.ContentLength > 0)
                {
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                    var fileExtension = Path.GetExtension(signature.FileName).ToLower();

                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        ModelState.AddModelError("Signature", "Only jpg, jpeg, and png files are allowed.");
                        return View(); // Return the view with the error
                    }

                    var transparentPath = Path.Combine(Server.MapPath("~/UploadedSignatures"), model.StudentID + "_transparent.png");

                    // Save the original image temporarily to create a transparent version
                    var tempPath = Path.Combine(Server.MapPath("~/UploadedSignatures"), model.StudentID + Path.GetExtension(signature.FileName));
                    signature.SaveAs(tempPath);

                    // Create a transparent version of the signature
                    MakeImageTransparent(tempPath, transparentPath);

                    // Clean up the temporary file
                    System.IO.File.Delete(tempPath); // Delete the original image

                    // Save the path of the transparent image to the model
                    model.Signature = "/UploadedSignatures/" + model.StudentID + "_transparent.png"; // Store relative path for display

            



                }

                // Check if the student ID already exists
                var existingStudent = db.Students.FirstOrDefault(s => s.StudentID == model.StudentID || s.StudentEmail == model.StudentEmail);
                if (existingStudent != null)
                {
                    ModelState.AddModelError("", "A student with this Email or ID already exists. Please login.");
                    return View("StudentRegister"); // Return the same view with errors
                }
                string hashedPassword = HashPassword(model.Password);

                var newStudent = new studentModel
                {
                    StudentID = model.StudentID,
                    StudentName = model.StudentName,
                    StudentEmail = model.StudentEmail,
                    Program = model.Program,
                    Div = model.Div,
                    Sem = model.Sem,
                    CollageYear = model.CollageYear,
                    Password = hashedPassword,  // Ensure password is hashed
                    Signature = model.Signature
                };

              

                try
                {
                    db.Students.Add(newStudent);
                    db.SaveChanges();

                    Debug.WriteLine("Student record saved successfully.");
                    return RedirectToAction("StudentLogin");

                 
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                {
                    foreach (var validationErrors in ex.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            System.Diagnostics.Debug.WriteLine($"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
                        }
                    }
                    ModelState.AddModelError("", "An error occurred while saving the data. Please check the inputs.");
                }

            }

            // Return the same view with errors if ModelState is invalid
            return View("StudentRegister"); // This handles the case where the ModelState is not valid.
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
 
        public ActionResult student_Login(studentLoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("StudentLogin", model);

            }

          
            // Find the faculty by Faculty ID
            var student = db.Students.FirstOrDefault(f => f.StudentID == model.StudentID);
            if (student == null)
            {
                ModelState.AddModelError("", "Invalid STUDENT ID.");
                return View("StudentLogin", model);

            }

      
            string hashedPassword = HashPassword(model.Password);

            // Validate the password
            if (student.Password != hashedPassword)
            {
                ModelState.AddModelError("", "Incorrect password.");
                return View("StudentLogin", model);

            }

            Session["StudentID"] = student.StudentID;
            return RedirectToAction("StudentDashboard");
        }

        // GET: Student/ViewLabTimeTable
        public ActionResult ViewLabTimeTable_student(int? labNo)
        {
            if (Session["StudentID"] == null)
            {
                return RedirectToAction("StudentLogin");
            }

            var labSchedules = db.Labs.Include("Faculty").Include("Subject").ToList();
            if (labNo.HasValue)
            {
                labSchedules = labSchedules.Where(l => l.LabNo == labNo.Value).ToList();
            }
            var upcomingEvents = db.Events.Include("Faculty").ToList();
            upcomingEvents = upcomingEvents.Where(e => e.Date >= DateTime.Now).ToList();
            ViewBag.UpcomingEvents = upcomingEvents;
            return View(labSchedules);
        }


        public ActionResult StudentViewProfile()
        {
            if (Session["StudentID"] == null)
            {
                return RedirectToAction("StudentLogin");
            }

            string studentId = Session["StudentID"].ToString();
            var student = db.Students.FirstOrDefault(s => s.StudentID == studentId);

            if (student == null)
            {
                return HttpNotFound();
            }

            // Create a view model if needed or directly return the student model
            return View(student);
           
        }

        [HttpPost]
        public JsonResult FetchSignature(string id)
        {
            // Assuming you have a Student model that has a Signature property
            var student = db.Students.FirstOrDefault(s => s.StudentID == id);

            if (student != null)
            {
                // Assuming the Signature property contains the path to the signature image
                string signaturePath = student.Signature;

                if (!string.IsNullOrEmpty(signaturePath))
                {
                    // You might want to send the path back to the client if needed
                    return Json(new { success = true, signaturePath = signaturePath });
                }
                else
                {
                    return Json(new { success = false, message = "Signature not found." });
                }
            }

            return Json(new { success = false, message = "Student not found." });
        }

        [HttpPost]
        public ActionResult RegisterLab(string StudentID, string Password, string LabNo)
        {
            if (Session["StudentID"] == null)
            {
                return RedirectToAction("StudentLogin");
            }

            var labdetails = dblab.Labs.ToList();
            ViewBag.LabDetails = labdetails;

            string hashedPassword = HashPassword(Password);
            // Validate the student ID and password
            var student = db.Students.FirstOrDefault(s => s.StudentID == StudentID && s.Password == hashedPassword);
            if (student == null)
            {
                ModelState.AddModelError("", "Student ID or Password is incorrect.");
                
                return View("ResourseRegister"); 
            }

            // Check if the selected lab has a schedule for the current time
            int labNumber;
            if (!int.TryParse(LabNo, out labNumber))
            {
                ModelState.AddModelError("", "Invalid Lab Number.");
                return View("ResourseRegister");
            }

            DateTime now = DateTime.Now;
            string currentDayOfWeek = now.DayOfWeek.ToString();
            TimeSpan currentTime = now.TimeOfDay;

            //Check if the selected lab is scheduled for the current time and day

           var isLabScheduled = dblab.Labs
               .Any(l => l.LabScheuleid == labNumber
                         && l.StartTime <= currentTime
                         && l.EndTime >= currentTime
                         && l.DayOfWeek == currentDayOfWeek);



            var labno = dblab.Labs
            .Where(l => l.LabScheuleid == labNumber)
            .Select(l => l.LabNo) // Assuming "LabNo" is the property you want to fetch
            .FirstOrDefault();
            if (!isLabScheduled)
            {
                ModelState.AddModelError("", $"Lab {labno} is not registered for any subject at this time on {currentDayOfWeek}.");
                return View("ResourseRegister");
            }

            //// Retrieve the lab schedule to set up the time range for registration validation
            var labSchedule = dblab.Labs
                .Where(l => l.LabScheuleid == labNumber && l.DayOfWeek == currentDayOfWeek)
                .Select(l => new { l.StartTime, l.EndTime })
                .FirstOrDefault();

            if (labSchedule == null)
            {
                ModelState.AddModelError("", "No lab schedule found for today.");
                return View("ResourseRegister");
            }

            // Define start and end times for the lab on the current date
            DateTime labStartTime = now.Date.Add(labSchedule.StartTime);
            DateTime labEndTime = now.Date.Add(labSchedule.EndTime);

            // Check if there is an existing registration within the lab schedule time range for this student
            bool existingRegistration = db.labRegisters
                .Any(r => r.LabScheuleid == labNumber
                       && r.StudentID == StudentID
                       && r.RegistrationTime >= labStartTime
                       && r.RegistrationTime <= labEndTime);

            if (existingRegistration)
            {
                ModelState.AddModelError("", "You have already registered for this lab during this time.");
                return View("ResourseRegister");
            }

            var signaturePath = db.Students
                      .Where(sig => sig.StudentID == StudentID)
                      .Select(sig => sig.Signature)
                      .FirstOrDefault();

            if (signaturePath == null)
            {
                ModelState.AddModelError("", "Signature not found for the student.");
                return View("ResourseRegister"); // Return to the view with the error message
            }

            string fileName = Path.GetFileName(signaturePath);
            string currentTimestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            string newFileName = $"{StudentID}_{currentTimestamp}{Path.GetExtension(fileName)}";

            string originalSignaturePath = Server.MapPath("~/UploadedSignatures/" + fileName);
            string targetDirectory = Server.MapPath("~/labregistersinature");

            if (!Directory.Exists(targetDirectory))
            {
                Directory.CreateDirectory(targetDirectory);
            }

            var labRegistrationSignaturePath = Path.Combine(targetDirectory, newFileName);

            if (System.IO.File.Exists(originalSignaturePath))
            {
                System.IO.File.Copy(originalSignaturePath, labRegistrationSignaturePath, overwrite: true);
            }
            else
            {
                ModelState.AddModelError("", "Original signature file not found.");
                return View("ResourseRegister");
            }
           // model.Signature = "/UploadedSignatures/" + model.StudentID + "_transparent.png";
            // Convert the absolute path to a relative path
            string relativeSignaturePath = "/labregistersinature/" + newFileName;
            string pcIPAddress = Request.UserHostAddress ?? "Unknown";


            //bool existingRegistrationPC = db.labRegisters
            //   .Any(r => r.LabScheuleid == labNumber
            //          && r.StudentID == StudentID
            //           && r.PCIPAddress == pcIPAddress
            //          && r.RegistrationTime >= labStartTime
            //          && r.RegistrationTime <= labEndTime);

            //if (existingRegistration)
            //{
            //    ModelState.AddModelError("", "someone is alredy register for this pc during this time.");
            //    return View("existingRegistrationPC");
            //}



            // Store the relative path in the model
            var labRegistration = new LabRegisterModel
            {
                StudentID = StudentID,
                LabScheuleid = labNumber,
                RegistrationTime = DateTime.Now,
                PCIPAddress = pcIPAddress,
                UploadedSignature = relativeSignaturePath // Store the relative path here
            };

            try
            {
                db.labRegisters.Add(labRegistration);
                db.SaveChanges();
                TempData["SuccessMessage"] = "You are successfully registered for this lab resource.";
               
                return RedirectToAction("ResourseRegister");
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                foreach (var validationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        System.Diagnostics.Debug.WriteLine($"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
                    }
                }
                ModelState.AddModelError("", "An error occurred while saving the data. Please check the inputs.");
            }
            return RedirectToAction("ResourseRegister");        
        }

        public ActionResult UpdateStudentData(string id)
        {
           

            studentModel student = db.Students.SingleOrDefault(e => e.StudentID == id);
            if (student == null)
            {
                return HttpNotFound();
            }

            return View(student); // Pass the student model to the view
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult update_studentdetails(studentModel model, HttpPostedFileBase signature)
        {
            string studentId = Session["StudentID"].ToString();
            var student = db.Students.FirstOrDefault(s => s.StudentID == studentId);

            if (ModelState.IsValid)
            {
                // Ensure we're not comparing with itself for existing records
                // var existingStudent = db.Students.FirstOrDefault(s => s.StudentID == model.StudentID);

                var hashedPassword = HashPassword(model.Password); // Ensure HashPassword function matches hashing logic used for storage
                var existingStudent = db.Students
                    .FirstOrDefault(s => s.StudentID == model.StudentID && s.Password == hashedPassword);

                if (existingStudent == null)
                {
                    ModelState.AddModelError("", "Student Password is invaild.");
                    var studentToReturn = db.Students.SingleOrDefault(e => e.StudentID == studentId);
                    return View("UpdateStudentData", studentToReturn);
                }


                
                existingStudent.StudentName = model.StudentName;
                existingStudent.StudentEmail = model.StudentEmail;
                existingStudent.Program = model.Program;
                existingStudent.Div = model.Div;
                existingStudent.Sem = model.Sem;
                existingStudent.CollageYear = model.CollageYear;



                // Hash the password if it's being updated
                if (!string.IsNullOrEmpty(model.Password)) 
                {
                    existingStudent.Password = HashPassword(model.Password); 
                }

                // Handle file upload for signature
                if (signature != null && signature.ContentLength > 0)
                {


                    var transparentPath = Path.Combine(Server.MapPath("~/UploadedSignatures"), model.StudentID + "_transparent.png");

                    var tempPath = Path.Combine(Server.MapPath("~/UploadedSignatures"), model.StudentID + Path.GetExtension(signature.FileName));
                    signature.SaveAs(tempPath);

                 
                    MakeImageTransparent(tempPath, transparentPath);

             
                    System.IO.File.Delete(tempPath); 

                   
                    existingStudent.Signature = "/UploadedSignatures/" + model.StudentID + "_transparent.png"; // Store relative path for display

                }
                try
                {
                    // Mark the entity as modified and save changes
                    db.Entry(existingStudent).State = EntityState.Modified;
                    db.SaveChanges();

                   
                    return RedirectToAction("StudentViewProfile");
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                {
                    // Log validation errors
                    foreach (var validationErrors in ex.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            System.Diagnostics.Debug.WriteLine($"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
                        }
                    }
                    ModelState.AddModelError("", "An error occurred while updating the data. Please check the inputs.");
                }
            }
            return View("StudentViewProfile"); 
        }
    }
}
