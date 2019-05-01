using System;
using System.Web.Http;
using BLL;
using System.Collections.Generic;
using Newtonsoft.Json;
using DAL;

namespace API.Controllers
{
    public class PatientController : ApiController
    {
        [HttpGet]
        public List<Provider_Patient_model> GetProviderPatientDetailByUserName(string UserName, Guid? OrganizationId, Guid? PracticeId)
        {
            return Patients.GetProviderPatientDetailByUserName(UserName, OrganizationId, PracticeId);
        }

        [HttpGet]
        public List<Provider_Patient_model> GetProviderPatientDetailBySeletedPatient(string UserName, Guid? OrganizationId, Guid? PracticeId, string patientName)
        {
            return Patients.GetProviderPatientDetailBySeletedPatient(UserName, OrganizationId, PracticeId, patientName);
        }

        [HttpGet]
        public Patient_model GetPatientByPatientID(string id)
        {
            Guid PatientId = new Guid(id);
            return Patients.GetPatientById(PatientId);
        }

        [HttpGet]
        public Patient_model GetPatientDetailsByUserName(string UserName)
        {
            return Patients.GetPatientDetailsByUserName(UserName);
        }

        [HttpGet]
        public List<PatientSurveyStatus_model> GetPatientSurveyStatusData(Guid? PatientID, Guid OrganizationId, Guid PracticeId, Guid ProviderId, int SurveyId, string FromDate, bool forEmail)
        {
            return Patients.GetPatientSurveyStatusData(PatientID, OrganizationId, PracticeId, ProviderId, SurveyId, FromDate, forEmail);
        }

        [HttpGet]
        public List<PatientSurveyStatus_model> GetPatientSurveyStatusByID(Guid ID)
        {
            return Patients.GetPatientSurveyStatusByID(ID);
        }

        [HttpPost]
        public string CreatePatient(Patient_model objPatient)
        {
            Guid? userId = Providers.GetUserIdByUserName(objPatient.User.UserName);

            return Patients.CreatePatient(objPatient, (Guid)userId);
        }

        [HttpPost]
        public bool UpdateUserData(Patient_model objPatient)
        {
            return Patients.UpdateUserData(objPatient);
        }

        [HttpPost]
        public object InsertPatientData(List<Patient_model> patientList)
        {
            int totalAddedPatient = 0;
            int totalNotAddedPatient = 0;
            try
            {
                for (int i = 0; i < patientList.Count; i++)
                {
                    var patientId = Patients.CreatePatient(patientList[i], (Guid)patientList[i].Address.CreatedBy);
                    if (patientId != null && patientId != "")
                    {
                        var eUrl = Request.RequestUri.Scheme + "://" + Request.RequestUri.Authority + "/api/Email/SendPatientRegistrationMail?PatientId=" + patientId + "&ProviderId=" + patientList[i].ProviderId + "&UserName=" + patientList[i].User.Email + "&Password=123456&UserId=" + patientList[i].User.ID;

                        string res = Entities.CustomeWebRequest(eUrl, "POST");

                        var AssocialteUrl = Request.RequestUri.Scheme + "://" + Request.RequestUri.Authority + "/api/Email/SendPatientAssociateToProvider?PatientId=" + patientId + "&OrganizationId=" + patientList[i].OrganizationID + "&PracticeId=" + patientList[i].PracticeID + "&ProviderId=" + patientList[i].ProviderId + "&UserName=" + patientList[i].User.Email;

                        Entities.CustomeWebRequest(AssocialteUrl, "POST");

                        totalAddedPatient++;
                    }
                    else
                    {
                        totalNotAddedPatient++;
                    }
                }
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
            }
            return Json(new
            {
                status = 1,
                dataList = patientList,
                TotalAddedPatient = totalAddedPatient,
                TotalNotAddedPatient = totalNotAddedPatient,
                TotalCsvRecords = patientList.Count
            });
        }

