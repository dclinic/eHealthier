using BLL;
using ePRom.Filters;
using ePRom.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Linq;
using DAL;
using System.Web.Security;
using WebMatrix.WebData;

namespace ePRom.Controllers
{
    [InitializeSimpleMembership]
    public class PatientController : Controller
    {
        public ActionResult InvalidDate()
        {
            ViewBag.StartDate = "";
            ViewBag.EndDate = "";

            string StartDate = Request.QueryString["StartDate"];
            string EndDate = Request.QueryString["EndDate"];

            if (!string.IsNullOrEmpty(StartDate) && !string.IsNullOrEmpty(EndDate))
            {
                ViewBag.StartDate = StartDate;
                ViewBag.EndDate = EndDate;
            }

            ViewBag.isFilterRequired = false;
            ViewBag.isMandatoryStep = false;
            return View();
        }

        public ActionResult ResponseSubmitted()
        {
            ViewBag.isFilterRequired = false;
            ViewBag.isMandatoryStep = false;
            return View();
        }


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
                return View();
            }
        }


        [Authorize(Roles = "patient")]
        public ActionResult MyEproms()
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

        [Authorize(Roles = "patient")]
        public ActionResult CompleteEprom()
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

        [Authorize(Roles = "provider")]
        public ActionResult MyDetails()
        {
            ViewBag.isFilterRequired = false;
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

        [Authorize(Roles = "patient")]
        public ActionResult patientEprom()
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

        [Authorize(Roles = "patient")]
        public ActionResult Dashboard()
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

        [Authorize(Roles = "patient")]
        public ActionResult PatientDashboard()
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
                    return RedirectToAction("RegisterCompleted", "Provider");
                else
                    return View();
            }
        }

        [Authorize(Roles = "patient")]
        public ActionResult patientdetails()
        {
            ViewBag.isMandatoryStep = false;
            return View();
        }

        [Authorize(Roles = "provider")]
        public ActionResult epromAllocation()
        {
            ViewBag.isMandatoryStep = false;
            return View();
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

        [HttpPost]
        public JsonResult UploadCsvFileAndGetData()
        {
            try
            {
                HttpFileCollection collection = System.Web.HttpContext.Current.Request.Files;
                var Params = System.Web.HttpContext.Current.Request.Params;
                if (Params != null)
                {
                    HttpPostedFile file = collection["file1"];
                    var ProviderId = Params["ProviderId"];
                    var OrganizationId = Params["OrganizationId"];
                    var PracticeId = Params["PracticeId"];

                    if (file != null)
                    {
                        return Json(HelperMethods.ReadCSVFile(file.InputStream, ProviderId, OrganizationId, PracticeId));
                    }
                    else
                    {
                        throw new FileNotFoundException("file not found.");
                    }
                }
                else
                {
                    return Json("");
                }
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return Json("");
            }
        }

        [HttpPost]
        public JsonResult UploadCsvFileAndInsertData(string userName, string ProviderId, string OrganizationId, string PracticeId)
        {
            string returnString = string.Empty;
            try
            {
                bool isRedytoInsert = true;
                bool IsPatientAssociate = false;
                Guid? userId = Providers.GetUserIdByUserName(userName);
                int TotalAddedPatient = 0;
                int TotalNotAddedPatient = 0;
                int TotalCsvRecords = 0;

                HttpFileCollection collection = System.Web.HttpContext.Current.Request.Files;

                HttpPostedFile file = collection["file"];
                if (file != null)
                {
                    var Params = System.Web.HttpContext.Current.Request.Params;
                    if (Params != null)
                    {
                        Guid GProviderId = new Guid(ProviderId);
                        Guid GOrganizationId = new Guid(OrganizationId);
                        Guid GPracticeId = new Guid(PracticeId);

                        var csvData = HelperMethods.ReadCSVFile(file.InputStream, ProviderId, OrganizationId, PracticeId);
                        if (csvData.Count > 0)
                        {
                            List<Patient_model> patientList = new List<Patient_model>();
                            List<State_model> objState = Addresses.GetStates();
                            List<Country_model> objCountry = Addresses.GetCountries();

                            TotalCsvRecords = csvData.Count;
                            foreach (var csvRecord in csvData)
                            {
                                int? StateID = null;
                                short? CountryID = null;

                                if (objState != null)
                                {
                                    State_model SelectedState = objState.Where(x => x.StateName.ToLower() == csvRecord.Address.State.Trim().ToLower()).FirstOrDefault();
                                    if (SelectedState != null)
                                    {
                                        StateID = SelectedState.ID;
                                    }
                                }

                                if (objCountry != null)
                                {
                                    Country_model SelectedCountry = objCountry.Where(x => x.CountryName.ToLower() == csvRecord.Address.Country.Trim().ToLower()).FirstOrDefault();
                                    if (SelectedCountry != null)
                                    {
                                        CountryID = SelectedCountry.ID;
                                    }
                                }

                                ProviderRegisterModel objProviderRegisterModel = new ProviderRegisterModel();
                                objProviderRegisterModel.SecretQuestionID = 0;
                                objProviderRegisterModel.Email = csvRecord.User.Email;
                                objProviderRegisterModel.Password = "123456";
                                objProviderRegisterModel.ConfirmPassword = "123456";
                                objProviderRegisterModel.isPatient = true;
                                objProviderRegisterModel.Role = "patient";

                                var provider = new ProviderController();

                                if (csvRecord.IsPatientExist == false)
                                {
                                    var result = WebSecurity.UserExists(objProviderRegisterModel.Email);
                                    if (result == false)
                                    {
                                        isRedytoInsert = true;
                                    }
                                    else
                                    {
                                        isRedytoInsert = false;
                                    }
                                }
                                else
                                {
                                    isRedytoInsert = false;
                                }


                                if (isRedytoInsert)
                                {
                                    Guid? UserId = provider.RegisterUser(objProviderRegisterModel);

                                    if (UserId != null && UserId != Guid.Empty)
                                    {
                                        Patient_model objPatientmodel = new Patient_model();

                                        #region Address

                                        Address_model Address = new Address_model();
                                        Address.Line1 = csvRecord.Address.Line1;
                                        Address.Line2 = csvRecord.Address.Line2;
                                        Address.Suburb = csvRecord.Address.Suburb;
                                        Address.StateID = StateID;
                                        Address.CountryID = CountryID;
                                        Address.ZipCode = csvRecord.Address.ZipCode;
                                        Address.IsActive = true;
                                        Address.CreatedDate = DateTime.Now;
                                        Address.ModifiedDate = DateTime.Now;
                                        Address.CreatedBy = userId;
                                        Address.ModifiedBy = userId;

                                        #endregion

                                        #region Contact

                                        Contact_model Contact = new Contact_model();
                                        Contact.Mobile = csvRecord.Contact.Phone;
                                        Contact.EmailPersonal = csvRecord.User.Email;
                                        //Contact.Mobile2 = csvRecord.MedicareNumber;
                                        Contact.IsActive = true;
                                        Contact.CreatedDate = DateTime.Now;
                                        Contact.ModifiedDate = DateTime.Now;
                                        Contact.CreatedBy = userId;
                                        Contact.ModifiedBy = userId;

                                        #endregion

                                        #region User

                                        DateTime? DOB = Convert.ToDateTime(csvRecord.User.DOB);

                                        User_model user = new User_model();
                                        user.ID = (Guid)UserId;
                                        user.FirstName = csvRecord.User.FirstName;
                                        user.LastName = csvRecord.User.LastName;
                                        user.MiddleName = csvRecord.User.MiddleName;
                                        user.UserName = csvRecord.User.Email;
                                        user.Email = csvRecord.User.Email;

                                        user.DOB = Convert.ToDateTime(Convert.ToDateTime(DOB).ToString("MM/dd/yyyy"));
                                        user.Gender = csvRecord.User.Gender;

                                        #endregion

                                        objPatientmodel.PatientUserId = (Guid)UserId;
                                        objPatientmodel.Address = Address;
                                        objPatientmodel.Contact = Contact;
                                        objPatientmodel.User = user;
                                        objPatientmodel.IHINumber = csvRecord.IHINumber;
                                        objPatientmodel.MedicareNumber = csvRecord.MedicareNumber;
                                        objPatientmodel.OrganizationID = GOrganizationId;
                                        objPatientmodel.PracticeID = GPracticeId;
                                        objPatientmodel.Salutation = csvRecord.Salutation;
                                        patientList.Add(objPatientmodel);
                                        TotalAddedPatient++;
                                    }
                                }
                                else
                                {
                                    var patient = Patients.SearchPatientDetailByUniqueNumber(csvRecord.IHINumber, csvRecord.MedicareNumber);

                                    if (patient != null)
                                    {
                                        IsPatientAssociate = true;
                                        Patients.CsvPatientProviderAssociate(GProviderId, patient.ID, GOrganizationId, GPracticeId, patient.User.Email);
                                    }
                                }
                            }
                            if (TotalAddedPatient > 0)
                            {
                                return Json(new
                                {
                                    patientList = patientList,
                                    status = 1,
                                    TotalCsvRecords = TotalCsvRecords,
                                    TotalAddedPatient = TotalAddedPatient,
                                    TotalNotAddedPatient = TotalNotAddedPatient
                                });
                            }
                            else if (TotalAddedPatient == 0)
                            {
                                return Json(new
                                {
                                    isPatientAssociate = IsPatientAssociate,
                                    patientList = patientList,
                                    status = 2,
                                    TotalCsvRecords = TotalCsvRecords,
                                    TotalAddedPatient = TotalAddedPatient,
                                    TotalNotAddedPatient = TotalNotAddedPatient
                                });
                            }
                        }
                        else
                        {
                            return Json(new
                            {
                                status = 2,
                            });
                        }
                    }
                    else
                    {
                        return Json(new
                        {
                            status = 2,
                        });
                    }
                }
                else
                {
                    throw new FileNotFoundException("file not found.");
                }
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
            }
            return Json(new
            {
                status = 3
            });
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult ResetPassword(PatientResetPassword model)
        {
            try
            {
                if (Session["userid"] != null)
                {
                    EPROMDBEntities db = new EPROMDBEntities();
                    model.Email = User.Identity.Name;

                    ViewBag.isMandatoryStep = false;
                    string Email = model.Email;
                    if (!string.IsNullOrEmpty(Email))
                    {
                        MembershipUser u = Membership.GetUser(Email, false);
                        if (u != null)
                        {
                            string token = WebSecurity.GeneratePasswordResetToken(Email);
                            bool result = WebSecurity.ResetPassword(token, model.NewPassword);
                            if (result)
                            {
                                Guid UserID = Guid.Empty;

                                string userId = Session["userid"].ToString();
                                if (userId != "")
                                {
                                    UserID = new Guid(userId);
                                }

                                UserSecretQuestion objUserSecretQuestion = new UserSecretQuestion();
                                objUserSecretQuestion.ID = Guid.NewGuid();
                                objUserSecretQuestion.UserID = UserID;
                                objUserSecretQuestion.SecretQuestionID = model.SecretQuestionId;
                                objUserSecretQuestion.Answer = model.Answer;
                                db.UserSecretQuestions.Add(objUserSecretQuestion);
                                db.SaveChanges();
                                Session["userid"] = null;
                                return RedirectToRoute("Login");
                            }
                            else
                            {
                                model.SecretQuestionList = ViewBag.SecretQuestionList;
                                ModelState.AddModelError("", "There is some issue. Please try again");
                            }
                        }
                    }
                }
                model.SecretQuestionList = ViewBag.SecretQuestionList;
            }
            catch (Exception)
            {
            }
            return View(model);
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult ResetPassword(string Token, PatientResetPassword model)
        {
            try
            {
                ModelState.Clear();
                model.SecretQuestionList = GetSecurityQuestion();
                ViewBag.SecretQuestionList = model.SecretQuestionList;
                foreach (var item in model.SecretQuestionList)
                {
                    model.SecretQuestionId = (short)item.ID;
                    return View(model);
                }
            }
            catch (Exception)
            {
            }
            return View(model);
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

    }
}