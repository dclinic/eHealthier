using System;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AttributeRouting.Web.Http;
using Newtonsoft.Json;
using BLL;

namespace API.Controllers
{
    public class EpromsController : ApiController
    {
        #region AdminEprom

        [System.Web.Http.HttpPost]
        public int Post(Survey_model survey)
        {
            if (survey.UserName != null && survey.UserName != "")
            {
                Guid? userId = Providers.GetUserIdByUserName(survey.UserName);
                return SurveyClass.CreateSurvey(survey, (Guid)userId);
            }
            return 0;
        }

        [System.Web.Http.HttpGet]
        public string GetEpromsBySurveyMonkey()
        {
            return JsonConvert.SerializeObject(SurveyClass.GetEpromsBySurveyMonkey());
        }

        [System.Web.Http.HttpGet]
        public string GetSurveyMonkeyCollectorID(string surveyId)
        {
            return SurveyClass.GetSurveyMonkeyCollectorIDBySurveyID(surveyId);
        }

        [System.Web.Http.HttpGet]
        public string GetSurveyMonkeyCollectorDetails_CollectorID(string CollectorId)
        {
            return SurveyClass.GetSurveyMonkeyCollectorDetails_CollectorID(CollectorId);
        }

        [System.Web.Http.HttpGet]
        public string GetSurveyMonkey_SurveyDetails(string surveyId)
        {
            return SurveyClass.GetSurveyMonkey_SurveyDetails(surveyId);
        }

        [System.Web.Http.HttpGet]
        public string GetSurveyMonkeyResponseBy_CollectorID(string collectorId)
        {
            return SurveyClass.GetSurveyMonkeyResponseBy_CollectorID(collectorId);
        }

        [System.Web.Http.HttpGet]
        public string GetSurveyTypes()
        {
            return JsonConvert.SerializeObject(SurveyTypeClass.GetSurveyTypes());
        }

        [System.Web.Http.HttpGet]
        public string GetSurveyById(short id)
        {
            return JsonConvert.SerializeObject(SurveyClass.GetSurveyById(id));
        }

        [System.Web.Http.HttpGet]
        public string GetSurveyByExternalID(string ExternalId)
        {
            return JsonConvert.SerializeObject(SurveyClass.GetEntityBy_ExternalID_Survey(ExternalId));
        }

        [System.Web.Http.HttpGet]
        public string GetSurveySearch(int? StartIndex = -1, int? EndIndex = -1, string SearchString = null, bool? IsActive = null)
        {
            int TotalCount = 0;

            return JsonConvert.SerializeObject(SurveyClass.SearchFilterSurvey(TotalCount, StartIndex, EndIndex, SearchString, IsActive));
        }

        [System.Web.Http.HttpPost]
        public void Put(Survey_model survey)
        {
            SurveyClass.UpdateSurvey(survey);
        }

        [System.Web.Http.HttpPost]
        public string UpdateStatus()
        {
            string Ids = Request.Headers.GetValues("Ids").FirstOrDefault();
            bool status = Convert.ToBoolean(Request.Headers.GetValues("Status").FirstOrDefault());

            return SurveyClass.UpdateSurveyIsActiveStatus(Ids, status);
        }

        [System.Web.Http.HttpPost]
        public string UpdateSurveyFileName(int Id, string FileName)
        {
            return SurveyClass.UpdateSurveyFileName_Survey(Id, FileName);
        }

        [System.Web.Http.HttpDelete]
        public string Delete(short id)
        {
            return SurveyClass.DeleteSurveyById(id);
        }


        [System.Web.Http.HttpDelete]
        public string DeleteMultiple()
        {
            string Ids = Request.Headers.GetValues("Ids").FirstOrDefault();
            return SurveyClass.DeleteMultipleSurvey(Ids);
        }

        [System.Web.Http.HttpGet]
        public int getDays(int SurveyID)
        {
            return SetDefaultTime.getDays(SurveyID);
        }

        [System.Web.Http.HttpPost]
        public string updateDays(int SurveyID, int Days)
        {
            return SetDefaultTime.updateDays(SurveyID, Days);
        }

        #endregion AdminEprom

        #region PatientEprom

        [System.Web.Http.HttpGet]
        public string GetSurevy_By_SurveyCategoryID(short id, short subId)
        {
            return JsonConvert.SerializeObject(SurveyClass.GetSurevy_By_SurveyCategoryID(id, subId));
        }

        [System.Web.Http.HttpPost]
        public string AddPatientSurvey(PatientSurvey_model patientSurvey)
        {
            return PatientSurveyClass.CreatePatientSurvey(patientSurvey);
        }

