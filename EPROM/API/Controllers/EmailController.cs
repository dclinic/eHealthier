using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BAL;
using DAL;
using Common;
using System.IO;
using System.Web.Security;
using System.Text.RegularExpressions;
using System.Web;
using BLL;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Drawing;

namespace API.Controllers
{
    public class EmailController : ApiController
    {
        public class Score
        {
            public string Title { get; set; }
            public double Value { get; set; }
        }

        public class ScoreList
        {
            public List<Score> List { get; set; }
        }


        SystemFlagBAL objSystemFlagBAL = null;
        Utilities objUtility = null;

        [HttpGet]
        public string ForgotPasswordEmail(string ToAddress, string URL)
        {
            string _Html = "";
            try
            {
                objUtility = new Utilities();
                objSystemFlagBAL = new SystemFlagBAL();
                string Subject = "Forgot Password";
                string MailBody = string.Empty;
                string Token1 = "";
                string EncryptToken = "";
                string CheckAlreadyRequested = objSystemFlagBAL.IsAlreadyReqForgetPwd(ToAddress, false);
                if (string.IsNullOrEmpty(CheckAlreadyRequested))
                {
                    Token1 = Guid.NewGuid().ToString();
                    EncryptToken = Utilities.EncodeTo64(Token1);
                    Token1 = objSystemFlagBAL.GenerateToken(ToAddress, Token1, false);
                }
                else
                {
                    Token1 = CheckAlreadyRequested;
                    EncryptToken = Utilities.EncodeTo64(Token1);
                }

                if (!string.IsNullOrEmpty(Token1) && !string.IsNullOrEmpty(EncryptToken))
                {
                    StreamReader reader = null;
                    try
                    {
                        reader = new StreamReader(HttpContext.Current.Server.MapPath("/Content/Html/ForgotPassword.html"));
                        MailBody = reader.ReadToEnd();
                        string atag = "<a href='" + URL + "?Token=" + EncryptToken + "'>Click Here</a>";
                        MailBody = MailBody.Replace("{{RESET_PASSWORD_LINK}}", atag);
                    }
                    finally
                    {
                        if (reader != null)
                            reader.Dispose();
                    }

                    string CID = "";
                    MailBody = objUtility.GetImagesInHTMLString(MailBody, ref CID);

                    string SenderEmailAddress = "";
                    string SenderPassword = "";
                    string SenderHostName = "";
                    string SenderPort = "";
                    string EnableSSL = "";

                    List<SystemFlag> objSystemFlagList = objSystemFlagBAL.GetAllSystemFlags();
                    if (objSystemFlagList != null && objSystemFlagList.Count > 0)
                    {
                        SystemFlag objSystemFlag = null;
                        objSystemFlag = objSystemFlagList.Where(x => x.SystemFlagName.Trim().ToLower() == "SenderEmailAddress".ToLower()).FirstOrDefault();
                        if (objSystemFlag != null)
                        {
                            if (!string.IsNullOrEmpty(objSystemFlag.Value))
                            {
                                SenderEmailAddress = objSystemFlag.Value;
                                objSystemFlag = null;
                            }
                            else if (!string.IsNullOrEmpty(objSystemFlag.DefaultValue))
                            {
                                SenderEmailAddress = objSystemFlag.DefaultValue;
                                objSystemFlag = null;
                            }
                        }

                        objSystemFlag = objSystemFlagList.Where(x => x.SystemFlagName.Trim().ToLower() == "SenderPassword".ToLower()).FirstOrDefault();
                        if (objSystemFlag != null)
                        {
                            if (!string.IsNullOrEmpty(objSystemFlag.Value))
                            {
                                SenderPassword = objSystemFlag.Value;
                                objSystemFlag = null;
                            }
                            else if (!string.IsNullOrEmpty(objSystemFlag.DefaultValue))
                            {
                                SenderPassword = objSystemFlag.DefaultValue;
                                objSystemFlag = null;
                            }
                        }


                        objSystemFlag = objSystemFlagList.Where(x => x.SystemFlagName.Trim().ToLower() == "SenderHostName".ToLower()).FirstOrDefault();
                        if (objSystemFlag != null)
                        {
                            if (!string.IsNullOrEmpty(objSystemFlag.Value))
                            {
                                SenderHostName = objSystemFlag.Value;
                                objSystemFlag = null;
                            }
                            else if (!string.IsNullOrEmpty(objSystemFlag.DefaultValue))
                            {
                                SenderHostName = objSystemFlag.DefaultValue;
                                objSystemFlag = null;
                            }
                        }

                        objSystemFlag = objSystemFlagList.Where(x => x.SystemFlagName.Trim().ToLower() == "SenderPort".ToLower()).FirstOrDefault();
                        if (objSystemFlag != null)
                        {
                            if (!string.IsNullOrEmpty(objSystemFlag.Value))
                            {
                                SenderPort = objSystemFlag.Value;
                                objSystemFlag = null;
                            }
                            else if (!string.IsNullOrEmpty(objSystemFlag.DefaultValue))
                            {
                                SenderPort = objSystemFlag.DefaultValue;
                                objSystemFlag = null;
                            }
                        }


                        objSystemFlag = objSystemFlagList.Where(x => x.SystemFlagName.Trim().ToLower() == "EnableSSL".ToLower()).FirstOrDefault();
                        if (objSystemFlag != null)
                        {
                            if (!string.IsNullOrEmpty(objSystemFlag.Value))
                            {
                                EnableSSL = objSystemFlag.Value;
                                objSystemFlag = null;
                            }
                            else if (!string.IsNullOrEmpty(objSystemFlag.DefaultValue))
                            {
                                EnableSSL = objSystemFlag.DefaultValue;
                                objSystemFlag = null;
                            }
                        }

                        if (!(string.IsNullOrEmpty(SenderEmailAddress) && string.IsNullOrEmpty(SenderPassword) && string.IsNullOrEmpty(SenderHostName) && string.IsNullOrEmpty(SenderPort) && string.IsNullOrEmpty(EnableSSL)))
                        {
                            Email EC = new Email(SenderEmailAddress, SenderPassword, SenderHostName, Convert.ToInt32(SenderPort), Convert.ToBoolean(EnableSSL), "");
                            bool result = EC.SendEmail("ePROMs", Subject, MailBody, CID, ToAddress, System.Net.Mail.MailPriority.Normal);
                            if (result)
                            {
                                _Html = "1";
                            }
                        }

                    }
                }
            }
            catch (Exception)
            {
                _Html = "";
            }

            return _Html;
        }

        [HttpGet]
        public string ValidateToken(string Token, bool isRegister)
        {
            string _Html = string.Empty;
            try
            {
                Token = Utilities.DecodeFrom64(Token);
                SystemFlagBAL objSystemFlagBAL = new SystemFlagBAL();
                string UserName = objSystemFlagBAL.ValidateToken(Token);
                if (!string.IsNullOrEmpty(UserName))
                {
                    UserName = objSystemFlagBAL.DeleteToken(UserName, Token, isRegister);
                    _Html = "1" + "~!@#" + UserName;
                }
            }
            catch (Exception)
            {
                _Html = string.Empty;
            }
            return _Html;
        }

