using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Digital_Resource_Occupancy_System.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult SelectPersonType()
        {
            return View();
        }
    }
}