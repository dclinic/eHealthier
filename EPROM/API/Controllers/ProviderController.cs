using System;
using System.Collections.Generic;
using System.Web.Http;
using BLL;
using Newtonsoft.Json;

namespace API.Controllers
{
    public class ProviderController : ApiController
    {
        [HttpGet]
        public List<Role_model> GetRole()
        {
            return Role.GetRole();
        }

        [HttpGet]
        public Guid? GetProviderIDByUserName(string UserName)
        {
            Guid userId = (Guid)Providers.GetUserIdByUserName(UserName);
            if (userId != Guid.Empty)
            {
                return Providers.GetProviderID(userId);
            }
            else
            {
                return Guid.Empty;
            }
        }

        public string Post(Provider_model objProvider)
        {
            return Providers.CreateProvider(objProvider);
        }

        public Guid? Put(Provider_model objProvider)
        {
            return Providers.UpdateProvider(objProvider);
        }

        public Provider_model Get(string UserName)
        {
            return Providers.GetProviderDetails(UserName);
        }

        [HttpGet]
        public List<ProviderType_model> GetProviderType()
        {
            return Providers.GetProviderTypes();
        }

        [HttpGet]
        public short? GetProviderTypeID(string UserName)
        {
            Guid? userId = Providers.GetUserIdByUserName(UserName);
            return Providers.GetProviderTypeID((Guid)userId);
        }

        [HttpPost]
        public string ChangeEmailVerificationStatus(string token)
        {
            string userName = HelperMethods.GetEntities().ValidateToken(token);
            if (!string.IsNullOrEmpty(userName))
            {
                HelperMethods.GetEntities().DeleteToken(userName, token, true);
            }
            return Providers.ChangeEmailVerificationStatus(userName).ToString();
        }

        [HttpPost]
        public string ChangeEmailVerificationStatusByUserName(string userName)
        {
            return Providers.ChangeEmailVerificationStatus(userName).ToString();
        }

        [HttpGet]
        public bool? GetEmailVerificationStatus(string UserName)
        {
            return Providers.GetEmailVerificationStatus(UserName);
        }

        [HttpGet]
        public List<Country_model> GetCountries()
        {
            return Addresses.GetCountries();
        }

        [HttpGet]
        public List<State_model> GetStates()
        {
            return Addresses.GetStates();
        }

        [HttpGet]
        public List<SecretQuestion_model> GetSecretQuestion()
        {
            return SecretQuestion.GetSecretQuestion();
        }

        [HttpGet]
        public string CheckSecretQuestionAnswer(string UserName, string QuestionId, string answer)
        {
            int SecretQuestionId = 0;
            if (QuestionId != null && QuestionId != "")
                SecretQuestionId = Convert.ToInt32(QuestionId);

            Guid? userId = Providers.GetUserIdByUserName(UserName);
            if (userId != Guid.Empty)
                return SecretQuestion.CheckSecretQuestionAnswer((Guid)userId, SecretQuestionId, answer);
            else
                return "Email is not correct";
        }

        [HttpGet]
        public bool CheckExistingUser(string UserName)
        {
            Guid? userId = Providers.GetUserIdByUserName(UserName);
            return UserDetails.CheckExistingUser(userId);
        }

        [HttpGet]
        public string GetSurveyMonkeydetailsForDashboard()
        {
            return JsonConvert.SerializeObject(SurveyClass.GetSurveyMonkeydetailsForDashboard());
        }

        [HttpGet]
        public string GetSurveyList()
        {
            return JsonConvert.SerializeObject(SurveyClass.GetSurveyList());
        }

        [HttpGet]
        public PatientSuggestions_model GetPatientSuggestionsbyPatientSurveyID(Guid PatientSurveyId)
        {
            return PatientSuggestionsClass.GetPatientSuggestionsbyPatientSurveyID(PatientSurveyId);
        }

        [HttpPost]
        public string CreatePatientSuggestion(PatientSuggestions_model suggestion)
        {
            return PatientSuggestionsClass.CreatePatientSuggestion(suggestion);
        }

