using System.Web.Mvc;
using Microsoft.AspNet.Identity.EntityFramework;
using System;

namespace IdentitySample.Controllers
{
    [PermissionAuthorize(Permissions.Home)]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