        [HttpPost]
        public string AddPatientIndicators(PatientIndicators_model patientIndicator)
        {
            return PatientIndicatorsClass.CreatePatientIndicators(patientIndicator);
        }

        [HttpDelete]
        public string DeletePatientIndicators(string Id)
        {
            int patientIndicatorId = Convert.ToInt32(Id);
            return PatientIndicatorsClass.DeletePatientIndicatorsById(patientIndicatorId);
        }

        [HttpGet]
        public string GetPatientIndicatorsByPatientID(string patientid)
        {
            if (patientid != null && patientid != "")
            {
                Guid patient = new Guid(patientid);
                if (patient != Guid.Empty)
                    return JsonConvert.SerializeObject(PatientIndicatorsClass.GetIndicators_By_patientId(patient));
                else
                    return "";
            }
            return "";
        }

        [HttpGet]
        public string GetLastProviderCode()
        {
            return Providers.GetLastProviderCode();

        }

        [HttpPost]
        public Guid CreatePatientSurveyStatus(PatientSurveyStatus_model objstatus)
        {
            return PatientSurveyStatusClass.CreatePatientSurveyStatus(objstatus);
        }

        [HttpPost]
        public string UpdatePatientSurveyStatus(PatientSurveyStatus_model objstatus)
        {
            return PatientSurveyStatusClass.UpdatePatientSurveyStatus(objstatus);
        }

        //[HttpGet]
        //public string GetPatientSurveyStatus(string PatientId, Guid OrganizationId, Guid PracticeId, Guid ProviderId, int SurveyId, string FromDate, string ToDate)
        //{
        //    Guid GPatientId = string.IsNullOrEmpty(PatientId) || PatientId == "undefined" || PatientId == "-1" ? Guid.Empty : new Guid(PatientId);

        //    return JsonConvert.SerializeObject(PatientSurveyStatusClass.GetPatientSurveyStatus(GPatientId, OrganizationId, PracticeId, ProviderId, SurveyId, FromDate, ToDate));
        //}

        [HttpGet]
        public string GetPatientSurveyStatus(string PatientId, Guid OrganizationId, Guid PracticeId, Guid ProviderId, int SurveyId, string FromDate, string ToDate, string PathwayID)
        {
            Guid GPatientId = string.IsNullOrEmpty(PatientId) || PatientId == "undefined" || PatientId == "-1" ? Guid.Empty : new Guid(PatientId);

            return JsonConvert.SerializeObject(PatientSurveyStatusClass.GetPatientSurveyStatus(GPatientId, OrganizationId, PracticeId, ProviderId, SurveyId, FromDate, ToDate, PathwayID));
        }

        [HttpGet]
        public PatientSurvey_model GetPatientSurveyByID(Guid PatientSurveyId)
        {
            return PatientSurveyClass.GetPatientSurveyByID(PatientSurveyId);
        }

        [HttpGet]
        public string CheckInvalidDate(Guid PatientSurveyId)
        {
            return PatientSurveyClass.CheckInvalidDate(PatientSurveyId);
        }

        [HttpGet]
        public string CheckTodayDate_PatientSurveyStatusExist(Guid PatientSurveyID)
        {
            return PatientSurveyStatusClass.CheckTodayDate_PatientSurveyStatusExist(PatientSurveyID);
        }

        [HttpGet]
        public string ActiveDeactivePatient(Guid? PatientID, bool status)
        {
            return Patients.ActiveDeactivePatient(PatientID, status);
        }

        [HttpGet]
        public List<ProviderPatientThirdPartyApp_model> GetPatientThirdPartyApp(Guid PatientID, int SurveyID)
        {
            return ProviderPatientThirdPartyAppClass.GetPatientThirdPartyApp(PatientID, SurveyID);
        }

        [HttpGet]
        public ConsumerDetail getConsumerDetails(string IHINo)
        {
            return LinkedEHR.getConsumerDetails(IHINo);
        }

        [HttpPost]
        public List<CINTScore> CINTScore(CINTScore objCINT)
        {
            return CINT.CINTScore(objCINT);
        }
    }
}