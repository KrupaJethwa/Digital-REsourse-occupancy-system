using Digital_Resource_Occupancy_System.Models;
using Digital_Resource_Occupancy_System.Models.Faculty;
using System.Linq;
using System.Web.Mvc;
using System.Security.Cryptography;
using System.Text;
using System;
using Digital_Resource_Occupancy_System.Models.Student;
using System.Data.Entity;

namespace Digital_Resource_Occupancy_System.Controllers
{
    public class FacultyController : Controller
    {
        private FacultyContext db = new FacultyContext();
        private StudentContext dbstud = new StudentContext();

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
            //if (Session["FacultyID"] == null)
            //{
            //    return RedirectToAction("FacultyLogin");
            //}

            return View();
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("FacultyLogin");
        }

       
        public ActionResult Attendance()
        {
            return View();
        }

        public ActionResult Reports()
        {
            return View();
        }

       


        public ActionResult StudentDetails(string StudentID)
        {
            // Fetch students as an IQueryable so you can apply filters and sorting dynamically
            var students = dbstud.Students.AsQueryable(); // Ensure Students is a valid DbSet<StudentModel>
            // Filter by Student ID if provided
            if (!string.IsNullOrEmpty(StudentID))
            {
                students = students.Where(s => s.StudentID.Equals(StudentID, StringComparison.OrdinalIgnoreCase));
            }

            // Sort the students by Student ID in uppercase and ascending order
            var sortedStudents = students.OrderBy(s => s.StudentID.ToUpper()).ToList(); // Materialize the query with ToList()

            // Return the view with the sorted list of students
            return View("StudentDetails", sortedStudents);
        }

        [HttpPost]
        public ActionResult Deletestudentdetails(string id)
        {
            // Find the lab schedule by ID
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


    }
}






