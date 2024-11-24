using Digital_Resource_Occupancy_System.Models;
using Digital_Resource_Occupancy_System.Models.Faculty;
using System.Linq;
using System.Web.Mvc;
using System.Security.Cryptography;
using System.Text;
using System;
using Digital_Resource_Occupancy_System.Models.Student;
using System.Data.Entity;
using Digital_Resource_Occupancy_System.Models.Lab;
using System.IO;
using System.Web;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace Digital_Resource_Occupancy_System.Controllers
{
    public class FacultyController : Controller
    {
        private FacultyContext db = new FacultyContext();
        private StudentContext dbstud = new StudentContext();
        private LabContext dblab = new LabContext();

        



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


        // GET: Faculty/Login

        // Method to hash a password using SHA256
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
        public ActionResult FacultyLogin()
        {
            ViewBag.UserType = "Faculty";
            return View(new FacultyLoginViewModel());
        }

        // POST: Faculty/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FacultyLogin(FacultyLoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Convert FacultyId to int
            int parsedFacultyId = model.FacultyId;

            // Find the faculty by Faculty ID
            var faculty = db.Faculties.FirstOrDefault(f => f.FacultyId == parsedFacultyId);
            if (faculty == null)
            {
                ModelState.AddModelError("", "Invalid Faculty ID.");
                return View(model);
            }

            // Hash the entered password to compare with the stored hash
            string hashedPassword = HashPassword(model.Password);

            // Validate the password
            if (faculty.Password != hashedPassword)
            {
                ModelState.AddModelError("", "Incorrect password.");
                return View(model);
            }

            // Login successful, set session or authentication here
            Session["FacultyID"] = faculty.FacultyId;
            Session["FacultyEmail"] = faculty.Email;

            // Redirect to dashboard or a protected page
            return RedirectToAction("FacultyDashboard");
        }



        public ActionResult FacultyRegister()
        {
            //return View();
            return View(new FacultyViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(FacultyViewModel model)
        {
            if (ModelState.IsValid)
            {
                int facultyId;
                bool isValidId = int.TryParse(model.FacultyId.ToString(), out facultyId);

                if (!isValidId)
                {
                    ModelState.AddModelError("", "Invalid Faculty ID. Please enter a valid integer.");
                    return View("FacultyRegister", model);
                }

                // Check if the email or faculty ID already exists
                var existingFaculty = db.Faculties.FirstOrDefault(f => f.Email == model.Email || f.FacultyId == model.FacultyId);
                if (existingFaculty != null)
                {
                    ModelState.AddModelError("", "A faculty with this Email or ID already exists. Please login.");
                    return View("FacultyRegister", model);
                }

                // Hash the password
                string hashedPassword = HashPassword(model.Password);

                // Create the new Faculty object with hashed password
                var newFaculty = new FacultyModel
                {
                    FacultyId = model.FacultyId,
                    FacultyName = model.FacultyName,
                    Email = model.Email,
                    Department = model.Department,
                    Password = hashedPassword
                };

                // Try saving to the database and catch validation errors
                try
                {
                    db.Faculties.Add(newFaculty);
                    db.SaveChanges();

                    // Redirect to the login page upon successful registration
                    return RedirectToAction("FacultyLogin");
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

            // If we got this far, something failed; redisplay form
            return View("FacultyRegister", model);
        }




        // Optional: Faculty Dashboard
        public ActionResult FacultyDashboard()
        {
            if (Session["FacultyID"] == null)
            {
                return RedirectToAction("FacultyLogin");
            }

            return View();
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("FacultyLogin");
        }


        public ActionResult Attendance(int? labNo, string dayOfWeek, DateTime? date,String StudentID)
        {
            if (Session["FacultyID"] == null)
            {
                return RedirectToAction("FacultyLogin");
            }

            // Initialize the base SQL query
            string query = @"
    SELECT 
        S.StudentID,
        S.StudentName,
        L.LabNo,
        LR.RegistrationTime,
        L.DayOfWeek, 
        CASE 
            WHEN LR.StudentID IS NOT NULL THEN 'Present'
            ELSE 'Absent'
        END AS AttendanceStatus
    FROM 
        Tbl_Student S
     JOIN 
        Tbl_LabRegister LR ON S.StudentID = LR.StudentID
      JOIN
        Tbl_Lab L ON LR.LabScheuleid = L.LabScheuleid";

            List<SqlParameter> parameters = new List<SqlParameter>();

            // Apply the filter for LabNo if provided
            if (labNo.HasValue)
            {
                query += " WHERE L.LabNo = @labno";
                parameters.Add(new SqlParameter("@labno", labNo.Value));
            }

            // Apply the filter for DayOfWeek if provided
            if (!string.IsNullOrEmpty(dayOfWeek))
            {
                if (parameters.Count > 0)
                {
                    query += " AND L.DayOfWeek = @dayOfWeek";
                }
                else
                {
                    query += " WHERE L.DayOfWeek = @dayOfWeek";
                }
                parameters.Add(new SqlParameter("@dayOfWeek", dayOfWeek));
            }

            if (date.HasValue)
            {
                if (parameters.Count > 0)
                {
                    query += " AND CONVERT(DATE, LR.RegistrationTime) = @date";
                }
                else
                {
                    query += " WHERE CONVERT(DATE, LR.RegistrationTime) = @date";
                }
                parameters.Add(new SqlParameter("@date", date.Value));
            }

            if (!string.IsNullOrEmpty(StudentID))
            {
                if (parameters.Count > 0)
                {
                    query += " AND S.StudentID = @StudentID";
                }
                else
                {
                    query += " WHERE S.StudentID = @StudentID";
                }
                parameters.Add(new SqlParameter("@StudentID", StudentID));
            }

            query += " ORDER BY L.LabNo, LR.RegistrationTime";

            // Execute the query with parameters
            var attendance = db.Database.SqlQuery<LabReportModel>(
                query,
                parameters.ToArray()
            ).ToList();

            // Get the total number of students
            int totalStudents = attendance.Count();

            // Pass data to the view
            ViewBag.TotalStudents = totalStudents;
            return View(attendance);
        }



        public ActionResult Reports(int? labNo, string dayOfWeek, DateTime? date, string Program,int? Collegeyear,int? Semester, int? Division, string SubCode)
        {
            if (Session["FacultyID"] == null)
            {
                return RedirectToAction("FacultyLogin");
            }

            // Initialize the base SQL query
            string query = @"
    SELECT 
        L.LabNo,
        LR.StudentID,
        S.StudentEmail,
        S.CollageYear,
        S.Program,
        S.Div,
        S.Sem,
        L.SubCode,
        L.DayOfWeek,
        L.StartTime,
        L.EndTime,
        LR.RegistrationTime,
        LR.PCIPAddress
    FROM 
        Tbl_Student S
    JOIN 
        Tbl_LabRegister LR ON S.StudentID = LR.StudentID
    JOIN
        Tbl_Lab L ON LR.LabScheuleid = L.LabScheuleid";

            List<SqlParameter> parameters = new List<SqlParameter>();

            // Apply the filter for LabNo if provided
            if (labNo.HasValue)
            {
              
                if (parameters.Count > 0)
                {
                    query += " AND L.LabNo = @labno";
                }
                else
                {
                    query += " WHERE L.LabNo = @labno";
                }
                parameters.Add(new SqlParameter("@labno", labNo.Value));

            }

            // Apply the filter for DayOfWeek if provided
            if (!string.IsNullOrEmpty(dayOfWeek))
            {
                if (parameters.Count > 0)
                {
                    query += " AND L.DayOfWeek = @dayOfWeek";
                }
                else
                {
                    query += " WHERE L.DayOfWeek = @dayOfWeek";
                }
                parameters.Add(new SqlParameter("@dayOfWeek", dayOfWeek));
            }

            if (date.HasValue)
            {
                if (parameters.Count > 0)
                {
                    query += " AND CONVERT(DATE, LR.RegistrationTime) = @date";
                }
                else
                {
                    query += " WHERE CONVERT(DATE, LR.RegistrationTime) = @date";
                }
                parameters.Add(new SqlParameter("@date", date.Value));
            }

            if (!string.IsNullOrEmpty(Program))
            {
                if (parameters.Count > 0)
                {
                    query += " AND  S.Program = @Program";
                }
                else
                {
                    query += " WHERE S.Program = @Program";
                }
                parameters.Add(new SqlParameter("@Program", Program));
            }

            if (!string.IsNullOrEmpty(SubCode))
            {
                if (parameters.Count > 0)
                {
                    query += " AND  L.SubCode = @SubCode";
                }
                else
                {
                    query += " WHERE L.SubCode = @SubCode";
                }
                parameters.Add(new SqlParameter("@SubCode", SubCode));
            }

            if (Collegeyear.HasValue)
            {
                if (parameters.Count > 0)
                {
                    query += " AND  S.CollageYear = @Collegeyear";
                }
                else
                {
                    query += " WHERE S.CollageYear = @Collegeyear";
                }
                parameters.Add(new SqlParameter("@Collegeyear", Collegeyear.Value));
            }

            if (Semester.HasValue)
            {
                if (parameters.Count > 0)
                {
                    query += " AND  S.Sem = @Semester";
                }
                else
                {
                    query += " WHERE  S.Sem = @Semester";
                }
                parameters.Add(new SqlParameter("@Semester", Semester.Value));
            }

            if (Division.HasValue)
            {
                if (parameters.Count > 0)
                {
                    query += " AND  S.Div = @Division";
                }
                else
                {
                    query += " WHERE  S.Div = @Division";
                }
                parameters.Add(new SqlParameter("@Division", Division.Value));
            }

            query += " ORDER BY L.LabNo, LR.RegistrationTime";

            // Execute the query with parameters
            var Reports = db.Database.SqlQuery<LabReportModel>(
                query,
                parameters.ToArray()
            ).ToList();

            // Get the total number of students
            int totalStudents = Reports.Count();

            // Pass data to the view
            ViewBag.TotalStudents = totalStudents;
            return View(Reports);




        }

       


        public ActionResult StudentDetails(string StudentID)
        {
            if (Session["FacultyID"] == null)
            {
                return RedirectToAction("FacultyLogin");
            }

            var students = dbstud.Students.AsQueryable(); 
            // Filter by Student ID if provided
            if (!string.IsNullOrEmpty(StudentID))
            {
                students = students.Where(s => s.StudentID.Equals(StudentID, StringComparison.OrdinalIgnoreCase));
            }

            var sortedStudents = students.OrderBy(s => s.StudentID.ToUpper()).ToList(); 

          
            return View("StudentDetails", sortedStudents);
        }

        [HttpPost]
        public ActionResult Deletestudentdetails(string id)
        {
            
            var student = dbstud.Students.SingleOrDefault(x => x.StudentID == id);

            // Check if it exists
            if (student != null)
            {
                dbstud.Students.Remove(student); // Remove the lab schedule
                dbstud.SaveChanges(); // Save changes to the database
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false });
            }

        }

        public ActionResult StudentLabRegisterDetails(string StudentID)
        {
            if (Session["FacultyID"] == null)
            {
                return RedirectToAction("FacultyLogin");
            }

            // Fetch all lab registrations from StudentLabreg
            var labs = dbstud.labRegisters.AsQueryable();

            // Fetch all lab details and create a dictionary for LabNo lookup
            var labDetails = dbstud.Labs.ToDictionary(l => l.LabScheuleid, l => l.LabNo);
            ViewBag.LabDetailsDict = labDetails;

            // Filter by Student ID if provided
            if (!string.IsNullOrEmpty(StudentID))
            {
                labs = labs.Where(s => s.StudentID.Equals(StudentID, StringComparison.OrdinalIgnoreCase));
            }

            var labsList = labs.ToList(); // Execute query and get results

            return View("StudentLabRegisterDetails", labsList);


        }

        public ActionResult Deletestudentlabdetails(int id)
        {


            var labRegister = dbstud.labRegisters.FirstOrDefault(lr => lr.LabRegisterID == id);


            // Check if it exists
            if (labRegister != null)
            {
                dbstud.labRegisters.Remove(labRegister); // Remove the lab schedule
                dbstud.SaveChanges(); // Save changes to the database
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false });
            }

        }

        public ActionResult UpdatestudentLabs(int? id)
        {
            if (Session["FacultyID"] == null)
            {
                return RedirectToAction("FacultyLogin", "Faculty");
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

            var labRegister = dbstud.labRegisters.FirstOrDefault(lr => lr.LabRegisterID == id);

            if (labRegister == null)
            {
                // If the LabRegister with the given ID is not found, return 404
                return HttpNotFound();
            }

            return View(labRegister);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateStudentlabRegDetails(LabRegisterModel updatedLabRegister)
        {

            var labdetails = dblab.Labs.ToList();
            DateTime now = updatedLabRegister.RegistrationTime;
            string currentDayOfWeek = updatedLabRegister.RegistrationTime.DayOfWeek.ToString(); // Extract day of the week
            TimeSpan currentTime = updatedLabRegister.RegistrationTime.TimeOfDay; // Extract only the time portion of the datetime

            // Check if there is a lab scheduled at the specified time and day
            var isLabScheduled = dblab.Labs
                .Any(l => l.LabScheuleid == updatedLabRegister.LabScheuleid
                          && l.StartTime <= currentTime
                          && l.EndTime >= currentTime
                          && l.DayOfWeek == currentDayOfWeek);

            var labno = dblab.Labs
                .Where(l => l.LabScheuleid == updatedLabRegister.LabScheuleid)
                .Select(l => l.LabNo) // Assuming "LabNo" is the property you want to fetch
                .FirstOrDefault();

            // If no lab is scheduled at the requested time
            //if (!isLabScheduled)
            //{
            //    ModelState.AddModelError("", $"Lab is not registered for any subject at that time on.");

            //    ViewBag.LabDetails = labdetails;
            //    return View("UpdatestudentLabs", updatedLabRegister); // Return with the model
            //}

            // Retrieve the lab schedule to check the start and end times
            var labSchedule = dblab.Labs
                .Where(l => l.LabScheuleid == updatedLabRegister.LabScheuleid)
                .Select(l => new { l.StartTime, l.EndTime })
                .FirstOrDefault();

            if (labSchedule == null)
            {
                ModelState.AddModelError("", "No lab schedule found for today.");

                ViewBag.LabDetails = labdetails;
                return View("UpdatestudentLabs", updatedLabRegister); // Return with the model
            }

            // Define start and end times for the lab
            DateTime labStartTime = now.Date.Add(labSchedule.StartTime); // Set to today's date with the start time
            DateTime labEndTime = now.Date.Add(labSchedule.EndTime); // Set to today's date with the end time

            // Check if there is already an existing registration within the valid time range
            bool existingRegistration = dbstud.labRegisters
                .Any(r => r.LabScheuleid == updatedLabRegister.LabScheuleid
                       && r.StudentID == updatedLabRegister.StudentID
                       && updatedLabRegister.RegistrationTime >= labStartTime
                       && updatedLabRegister.RegistrationTime <= labEndTime);

            if (existingRegistration)
            {
                ModelState.AddModelError("", "You have already registered for this lab during this time.");
             
                ViewBag.LabDetails = labdetails;
                return View("UpdatestudentLabs", updatedLabRegister); // Return with the model
            }

            // Retrieve the existing record based on LabRegisterID
            var labRegister = dbstud.labRegisters
                .SingleOrDefault(l => l.LabRegisterID == updatedLabRegister.LabRegisterID);

            if (labRegister != null)
            {
                // Update the necessary fields
                labRegister.LabScheuleid = updatedLabRegister.LabScheuleid;
               
              

                // Save changes to the database
                dbstud.SaveChanges();

                TempData["SuccessMessage"] = "Lab registration details updated successfully.";
                return RedirectToAction("StudentLabRegisterDetails"); // Redirect to the details view
            }

            // If the record could not be found or updated
            ModelState.AddModelError("", "Record not found or could not be updated.");
          
            ViewBag.LabDetails = labdetails;
            return View("UpdatestudentLabs", updatedLabRegister); // Return with the model
        }



    }
}






