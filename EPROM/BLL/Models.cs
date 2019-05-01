using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class Score
    {
        public string Title { get; set; }
        public double Value { get; set; }
    }

    public class Factor
    {
        public string Title { get; set; }
        public string Value { get; set; }
    }

    #region Role_model

    public class Role_model
    {
        public int RoleID { get; set; }
        public string RoleName { get; set; }
    }
    #endregion Role_model

    #region User

    #region Menu
    public class Menu_model
    {
        public int ID { get; set; }
        public string CategoryName { get; set; }
        public bool? IsActive { get; set; }
    }
    #endregion Menu

    #region usp_select_ItemsByCategoryId_Result_model
    public class usp_select_ItemsByCategoryId_Result_model
    {
        public long? RowNo { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int RestaurantItemId { get; set; }
        public string ItemName { get; set; }
        public decimal? Price { get; set; }
        public string Label { get; set; }
        public bool? IsActive { get; set; }
        public string Description { get; set; }
        public Guid RestaurantItemImageID { get; set; }
        public Guid ImageId { get; set; }
        public short? DisplayOrder { get; set; }
        public string ImagePath { get; set; }
        public string Alt { get; set; }
        public string RestaurantName { get; set; }
        public int? RestaurantId { get; set; }
    }
    #endregion usp_select_ItemsByCategoryId_Result_model

    #region usp_select_ResturantItemIDList_ByAllFilterOptions_Result_model
    public class usp_select_ResturantItemIDList_ByAllFilterOptions_Result_model
    {
        public long? RowNo { get; set; }
        public int RestaurantItemId { get; set; }
        public string ItemName { get; set; }
        public decimal? Price { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
        public int? CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int? RestaurantId { get; set; }
        public string RestaurantName { get; set; }
        public short? DisplayOrder { get; set; }
        public System.Guid ImageID { get; set; }
        public string ImagePath { get; set; }
        public string Alt { get; set; }
    }
    #endregion usp_select_ResturantItemIDList_ByAllFilterOptions_Result_model

    #region usp_select_ResturantItemIDList_ByAllFilterOptions_Custom_model
    public class usp_select_ResturantItemIDList_ByAllFilterOptions_Custom_model
    {
        public List<usp_select_ResturantItemIDList_ByAllFilterOptions_Result_model> RestaurantItemsList { get; set; }
        public int TotalCount { get; set; }
    }
    #endregion usp_select_ResturantItemIDList_ByAllFilterOptions_Custom_model

    public class User_model
    {
        public User_model() { }

        public Guid ID { get; set; }
        public string Email { get; set; }
        public bool? EmailConfirmed { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public string PhoneNumber { get; set; }
        public bool? PhoneNumberConfirmed { get; set; }
        public bool? TwoFactorEnabled { get; set; }
        public DateTime? LockoutEndDateUtc { get; set; }
        public string LockoutEnabled { get; set; }
        public int? AccessFailedCount { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public DateTime? DOB { get; set; }
        public string Gender { get; set; }
        public string OrganizationName { get; set; }
    }

    #endregion User

    #region Customer Module

    #region Customer_model
    public class Customer_model
    {
        public int ID { get; set; }
        public int UserId { get; set; }
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public string ContactNo { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
    #endregion Customer_model

    #region usp_search_CustomerByFilter_Result_model
    public class usp_search_CustomerByFilter_Result_model
    {
        public long? RowNo { get; set; }
        public int ID { get; set; }
        public int? UserId { get; set; }
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public string ContactNo { get; set; }
        public bool? IsActive { get; set; }
        public System.DateTime? CreatedDate { get; set; }
    }
    #endregion usp_search_CustomerByFilter_Result_model

    #region usp_search_CustomerByFilterCustom_model
    public class usp_search_CustomerByFilterCustom_model
    {
        public List<usp_search_CustomerByFilter_Result_model> CustomerSearchFilterList { get; set; }
        public int TotalCount { get; set; }
    }
    #endregion usp_search_CustomerByFilterCustom_model

    #endregion Customer Module

    #region PatientCategory Module

    #region PatientCategory_model

    public class PatientCategory_model
    {
        public short ID { get; set; }
        public string PatientCategoryName { get; set; }
        public string Description { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? ModifiedBy { get; set; }
    }

    #endregion PatientCategory_model

    #region usp_search_PatientCategoryByFilter_Result_model
    public class usp_search_PatientCategoryByFilter_Result_model
    {
        public long? RowNo { get; set; }
        public int ID { get; set; }
        public string PatientCategoryName { get; set; }
        public string Description { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
    #endregion usp_search_PatientCategoryByFilter_Result_model

    #region usp_search_PatientCategoryByFilterCustom_model
    public class usp_search_PatientCategoryByFilterCustom_model
    {
        public List<usp_search_PatientCategoryByFilter_Result_model> PatientCategorySearchFilterList { get; set; }
        public int TotalCount { get; set; }
    }
    #endregion usp_search_PatientCategoryByFilterCustom_model

    #endregion PatientCategory Module

    #region SurveyCategory Module

    #region SurveyCategory_model
    public class SurveyCategory_model
    {
        public short ID { get; set; }
        public string SurvayCategoryName { get; set; }
        public string Description { get; set; }
        public short? ParentSurveyCategoryID { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? ModifiedBy { get; set; }
        public List<SurveyCategory_model> SubCategoryList { get; set; }
    }

    #endregion SurveyCategory_model

    #region usp_search_SurveyCategoryByFilter_Result_model
    public class usp_search_SurveyCategoryByFilter_Result_model
    {
        public long? RowNo { get; set; }
        public int ID { get; set; }
        public string SurvayCategoryName { get; set; }
        public short? ParentSurveyCategoryID { get; set; }
        public string Description { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
    #endregion usp_search_SurveyCategoryByFilter_Result_model

    #region usp_search_SurveyCategoryByFilterCustom_model
    public class usp_search_SurveyCategoryByFilterCustom_model
    {
        public List<usp_search_SurveyCategoryByFilter_Result_model> SurveyCategorySearchFilterList { get; set; }
        public int TotalCount { get; set; }
    }
    #endregion usp_search_SurveyCategoryByFilterCustom_model

    #endregion Category Module

    #region SurveyTypes Module

    #region SurveyTypes_model
    public class SurveyTypes_model
    {
        public int ID { get; set; }
        public string SurvayTypeName { get; set; }
        public string Description { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
    #endregion SurveyTypes_model
    #endregion SurveyTypes Module

    #region ThirdPartyApp Module

    #region ThirdPartyApp_model

    public class ThirdPartyApp_model
    {
        public int ID { get; set; }
        public string AppName { get; set; }
        public string URL { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string MobileNo { get; set; }
        public string PhoneNo { get; set; }
        public short? SurveyCategoryID { get; set; }
        public short? SurveySubCategoryID { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? ModifiedBy { get; set; }
    }

    #endregion ThirdPartyApp_model

    #region usp_search_ThirdPartyAppByFilter_Result_model
    public class usp_search_ThirdPartyAppByFilter_Result_model
    {
        public long? RowNo { get; set; }
        public int ID { get; set; }
        public string AppName { get; set; }
        public string URL { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string MobileNo { get; set; }
        public string PhoneNo { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
    #endregion usp_search_ThirdPartyAppByFilter_Result_model

    #region usp_search_ThirdPartyAppByFilterCustom_model
    public class usp_search_ThirdPartyAppByFilterCustom_model
    {
        public List<usp_search_ThirdPartyAppByFilter_Result_model> ThirdPartyAppSearchFilterList { get; set; }
        public int TotalCount { get; set; }
    }
    #endregion usp_search_ThirdPartyAppByFilterCustom_model

    #endregion ThirdPartyApp Module

    #region Survey Module

    #region Survey_model

    public class Survey_model
    {
        public int SurveyID { get; set; }
        public string Title { get; set; }
        public string ExternalTitle { get; set; }
        public string ExternalID { get; set; }
        public string CollectorID { get; set; }
        public string URL { get; set; }
        public string ContentCode { get; set; }
        public Guid? CreatedByUserID { get; set; }
        public short? SurveyCategoryID { get; set; }
        public short? SurveySubCategoryID { get; set; }
        public short? SurveyTypeID { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Language { get; set; }
        public bool? IsPublish { get; set; }
        public string Description { get; set; }
        public string FileName { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? ModifiedBy { get; set; }
        public string UserName { get; set; }

        public decimal? NormValue { get; set; }
        public int? Days { get; set; }
    }
    #endregion Survey_model

    #region usp_search_SurveyByFilter_Result_model
    public class usp_search_SurveyByFilter_Result_model
    {
        public long? RowNo { get; set; }
        public int ID { get; set; }
        public string Title { get; set; }
        public string ExternalTitle { get; set; }
        public string ExternalID { get; set; }
        public string CollectorID { get; set; }
        public string URL { get; set; }
        public string ContentCode { get; set; }
        public Guid? CreatedByUserID { get; set; }
        public short? SurveyCategoryID { get; set; }
        public string SurvayCategoryName { get; set; }
        public short? SurveyTypeID { get; set; }
        public string SurvayTypeName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Language { get; set; }
        public bool? IsPublish { get; set; }
        public string Description { get; set; }
        public string FileName { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? ModifiedBy { get; set; }

        public int? Days { get; set; }
    }
    #endregion usp_search_SurveyByFilter_Result_model

    #region usp_search_SurveyByFilterCustom_model
    public class usp_search_SurveyByFilterCustom_model
    {
        public List<usp_search_SurveyByFilter_Result_model> SurveySearchFilterList { get; set; }
        public int TotalCount { get; set; }
    }
    #endregion usp_search_SurveyByFilterCustom_model
    #endregion Survey Module

    #region SystemFlags Module

    #region SystemFlags_model

    public class SystemFlags_model
    {
        public int ID { get; set; }
        public string SystemFlagName { get; set; }
        public int FlagGroupID { get; set; }
        public string DefaultValue { get; set; }
        public string Value { get; set; }
        public int? DisplayOrder { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
    #endregion SystemFlags_model

    #region usp_search_SystemFlagByFilter_Result_model
    public class usp_search_SystemFlagByFilter_Result_model
    {
        public long? RowNo { get; set; }
        public int ID { get; set; }
        public string SystemFlagName { get; set; }
        public int FlagGroupID { get; set; }
        public string DefaultValue { get; set; }
        public string Value { get; set; }
        public int? DisplayOrder { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
    #endregion usp_search_SystemFlagByFilter_Result_model

    #region usp_search_SystemFlagByFilterCustom_model
    public class usp_search_SystemFlagByFilterCustom_model
    {
        public List<usp_search_SystemFlagByFilter_Result_model> SystemFlagSearchFilterList { get; set; }
        public int TotalCount { get; set; }
    }
    #endregion usp_search_SystemFlagByFilterCustom_model

    #endregion  SystemFlags Module

    #region FlagGroup Module

    #region FlagGroup_model

    public class FlagGroup_model
    {
        public int ID { get; set; }
        public string FlagGroupName { get; set; }
        public string Description { get; set; }
        public int? DisplayOrder { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDelete { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
    #endregion FlagGroup_model

    #region usp_search_FlagGroupByFilter_Result_model
    public class usp_search_FlagGroupByFilter_Result_model
    {
        public long? RowNo { get; set; }
        public int ID { get; set; }
        public string FlagGroupName { get; set; }
        public string Description { get; set; }
        public int? DisplayOrder { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDelete { get; set; }
        public System.DateTime? CreatedDate { get; set; }
        public System.DateTime? ModifiedDate { get; set; }
    }
    #endregion usp_search_FlagGroupByFilter_Result_model

    #region usp_search_FlagGroupByFilterCustom_model
    public class usp_search_FlagGroupByFilterCustom_model
    {
        public List<usp_search_FlagGroupByFilter_Result_model> FlagGroupSearchFilterList { get; set; }
        public int TotalCount { get; set; }
    }
    #endregion usp_search_FlagGroupByFilterCustom_model

    #endregion FlagGroup Module

    #region Organization Type Module

    public class OrganizationTypeModel
    {
        public int ID { get; set; }
        public string OrganizationType1 { get; set; }
        public Nullable<bool> IsActive { get; set; }
    }

    public class usp_search_OTByFilter_Result_model
    {
        public long? RowNo { get; set; }
        public int ID { get; set; }
        public string OrganizationType { get; set; }
        public bool? IsActive { get; set; }
    }

    public class usp_search_OTByFilterCustom_model
    {
        public List<usp_search_OTByFilter_Result_model> OTSearchFilterList { get; set; }
        public int TotalCount { get; set; }
    }

    #endregion Organization Type Module

    #region User Type Module

    public class UserTypeModel
    {
        public int ID { get; set; }
        public string UserType1 { get; set; }
        public Nullable<bool> IsActive { get; set; }
    }

    public class usp_search_UTByFilter_Result_model
    {
        public long? RowNo { get; set; }
        public int ID { get; set; }
        public string UserType { get; set; }
        public bool? IsActive { get; set; }
    }

    public class usp_search_UTByFilterCustom_model
    {
        public List<usp_search_UTByFilter_Result_model> UTSearchFilterList { get; set; }
        public int TotalCount { get; set; }
    }

    #endregion User Type Module

    #region Indicators Module

    #region Indicators_model

    public class Indicators_model
    {
        public short ID { get; set; }
        public string IndicatorName { get; set; }
        public string Description { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }

    #endregion Indicators_model

    #region usp_search_IndicatorsByFilter_Result_model
    public class usp_search_IndicatorsByFilter_Result_model
    {
        public long? RowNo { get; set; }
        public int ID { get; set; }
        public string IndicatorName { get; set; }
        public string Description { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
    #endregion usp_search_IndicatorsByFilter_Result_model

    #region usp_search_IndicatorsByFilterCustom_model
    public class usp_search_IndicatorsByFilterCustom_model
    {
        public List<usp_search_IndicatorsByFilter_Result_model> IndicatorsSearchFilterList { get; set; }
        public int TotalCount { get; set; }
    }
    #endregion usp_search_IndicatorsByFilterCustom_model

    #endregion Indicators Module

    #region Provides

    public class Provider_model
    {
        public Provider_model()
        {
            User = new User_model();
        }

        public Guid ID { get; set; }
        public Guid? UserID { get; set; }
        public string Code { get; set; }
        public string RoleName { get; set; }
        public short? ProviderTypeID { get; set; }
        public string MedicareProviderNumber { get; set; }
        public string HealthProviderIdentifier { get; set; }
        public int? ContactID { get; set; }
        public long? AddressID { get; set; }
        public int? SalutationID { get; set; }
        public string PHN { get; set; }
        public string LHD { get; set; }
        public string LHN { get; set; }
        public string Description { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? ModifiedBy { get; set; }
        public User_model User { get; set; }
        public Address_model Address { get; set; }
        public Contact_model Contact { get; set; }
        public Guid SecretQuestionIDMain { get; set; }
        public short SecretQuestionID { get; set; }
        public string Answer { get; set; }
    }

    public class ProviderType_model
    {
        public ProviderType_model() { }

        public short ID { get; set; }
        public string ProviderTypeName { get; set; }
        public string Description { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? ModifiedBy { get; set; }

    }

    public class Address_model
    {
        public Address_model() { }

        public long ID { get; set; }
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string Suburb { get; set; }
        public string State { get; set; }
        public int? StateID { get; set; }
        public string Country { get; set; }
        public short? CountryID { get; set; }
        public string ZipCode { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public string Description { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? ModifiedBy { get; set; }
        public Guid? UserId { get; set; }
    }

    public class State_model
    {
        public State_model() { }

        public int ID { get; set; }
        public string Code { get; set; }
        public string StateName { get; set; }
        public bool? IsActive { get; set; }
        public Guid? CreatedDate { get; set; }
        public Guid? ModifiedDate { get; set; }

    }

    public class Country_model
    {
        public Country_model() { }

        public short ID { get; set; }
        public string ISOCode { get; set; }
        public string ISOCode3 { get; set; }
        public string CountryName { get; set; }
        public string DialCode { get; set; }
        public int? Population { get; set; }
        public decimal? Area { get; set; }
        public string Description { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDelete { get; set; }
        public Guid? CreatedDate { get; set; }
        public Guid? ModifiedDate { get; set; }

    }

    public class Contact_model
    {
        public Contact_model() { }

        public int ID { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }
        public string Mobile2 { get; set; }
        public string FAX { get; set; }
        public string Pager { get; set; }
        public string Emergency { get; set; }
        public string EmailBusiness { get; set; }
        public string EmailPersonal { get; set; }
        public string Skype { get; set; }
        public string LinkedIN { get; set; }
        public string Website { get; set; }
        public string OtherFieldsJSON { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? ModifiedBy { get; set; }
        public Guid? UserId { get; set; }
    }

    #endregion Providers

    #region SecretQuestions

    public class SecretQuestion_model
    {
        public int ID { get; set; }
        public string Questions { get; set; }
        public bool? IsActive { get; set; }
        public string Description { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? ModifiedBy { get; set; }
    }

    #endregion

    #region UserSecretQuestions

    public class UserSecretQuestions_model
    {
        public int ID { get; set; }
        public Guid? UserID { get; set; }
        public int SecretQuestionID { get; set; }
        public string OtherQuestion { get; set; }
        public string Answer { get; set; }
        public string Description { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? ModifiedBy { get; set; }
    }

    #endregion

    #region patient

    public class Patient_model
    {
        public Patient_model()
        {
            User = new User_model();
            Address = new Address_model();
            Contact = new Contact_model();
            PatientCategory = new PatientCategories_model();
            PatientProvider = new PatientProviders_model();
        }

        public Guid ID { get; set; }
        public Guid UserID { get; set; }
        public string IHINumber { get; set; }
        public string MedicareNumber { get; set; }
        public string Code { get; set; }
        public int? ContactID { get; set; }
        public short? PatientCategoryID { get; set; }
        public long? AddressID { get; set; }
        public string Salutation { get; set; }
        public string Description { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? ModifiedBy { get; set; }
        public Guid ProviderId { get; set; }
        public Guid PatientUserId { get; set; }
        public Guid SecretQuestionIDMain { get; set; }
        public short SecretQuestionID { get; set; }
        public string Answer { get; set; }
        public bool isDuplicateMedicareNo { get; set; }
        public Guid? OrganizationID { get; set; }
        public string OrganizationName { get; set; }
        public Guid? PracticeID { get; set; }
        public string PracticeName { get; set; }
        public bool IsPatientExist { get; set; }
        public string ProviderName { get; set; }
        public string ProviderEmail { get; set; }


        public string State { get; set; }
        public string Country { get; set; }

        public DateTime? UpdateDOB { get; set; }

        public Address_model Address { get; set; }
        public Contact_model Contact { get; set; }
        public PatientCategories_model PatientCategory { get; set; }
        public PatientProviders_model PatientProvider { get; set; }
        public User_model User { get; set; }
        public List<PatientOrgPractice_model> PatientOrgList { get; set; }
    }

    public class PatientProviders_model
    {
        public Guid ID { get; set; }
        public Guid ProviderID { get; set; }
        public Guid PatientID { get; set; }
        public Guid? OrganizationID { get; set; }
        public Guid? PracticeID { get; set; }
        public string Description { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? ModifiedBy { get; set; }
    }

    public class Provider_Patient_model
    {
        public Provider_Patient_model()
        {
            Patients = new Patient_model();
            Providers = new Provider_model();
        }

        public Guid ID { get; set; }
        public Guid ProviderID { get; set; }
        public Guid PatientID { get; set; }
        public Guid? OrganizationID { get; set; }
        public Guid? PracticeID { get; set; }
        public string Description { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? ModifiedBy { get; set; }
        public List<PatientSurvey_model> SurvayList { get; set; }

        public Patient_model Patients { get; set; }
        public Provider_model Providers { get; set; }
    }

    public class PatientCategories_model
    {
        public short ID { get; set; }
        public string PatientCategoryName { get; set; }
        public string Description { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? ModifiedBy { get; set; }
    }

    public class PatientOrgPractice_model
    {
        public Guid PatientID { get; set; }
        public Guid ProviderID { get; set; }
        public string ProviderName { get; set; }
        public Guid? OrganizationID { get; set; }
        public string OrganizationName { get; set; }
        public Guid? PracticeID { get; set; }
        public string PracticeName { get; set; }
    }

    #endregion patient

    #region CsvPatient

    public class CsvPatient
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string DOB { get; set; }
        public string Gender { get; set; }
        public string Suburb { get; set; }
        public string PostalCode { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string PrimaryPhone { get; set; }
        public string Email { get; set; }
        public string MedicareNumber { get; set; }
        public string IHINumber { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string SecurityQuestion { get; set; }
        public string Answer { get; set; }
        public string OrganizationName { get; set; }
        public string PracticeName { get; set; }
        public string Salutation { get; set; }
        public bool isAssociate { get; set; }
    }

    #endregion CsvPatient

    #region patientSurvey

    public class PatientSurvey_model
    {
        public Guid PatientSurveyStatusID { get; set; }
        public Guid ID { get; set; }
        public Guid? PatientID { get; set; }
        public int? SurveyID { get; set; }
        public Guid? ProviderID { get; set; }
        public string ProviderName { get; set; }
        public Guid? OrganizationID { get; set; }
        public string OrganizationName { get; set; }
        public Guid? PracticeID { get; set; }
        public string PracticeName { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ExternalTitle { get; set; }
        public string ExternalID { get; set; }
        public string ContentCode { get; set; }
        public string CollectorID { get; set; }
        public string Email { get; set; }
        public string Score { get; set; }
        public string Status { get; set; }
        public bool isSurveyValid { get; set; }
        public bool isReassign { get; set; }
        public List<PatientSurvey_model> surveyList { get; set; }
        public bool IsSurveyWithValidDate { get; set; }
        public User_model User { get; set; }
        public Patient_model Patient { get; set; }
        public Guid? PatientSurvey_Pathway_PatientSurveyStatus_ID { get; set; }
        public Guid? PathwayID { get; set; }
        public bool? IsSend { get; set; }

    }

    #endregion patientSurvey

    #region PatientIndicators Module

    #region PatientIndicators_model

    public class PatientIndicators_model
    {
        public int ID { get; set; }
        public Guid PatientID { get; set; }
        public short IndicatorID { get; set; }
        public string IndicatorName { get; set; }
        public string Unit { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? Frequency { get; set; }
        public string Goal { get; set; }
        public string Comments { get; set; }
        public string Status { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? ModifiedBy { get; set; }
        public List<PatientIndicators_model> PatientIndicatorList { get; set; }
    }

    #endregion PatientIndicators_model

    #endregion PatientIndicators Module

    #region PatientSuggestions Module

    #region PatientSuggestions_model

    public class PatientSuggestions_model
    {
        public int ID { get; set; }
        public Guid PatientSurveyID { get; set; }
        public string Suggestions { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? ModifiedBy { get; set; }
    }

    #endregion PatientSuggestions_model

    #endregion PatientSuggestions Module

    #region PatientProvider_custom

    public class PatientProvider_custom
    {
        public Guid PatientSurveyID { get; set; }
        public DateTime? DueDate { get; set; }
        public string ProviderName { get; set; }
        public string OrganizationName { get; set; }
        public string PracticeName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string ZipCode { get; set; }
        public string TelephoneNo { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
    }

    #endregion PatientProvider_custom

    #region Organizations
    public class Organization_Model
    {
        public Guid ID { get; set; }
        public string OrganizationName { get; set; }
        public int? SalutationID { get; set; }
        public Guid? UserId { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Guid? ModifiedBy { get; set; }
    }

    public class Organizations_custom_model
    {
        public Guid ID { get; set; }
        public Guid? UserID { get; set; }
        public string RoleName { get; set; }
        public User_model User { get; set; }
        public Address_model Address { get; set; }
        public Contact_model Contact { get; set; }
        public string OrganizationName { get; set; }
        public int? SalutationID { get; set; }
        public Organization_Model Organization { get; set; }
    }
    #endregion Organizations

    #region ProviderOrganizations

    public class ProviderOrganizations_model
    {
        public Guid ID { get; set; }
        public Guid ProviderID { get; set; }
        public Guid? OrganizationID { get; set; }
        public string OrganizationName { get; set; }
        public string Designation { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Description { get; set; }
        public string UserName { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? ModifiedBy { get; set; }
        public List<ProviderPractice_model> ProviderPracticeList { get; set; }
        public List<Practice_Model> PracticeList { get; set; }
    }

    #endregion ProviderOrganizations

    #region PatientSurveyStatus

    public class PatientSurveyStatus_model
    {
        public Guid ID { get; set; }
        public Guid? PatientSurveyId { get; set; }
        public string Email { get; set; }
        public string Status { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string Score { get; set; }
        public string ExternalTitle { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int SurveyID { get; set; }
        public decimal? NormValue { get; set; }
    }
    #endregion PatientSurveyStatus

    #region Practice
    public class Practice_Model
    {
        public Guid ID { get; set; }
        public string PracticeName { get; set; }
        public string MedicareNumber { get; set; }
        public bool IsActive { get; set; }
        public int? SalutationID { get; set; }
        public Guid? UserId { get; set; }
        public Guid? OrganizationId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Guid? ModifiedBy { get; set; }
        public User_model User { get; set; }
        public Contact_model Contact { get; set; }
        public Address_model Address { get; set; }
    }

    public class Practice_custom_model
    {
        public Guid ID { get; set; }
        public string CurrentUserName { get; set; }
        public Guid? OrganizationID { get; set; }
        public string RoleName { get; set; }
        public User_model User { get; set; }
        public Address_model Address { get; set; }
        public Contact_model Contact { get; set; }
        public Practice_Model Practice { get; set; }
        public List<Practice_Model> PractitionerList { get; set; }
    }
    #endregion Practice

    #region PracticeRole
    public class PracticeRole_Model
    {
        public int RoleID { get; set; }
        public string RoleName { get; set; }
        public bool? IsActive { get; set; }
        public Guid? PracticeId { get; set; }
        public Guid? OrganizationId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? UserId { get; set; }
    }
    #endregion PracticeRole

    #region ProviderPractice

    public class ProviderPractice_model
    {
        public Guid Id { get; set; }
        public string PracticeName { get; set; }
        public Guid ProviderID { get; set; }
        public Guid PracticeId { get; set; }
        public Guid ProviderOrganizationId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? ModifiedBy { get; set; }
        public List<PracticeRole_Model> PracticeRoleList { get; set; }
        public List<ProviderPracticeRole_model> ProviderPracticeRoleList { get; set; }
    }

    #endregion ProviderPractice

    #region ProviderPracticeRole_model

    public class ProviderPracticeRole_model
    {
        public int Id { get; set; }
        public int? RoleID { get; set; }
        public Guid? ProviderPracticeID { get; set; }
    }

    public class ProviderPracticeRole_custom_model
    {
        public List<ProviderPracticeRole_model> ProviderPracticeRoleList { get; set; }
        public Guid ProviderPracticeId { get; set; }
    }
    #endregion ProviderPracticeRole_model

    #region ProviderPatientThirdPartyApp

    public class ProviderPatientThirdPartyApp_model
    {
        public int ID { get; set; }
        public string AppName { get; set; }
        public string URL { get; set; }
        public int ThirdPartyAppID { get; set; }
        public Guid ProviderID { get; set; }
        public Guid PatientID { get; set; }
        public int SurveyID { get; set; }
        public Guid PatientSurveyID { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? ModifiedBy { get; set; }
        public Guid? OrganizationID { get; set; }
        public string OrganizationName { get; set; }
        public Guid? PracticeID { get; set; }
        public string PracticeName { get; set; }
    }

    #endregion ProviderPatientThirdPartyApp

    #region Salutation_Model

    public class Salutation_Model
    {
        public int ID { get; set; }
        public string SalutationName { get; set; }
        public bool? IsActive { get; set; }
    }

    #endregion Salutation_Model

    public class ResponseClass
    {
        public string answer_text { get; set; }
        public string question_title { get; set; }
    }

    public class ResponseCustomClass
    {
        public List<ResponseClass> responselist { get; set; }
        public string Eprom_title { get; set; }
    }

    #region Pathway Module


    #region usp_search_PathwayByFilter_Result_model
    public class usp_search_PathwayByFilter_Result_model
    {
        public long? RowNo { get; set; }
        public Guid ID { get; set; }
        public string PathwayName { get; set; }
        public string Description { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
    #endregion usp_search_PathwayByFilter_Result_model

    #region usp_search_PathwayByFilterCustom_model
    public class usp_search_PathwayByFilterCustom_model
    {
        public List<usp_search_PathwayByFilter_Result_model> PathwaySearchFilterList { get; set; }
        public int TotalCount { get; set; }
    }
    #endregion usp_search_PathwayByFilterCustom_model

    #region Pathway_model

    public class Pathway_model
    {
        public Guid ID { get; set; }
        public string PathwayName { get; set; }
        public string Description { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
    }

    #endregion Pathway_model


    #region PatientSurvey_Pathway_PatientSurveyStatus_model

    public class PatientSurvey_Pathway_PatientSurveyStatus_model
    {
        public Guid ID { get; set; }
        public Guid PatientSurveyID { get; set; }
        public Guid PathwayID { get; set; }
        public Guid? PatientSurveyStatusID { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
    }

    #endregion PatientSurvey_Pathway_PatientSurveyStatus_model


    #endregion Pathway Module

    public class ConsumerDetail
    {
        public string Firstname { get; set; }
        public string Middlename { get; set; }
        public string Lastname { get; set; }
        public Nullable<System.DateTime> DOB { get; set; }
        public string Gender { get; set; }
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string Suburb { get; set; }
        public string StateID { get; set; }
        public string Pincode { get; set; }
        public string Email { get; set; }
        public string IHI { get; set; }
        public string HCID { get; set; }
        public Nullable<long> ContactID { get; set; }


        public string country { get; set; }
        public string telecom { get; set; }


        public int SID { get; set; }
        public int CID { get; set; }
    }
}