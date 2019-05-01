using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;

namespace ePRom.Controllers
{
    public class PracticeController : Controller
    {
        public ActionResult OrganizationPracticeList()
        {
            Session["isOrganizationCreated"] = null;
            ViewBag.isMandatoryStep = false;
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToRoute("Login");
            }
            else
            {
                return View();
            }
        }

        public ActionResult Create()
        {
            ViewBag.isMandatoryStep = false;
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToRoute("Login");
            }
            else
            {
                return View();
            }
        }

        public ActionResult OrganizationPracticeDetail()
        {
            ViewBag.isMandatoryStep = false;
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToRoute("Login");
            }
            else
            {
                return View();
            }
        }

        public ActionResult MyDetails()
        {
            ViewBag.isMandatoryStep = false;
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToRoute("Login");
            }
            else
            {
                return View();
            }
        }
    }
}
