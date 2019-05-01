using BAL;
using BLL;
using Common;
using DAL;
using ePRom.Filters;
using ePRom.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;
using WebMatrix.WebData;

namespace ePRom.Controllers
{
    [InitializeSimpleMembership]
    public class ProviderController : Controller
    {
        [AllowAnonymous]
        public ActionResult Register()
        {
            ViewBag.isFilterRequired = false;
            return View();
        }

        [AllowAnonymous]
        public ActionResult Login()
        {
            LoginModel model = new LoginModel();

            if (Request.QueryString["userid"] != null && Request.QueryString["userid"].ToString() != "")
                Session["userid"] = Request.QueryString["userid"];

            string patientSurveyId = Request.QueryString["patientsurveyid"];

            if (patientSurveyId != null && patientSurveyId != "")
            {
                string email = Request.QueryString["email"];

                Session["Email"] = email;
                Session["PatientSurveyId"] = patientSurveyId;

                if (!User.Identity.IsAuthenticated)
                {
                    return RedirectToRoute("Login");
                }
                else
                {
                    string URL = Request.Url.Scheme + "://" + Request.Url.Authority + "/Patient/MyEproms?patientsurveyid=" + patientSurveyId + "&email=" + email;
                    Response.Redirect(URL);
                }
            }
            else
            {
                string deactive = Request.QueryString["deactive"];
                if (deactive == "true")
                {
                    WebSecurity.Logout();
                    Session.RemoveAll();
                }

                ViewBag.ReturnUrl = @Url.Action("Login");
                ModelState.Clear();

                var strMessage = TempData["ErrorMessage"];

                ViewBag.EmailURL = Request.Url.Scheme + "://" + Request.Url.Authority + "/Provider/ValidateToken";
                model.SecretQuestionList = GetSecurityQuestion();
                if (strMessage != null)
                {
                    model.UserName = TempData["UserName"].ToString();
                    TempData.Remove("UserName");
                    TempData.Remove("ErrorMessage");
                    ModelState.AddModelError("", "Your account is locked please contact to administrator");
                }
            }
            return View(model);
        }

        [Authorize(Roles = "provider")]
        public ActionResult Dashboard()
        {
            ViewBag.isMandatoryStep = false;
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToRoute("Login");
            }
            else
            {
                string bUrl = Request.Url.Scheme + "://" + Request.Url.Authority;
                var eUrl = bUrl + "/api/Provider/GetEmailVerificationStatus?UserName=" + User.Identity.Name;
                string res = Entities.CustomeWebRequest(eUrl, "GET");
                bool checkConfirmation = false;
                if (!string.IsNullOrEmpty(res.Trim()))
                {
                    checkConfirmation = Convert.ToBoolean(res);
                }

                if (!checkConfirmation)
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

        [AllowAnonymous]
        public ActionResult RegisterCompleted()
        {
            ViewBag.isMandatoryStep = false;
            if (Utilities.IsRoleSelected(User.Identity.Name))
            {
                return View();
            }
            else
            {
                return RedirectToRoute("Role");
            }
        }

        [AllowAnonymous] //ROle Page
        public ActionResult Registration()
        {
            ViewBag.isMandatoryStep = true;
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToRoute("Login");
            }
            else
            {
                Entities.SetCookie(User.Identity.Name, "username");
                string[] UserInProvider = Roles.FindUsersInRole("provider", User.Identity.Name);
                string[] UserInOrg = Roles.FindUsersInRole("organization", User.Identity.Name);
                string[] UserInPractice = Roles.FindUsersInRole("practice", User.Identity.Name);
                string[] UserInPatient = Roles.FindUsersInRole("patient", User.Identity.Name);

                if (Utilities.IsRoleSelected(User.Identity.Name))
                {
                    return View();
                }
                else
                {
                    if (UserInProvider.Length > 0)
                    {
                        return RedirectToAction("RegistrationDetails", "Provider", new { area = "" });
                    }
                    else if (UserInOrg.Length > 0)
                    {
                        return RedirectToAction("OrganizationDetail", "Organization", new { area = "" });
                    }
                    else if (UserInPractice.Length > 0)
                    {
                        return RedirectToAction("Practice", "MyDetails", new { area = "" });
                    }
                    else if (UserInPatient.Length > 0)
                    {
                        return RedirectToAction("PatientDashboard", "Patient", new { area = "" });
                    }
                }
            }
            return View();
        }