        [HttpPost]
        public string SendVerificationMail(string ToAddress)
        {
            string returnValue = "";
            try
            {
                objUtility = new Utilities();
                objSystemFlagBAL = new SystemFlagBAL();
                string subject = "ePROMs Verification";
                string mailBody = "<div><br></div> <div>Welcome to your Behavioral Intervention Care App, ePROMs.</div>&nbsp;&nbsp;<div><br></div><div>To access your account, you must first verify your email. Click on the link below to verify your email address.&nbsp;&nbsp;</div><div>Use the link : <a href={{VERIFICATION_LINK}}> click to verify </a><br/><br></div><div><br></div><div><br></div><div style=\"margin-top:35px\">Thank you for choosing us!</div><div><img src=\"/Resources/Images/ePROMS-new-logo.png\" style=\"max-width: 200px; margin-top:20px;\" /></div><div><br></div>";

                string CID = "";
                mailBody = objUtility.GetImagesInHTMLString(mailBody, ref CID);

                string SenderEmailAddress = "";
                string SenderPassword = "";
                string SenderHostName = "";
                string SenderPort = "";
                string EnableSSL = "";

                string Token1 = "";
                string EncryptToken = "";
                string CheckAlreadyRequested = objSystemFlagBAL.IsAlreadyReqForgetPwd(ToAddress, true);
                if (string.IsNullOrEmpty(CheckAlreadyRequested))
                {
                    Token1 = Guid.NewGuid().ToString();
                    EncryptToken = Utilities.EncodeTo64(Token1);
                    Token1 = objSystemFlagBAL.GenerateToken(ToAddress, Token1, true);
                }
                else
                {
                    return "1";
                }


                string URL = Request.RequestUri.Scheme + "://" + Request.RequestUri.Authority + "/Login";

                string nURl = "" + URL + "?Token=" + EncryptToken + "";
                mailBody = mailBody.Replace("{{VERIFICATION_LINK}}", nURl);

                if (string.IsNullOrEmpty(Token1) && string.IsNullOrEmpty(EncryptToken))
                {
                    return returnValue;
                }

                List<SystemFlag> objSystemFlagList = objSystemFlagBAL.GetAllSystemFlags();
                if (objSystemFlagList != null && objSystemFlagList.Count > 0)
                {
                    SystemFlag objSystemFlag = null;
                    objSystemFlag = objSystemFlagList.Where(x => x.SystemFlagName.Trim().ToLower() == "SenderEmailAddress".ToLower()).FirstOrDefault();
                    if (objSystemFlag != null)
                    {
                        if (!string.IsNullOrEmpty(objSystemFlag.Value))
                        {
                            SenderEmailAddress = objSystemFlag.Value;
                            objSystemFlag = null;
                        }
                        else if (!string.IsNullOrEmpty(objSystemFlag.DefaultValue))
                        {
                            SenderEmailAddress = objSystemFlag.DefaultValue;
                            objSystemFlag = null;
                        }
                    }

                    objSystemFlag = objSystemFlagList.Where(x => x.SystemFlagName.Trim().ToLower() == "SenderPassword".ToLower()).FirstOrDefault();
                    if (objSystemFlag != null)
                    {
                        if (!string.IsNullOrEmpty(objSystemFlag.Value))
                        {
                            SenderPassword = objSystemFlag.Value;
                            objSystemFlag = null;
                        }
                        else if (!string.IsNullOrEmpty(objSystemFlag.DefaultValue))
                        {
                            SenderPassword = objSystemFlag.DefaultValue;
                            objSystemFlag = null;
                        }
                    }


                    objSystemFlag = objSystemFlagList.Where(x => x.SystemFlagName.Trim().ToLower() == "SenderHostName".ToLower()).FirstOrDefault();
                    if (objSystemFlag != null)
                    {
                        if (!string.IsNullOrEmpty(objSystemFlag.Value))
                        {
                            SenderHostName = objSystemFlag.Value;
                            objSystemFlag = null;
                        }
                        else if (!string.IsNullOrEmpty(objSystemFlag.DefaultValue))
                        {
                            SenderHostName = objSystemFlag.DefaultValue;
                            objSystemFlag = null;
                        }
                    }

                    objSystemFlag = objSystemFlagList.Where(x => x.SystemFlagName.Trim().ToLower() == "SenderPort".ToLower()).FirstOrDefault();
                    if (objSystemFlag != null)
                    {
                        if (!string.IsNullOrEmpty(objSystemFlag.Value))
                        {
                            SenderPort = objSystemFlag.Value;
                            objSystemFlag = null;
                        }
                        else if (!string.IsNullOrEmpty(objSystemFlag.DefaultValue))
                        {
                            SenderPort = objSystemFlag.DefaultValue;
                            objSystemFlag = null;
                        }
                    }


                    objSystemFlag = objSystemFlagList.Where(x => x.SystemFlagName.Trim().ToLower() == "EnableSSL".ToLower()).FirstOrDefault();
                    if (objSystemFlag != null)
                    {
                        if (!string.IsNullOrEmpty(objSystemFlag.Value))
                        {
                            EnableSSL = objSystemFlag.Value;
                            objSystemFlag = null;
                        }
                        else if (!string.IsNullOrEmpty(objSystemFlag.DefaultValue))
                        {
                            EnableSSL = objSystemFlag.DefaultValue;
                            objSystemFlag = null;
                        }
                    }

                    if (!(string.IsNullOrEmpty(SenderEmailAddress) && string.IsNullOrEmpty(SenderPassword) && string.IsNullOrEmpty(SenderHostName) && string.IsNullOrEmpty(SenderPort) && string.IsNullOrEmpty(EnableSSL)))
                    {
                        Email EC = new Email(SenderEmailAddress, SenderPassword, SenderHostName, Convert.ToInt32(SenderPort), Convert.ToBoolean(EnableSSL), "");
                        bool result = EC.SendEmail("ePROMs", subject, mailBody, CID, ToAddress, System.Net.Mail.MailPriority.Normal, null);
                        if (result)
                        {
                            returnValue = "1";
                        }
                    }

                }
            }
            catch (Exception)
            {

            }

            return returnValue;
        }

