using System.Web;
using System.Web.Mvc;

namespace Digital_Resource_Occupancy_System
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