        [AllowAnonymous]
        public ActionResult RegistrationDetails()
        {
            ViewBag.isMandatoryStep = false;

            if (Request.QueryString["from"] != null && Request.QueryString["from"].ToString() != "")
            {
                string from = Request.QueryString["from"];
                if (from == "role")
                {
                    Session["isProviderCreated"] = "false";
                    ViewBag.isMandatoryStep = true;
                }
            }

            ViewBag.isFilterRequired = false;
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToRoute("Login");
            }
            else
            {
                string bUrl = Request.Url.Scheme + "://" + Request.Url.Authority;
                var eUrl = bUrl + "/api/Provider/GetEmailVerificationStatus?UserName=" + User.Identity.Name;
                string res = Entities.CustomeWebRequest(eUrl, "GET");
                bool checkConfirmation = false;
                if (!string.IsNullOrEmpty(res.Trim()))
                {
                    checkConfirmation = Convert.ToBoolean(res);
                }

                if (!checkConfirmation)
                    return RedirectToAction("Login");
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

        [Authorize(Roles = "provider")]
        public ActionResult ContactDetails()
        {
            ViewBag.isFilterRequired = false;
            ViewBag.isMandatoryStep = false;
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToRoute("Login");
            }
            else
            {
                string bUrl = Request.Url.Scheme + "://" + Request.Url.Authority;
                var eUrl = bUrl + "/api/Provider/GetEmailVerificationStatus?UserName=" + User.Identity.Name;
                string res = Entities.CustomeWebRequest(eUrl, "GET");
                bool checkConfirmation = false;
                if (!string.IsNullOrEmpty(res.Trim()))
                {
                    checkConfirmation = Convert.ToBoolean(res);
                }

                if (!checkConfirmation)
                    return RedirectToAction("Login");
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

        [Authorize(Roles = "provider")]
        public ActionResult Preferences()
        {
            ViewBag.isMandatoryStep = false;

            if (Request.QueryString["from"] != null && Request.QueryString["from"].ToString() != "")
            {
                string from = Request.QueryString["from"];
                if (from == "role")
                {
                    Session["isProviderCreated"] = "false";
                    ViewBag.isMandatoryStep = true;
                }
            }

            ViewBag.isFilterRequired = false;
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToRoute("Login");
            }
            else
            {
                string bUrl = Request.Url.Scheme + "://" + Request.Url.Authority;
                var eUrl = bUrl + "/api/Provider/GetEmailVerificationStatus?UserName=" + User.Identity.Name;
                string res = Entities.CustomeWebRequest(eUrl, "GET");
                bool checkConfirmation = false;
                if (!string.IsNullOrEmpty(res.Trim()))
                {
                    checkConfirmation = Convert.ToBoolean(res);
                }

                if (!checkConfirmation)
                    return RedirectToAction("Login");
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

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(LoginModel model, [System.Web.Http.FromUri]string Token)
        {
            if (!ModelState.IsValid)
                return View(model);

            SystemFlagBAL objSystemFlagBAL = new SystemFlagBAL();

            var email = "";
            if (Session["Email"] != null)
            {
                email = Session["Email"].ToString();
            }

            var PatientSurveyId = Session["PatientSurveyId"];
            if (email == model.UserName)
            {
                if (PatientSurveyId != null && PatientSurveyId.ToString() != "")
                {
                    if (WebSecurity.Login(model.UserName, model.Password, persistCookie: model.RememberMe))
                    {
                        string URL = Request.Url.Scheme + "://" + Request.Url.Authority + "/Patient/MyEproms?patientsurveyid=" + PatientSurveyId + "&email=" + email;
                        Session["PatientSurveyId"] = "";
                        Session["Email"] = "";
                        Response.Redirect(URL);
                    }
                }
                else
                {
                    Session["isConfirmed"] = "";
                    if (ModelState.IsValid && WebSecurity.Login(model.UserName, model.Password, persistCookie: model.RememberMe))
                    {
                        ModelState.Clear();
                        Session["CurrentUser"] = model.UserName;
                        string[] UserInAdmin = Roles.FindUsersInRole("admin", model.UserName);
                        string[] UserInProvider = Roles.FindUsersInRole("provider", model.UserName);
                        string[] UserInPatient = Roles.FindUsersInRole("patient", model.UserName);

                        if (UserInAdmin.Length > 0)
                        {
                            Entities.SetCookie(model.UserName, "username");
                            Response.Redirect("/Default/Index");
                        }
                        else
                        {
                            string bUrl = Request.Url.Scheme + "://" + Request.Url.Authority;
                            if (!string.IsNullOrEmpty(Token))
                            {
                                var dToken = Utilities.DecodeFrom64(Token);
                                var nUrl = bUrl + "/api/Provider/ChangeEmailVerificationStatus?token=" + dToken;
                                Entities.CustomeWebRequest(nUrl);
                            }

                            var eUrl = bUrl + "/api/Provider/GetEmailVerificationStatus?UserName=" + model.UserName;
                            string res = Entities.CustomeWebRequest(eUrl, "GET");
                            bool checkConfirmation = false;
                            if (!string.IsNullOrEmpty(res.Trim()))
                            {
                                checkConfirmation = Convert.ToBoolean(res);
                            }

                            Entities.SetCookie(model.UserName, "username");

                            if (UserInPatient.Length > 0)
                            {
                                if (!checkConfirmation)
                                {
                                    Session["isConfirmed"] = false;
                                    return RedirectToRoute("Login");
                                }
                                else
                                {
                                    Session["isConfirmed"] = "";
                                    return RedirectToAction("PatientDashboard", "Patient", new { area = "" });
                                }
                            }

                            if (!checkConfirmation)
                            {
                                Session["isConfirmed"] = false;
                                Session["CurrentUser"] = null;
                                WebSecurity.Logout();
                                return RedirectToRoute("Login");
                            }
                            else
                            {
                                Session["isConfirmed"] = "";
                                
                                if (UserInProvider.Length > 0)
                                {
                                    Entities.SetCookie("T", "tmpData");
                                    return RedirectToAction("epromAllocation", "Patient", new { area = "" });
                                }
                                else if (UserInPatient.Length > 0)
                                {
                                    return RedirectToAction("PatientDashboard", "Patient", new { area = "" });
                                }
                            }
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "The user name or password provided is incorrect.");
                    }
                }
            }
            else
            {
                Session["isConfirmed"] = "";

                if (Session["CurrentUser"] != null && Session["CurrentUser"].ToString() != "" && Session["CurrentUser"].ToString() != model.UserName)
                {
                    ModelState.AddModelError("", "Your System is already login with other account");
                    return View();
                }

                if (ModelState.IsValid && WebSecurity.Login(model.UserName, model.Password, persistCookie: model.RememberMe))
                {
                    ModelState.Clear();

                    if (Session["CurrentUser"] == null || Session["CurrentUser"].ToString() == "")
                    {
                        Session["CurrentUser"] = model.UserName;
                    }

                    string[] UserInAdmin = Roles.FindUsersInRole("admin", model.UserName);
                    string[] UserInProvider = Roles.FindUsersInRole("provider", model.UserName);
                    string[] UserInPatient = Roles.FindUsersInRole("patient", model.UserName);
                    string[] UserInOrg = Roles.FindUsersInRole("organization", model.UserName);
                    string[] UserInPractice = Roles.FindUsersInRole("practice", model.UserName);

                    bool? ActiveDeActive = false;

                    if (UserInAdmin.Length > 0 || UserInProvider.Length > 0 || UserInPatient.Length > 0 || UserInOrg.Length > 0 || UserInPractice.Length > 0)
                    {
                        Patient_model PatientDetail = Patients.GetPatientDetailsByUserName(model.UserName);
                        if (PatientDetail != null)
                        {
                            ActiveDeActive = PatientDetail.IsActive;
                        }

                        if (UserInAdmin.Length > 0)
                        {
                            Entities.SetCookie(model.UserName, "username");
                            Response.Redirect("/Default/Index");
                        }
                        else
                        {
                            string bUrl = Request.Url.Scheme + "://" + Request.Url.Authority;
                            if (!string.IsNullOrEmpty(Token))
                            {
                                var dToken = Utilities.DecodeFrom64(Token);
                                var nUrl = bUrl + "/api/Provider/ChangeEmailVerificationStatus?token=" + dToken;
                                Entities.CustomeWebRequest(nUrl);
                            }

                            var eUrl = bUrl + "/api/Provider/GetEmailVerificationStatus?UserName=" + model.UserName;
                            string res = Entities.CustomeWebRequest(eUrl, "GET");
                            bool checkConfirmation = false;
                            if (!string.IsNullOrEmpty(res.Trim()))
                            {
                                checkConfirmation = Convert.ToBoolean(res);
                            }

                            Entities.SetCookie(model.UserName, "username");

                            if (UserInPatient.Length > 0)
                            {
                                if (ActiveDeActive == true)
                                {
                                    if (!checkConfirmation && Session["userid"] != null)
                                    {
                                        var nUrl = bUrl + "/api/Provider/ChangeEmailVerificationStatusByUserName?userName=" + model.UserName;
                                        Entities.CustomeWebRequest(nUrl);
                                        Session["isConfirmed"] = true;
                                        return RedirectToRoute("resetpassword");
                                    }
                                    else if (!checkConfirmation && Session["userid"] == null)
                                    {
                                        ModelState.AddModelError("", "Please Check your registered email to verify ePROMS account");
                                    }
                                    else
                                    {
                                        Session["isConfirmed"] = "";
                                        return RedirectToAction("PatientDashboard", "Patient", new { area = "" });
                                    }
                                }
                                else
                                {
                                    Session.RemoveAll();
                                    Session.Abandon();
                                    ModelState.AddModelError("", "Patient is not active. please contact to your provider");
                                }
                            }
                            else if (UserInPractice.Length > 0)
                            {
                                if (!checkConfirmation && Session["userid"] != null)
                                {
                                    var nUrl = bUrl + "/api/Provider/ChangeEmailVerificationStatusByUserName?userName=" + model.UserName;
                                    Entities.CustomeWebRequest(nUrl);
                                    Session["isConfirmed"] = true;
                                    return RedirectToRoute("resetpassword");
                                }
                                else if (!checkConfirmation && Session["userid"] == null)
                                {
                                    ModelState.AddModelError("", "Please Check your registered email to verify ePROMS account");
                                }
                                else
                                {
                                    Session["isConfirmed"] = "";
                                    return RedirectToAction("MyDetails", "Practice", new { area = "" });
                                }
                            }

                            if (!checkConfirmation)
                            {
                                Session["isConfirmed"] = false;
                                Session["CurrentUser"] = null;
                                WebSecurity.Logout();
                                return RedirectToRoute("Login");
                            }
                            else
                            {
                                Session["isConfirmed"] = "";
                                if (UserInProvider.Length > 0)
                                {
                                    if (Session["isProviderCreated"] != null && Session["isProviderCreated"].ToString() == "false")
                                    {
                                        return RedirectToAction("RegistrationDetails", "Provider", new { from = "role" });
                                    }
                                    else
                                    {
                                        Entities.SetCookie("T", "tmpData");
                                        return RedirectToAction("epromAllocation", "Patient", new { area = "" });
                                    }
                                }
                                else if (UserInOrg.Length > 0)
                                {
                                    return RedirectToAction("OrganizationDetail", "Organization", new { area = "" });
                                }
                            }
                        }
                    }
                    else
                    {
                        string bUrl = Request.Url.Scheme + "://" + Request.Url.Authority;
                        if (!string.IsNullOrEmpty(Token))
                        {
                            var dToken = Utilities.DecodeFrom64(Token);
                            var nUrl = bUrl + "/api/Provider/ChangeEmailVerificationStatus?token=" + dToken;
                            Entities.CustomeWebRequest(nUrl);
                        }

                        var eUrl = bUrl + "/api/Provider/GetEmailVerificationStatus?UserName=" + model.UserName;
                        string res = Entities.CustomeWebRequest(eUrl, "GET");
                        bool checkConfirmation = false;
                        if (!string.IsNullOrEmpty(res.Trim()))
                        {
                            checkConfirmation = Convert.ToBoolean(res);
                        }

                        if (checkConfirmation)
                        {
                            Session["isConfirmed"] = true;
                            return RedirectToRoute("Role");
                        }
                        else
                        {
                            Session["isConfirmed"] = false;
                            Session["CurrentUser"] = null;
                            WebSecurity.Logout();
                            return RedirectToRoute("Login");
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", "The user name or password provided is incorrect.");
                }
            }

            return View();
        }

        [HttpPost]
        public ActionResult LogOff()
        {
            bool IsAdmin = User.IsInRole("admin");
            bool IsProvider = User.IsInRole("provider");
            bool IsPatient = User.IsInRole("patient");

            WebSecurity.Logout();
            ModelState.Clear();

            Entities.SetCookie("", "username");
            Entities.SetCookie("", "isExistCINT");
            Entities.SetCookie("T", "tmpData");

            Session["Email"] = null;
            Session["PatientSurveyId"] = null;
            Session["isConfirmed"] = null;
            Session["CurrentUser"] = null;
            Session["isExistCINT"] = null;

            Session.Abandon();
            Session.Clear();
            ClearCache();
            clearchachelocalall();

            if (IsAdmin)
                return RedirectToRoute("admin/login");
            else
                return RedirectToRoute("Login");
        }

        public static void ClearCache()
        {
            System.Web.HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            System.Web.HttpContext.Current.Response.Cache.SetExpires(DateTime.Now);
            System.Web.HttpContext.Current.Response.Cache.SetNoServerCaching();
            System.Web.HttpContext.Current.Response.Cache.SetNoStore();
            System.Web.HttpContext.Current.Response.Cookies.Clear();
            System.Web.HttpContext.Current.Request.Cookies.Clear();
        }

        private void clearchachelocalall()
        {
            string GooglePath = Environment.GetEnvironmentVariable("USERPROFILE") + @"\AppData\Local\Google\Chrome\User Data\Default\";
            string MozilaPath = Environment.GetEnvironmentVariable("USERPROFILE") + @"\AppData\Roaming\Mozilla\Firefox\";
            string Opera1 = Environment.GetEnvironmentVariable("USERPROFILE") + @"\AppData\Local\Opera\Opera";
            string Opera2 = Environment.GetEnvironmentVariable("USERPROFILE") + @"\AppData\Roaming\Opera\Opera";
            string Safari1 = Environment.GetEnvironmentVariable("USERPROFILE") + @"\AppData\Local\Apple Computer\Safari";
            string Safari2 = Environment.GetEnvironmentVariable("USERPROFILE") + @"\AppData\Roaming\Apple Computer\Safari";
            string IE1 = Environment.GetEnvironmentVariable("USERPROFILE") + @"\AppData\Local\Microsoft\Intern~1";
            string IE2 = Environment.GetEnvironmentVariable("USERPROFILE") + @"\AppData\Local\Microsoft\Windows\History";
            string IE3 = Environment.GetEnvironmentVariable("USERPROFILE") + @"\AppData\Local\Microsoft\Windows\Tempor~1";
            string IE4 = Environment.GetEnvironmentVariable("USERPROFILE") + @"\AppData\Roaming\Microsoft\Windows\Cookies";
            string Flash = Environment.GetEnvironmentVariable("USERPROFILE") + @"\AppData\Roaming\Macromedia\Flashp~1";

            ClearAllSettings(new string[] { GooglePath, MozilaPath, Opera1, Opera2, Safari1, Safari2, IE1, IE2, IE3, IE4, Flash });
        }

        public void ClearAllSettings(string[] ClearPath)
        {
            foreach (string HistoryPath in ClearPath)
            {
                if (Directory.Exists(HistoryPath))
                {
                    DoDelete(new DirectoryInfo(HistoryPath));
                }
            }
        }

        void DoDelete(DirectoryInfo folder)
        {
            try
            {
                foreach (FileInfo file in folder.GetFiles())
                {
                    try
                    {
                        file.Delete();
                    }
                    catch { }
                }
                foreach (DirectoryInfo subfolder in folder.GetDirectories())
                {
                    DoDelete(subfolder);
                }
            }
            catch { }
        }

        [HttpPost]
        public ActionResult Register(ProviderRegisterModel model)
        {
            model.UserId = RegisterUser(model);
            return Json(model);
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }

        public IEnumerable<SecretQuestionModel> GetSecurityQuestion()
        {
            EPROMDBEntities db = new EPROMDBEntities();
            IEnumerable<SecretQuestionModel> questionList = db.SecretQuestions.Select(x => new SecretQuestionModel
            {
                ID = x.ID,
                Questions = x.Questions,
                Description = x.Description,
                IsActive = x.IsActive
            }).ToList();

            return questionList;
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult ResetPassword(string Token, ResetPassword model)
        {
            try
            {
                ModelState.Clear();
                model.Token = Token;
            }
            catch (Exception)
            {
            }
            return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult ChangePassword(string Email, string NewPassword)
        {
            ResetPassword model = new ResetPassword();
            try
            {
                ViewBag.isMandatoryStep = false;
                if (!string.IsNullOrEmpty(Email))
                {

                    MembershipUser u = Membership.GetUser(Email, false);
                    if (u != null)
                    {
                        string token = WebSecurity.GeneratePasswordResetToken(Email);
                        bool result = WebSecurity.ResetPassword(token, NewPassword);
                        if (result)
                        {
                            return RedirectToRoute("Login");
                        }
                        else
                        {
                            ModelState.AddModelError("", "There is some issue. Please try again");
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult ResetPassword(ResetPassword model)
        {
            try
            {
                ViewBag.isMandatoryStep = false;
                string Email = Utilities.Decrypt(model.Token);
                if (!string.IsNullOrEmpty(Email))
                {

                    MembershipUser u = Membership.GetUser(Email, false);
                    if (u != null)
                    {
                        string token = WebSecurity.GeneratePasswordResetToken(Email);
                        bool result = WebSecurity.ResetPassword(token, model.NewPassword);
                        if (result)
                        {
                            WebSecurity.Logout();
                            ModelState.Clear();
                            return RedirectToRoute("Login");
                        }
                        else
                        {
                            ModelState.AddModelError("", "There is some issue. Please try again");
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            return View(model);
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult ValidateToken(string Token)
        {
            try
            {
                ViewBag.isMandatoryStep = false;
                string URL = Request.Url.Scheme + "://" + Request.Url.Authority + "/ValidateToken";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Request.Url.Scheme + "://" + Request.Url.Authority + "/api/Email/ValidateToken?Token=" + Token + "&isRegister=false");
                request.Method = "GET";
                request.ContentType = "application/json";
                WebResponse response = request.GetResponse();
                string result = new StreamReader(response.GetResponseStream()).ReadToEnd();
                result = new JavaScriptSerializer().Deserialize<string>(result);
                if (!string.IsNullOrEmpty(result))
                {
                    string[] objArray = result.Split(new string[] { "~!@#" }, StringSplitOptions.None);
                    if (objArray.Length > 0)
                    {
                        if (objArray[0] == "1")
                        {
                            string Email = objArray[1];
                            Email = Utilities.Encrypt(Email);
                            return Redirect("/Provider/ResetPassword?Token=" + Email);
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            return View();
        }

        public Guid? RegisterUser(ProviderRegisterModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (!WebSecurity.UserExists(model.Email))
                    {
                        var db = new EPROMDBEntities();

                        int? userid = db.UserProfiles.Where(x => x.UserName == model.Email).Select(x => x.UserId).FirstOrDefault();
                        if (userid == null)
                        {
                            return null;
                        }

                        string returnUser = WebSecurity.CreateUserAndAccount(model.Email, model.Password);
                        if (model.Role != null && model.Role != "")
                        {
                            Roles.AddUserToRole(model.Email, model.Role);
                        }

                        try
                        {
                            UserDetail objUserdetail = new UserDetail();
                            objUserdetail.ID = Guid.NewGuid();
                            objUserdetail.Email = model.Email;
                            objUserdetail.UserName = model.Email;
                            db.UserDetails.Add(objUserdetail);
                            db.SaveChanges();

                            model.UserId = objUserdetail.ID;

                            if (model.SecretQuestionID > 0)
                            {
                                UserSecretQuestion objUserSecretQuestion = new UserSecretQuestion();
                                objUserSecretQuestion.ID = Guid.NewGuid();
                                objUserSecretQuestion.UserID = objUserdetail.ID;
                                objUserSecretQuestion.SecretQuestionID = model.SecretQuestionID;
                                objUserSecretQuestion.Answer = model.Answer;
                                db.UserSecretQuestions.Add(objUserSecretQuestion);
                                db.SaveChanges();
                            }
                        }
                        catch (Exception)
                        {

                        }

                        if (model.Role == null || model.Role == "")
                            Entities.SetCookie(model.Email, "username");
                    }
                    else
                    {
                        ModelState.AddModelError("", "User name already exists. Please enter a different user name.");
                    }
                }
                catch (MembershipCreateUserException e)
                {
                    ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
                }
            }

            return model.UserId;
        }

        [HttpPost]
        public string CreateRole(UserRole model)
        {
            try
            {
                var db = new EPROMDBEntities();
                var role = Roles.GetRolesForUser(model.UserName);
                if (role.Length == 0)
                {
                    Roles.AddUserToRole(model.UserName, model.RoleName);
                    return "1";
                }
                else
                    return "0";
            }
            catch (MembershipCreateUserException e)
            {
                ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
            }
            return "";
        }

        public ActionResult ProviderOrganization()
        {
            Session["isProviderCreated"] = null;
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

        public ActionResult SearchPatient()
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