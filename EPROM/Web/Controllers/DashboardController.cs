using System;
using Common;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Mvc;

namespace ePRom.Controllers
{
    public class DashboardController : Controller
    {
        [Authorize(Roles = "provider")]
        public ActionResult MyPatientDashaboard()
        {
            ViewBag.isMandatoryStep = false;
            ViewBag.isFilterRequired = false;

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

        public ActionResult Filter()
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

        public ActionResult Patient()
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
        public ActionResult Population()
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

    }
}
