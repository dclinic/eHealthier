using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ePRom.Controllers
{
    public class PatientEpromController : Controller
    {
        [Authorize(Roles = "provider")]
        public ActionResult Index()
        {
            ViewBag.isMandatoryStep = false;

            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToRoute("Login");
            }
            else
            {
                if (Utilities.IsRoleSelected(User.Identity.Name))
                {
                    return View();
                }
                else
                {
                    return RedirectToRoute("Role");
                }
            }
        }

        [Authorize(Roles = "provider")]
        public ActionResult Add()
        {
            ViewBag.isFilterRequired = false;
            ViewBag.isMandatoryStep = false;
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToRoute("Login");
            }
            else
            {
                if (Utilities.IsRoleSelected(User.Identity.Name))
                {
                    return View();
                }
                else
                {
                    return RedirectToRoute("Role");
                }
            }
        }
    }
}