        [System.Web.Http.HttpPost]
        public string AddPatientSurveyEProms(string organizationID, string practiceID, string providerID, string patientID, string surveyID)
        {
            Guid GOrganizationId = string.IsNullOrEmpty(organizationID) || organizationID == "undefined" || organizationID == "-1" ? Guid.Empty : new Guid(organizationID);
            Guid GPracticeId = string.IsNullOrEmpty(practiceID) || practiceID == "undefined" || practiceID == "-1" ? Guid.Empty : new Guid(practiceID);
            Guid GProviderId = string.IsNullOrEmpty(providerID) || providerID == "undefined" || providerID == "-1" ? Guid.Empty : new Guid(providerID);
            Guid GPatientId = string.IsNullOrEmpty(patientID) || patientID == "undefined" || patientID == "-1" ? Guid.Empty : new Guid(patientID);

            return PatientSurveyClass.AddPatientSurveyEProms(GOrganizationId, GPracticeId, GProviderId, GPatientId, surveyID);
        }

        [System.Web.Http.HttpDelete]
        public string DeletePatientSurvey(string Id)
        {
            Guid patientSurveyId = new Guid(Id);
            return PatientSurveyClass.DeletePatientSurveyById(patientSurveyId);
        }

        [System.Web.Http.HttpGet]
        public string GetSurveyByPatient_Provider_Org_Practice_IDs(string PatientId, string ProviderId, string OrganizationId, string PracticeId, bool isAllPatient, bool isCompleted)
        {
            Guid GPatientId = string.IsNullOrEmpty(PatientId) || PatientId == "undefined" || PatientId == "-1" ? Guid.Empty : new Guid(PatientId);
            Guid GProviderId = string.IsNullOrEmpty(ProviderId) || ProviderId == "undefined" ? Guid.Empty : new Guid(ProviderId);
            Guid GOrganizationId = string.IsNullOrEmpty(OrganizationId) || OrganizationId == "undefined" ? Guid.Empty : new Guid(OrganizationId);
            Guid GPracticeId = string.IsNullOrEmpty(PracticeId) || PracticeId == "undefined" ? Guid.Empty : new Guid(PracticeId);

            return JsonConvert.SerializeObject(PatientSurveyClass.GetSurveyByPatient_Provider_Org_Practice_IDs(GPatientId, GProviderId, GOrganizationId, GPracticeId, isAllPatient, isCompleted));
        }

        [System.Web.Http.HttpGet]
        public string GetSurveyByPatient_Org_Practice_IDs(string PatientId, string ProviderID, string OrganizationId, string PracticeId, bool isAllPatient, bool isCompleted)
        {
            Guid GPatientId = string.IsNullOrEmpty(PatientId) || PatientId == "undefined" || PatientId == "-1" ? Guid.Empty : new Guid(PatientId);
            Guid GProviderID = string.IsNullOrEmpty(ProviderID) || ProviderID == "undefined" || ProviderID == "-1" ? Guid.Empty : new Guid(ProviderID);
            Guid GOrganizationId = string.IsNullOrEmpty(OrganizationId) || OrganizationId == "undefined" ? Guid.Empty : new Guid(OrganizationId);
            Guid GPracticeId = string.IsNullOrEmpty(PracticeId) || PracticeId == "undefined" ? Guid.Empty : new Guid(PracticeId);

            return JsonConvert.SerializeObject(PatientSurveyClass.GetSurveyByPatient_Org_Practice_IDs(GPatientId, GProviderID, GOrganizationId, GPracticeId, isAllPatient, isCompleted));
        }

        [System.Web.Http.HttpGet]
        public string GetSurveyByPatient_Provider_ID(string PatientId, string ProviderId, bool isCompleted)
        {
            Guid GPatientId = string.IsNullOrEmpty(PatientId) || PatientId == "undefined" ? Guid.Empty : new Guid(PatientId);
            Guid GProviderId = string.IsNullOrEmpty(ProviderId) || ProviderId == "undefined" ? Guid.Empty : new Guid(ProviderId);

            return JsonConvert.SerializeObject(PatientSurveyClass.GetPatientSurveyBy_patientId_ProviderId(GPatientId, GProviderId, isCompleted));
        }

        [System.Web.Http.HttpGet]
        public string GetPatientSurveyStatusBy_PatientId_ProviderId(Guid PatientId, Guid ProviderId)
        {
            return JsonConvert.SerializeObject(PatientSurveyStatusClass.GetPatientSurveyStatusBy_PatientId_ProviderId(PatientId, ProviderId, false));
        }

        [System.Web.Http.HttpGet]
        public string GetPatientSurveyStatusBy_PatientId(Guid PatientId, bool isArchive)
        {
            return JsonConvert.SerializeObject(PatientSurveyStatusClass.GetPatientSurveyStatusBy_PatientId_ProviderId(PatientId, Guid.Empty, isArchive));
        }
        #endregion PatientEprom

        #region ProviderEprom

        [System.Web.Http.HttpGet]
        public List<Pathway_model> GetPathwayList(string ProviderId, string PracticeID, string OrganizationId)
        {
            if (!string.IsNullOrEmpty(ProviderId))
            {
                Guid GuidProviderId = new Guid(ProviderId);
                return PatientSurveyClass.GetPathwayList(GuidProviderId);
            }
            else
            {
                return null;
            }
        }

        #endregion
    }
}