        [HttpGet]
        public List<Organization_Model> GetOrganizationList()
        {
            return OrganizationsClass.GetOrganizationList();
        }

        [HttpPost]
        public string CreateProviderOrganization(ProviderOrganizations_model providerOrg)
        {
            return ProviderOrganizationClass.CreateProviderOrganization(providerOrg);
        }

        [HttpGet]
        public List<ProviderOrganizations_model> GetProviderOrganizationByProviderID(Guid ProviderId)
        {
            return ProviderOrganizationClass.GetProviderOrganizationByProviderID(ProviderId);
        }

        [HttpDelete]
        public string DeleteProviderOrganization(Guid Id, Guid OrganizationId)
        {
            return ProviderOrganizationClass.DeleteProviderOrganization(Id, OrganizationId);
        }

        [HttpGet]
        public bool CheckExistingMedicure(string MedicareNumber)
        {
            return Patients.CheckExistingMedicure(MedicareNumber);
        }

        [HttpGet]
        public bool CheckExistingIHINumber(string IHINumber)
        {
            return Patients.CheckExistingIHINumber(IHINumber);
        }

        [HttpGet]
        public string GetProviderIdFromUserId(string UserName)
        {
            return ProviderOrganizationClass.GetProviderIdFromUserName(UserName);
        }

        [HttpGet]
        public List<ProviderPractice_model> GetProviderPracticeByProviderId(string ProviderId)
        {
            return ProviderPractice.GetProviderPracticeByProviderId(ProviderId);
        }

        [HttpDelete]
        public string DeleteProviderPractice(string ProviderPracticeId, Guid PracticeId)
        {
            return ProviderPractice.DeleteProviderPractice(ProviderPracticeId, PracticeId);
        }

        [HttpPost]
        public string CreateProviderPractice(ProviderPractice_model Practice)
        {
            return ProviderPractice.CreateProviderPractice(Practice);
        }

        [HttpPost]
        public bool SaveProviderPracticeRole(ProviderPracticeRole_custom_model ProviderPracticeRoles)
        {
            return ProviderPracticeRoleClass.SaveProviderPracticeRole(ProviderPracticeRoles);
        }

        [HttpPost]
        public bool SaveProviderPatientThirdPartyApp(ProviderPatientThirdPartyApp_model model)
        {
            return ProviderPatientThirdPartyAppClass.SaveProviderPatientThirdPartyApp(model);
        }

        [HttpGet]
        public List<ProviderPatientThirdPartyApp_model> GetProviderPatientThirdPartyApp(Guid PatientID, Guid OrganizationID, Guid PracticeID, Guid ProviderID, int SurveyID)
        {
            return ProviderPatientThirdPartyAppClass.GetProviderPatientThirdPartyApp(PatientID, OrganizationID, PracticeID, ProviderID, SurveyID);
        }

        [HttpDelete]
        public bool DeletePatientThirdPartyApp(int ThirdPartyAppId, Guid ProviderID, Guid OrganizationID, Guid PracticeID, Guid PatientID, int SurveyID)
        {
            return ProviderPatientThirdPartyAppClass.DeletePatientThirdPartyApp(ThirdPartyAppId, ProviderID, OrganizationID, PracticeID, PatientID, SurveyID);
        }

        [HttpGet]
        public List<Salutation_Model> GetSalutationList()
        {
            return SalutationClass.GetSalutationList();
        }

        [HttpGet]
        public Patient_model SearchPatientDetail(string IHINumber, string MedicareNumber, Guid ProviderId, Guid OrganizationId, Guid PracticeId)
        {
            return Patients.SearchPatientDetail(IHINumber, MedicareNumber, ProviderId, OrganizationId, PracticeId);
        }

        [HttpPost]
        public string CreatePatientProvider(PatientProviders_model patient)
        {
            return Patients.CreatePatientProvider(patient);
        }
    }
}