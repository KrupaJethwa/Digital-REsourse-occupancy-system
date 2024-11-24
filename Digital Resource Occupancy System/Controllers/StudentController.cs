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
            return View();
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

                    // Save other model data to the database
                    // ... (your existing code for saving data)



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
                    Div = model.Div,
                    Sem = model.Sem,
                    CollageYear = model.CollageYear,
                    Password = hashedPassword,  // Ensure password is hashed
                    Signature = model.Signature
                };

                //db.Students.Add(newStudent);
                //db.SaveChanges();

                //Debug.WriteLine("Student record saved successfully.");
                //return RedirectToAction("StudentLogin");

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

            // Convert FacultyId to int


            // Find the faculty by Faculty ID
            var student = db.Students.FirstOrDefault(f => f.StudentID == model.StudentID);
            if (student == null)
            {
                ModelState.AddModelError("", "Invalid STUDENT ID.");
                return View("StudentLogin", model);

            }

            // Hash the entered password to compare with the stored hash
            string hashedPassword = HashPassword(model.Password);

            // Validate the password
            if (student.Password != hashedPassword)
            {
                ModelState.AddModelError("", "Incorrect password.");
                return View("StudentLogin", model);

            }

            // Login successful, set session or authentication here
            Session["StudentID"] = student.StudentID;
            //Session["FacultyEmail"] = faculty.Email;

            // Redirect to dashboard or a protected page
            return RedirectToAction("ResourseRegister");
        }

        // GET: Student/ViewLabTimeTable
        public ActionResult ViewLabTimeTable_student(int? labNo)
        {
            // Fetch all the lab schedules
            var labSchedules = db.Labs.Include("Faculty").Include("Subject").ToList();

            // Optional: Filter based on LabNo if provided
            if (labNo.HasValue)
            {
                labSchedules = labSchedules.Where(l => l.LabNo == labNo.Value).ToList();
            }

            // Fetch upcoming events
            var upcomingEvents = db.Events.Include("Faculty").ToList();

            // Filter events for future dates
            upcomingEvents = upcomingEvents.Where(e => e.Date >= DateTime.Now).ToList();

            // Store upcoming events in ViewBag
            ViewBag.UpcomingEvents = upcomingEvents;

            // Pass lab schedules to the view
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

    }


}
