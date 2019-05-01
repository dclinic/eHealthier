using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ePRom
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
               name: "Login",
               url: "Login",
               defaults: new { controller = "Provider", action = "Login", id = UrlParameter.Optional }
           );

            routes.MapRoute(
                name: "Role",
                url: "Role",
                defaults: new { controller = "Provider", action = "Registration", id = UrlParameter.Optional }
            );

            routes.MapRoute(
               name: "resetpassword",
               url: "resetpassword",
               defaults: new { controller = "Patient", action = "ResetPassword", id = UrlParameter.Optional }
           );

            routes.MapRoute(
              name: "Register",
              url: "Register",
              defaults: new { controller = "Provider", action = "Register", id = UrlParameter.Optional }
          );

            routes.MapRoute(
            name: "OrganizationPracticeDetail",
            url: "MyPracticeDetail",
            defaults: new { controller = "Practice", action = "OrganizationPracticeDetail", id = UrlParameter.Optional }
        );

            routes.MapRoute(
           name: "OrganizationPracticeList",
           url: "MyPracticeList",
           defaults: new { controller = "Practice", action = "OrganizationPracticeList", id = UrlParameter.Optional }
            );

            routes.MapRoute(
          name: "SearchPatient",
          url: "SearchPatient",
          defaults: new { controller = "Provider", action = "SearchPatient", id = UrlParameter.Optional }
           );

            routes.MapRoute(
              name: "AdminLogin",
              url: "admin/login",
              defaults: new { controller = "Account", action = "Login", id = UrlParameter.Optional }
          );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Provider", action = "Login", id = UrlParameter.Optional }
            );
        }
    }
}