        [HttpPost]
        public string SendPatientRegistrationMail(Guid PatientId, Guid ProviderId, Guid OrganizationId, Guid PracticeId, string UserName, string Password, Guid UserId)
        {
            string returnValue = "";
            try
            {
                string MailBody = string.Empty;
                Utilities objUtility = new Utilities();
                SystemFlagBAL objSystemFlagBAL = new SystemFlagBAL();

                Guid? userid = Providers.GetUserIdByUserName(UserName);
                if (userid != Guid.Empty)
                {
                    var PatientUser = HelperMethods.GetEntities().GetUserDetailsByUserId(userid);
                    var objDetails = Patients.GetProviderDetailByPatient_Org_PracticeId(PatientId, ProviderId, OrganizationId, PracticeId);

                    string providerEmailID = HelperMethods.GetEntities().getProviderEmailID(ProviderId);

                    string URL = Request.RequestUri.Scheme + "://" + Request.RequestUri.Authority + "/Login?userid=" + UserId;

                    string subject = "ePROMs Registration Detail";

                    StreamReader reader = null;
                    try
                    {
                        reader = new StreamReader(HttpContext.Current.Server.MapPath("/Content/Html/PatientRegistration.html"));
                        MailBody = reader.ReadToEnd();
                        MailBody = MailBody.Replace("{{PatientName}}", PatientUser.FirstName + " " + PatientUser.LastName)
                                     .Replace("{{DoctorName}}", "Dr." + objDetails.ProviderName)
                                     .Replace("{{OrganizationName}}", "'" + objDetails.OrganizationName + "'")
                                     .Replace("{{PracticeName}}", "'" + objDetails.PracticeName + "'")
                                     .Replace("{{address1}}", objDetails.Address == null ? "" : objDetails.Address.Line1)
                                     .Replace("{{address2}}", objDetails.Address == null ? "" : objDetails.Address.Line2)
                                     .Replace("{{ZipCode}}", objDetails.Address == null ? "" : objDetails.Address.ZipCode)
                                     .Replace("{{TelephoneNo}}", objDetails.Contact == null ? "" : objDetails.Contact.Mobile)
                                     .Replace("{{Fax}}", objDetails.Contact == null ? "" : objDetails.Contact.FAX)
                                     .Replace("{{Email}}", providerEmailID)
                                     .Replace("{{LoginLink}}", URL)
                                     .Replace("{{UserName}}", UserName)
                                     .Replace("{{Password}}", Password);
                    }
                    finally
                    {
                        if (reader != null)
                            reader.Dispose();
                    }

                    string CID = "";
                    MailBody = objUtility.GetImagesInHTMLString(MailBody, ref CID);

                    string SenderEmailAddress = "";
                    string SenderPassword = "";
                    string SenderHostName = "";
                    string SenderPort = "";
                    string EnableSSL = "";

                    string Token1 = "";
                    string EncryptToken = "";
                    string CheckAlreadyRequested = objSystemFlagBAL.IsAlreadyReqForgetPwd(UserName, true);
                    if (string.IsNullOrEmpty(CheckAlreadyRequested))
                    {
                        Token1 = Guid.NewGuid().ToString();
                        EncryptToken = Utilities.EncodeTo64(Token1);
                        Token1 = objSystemFlagBAL.GenerateToken(UserName, Token1, true);
                    }
                    else
                    {
                        return "1";
                    }

                    if (string.IsNullOrEmpty(Token1) && string.IsNullOrEmpty(EncryptToken))
                    {
                        return returnValue;
                    }

                    List<SystemFlag> objSystemFlagList = objSystemFlagBAL.GetAllSystemFlags();
                    if (objSystemFlagList != null && objSystemFlagList.Count > 0)
                    {
                        SystemFlag objSystemFlag = null;
                        objSystemFlag = objSystemFlagList.Where(x => x.SystemFlagName.Trim().ToLower() == "SenderEmailAddress".ToLower()).FirstOrDefault();
                        if (objSystemFlag != null)
                        {
                            if (!string.IsNullOrEmpty(objSystemFlag.Value))
                            {
                                SenderEmailAddress = objSystemFlag.Value;
                                objSystemFlag = null;
                            }
                            else if (!string.IsNullOrEmpty(objSystemFlag.DefaultValue))
                            {
                                SenderEmailAddress = objSystemFlag.DefaultValue;
                                objSystemFlag = null;
                            }
                        }

                        objSystemFlag = objSystemFlagList.Where(x => x.SystemFlagName.Trim().ToLower() == "SenderPassword".ToLower()).FirstOrDefault();
                        if (objSystemFlag != null)
                        {
                            if (!string.IsNullOrEmpty(objSystemFlag.Value))
                            {
                                SenderPassword = objSystemFlag.Value;
                                objSystemFlag = null;
                            }
                            else if (!string.IsNullOrEmpty(objSystemFlag.DefaultValue))
                            {
                                SenderPassword = objSystemFlag.DefaultValue;
                                objSystemFlag = null;
                            }
                        }


                        objSystemFlag = objSystemFlagList.Where(x => x.SystemFlagName.Trim().ToLower() == "SenderHostName".ToLower()).FirstOrDefault();
                        if (objSystemFlag != null)
                        {
                            if (!string.IsNullOrEmpty(objSystemFlag.Value))
                            {
                                SenderHostName = objSystemFlag.Value;
                                objSystemFlag = null;
                            }
                            else if (!string.IsNullOrEmpty(objSystemFlag.DefaultValue))
                            {
                                SenderHostName = objSystemFlag.DefaultValue;
                                objSystemFlag = null;
                            }
                        }

                        objSystemFlag = objSystemFlagList.Where(x => x.SystemFlagName.Trim().ToLower() == "SenderPort".ToLower()).FirstOrDefault();
                        if (objSystemFlag != null)
                        {
                            if (!string.IsNullOrEmpty(objSystemFlag.Value))
                            {
                                SenderPort = objSystemFlag.Value;
                                objSystemFlag = null;
                            }
                            else if (!string.IsNullOrEmpty(objSystemFlag.DefaultValue))
                            {
                                SenderPort = objSystemFlag.DefaultValue;
                                objSystemFlag = null;
                            }
                        }


                        objSystemFlag = objSystemFlagList.Where(x => x.SystemFlagName.Trim().ToLower() == "EnableSSL".ToLower()).FirstOrDefault();
                        if (objSystemFlag != null)
                        {
                            if (!string.IsNullOrEmpty(objSystemFlag.Value))
                            {
                                EnableSSL = objSystemFlag.Value;
                                objSystemFlag = null;
                            }
                            else if (!string.IsNullOrEmpty(objSystemFlag.DefaultValue))
                            {
                                EnableSSL = objSystemFlag.DefaultValue;
                                objSystemFlag = null;
                            }
                        }

                        if (!(string.IsNullOrEmpty(SenderEmailAddress) && string.IsNullOrEmpty(SenderPassword) && string.IsNullOrEmpty(SenderHostName) && string.IsNullOrEmpty(SenderPort) && string.IsNullOrEmpty(EnableSSL)))
                        {
                            Email EC = new Email(SenderEmailAddress, SenderPassword, SenderHostName, Convert.ToInt32(SenderPort), Convert.ToBoolean(EnableSSL), "");
                            bool result = EC.SendEmail("ePROMs", subject, MailBody, CID, UserName, System.Net.Mail.MailPriority.Normal, null);
                            if (result)
                            {
                                returnValue = "1";
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {

            }

            return returnValue;
        }

        [HttpPost]
        public string SendPracticeRegistrationMail(string ToAddress, string Password, Guid UserId)
        {
            string returnValue = "";
            try
            {
                objUtility = new Utilities();
                objSystemFlagBAL = new SystemFlagBAL();
                string subject = "ePROMs Registration Detail";
                string mailBody = "<div><br></div> <div>Welcome to ePROMs.</div>&nbsp;&nbsp;<div><br><div>Organizer invite you for ePROM, below is your registration details </div><div style=\"margin-top:10px;\">User name : " + ToAddress + "</div><div style=\"margin-top:10px;\">Password : " + Password + "</div></div><div style=\"margin-top:10px;\">Click on the below link to access your account.&nbsp;&nbsp;</div><div style=\"margin-top:10px;\"><a href={{VERIFICATION_LINK}}> click to here </a><br/><br></div><div><br></div><div><br></div><div style=\"margin-top:35px\">Thank you for choosing us!</div><div><img src=\"/Resources/Images/ePROMS-new-logo.png\" style=\"max-width: 200px; margin-top:20px;\" /></div><div><br></div>";

                string CID = "";
                mailBody = objUtility.GetImagesInHTMLString(mailBody, ref CID);

                string SenderEmailAddress = "";
                string SenderPassword = "";
                string SenderHostName = "";
                string SenderPort = "";
                string EnableSSL = "";

                string Token1 = "";
                string EncryptToken = "";
                string CheckAlreadyRequested = objSystemFlagBAL.IsAlreadyReqForgetPwd(ToAddress, true);
                if (string.IsNullOrEmpty(CheckAlreadyRequested))
                {
                    Token1 = Guid.NewGuid().ToString();
                    EncryptToken = Utilities.EncodeTo64(Token1);
                    Token1 = objSystemFlagBAL.GenerateToken(ToAddress, Token1, true);
                }
                else
                {
                    return "1";
                }


                string URL = Request.RequestUri.Scheme + "://" + Request.RequestUri.Authority + "/Login";

                string nURl = "" + URL + "?userid=" + UserId + "";
                mailBody = mailBody.Replace("{{VERIFICATION_LINK}}", nURl);

                if (string.IsNullOrEmpty(Token1) && string.IsNullOrEmpty(EncryptToken))
                {
                    return returnValue;
                }

                List<SystemFlag> objSystemFlagList = objSystemFlagBAL.GetAllSystemFlags();
                if (objSystemFlagList != null && objSystemFlagList.Count > 0)
                {
                    SystemFlag objSystemFlag = null;
                    objSystemFlag = objSystemFlagList.Where(x => x.SystemFlagName.Trim().ToLower() == "SenderEmailAddress".ToLower()).FirstOrDefault();
                    if (objSystemFlag != null)
                    {
                        if (!string.IsNullOrEmpty(objSystemFlag.Value))
                        {
                            SenderEmailAddress = objSystemFlag.Value;
                            objSystemFlag = null;
                        }
                        else if (!string.IsNullOrEmpty(objSystemFlag.DefaultValue))
                        {
                            SenderEmailAddress = objSystemFlag.DefaultValue;
                            objSystemFlag = null;
                        }
                    }

                    objSystemFlag = objSystemFlagList.Where(x => x.SystemFlagName.Trim().ToLower() == "SenderPassword".ToLower()).FirstOrDefault();
                    if (objSystemFlag != null)
                    {
                        if (!string.IsNullOrEmpty(objSystemFlag.Value))
                        {
                            SenderPassword = objSystemFlag.Value;
                            objSystemFlag = null;
                        }
                        else if (!string.IsNullOrEmpty(objSystemFlag.DefaultValue))
                        {
                            SenderPassword = objSystemFlag.DefaultValue;
                            objSystemFlag = null;
                        }
                    }


                    objSystemFlag = objSystemFlagList.Where(x => x.SystemFlagName.Trim().ToLower() == "SenderHostName".ToLower()).FirstOrDefault();
                    if (objSystemFlag != null)
                    {
                        if (!string.IsNullOrEmpty(objSystemFlag.Value))
                        {
                            SenderHostName = objSystemFlag.Value;
                            objSystemFlag = null;
                        }
                        else if (!string.IsNullOrEmpty(objSystemFlag.DefaultValue))
                        {
                            SenderHostName = objSystemFlag.DefaultValue;
                            objSystemFlag = null;
                        }
                    }

                    objSystemFlag = objSystemFlagList.Where(x => x.SystemFlagName.Trim().ToLower() == "SenderPort".ToLower()).FirstOrDefault();
                    if (objSystemFlag != null)
                    {
                        if (!string.IsNullOrEmpty(objSystemFlag.Value))
                        {
                            SenderPort = objSystemFlag.Value;
                            objSystemFlag = null;
                        }
                        else if (!string.IsNullOrEmpty(objSystemFlag.DefaultValue))
                        {
                            SenderPort = objSystemFlag.DefaultValue;
                            objSystemFlag = null;
                        }
                    }


                    objSystemFlag = objSystemFlagList.Where(x => x.SystemFlagName.Trim().ToLower() == "EnableSSL".ToLower()).FirstOrDefault();
                    if (objSystemFlag != null)
                    {
                        if (!string.IsNullOrEmpty(objSystemFlag.Value))
                        {
                            EnableSSL = objSystemFlag.Value;
                            objSystemFlag = null;
                        }
                        else if (!string.IsNullOrEmpty(objSystemFlag.DefaultValue))
                        {
                            EnableSSL = objSystemFlag.DefaultValue;
                            objSystemFlag = null;
                        }
                    }

                    if (!(string.IsNullOrEmpty(SenderEmailAddress) && string.IsNullOrEmpty(SenderPassword) && string.IsNullOrEmpty(SenderHostName) && string.IsNullOrEmpty(SenderPort) && string.IsNullOrEmpty(EnableSSL)))
                    {
                        Email EC = new Email(SenderEmailAddress, SenderPassword, SenderHostName, Convert.ToInt32(SenderPort), Convert.ToBoolean(EnableSSL), "");
                        bool result = EC.SendEmail("ePROMs", subject, mailBody, CID, ToAddress, System.Net.Mail.MailPriority.Normal, null);
                        if (result)
                        {
                            returnValue = "1";
                        }
                    }

                }
            }
            catch (Exception)
            {

            }

            return returnValue;
        }


        [HttpPost]
        public string SendPatientAssociateToProvider(Guid PatientId, Guid OrganizationId, Guid PracticeId, Guid ProviderId, string UserName)
        {
            string returnValue = "";
            try
            {
                string MailBody = string.Empty;
                objUtility = new Utilities();
                objSystemFlagBAL = new SystemFlagBAL();

                Guid? userid = Providers.GetUserIdByUserName(UserName);
                if (userid != Guid.Empty)
                {
                    var PatientUser = HelperMethods.GetEntities().GetUserDetailsByUserId(userid);
                    var objDetails = Patients.GetProviderDetailByPatient_Org_PracticeId(PatientId, ProviderId, OrganizationId, PracticeId);
                    string providerEmailID = HelperMethods.GetEntities().getProviderEmailID(ProviderId);
                    string ToAddress = UserName;

                    string subject = "ePROM Provider Associate";
                    string URL = Request.RequestUri.Scheme + "://" + Request.RequestUri.Authority + "/Login";

                    StreamReader reader = null;
                    try
                    {
                        reader = new StreamReader(HttpContext.Current.Server.MapPath("/Content/Html/PatientAssociateProvider.html"));
                        MailBody = reader.ReadToEnd();
                        MailBody = MailBody.Replace("{{PatientName}}", PatientUser.FirstName + " " + PatientUser.LastName)
                                     .Replace("{{DoctorName}}", "Dr." + objDetails.ProviderName)
                                     .Replace("{{OrganizationName}}", "'" + objDetails.OrganizationName + "'")
                                     .Replace("{{PracticeName}}", "'" + objDetails.PracticeName + "'")
                                     .Replace("{{address1}}", objDetails.Address == null ? "" : objDetails.Address.Line1)
                                     .Replace("{{address2}}", objDetails.Address == null ? "" : objDetails.Address.Line2)
                                     .Replace("{{ZipCode}}", objDetails.Address == null ? "" : objDetails.Address.ZipCode)
                                     .Replace("{{TelephoneNo}}", objDetails.Contact == null ? "" : objDetails.Contact.Mobile)
                                     .Replace("{{Fax}}", objDetails.Contact == null ? "" : objDetails.Contact.FAX)
                                     .Replace("{{Email}}", providerEmailID)
                                     .Replace("{{LoginLink}}", URL);
                    }
                    finally
                    {
                        if (reader != null)
                            reader.Dispose();
                    }

                    string CID = "";
                    MailBody = objUtility.GetImagesInHTMLString(MailBody, ref CID);

                    string SenderEmailAddress = "";
                    string SenderPassword = "";
                    string SenderHostName = "";
                    string SenderPort = "";
                    string EnableSSL = "";

                    List<SystemFlag> objSystemFlagList = objSystemFlagBAL.GetAllSystemFlags();
                    if (objSystemFlagList != null && objSystemFlagList.Count > 0)
                    {
                        SystemFlag objSystemFlag = null;
                        objSystemFlag = objSystemFlagList.Where(x => x.SystemFlagName.Trim().ToLower() == "SenderEmailAddress".ToLower()).FirstOrDefault();
                        if (objSystemFlag != null)
                        {
                            if (!string.IsNullOrEmpty(objSystemFlag.Value))
                            {
                                SenderEmailAddress = objSystemFlag.Value;
                                objSystemFlag = null;
                            }
                            else if (!string.IsNullOrEmpty(objSystemFlag.DefaultValue))
                            {
                                SenderEmailAddress = objSystemFlag.DefaultValue;
                                objSystemFlag = null;
                            }
                        }

                        objSystemFlag = objSystemFlagList.Where(x => x.SystemFlagName.Trim().ToLower() == "SenderPassword".ToLower()).FirstOrDefault();
                        if (objSystemFlag != null)
                        {
                            if (!string.IsNullOrEmpty(objSystemFlag.Value))
                            {
                                SenderPassword = objSystemFlag.Value;
                                objSystemFlag = null;
                            }
                            else if (!string.IsNullOrEmpty(objSystemFlag.DefaultValue))
                            {
                                SenderPassword = objSystemFlag.DefaultValue;
                                objSystemFlag = null;
                            }
                        }


                        objSystemFlag = objSystemFlagList.Where(x => x.SystemFlagName.Trim().ToLower() == "SenderHostName".ToLower()).FirstOrDefault();
                        if (objSystemFlag != null)
                        {
                            if (!string.IsNullOrEmpty(objSystemFlag.Value))
                            {
                                SenderHostName = objSystemFlag.Value;
                                objSystemFlag = null;
                            }
                            else if (!string.IsNullOrEmpty(objSystemFlag.DefaultValue))
                            {
                                SenderHostName = objSystemFlag.DefaultValue;
                                objSystemFlag = null;
                            }
                        }

                        objSystemFlag = objSystemFlagList.Where(x => x.SystemFlagName.Trim().ToLower() == "SenderPort".ToLower()).FirstOrDefault();
                        if (objSystemFlag != null)
                        {
                            if (!string.IsNullOrEmpty(objSystemFlag.Value))
                            {
                                SenderPort = objSystemFlag.Value;
                                objSystemFlag = null;
                            }
                            else if (!string.IsNullOrEmpty(objSystemFlag.DefaultValue))
                            {
                                SenderPort = objSystemFlag.DefaultValue;
                                objSystemFlag = null;
                            }
                        }


                        objSystemFlag = objSystemFlagList.Where(x => x.SystemFlagName.Trim().ToLower() == "EnableSSL".ToLower()).FirstOrDefault();
                        if (objSystemFlag != null)
                        {
                            if (!string.IsNullOrEmpty(objSystemFlag.Value))
                            {
                                EnableSSL = objSystemFlag.Value;
                                objSystemFlag = null;
                            }
                            else if (!string.IsNullOrEmpty(objSystemFlag.DefaultValue))
                            {
                                EnableSSL = objSystemFlag.DefaultValue;
                                objSystemFlag = null;
                            }
                        }

                        if (!(string.IsNullOrEmpty(SenderEmailAddress) && string.IsNullOrEmpty(SenderPassword) && string.IsNullOrEmpty(SenderHostName) && string.IsNullOrEmpty(SenderPort) && string.IsNullOrEmpty(EnableSSL)))
                        {
                            Email EC = new Email(SenderEmailAddress, SenderPassword, SenderHostName, Convert.ToInt32(SenderPort), Convert.ToBoolean(EnableSSL), "");
                            bool result = EC.SendEmail("ePROMs", subject, MailBody, CID, ToAddress, System.Net.Mail.MailPriority.Normal, null);
                            if (result)
                            {
                                returnValue = "1";
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {

            }
            return returnValue;
        }


        [HttpPost]
        public string NotifyToPatient(Guid ProviderId, Guid PatientId, Guid OrganizationId, Guid PracticeId, int SurveyId, string ToAddress, string PatientName, string epromTitle, string Suggestion)
        {
            string returnValue = "";
            string MailBody = string.Empty, URL = "";
            try
            {
                string providerEmailID = HelperMethods.GetEntities().getProviderEmailID(ProviderId);
                var objDetails = PatientSurveyClass.GetpatientProvidersDetails(PatientId, OrganizationId, PracticeId, ProviderId, SurveyId);
                if (objDetails != null)
                {
                    var objThirdPartyApp = ProviderPatientThirdPartyAppClass.GetProviderPatientThirdPartyApp(PatientId, OrganizationId, PracticeId, ProviderId, SurveyId);
                    string strThirdPartyApp = "";
                    for (int i = 0; i < objThirdPartyApp.Count; i++)
                    {
                        if (i == 0)
                        {
                            strThirdPartyApp += "<table style=\" border-collapse: collapse;\" border=\"1\" cellpadding=\"8\"><thead><tr><th>App Name</th><th> URL</th></tr></thead><tbody>";
                        }

                        strThirdPartyApp += "<tr><td>" + objThirdPartyApp[i].AppName + "</td><td>" + objThirdPartyApp[i].URL + "</td></tr>";
                    }

                    objUtility = new Utilities();
                    objSystemFlagBAL = new SystemFlagBAL();
                    string subject = "ePROMs Treatment - Message";
                    string CID = "";

                    StreamReader reader = null;
                    try
                    {
                        reader = new StreamReader(HttpContext.Current.Server.MapPath("/Content/Html/ePromTreatment.html"));
                        MailBody = reader.ReadToEnd();
                        URL = Request.RequestUri.Scheme + "://" + Request.RequestUri.Authority + "/Login";
                        MailBody = MailBody.Replace("{{PatientName}}", PatientName)
                                     .Replace("{{OrganizationName}}", objDetails.OrganizationName)
                                     .Replace("{{PracticeName}}", objDetails.PracticeName)
                                     .Replace("{{suggestion}}", Suggestion)
                                     .Replace("{{ThirdPartyApp}}", strThirdPartyApp)
                                     .Replace("{{epromTitle}}", epromTitle)
                                     .Replace("{{dueDate}}", Convert.ToDateTime(objDetails.DueDate).ToString("dd/MM/yyyy"))
                                     .Replace("{{LoginLink}}", URL)
                                     .Replace("{{DoctorName}}", "Dr." + objDetails.ProviderName)
                                     .Replace("{{address1}}", objDetails.Address1)
                                     .Replace("{{address2}}", objDetails.Address2)
                                     .Replace("{{ZipCode}}", objDetails.ZipCode)
                                     .Replace("{{TelephoneNo}}", objDetails.TelephoneNo)
                                     .Replace("{{Fax}}", objDetails.Fax)
                                     .Replace("{{Email}}", providerEmailID);
                    }
                    finally
                    {
                        if (reader != null)
                            reader.Dispose();
                    }

                    MailBody = objUtility.GetImagesInHTMLString(MailBody, ref CID);

                    string SenderEmailAddress = "";
                    string SenderPassword = "";
                    string SenderHostName = "";
                    string SenderPort = "";
                    string EnableSSL = "";

                    List<SystemFlag> objSystemFlagList = objSystemFlagBAL.GetAllSystemFlags();
                    if (objSystemFlagList != null && objSystemFlagList.Count > 0)
                    {
                        SystemFlag objSystemFlag = null;
                        objSystemFlag = objSystemFlagList.Where(x => x.SystemFlagName.Trim().ToLower() == "SenderEmailAddress".ToLower()).FirstOrDefault();
                        if (objSystemFlag != null)
                        {
                            if (!string.IsNullOrEmpty(objSystemFlag.Value))
                            {
                                SenderEmailAddress = objSystemFlag.Value;
                                objSystemFlag = null;
                            }
                            else if (!string.IsNullOrEmpty(objSystemFlag.DefaultValue))
                            {
                                SenderEmailAddress = objSystemFlag.DefaultValue;
                                objSystemFlag = null;
                            }
                        }

                        objSystemFlag = objSystemFlagList.Where(x => x.SystemFlagName.Trim().ToLower() == "SenderPassword".ToLower()).FirstOrDefault();
                        if (objSystemFlag != null)
                        {
                            if (!string.IsNullOrEmpty(objSystemFlag.Value))
                            {
                                SenderPassword = objSystemFlag.Value;
                                objSystemFlag = null;
                            }
                            else if (!string.IsNullOrEmpty(objSystemFlag.DefaultValue))
                            {
                                SenderPassword = objSystemFlag.DefaultValue;
                                objSystemFlag = null;
                            }
                        }


                        objSystemFlag = objSystemFlagList.Where(x => x.SystemFlagName.Trim().ToLower() == "SenderHostName".ToLower()).FirstOrDefault();
                        if (objSystemFlag != null)
                        {
                            if (!string.IsNullOrEmpty(objSystemFlag.Value))
                            {
                                SenderHostName = objSystemFlag.Value;
                                objSystemFlag = null;
                            }
                            else if (!string.IsNullOrEmpty(objSystemFlag.DefaultValue))
                            {
                                SenderHostName = objSystemFlag.DefaultValue;
                                objSystemFlag = null;
                            }
                        }

                        objSystemFlag = objSystemFlagList.Where(x => x.SystemFlagName.Trim().ToLower() == "SenderPort".ToLower()).FirstOrDefault();
                        if (objSystemFlag != null)
                        {
                            if (!string.IsNullOrEmpty(objSystemFlag.Value))
                            {
                                SenderPort = objSystemFlag.Value;
                                objSystemFlag = null;
                            }
                            else if (!string.IsNullOrEmpty(objSystemFlag.DefaultValue))
                            {
                                SenderPort = objSystemFlag.DefaultValue;
                                objSystemFlag = null;
                            }
                        }


                        objSystemFlag = objSystemFlagList.Where(x => x.SystemFlagName.Trim().ToLower() == "EnableSSL".ToLower()).FirstOrDefault();
                        if (objSystemFlag != null)
                        {
                            if (!string.IsNullOrEmpty(objSystemFlag.Value))
                            {
                                EnableSSL = objSystemFlag.Value;
                                objSystemFlag = null;
                            }
                            else if (!string.IsNullOrEmpty(objSystemFlag.DefaultValue))
                            {
                                EnableSSL = objSystemFlag.DefaultValue;
                                objSystemFlag = null;
                            }
                        }

                        if (!(string.IsNullOrEmpty(SenderEmailAddress) && string.IsNullOrEmpty(SenderPassword) && string.IsNullOrEmpty(SenderHostName) && string.IsNullOrEmpty(SenderPort) && string.IsNullOrEmpty(EnableSSL)))
                        {
                            Email EC = new Email(SenderEmailAddress, SenderPassword, SenderHostName, Convert.ToInt32(SenderPort), Convert.ToBoolean(EnableSSL), "");
                            bool result = EC.SendEmail("Dr." + objDetails.ProviderName, subject, MailBody, CID, ToAddress, System.Net.Mail.MailPriority.Normal, null);
                            if (result)
                            {
                                returnValue = "1";
                            }
                        }

                    }
                }
            }
            catch (Exception)
            {

            }

            return returnValue;
        }

        [HttpPost]
        public string SendEpromsCompleteMailToPatient()
        {
            string returnValue = "";
            try
            {
                string ImageName = Guid.NewGuid() + ".png";
                string ImagePath = "", ImageFullPath = "";

                Guid PatientId = new Guid(Request.Headers.GetValues("PatientId").FirstOrDefault());
                string UserName = Request.Headers.GetValues("UserName").FirstOrDefault();
                string ePromTitle = Request.Headers.GetValues("ePromTitle").FirstOrDefault();
                string EpromScore = Request.Headers.GetValues("ScoreRow").FirstOrDefault();
                string strPatientSurveyId = Request.Headers.GetValues("PatientSurveyId").FirstOrDefault();
                string Imagefile = Request.Headers.GetValues("Imagefile").FirstOrDefault();
                string TableData = Request.Headers.GetValues("TableData").FirstOrDefault();

                Guid PatientSurveyId = Guid.Empty;

                if (strPatientSurveyId != null && strPatientSurveyId != "")
                {
                    PatientSurveyId = new Guid(strPatientSurveyId);
                }

                if (!string.IsNullOrEmpty(Imagefile))
                {
                    string DirectoryName = "~/Content/Image/";
                    string DirectoryPath = HttpContext.Current.Server.MapPath(DirectoryName);

                    if (!Directory.Exists(DirectoryPath))
                    {
                        Directory.CreateDirectory(DirectoryPath);
                    }

                    byte[] data;
                    string ImageURL = "http://export.highcharts.com/" + Imagefile;
                    using (WebClient client = new WebClient())
                    {
                        data = client.DownloadData(ImageURL);
                    }

                    ImageFullPath = DirectoryPath + "/" + ImageName;
                    File.WriteAllBytes(ImageFullPath, data);
                    ImagePath = "../Content/Image/" + ImageName;
                }

                string MailBody = string.Empty;
                objUtility = new Utilities();
                objSystemFlagBAL = new SystemFlagBAL();

                Guid? userid = Providers.GetUserIdByUserName(UserName);
                if (userid != Guid.Empty)
                {
                    var PatientUser = HelperMethods.GetEntities().GetUserDetailsByUserId(userid);
                    var objDetails = Patients.GetProviderDetailByPatientSurveyID(PatientId, PatientSurveyId);
                    string providerEmailID = HelperMethods.GetEntities().getProviderEmailID(objDetails.ProviderId);
                    string ToAddress = UserName;

                    string subject = "ePROMs " + ePromTitle + " - RES";

                    var PracticeId = objDetails.PracticeID ?? Guid.Empty;
                    var pDetail = HelperMethods.GetEntities().GetPracticeByID(PracticeId);
                    var uid = pDetail.UserId ?? Guid.Empty;
                    var practiceDetail = PracticeClass.GetPracticeDetail(uid);

                    if (ePromTitle == "Preventive ePROMs for Population Health Management by GPs")
                    {
                        StreamReader reader = null;
                        try
                        {
                            reader = new StreamReader(HttpContext.Current.Server.MapPath("/Content/Html/EpromCompleteMailToPatientForTable.html"));
                            MailBody = reader.ReadToEnd();
                            MailBody = MailBody.Replace("{{PatientName}}", PatientUser.FirstName + " " + PatientUser.LastName)
                                     .Replace("{{epromTitle}}", ePromTitle)
                                     .Replace("{{DoctorName}}", practiceDetail.Practice.PracticeName)
                                     //.Replace("{{DoctorName}}", "Dr." + ProviderName)
                                     .Replace("{{OrganizationName}}", objDetails.OrganizationName)
                                     .Replace("{{PracticeName}}", objDetails.PracticeName)
                                     .Replace("{{ScoreRow}}", EpromScore)
                                     .Replace("{{address1}}", practiceDetail.Address == null ? "" : practiceDetail.Address.Line1)
                                     .Replace("{{address2}}", practiceDetail.Address == null ? "" : practiceDetail.Address.Line2)
                                     .Replace("{{ZipCode}}", practiceDetail.Address == null ? "" : practiceDetail.Address.ZipCode)
                                     .Replace("{{TelephoneNo}}", practiceDetail.Contact == null ? "" : practiceDetail.Contact.Mobile)
                                     .Replace("{{Fax}}", practiceDetail.Contact == null ? "" : practiceDetail.Contact.FAX)
                                     //.Replace("{{address1}}", objDetails.Address == null ? "" : objDetails.Address.Line1)
                                     //.Replace("{{address2}}", objDetails.Address == null ? "" : objDetails.Address.Line2)
                                     //.Replace("{{ZipCode}}", objDetails.Address == null ? "" : objDetails.Address.ZipCode)
                                     //.Replace("{{TelephoneNo}}", objDetails.Contact == null ? "" : objDetails.Contact.Mobile)
                                     //.Replace("{{Fax}}", objDetails.Contact == null ? "" : objDetails.Contact.FAX)
                                     .Replace("{{Email}}", practiceDetail.User.Email)
                                     //.Replace("{{Email}}", providerEmailID)
                                     .Replace("{{ScoreRow}}", EpromScore)
                                     .Replace("{{TableData}}", TableData);
                        }
                        finally
                        {
                            if (reader != null)
                                reader.Dispose();
                        }
                    }
                    else
                    {
                        StreamReader reader = null;
                        try
                        {
                            reader = new StreamReader(HttpContext.Current.Server.MapPath("/Content/Html/EpromCompleteMailToPatient.html"));
                            MailBody = reader.ReadToEnd();
                            MailBody = MailBody.Replace("{{PatientName}}", PatientUser.FirstName + " " + PatientUser.LastName)
                                     .Replace("{{epromTitle}}", ePromTitle)
                                     .Replace("{{DoctorName}}", practiceDetail.Practice.PracticeName)
                                     //.Replace("{{DoctorName}}", "Dr." + ProviderName)
                                     .Replace("{{OrganizationName}}", objDetails.OrganizationName)
                                     .Replace("{{PracticeName}}", objDetails.PracticeName)
                                     .Replace("{{ScoreRow}}", EpromScore)
                                     .Replace("{{address1}}", practiceDetail.Address == null ? "" : practiceDetail.Address.Line1)
                                     .Replace("{{address2}}", practiceDetail.Address == null ? "" : practiceDetail.Address.Line2)
                                     .Replace("{{ZipCode}}", practiceDetail.Address == null ? "" : practiceDetail.Address.ZipCode)
                                     .Replace("{{TelephoneNo}}", practiceDetail.Contact == null ? "" : practiceDetail.Contact.Mobile)
                                     .Replace("{{Fax}}", practiceDetail.Contact == null ? "" : practiceDetail.Contact.FAX)
                                     //.Replace("{{address1}}", objDetails.Address == null ? "" : objDetails.Address.Line1)
                                     //.Replace("{{address2}}", objDetails.Address == null ? "" : objDetails.Address.Line2)
                                     //.Replace("{{ZipCode}}", objDetails.Address == null ? "" : objDetails.Address.ZipCode)
                                     //.Replace("{{TelephoneNo}}", objDetails.Contact == null ? "" : objDetails.Contact.Mobile)
                                     //.Replace("{{Fax}}", objDetails.Contact == null ? "" : objDetails.Contact.FAX)
                                     .Replace("{{Email}}", practiceDetail.User.Email)
                                     //.Replace("{{Email}}", providerEmailID)
                                     .Replace("{{ScoreRow}}", EpromScore)
                                     .Replace("{{ImageName}}", ImagePath);
                        }
                        finally
                        {
                            if (reader != null)
                                reader.Dispose();
                        }
                    }

                    string CID = "";
                    MailBody = objUtility.GetImagesInHTMLString(MailBody, ref CID);

                    string SenderEmailAddress = "";
                    string SenderPassword = "";
                    string SenderHostName = "";
                    string SenderPort = "";
                    string EnableSSL = "";

                    List<SystemFlag> objSystemFlagList = objSystemFlagBAL.GetAllSystemFlags();
                    if (objSystemFlagList != null && objSystemFlagList.Count > 0)
                    {
                        SystemFlag objSystemFlag = null;
                        objSystemFlag = objSystemFlagList.Where(x => x.SystemFlagName.Trim().ToLower() == "SenderEmailAddress".ToLower()).FirstOrDefault();
                        if (objSystemFlag != null)
                        {
                            if (!string.IsNullOrEmpty(objSystemFlag.Value))
                            {
                                SenderEmailAddress = objSystemFlag.Value;
                                objSystemFlag = null;
                            }
                            else if (!string.IsNullOrEmpty(objSystemFlag.DefaultValue))
                            {
                                SenderEmailAddress = objSystemFlag.DefaultValue;
                                objSystemFlag = null;
                            }
                        }

                        objSystemFlag = objSystemFlagList.Where(x => x.SystemFlagName.Trim().ToLower() == "SenderPassword".ToLower()).FirstOrDefault();
                        if (objSystemFlag != null)
                        {
                            if (!string.IsNullOrEmpty(objSystemFlag.Value))
                            {
                                SenderPassword = objSystemFlag.Value;
                                objSystemFlag = null;
                            }
                            else if (!string.IsNullOrEmpty(objSystemFlag.DefaultValue))
                            {
                                SenderPassword = objSystemFlag.DefaultValue;
                                objSystemFlag = null;
                            }
                        }


                        objSystemFlag = objSystemFlagList.Where(x => x.SystemFlagName.Trim().ToLower() == "SenderHostName".ToLower()).FirstOrDefault();
                        if (objSystemFlag != null)
                        {
                            if (!string.IsNullOrEmpty(objSystemFlag.Value))
                            {
                                SenderHostName = objSystemFlag.Value;
                                objSystemFlag = null;
                            }
                            else if (!string.IsNullOrEmpty(objSystemFlag.DefaultValue))
                            {
                                SenderHostName = objSystemFlag.DefaultValue;
                                objSystemFlag = null;
                            }
                        }

                        objSystemFlag = objSystemFlagList.Where(x => x.SystemFlagName.Trim().ToLower() == "SenderPort".ToLower()).FirstOrDefault();
                        if (objSystemFlag != null)
                        {
                            if (!string.IsNullOrEmpty(objSystemFlag.Value))
                            {
                                SenderPort = objSystemFlag.Value;
                                objSystemFlag = null;
                            }
                            else if (!string.IsNullOrEmpty(objSystemFlag.DefaultValue))
                            {
                                SenderPort = objSystemFlag.DefaultValue;
                                objSystemFlag = null;
                            }
                        }


                        objSystemFlag = objSystemFlagList.Where(x => x.SystemFlagName.Trim().ToLower() == "EnableSSL".ToLower()).FirstOrDefault();
                        if (objSystemFlag != null)
                        {
                            if (!string.IsNullOrEmpty(objSystemFlag.Value))
                            {
                                EnableSSL = objSystemFlag.Value;
                                objSystemFlag = null;
                            }
                            else if (!string.IsNullOrEmpty(objSystemFlag.DefaultValue))
                            {
                                EnableSSL = objSystemFlag.DefaultValue;
                                objSystemFlag = null;
                            }
                        }

                        if (!(string.IsNullOrEmpty(SenderEmailAddress) && string.IsNullOrEmpty(SenderPassword) && string.IsNullOrEmpty(SenderHostName) && string.IsNullOrEmpty(SenderPort) && string.IsNullOrEmpty(EnableSSL)))
                        {
                            Email EC = new Email(SenderEmailAddress, SenderPassword, SenderHostName, Convert.ToInt32(SenderPort), Convert.ToBoolean(EnableSSL), "");
                            bool result = EC.SendEmail("ePROMs", subject, MailBody, CID, ToAddress, System.Net.Mail.MailPriority.Normal, null);
                            if (result)
                            {
                                returnValue = SendEpromsCompleteMailToProvider(UserName, userid, ePromTitle, EpromScore, SenderEmailAddress, SenderHostName, SenderPort, SenderPassword, EnableSSL, PatientUser, objDetails, ImagePath, ImageFullPath, TableData);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, "", ex.StackTrace);
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
            }
            return returnValue;
        }

        [HttpPost]
        public string SendEpromsCompleteMailToProvider(string userName, Guid? userid, string ePromTitle, string ScoreRow, string SenderEmailAddress, string SenderHostName, string SenderPort, string SenderPassword, string EnableSSL, UserDetail User, Patient_model objDetails, string Imagefile, string ImagePath, string TableData)
        {
            string returnValue = "";
            try
            {
                string MailBody = string.Empty;
                objUtility = new Utilities();
                objSystemFlagBAL = new SystemFlagBAL();

                if (ScoreRow.IndexOf("Your") > -1)
                {
                    ScoreRow = ScoreRow.Replace("Your", "Patient's ");
                }

                if (userid != Guid.Empty)
                {
                    string ToAddress = objDetails.ProviderEmail;
                    string providerEmailID = HelperMethods.GetEntities().getProviderEmailID(objDetails.ProviderId);
                    string subject = "ePROMs " + ePromTitle + " - RES";

                    if (ePromTitle == "Preventive ePROMs for Population Health Management by GPs")
                    {
                        StreamReader reader = null;

                        try
                        {
                            reader = new StreamReader(HttpContext.Current.Server.MapPath("/Content/Html/EpromCompleteMailToProviderForTable.html"));
                            MailBody = reader.ReadToEnd();
                            MailBody = MailBody.Replace("{{PatientName}}", User.FirstName + " " + User.LastName)
                                    .Replace("{{epromTitle}}", ePromTitle)
                                    .Replace("{{DoctorName}}", "Dr." + objDetails.PracticeName)
                                    //.Replace("{{DoctorName}}", "Dr." + objDetails.ProviderName)
                                    .Replace("{{OrganizationName}}", objDetails.OrganizationName)
                                    .Replace("{{PracticeName}}", objDetails.PracticeName)
                                    .Replace("{{address1}}", objDetails.Address == null ? "" : objDetails.Address.Line1)
                                    .Replace("{{address2}}", objDetails.Address == null ? "" : objDetails.Address.Line2)
                                    .Replace("{{ZipCode}}", objDetails.Address == null ? "" : objDetails.Address.ZipCode)
                                    .Replace("{{TelephoneNo}}", objDetails.Contact == null ? "" : objDetails.Contact.Mobile)
                                    .Replace("{{Fax}}", objDetails.Contact == null ? "" : objDetails.Contact.FAX)
                                    .Replace("{{Email}}", objDetails.User.Email)
                                    //.Replace("{{Email}}", providerEmailID)
                                    .Replace("{{ScoreRow}}", ScoreRow)
                                    .Replace("{{TableData}}", TableData);
                        }
                        finally
                        {
                            if (reader != null)
                                reader.Dispose();
                        }
                    }
                    else
                    {
                        StreamReader reader = null;

                        try
                        {
                            reader = new StreamReader(HttpContext.Current.Server.MapPath("/Content/Html/EpromCompleteMailToProvider.html"));
                            MailBody = reader.ReadToEnd();
                            MailBody = MailBody.Replace("{{PatientName}}", User.FirstName + " " + User.LastName)
                                    .Replace("{{epromTitle}}", ePromTitle)
                                    .Replace("{{DoctorName}}", objDetails.PracticeName)
                                    //.Replace("{{DoctorName}}", "Dr." + objDetails.ProviderName)
                                    .Replace("{{OrganizationName}}", objDetails.OrganizationName)
                                    .Replace("{{PracticeName}}", objDetails.PracticeName)
                                    .Replace("{{address1}}", objDetails.Address == null ? "" : objDetails.Address.Line1)
                                    .Replace("{{address2}}", objDetails.Address == null ? "" : objDetails.Address.Line2)
                                    .Replace("{{ZipCode}}", objDetails.Address == null ? "" : objDetails.Address.ZipCode)
                                    .Replace("{{TelephoneNo}}", objDetails.Contact == null ? "" : objDetails.Contact.Mobile)
                                    .Replace("{{Fax}}", objDetails.Contact == null ? "" : objDetails.Contact.FAX)
                                    .Replace("{{Email}}", objDetails.User.Email)
                                    //.Replace("{{Email}}", providerEmailID)
                                    .Replace("{{ScoreRow}}", ScoreRow)
                                    .Replace("{{ImageName}}", Imagefile);
                        }
                        finally
                        {
                            if (reader != null)
                                reader.Dispose();
                        }
                    }

                    string CID = "";
                    MailBody = objUtility.GetImagesInHTMLString(MailBody, ref CID);

                    if (!(string.IsNullOrEmpty(SenderEmailAddress) && string.IsNullOrEmpty(SenderPassword) && string.IsNullOrEmpty(SenderHostName) && string.IsNullOrEmpty(SenderPort) && string.IsNullOrEmpty(EnableSSL)))
                    {
                        Email EC = new Email(SenderEmailAddress, SenderPassword, SenderHostName, Convert.ToInt32(SenderPort), Convert.ToBoolean(EnableSSL), "");
                        bool result = EC.SendEmail("ePROMs", subject, MailBody, CID, ToAddress, System.Net.Mail.MailPriority.Normal, null);
                        if (result)
                        {
                            if (File.Exists(ImagePath))
                            {
                                File.Delete(ImagePath);
                            }
                            returnValue = "Email sent successfully!";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, "", ex.StackTrace);
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
            }
            return returnValue;
        }

    }
}
