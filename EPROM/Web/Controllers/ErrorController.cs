using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace ePRom.Controllers
{
    public class ErrorController : Controller
    {
        [AllowAnonymous]
        public ActionResult Unauthorize()
        {
            ViewBag.isFilterRequired = false;
            object user = Membership.GetUser();
            if (user != null)
            {
                bool IsAdmin = User.IsInRole("admin");
                bool IsProvider = User.IsInRole("provider");
                bool IsPatient = User.IsInRole("patient");

                if (IsAdmin)
                {
                    ViewBag.UserRole = "admin";
                }

                if (IsProvider)
                {
                    ViewBag.UserRole = "provider";
                }

                if (IsPatient)
                {
                    ViewBag.UserRole = "patient";
                }

                if (!User.Identity.IsAuthenticated)
                {
                    return RedirectToRoute("AdminLogin");
                }
                else
                {
                    return View();
                }
            }
            return View();
        }
    }
}
