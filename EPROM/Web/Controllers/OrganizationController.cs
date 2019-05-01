using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Security;

namespace ePRom.Controllers
{
    public class OrganizationController : Controller
    {
        [System.Web.Http.Authorize(Roles = "organization")]
        public ActionResult OrganizationDetail()
        {
            ViewBag.isMandatoryStep = false;
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToRoute("Login");
            }
            else
            {
                if (Request.QueryString["from"] != null && Request.QueryString["from"].ToString() != "")
                {
                    string from = Request.QueryString["from"];
                    if (from == "role")
                    {
                        Session["isOrganizationCreated"] = "false";
                        ViewBag.isMandatoryStep = true;
                    }
                }
                if (Session["isOrganizationCreated"] != null && Session["isOrganizationCreated"].ToString() == "false")
                {
                    ViewBag.isMandatoryStep = true;
                }

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
