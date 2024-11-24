using Digital_Resource_Occupancy_System.Models.Faculty;
using Digital_Resource_Occupancy_System.Models.Lab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace Digital_Resource_Occupancy_System.Controllers
{
    public class LabController : Controller
    {
        private LabContext db = new LabContext();
        private FacultyContext db1 = new  FacultyContext();
     
        public ActionResult ViewLabTimeTable(int? labNo)
        {

            if (Session["FacultyID"] == null)
            {
                return RedirectToAction("FacultyLogin", "Faculty");
            }

            var upcomingEvents = db.Events
        .Include(e => e.Faculty)  
        .Where(e => e.Date >= DateTime.Now) 
        .OrderBy(e => e.Date)  
        .ToList();

            // Fetch lab timetable, ordered by LabNo
            var labs = db.Labs
                .Include(l => l.Faculty)  
                .Include(l => l.Subject)  
                .AsQueryable();  

         
            if (labNo.HasValue)
            {
                labs = labs.Where(l => l.LabNo == labNo.Value);  
            }

            
            var sortedLabs = labs
                .OrderBy(l => l.LabNo)  
                .ToList();

          
            var labdetails = db.LabDetails.ToList();

           
            ViewBag.UpcomingEvents = upcomingEvents;
            ViewBag.LabDetails = labdetails;

            return View(sortedLabs);
        }

        public ActionResult AddLabTimeTable()
        {
            if (Session["FacultyID"] == null)
            {
                return RedirectToAction("FacultyLogin", "Faculty");
            }

            var faculties = db1.Faculties.ToList();
            var subjects = db.Subjects.ToList();
            var labdetails = db.LabDetails.ToList();

           
            if (!faculties.Any())
            {
                ModelState.AddModelError("", "No faculties found.");
            }
            if (!subjects.Any())
            {
                ModelState.AddModelError("", "No subjects found.");
            }

            // Pass the lists to the view using ViewBag
            ViewBag.Faculties = faculties;
            ViewBag.Subjects = subjects;
            ViewBag.LabDetails = labdetails;

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult addTimeTable(LabScheduleModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if the same lab, same day, and time already exists
                var existingLab = db.Labs.FirstOrDefault(l => l.LabNo == model.LabNo &&
                                                      l.DayOfWeek == model.DayOfWeek &&
                                                      ((l.StartTime <= model.EndTime && model.StartTime <= l.EndTime)));

                if (existingLab != null)
                {
                    // Proper string interpolation with formatting
                    ModelState.AddModelError("", $"Lab {existingLab.LabNo} is already scheduled from {existingLab.StartTime.ToString(@"hh\:mm")} to {existingLab.EndTime.ToString(@"hh\:mm")} on {existingLab.DayOfWeek}. ");
                    var faculties = db1.Faculties.ToList();
                    var subjects = db.Subjects.ToList();

                    var labdetails = db.LabDetails.ToList(); 

                    ViewBag.Faculties = faculties;
                    ViewBag.Subjects = subjects;
                    ViewBag.LabDetails = labdetails;

                    return View("AddLabTimeTable", model); // Return the view with error
                }

                var newLab = new LabModel
                {
                    LabNo = model.LabNo,
                    FacultyId = model.FacultyId,
                    SubCode = model.SubCode,
                    DIV = model.DIV,
                    DayOfWeek = model.DayOfWeek,
                    StartTime = model.StartTime,
                    EndTime = model.EndTime
                };

                try
                {
                    db.Labs.Add(newLab);
                    db.SaveChanges();

                    // Redirect to the timetable page upon successful registration
                    return RedirectToAction("ViewLabTimeTable");
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

                return RedirectToAction("AddLabTimeTable");
            }

            // Reload faculties and subjects if validation fails
            ViewBag.Faculties = db1.Faculties.ToList();
            ViewBag.Subjects = db.Subjects.ToList();
            ViewBag.LabDetails = db.LabDetails.ToList();
            // Ensure Subjects are loaded again

            return View(model);
        }

        public ActionResult EventSchedule()
        {
            if (Session["FacultyID"] == null)
            {
                return RedirectToAction("FacultyLogin", "Faculty");
            }
            // Fetch faculties and subjects from the database
            var faculties = db1.Faculties.ToList();
            var labdetails = db.LabDetails.ToList();

            // Check if faculties or subjects are null or empty
            if (!faculties.Any())
            {
                ModelState.AddModelError("", "No faculties found.");
            }

            // Pass the lists to the view using ViewBag
            ViewBag.Faculties = faculties;
            ViewBag.LabDetails = labdetails;
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddEventschdule(EventScheduleModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if the same lab, same day, and time already exists
                var existingLab = db.Events.FirstOrDefault(l => l.LabNo == model.LabNo &&
                                                      l.Date == model.Date &&
                                                      ((l.StartTime <= model.EndTime && model.StartTime <= l.EndTime)));

                if (existingLab != null)
                {
                    // Proper string interpolation with formatting
                    ModelState.AddModelError("", $"Lab {existingLab.LabNo} is already scheduled from {existingLab.StartTime.ToString(@"hh\:mm")} to {existingLab.EndTime.ToString(@"hh\:mm")} on {existingLab.Date}. ");
                    var faculties = db1.Faculties.ToList();
                    var labdetails = db.LabDetails.ToList();

                    ViewBag.Faculties = faculties;
                    ViewBag.LabDetails = labdetails;

                    return View("EventSchedule", model); // Return the view with error
                }
                var newEvent = new EventModel
                {
                    LabNo = model.LabNo,
                    EventName = model.EventName,
                    FacultyId = model.FacultyId,
                    Date = model.Date,
                    StartTime = model.StartTime,
                    EndTime = model.EndTime
                };
                System.Diagnostics.Debug.WriteLine($"Saving event: {newEvent.EventName}, {newEvent.Date}, {newEvent.StartTime}, {newEvent.EndTime}");

                try
                {
                    db.Events.Add(newEvent);
                    db.SaveChanges();

                    // Redirect to the timetable page upon successful registration
                    return RedirectToAction("ViewLabTimeTable");
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

                return RedirectToAction("EventSchedule");
            }

            // Reload faculties and subjects if validation fails
            ViewBag.Faculties = db1.Faculties.ToList();
            ViewBag.Subjects = db.Subjects.ToList();
            ViewBag.LabDetails = db.LabDetails.ToList();
            // Ensure Subjects are loaded again

            return View(model);
        }


        public ActionResult UpdateLabTimeTable(int id)
        {
            if (Session["FacultyID"] == null)
            {
                return RedirectToAction("FacultyLogin", "Faculty");
            }
            LabModel lab = db.Labs.SingleOrDefault(e => e.LabScheuleid == id);
            if (lab == null)
            {
                return HttpNotFound();
            }

            // Populate the Faculty dropdown using FacultyContext
            ViewBag.FacultyId = new SelectList(db1.Faculties, "FacultyId", "FacultyName", lab.FacultyId);
            ViewBag.SubCode = new SelectList(db.Subjects, "SubCode", "Subject", lab.SubCode);

            // Populate the Lab dropdown, assuming you have a method for that
            ViewBag.LabNo = new SelectList(db.LabDetails, "LabNo", "LabNo", lab.LabNo);



            return View(lab);
        }

      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update_TimeTable(LabModel model)
        {
            if (ModelState.IsValid)
            {
                // Check for scheduling conflicts with other labs
                var existingLab = db.Labs.FirstOrDefault(l => l.LabNo == model.LabNo &&
                                                              l.DayOfWeek == model.DayOfWeek &&
                                                              ((l.StartTime <= model.EndTime && model.StartTime <= l.EndTime)) &&
                                                              l.LabScheuleid != model.LabScheuleid); // Ensure we don't compare with itself

                if (existingLab != null)
                {
                    // Display error if there is a scheduling conflict
                    ModelState.AddModelError("", $"Lab {existingLab.LabNo} is already scheduled from {existingLab.StartTime.ToString(@"hh\:mm")} to {existingLab.EndTime.ToString(@"hh\:mm")} on {existingLab.DayOfWeek}. ");
                    ViewBag.Faculties = db1.Faculties.ToList();
                    ViewBag.Subjects = db.Subjects.ToList();
                    ViewBag.LabDetails = db.LabDetails.ToList();
                    return View(model); // Return the view with the error
                }

                try
                {
                    // Update the existing lab details
                    db.Entry(model).State = EntityState.Modified; // Set the state to Modified
                    db.SaveChanges(); // Save changes to the database

                    // Redirect to the timetable page upon successful update
                    return RedirectToAction("ViewLabTimeTable");
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

            // Reload faculties and subjects if validation fails
            ViewBag.Faculties = db1.Faculties.ToList();
            ViewBag.Subjects = db.Subjects.ToList();
            ViewBag.LabDetails = db.LabDetails.ToList();

            return View(model); // Return the model back to the view if validation fails
        }



        [HttpPost]
        public ActionResult DeleteLabSchedule(int id)
        {
            // Find the lab schedule by ID
            var labSchedule = db.Labs.SingleOrDefault(x => x.LabScheuleid == id);

            // Check if it exists
            if (labSchedule != null)
            {
                db.Labs.Remove(labSchedule); // Remove the lab schedule
                db.SaveChanges(); // Save changes to the database
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false });
            }
          
        }

        

        [HttpPost]
        public ActionResult DeleteLabEvent(int id)
        {
            // Find the lab schedule by ID
            var EventSchedule = db.Events.SingleOrDefault(x => x.LabeventScheduleId == id);

            // Check if it exists
            if (EventSchedule != null)
            {
                db.Events.Remove(EventSchedule); // Remove the lab schedule
                db.SaveChanges(); // Save changes to the database
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false });
            }

        }

        public ActionResult UpdateEvent(int id)
        {
            if (Session["FacultyID"] == null)
            {
                return RedirectToAction("FacultyLogin", "Faculty");
            }

            EventModel Event = db.Events.SingleOrDefault(e => e.LabeventScheduleId == id);
            if (Event == null)
            {
                return HttpNotFound();
            }
            // Populate the Faculty dropdown using FacultyContext
            ViewBag.FacultyId = new SelectList(db1.Faculties, "FacultyId", "FacultyName", Event.FacultyId);
           
            // Populate the Lab dropdown, assuming you have a method for that
            ViewBag.LabNo = new SelectList(db.LabDetails, "LabNo", "LabNo", Event.LabNo);
            return View(Event);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult update_EventTimeTable(EventModel model)
        {
            if (ModelState.IsValid)
            {
                // Check for scheduling conflicts with other labs
                var existingLab = db.Events.FirstOrDefault(l => l.LabNo == model.LabNo &&
                                                              l.Date == model.Date &&
                                                              ((l.StartTime <= model.EndTime && model.StartTime <= l.EndTime)) &&
                                                              l.LabeventScheduleId != model.LabeventScheduleId); // Ensure we don't compare with itself

                if (existingLab != null)
                {
                    
                    ModelState.AddModelError("", $"Lab {existingLab.LabNo} is already scheduled from {existingLab.StartTime.ToString(@"hh\:mm")} to {existingLab.EndTime.ToString(@"hh\:mm")} on {existingLab.Date}. ");
                    ViewBag.Faculties = db1.Faculties.ToList();
                    ViewBag.LabDetails = db.LabDetails.ToList();
                    return View(model); 
                }

                try
                {
                  
                    db.Entry(model).State = EntityState.Modified; 
                    db.SaveChanges(); 

                   
                    return RedirectToAction("ViewLabTimeTable");
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

         
            ViewBag.Faculties = db1.Faculties.ToList();
            ViewBag.LabDetails = db.LabDetails.ToList();

            return View(model); 
        }
    }
}