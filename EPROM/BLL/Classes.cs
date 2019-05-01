using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Net;
using DAL;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using BAL;
using Common;
using System.IO;
using BLL;
using System.Globalization;
using System.Web.Script.Serialization;
using System.Data.Entity;
using static DAL.Entities;

namespace BLL
{
    public class SurveyMonkey_Page
    {
        public string position { get; set; }
        public string id { get; set; }
        public string href { get; set; }
        public string description { get; set; }
        public string title { get; set; }
    }

    public class SurveyMonkey
    {
        public string name { get; set; }
        public string title { get; set; }
        public string id { get; set; }
        public string href { get; set; }
        public string collectorID { get; set; }
        public int count { get; set; }
        public List<SurveyMonkey_Question> Questions { get; set; }
    }

    public class SurveyMonkey_Question
    {
        public string position { get; set; }
        public string href { get; set; }
        public string heading { get; set; }
        public string id { get; set; }
    }

    public static class Role
    {
        #region GetRole

        public static List<Role_model> GetRole()
        {
            List<Role_model> Role = HelperMethods.GetEntities().GetEntity_Role().Where(x => x.RoleName != "admin" && x.RoleName != "patient").ToList().AsEnumerable().Select(x =>
             new Role_model
             {
                 RoleID = x.RoleId,
                 RoleName = x.RoleName
             }).ToList();

            return Role;
        }

        #endregion GetRole
    }


    public static class SystemFlags
    {
        #region CreateSystemFlag

        public static int CreateSystemFlag(SystemFlags_model _SystemFlags)
        {
            var systemFlag = new SystemFlag
            {
                ID = _SystemFlags.ID,
                SystemFlagName = _SystemFlags.SystemFlagName,
                FlagGroupID = _SystemFlags.FlagGroupID,
                DefaultValue = _SystemFlags.DefaultValue,
                Value = _SystemFlags.Value,
                DisplayOrder = (_SystemFlags.DisplayOrder == null ? HelperMethods.GetEntities().GetEntityDisplayOrderAutoGenerate_SystemFlag() : _SystemFlags.DisplayOrder), //_SystemFlags.DisplayOrder,
                IsActive = _SystemFlags.IsActive,
                CreatedDate = Utilities.ConvertServerCurrentDateTimeToClientDateTime(DateTime.Now),
            };
            return HelperMethods.GetEntities().SaveEntity_SystemFlags(systemFlag);
        }

        public static int CheckDisplayOrderExistOrNot(int Display_Order)
        {
            int DisplayOrder = HelperMethods.GetEntities().GetEntityCheckDisplayOrderExistOrNot(Display_Order);
            if (DisplayOrder == 0)
            {
                return 0;
            }
            else
            {
                return DisplayOrder;
            }
        }

        #endregion CreateSystemFlag

        #region UpdateSystemFlag

        public static void UpdateSystemFlag(SystemFlags_model _Systemflag)
        {
            SystemFlag SystemFlag = HelperMethods.GetEntities().GetEntityById_SystemFlag((int)_Systemflag.ID);
            if (SystemFlag != null)
            {
                SystemFlag.SystemFlagName = _Systemflag.SystemFlagName;
                SystemFlag.FlagGroupID = _Systemflag.FlagGroupID;
                SystemFlag.DefaultValue = _Systemflag.DefaultValue;
                SystemFlag.Value = _Systemflag.Value;
                SystemFlag.DisplayOrder = _Systemflag.DisplayOrder;
                SystemFlag.IsActive = _Systemflag.IsActive;
                SystemFlag.ModifiedDate = Utilities.ConvertServerCurrentDateTimeToClientDateTime(DateTime.Now);
                HelperMethods.GetEntities().UpdateEntity_SystemFlag(SystemFlag);
            }
        }

        public static string UpdateSystemFlagIsActiveStatus(string Ids, bool Status)
        {
            string str = "";
            string[] values = Ids.Split(',');
            int length = values.Length;

            for (int i = 0; i < length; i++)
            {
                if (values[i] != "")
                {
                    int ID = Convert.ToInt32(values[i]);
                    str = HelperMethods.GetEntities().UpdateEntityIsActiveStatus_SystemFlag(ID, Status);
                }
            }
            return str;
        }

        #endregion UpdateSystemFlag

        #region DeleteSystemFlag

        public static string DeleteSystemFlagById(int id)
        {
            string str = "";
            List<FlagGroup> FlagGroupItem = HelperMethods.GetEntities().GetEntityListById_FlagGroups(id);
            if (FlagGroupItem.Count == 0)
            {
                HelperMethods.GetEntities().DeleteEntity_SystemFlag(id);
                str = "Successfully Deleted!";
            }
            return str;
        }

        public static string DeleteMultipleSystemFlag(string Ids)
        {
            string str = "";
            string[] values = Ids.Split(',');
            int length = values.Length;

            for (int i = 0; i < length; i++)
            {
                if (values[i] != "")
                {
                    int ID = Convert.ToInt32(values[i]);

                    List<FlagGroup> FlagGroupItem = HelperMethods.GetEntities().GetEntityListById_FlagGroups(ID);

                    if (FlagGroupItem.Count == 0)
                    {
                        HelperMethods.GetEntities().DeleteEntity_SystemFlag(ID);
                        str = "Successfully Deleted!";
                    }
                }
            }
            return str;
        }

        #endregion DeleteSystemFlag

        #region GetSystemFlag

        public static List<SystemFlags_model> GetSystemFlag()
        {
            List<SystemFlags_model> SystemFlag = HelperMethods.GetEntities().GetEntity_SystemFlag().ToList().AsEnumerable().Select(x =>
             new SystemFlags_model
             {
                 ID = x.ID,
                 SystemFlagName = x.SystemFlagName,
                 DisplayOrder = x.DisplayOrder,
                 DefaultValue = x.DefaultValue,
                 Value = x.Value,
                 CreatedDate = x.CreatedDate,
                 FlagGroupID = x.FlagGroupID,
                 ModifiedDate = x.ModifiedDate,
                 IsActive = x.IsActive
             }).ToList();

            return SystemFlag;
        }

        #endregion GetSystemFlag

        #region SearchFilterSystemFlag
        public static usp_search_SystemFlagByFilterCustom_model SearchFilterSystemFlag(int TotalCount, int? StartIndex = -1, int? EndIndex = -1, string SearchString = null, bool? IsActive = null)
        {
            if (SearchString == "undefined")
                SearchString = null;

            List<usp_search_SystemFlagByFilter_Result> list = HelperMethods.GetEntities().GetEntityBySearchFilter_SystemFlag(out TotalCount, StartIndex, EndIndex, SearchString, IsActive).ToList();

            var objList = list.ToList().AsEnumerable().Select(x =>
            new usp_search_SystemFlagByFilter_Result_model
            {
                RowNo = x.RowNo,
                ID = x.ID,
                SystemFlagName = x.SystemFlagName,
                DisplayOrder = x.DisplayOrder,
                DefaultValue = x.DefaultValue,
                Value = x.Value,
                CreatedDate = x.CreatedDate,
                FlagGroupID = x.FlagGroupID,
                ModifiedDate = x.ModifiedDate,
                IsActive = x.IsActive
            }).ToList();

            return (new usp_search_SystemFlagByFilterCustom_model { SystemFlagSearchFilterList = objList, TotalCount = TotalCount });
        }

        #endregion SearchFilterSystemFlag

        #region GetSystemFlagById

        public static SystemFlags_model GetSystemFlagById(int id)
        {
            SystemFlag SystemFlag = HelperMethods.GetEntities().GetEntityById_SystemFlag(id);
            try
            {
                return new SystemFlags_model
                {
                    ID = SystemFlag.ID,
                    SystemFlagName = SystemFlag.SystemFlagName,
                    FlagGroupID = SystemFlag.FlagGroupID,
                    IsActive = SystemFlag.IsActive,
                    DefaultValue = SystemFlag.DefaultValue,
                    Value = SystemFlag.Value,
                    DisplayOrder = SystemFlag.DisplayOrder,
                    CreatedDate = SystemFlag.CreatedDate,
                    ModifiedDate = SystemFlag.ModifiedDate
                };
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        #endregion GetSystemFlagById

        #region GetSystemFlagById

        public static List<SystemFlags_model> GetEntityById_FlagGroupId(int FlagGroupId)
        {
            List<SystemFlags_model> SystemFlag = new List<SystemFlags_model>();
            try
            {
                var obj = HelperMethods.GetEntities().GetEntitySystemFlag_ByFlagGroupId(FlagGroupId);
                SystemFlag = HelperMethods.GetEntities().GetEntity_SystemFlag().ToList().AsEnumerable().Select(x =>
                            new SystemFlags_model
                            {
                                ID = x.ID,
                                SystemFlagName = x.SystemFlagName,
                                DisplayOrder = x.DisplayOrder,
                                DefaultValue = x.DefaultValue,
                                Value = x.Value,
                                CreatedDate = x.CreatedDate,
                                FlagGroupID = x.FlagGroupID,
                                ModifiedDate = x.ModifiedDate,
                                IsActive = x.IsActive
                            }).ToList();

                return SystemFlag;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        #endregion GetSystemFlagById

    }

    public static class FlagGroups
    {
        #region CreateFlagGroups

        public static int CreateFlagGroup(FlagGroup_model _FlagGroup)
        {
            var fgroup = new FlagGroup
            {
                ID = _FlagGroup.ID,
                FlagGroupName = _FlagGroup.FlagGroupName,
                Description = _FlagGroup.Description,
                DisplayOrder = (_FlagGroup.DisplayOrder == null ? HelperMethods.GetEntities().GetEntityDisplayOrderAutoGenerate_SystemFlag() : _FlagGroup.DisplayOrder),
                IsActive = _FlagGroup.IsActive,
                CreatedDate = Utilities.ConvertServerCurrentDateTimeToClientDateTime(DateTime.Now),
            };
            return HelperMethods.GetEntities().SaveEntity_FlagGroups(fgroup);
        }

        #endregion CreateSystemFlag

        #region DisplayOrderExists

        public static int CheckDisplayOrderExist_FlagGroup(int Display_Order)
        {
            int DisplayOrder = HelperMethods.GetEntities().GetEntity_CheckDisplayOrderExist_FlagGroups(Display_Order);
            if (DisplayOrder == 0)
            {
                return 0;
            }
            else
            {
                return DisplayOrder;
            }
        }

        #endregion DisplayOrderExists

        #region UpdateFlagGroups

        public static void UpdateFlagGroup(FlagGroup_model _FlagGroup)
        {
            FlagGroup flaggroup = HelperMethods.GetEntities().GetEntityById_FlagGroup((int)_FlagGroup.ID);
            flaggroup.FlagGroupName = _FlagGroup.FlagGroupName;
            flaggroup.Description = _FlagGroup.Description;
            flaggroup.DisplayOrder = _FlagGroup.DisplayOrder;
            flaggroup.IsActive = _FlagGroup.IsActive;
            flaggroup.ModifiedDate = Utilities.ConvertServerCurrentDateTimeToClientDateTime(DateTime.Now);
            HelperMethods.GetEntities().UpdateEntity_FlagGroups(flaggroup);
        }

        public static string UpdateIsActiveStatus_FlagGroup(string Ids, bool Status)
        {
            string str = "";
            string[] values = Ids.Split(',');
            int length = values.Length;

            for (int i = 0; i < length; i++)
            {
                if (values[i] != "")
                {
                    int ID = Convert.ToInt32(values[i]);
                    str = HelperMethods.GetEntities().UpdateEntityIsActiveStatus_FlagGroup(ID, Status);
                }
            }
            return str;
        }

        #endregion UpdateFlagGroups

        #region DeleteFlagGroups

        public static string DeleteFlagGroupById(int id)
        {
            string str = "";
            List<FlagGroup> FlagGroupItem = HelperMethods.GetEntities().GetEntityListById_FlagGroups(id);
            SystemFlag objsystemflag = HelperMethods.GetEntities().GetEntityBySystemFlagGroupID_FlagGroup(id);
            if (objsystemflag == null)
            {
                if (FlagGroupItem.Count > 0)
                {
                    HelperMethods.GetEntities().DeleteEntity_FlagGroups(id);
                    str = "Flag Group successfully Deleted!";
                }
            }
            else
            {
                str = "Already in used";
            }
            return str;
        }

        public static string DeleteMultiple_FlagGroup(string Ids)
        {
            string str = "";
            string[] values = Ids.Split(',');
            int length = values.Length;

            for (int i = 0; i < length; i++)
            {
                if (values[i] != "")
                {
                    int ID = Convert.ToInt32(values[i]);

                    List<FlagGroup> FlagGroupItem = HelperMethods.GetEntities().GetEntityListById_FlagGroups(ID);

                    if (FlagGroupItem.Count == 0)
                    {
                        HelperMethods.GetEntities().DeleteEntity_FlagGroups(ID);
                        str = "Successfully Deleted!";
                    }
                }
            }
            return str;
        }

        #endregion DeleteFlagGroups

        #region GetFlagGroup

        public static List<FlagGroup_model> GetFlagGroup()
        {
            List<FlagGroup_model> SystemFlag = HelperMethods.GetEntities().GetEntity_FlagGroup().ToList().AsEnumerable().Select(x =>
             new FlagGroup_model
             {
                 ID = x.ID,
                 FlagGroupName = x.FlagGroupName,
                 DisplayOrder = x.DisplayOrder,
                 Description = x.Description,
                 IsDelete = x.IsDelete,
                 CreatedDate = x.CreatedDate,
                 ModifiedDate = x.ModifiedDate,
                 IsActive = x.IsActive
             }).ToList();

            return SystemFlag;
        }

        #endregion GetFlagGroup

        #region GetFlagGroupById

        public static FlagGroup_model GetFlagGroupById(int id)
        {
            FlagGroup FlagGroup = HelperMethods.GetEntities().GetEntityById_FlagGroup(id);
            try
            {
                return new FlagGroup_model
                {
                    ID = FlagGroup.ID,
                    FlagGroupName = FlagGroup.FlagGroupName,
                    Description = FlagGroup.Description,
                    IsActive = FlagGroup.IsActive,
                    IsDelete = FlagGroup.IsDelete,
                    DisplayOrder = FlagGroup.DisplayOrder,
                    CreatedDate = FlagGroup.CreatedDate,
                    ModifiedDate = FlagGroup.ModifiedDate
                };
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        #endregion GetFlagGroupById

        #region SearchFilterFlagGroup

        public static usp_search_FlagGroupByFilterCustom_model SearchFilter_FlagGroup(int TotalCount, int? StartIndex = -1, int? EndIndex = -1, string SearchString = null, bool? IsActive = null)
        {
            if (SearchString == "undefined")
                SearchString = null;

            List<usp_search_FlagGroupByFilter_Result> list = HelperMethods.GetEntities().GetEntityBySearchFilter_FlagGroups(out TotalCount, StartIndex, EndIndex, SearchString, IsActive).ToList();

            var objList = list.ToList().AsEnumerable().Select(x =>
            new usp_search_FlagGroupByFilter_Result_model
            {
                RowNo = x.RowNo,
                ID = x.ID,
                FlagGroupName = x.FlagGroupName,
                Description = x.Description,
                DisplayOrder = x.DisplayOrder,
                CreatedDate = x.CreatedDate,
                ModifiedDate = x.ModifiedDate,
                IsActive = x.IsActive
            }).ToList();

            return (new usp_search_FlagGroupByFilterCustom_model { FlagGroupSearchFilterList = objList, TotalCount = TotalCount });
        }

        #endregion SearchFilterFlagGroup
    }

    public static class PatientCategoriers
    {
        #region CreatePatientCategory     

        public static int CreatePatientCategory(PatientCategory_model _category)
        {
            var category = new PatientCategory
            {
                PatientCategoryName = _category.PatientCategoryName,
                Description = _category.Description,
                IsActive = _category.IsActive,
                CreatedDate = Utilities.ConvertServerCurrentDateTimeToClientDateTime(DateTime.Now)
            };
            return HelperMethods.GetEntities().SaveEntity_PatientCategory(category);
        }

        #endregion CreatePatientCategory

        #region GetPatientCategoryById

        public static PatientCategory_model GetPatientCategoryById(short id)
        {
            PatientCategory category = HelperMethods.GetEntities().GetEntityById_PatientCategories(id);
            try
            {
                return new PatientCategory_model
                {
                    ID = category.ID,
                    PatientCategoryName = category.PatientCategoryName,
                    Description = category.Description,
                    IsActive = category.IsActive,
                    CreatedDate = Utilities.ConvertServerCurrentDateTimeToClientDateTime(DateTime.Now)
                };
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        #endregion GetPatientCategoryById

        #region GetPatientCategoryList

        public static List<PatientCategory_model> GetPatientCategoryList()
        {
            List<PatientCategory_model> category = HelperMethods.GetEntities().GetEntity_PatientCategoriesList().ToList().AsEnumerable().Select(x =>
             new PatientCategory_model
             {
                 ID = x.ID,
                 PatientCategoryName = x.PatientCategoryName,
                 Description = x.Description,
                 IsActive = x.IsActive,
                 CreatedDate = Utilities.ConvertServerCurrentDateTimeToClientDateTime(DateTime.Now)
             }).ToList();

            return category;
        }

        #endregion GetPatientCategoryList

        #region DeletePatientCategory
        public static string DeletePatientCategoryById(short id)
        {
            string str = "";
            bool isExistsinPatient = HelperMethods.GetEntities().CheckPatientCategoryExistsInPatients(id);
            if (!isExistsinPatient)
            {
                HelperMethods.GetEntities().DeleteEntity_PatientCategory(id);
                str = "Successfully Deleted!";
            }
            else
            {
                str = "Child Record Exists";
            }
            return str;
        }

        public static string DeleteMultiplePatientCategory(string Ids)
        {
            string str = "";
            string[] values = Ids.Split(',');
            int length = values.Length;

            for (int i = 0; i < length; i++)
            {
                if (values[i] != "")
                {
                    short ID = Convert.ToInt16(values[i]);

                    bool isExistsinPatient = HelperMethods.GetEntities().CheckPatientCategoryExistsInPatients(ID);
                    if (!isExistsinPatient)
                    {
                        HelperMethods.GetEntities().DeleteEntity_PatientCategory(ID);
                        str = "Successfully Deleted!";
                    }
                    else
                    {
                        str = "Child Record Exists";
                    }
                }
            }
            return str;
        }

        #endregion DeletePatientCategory

        #region UpdatePatientCategory

        public static void UpdatePatientCategory(PatientCategory_model _category)
        {
            PatientCategory category = HelperMethods.GetEntities().GetEntityById_PatientCategories((short)_category.ID);
            category.PatientCategoryName = _category.PatientCategoryName;
            category.Description = _category.Description;
            category.IsActive = _category.IsActive;

            HelperMethods.GetEntities().UpdateEntity_PatientCategory(category);
        }

        public static string UpdatePatientCategoryIsActiveStatus(string Ids, bool Status)
        {
            string str = "";
            string[] values = Ids.Split(',');
            int length = values.Length;

            for (int i = 0; i < length; i++)
            {
                if (values[i] != "")
                {
                    short ID = Convert.ToInt16(values[i]);
                    str = HelperMethods.GetEntities().UpdateEntityIsActiveStatus_PatientCategory(ID, Status);
                }
            }
            return str;
        }

        #endregion UpdatePatientCategory

        #region SearchFilterPatientCategory
        public static usp_search_PatientCategoryByFilterCustom_model SearchFilterPatientCategory(int TotalCount, int? StartIndex = -1, int? EndIndex = -1, string SearchString = null, bool? IsActive = null)
        {
            if (SearchString == "undefined")
                SearchString = null;

            List<usp_search_PatientCategoryByFilter_Result> list = HelperMethods.GetEntities().GetEntityBySearchFilter_PatientCategory(out TotalCount, StartIndex, EndIndex, SearchString, IsActive).ToList();

            var objList = list.ToList().AsEnumerable().Select(x =>
            new usp_search_PatientCategoryByFilter_Result_model
            {
                RowNo = x.RowNo,
                ID = x.ID,
                PatientCategoryName = x.PatientCategoryName,
                Description = x.Description,
                CreatedDate = x.CreatedDate,
                IsActive = x.IsActive
            }).ToList();

            return (new usp_search_PatientCategoryByFilterCustom_model { PatientCategorySearchFilterList = objList, TotalCount = TotalCount });

        }

        #endregion SearchFilterPatientCategory

    }

    public static class SurveyCategoriers
    {
        #region CreateSurveyCategory

        /// <summary>Creates a new Category.</summary>
        /// <param name="_gory">The Category to be created and saved to the database.</param>
        /// <returns>The id of the newly-created Category entity.</returns>
        /// 

        public static int CreateCategory(SurveyCategory_model _category)
        {
            var category = new SurveyCategory
            {
                SurvayCategoryName = _category.SurvayCategoryName,
                Description = _category.Description,
                ParentSurveyCategoryID = _category.ParentSurveyCategoryID,
                IsActive = _category.IsActive,
                CreatedDate = Utilities.ConvertServerCurrentDateTimeToClientDateTime(DateTime.Now)
            };
            return HelperMethods.GetEntities().SaveEntity_SurveyCategory(category);
        }

        #endregion CreateCategory

        #region GetSurveyCategoryById

        public static SurveyCategory_model GetSurveyCategoryById(short id)
        {
            SurveyCategory category = HelperMethods.GetEntities().GetEntityById_SurveyCategories(id);
            try
            {
                return new SurveyCategory_model
                {
                    ID = category.ID,
                    SurvayCategoryName = category.SurvayCategoryName,
                    Description = category.Description,
                    ParentSurveyCategoryID = category.ParentSurveyCategoryID,
                    IsActive = category.IsActive,
                    CreatedDate = Utilities.ConvertServerCurrentDateTimeToClientDateTime(DateTime.Now)
                };
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        public static List<SurveyCategory_model> GetSubCategoryById(short id)
        {
            List<SurveyCategory_model> category = HelperMethods.GetEntities().GetEntity_SubSurveyCategoriesList(id).ToList().AsEnumerable().Select(x =>
             new SurveyCategory_model
             {
                 ID = x.ID,
                 SurvayCategoryName = x.SurvayCategoryName,
                 Description = x.Description,
                 ParentSurveyCategoryID = x.ParentSurveyCategoryID,
                 IsActive = x.IsActive,
                 CreatedDate = Utilities.ConvertServerCurrentDateTimeToClientDateTime(DateTime.Now)
             }).ToList();

            return category;
        }


        #endregion GetSurveyCategoryById

        #region GetSurveyCategory
        public static List<SurveyCategory_model> GetCategory(short id)
        {
            List<SurveyCategory_model> category = HelperMethods.GetEntities().GetEntity_SurveyCategoriesList(id).ToList().AsEnumerable().Select(x =>
             new SurveyCategory_model
             {
                 ID = x.ID,
                 SurvayCategoryName = x.SurvayCategoryName,
                 Description = x.Description,
                 ParentSurveyCategoryID = x.ParentSurveyCategoryID,
                 IsActive = x.IsActive,
                 CreatedDate = Utilities.ConvertServerCurrentDateTimeToClientDateTime(DateTime.Now)
             }).ToList();

            return category;
        }

        public static List<SurveyCategory_model> GetCategoryList()
        {
            List<SurveyCategory_model> category = HelperMethods.GetEntities().GetEntity_CategoriesList().ToList().AsEnumerable().Select(x =>
             new SurveyCategory_model
             {
                 ID = x.ID,
                 SurvayCategoryName = x.SurvayCategoryName,
                 Description = x.Description,
                 ParentSurveyCategoryID = x.ParentSurveyCategoryID,
                 IsActive = x.IsActive,
                 CreatedDate = Utilities.ConvertServerCurrentDateTimeToClientDateTime(DateTime.Now),
                 SubCategoryList = SurveyCategoriers.GetSubCategoryById(x.ID),
             }).ToList();

            return category;
        }

        #endregion GetCategory

        #region DeleteCategory
        public static string DeleteSurveyCategoryById(short id)
        {
            string str = "";
            var isExist = HelperMethods.GetEntities().CheckSurveyCategoryExist_Survey(id);
            if (!isExist)
            {
                HelperMethods.GetEntities().DeleteEntity_SurveyCategory(id);
                str = "Successfully Deleted!";
            }
            else
            {
                str = "Can not delete.";
            }
            return str;
        }

        public static string DeleteMultipleSurveyCategory(string Ids)
        {
            string str = "";
            string[] values = Ids.Split(',');
            int length = values.Length;

            for (int i = 0; i < length; i++)
            {
                if (values[i] != "")
                {
                    short ID = Convert.ToInt16(values[i]);

                    //List<RestaurantItem> restaurantitem = HelperMethods.GetEntities().GetEntityByCategoryId_RestaurantItem(ID);

                    //if (restaurantitem.Count == 0)
                    //{
                    HelperMethods.GetEntities().DeleteEntity_SurveyCategory(ID);
                    str = "Successfully Deleted!";
                    //}
                    //else
                    //{
                    //    str = "Child Record Exists";
                    //}
                }
            }
            return str;
        }

        #endregion DeleteCategory

        #region UpdateCategory

        public static void UpdateSurveyCategory(SurveyCategory_model _category)
        {
            SurveyCategory category = HelperMethods.GetEntities().GetEntityById_SurveyCategories((short)_category.ID);
            category.SurvayCategoryName = _category.SurvayCategoryName;
            category.Description = _category.Description;
            category.ParentSurveyCategoryID = _category.ParentSurveyCategoryID;
            category.IsActive = _category.IsActive;

            HelperMethods.GetEntities().UpdateEntity_SurveyCategory(category);
        }

        public static string UpdateCategoryIsActiveStatus(string Ids, bool Status)
        {
            string str = "";
            string[] values = Ids.Split(',');
            int length = values.Length;

            for (int i = 0; i < length; i++)
            {
                if (values[i] != "")
                {
                    short ID = Convert.ToInt16(values[i]);
                    str = HelperMethods.GetEntities().UpdateEntityIsActiveStatus_SurveyCategory(ID, Status);
                }
            }
            return str;
        }

        #endregion UpdateCategory

        #region SearchFilterCategory
        public static usp_search_SurveyCategoryByFilterCustom_model SearchFilterCategory(int TotalCount, int? StartIndex = -1, int? EndIndex = -1, string SearchString = null, bool? IsActive = null)
        {
            if (SearchString == "undefined")
                SearchString = null;

            List<usp_search_SurveyCategoryByFilter_Result> list = HelperMethods.GetEntities().GetEntityBySearchFilter_SurveyCategory(out TotalCount, StartIndex, EndIndex, SearchString, IsActive).ToList();

            var objList = list.ToList().AsEnumerable().Select(x =>
            new usp_search_SurveyCategoryByFilter_Result_model
            {
                RowNo = x.RowNo,
                ID = x.ID,
                SurvayCategoryName = x.SurvayCategoryName,
                Description = x.Description,
                ParentSurveyCategoryID = x.ParentSurveyCategoryID,
                CreatedDate = x.CreatedDate,
                IsActive = x.IsActive
            }).ToList();

            return (new usp_search_SurveyCategoryByFilterCustom_model { SurveyCategorySearchFilterList = objList, TotalCount = TotalCount });

        }

        #endregion SearchFilterCategory

    }

    public static class OT
    {
        public static int CreateOT(OrganizationTypeModel _ot)
        {
            var ot = new OrganizationType
            {
                OrganizationType1 = _ot.OrganizationType1,
                IsActive = _ot.IsActive,
            };
            return HelperMethods.GetEntities().SaveEntity_OT(ot);
        }

        public static OrganizationTypeModel GetOTByID(int ID)
        {
            OrganizationType ot = HelperMethods.GetEntities().GetEntityById_OT(ID);
            try
            {
                return new OrganizationTypeModel
                {
                    ID = ot.ID,
                    OrganizationType1 = ot.OrganizationType1,
                    IsActive = ot.IsActive
                };
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        public static List<OrganizationTypeModel> GetOTList()
        {
            List<OrganizationTypeModel> category = HelperMethods.GetEntities().GetEntity_OTList().ToList().AsEnumerable().Select(x =>
             new OrganizationTypeModel
             {
                 ID = x.ID,
                 OrganizationType1 = x.OrganizationType1,
                 IsActive = x.IsActive,
             }).ToList();

            return category;
        }

        public static usp_search_OTByFilterCustom_model SearchFilterOT(int TotalCount, int? StartIndex = -1, int? EndIndex = -1, string SearchString = null, bool? IsActive = null)
        {
            if (SearchString == "undefined")
                SearchString = null;

            List<usp_search_OrganizationTypeByFilter_Result> list = HelperMethods.GetEntities().GetEntityBySearchFilter_OT(out TotalCount, StartIndex, EndIndex, SearchString, IsActive).ToList();

            var objList = list.ToList().AsEnumerable().Select(x =>
            new usp_search_OTByFilter_Result_model
            {
                RowNo = x.RowNo,
                ID = x.ID,
                OrganizationType = x.OrganizationType,
                IsActive = x.IsActive
            }).ToList();

            return (new usp_search_OTByFilterCustom_model { OTSearchFilterList = objList, TotalCount = TotalCount });
        }

        public static void UpdateOT(OrganizationTypeModel _ot)
        {
            OrganizationType ot = HelperMethods.GetEntities().GetEntityById_OTs(_ot.ID);
            ot.OrganizationType1 = _ot.OrganizationType1;
            ot.IsActive = _ot.IsActive;

            HelperMethods.GetEntities().UpdateEntity_OT(ot);
        }

        public static string UpdateOTIsActiveStatus(string Ids, bool Status)
        {
            string str = "";
            string[] values = Ids.Split(',');
            int length = values.Length;

            for (int i = 0; i < length; i++)
            {
                if (values[i] != "")
                {
                    short ID = Convert.ToInt16(values[i]);
                    str = HelperMethods.GetEntities().UpdateEntityIsActiveStatus_OT(ID, Status);
                }
            }
            return str;
        }

        public static string DeleteOTById(int ID)
        {
            string str = "";
            HelperMethods.GetEntities().DeleteEntity_OT(ID);
            str = "Successfully Deleted!";
            return str;
        }

        public static string DeleteMultipleOT(string Ids)
        {
            string str = "";
            string[] values = Ids.Split(',');
            int length = values.Length;

            for (int i = 0; i < length; i++)
            {
                if (values[i] != "")
                {
                    int ID = Convert.ToInt32(values[i]);
                    HelperMethods.GetEntities().DeleteEntity_OT(ID);
                    str = "Successfully Deleted!";
                }
            }
            return str;
        }
    }

    public static class UT
    {
        public static int CreateUT(UserTypeModel _ut)
        {
            var ut = new UserType
            {
                UserType1 = _ut.UserType1,
                IsActive = _ut.IsActive,
            };
            return HelperMethods.GetEntities().SaveEntity_UT(ut);
        }

        public static UserTypeModel GetUTByID(int ID)
        {
            UserType ut = HelperMethods.GetEntities().GetEntityById_UT(ID);
            try
            {
                return new UserTypeModel
                {
                    ID = ut.ID,
                    UserType1 = ut.UserType1,
                    IsActive = ut.IsActive,
                };
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        public static List<UserTypeModel> GetUTList()
        {
            List<UserTypeModel> category = HelperMethods.GetEntities().GetEntity_UTList().ToList().AsEnumerable().Select(x =>
             new UserTypeModel
             {
                 ID = x.ID,
                 UserType1 = x.UserType1,
                 IsActive = x.IsActive,
             }).ToList();

            return category;
        }

        public static usp_search_UTByFilterCustom_model SearchFilterUT(int TotalCount, int? StartIndex = -1, int? EndIndex = -1, string SearchString = null, bool? IsActive = null)
        {
            if (SearchString == "undefined")
                SearchString = null;

            List<usp_search_UserTypeByFilter_Result> list = HelperMethods.GetEntities().GetEntityBySearchFilter_UT(out TotalCount, StartIndex, EndIndex, SearchString, IsActive).ToList();

            var objList = list.ToList().AsEnumerable().Select(x =>
            new usp_search_UTByFilter_Result_model
            {
                RowNo = x.RowNo,
                ID = x.ID,
                UserType = x.UserType,
                IsActive = x.IsActive
            }).ToList();

            return (new usp_search_UTByFilterCustom_model { UTSearchFilterList = objList, TotalCount = TotalCount });
        }

        public static void UpdateUT(UserTypeModel _ut)
        {
            UserType ut = HelperMethods.GetEntities().GetEntityById_UTs(_ut.ID);
            ut.UserType1 = _ut.UserType1;
            ut.IsActive = _ut.IsActive;

            HelperMethods.GetEntities().UpdateEntity_UT(ut);
        }

        public static string UpdateUTIsActiveStatus(string Ids, bool Status)
        {
            string str = "";
            string[] values = Ids.Split(',');
            int length = values.Length;

            for (int i = 0; i < length; i++)
            {
                if (values[i] != "")
                {
                    short ID = Convert.ToInt16(values[i]);
                    str = HelperMethods.GetEntities().UpdateEntityIsActiveStatus_UT(ID, Status);
                }
            }
            return str;
        }

        public static string DeleteUTById(int ID)
        {
            string str = "";
            HelperMethods.GetEntities().DeleteEntity_UT(ID);
            str = "Successfully Deleted!";
            return str;
        }

        public static string DeleteMultipleUT(string Ids)
        {
            string str = "";
            string[] values = Ids.Split(',');
            int length = values.Length;

            for (int i = 0; i < length; i++)
            {
                if (values[i] != "")
                {
                    int ID = Convert.ToInt32(values[i]);
                    HelperMethods.GetEntities().DeleteEntity_UT(ID);
                    str = "Successfully Deleted!";
                }
            }
            return str;
        }
    }

    public static class Indicators
    {
        #region CreateIndicators 

        public static int CreateIndicators(Indicators_model _indicator)
        {
            var indicator = new Indicator
            {
                IndicatorName = _indicator.IndicatorName,
                Description = _indicator.Description,
                IsActive = _indicator.IsActive,
                CreatedDate = Utilities.ConvertServerCurrentDateTimeToClientDateTime(DateTime.Now)
            };
            return HelperMethods.GetEntities().SaveEntity_Indicators(indicator);
        }

        #endregion CreateIndicators

        #region GetIndicatorsById

        public static Indicators_model GetIndicatorById(short id)
        {
            Indicator indicator = HelperMethods.GetEntities().GetEntityById_Indicators(id);
            try
            {
                return new Indicators_model
                {
                    ID = indicator.ID,
                    IndicatorName = indicator.IndicatorName,
                    Description = indicator.Description,
                    IsActive = indicator.IsActive,
                    CreatedDate = Utilities.ConvertServerCurrentDateTimeToClientDateTime(DateTime.Now)
                };
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        #endregion GetIndicatorsById

        #region GetIndicatorList

        public static List<Indicators_model> GetIndicatorList()
        {
            List<Indicators_model> category = HelperMethods.GetEntities().GetEntity_IndicatorsList().ToList().AsEnumerable().Select(x =>
             new Indicators_model
             {
                 ID = x.ID,
                 IndicatorName = x.IndicatorName,
                 Description = x.Description,
                 IsActive = x.IsActive,
                 CreatedDate = Utilities.ConvertServerCurrentDateTimeToClientDateTime(DateTime.Now)
             }).ToList();

            return category;
        }

        #endregion GetIndicatorList

        #region DeleteIndicator
        public static string DeleteIndicatorById(short id)
        {
            string str = "";
            bool isExists = HelperMethods.GetEntities().CheckIndicatorExistsInPatientIndicators(id);
            if (!isExists)
            {
                HelperMethods.GetEntities().DeleteEntity_Indicators(id);
                str = "Successfully Deleted!";
            }
            else
            {
                str = "Child Record Exists";
            }
            return str;
        }

        public static string DeleteMultipleIndicator(string Ids)
        {
            string str = "";
            string[] values = Ids.Split(',');
            int length = values.Length;

            for (int i = 0; i < length; i++)
            {
                if (values[i] != "")
                {
                    short ID = Convert.ToInt16(values[i]);

                    bool isExists = HelperMethods.GetEntities().CheckIndicatorExistsInPatientIndicators(ID);
                    if (!isExists)
                    {
                        HelperMethods.GetEntities().DeleteEntity_Indicators(ID);
                        str = "Successfully Deleted!";
                    }
                    else
                    {
                        str = "Child Record Exists";
                    }
                }
            }
            return str;
        }

        #endregion DeleteIndicator

        #region UpdateIndicator

        public static void UpdateIndicator(Indicators_model _indicator)
        {
            Indicator indicator = HelperMethods.GetEntities().GetEntityById_Indicators((short)_indicator.ID);
            indicator.IndicatorName = _indicator.IndicatorName;
            indicator.Description = _indicator.Description;
            indicator.IsActive = _indicator.IsActive;

            HelperMethods.GetEntities().UpdateEntity_Indicators(indicator);
        }

        public static string UpdateIndicatorIsActiveStatus(string Ids, bool Status)
        {
            string str = "";
            string[] values = Ids.Split(',');
            int length = values.Length;

            for (int i = 0; i < length; i++)
            {
                if (values[i] != "")
                {
                    short ID = Convert.ToInt16(values[i]);
                    str = HelperMethods.GetEntities().UpdateEntityIsActiveStatus_Indicators(ID, Status);
                }
            }
            return str;
        }

        #endregion UpdateIndicator

        #region SearchFilterIndicator
        public static usp_search_IndicatorsByFilterCustom_model SearchFilterIndicator(int TotalCount, int? StartIndex = -1, int? EndIndex = -1, string SearchString = null, bool? IsActive = null)
        {
            if (SearchString == "undefined")
                SearchString = null;

            List<usp_search_IndicatorsByFilter_Result> list = HelperMethods.GetEntities().GetEntityBySearchFilter_Indicators(out TotalCount, StartIndex, EndIndex, SearchString, IsActive).ToList();

            var objList = list.ToList().AsEnumerable().Select(x =>
            new usp_search_IndicatorsByFilter_Result_model
            {
                RowNo = x.RowNo,
                ID = x.ID,
                IndicatorName = x.IndicatorName,
                Description = x.Description,
                CreatedDate = x.CreatedDate,
                IsActive = x.IsActive
            }).ToList();

            return (new usp_search_IndicatorsByFilterCustom_model { IndicatorsSearchFilterList = objList, TotalCount = TotalCount });

        }

        #endregion SearchFilterIndicator

    }

    public static class ThirdPartyAppClass
    {
        #region CreateThirdPartyApp

        public static int CreateThirdPartyApp(ThirdPartyApp_model _app)
        {
            var app = new ThirdPartyApp
            {
                AppName = _app.AppName,
                URL = _app.URL,
                SurveyCategoryID = _app.SurveyCategoryID,
                SurveySubCategoryID = _app.SurveySubCategoryID,
                Address = _app.Address,
                Email = _app.Email,
                MobileNo = _app.MobileNo,
                PhoneNo = _app.PhoneNo,
                IsActive = _app.IsActive,
                CreatedDate = Utilities.ConvertServerCurrentDateTimeToClientDateTime(DateTime.Now)
            };
            return HelperMethods.GetEntities().SaveEntity_ThirdPartyApp(app);
        }

        #endregion CreateThirdPartyApp

        #region GetThirdPartyAppById

        public static ThirdPartyApp_model GetThirdPartyAppById(int id)
        {
            ThirdPartyApp app = HelperMethods.GetEntities().GetEntityById_ThirdPartyApp(id);
            try
            {
                return new ThirdPartyApp_model
                {
                    ID = app.ID,
                    AppName = app.AppName,
                    URL = app.URL,
                    SurveyCategoryID = app.SurveyCategoryID,
                    SurveySubCategoryID = app.SurveySubCategoryID,
                    Address = app.Address,
                    Email = app.Email,
                    MobileNo = app.MobileNo,
                    PhoneNo = app.PhoneNo,
                    IsActive = app.IsActive,
                    CreatedDate = app.CreatedDate
                };
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        #endregion GetThirdPartyAppById

        #region GetThirdPartyAppList

        public static List<ThirdPartyApp_model> GetThirdPartyAppList()
        {
            List<ThirdPartyApp_model> app = HelperMethods.GetEntities().GetEntity_ThirdPartyAppList().ToList().AsEnumerable().Select(x => new ThirdPartyApp_model
            {
                ID = x.ID,
                AppName = x.AppName,
                URL = x.URL,
                SurveyCategoryID = x.SurveyCategoryID,
                SurveySubCategoryID = x.SurveySubCategoryID,
                Address = x.Address,
                Email = x.Email,
                MobileNo = x.MobileNo,
                PhoneNo = x.PhoneNo,
                IsActive = x.IsActive,
                CreatedDate = Utilities.ConvertServerCurrentDateTimeToClientDateTime(DateTime.Now)
            }).ToList();

            return app;
        }

        public static List<ThirdPartyApp_model> GetThirdPartyAppByCategoryID(short SurveyID)
        {
            Survey_model objSurvey = SurveyClass.GetSurveyById(SurveyID);

            if (objSurvey != null)
            {
                List<ThirdPartyApp_model> app = HelperMethods.GetEntities().GetEntity_ThirdPartyAppByCategoryID(objSurvey.SurveyCategoryID, objSurvey.SurveySubCategoryID).ToList().AsEnumerable().Select(x => new ThirdPartyApp_model
                {
                    ID = x.ID,
                    AppName = x.AppName,
                    URL = x.URL,
                    SurveyCategoryID = x.SurveyCategoryID,
                    SurveySubCategoryID = x.SurveySubCategoryID,
                    Address = x.Address,
                    Email = x.Email,
                    MobileNo = x.MobileNo,
                    PhoneNo = x.PhoneNo,
                    IsActive = x.IsActive,
                    CreatedDate = Utilities.ConvertServerCurrentDateTimeToClientDateTime(DateTime.Now)
                }).ToList();
                return app;
            }
            return null;
        }

        #endregion GetThirdPartyAppList

        #region DeleteThirdPartyApp
        public static string DeleteThirdPartyAppById(int id)
        {
            string str = "";
            bool isExists = HelperMethods.GetEntities().CheckProviderPatient_ThirdPartyApp(id);
            if (!isExists)
            {
                HelperMethods.GetEntities().DeleteEntity_ThirdPartyApp(id);
                str = "Successfully Deleted!";
            }
            else
            {
                str = "Child Record Exists";
            }
            return str;
        }

        public static string DeleteMultipleThirdPartyApp(string Ids)
        {
            string str = "";
            string[] values = Ids.Split(',');
            int length = values.Length;

            for (int i = 0; i < length; i++)
            {
                if (values[i] != "")
                {
                    short ID = Convert.ToInt16(values[i]);

                    bool isExists = HelperMethods.GetEntities().CheckProviderPatient_ThirdPartyApp(ID);
                    if (!isExists)
                    {
                        HelperMethods.GetEntities().DeleteEntity_ThirdPartyApp(ID);
                        str = "Successfully Deleted!";
                    }
                    else
                    {
                        str = "Child Record Exists";
                    }
                }
            }
            return str;
        }

        #endregion DeleteThirdPartyApp

        #region UpdateThirdPartyApp

        public static void UpdateThirdPartyApp(ThirdPartyApp_model _app)
        {
            ThirdPartyApp app = HelperMethods.GetEntities().GetEntityById_ThirdPartyApp((int)_app.ID);
            app.AppName = _app.AppName;
            app.URL = _app.URL;
            app.SurveyCategoryID = _app.SurveyCategoryID;
            app.SurveySubCategoryID = _app.SurveySubCategoryID;
            app.Email = _app.Email;
            app.Address = _app.Address;
            app.MobileNo = _app.MobileNo;
            app.PhoneNo = _app.PhoneNo;
            app.IsActive = _app.IsActive;

            HelperMethods.GetEntities().UpdateEntity_ThirdPartyApp(app);
        }

        public static string UpdateThirdPartyAppIsActiveStatus(string Ids, bool Status)
        {
            string str = "";
            string[] values = Ids.Split(',');
            int length = values.Length;

            for (int i = 0; i < length; i++)
            {
                if (values[i] != "")
                {
                    short ID = Convert.ToInt16(values[i]);
                    str = HelperMethods.GetEntities().UpdateEntityIsActiveStatus_ThirdPartyApp(ID, Status);
                }
            }
            return str;
        }

        #endregion UpdateThirdPartyApp

        #region SearchFilterThirdPartyApp
        public static usp_search_ThirdPartyAppByFilterCustom_model SearchFilterThirdPartyApp(int TotalCount, int? StartIndex = -1, int? EndIndex = -1, string SearchString = null, bool? IsActive = null)
        {
            if (SearchString == "undefined")
                SearchString = null;

            List<usp_search_ThirdPartyAppByFilter_Result> list = HelperMethods.GetEntities().GetEntityBySearchFilter_ThirdPartyApp(out TotalCount, StartIndex, EndIndex, SearchString, IsActive).ToList();

            var objList = list.ToList().AsEnumerable().Select(x =>
            new usp_search_ThirdPartyAppByFilter_Result_model
            {
                RowNo = x.RowNo,
                ID = x.ID,
                AppName = x.AppName,
                URL = x.URL,
                Address = x.Address,
                Email = x.Email,
                MobileNo = x.MobileNo,
                PhoneNo = x.PhoneNo,
                CreatedDate = x.CreatedDate,
                IsActive = x.IsActive
            }).ToList();

            return (new usp_search_ThirdPartyAppByFilterCustom_model { ThirdPartyAppSearchFilterList = objList, TotalCount = TotalCount });

        }

        #endregion SearchFilterThirdPartyApp

    }

    public static class Providers
    {
        #region Providers
        public static string CreateProvider(Provider_model _objProvider)
        {
            Guid? userid = GetUserIdByUserName(_objProvider.User.UserName);
            var pr = CheckExistingProvider(userid);
            if (pr != null)
            {
                //if (pr.ProviderTypeID == null && _objProvider.ProviderTypeID != null)
                //{
                return UpdateProviderTypeID((Guid)userid, (short)_objProvider.ProviderTypeID);
                //}

            }

            var provider = new Provider
            {
                ID = Guid.NewGuid(),
                UserID = userid,
                Code = GetLastProviderCode(),
                SalutationID = _objProvider.SalutationID,
                IsActive = true,
                CreatedDate = Utilities.ConvertServerCurrentDateTimeToClientDateTime(DateTime.Now),
                CreatedBy = userid,
            };

            return HelperMethods.GetEntities().SaveEntity_Providers(provider);
        }

        public static Guid? UpdateProvider(Provider_model _objProvider)
        {
            Guid? userid = GetUserIdByUserName(_objProvider.User.UserName);
            Guid? providerId = GetProviderID((Guid)userid);

            if (providerId == Guid.Empty)
            {
                var providerdetail = new Provider
                {
                    ID = Guid.NewGuid(),
                    UserID = userid,
                    Code = GetLastProviderCode(),
                    SalutationID = _objProvider.SalutationID,
                    IsActive = true,
                    ModifiedDate = Utilities.ConvertServerCurrentDateTimeToClientDateTime(DateTime.Now),
                    ModifiedBy = userid,
                };

                string Id = HelperMethods.GetEntities().SaveEntity_Providers(providerdetail);
                if (Id != null && Id != "")
                {
                    providerId = new Guid(Id);
                }
            }

            var ud = new UserDetail
            {
                ID = (Guid)userid,
                UserName = _objProvider.User.UserName,
                Email = _objProvider.User.Email,
                FirstName = _objProvider.User.FirstName,
                LastName = _objProvider.User.LastName,
                MiddleName = _objProvider.User.MiddleName,
                DOB = _objProvider.User.DOB,
                Gender = _objProvider.User.Gender
            };

            var addressID = Addresses.ManageProviderAddress((Guid)userid, _objProvider.Address);
            var contactID = ManageContactDetails((Guid)userid, _objProvider.Contact);
            var providerTypeId = GetProviderTypeID((Guid)userid);

            var provider = new Provider
            {
                ID = (Guid)providerId,
                UserID = userid,
                ProviderTypeID = providerTypeId,
                AddressID = addressID,
                ContactID = contactID,
                SalutationID = _objProvider.SalutationID,
                MedicareProviderNumber = _objProvider.MedicareProviderNumber,
                HealthProviderIdentifier = _objProvider.HealthProviderIdentifier,
                PHN = _objProvider.PHN,
                LHD = _objProvider.LHD,
                LHN = _objProvider.LHN,
                Description = _objProvider.Description,
                IsActive = _objProvider.IsActive,
                ModifiedDate = Utilities.ConvertServerCurrentDateTimeToClientDateTime(DateTime.Now),
                ModifiedBy = userid,
                UserDetail = ud
            };

            return HelperMethods.GetEntities().UpdateEntity_Providers(provider);
        }

        public static Guid? GetUserIdByUserName(string userName)
        {
            return HelperMethods.GetEntities().GetUserIdByUserName(userName);
        }

        public static string GetLastProviderCode()
        {
            return HelperMethods.GetEntities().GetLastProviderCode();
        }

        public static List<ProviderType_model> GetProviderTypes()
        {
            return HelperMethods.GetEntities().GetProviderTypes().Select(x => new ProviderType_model
            {
                ID = x.ID,
                ProviderTypeName = x.ProviderTypeName,
                Description = x.Description,
                IsActive = x.IsActive
            }).ToList();
        }

        private static Provider CheckExistingProvider(Guid? UserID)
        {
            return HelperMethods.GetEntities().CheckExistingProvider(UserID);
        }

        public static bool ChangeEmailVerificationStatus(string userName)
        {
            Guid? userId = GetUserIdByUserName(userName);
            if (userId == null)
            {
                return false;
            }

            return HelperMethods.GetEntities().ChangeEmailVerificationStatus(userId);
        }

        public static bool? GetEmailVerificationStatus(string userName)
        {
            Guid? userId = GetUserIdByUserName(userName);
            if (userId == null)
            {
                return false;
            }

            return HelperMethods.GetEntities().GetEmailVerificationStatus(userId);
        }

        public static Guid? GetProviderID(Guid userID)
        {
            return HelperMethods.GetEntities().GetProviderID(userID);
        }

        private static string UpdateProviderTypeID(Guid userid, short providerTypeID)
        {
            return HelperMethods.GetEntities().UpdateProviderTypeID(userid, providerTypeID);
        }

        public static int? ManageContactDetails(Guid userId, Contact_model _objContact)
        {
            if (_objContact == null)
            {
                return null;
            }

            var contact = new Contact
            {
                Phone = _objContact.Phone,
                Mobile = _objContact.Mobile,
                Mobile2 = _objContact.Mobile2,
                FAX = _objContact.FAX,
                Pager = _objContact.Pager,
                Emergency = _objContact.Emergency,
                EmailBusiness = _objContact.EmailBusiness,
                EmailPersonal = _objContact.EmailPersonal,
                Skype = _objContact.Skype,
                LinkedIN = _objContact.LinkedIN,
                Website = _objContact.Website,
                OtherFieldsJSON = _objContact.OtherFieldsJSON,
                Description = _objContact.Description,
                IsActive = _objContact.IsActive,
                CreatedDate = Utilities.ConvertServerCurrentDateTimeToClientDateTime(DateTime.Now),
                CreatedBy = _objContact.CreatedBy
            };

            return HelperMethods.GetEntities().ManageProviderContactDetails(userId, contact);
        }

        public static short? GetProviderTypeID(Guid userID)
        {
            return HelperMethods.GetEntities().GetProviderTypeID(userID);
        }

        public static Provider_model GetProviderDetails(string UserName)
        {
            if (string.IsNullOrEmpty(UserName))
            {
                return null;
            }

            Guid? userid = GetUserIdByUserName(UserName);
            Guid? providerId = GetProviderID((Guid)userid);

            var obj = HelperMethods.GetEntities().GetProviderDetails(providerId);
            if (obj == null)
            {
                return null;
            }

            return new Provider_model
            {
                ID = obj.ID,
                Code = obj.Code,
                Address = Addresses.GetAddressDetails(obj.AddressID, Guid.Empty),
                Contact = GetContactDetails(obj.ContactID, Guid.Empty),
                HealthProviderIdentifier = obj.HealthProviderIdentifier,
                MedicareProviderNumber = obj.MedicareProviderNumber,
                SalutationID = obj.SalutationID,
                PHN = obj.PHN,
                LHD = obj.LHD,
                LHN = obj.LHN,
                ProviderTypeID = obj.ProviderTypeID,
                User = new User_model { UserName = obj.UserDetail.UserName, FirstName = obj.UserDetail.FirstName, MiddleName = obj.UserDetail.MiddleName, LastName = obj.UserDetail.LastName, DOB = obj.UserDetail.DOB, Gender = obj.UserDetail.Gender },
                Description = obj.Description,
                CreatedDate = obj.CreatedDate,
                ModifiedDate = obj.ModifiedDate,
                IsActive = obj.IsActive,
                SecretQuestionID = HelperMethods.GetEntities().getUserQuestionByUserID((Guid)userid) != null ? (short)HelperMethods.GetEntities().getUserQuestionByUserID((Guid)userid).SecretQuestionID : (short)0,
                Answer = HelperMethods.GetEntities().getUserQuestionByUserID((Guid)userid) != null ? HelperMethods.GetEntities().getUserQuestionByUserID((Guid)userid).Answer : "",
                SecretQuestionIDMain = HelperMethods.GetEntities().getUserQuestionByUserID((Guid)userid) != null ? HelperMethods.GetEntities().getUserQuestionByUserID((Guid)userid).ID : Guid.Empty
            };
        }

        public static Contact_model GetContactDetails(int? contactID, Guid userId)
        {
            if (contactID != null && contactID != int.MinValue)
            {
                var obj = HelperMethods.GetEntities().GetContactDetails(contactID);
                if (obj == null)
                {
                    return null;
                }

                return new Contact_model
                {
                    Phone = obj.Phone,
                    Mobile = obj.Mobile,
                    Mobile2 = obj.Mobile2,
                    FAX = obj.FAX,
                    Pager = obj.Pager,
                    Emergency = obj.Emergency,
                    EmailBusiness = obj.EmailBusiness,
                    EmailPersonal = obj.EmailPersonal,
                    Skype = obj.Skype,
                    LinkedIN = obj.LinkedIN,
                    Website = obj.Website,
                    OtherFieldsJSON = obj.OtherFieldsJSON,
                    Description = obj.Description,
                    IsActive = obj.IsActive,
                    CreatedDate = obj.CreatedDate,
                    ModifiedDate = obj.ModifiedDate,
                    CreatedBy = obj.CreatedBy,
                    ModifiedBy = obj.ModifiedBy,
                };
            }
            else if (userId != Guid.Empty)
            {
                var obj = HelperMethods.GetEntities().GetContactDetails_ByUserId(userId);
                if (obj == null)
                {
                    return null;
                }
                return new Contact_model
                {
                    Phone = obj.Phone,
                    Mobile = obj.Mobile,
                    Mobile2 = obj.Mobile2,
                    FAX = obj.FAX,
                    Pager = obj.Pager,
                    Emergency = obj.Emergency,
                    EmailBusiness = obj.EmailBusiness,
                    EmailPersonal = obj.EmailPersonal,
                    Skype = obj.Skype,
                    LinkedIN = obj.LinkedIN,
                    Website = obj.Website,
                    OtherFieldsJSON = obj.OtherFieldsJSON,
                    Description = obj.Description,
                    IsActive = obj.IsActive,
                    CreatedDate = obj.CreatedDate,
                    ModifiedDate = obj.ModifiedDate,
                    CreatedBy = obj.CreatedBy,
                    ModifiedBy = obj.ModifiedBy,
                };
            }
            return null;
        }

        #endregion Providers
    }

    public static class Addresses
    {
        #region Address
        public static long? ManageProviderAddress(Guid userId, Address_model _objAddress)
        {
            if (_objAddress == null)
            {
                return null;
            }

            var add = new Address
            {
                Line1 = _objAddress.Line1,
                Line2 = _objAddress.Line2,
                StateID = _objAddress.StateID,
                CountryID = _objAddress.CountryID,
                Suburb = _objAddress.Suburb,
                ZipCode = _objAddress.ZipCode,
                IsActive = _objAddress.IsActive,
                UserId = _objAddress.UserId
            };

            return HelperMethods.GetEntities().ManageProviderAddress(userId, add);
        }

        public static long? ManageAddress(Guid userId, Address_model _objAddress)
        {
            if (_objAddress == null)
            {
                return null;
            }

            var add = new Address
            {
                Line1 = _objAddress.Line1,
                Line2 = _objAddress.Line2,
                StateID = _objAddress.StateID,
                CountryID = _objAddress.CountryID,
                Suburb = _objAddress.Suburb,
                ZipCode = _objAddress.ZipCode,
                IsActive = _objAddress.IsActive,
                UserId = _objAddress.UserId
            };

            return HelperMethods.GetEntities().ManageAddress(userId, add);
        }

        public static Address_model GetAddressDetails(long? addressID, Guid userId)
        {
            if (addressID != null && addressID != long.MinValue)
            {
                var obj = HelperMethods.GetEntities().GetAddressDetails(addressID);
                if (obj == null)
                {
                    return null;
                }
                return new Address_model
                {
                    ID = obj.ID,
                    Country = obj.Country.CountryName,
                    CountryID = obj.CountryID,
                    State = obj.State.StateName,
                    StateID = obj.StateID,
                    Line1 = obj.Line1,
                    Line2 = obj.Line2,
                    ZipCode = obj.ZipCode,
                    Description = obj.Description,
                    Suburb = obj.Suburb,
                    UserId = obj.UserId
                };
            }
            else if (userId != Guid.Empty)
            {
                var obj = HelperMethods.GetEntities().GetAddressDetails_ByUserId(userId);
                if (obj == null)
                {
                    return null;
                }
                return new Address_model
                {
                    ID = obj.ID,
                    Country = obj.Country.CountryName,
                    CountryID = obj.CountryID,
                    State = obj.State.StateName,
                    StateID = obj.StateID,
                    Line1 = obj.Line1,
                    Line2 = obj.Line2,
                    ZipCode = obj.ZipCode,
                    Description = obj.Description,
                    Suburb = obj.Suburb,
                    UserId = obj.UserId
                };
            }
            return null;
        }

        public static List<State_model> GetStates()
        {
            return HelperMethods.GetEntities().GetStates().Select(x => new State_model
            {
                ID = x.ID,
                Code = x.Code,
                StateName = x.StateName,
                IsActive = x.IsActive
            }).ToList();
        }

        public static List<Country_model> GetCountries()
        {
            return HelperMethods.GetEntities().GetCountries().Select(x => new Country_model
            {
                ID = x.ID,
                DialCode = x.DialCode,
                CountryName = x.CountryName,
                IsActive = x.IsActive
            }).ToList();
        }

        #endregion Address
    }

    public static class SecretQuestion
    {
        #region SecretQuestion

        public static List<SecretQuestion_model> GetSecretQuestion()
        {
            return HelperMethods.GetEntities().GetEntity_SecretQuestion().Select(x => new SecretQuestion_model
            {
                ID = x.ID,
                Questions = x.Questions,
                Description = x.Description,
                IsActive = x.IsActive
            }).ToList();
        }

        public static string CheckSecretQuestionAnswer(Guid userId, int QuestionId, string answer)
        {
            return HelperMethods.GetEntities().CheckSecretQuestionAnswer(userId, QuestionId, answer);
        }

        #endregion SecretQuestion
    }

    public static class UserDetails
    {
        #region UserDetails

        public static bool CheckExistingUser(Guid? UserID)
        {
            return HelperMethods.GetEntities().CheckExistingUser(UserID);
        }

        #endregion UserDetails
    }

    public static class Patients
    {
        #region Patients

        public static string CreatePatient(Patient_model _patient, Guid userId)
        {
            try
            {
                var add = new Address
                {
                    Line1 = _patient.Address.Line1,
                    Line2 = _patient.Address.Line2,
                    StateID = _patient.Address.StateID,
                    CountryID = _patient.Address.CountryID,
                    Suburb = _patient.Address.Suburb,
                    ZipCode = _patient.Address.ZipCode,
                    IsActive = _patient.Address.IsActive,
                    UserId = _patient.Address.UserId,
                };

                long? AddressID = HelperMethods.GetEntities().Save_Address(add);

                var contact = new Contact
                {
                    Phone = _patient.Contact.Phone,
                    Mobile = _patient.Contact.Mobile,
                    Mobile2 = _patient.Contact.Mobile2,
                    FAX = _patient.Contact.FAX,
                    Pager = _patient.Contact.Pager,
                    Emergency = _patient.Contact.Emergency,
                    EmailBusiness = _patient.Contact.EmailBusiness,
                    EmailPersonal = _patient.Contact.EmailPersonal,
                    Skype = _patient.Contact.Skype,
                    LinkedIN = _patient.Contact.LinkedIN,
                    Website = _patient.Contact.Website,
                    OtherFieldsJSON = _patient.Contact.OtherFieldsJSON,
                    Description = _patient.Contact.Description,
                    IsActive = true,
                    CreatedDate = _patient.Contact.CreatedDate,
                    ModifiedDate = _patient.Contact.ModifiedDate,
                    CreatedBy = _patient.Contact.CreatedBy,
                    ModifiedBy = _patient.Contact.ModifiedBy
                };

                int? ContactID = HelperMethods.GetEntities().Save_Contact(contact);

                Guid PatientId = Guid.NewGuid();
                var patient = new Patient
                {
                    ID = PatientId,
                    UserID = _patient.PatientUserId,
                    ContactID = ContactID,
                    Code = _patient.Code,
                    PatientCategoryID = 1,
                    AddressID = AddressID,
                    Salutation = _patient.Salutation,
                    Description = _patient.Description,
                    MedicareNumber = _patient.MedicareNumber,
                    IHINumber = _patient.IHINumber,
                    IsActive = true,
                    CreatedDate = Utilities.ConvertServerCurrentDateTimeToClientDateTime(DateTime.Now)
                };
                HelperMethods.GetEntities().SaveEntity_Patient(patient);

                Guid? ProviderId = HelperMethods.GetEntities().GetProviderID(userId);
                var patientprovider = new PatientProvider
                {
                    ID = Guid.NewGuid(),
                    ProviderID = (Guid)ProviderId,
                    PatientID = PatientId,
                    OrganizationID = _patient.OrganizationID,
                    PracticeID = _patient.PracticeID,
                    Description = _patient.Description,
                    IsActive = true,
                    CreatedDate = Utilities.ConvertServerCurrentDateTimeToClientDateTime(DateTime.Now)
                };

                HelperMethods.GetEntities().SaveEntity_PatientProvider(patientprovider);

                var user = new UserDetail
                {
                    ID = _patient.PatientUserId,
                    FirstName = _patient.User.FirstName,
                    LastName = _patient.User.LastName,
                    MiddleName = _patient.User.MiddleName,
                    DOB = _patient.User.DOB.Value.AddDays(1),
                    Gender = _patient.User.Gender
                };

                HelperMethods.GetEntities().Entity_UpdateuserDetail(user);
                return PatientId.ToString();
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return "";
            }
        }

        public static string CsvPatientProviderAssociate(Guid ProviderId, Guid PatientId, Guid OrganizationId, Guid PracticeId, string Email)
        {
            try
            {
                bool isExist = HelperMethods.GetEntities().CheckPatientProviderExist(ProviderId, PatientId, OrganizationId, PracticeId);

                if (!isExist)
                {
                    var patientprovider = new PatientProvider
                    {
                        ID = Guid.NewGuid(),
                        ProviderID = (Guid)ProviderId,
                        PatientID = PatientId,
                        OrganizationID = OrganizationId,
                        PracticeID = PracticeId,
                        Description = "",
                        IsActive = true,
                        CreatedDate = Utilities.ConvertServerCurrentDateTimeToClientDateTime(DateTime.Now)
                    };

                    bool returnval = HelperMethods.GetEntities().SaveEntity_PatientProvider(patientprovider);

                    if (returnval)
                    {
                        var AssocialteUrl = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + "/api/Email/SendPatientAssociateToProvider?PatientId=" + PatientId + "&OrganizationId=" + OrganizationId + "&PracticeId=" + PracticeId + "&ProviderId=" + PracticeId + "&UserName=" + Email;

                        Entities.CustomeWebRequest(AssocialteUrl, "POST");
                    }
                    return "Patient associate successfully.";
                }
                else
                {
                    return "Patient already associate successfully.";
                }
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return "";
            }
        }

        public static string CreatePatientProvider(PatientProviders_model _patient)
        {
            try
            {
                var patientprovider = new PatientProvider
                {
                    ID = Guid.NewGuid(),
                    ProviderID = (Guid)_patient.ProviderID,
                    PatientID = _patient.PatientID,
                    OrganizationID = _patient.OrganizationID,
                    PracticeID = _patient.PracticeID,
                    Description = _patient.Description,
                    IsActive = true,
                    CreatedDate = Utilities.ConvertServerCurrentDateTimeToClientDateTime(DateTime.Now)
                };

                HelperMethods.GetEntities().SaveEntity_PatientProvider(patientprovider);
                return "Patient associate successfully.";
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return "";
            }
        }

        public static List<Provider_Patient_model> GetProviderPatientDetailByUserName(string userName, Guid? OrganizationId, Guid? PracticeId)
        {
            Guid? userid = Providers.GetUserIdByUserName(userName);
            Guid? ProviderId = HelperMethods.GetEntities().GetProviderID((Guid)userid);


            var aaa = HelperMethods.GetEntities().GetProviderPatientDetailByUserID(ProviderId, OrganizationId, PracticeId).Select(x => new Provider_Patient_model
            {
                ID = x.ID,
                ProviderID = x.ProviderID,
                PatientID = x.PatientID,
                Description = x.Description,
                IsActive = x.IsActive,
                CreatedDate = x.CreatedDate,
                ModifiedDate = x.ModifiedDate,
                CreatedBy = x.CreatedBy,
                ModifiedBy = x.ModifiedBy,
                Providers = Providers.GetProviderDetails(userName),
                OrganizationID = x.OrganizationID,
                PracticeID = x.PracticeID,
                Patients = new Patient_model()
                {
                    ID = x.Patient.ID,
                    UserID = x.Patient.UserID,
                    ContactID = x.Patient.ContactID,
                    Code = x.Patient.Code,
                    PatientCategoryID = x.Patient.PatientCategoryID,
                    AddressID = x.Patient.AddressID,
                    Description = x.Patient.Description,
                    Salutation = x.Patient.Salutation,
                    MedicareNumber = x.Patient.MedicareNumber,
                    IHINumber = x.Patient.IHINumber,
                    IsActive = x.Patient.IsActive,
                    CreatedDate = x.Patient.CreatedDate,
                    ModifiedDate = x.Patient.ModifiedDate,
                    CreatedBy = x.Patient.CreatedBy,
                    ModifiedBy = x.Patient.ModifiedBy,
                    User = new User_model { UserName = x.Patient.UserDetail.UserName, FirstName = x.Patient.UserDetail.FirstName, MiddleName = x.Patient.UserDetail.MiddleName, LastName = x.Patient.UserDetail.LastName, Email = x.Patient.UserDetail.Email, ID = x.Patient.UserDetail.ID, DOB = x.Patient.UserDetail.DOB, Gender = x.Patient.UserDetail.Gender, EmailConfirmed = x.Patient.UserDetail.EmailConfirmed },
                    Address = Addresses.GetAddressDetails(x.Patient.AddressID, Guid.Empty),
                    Contact = Providers.GetContactDetails(x.Patient.ContactID, Guid.Empty)
                },
                SurvayList = PatientSurveyClass.GetSurveyByPatient_Provider_Org_Practice_IDs(x.PatientID, x.ProviderID, (Guid)x.OrganizationID, (Guid)x.PracticeID, false, false),
            }).OrderBy(x => x.Patients.User.FirstName).ToList();

            return aaa;
        }

        public static List<Provider_Patient_model> GetProviderPatientDetailBySeletedPatient(string userName, Guid? OrganizationId, Guid? PracticeId, string patientName)
        {
            Guid? userid = Providers.GetUserIdByUserName(userName);
            Guid? ProviderId = HelperMethods.GetEntities().GetProviderID((Guid)userid);

            var aaa = HelperMethods.GetEntities().GetProviderPatientDetailBySeletedPatient(ProviderId, OrganizationId, PracticeId, patientName).Select(x => new Provider_Patient_model
            {
                ID = x.ID,
                ProviderID = x.ProviderID,
                PatientID = x.PatientID,
                Description = x.Description,
                IsActive = x.IsActive,
                CreatedDate = x.CreatedDate,
                ModifiedDate = x.ModifiedDate,
                CreatedBy = x.CreatedBy,
                ModifiedBy = x.ModifiedBy,
                Providers = Providers.GetProviderDetails(userName),
                OrganizationID = x.OrganizationID,
                PracticeID = x.PracticeID,
                Patients = new Patient_model()
                {
                    ID = x.Patient.ID,
                    UserID = x.Patient.UserID,
                    ContactID = x.Patient.ContactID,
                    Code = x.Patient.Code,
                    PatientCategoryID = x.Patient.PatientCategoryID,
                    AddressID = x.Patient.AddressID,
                    Description = x.Patient.Description,
                    Salutation = x.Patient.Salutation,
                    MedicareNumber = x.Patient.MedicareNumber,
                    IHINumber = x.Patient.IHINumber,
                    IsActive = x.Patient.IsActive,
                    CreatedDate = x.Patient.CreatedDate,
                    ModifiedDate = x.Patient.ModifiedDate,
                    CreatedBy = x.Patient.CreatedBy,
                    ModifiedBy = x.Patient.ModifiedBy,
                    User = new User_model { UserName = x.Patient.UserDetail.UserName, FirstName = x.Patient.UserDetail.FirstName, MiddleName = x.Patient.UserDetail.MiddleName, LastName = x.Patient.UserDetail.LastName, Email = x.Patient.UserDetail.Email, ID = x.Patient.UserDetail.ID, DOB = x.Patient.UserDetail.DOB, Gender = x.Patient.UserDetail.Gender, EmailConfirmed = x.Patient.UserDetail.EmailConfirmed },
                    Address = Addresses.GetAddressDetails(x.Patient.AddressID, Guid.Empty),
                    Contact = Providers.GetContactDetails(x.Patient.ContactID, Guid.Empty)
                },
                SurvayList = PatientSurveyClass.GetSurveyByPatient_Provider_Org_Practice_IDs(x.PatientID, x.ProviderID, (Guid)x.OrganizationID, (Guid)x.PracticeID, false, false),
            }).OrderBy(x => x.Patients.User.FirstName).ToList();

            return aaa;
        }

        public static Patient_model GetPatientById(Guid PatientId)
        {
            Patient patient = HelperMethods.GetEntities().GetEntityById_Patient(PatientId);
            return new Patient_model
            {
                ID = patient.ID,
                UserID = patient.UserID,
                ContactID = patient.ContactID,
                Contact = Providers.GetContactDetails(patient.ContactID, Guid.Empty),
                Code = patient.Code,
                PatientCategoryID = patient.PatientCategoryID,
                AddressID = patient.AddressID,
                Address = Addresses.GetAddressDetails(patient.AddressID, Guid.Empty),
                Description = patient.Description,
                Salutation = patient.Salutation,
                MedicareNumber = patient.MedicareNumber,
                IHINumber = patient.IHINumber,
                ProviderName = patient.UserDetail.FirstName + "  " + patient.UserDetail.LastName,
                IsActive = patient.IsActive,
            };
        }

        public static Patient_model GetProviderDetailByPatientSurveyID(Guid PatientId, Guid PatientSurveyId)
        {
            List<PatientSurvey> patientSurvey = HelperMethods.GetEntities().GetEntityByID_PatientSurveyList(PatientSurveyId);

            if (patientSurvey != null && patientSurvey.Count() > 0)
            {
                PatientProvider provider = HelperMethods.GetEntities().GetPatientProviderByIds(PatientId, patientSurvey[0].ProviderID, patientSurvey[0].OrganizationID, patientSurvey[0].PracticeID);

                return new Patient_model
                {
                    ID = provider.PatientID,
                    UserID = (Guid)provider.Provider.UserID,
                    ProviderId = provider.ProviderID,
                    PracticeID = (Guid)patientSurvey[0].PracticeID,
                    ContactID = provider.Provider.ContactID,
                    Contact = Providers.GetContactDetails(provider.Provider.ContactID, Guid.Empty),
                    Code = provider.Provider.Code,
                    AddressID = provider.Provider.AddressID,
                    Address = Addresses.GetAddressDetails(provider.Provider.AddressID, Guid.Empty),
                    Description = provider.Provider.Description,
                    ProviderName = provider.Provider.UserDetail.FirstName + "  " + provider.Provider.UserDetail.LastName,
                    IsActive = provider.Provider.IsActive,
                    OrganizationName = HelperMethods.GetEntities().GetOrganizationsByID((Guid)patientSurvey[0].OrganizationID).OrganizationName,
                    PracticeName = HelperMethods.GetEntities().GetPracticeByID((Guid)patientSurvey[0].PracticeID).PracticeName,
                    ProviderEmail = provider.Provider.UserDetail.Email
                };
            }
            return null;
        }

        public static Patient_model GetProviderDetailByPatient_Org_PracticeId(Guid PatientId, Guid ProviderId, Guid OrganizationId, Guid PracticeId)
        {
            PatientProvider provider = HelperMethods.GetEntities().GetPatientProviderByIds(PatientId, ProviderId, OrganizationId, PracticeId);

            if (provider != null)
            {
                return new Patient_model
                {
                    ID = provider.PatientID,
                    UserID = (Guid)provider.Provider.UserID,
                    ProviderId = provider.ProviderID,
                    ContactID = provider.Provider.ContactID,
                    Contact = Providers.GetContactDetails(provider.Provider.ContactID, Guid.Empty),
                    Code = provider.Provider.Code,
                    AddressID = provider.Provider.AddressID,
                    Address = Addresses.GetAddressDetails(provider.Provider.AddressID, Guid.Empty),
                    Description = provider.Provider.Description,
                    ProviderName = provider.Provider.UserDetail.FirstName + "  " + provider.Provider.UserDetail.LastName,
                    IsActive = provider.Provider.IsActive,
                    OrganizationName = HelperMethods.GetEntities().GetOrganizationsByID(OrganizationId).OrganizationName,
                    PracticeName = HelperMethods.GetEntities().GetPracticeByID(PracticeId).PracticeName,
                    ProviderEmail = provider.Provider.UserDetail.Email
                };
            }
            return null;
        }

        public static bool UpdateUserData(Patient_model patient_data)
        {
            try
            {
                UserDetail objUserdetail = new UserDetail();
                objUserdetail.ID = patient_data.UserID;
                objUserdetail.FirstName = patient_data.User.FirstName;
                objUserdetail.LastName = patient_data.User.LastName;
                objUserdetail.MiddleName = patient_data.User.MiddleName;
                objUserdetail.DOB = patient_data.UpdateDOB.Value.AddDays(1);
                objUserdetail.Gender = patient_data.User.Gender;
                HelperMethods.GetEntities().Entity_UpdateuserDetail(objUserdetail);

                Contact objcontact = new Contact();
                objcontact.Mobile = patient_data.Contact.Mobile;
                objcontact.Mobile2 = patient_data.Contact.Mobile2;
                objcontact.EmailPersonal = patient_data.Contact.EmailPersonal;
                objcontact.EmailBusiness = patient_data.Contact.EmailBusiness;
                objcontact.ID = (int)patient_data.ContactID;
                HelperMethods.GetEntities().Entity_UpdatecontactDetail(objcontact);

                Address objAddress = new Address();
                objAddress.ID = patient_data.Address.ID;
                objAddress.Line1 = patient_data.Address.Line1;
                objAddress.Line2 = patient_data.Address.Line2;
                objAddress.ModifiedDate = Utilities.ConvertServerCurrentDateTimeToClientDateTime(DateTime.Now);
                objAddress.StateID = patient_data.Address.StateID;
                objAddress.CountryID = patient_data.Address.CountryID;
                objAddress.ZipCode = patient_data.Address.ZipCode;
                objAddress.UserId = patient_data.Address.UserId;
                objAddress.Suburb = patient_data.Address.Suburb;
                HelperMethods.GetEntities().Entity_UpdateAddress(objAddress);

                Patient objpatient = new Patient();
                objpatient.ID = patient_data.ID;
                objpatient.ContactID = patient_data.ContactID;
                objpatient.Code = patient_data.Code;
                objpatient.PatientCategoryID = patient_data.PatientCategoryID;
                objpatient.AddressID = patient_data.AddressID;
                objpatient.Description = patient_data.Description;
                objpatient.MedicareNumber = patient_data.MedicareNumber;
                objpatient.IHINumber = patient_data.IHINumber;
                objpatient.Salutation = patient_data.Salutation;
                objpatient.ModifiedDate = Utilities.ConvertServerCurrentDateTimeToClientDateTime(DateTime.Now);
                objpatient.ModifiedBy = patient_data.ID;
                objpatient.IsActive = true;
                HelperMethods.GetEntities().UpdateEntity_Patient(objpatient);

                return true;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return false;
            }
        }

        public static Patient_model GetPatientDetailsByUserName(string userName)
        {
            Guid? userid = Providers.GetUserIdByUserName(userName);
            Guid? PatientId = HelperMethods.GetEntities().GetPatientID((Guid)userid);
            if (PatientId != Guid.Empty)
            {
                var patient = HelperMethods.GetEntities().GetPatientDetailsByUserID(PatientId);

                if (patient != null)
                {
                    return new Patient_model
                    {
                        ID = (Guid)PatientId,
                        UserID = patient.UserID,
                        ContactID = patient.ContactID,
                        Code = patient.Code,
                        PatientCategoryID = patient.PatientCategoryID,
                        AddressID = patient.AddressID,
                        Salutation = patient.Salutation,
                        Description = patient.Description,
                        MedicareNumber = patient.MedicareNumber,
                        IHINumber = patient.IHINumber,
                        IsActive = patient.IsActive,
                        CreatedDate = patient.CreatedDate,
                        ModifiedDate = patient.ModifiedDate,
                        CreatedBy = patient.CreatedBy,
                        ModifiedBy = patient.ModifiedBy,
                        PatientOrgList = Patients.GetPatientsOrgPractice(PatientId),
                        User = new User_model { ID = patient.UserDetail.ID, UserName = patient.UserDetail.UserName, FirstName = patient.UserDetail.FirstName, MiddleName = patient.UserDetail.MiddleName, LastName = patient.UserDetail.LastName, PhoneNumber = patient.UserDetail.PhoneNumber, PasswordHash = patient.UserDetail.PasswordHash, Email = patient.UserDetail.Email, EmailConfirmed = patient.UserDetail.EmailConfirmed, DOB = patient.UserDetail.DOB, Gender = patient.UserDetail.Gender },
                        Address = Addresses.GetAddressDetails(patient.AddressID, Guid.Empty),
                        Contact = Providers.GetContactDetails(patient.ContactID, Guid.Empty),
                        SecretQuestionID = HelperMethods.GetEntities().getUserQuestionByUserID((Guid)userid) != null ? (short)HelperMethods.GetEntities().getUserQuestionByUserID((Guid)userid).SecretQuestionID : (short)0,
                        Answer = HelperMethods.GetEntities().getUserQuestionByUserID((Guid)userid) != null ? HelperMethods.GetEntities().getUserQuestionByUserID((Guid)userid).Answer : "",
                        SecretQuestionIDMain = HelperMethods.GetEntities().getUserQuestionByUserID((Guid)userid) != null ? HelperMethods.GetEntities().getUserQuestionByUserID((Guid)userid).ID : Guid.Empty
                    };
                }
            }
            return null;
        }

        public static List<PatientOrgPractice_model> GetPatientsOrgPractice(Guid? patientId)
        {
            return HelperMethods.GetEntities().GetPatientProviderListByPatientID(patientId).Select(x => new PatientOrgPractice_model
            {
                PatientID = x.PatientID,
                ProviderID = x.ProviderID,
                ProviderName = x.Provider.UserDetail.FirstName + " " + x.Provider.UserDetail.LastName,
                OrganizationID = x.OrganizationID,
                OrganizationName = x.Organization.OrganizationName,
                PracticeID = x.PracticeID,
                PracticeName = x.Practice.PracticeName,
            }).ToList();
        }

        public static List<PatientSurveyStatus_model> GetPatientSurveyStatusData(Guid? PatientID, Guid OrganizationID, Guid PracticeID, Guid ProviderID, int SurveyId, string strFromDate, bool forEmail)
        {
            DateTime? FromDate = null;

            if (!string.IsNullOrEmpty(strFromDate))
            {
                string[] StartDateArray = strFromDate.Split('/');
                if (StartDateArray.Length > 0)
                {
                    FromDate = new DateTime(Convert.ToInt32(StartDateArray[2]), Convert.ToInt32(StartDateArray[0]), Convert.ToInt32(StartDateArray[1]), 0, 0, 0);
                }
            }
            else if (forEmail == true)
            {
                FromDate = DateTime.Today.Date;
            }
            else
            {
                FromDate = DateTime.Today.AddDays(-10);
            }

            PatientSurvey pSurvey = HelperMethods.GetEntities().GetEntity_PatientSurvey(PatientID, OrganizationID, PracticeID, ProviderID, SurveyId);

            if (pSurvey != null)
            {
                var psm = HelperMethods.GetEntities().GetPatientSurveyStatusData(pSurvey.ID, FromDate).Select(x => new PatientSurveyStatus_model
                {
                    ID = x.ID,
                    SurveyID = SurveyId,
                    NormValue = pSurvey.Survey.NormValue,
                    PatientSurveyId = x.PatientSurveyID,
                    CreatedDate = Convert.ToDateTime(x.CreatedDate).Date,
                    Email = x.Email,
                    ExternalTitle = pSurvey.Survey.ExternalTitle,
                    Score = x.Score
                }).ToList();
                return psm;
            }

            return null;
        }

        public static List<PatientSurveyStatus_model> GetPatientSurveyStatusByID(Guid UniqueId)
        {
            PatientSurveyStatu pSurveyStatus = HelperMethods.GetEntities().GetPatientSurveyStatusById(UniqueId);

            if (pSurveyStatus != null)
            {
                return HelperMethods.GetEntities().GetEntityByID_PatientSurveyList(pSurveyStatus.PatientSurveyID).Select(x => new PatientSurveyStatus_model
                {
                    ID = pSurveyStatus.ID,
                    PatientSurveyId = x.ID,
                    CreatedDate = x.CreatedDate,
                    Email = pSurveyStatus.Email,
                    ExternalTitle = x.Survey.ExternalTitle,
                    Score = pSurveyStatus.Score
                }).ToList();
            }

            return null;
        }

        public static Patient_model SearchPatientDetail(string IHINumber, string MedicareNumber, Guid ProviderId, Guid OrganizationId, Guid PracticeId)
        {
            bool isPatientExist = HelperMethods.GetEntities().CheckPatientExist(IHINumber, MedicareNumber, ProviderId, OrganizationId, PracticeId);
            var patient = HelperMethods.GetEntities().SearchPatientDetailByUniqueNumber(IHINumber, MedicareNumber);

            if (patient != null)
            {
                var patientDetail = new Patient_model
                {
                    ID = patient.ID,
                    UserID = patient.UserID,
                    ContactID = patient.ContactID,
                    Code = patient.Code,
                    PatientCategoryID = patient.PatientCategoryID,
                    AddressID = patient.AddressID,
                    Salutation = patient.Salutation,
                    Description = patient.Description,
                    IHINumber = patient.IHINumber,
                    MedicareNumber = patient.MedicareNumber,
                    IsActive = patient.IsActive,
                    CreatedDate = patient.CreatedDate,
                    ModifiedDate = patient.ModifiedDate,
                    CreatedBy = patient.CreatedBy,
                    ModifiedBy = patient.ModifiedBy,
                    User = new User_model { ID = patient.ID, UserName = patient.UserDetail.UserName, FirstName = patient.UserDetail.FirstName, MiddleName = patient.UserDetail.MiddleName, LastName = patient.UserDetail.LastName, PhoneNumber = patient.UserDetail.PhoneNumber, PasswordHash = patient.UserDetail.PasswordHash, Email = patient.UserDetail.Email, EmailConfirmed = patient.UserDetail.EmailConfirmed, DOB = patient.UserDetail.DOB, Gender = patient.UserDetail.Gender },
                    Address = Addresses.GetAddressDetails(patient.AddressID, Guid.Empty),
                    Contact = Providers.GetContactDetails(patient.ContactID, Guid.Empty),
                    IsPatientExist = isPatientExist,
                };
                return patientDetail;
            }
            return null;
        }

        public static Patient_model SearchPatientDetailByUniqueNumber(string IHINumber, string MedicareNumber)
        {
            var patient = HelperMethods.GetEntities().SearchPatientDetailByUniqueNumber(IHINumber, MedicareNumber);

            if (patient != null)
            {
                var patientDetail = new Patient_model
                {
                    ID = patient.ID,
                    UserID = patient.UserID,
                    ContactID = patient.ContactID,
                    Code = patient.Code,
                    PatientCategoryID = patient.PatientCategoryID,
                    AddressID = patient.AddressID,
                    Salutation = patient.Salutation,
                    Description = patient.Description,
                    IHINumber = patient.IHINumber,
                    MedicareNumber = patient.MedicareNumber,
                    IsActive = patient.IsActive,
                    CreatedDate = patient.CreatedDate,
                    ModifiedDate = patient.ModifiedDate,
                    CreatedBy = patient.CreatedBy,
                    ModifiedBy = patient.ModifiedBy,
                    User = new User_model { ID = patient.ID, UserName = patient.UserDetail.UserName, FirstName = patient.UserDetail.FirstName, MiddleName = patient.UserDetail.MiddleName, LastName = patient.UserDetail.LastName, PhoneNumber = patient.UserDetail.PhoneNumber, PasswordHash = patient.UserDetail.PasswordHash, Email = patient.UserDetail.Email, EmailConfirmed = patient.UserDetail.EmailConfirmed, DOB = patient.UserDetail.DOB, Gender = patient.UserDetail.Gender },
                    Address = Addresses.GetAddressDetails(patient.AddressID, Guid.Empty),
                    Contact = Providers.GetContactDetails(patient.ContactID, Guid.Empty)
                };
                return patientDetail;
            }
            return null;
        }

        public static string ActiveDeactivePatient(Guid? PatientID, bool status)
        {
            string return_result = "success";
            try
            {
                Patient objpatient = new Patient();
                objpatient.ID = (Guid)PatientID;
                objpatient.IsActive = status;
                objpatient.ModifiedBy = PatientID;
                HelperMethods.GetEntities().UpdateStatus_Patient(objpatient);

            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
            }
            return return_result;
        }

        public static bool CheckExistingMedicure(string MedicareNumber)
        {
            try
            {
                return HelperMethods.GetEntities().CheckExistingMedicure(MedicareNumber);
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return false;
            }
        }

        public static bool CheckExistingIHINumber(string IHINumber)
        {
            try
            {
                return HelperMethods.GetEntities().CheckExistingIHINumber(IHINumber);
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return false;
            }
        }


        #endregion Patients
    }

    public static class SurveyClass
    {
        #region CreateSurvey

        public static int CreateSurvey(Survey_model _survey, Guid userId)
        {
            var survey = new Survey
            {
                Title = _survey.Title,
                ExternalTitle = _survey.ExternalTitle,
                ExternalID = _survey.ExternalID,
                CollectorID = _survey.CollectorID,
                ContentCode = _survey.ContentCode,
                URL = _survey.URL,
                CreatedByUserID = userId,
                SurveyCategoryID = _survey.SurveyCategoryID,
                SurveySubCategoryID = _survey.SurveySubCategoryID,
                SurveyTypeID = _survey.SurveyTypeID,
                StartDate = TimeZoneInfo.ConvertTimeToUtc((DateTime)_survey.StartDate),
                EndDate = _survey.EndDate,
                Language = "English",
                IsPublish = _survey.IsPublish,
                Description = _survey.Description,
                FileName = _survey.FileName,
                IsActive = true,
                NormValue = _survey.NormValue,
                Days = _survey.Days,
                CreatedDate = Utilities.ConvertServerCurrentDateTimeToClientDateTime(DateTime.Now)
            };
            int SurveyID = HelperMethods.GetEntities().SaveEntity_Survey(survey);

            return SurveyID;
        }

        #endregion CreateSurvey

        #region UpdateSurvey

        public static void UpdateSurvey(Survey_model _survey)
        {
            Survey survey = HelperMethods.GetEntities().GetEntityById_Survey((short)_survey.SurveyID);

            survey.ID = _survey.SurveyID;
            survey.Title = _survey.Title;
            survey.ExternalTitle = _survey.ExternalTitle;
            survey.ExternalID = _survey.ExternalID;
            survey.CollectorID = _survey.CollectorID;
            survey.ContentCode = _survey.ContentCode;
            survey.URL = _survey.URL;
            survey.SurveyCategoryID = _survey.SurveyCategoryID;
            survey.SurveySubCategoryID = _survey.SurveySubCategoryID;
            survey.SurveyTypeID = _survey.SurveyTypeID;
            survey.StartDate = _survey.StartDate;
            survey.EndDate = _survey.EndDate;
            survey.Language = "English";
            survey.IsPublish = _survey.IsPublish;
            survey.FileName = _survey.FileName;
            survey.Description = _survey.Description;
            survey.IsActive = true;
            survey.NormValue = _survey.NormValue;
            survey.Days = _survey.Days;
            survey.ModifiedDate = Utilities.ConvertServerCurrentDateTimeToClientDateTime(DateTime.Now);

            HelperMethods.GetEntities().UpdateEntity_Survey(survey);
        }

        public static string UpdateSurveyIsActiveStatus(string Ids, bool Status)
        {
            string str = "";
            string[] values = Ids.Split(',');
            int length = values.Length;

            for (int i = 0; i < length; i++)
            {
                if (values[i] != "")
                {
                    short ID = Convert.ToInt16(values[i]);
                    str = HelperMethods.GetEntities().UpdateEntityIsActiveStatus_Survey(ID, Status);
                }
            }
            return str;
        }

        public static string UpdateSurveyFileName_Survey(int Id, string FileName)
        {
            try
            {
                return HelperMethods.GetEntities().UpdateSurveyFileName_Survey(Id, FileName);
            }
            catch (Exception)
            {
                return "";
            }
        }


        #endregion UpdateSurvey

        #region GetSurveyById

        public static Survey_model GetSurveyById(short id)
        {
            try
            {
                Survey survey = HelperMethods.GetEntities().GetEntityById_Survey(id);
                if (survey != null)
                {
                    return new Survey_model
                    {
                        SurveyID = survey.ID,
                        Title = survey.Title,
                        ExternalTitle = survey.ExternalTitle,
                        ExternalID = survey.ExternalID,
                        CollectorID = survey.CollectorID,
                        ContentCode = survey.ContentCode,
                        URL = survey.URL,
                        SurveyCategoryID = survey.SurveyCategoryID,
                        SurveySubCategoryID = survey.SurveySubCategoryID,
                        SurveyTypeID = survey.SurveyTypeID,
                        StartDate = survey.StartDate,
                        EndDate = survey.EndDate,
                        Language = "English",
                        IsPublish = survey.IsPublish,
                        FileName = survey.FileName,
                        Description = survey.Description,
                        IsActive = true,
                        NormValue = survey.NormValue,
                        Days = survey.Days,
                        CreatedDate = Utilities.ConvertServerCurrentDateTimeToClientDateTime(DateTime.Now)
                    };
                }
                return null;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        public static List<Survey_model> GetSurveyList()
        {
            try
            {
                List<Survey_model> SurveyList = HelperMethods.GetEntities().GetEntity_SurveyList().AsEnumerable().Select(x => new Survey_model
                {
                    SurveyID = x.ID,
                    Title = x.Title,
                    ExternalTitle = x.ExternalTitle,
                    ExternalID = x.ExternalID,
                    CollectorID = x.CollectorID,
                    ContentCode = x.ContentCode,
                    URL = x.URL,
                    SurveyCategoryID = x.SurveyCategoryID,
                    SurveySubCategoryID = x.SurveySubCategoryID,
                    SurveyTypeID = x.SurveyTypeID,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    Language = "English",
                    IsPublish = x.IsPublish,
                    Description = x.Description,
                    FileName = x.FileName,
                    IsActive = x.IsActive,
                    Days = x.Days,
                    NormValue = x.NormValue,
                    CreatedDate = Utilities.ConvertServerCurrentDateTimeToClientDateTime(DateTime.Now)
                }).ToList();

                return SurveyList;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        #endregion GetSurveyById

        #region SearchFilterSurvey
        public static usp_search_SurveyByFilterCustom_model SearchFilterSurvey(int TotalCount, int? StartIndex = -1, int? EndIndex = -1, string SearchString = null, bool? IsActive = null)
        {
            if (SearchString == "undefined")
                SearchString = null;

            List<usp_search_SurveyByFilter_Result> list = HelperMethods.GetEntities().GetEntityBySearchFilter_Survey(out TotalCount, StartIndex, EndIndex, SearchString, IsActive).ToList();

            var objList = list.ToList().AsEnumerable().Select(x =>
            new usp_search_SurveyByFilter_Result_model
            {
                RowNo = x.RowNo,
                ID = x.ID,
                Title = x.Title,
                ExternalTitle = x.ExternalTitle,
                ExternalID = x.ExternalID,
                ContentCode = x.ContentCode,
                URL = x.URL,
                CreatedByUserID = x.CreatedByUserID,
                SurveyCategoryID = x.SurveyCategoryID,
                SurvayCategoryName = x.SurvayCategoryName,
                SurveyTypeID = x.SurveyTypeID,
                SurvayTypeName = x.SurvayTypeName,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                Language = x.Language,
                IsPublish = x.IsPublish,
                Description = x.Description,
                FileName = x.FileName,
                CreatedDate = x.CreatedDate,
                Days = x.Days,
                IsActive = x.IsActive
            }).ToList();

            return (new usp_search_SurveyByFilterCustom_model { SurveySearchFilterList = objList, TotalCount = TotalCount });

        }

        #endregion SearchFilterSurvey

        #region DeleteSurvey
        public static string DeleteSurveyById(short SurveyId)
        {
            string str = "";
            bool isExist = HelperMethods.GetEntities().CheckPatientSurveyExistBySurveyID(SurveyId);

            if (!isExist)
            {
                HelperMethods.GetEntities().DeleteEntity_Survey(SurveyId);
                str = "Successfully Deleted!";
            }
            else
            {
                str = "Eprom already in Use";
            }
            return str;
        }

        public static string DeleteMultipleSurvey(string Ids)
        {
            string str = "";
            string[] values = Ids.Split(',');
            int length = values.Length;

            for (int i = 0; i < length; i++)
            {
                if (values[i] != "")
                {
                    short ID = Convert.ToInt16(values[i]);
                    bool isExist = HelperMethods.GetEntities().CheckPatientSurveyExistBySurveyID(ID);

                    if (!isExist)
                    {
                        HelperMethods.GetEntities().DeleteEntity_Survey(ID);
                        str = "Successfully Deleted!";
                    }
                    else
                    {
                        str = "ePROM already in Use";
                    }
                }
            }
            return str;
        }

        #endregion DeleteSurvey

        #region GetSurevyBySurveyCategoryID

        public static List<Survey_model> GetSurevy_By_SurveyCategoryID(short SurveyCategoryID, short SurveySubCategoryID)
        {
            try
            {
                List<Survey_model> SurveyList = HelperMethods.GetEntities().GetEntityBy_SurveyCategoryId_Survey(SurveyCategoryID, SurveySubCategoryID).AsEnumerable().Select(x => new Survey_model
                {
                    SurveyID = x.ID,
                    Title = x.Title,
                    ExternalTitle = x.ExternalTitle,
                    ExternalID = x.ExternalID,
                    ContentCode = x.ContentCode,
                    URL = x.URL,
                    SurveyCategoryID = x.SurveyCategoryID,
                    SurveySubCategoryID = x.SurveySubCategoryID,
                    SurveyTypeID = x.SurveyTypeID,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    Language = "English",
                    IsPublish = x.IsPublish,
                    FileName = x.FileName,
                    Description = x.Description,
                    IsActive = x.IsActive,
                    CreatedDate = Utilities.ConvertServerCurrentDateTimeToClientDateTime(DateTime.Now)
                }).ToList();

                return SurveyList;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }
        #endregion GetSurevyBySurveyCategoryID

        #region GetSurveyByExternalID

        public static Survey_model GetEntityBy_ExternalID_Survey(string ExternalID)
        {
            try
            {
                Survey survey = HelperMethods.GetEntities().GetEntityBy_ExternalID_Survey(ExternalID);

                if (survey != null)
                {
                    return new Survey_model
                    {
                        SurveyID = survey.ID,
                        Title = survey.Title,
                        ExternalTitle = survey.ExternalTitle,
                        ExternalID = survey.ExternalID,
                        CollectorID = survey.CollectorID,
                        ContentCode = survey.ContentCode,
                        URL = survey.URL,
                        SurveyCategoryID = survey.SurveyCategoryID,
                        SurveySubCategoryID = survey.SurveySubCategoryID,
                        SurveyTypeID = survey.SurveyTypeID,
                        StartDate = survey.StartDate,
                        EndDate = survey.EndDate,
                        Language = "English",
                        IsPublish = survey.IsPublish,
                        FileName = survey.FileName,
                        Description = survey.Description,
                        IsActive = survey.IsActive
                    };
                }
                return null;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }
        #endregion GetSurveyByExternalID

        #region GetEpromsBySurveyMonkey

        public static List<SurveyMonkey> GetEpromsBySurveyMonkey()
        {
            try
            {
                List<SystemFlags_model> systemFlagList = SystemFlags.GetSystemFlag();

                var listApiKey = systemFlagList.Where(x => x.SystemFlagName.Trim().ToLower() == "SurveyMonkeyAPIKey".ToLower()).FirstOrDefault();

                var listAccessToken = systemFlagList.Where(x => x.SystemFlagName.Trim().ToLower() == "SurverMonkeyAccessToken".ToLower()).FirstOrDefault();

                string result = HelperMethods.GetEntities().GetSurveyMonkeyEproms(listApiKey.Value, listAccessToken.Value);

                var list = JObject.Parse(result)["data"].ToObject<SurveyMonkey[]>();

                List<Survey_model> surveylist = SurveyClass.GetSurveyList();

                List<SurveyMonkey> newList = list.AsEnumerable().Select(x => new SurveyMonkey
                {
                    title = x.title,
                    id = x.id,
                    href = x.href,
                    collectorID = ""
                }).ToList();


                for (var j = 0; j < surveylist.Count(); j++)
                {
                    for (var i = 0; i < newList.Count(); i++)
                    {
                        if (newList[i].id == surveylist[j].ExternalID)
                        {
                            newList.RemoveAt(i);
                        }
                    }
                }

                return newList;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        #endregion GetEpromsBySurveyMonkey

        #region GetSurveyMonkeyCollectorIDBySurveyID

        public static string GetSurveyMonkeyCollectorIDBySurveyID(string SurveyID)
        {
            try
            {
                if (SurveyID != null)
                {
                    List<SystemFlags_model> systemFlagList = SystemFlags.GetSystemFlag();

                    var listApiKey = systemFlagList.Where(x => x.SystemFlagName.Trim().ToLower() == "SurveyMonkeyAPIKey".ToLower()).FirstOrDefault();

                    var listAccessToken = systemFlagList.Where(x => x.SystemFlagName.Trim().ToLower() == "SurverMonkeyAccessToken".ToLower()).FirstOrDefault();

                    string result = HelperMethods.GetEntities().GetSurveyMonkeyCollectorIDBySurveyID(listApiKey.Value, listAccessToken.Value, SurveyID);

                    var list = JObject.Parse(result)["data"].ToObject<SurveyMonkey[]>();

                    string CollectorID = "";

                    for (var i = 0; i < list.Count(); i++)
                    {
                        if (list[i].name.Contains("Web Link eHealthier"))
                        {
                            CollectorID = list[i].id;
                        }
                    }

                    return CollectorID;
                }
                else
                    return "";
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return "";
            }
        }

        #endregion GetSurveyMonkeyCollectorIDBySurveyID

        #region GetSurveyMonkeyCollectorDetails_CollectorID

        public static string GetSurveyMonkeyCollectorDetails_CollectorID(string CollectorId)
        {
            try
            {
                if (CollectorId != null)
                {
                    List<SystemFlags_model> systemFlagList = SystemFlags.GetSystemFlag();

                    var listApiKey = systemFlagList.Where(x => x.SystemFlagName.Trim().ToLower() == "SurveyMonkeyAPIKey".ToLower()).FirstOrDefault();

                    var listAccessToken = systemFlagList.Where(x => x.SystemFlagName.Trim().ToLower() == "SurverMonkeyAccessToken".ToLower()).FirstOrDefault();

                    string result = HelperMethods.GetEntities().GetSurveyMonkeyCollectorDetails_CollectorID(listApiKey.Value, listAccessToken.Value, CollectorId);

                    var strResult = JObject.Parse(result).ToString();

                    return strResult;
                }
                else
                {
                    return "";
                }
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return "";
            }
        }

        #endregion GetSurveyMonkeyCollectorDetails_CollectorID

        #region GetSurveyMonkey_SurveyDetails

        public static string GetSurveyMonkey_SurveyDetails(string SurveyID)
        {
            try
            {
                if (SurveyID != null)
                {
                    List<SystemFlags_model> systemFlagList = SystemFlags.GetSystemFlag();

                    var listApiKey = systemFlagList.Where(x => x.SystemFlagName.Trim().ToLower() == "SurveyMonkeyAPIKey".ToLower()).FirstOrDefault();

                    var listAccessToken = systemFlagList.Where(x => x.SystemFlagName.Trim().ToLower() == "SurverMonkeyAccessToken".ToLower()).FirstOrDefault();

                    string result = HelperMethods.GetEntities().GetSurveyMonkey_SurveyDetails(listApiKey.Value, listAccessToken.Value, SurveyID);

                    var strResult = JObject.Parse(result).ToString();

                    return strResult;
                }
                else
                {
                    return "";
                }
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return "";
            }
        }

        #endregion GetSurveyMonkey_SurveyDetails

        #region GetSurveyMonkeyResponseBy_CollectorID

        public static string GetSurveyMonkeyResponseBy_CollectorID(string CollectorID)
        {
            try
            {
                if (CollectorID != null)
                {
                    List<SystemFlags_model> systemFlagList = SystemFlags.GetSystemFlag();

                    var listApiKey = systemFlagList.Where(x => x.SystemFlagName.Trim().ToLower() == "SurveyMonkeyAPIKey".ToLower()).FirstOrDefault();

                    var listAccessToken = systemFlagList.Where(x => x.SystemFlagName.Trim().ToLower() == "SurverMonkeyAccessToken".ToLower()).FirstOrDefault();

                    string result = HelperMethods.GetEntities().GetSurveyMonkeyResponseBy_CollectorID(listApiKey.Value, listAccessToken.Value, CollectorID);

                    var strResult = JObject.Parse(result).ToString();
                    return strResult;
                }
                return "";
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return "";
            }
        }

        #endregion GetSurveyMonkeyResponseBy_CollectorID

        #region GetAllSurveyBySurveyMonkey

        public static List<SurveyMonkey> GetAllSurveyBySurveyMonkey()
        {
            try
            {
                List<SystemFlags_model> systemFlagList = SystemFlags.GetSystemFlag();

                var listApiKey = systemFlagList.Where(x => x.SystemFlagName.Trim().ToLower() == "SurveyMonkeyAPIKey".ToLower()).FirstOrDefault();

                var listAccessToken = systemFlagList.Where(x => x.SystemFlagName.Trim().ToLower() == "SurverMonkeyAccessToken".ToLower()).FirstOrDefault();

                string result = HelperMethods.GetEntities().GetSurveyMonkeyEproms(listApiKey.Value, listAccessToken.Value);

                var list = JObject.Parse(result)["data"].ToObject<SurveyMonkey[]>();

                List<Survey_model> surveylist = SurveyClass.GetSurveyList();

                List<SurveyMonkey> newList = list.AsEnumerable().Select(x => new SurveyMonkey
                {
                    title = x.title,
                    id = x.id,
                    href = x.href
                }).ToList();

                return newList;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        #endregion GetAllSurveyBySurveyMonkey

        #region GetSurveyMonkeypageDetailsBySurveyID

        public static List<SurveyMonkey_Page> GetSurveyMonkeypageDetailsBySurveyID(string SurveyID)
        {
            try
            {
                List<SystemFlags_model> systemFlagList = SystemFlags.GetSystemFlag();

                var listApiKey = systemFlagList.Where(x => x.SystemFlagName.Trim().ToLower() == "SurveyMonkeyAPIKey".ToLower()).FirstOrDefault();

                var listAccessToken = systemFlagList.Where(x => x.SystemFlagName.Trim().ToLower() == "SurverMonkeyAccessToken".ToLower()).FirstOrDefault();

                string result = HelperMethods.GetEntities().GetSurveyMonkeyPagesBySurveyID(listApiKey.Value, SurveyID, listAccessToken.Value);

                var list = JObject.Parse(result)["data"].ToObject<SurveyMonkey_Page[]>();


                List<SurveyMonkey_Page> newList = list.AsEnumerable().Select(x => new SurveyMonkey_Page
                {
                    title = x.title,
                    description = x.description,
                    position = x.position,
                    id = x.id,
                    href = x.href
                }).ToList();

                return newList;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        #endregion GetSurveyMonkeypageDetailsBySurveyID

        #region GetSurveyMonkeyQuestionsByPageID

        public static List<SurveyMonkey_Question> GetSurveyMonkeyQuestionsByPageID(string SurveyID, string PageID)
        {
            try
            {
                List<SystemFlags_model> systemFlagList = SystemFlags.GetSystemFlag();

                var listApiKey = systemFlagList.Where(x => x.SystemFlagName.Trim().ToLower() == "SurveyMonkeyAPIKey".ToLower()).FirstOrDefault();

                var listAccessToken = systemFlagList.Where(x => x.SystemFlagName.Trim().ToLower() == "SurverMonkeyAccessToken".ToLower()).FirstOrDefault();

                string result = HelperMethods.GetEntities().GetSurveyMonkeyQuestionsListByPageID(listApiKey.Value, listAccessToken.Value, SurveyID, PageID);

                var list = JObject.Parse(result)["data"].ToObject<SurveyMonkey_Question[]>();
                var count = JObject.Parse(result)["total"].ToString();

                List<SurveyMonkey_Question> newList = list.AsEnumerable().Select(x => new SurveyMonkey_Question
                {
                    heading = x.heading,
                    position = x.position,
                    id = x.id,
                    href = x.href
                }).ToList();

                return newList;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        #endregion GetSurveyMonkeyQuestionsByPageID

        #region GetSurveyMonkeydetailsForDashboard

        public static List<SurveyMonkey> GetSurveyMonkeydetailsForDashboard()
        {
            try
            {
                var strSurvey = SurveyClass.GetAllSurveyBySurveyMonkey();
                if (strSurvey != null)
                {
                    List<SurveyMonkey_Page> pagelist = new List<SurveyMonkey_Page>();

                    List<SurveyMonkey> questioncount = new List<SurveyMonkey>();

                    var QuestionList = strSurvey.AsEnumerable().Select(x => new SurveyMonkey
                    {
                        title = x.title,
                        id = x.id,
                        href = "",
                        count = 0,
                        Questions = { }
                    }).ToList();

                    for (var i = 0; i < QuestionList.Count(); i++)
                    {
                        if (QuestionList[i].id != null)
                        {
                            pagelist = SurveyClass.GetSurveyMonkeypageDetailsBySurveyID(QuestionList[i].id);
                            if (pagelist != null)
                            {
                                var list = SurveyClass.GetSurveyMonkeyQuestionsByPageID(QuestionList[i].id, pagelist[0].id);
                                if (list != null)
                                {
                                    QuestionList[i].Questions = list;
                                    QuestionList[i].count = list.Count();
                                }
                            }
                        }
                    }

                    return QuestionList;
                }
                return null;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        #endregion GetSurveyMonkeydetailsForDashboard
    }

    public static class SurveyTypeClass
    {
        #region GetSurveyType

        public static List<SurveyTypes_model> GetSurveyTypes()
        {
            List<SurveyTypes_model> SurveyType = HelperMethods.GetEntities().GetEntity_SurveyTypes().ToList().AsEnumerable().Select(x =>
             new SurveyTypes_model
             {
                 ID = x.ID,
                 SurvayTypeName = x.SurvayTypeName,
                 Description = x.Description,
                 IsActive = x.IsActive,
                 CreatedDate = x.CreatedDate
             }).ToList();

            return SurveyType;
        }

        #endregion GetSurveyType       
    }

    public static class PatientSurveyClass
    {
        #region CreatePatientSurvey

        public static string CreatePatientSurvey(PatientSurvey_model _survey)
        {
            try
            {
                string patientSurveyId = "";

                bool isExist = HelperMethods.GetEntities().CheckPatientSurveyExist(_survey.ProviderID, _survey.OrganizationID, _survey.PracticeID, _survey.PatientID, (int)_survey.SurveyID);
                DateTime? StartDate = null;
                DateTime? EndDate = null;

                if (!string.IsNullOrEmpty(_survey.StartDate))
                {
                    string[] StartDateArray = _survey.StartDate.Split('/');
                    if (StartDateArray.Length > 0)
                    {
                        StartDate = new DateTime(Convert.ToInt32(StartDateArray[2]), Convert.ToInt32(StartDateArray[0]), Convert.ToInt32(StartDateArray[1]), 0, 0, 0);
                    }
                }

                if (!string.IsNullOrEmpty(_survey.EndDate))
                {
                    string[] EndDateArray = _survey.EndDate.Split('/');
                    if (EndDateArray.Length > 0)
                    {
                        EndDate = new DateTime(Convert.ToInt32(EndDateArray[2]), Convert.ToInt32(EndDateArray[0]), Convert.ToInt32(EndDateArray[1]), 0, 0, 0);
                    }
                }

                if (isExist)
                {
                    var patientsurvey = new PatientSurvey
                    {
                        ID = _survey.ID,
                        PatientID = _survey.PatientID,
                        SurveyID = _survey.SurveyID,
                        OrganizationID = _survey.OrganizationID,
                        PracticeID = _survey.PracticeID,
                        ProviderID = _survey.ProviderID,
                        StartDate = Utilities.ConvertServerCurrentDateTimeToClientDateTime(StartDate),
                        EndDate = Utilities.ConvertServerCurrentDateTimeToClientDateTime(EndDate),
                        IsActive = _survey.IsActive,
                        ModifiedDate = Utilities.ConvertServerCurrentDateTimeToClientDateTime(DateTime.Now),
                        IsSend = true
                    };

                    patientSurveyId = HelperMethods.GetEntities().UpdateEntity_PatientSurvey(patientsurvey);

                    #region PathwayFunctionality
                    if (_survey.PathwayID != null && _survey.PathwayID != new Guid("00000000-0000-0000-0000-000000000000"))
                    {
                        int Count_PatientSurvey_Pathway_PatientSurveyStatus = HelperMethods.GetEntities().Check_PatientSurvey_Pathway_PatientSurveyStatus(_survey.ProviderID, _survey.OrganizationID, _survey.PracticeID, _survey.PatientID, (int)_survey.SurveyID, _survey.ID, _survey.PathwayID);

                        if (Count_PatientSurvey_Pathway_PatientSurveyStatus > 0)
                        {
                            //if entry exits but status is not present then update that entry
                            Guid ID_PatientSurvey_Pathway_PatientSurveyStatus = HelperMethods.GetEntities().Check_PatientSurvey_Pathway_PatientSurveyStatus_WithOut_Status(_survey.ProviderID, _survey.OrganizationID, _survey.PracticeID, _survey.PatientID, (int)_survey.SurveyID, _survey.ID, _survey.PathwayID);

                            if (ID_PatientSurvey_Pathway_PatientSurveyStatus != null && ID_PatientSurvey_Pathway_PatientSurveyStatus != new Guid("00000000-0000-0000-0000-000000000000"))
                            {
                                //as there is PatientSurveyStatusID is not available so will update that same entry.
                                var PatientSurvey_Pathway_PatientSurveyStatus = new PatientSurvey_Pathway_PatientSurveyStatus
                                {
                                    ID = ID_PatientSurvey_Pathway_PatientSurveyStatus,
                                    PatientSurveyID = _survey.ID,
                                    PathwayID = new Guid(_survey.PathwayID.ToString()),
                                    UpdatedOn = Utilities.ConvertServerCurrentDateTimeToClientDateTime(DateTime.Now)
                                };

                                HelperMethods.GetEntities().UpdateEntity_PatientSurvey_Pathway_PatientSurveyStatus(PatientSurvey_Pathway_PatientSurveyStatus);
                                _survey.PatientSurvey_Pathway_PatientSurveyStatus_ID = ID_PatientSurvey_Pathway_PatientSurveyStatus;
                            }
                            else
                            {

                                //as there is PatientSurveyStatusID present so will create new entry.
                                var PatientSurvey_Pathway_PatientSurveyStatus = new PatientSurvey_Pathway_PatientSurveyStatus
                                {
                                    ID = Guid.NewGuid(),
                                    PatientSurveyID = _survey.ID,
                                    PathwayID = new Guid(_survey.PathwayID.ToString()),
                                    CreatedOn = Utilities.ConvertServerCurrentDateTimeToClientDateTime(DateTime.Now)
                                };

                                Guid PatientSurvey_Pathway_PatientSurveyStatus_ID = HelperMethods.GetEntities().SaveEntity_PatientSurvey_Pathway_PatientSurveyStatus(PatientSurvey_Pathway_PatientSurveyStatus);
                                _survey.PatientSurvey_Pathway_PatientSurveyStatus_ID = PatientSurvey_Pathway_PatientSurveyStatus_ID;

                            }
                        }
                        else
                        {
                            //Made new entry.
                            var PatientSurvey_Pathway_PatientSurveyStatus = new PatientSurvey_Pathway_PatientSurveyStatus
                            {
                                ID = Guid.NewGuid(),
                                PatientSurveyID = _survey.ID,
                                PathwayID = new Guid(_survey.PathwayID.ToString()),
                                CreatedOn = Utilities.ConvertServerCurrentDateTimeToClientDateTime(DateTime.Now)
                            };

                            Guid PatientSurvey_Pathway_PatientSurveyStatus_ID = HelperMethods.GetEntities().SaveEntity_PatientSurvey_Pathway_PatientSurveyStatus(PatientSurvey_Pathway_PatientSurveyStatus);
                            _survey.PatientSurvey_Pathway_PatientSurveyStatus_ID = PatientSurvey_Pathway_PatientSurveyStatus_ID;
                        }
                    }
                    #endregion

                    AssignEpromsDetails(_survey.PatientID, patientsurvey.OrganizationID, patientsurvey.PracticeID, patientsurvey.ProviderID, _survey.SurveyID, patientSurveyId);
                }
                else
                {
                    var patientsurvey = new PatientSurvey
                    {
                        ID = Guid.NewGuid(),
                        PatientID = _survey.PatientID,
                        SurveyID = _survey.SurveyID,
                        OrganizationID = _survey.OrganizationID,
                        PracticeID = _survey.PracticeID,
                        ProviderID = _survey.ProviderID,
                        StartDate = Utilities.ConvertServerCurrentDateTimeToClientDateTime(StartDate),
                        EndDate = Utilities.ConvertServerCurrentDateTimeToClientDateTime(EndDate),
                        IsActive = true,
                        CreatedDate = Utilities.ConvertServerCurrentDateTimeToClientDateTime(DateTime.Now),
                        IsSend = true
                    };

                    patientSurveyId = HelperMethods.GetEntities().SaveEntity_PatientSurvey(patientsurvey);

                    #region PathwayFunctionality
                    if (_survey.PathwayID != null && _survey.PathwayID != new Guid("00000000-0000-0000-0000-000000000000"))
                    {
                        int Count_PatientSurvey_Pathway_PatientSurveyStatus = HelperMethods.GetEntities().Check_PatientSurvey_Pathway_PatientSurveyStatus(_survey.ProviderID, _survey.OrganizationID, _survey.PracticeID, _survey.PatientID, (int)_survey.SurveyID, new Guid(patientSurveyId), _survey.PathwayID);

                        if (Count_PatientSurvey_Pathway_PatientSurveyStatus > 0)
                        {
                            //if entry exits but status is not present then update that entry
                            Guid ID_PatientSurvey_Pathway_PatientSurveyStatus = HelperMethods.GetEntities().Check_PatientSurvey_Pathway_PatientSurveyStatus_WithOut_Status(_survey.ProviderID, _survey.OrganizationID, _survey.PracticeID, _survey.PatientID, (int)_survey.SurveyID, new Guid(patientSurveyId), _survey.PathwayID);

                            if (ID_PatientSurvey_Pathway_PatientSurveyStatus != null && ID_PatientSurvey_Pathway_PatientSurveyStatus != new Guid("00000000-0000-0000-0000-000000000000"))
                            {
                                //as there is PatientSurveyStatusID is not available so will update that same entry.
                                var PatientSurvey_Pathway_PatientSurveyStatus = new PatientSurvey_Pathway_PatientSurveyStatus
                                {
                                    ID = ID_PatientSurvey_Pathway_PatientSurveyStatus,
                                    PatientSurveyID = new Guid(patientSurveyId),
                                    PathwayID = new Guid(_survey.PathwayID.ToString()),
                                    UpdatedOn = Utilities.ConvertServerCurrentDateTimeToClientDateTime(DateTime.Now)
                                };

                                HelperMethods.GetEntities().UpdateEntity_PatientSurvey_Pathway_PatientSurveyStatus(PatientSurvey_Pathway_PatientSurveyStatus);
                                _survey.PatientSurvey_Pathway_PatientSurveyStatus_ID = ID_PatientSurvey_Pathway_PatientSurveyStatus;
                            }
                            else
                            {

                                //as there is PatientSurveyStatusID present so will create new entry.
                                var PatientSurvey_Pathway_PatientSurveyStatus = new PatientSurvey_Pathway_PatientSurveyStatus
                                {
                                    ID = Guid.NewGuid(),
                                    PatientSurveyID = new Guid(patientSurveyId),
                                    PathwayID = new Guid(_survey.PathwayID.ToString()),
                                    CreatedOn = Utilities.ConvertServerCurrentDateTimeToClientDateTime(DateTime.Now)
                                };

                                Guid PatientSurvey_Pathway_PatientSurveyStatus_ID = HelperMethods.GetEntities().SaveEntity_PatientSurvey_Pathway_PatientSurveyStatus(PatientSurvey_Pathway_PatientSurveyStatus);
                                _survey.PatientSurvey_Pathway_PatientSurveyStatus_ID = PatientSurvey_Pathway_PatientSurveyStatus_ID;

                            }
                        }
                        else
                        {
                            //Made new entry.
                            var PatientSurvey_Pathway_PatientSurveyStatus = new PatientSurvey_Pathway_PatientSurveyStatus
                            {
                                ID = Guid.NewGuid(),
                                PatientSurveyID = new Guid(patientSurveyId),
                                PathwayID = new Guid(_survey.PathwayID.ToString()),
                                CreatedOn = Utilities.ConvertServerCurrentDateTimeToClientDateTime(DateTime.Now)
                            };

                            Guid PatientSurvey_Pathway_PatientSurveyStatus_ID = HelperMethods.GetEntities().SaveEntity_PatientSurvey_Pathway_PatientSurveyStatus(PatientSurvey_Pathway_PatientSurveyStatus);
                            _survey.PatientSurvey_Pathway_PatientSurveyStatus_ID = PatientSurvey_Pathway_PatientSurveyStatus_ID;
                        }
                    }
                    #endregion

                    AssignEpromsDetails(_survey.PatientID, patientsurvey.OrganizationID, patientsurvey.PracticeID, patientsurvey.ProviderID, _survey.SurveyID, patientSurveyId);
                }

                return patientSurveyId;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return "";
            }
        }

        public static string AddPatientSurveyEProms(Guid organizationID, Guid practiceID, Guid providerID, Guid patientID, string surveyID)
        {
            return HelperMethods.GetEntities().AddPatientSurveyEProms(organizationID, practiceID, providerID, patientID, surveyID);
        }

        #endregion CreatePatientSurvey

        #region AssignEpromsEmail

        public static string AssignEpromsDetails(Guid? PatientId, Guid? OrganizationId, Guid? PracticeId, Guid? ProviderId, int? SurveyId, string PatientSurveyId)
        {
            string returnstr = "";
            var PatientDetails = HelperMethods.GetEntities().GetEntityById_Patient((Guid)PatientId);
            var Patient_Provider = HelperMethods.GetEntities().GetPatientDetailsByUserID(PatientId);
            var SurveyDetails = HelperMethods.GetEntities().GetEntityById_Survey((short)SurveyId);


            if (Patient_Provider != null)
            {
                //var ProviderId = Patient_Provider[0].ProviderID;
                var provider = HelperMethods.GetEntities().GetProviderDetails(ProviderId);
                if (provider != null)
                {
                    var userId = provider.UserID;
                    var user = HelperMethods.GetEntities().GetUserDetailsByUserId(userId);
                    if (user != null)
                    {
                        var ProviderName = user.FirstName + " " + user.LastName;
                        var PatientUser = HelperMethods.GetEntities().GetUserDetailsByUserId(PatientDetails.UserID);
                        if (PatientUser != null && SurveyDetails != null)
                        {
                            returnstr = EmailClass.SendEpromsUrltoPatient((Guid)PatientId, (Guid)OrganizationId, (Guid)PracticeId, (Guid)ProviderId, (int)SurveyId, PatientUser.Email, SurveyDetails.ExternalTitle, SurveyDetails.ContentCode, ProviderName, PatientUser.FirstName + " " + PatientUser.LastName, PatientSurveyId);
                        }
                    }
                }
            }
            return returnstr;
        }

        #endregion AssignEpromsEmail

        #region GetPatientSurvey

        public static List<PatientSurvey_model> GetSurveyByPatient_Provider_Org_Practice_IDs(Guid PatientId, Guid ProviderID, Guid OrganizationID, Guid PracticeID, bool isAllPatient, bool IsCompleted)
        {
            try
            {
                List<PatientSurvey_model> SurveyList = HelperMethods.GetEntities().GetEntityByPatient_Provider_Org_Practice_ID_PatientSurvey(PatientId, ProviderID, OrganizationID, PracticeID, isAllPatient, IsCompleted).AsEnumerable().Select(x => new PatientSurvey_model
                {
                    ID = x.ID,
                    PatientID = x.PatientID,
                    ProviderID = x.ProviderID,
                    ProviderName = x.Provider.UserDetail.FirstName + " " + x.Provider.UserDetail.LastName,
                    SurveyID = x.SurveyID,
                    OrganizationID = x.OrganizationID,
                    OrganizationName = x.Organization.OrganizationName,
                    PracticeID = x.PracticeID,
                    PracticeName = x.Practice.PracticeName,
                    StartDate = x.StartDate == null ? null : Convert.ToDateTime(x.StartDate).ToString("MM'/'dd'/'yyyy"),
                    EndDate = x.EndDate == null ? null : Convert.ToDateTime(x.EndDate).ToString("MM'/'dd'/'yyyy"),
                    IsActive = x.IsActive,
                    ExternalID = x.Survey.ExternalID,
                    ExternalTitle = x.Survey.ExternalTitle,
                    ContentCode = x.Survey.ContentCode,
                    CollectorID = x.Survey.CollectorID,
                    IsSend = x.IsSend,
                    PatientSurvey_Pathway_PatientSurveyStatus_ID = x.PatientSurvey_Pathway_PatientSurveyStatus.AsEnumerable().Where(y => y.ID != null)
                              .Select(z => z.ID)
                              .FirstOrDefault(),
                    PathwayID = x.PatientSurvey_Pathway_PatientSurveyStatus.AsEnumerable().Where(y => y.PathwayID != null)
                              .Select(z => z.PathwayID)
                              .FirstOrDefault(),
                    Status = PatientSurveyClass.SetSurveyStatus((Guid)x.PatientID, (Guid)x.OrganizationID, (Guid)x.PracticeID, (Guid)x.ProviderID, (int)x.SurveyID),

                    User = new User_model { UserName = x.Patient.UserDetail.UserName, FirstName = x.Patient.UserDetail.FirstName, MiddleName = x.Patient.UserDetail.MiddleName, LastName = x.Patient.UserDetail.LastName, DOB = x.Patient.UserDetail.DOB, Gender = x.Patient.UserDetail.Gender, Email = x.Patient.UserDetail.Email },

                    Patient = new Patient_model { MedicareNumber = x.Patient.MedicareNumber, IHINumber = x.Patient.IHINumber },
                }).OrderBy(x => x.ExternalTitle).ToList();

                return SurveyList;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        public static List<PatientSurvey_model> GetSurveyByPatient_Org_Practice_IDs(Guid PatientId, Guid ProviderID, Guid OrganizationID, Guid PracticeID, bool isAllPatient, bool IsCompleted)
        {
            try
            {
                List<PatientSurvey_model> SurveyList = HelperMethods.GetEntities().GetEntityByPatient_Org_Practice_ID_PatientSurvey(PatientId, ProviderID, OrganizationID, PracticeID, isAllPatient, IsCompleted).AsEnumerable().Select(x => new PatientSurvey_model
                {
                    ID = x.ID,
                    PatientID = x.PatientID,
                    ProviderID = x.ProviderID,
                    ProviderName = x.Provider.UserDetail.FirstName + " " + x.Provider.UserDetail.LastName,
                    SurveyID = x.SurveyID,
                    OrganizationID = x.OrganizationID,
                    OrganizationName = x.Organization.OrganizationName,
                    PracticeID = x.PracticeID,
                    PracticeName = x.Practice.PracticeName,
                    StartDate = x.StartDate == null ? null : Convert.ToDateTime(x.StartDate).ToString("MM'/'dd'/'yyyy"),
                    EndDate = x.EndDate == null ? null : Convert.ToDateTime(x.EndDate).ToString("MM'/'dd'/'yyyy"),
                    IsActive = x.IsActive,
                    ExternalID = x.Survey.ExternalID,
                    ExternalTitle = x.Survey.ExternalTitle,
                    ContentCode = x.Survey.ContentCode,
                    CollectorID = x.Survey.CollectorID,
                    IsSend = x.IsSend,
                    PatientSurvey_Pathway_PatientSurveyStatus_ID = x.PatientSurvey_Pathway_PatientSurveyStatus.AsEnumerable().Where(y => y.ID != null)
                              .Select(z => z.ID)
                              .FirstOrDefault(),
                    PathwayID = x.PatientSurvey_Pathway_PatientSurveyStatus.AsEnumerable().Where(y => y.PathwayID != null)
                              .Select(z => z.PathwayID)
                              .FirstOrDefault(),
                    Status = PatientSurveyClass.SetSurveyStatus((Guid)x.PatientID, (Guid)x.OrganizationID, (Guid)x.PracticeID, (Guid)x.ProviderID, (int)x.SurveyID),

                    User = new User_model { UserName = x.Patient.UserDetail.UserName, FirstName = x.Patient.UserDetail.FirstName, MiddleName = x.Patient.UserDetail.MiddleName, LastName = x.Patient.UserDetail.LastName, DOB = x.Patient.UserDetail.DOB, Gender = x.Patient.UserDetail.Gender, Email = x.Patient.UserDetail.Email },

                    Patient = new Patient_model { MedicareNumber = x.Patient.MedicareNumber, IHINumber = x.Patient.IHINumber },
                }).OrderBy(x => x.ExternalTitle).ToList();

                return SurveyList;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        public static string CheckSurveyWasCompletedValidDate(DateTime? StartDate, DateTime? EndDate, DateTime? CompletedDate, string Status)
        {
            try
            {
                string ReturnVal = "";
                bool isCompletedInRange = true;


                if (CompletedDate != null)
                {
                    isCompletedInRange = PatientSurveyClass.CheckSurveyCompletedDate(StartDate, EndDate, CompletedDate);
                }

                DateTime? TodayDate = Utilities.ConvertServerCurrentDateTimeToClientDateTime(DateTime.Now).Value.Date;

                if (TodayDate < StartDate.Value.Date)
                {
                    ReturnVal = "Disabled";
                }
                else if (!isCompletedInRange && EndDate.Value.Date < TodayDate)
                {
                    ReturnVal = "Expired";
                }
                else if ((!isCompletedInRange && Status == "Completed") || Status == "")
                {
                    ReturnVal = "Pending";
                }
                else if (!isCompletedInRange)
                {
                    ReturnVal = "Pending";
                }
                else
                {
                    ReturnVal = Status;
                }

                return ReturnVal;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return "";
            }
        }

        public static bool CheckSurveyCompletedDate(DateTime? StartDate, DateTime? EndDate, DateTime? CompletedDate)
        {
            try
            {
                bool isValid = true;
                if (StartDate != null && EndDate != null && CompletedDate != null)
                {
                    if (CompletedDate.Value.Date >= StartDate.Value.Date && CompletedDate.Value.Date <= EndDate.Value.Date)
                    {
                        isValid = true;
                    }
                    else
                    {
                        isValid = false;
                    }
                }
                return isValid;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return false;
            }
        }

        public static List<PatientSurvey_model> GetPatientSurveyBy_patientId_ProviderId(Guid PatientId, Guid ProviderID, bool isCompleted)
        {
            try
            {
                List<PatientSurvey_model> SurveyList = HelperMethods.GetEntities().GetEntity_PatientSurvey_PatientID(PatientId, ProviderID, isCompleted).AsEnumerable().Select(x => new PatientSurvey_model
                {
                    ID = x.ID,
                    PatientID = x.PatientID,
                    ProviderID = x.ProviderID,
                    ProviderName = x.Provider.UserDetail.FirstName + " " + x.Provider.UserDetail.LastName,
                    SurveyID = x.SurveyID,
                    OrganizationID = x.OrganizationID,
                    OrganizationName = x.Organization.OrganizationName,
                    PracticeID = x.PracticeID,
                    PracticeName = x.Practice.PracticeName,
                    StartDate = x.StartDate == null ? null : x.StartDate.Value.ToString("MM'/'dd'/'yyyy", CultureInfo.InvariantCulture),
                    EndDate = x.EndDate == null ? null : x.EndDate.Value.ToString("MM'/'dd'/'yyyy", CultureInfo.InvariantCulture),
                    IsActive = x.IsActive,
                    ExternalID = x.Survey.ExternalID,
                    ExternalTitle = x.Survey.ExternalTitle,
                    ContentCode = x.Survey.ContentCode,
                    CollectorID = x.Survey.CollectorID,
                    IsSend = x.IsSend,
                    Status = PatientSurveyClass.SetSurveyStatus((Guid)x.PatientID, (Guid)x.OrganizationID, (Guid)x.PracticeID, (Guid)x.ProviderID, (int)x.SurveyID),
                    CreatedDate = PatientSurveyStatusClass.GetPatientSurveyStatus((Guid)x.PatientID, (Guid)x.OrganizationID, (Guid)x.PracticeID, (Guid)x.ProviderID, (int)x.SurveyID) == null ? null : PatientSurveyStatusClass.GetPatientSurveyStatus((Guid)x.PatientID, (Guid)x.OrganizationID, (Guid)x.PracticeID, (Guid)x.ProviderID, (int)x.SurveyID).CreatedDate,
                    User = new User_model { UserName = x.Patient.UserDetail.UserName, FirstName = x.Patient.UserDetail.FirstName, MiddleName = x.Patient.UserDetail.MiddleName, LastName = x.Patient.UserDetail.LastName, DOB = x.Patient.UserDetail.DOB, Gender = x.Patient.UserDetail.Gender, Email = x.Patient.UserDetail.Email },
                    Patient = new Patient_model { MedicareNumber = x.Patient.MedicareNumber, IHINumber = x.Patient.IHINumber },
                    isSurveyValid = PatientSurveyStatusClass.CheckSurveyIsValidToSubmit(x.ID, x.StartDate, x.EndDate, PatientSurveyStatusClass.GetPatientSurveyStatus((Guid)x.PatientID, (Guid)x.OrganizationID, (Guid)x.PracticeID, (Guid)x.ProviderID, (int)x.SurveyID) == null ? null : PatientSurveyStatusClass.GetPatientSurveyStatus((Guid)x.PatientID, (Guid)x.OrganizationID, (Guid)x.PracticeID, (Guid)x.ProviderID, (int)x.SurveyID).CreatedDate, PatientSurveyClass.SetSurveyStatus((Guid)x.PatientID, (Guid)x.OrganizationID, (Guid)x.PracticeID, (Guid)x.ProviderID, (int)x.SurveyID))
                }).ToList();

                return SurveyList;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        public static PatientSurvey_model GetEntityByPatient_Org_Practice_Provider_Survey_PatientSurvey(Guid? PatientId, Guid? OrganizationId, Guid? PracticeId, Guid? ProviderId, int SurveyId)
        {
            try
            {
                PatientSurvey patientSurvey = HelperMethods.GetEntities().GetEntity_PatientSurvey((Guid)PatientId, OrganizationId, PracticeId, ProviderId, (int)SurveyId);
                if (patientSurvey != null)
                {
                    DateTime? StartDate = null;
                    DateTime? EndDate = null;

                    if (patientSurvey.StartDate != null)
                    {
                        StartDate = patientSurvey.StartDate.Value;
                    }

                    if (patientSurvey.EndDate != null)
                    {
                        EndDate = patientSurvey.EndDate.Value;
                    }

                    return new PatientSurvey_model
                    {
                        ID = patientSurvey.ID,
                        StartDate = StartDate != null ? StartDate.Value.ToString("MM'/'dd'/'yyyy", CultureInfo.InvariantCulture) : "",
                        EndDate = EndDate != null ? EndDate.Value.ToString("MM'/'dd'/'yyyy", CultureInfo.InvariantCulture) : "",
                        ContentCode = patientSurvey.Survey.ContentCode,
                        CollectorID = patientSurvey.Survey.CollectorID,
                        PatientID = patientSurvey.PatientID,
                        ExternalID = patientSurvey.Survey.ExternalID,
                        ExternalTitle = patientSurvey.Survey.ExternalTitle,
                        Email = patientSurvey.Patient.UserDetail.Email,
                        IsSurveyWithValidDate = PatientSurveyClass.CheckSurveyIsInValidDate(patientSurvey.StartDate, patientSurvey.EndDate)
                    };
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        public static List<PatientSurvey_model> GetEntityByPatient_Survey_PatientSurvey(Guid PatientId, int SurveyId)
        {
            try
            {
                List<PatientSurvey> patientSurvey = HelperMethods.GetEntities().GetEntityByPatientIdSurveyID_PatientSurvey((Guid)PatientId, (int)SurveyId);
                if (patientSurvey != null && patientSurvey.Count > 0)
                {
                    for (int i = 0; i < patientSurvey.Count; i++)
                    {
                        PatientSurvey psurvey = patientSurvey[i];
                        if (psurvey.StartDate != null)
                        {
                            psurvey.StartDate = psurvey.StartDate.Value;
                        }

                        if (psurvey.EndDate != null)
                        {
                            psurvey.EndDate = psurvey.EndDate.Value;
                        }
                    }

                    return patientSurvey.Select(x => new PatientSurvey_model
                    {
                        ID = x.ID,
                        StartDate = x.StartDate != null ? x.StartDate.Value.ToString("MM'/'dd'/'yyyy", CultureInfo.InvariantCulture) : null,
                        EndDate = x.EndDate != null ? x.EndDate.Value.ToString("MM'/'dd'/'yyyy", CultureInfo.InvariantCulture) : null,
                        ContentCode = x.Survey.ContentCode,
                        CollectorID = x.Survey.CollectorID,
                        PatientID = x.PatientID,
                        ExternalID = x.Survey.ExternalID,
                        ExternalTitle = x.Survey.ExternalTitle,
                        Email = x.Patient.UserDetail.Email,
                        IsSurveyWithValidDate = PatientSurveyClass.CheckSurveyIsInValidDate(x.StartDate, x.EndDate)
                    }).ToList();
                }
                return null;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        public static PatientSurvey_model GetPatientSurveyByID(Guid PatientSurveyId)
        {
            try
            {
                var patientSurvey = HelperMethods.GetEntities().GetEntityByID_PatientSurveyList(PatientSurveyId);
                if (patientSurvey != null && patientSurvey.Count() > 0)
                {
                    PatientSurvey pSurvey = patientSurvey[0];
                    DateTime? StartDate = null;
                    DateTime? EndDate = null;

                    if (pSurvey.StartDate != null)
                    {
                        StartDate = pSurvey.StartDate.Value;
                    }

                    if (pSurvey.EndDate != null)
                    {
                        EndDate = pSurvey.EndDate.Value;
                    }

                    return new PatientSurvey_model
                    {
                        ID = pSurvey.ID,
                        SurveyID = pSurvey.SurveyID,
                        StartDate = StartDate != null ? StartDate.Value.ToString("MM'/'dd'/'yyyy", CultureInfo.InvariantCulture) : "",
                        EndDate = EndDate != null ? EndDate.Value.ToString("MM'/'dd'/'yyyy", CultureInfo.InvariantCulture) : "",
                        ContentCode = pSurvey.Survey.ContentCode,
                        CollectorID = pSurvey.Survey.CollectorID,
                        PatientID = pSurvey.PatientID,
                        ProviderID = pSurvey.ProviderID,
                        ExternalID = pSurvey.Survey.ExternalID,
                        ExternalTitle = pSurvey.Survey.ExternalTitle,
                        Email = pSurvey.Patient.UserDetail.Email,
                        OrganizationID = pSurvey.OrganizationID,
                        PracticeID = pSurvey.PracticeID,
                        IsSurveyWithValidDate = PatientSurveyClass.CheckSurveyIsInValidDate(pSurvey.StartDate, pSurvey.EndDate)
                    };
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        public static string CheckInvalidDate(Guid PatientSurveyId)
        {
            string errormsg = "";
            try
            {
                var patientSurvey = HelperMethods.GetEntities().GetEntityByID_PatientSurveyList(PatientSurveyId);
                if (patientSurvey != null && patientSurvey.Count() > 0)
                {
                    PatientSurvey pSurvey = patientSurvey[0];
                    DateTime? StartDate = null;
                    DateTime? EndDate = null;

                    if (pSurvey.StartDate != null)
                    {
                        StartDate = pSurvey.StartDate.Value;
                    }

                    if (pSurvey.EndDate != null)
                    {
                        EndDate = pSurvey.EndDate.Value;
                    }

                    DateTime? CurrentDateTime = Utilities.ConvertServerCurrentDateTimeToClientDateTime(DateTime.Now);
                    if (StartDate != null && EndDate != null && CurrentDateTime != null)
                    {
                        if (CurrentDateTime.Value.Date <= StartDate.Value.Date && CurrentDateTime.Value.Date <= EndDate.Value.Date)
                        {
                            string sd = StartDate != null ? StartDate.Value.ToString("dd'/'MM'/'yyyy", CultureInfo.InvariantCulture) : "";
                            errormsg = "This ePROM will be disabled upto " + sd;
                        }
                        else
                        {
                            string ed = EndDate != null ? EndDate.Value.ToString("dd'/'MM'/'yyyy", CultureInfo.InvariantCulture) : "";
                            errormsg = "This ePROM was expired on " + ed;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
            }
            return errormsg;
        }

        public static bool CheckSurveyIsInValidDate(DateTime? StartDate, DateTime? EndDate)
        {
            try
            {
                bool isValid = true;
                DateTime? CurrentDateTime = Utilities.ConvertServerCurrentDateTimeToClientDateTime(DateTime.Now);
                if (StartDate != null && EndDate != null && CurrentDateTime != null)
                {
                    if (CurrentDateTime.Value.Date >= StartDate.Value.Date && CurrentDateTime.Value.Date <= EndDate.Value.Date)
                    {
                        isValid = true;
                    }
                    else
                    {
                        isValid = false;
                    }
                }
                return isValid;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return false;
            }
        }

        public static string SetSurveyStatus(Guid PatientID, Guid OrganizationID, Guid PracticeID, Guid ProviderID, int SurveyID)
        {
            try
            {
                string status = "";
                DateTime? toDayDate = Utilities.ConvertServerCurrentDateTimeToClientDateTime(DateTime.Now).Value.Date;

                PatientSurveyStatus_model patientSurvey = PatientSurveyStatusClass.GetPatientSurveyStatus(PatientID, OrganizationID, PracticeID, ProviderID, SurveyID);

                if (patientSurvey != null)
                {
                    bool isActive = HelperMethods.GetEntities().CheckPatientSurvey_Active((Guid)patientSurvey.PatientSurveyId);
                    if (toDayDate < patientSurvey.StartDate.Value.Date)
                    {
                        status = "Disabled";
                        return status;
                    }

                    if (patientSurvey.Status != "Completed")
                    {
                        status = patientSurvey.Status;

                        if (!isActive)
                        {
                            status = "InActive";
                            return status;
                        }
                        else if (patientSurvey.EndDate.Value.Date < toDayDate)
                        {
                            status = "Expired";
                            return status;
                        }
                        return status;
                    }
                    else
                    {
                        if (!isActive)
                        {
                            status = "InActive";
                            return status;
                        }
                        else
                        {
                            status = PatientSurveyClass.CheckSurveyWasCompletedValidDate(patientSurvey.StartDate, patientSurvey.EndDate, patientSurvey.CreatedDate, patientSurvey.Status);
                        }
                        return status;
                    }
                }
                else
                {
                    PatientSurvey pSurvey = HelperMethods.GetEntities().GetEntity_PatientSurvey(PatientID, OrganizationID, PracticeID, ProviderID, SurveyID);

                    status = "Pending";

                    if (pSurvey != null)
                    {
                        bool isActive = HelperMethods.GetEntities().CheckPatientSurvey_Active((Guid)pSurvey.ID);

                        if (toDayDate < pSurvey.StartDate.Value.Date)
                        {
                            status = "Disabled";
                            return status;
                        }

                        if (!isActive)
                        {
                            status = "InActive";
                            return status;
                        }
                        else if (pSurvey.EndDate.Value.Date < toDayDate)
                        {
                            status = "Expired";
                            return status;
                        }
                    }
                    return status;
                }
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return "";
            }
        }

        #endregion GetPatientSurvey

        #region DeleteSurvey
        public static string DeletePatientSurveyById(Guid Id)
        {
            try
            {
                bool isThirdpartyAppExist = HelperMethods.GetEntities().CheckProviderPatientThirdPartyAppExist(Id);
                bool isPatientSuggestionExist = HelperMethods.GetEntities().CheckPatientSuggestionExist(Id);
                bool isPatientSurveyStatusExist = HelperMethods.GetEntities().CheckPatientSurveyExist(Id);
                if (!isPatientSurveyStatusExist && !isPatientSuggestionExist && !isThirdpartyAppExist)
                {
                    HelperMethods.GetEntities().DeleteEntity_PatientSurvey(Id);
                    return "1";
                }
                else
                {
                    return "0";
                }
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return "0";
            }
        }

        #endregion DeleteSurvey

        #region GetpatientProvidersDetails

        public static PatientProvider_custom GetpatientProvidersDetails(Guid PatientId, Guid OrganizationId, Guid PracticeId, Guid ProviderId, int? SurveyId)
        {
            PatientProvider_custom objPatientProvider = new PatientProvider_custom();
            var PatientSurvey = HelperMethods.GetEntities().GetEntity_PatientSurvey(PatientId, OrganizationId, PracticeId, ProviderId, (int)SurveyId);

            if (PatientSurvey != null)
            {
                objPatientProvider.DueDate = PatientSurvey.EndDate;
                objPatientProvider.PatientSurveyID = PatientSurvey.ID;
            }

            objPatientProvider.OrganizationName = HelperMethods.GetEntities().GetOrganizationsByID(OrganizationId).OrganizationName;
            objPatientProvider.PracticeName = HelperMethods.GetEntities().GetPracticeByID(PracticeId).PracticeName;

            var Patient_Provider = HelperMethods.GetEntities().GetPatientDetailsByUserID(PatientId);

            if (Patient_Provider != null)
            {
                var provider = HelperMethods.GetEntities().GetProviderDetails(ProviderId);
                if (provider != null)
                {
                    var userId = provider.UserID;
                    var user = HelperMethods.GetEntities().GetUserDetailsByUserId(userId);
                    if (user != null)
                    {
                        objPatientProvider.ProviderName = user.FirstName + "  " + user.LastName;
                        objPatientProvider.Email = user.Email;
                        var contact = HelperMethods.GetEntities().GetContactDetails(provider.ContactID);
                        if (contact != null)
                        {
                            objPatientProvider.Fax = contact.FAX;
                            objPatientProvider.TelephoneNo = contact.Phone;
                        }

                        var address = HelperMethods.GetEntities().GetAddressDetails(provider.AddressID);
                        if (address != null)
                        {
                            objPatientProvider.Address1 = address.Line1;
                            objPatientProvider.Address2 = address.Line2;
                            objPatientProvider.ZipCode = address.ZipCode;
                        }
                    }
                }
            }
            return objPatientProvider;
        }

        public static List<Pathway_model> GetPathwayList(Guid guidProviderId)
        {
            return HelperMethods.GetEntities().GetPathwayList(guidProviderId).AsEnumerable().Select(x => new Pathway_model
            {
                ID = x.ID,
                PathwayName = x.PathwayName,
                Description = x.Description,
                IsActive = (bool)x.IsActive,
                CreatedOn = x.CreatedOn
            }).ToList();
        }

        #endregion AssignEpromsEmail
    }

    public static class EmailClass
    {
        public static string SendEpromsUrltoPatient(Guid PatientId, Guid OrganizationId, Guid PracticeId, Guid ProviderId, int SurveyId, string ToAddress, string ePromTitle, string ePromsUrl, string ProviderName, string PatientName, string PatientSurveyId)
        {
            string returnValue = "";
            try
            {
                string MailBody = string.Empty;
                Utilities objUtility = new Utilities();
                SystemFlagBAL objSystemFlagBAL = new SystemFlagBAL();
                string subject = "eProms " + ePromTitle + " - INV";

                var objDetails1 = PatientSurveyClass.GetpatientProvidersDetails(PatientId, OrganizationId, PracticeId, ProviderId, SurveyId);

                var objDetails = Patients.GetProviderDetailByPatient_Org_PracticeId(PatientId, ProviderId, OrganizationId, PracticeId);
                string providerEmailID = HelperMethods.GetEntities().getProviderEmailID(ProviderId);

                string URL = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + "/Login?patientsurveyid=" + PatientSurveyId + "&email=" + ToAddress;

                var formattedDate = string.Format(new MyCustomDateProvider(), "{0}", objDetails1.DueDate);

                var pDetail = HelperMethods.GetEntities().GetPracticeByID(PracticeId);
                var uid = pDetail.UserId ?? Guid.Empty;
                var practiceDetail = PracticeClass.GetPracticeDetail(uid);

                StreamReader reader = null;
                try
                {
                    reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("/Content/Html/AssignEpromToPatient.html"));
                    MailBody = reader.ReadToEnd();
                    MailBody = MailBody.Replace("{{PatientName}}", PatientName)
                                 .Replace("{{epromTitle}}", ePromTitle)
                                 .Replace("{{DoctorName}}", practiceDetail.Practice.PracticeName)
                                 //.Replace("{{DoctorName}}", "Dr." + ProviderName)
                                 .Replace("{{address1}}", objDetails.Address == null ? "" : practiceDetail.Address.Line1)
                                 .Replace("{{address2}}", objDetails.Address == null ? "" : practiceDetail.Address.Line2)
                                 .Replace("{{ZipCode}}", objDetails.Address == null ? "" : practiceDetail.Address.ZipCode)
                                 .Replace("{{Telephone}}", objDetails.Contact == null ? "" : practiceDetail.Contact.Mobile)
                                 .Replace("{{Fax}}", objDetails.Contact == null ? "" : practiceDetail.Contact.FAX)
                                 //.Replace("{{address1}}", objDetails.Address == null ? "" : objDetails.Address.Line1)
                                 //.Replace("{{address2}}", objDetails.Address == null ? "" : objDetails.Address.Line2)
                                 //.Replace("{{ZipCode}}", objDetails.Address == null ? "" : objDetails.Address.ZipCode)
                                 //.Replace("{{Telephone}}", objDetails.Contact == null ? "" : objDetails.Contact.Mobile)
                                 //.Replace("{{Fax}}", objDetails.Contact == null ? "" : objDetails.Contact.FAX)
                                 .Replace("{{Email}}", practiceDetail.User.Email)
                                 //.Replace("{{Email}}", providerEmailID)
                                 .Replace("{{URL}}", URL)
                                 .Replace("{{OrganizationName}}", objDetails.OrganizationName)
                                 .Replace("{{PracticeName}}", objDetails.PracticeName)
                                 .Replace("{{dueDate}}", formattedDate);
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
                        bool result = EC.SendEmail("eProms", subject, MailBody, CID, ToAddress, System.Net.Mail.MailPriority.Normal, null);
                        if (result)
                        {
                            returnValue = "Mail Send Successfully!";
                        }
                    }
                }
            }
            catch (Exception)
            {

            }
            return returnValue;
        }
    }

    public static class PatientIndicatorsClass
    {
        #region CreatePatientIndicators

        public static string CreatePatientIndicators(PatientIndicators_model _patientIndicator)
        {
            try
            {
                string returnstr = "";
                for (var i = 0; i < _patientIndicator.PatientIndicatorList.Count(); i++)
                {
                    var indicator = _patientIndicator.PatientIndicatorList[i];
                    bool isExist = HelperMethods.GetEntities().CheckPatientIndicatorExist((Guid)_patientIndicator.PatientID, (short)indicator.IndicatorID);

                    if (isExist)
                    {
                        var patientindicator = new PatientIndicator
                        {
                            ID = indicator.ID,
                            PatientID = _patientIndicator.PatientID,
                            IndicatorID = indicator.IndicatorID,
                            StartDate = indicator.StartDate,
                            EndDate = indicator.EndDate,
                            Frequency = indicator.Frequency,
                            Unit = indicator.Unit,
                            Goal = indicator.Goal,
                            Comments = indicator.Comments,
                            IsActive = indicator.IsActive,
                            Status = indicator.Status,
                            ModifiedDate = Utilities.ConvertServerCurrentDateTimeToClientDateTime(DateTime.Now)
                        };

                        returnstr = HelperMethods.GetEntities().UpdateEntity_PatientIndicators(patientindicator);
                    }
                    else
                    {
                        var patientindicator = new PatientIndicator
                        {
                            PatientID = _patientIndicator.PatientID,
                            IndicatorID = indicator.IndicatorID,
                            StartDate = indicator.StartDate,
                            EndDate = indicator.EndDate,
                            Frequency = indicator.Frequency,
                            Unit = indicator.Unit,
                            Goal = indicator.Goal,
                            Comments = indicator.Comments,
                            IsActive = indicator.IsActive,
                            Status = indicator.Status,
                            CreatedDate = Utilities.ConvertServerCurrentDateTimeToClientDateTime(DateTime.Now)
                        };
                        returnstr = HelperMethods.GetEntities().SaveEntity_PatientIndicators(patientindicator);
                    }
                }
                return returnstr;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return "";
            }
        }

        #endregion CreatePatientIndicators

        #region GetPatientIndicators

        public static List<PatientIndicators_model> GetIndicators_By_patientId(Guid PatientId)
        {
            try
            {
                List<PatientIndicators_model> IndicatorList = HelperMethods.GetEntities().GetEntityById_PatientIndicators(PatientId).AsEnumerable().Select(x => new PatientIndicators_model
                {
                    ID = x.ID,
                    PatientID = x.PatientID,
                    IndicatorID = x.IndicatorID,
                    IndicatorName = HelperMethods.GetEntities().GetEntityById_Indicators(x.IndicatorID).IndicatorName,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    Frequency = x.Frequency,
                    Unit = x.Unit,
                    Goal = x.Goal,
                    Comments = x.Comments,
                    IsActive = x.IsActive,
                    Status = x.Status
                }).ToList();

                return IndicatorList;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        #endregion GetPatientIndicators

        #region DeletePatientIndicators
        public static string DeletePatientIndicatorsById(int Id)
        {
            try
            {
                HelperMethods.GetEntities().DeleteEntity_PatientIndicators(Id);
                return "Indicators Deleted Successfully!";
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return "";
            }
        }

        #endregion DeletePatientIndicators
    }

    public static class PatientSuggestionsClass
    {
        #region GetPatientSuggestionsByPatientID

        public static PatientSuggestions_model GetPatientSuggestionsbyPatientSurveyID(Guid PatientSurveyId)
        {
            PatientSuggestion suggestion = HelperMethods.GetEntities().GetEntityByPatientSurveyId_PatientSuggestion(PatientSurveyId);
            if (suggestion != null)
            {
                return new PatientSuggestions_model
                {
                    ID = suggestion.ID,
                    PatientSurveyID = (Guid)suggestion.PatientSurveyID,
                    Suggestions = suggestion.Suggestions,
                    IsActive = suggestion.IsActive

                };
            }
            else
                return null;
        }

        #endregion GetPatientSuggestionsByPatientID       

        #region CreatePatientSuggestion

        public static string CreatePatientSuggestion(PatientSuggestions_model _suggestion)
        {
            try
            {
                string returnstr = "";
                if (_suggestion.PatientSurveyID != Guid.Empty)
                {
                    bool isExist = HelperMethods.GetEntities().CheckPatientSuggestionExist((Guid)_suggestion.PatientSurveyID);

                    if (isExist)
                    {
                        var suggestion = new PatientSuggestion
                        {
                            ID = _suggestion.ID,
                            PatientSurveyID = _suggestion.PatientSurveyID,
                            Suggestions = _suggestion.Suggestions,
                            IsActive = true,
                            ModifiedDate = Utilities.ConvertServerCurrentDateTimeToClientDateTime(DateTime.Now)
                        };

                        returnstr = HelperMethods.GetEntities().UpdateEntity_PatientSuggestion(suggestion);
                    }
                    else
                    {
                        var suggestion = new PatientSuggestion
                        {
                            PatientSurveyID = _suggestion.PatientSurveyID,
                            Suggestions = _suggestion.Suggestions,
                            IsActive = true,
                            CreatedDate = Utilities.ConvertServerCurrentDateTimeToClientDateTime(DateTime.Now)
                        };
                        returnstr = HelperMethods.GetEntities().SaveEntity_PatientSuggestion(suggestion);
                    }
                }
                return returnstr;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return "";
            }
        }

        #endregion CreatePatientSuggestion
    }

    public static class OrganizationsClass
    {
        #region GetOrganization

        public static List<Organization_Model> GetOrganizationList()
        {
            return HelperMethods.GetEntities().GetOrganizationList().AsEnumerable().Select(x => new Organization_Model
            {
                ID = x.ID,
                OrganizationName = x.OrganizationName,
                SalutationID = x.SalutationID,
                UserId = x.UserId,
                IsActive = (bool)x.IsActive
            }).ToList();
        }

        #endregion GetOrganization

        #region GetOrganizationIdByUserID

        public static Guid? GetOrganizationIdByUserID(Guid? UserId)
        {
            var obj = HelperMethods.GetEntities().GetOrganizationsByUserID(UserId);
            if (obj != null)
            {
                return obj.ID;
            }
            return Guid.Empty;
        }

        #endregion GetOrganizationIdByUserID

        #region ManageOrganizationDetail

        public static bool ManageOrganizationDetail(Organizations_custom_model _objOrganization)
        {
            Guid? userid = Providers.GetUserIdByUserName(_objOrganization.User.UserName);

            var ud = new UserDetail
            {
                ID = (Guid)userid,
                UserName = _objOrganization.User.UserName,
                Email = _objOrganization.User.Email,
                FirstName = _objOrganization.User.FirstName,
                LastName = _objOrganization.User.LastName,
                MiddleName = _objOrganization.User.MiddleName,
                DOB = _objOrganization.User.DOB,
                Gender = _objOrganization.User.Gender
            };

            _objOrganization.Address.UserId = userid;

            Addresses.ManageAddress((Guid)userid, _objOrganization.Address);

            if (_objOrganization.ID == null || _objOrganization.ID == Guid.Empty)
            {
                var organization = new Organization
                {
                    ID = Guid.NewGuid(),
                    OrganizationName = _objOrganization.OrganizationName,
                    IsActive = true,
                    UserId = (Guid)userid,
                    CreatedDate = Utilities.ConvertServerCurrentDateTimeToClientDateTime(DateTime.Now),
                    CreatedBy = (Guid)userid
                };

                HelperMethods.GetEntities().Entity_CreateOrganizations(organization);
            }
            else
            {
                var organization = new Organization
                {
                    ID = _objOrganization.ID,
                    OrganizationName = _objOrganization.OrganizationName,
                    SalutationID = _objOrganization.SalutationID,
                    UserId = (Guid)userid,
                    IsActive = true,
                    CreatedDate = Utilities.ConvertServerCurrentDateTimeToClientDateTime(DateTime.Now),
                    CreatedBy = (Guid)userid
                };

                HelperMethods.GetEntities().Entity_UpdateOrganizations(organization);
            }
            return HelperMethods.GetEntities().Entity_UpdateuserDetail(ud);
        }

        #endregion ManageOrganizationDetail

        #region GetOrganizationDetail
        public static Organizations_custom_model GetOrganizationDetail(string UserName)
        {
            if (string.IsNullOrEmpty(UserName))
            {
                return null;
            }

            Guid? userid = Providers.GetUserIdByUserName(UserName);

            if (userid != null && userid != Guid.Empty)
            {
                var user = HelperMethods.GetEntities().GetUserDetailsByUserId((Guid)userid);
                var organizationDetail = HelperMethods.GetEntities().GetOrganizationsByUserID((Guid)userid);
                if (organizationDetail != null && user != null)
                {
                    return new Organizations_custom_model
                    {
                        Address = Addresses.GetAddressDetails(long.MinValue, (Guid)userid),
                        User = new User_model { UserName = user.UserName, FirstName = user.FirstName, MiddleName = user.MiddleName, LastName = user.LastName, DOB = user.DOB, Gender = user.Gender },
                        OrganizationName = HelperMethods.GetEntities().GetOrganizationsByUserID((Guid)userid).OrganizationName,
                        ID = organizationDetail == null ? Guid.Empty : organizationDetail.ID,
                        SalutationID = organizationDetail.SalutationID
                    };
                }
            }
            return null;
        }
        #endregion ManageOrganizationDetail
    }

    public static class ProviderOrganizationClass
    {
        #region ProviderOrganization

        #region GetProviderIdFromUserName

        public static string GetProviderIdFromUserName(string UserName)
        {
            Guid? userid = Providers.GetUserIdByUserName(UserName);
            Guid? ProviderId = Providers.GetProviderID((Guid)userid);
            if (ProviderId != Guid.Empty)
            {
                return ProviderId.ToString();
            }
            else
                return string.Empty;
        }

        #endregion GetProviderIdFromUserId

        #region GetProviderOrganization

        public static List<ProviderOrganizations_model> GetProviderOrganizationByProviderID(Guid ProviderID)
        {
            return HelperMethods.GetEntities().GetEntityByProviderID_ProviderOrganizations(ProviderID).AsEnumerable().Select(x => new ProviderOrganizations_model
            {
                ID = x.ID,
                OrganizationID = x.OrganizationID,
                OrganizationName = HelperMethods.GetEntities().GetOrganizationsByID((Guid)x.OrganizationID).OrganizationName,
                ProviderID = x.ProviderID,
                PracticeList = PracticeClass.GetRemainingPracticeListBy_OrganizationID("", x.OrganizationID.ToString(), x.ProviderID),
                ProviderPracticeList = x.ProviderPractices.Select(p => new ProviderPractice_model
                {
                    Id = p.ID,
                    ProviderID = (Guid)p.ProviderId,
                    PracticeName = p.Practice.PracticeName,
                    PracticeId = (Guid)p.PracticeId,
                    ProviderOrganizationId = (Guid)p.ProviderOrganizationId,
                    CreatedDate = p.CreatedDate,
                    ModifiedDate = p.ModifiedDate,
                    CreatedBy = p.CreatedBy,
                    ModifiedBy = p.ModifiedBy,
                    ProviderPracticeRoleList = p.ProviderPracticeRoles.Select(z => new ProviderPracticeRole_model
                    {
                        Id = z.ID,
                        RoleID = z.RoleID,
                        ProviderPracticeID = z.ProviderPracticeID
                    }).ToList(),
                    PracticeRoleList = HelperMethods.GetEntities().GetEntity_PracticeRoleList(x.OrganizationID, (Guid)p.PracticeId).Select(r => new PracticeRole_Model
                    {
                        RoleID = r.RoleId,
                        RoleName = r.RoleName
                    }).ToList(),
                }).ToList(),
                Description = x.Description,
                Designation = x.Designation,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                IsActive = x.IsActive

            }).ToList();
        }


        public static List<Organization_Model> GetOrganizationByProviderId(Guid ProviderID)
        {
            return HelperMethods.GetEntities().GetProviderOrganizationsByProviderId(ProviderID).AsEnumerable().Select(x => new Organization_Model
            {
                ID = x.ID,
                OrganizationName = x.OrganizationName,
                SalutationID = x.SalutationID,
                IsActive = (bool)x.IsActive,
                UserId = x.UserId

            }).ToList();
        }

        #endregion GetProviderOrganization        

        #region CreateProviderOrganization

        public static string CreateProviderOrganization(ProviderOrganizations_model _providerOrganization)
        {
            try
            {
                Guid? userid = Providers.GetUserIdByUserName(_providerOrganization.UserName);
                Guid? ProviderId = Providers.GetProviderID((Guid)userid);
                if (ProviderId != Guid.Empty)
                {
                    bool IsExists = HelperMethods.GetEntities().CheckExists_ProviderOrganizations((Guid)_providerOrganization.OrganizationID, (Guid)ProviderId);

                    if (!IsExists)
                    {
                        var providerOrg = new ProviderOrganization
                        {
                            ID = Guid.NewGuid(),
                            ProviderID = (Guid)ProviderId,
                            OrganizationID = _providerOrganization.OrganizationID,
                            Designation = _providerOrganization.Designation,
                            Description = _providerOrganization.Description,
                            StartDate = _providerOrganization.StartDate,
                            EndDate = _providerOrganization.EndDate,
                            IsActive = true,
                            CreatedDate = Utilities.ConvertServerCurrentDateTimeToClientDateTime(DateTime.Now)
                        };

                        return HelperMethods.GetEntities().SaveEntity_ProviderOrganization(providerOrg);
                    }
                    else
                    {
                        var providerOrg = new ProviderOrganization
                        {
                            ID = _providerOrganization.ID,
                            ProviderID = (Guid)ProviderId,
                            OrganizationID = _providerOrganization.OrganizationID,
                            Designation = _providerOrganization.Designation,
                            Description = _providerOrganization.Description,
                            StartDate = _providerOrganization.StartDate,
                            EndDate = _providerOrganization.EndDate,
                            IsActive = true,
                            ModifiedDate = Utilities.ConvertServerCurrentDateTimeToClientDateTime(DateTime.Now)
                        };

                        return HelperMethods.GetEntities().UpdateEntity_ProviderOrganization(providerOrg);
                    }
                }
                return "";
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return "";
            }
        }

        #endregion CreateProviderOrganization

        #region DeleteProviderOrganization
        public static string DeleteProviderOrganization(Guid Id, Guid OrganizationID)
        {
            try
            {
                bool isExist = HelperMethods.GetEntities().CheckExists_PatientProviderOrganizations(OrganizationID, Guid.Empty);
                if (!isExist)
                {
                    HelperMethods.GetEntities().DeleteEntity_ProviderOrganization(Id);
                    return "Organization Deleted Successfully!";
                }
                else
                {
                    return "Organization is in use!";
                }
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return "";
            }
        }

        #endregion DeleteProviderOrganization

        #endregion ProviderOrganization
    }

    public static class PatientSurveyStatusClass
    {
        #region PatientSurveyStatus


        #region CreatePatientSurveyStatus

        public static Guid CreatePatientSurveyStatus(PatientSurveyStatus_model _surveyStatus)
        {
            try
            {
                var surveystatus = new PatientSurveyStatu
                {
                    ID = Guid.NewGuid(),
                    PatientSurveyID = _surveyStatus.PatientSurveyId,
                    Email = _surveyStatus.Email,
                    Status = "Pending",
                    CreatedDate = Utilities.ConvertServerCurrentDateTimeToClientDateTime(DateTime.Now),
                    Score = ""
                };

                return HelperMethods.GetEntities().Create_PatientSurveyStatus(surveystatus);
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return new Guid();
            }
        }

        #endregion CreatePatientSurveyStatus

        #region UpdatePatientSurveyStatus

        public static string UpdatePatientSurveyStatus(PatientSurveyStatus_model _surveyStatus)
        {
            try
            {
                var surveystatus = new PatientSurveyStatu
                {
                    ID = _surveyStatus.ID,
                    Status = _surveyStatus.Status,
                    Score = _surveyStatus.Score
                };

                return HelperMethods.GetEntities().Update_PatientSurveyStatus(surveystatus);
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return "";
            }
        }

        #endregion UpdatePatientSurveyStatus

        #region GetPatientSurveyStatus

        public static List<PatientSurveyStatus_model> GetPatientSurveyStatus(Guid PatientId, Guid OrganizationId, Guid PracticeId, Guid ProviderId, int SurveyId, string strFromDate, string strToDate, string PathwayID)
        {
            DateTime? FromDate = null, ToDate = null;

            if (strFromDate != null && strFromDate != "undefined" && strFromDate != "null")
            {
                string[] StartDateArray = strFromDate.Split('/');
                if (StartDateArray.Length > 0)
                {
                    FromDate = new DateTime(Convert.ToInt32(StartDateArray[2]), Convert.ToInt32(StartDateArray[0]), Convert.ToInt32(StartDateArray[1]), 0, 0, 0);
                }
            }

            if (strToDate != null && strToDate != "undefined" && strToDate != "null")
            {
                string[] ToDateArray = strToDate.Split('/');
                if (ToDateArray.Length > 0)
                {
                    ToDate = new DateTime(Convert.ToInt32(ToDateArray[2]), Convert.ToInt32(ToDateArray[0]), Convert.ToInt32(ToDateArray[1]), 23, 59, 59);
                }
            }

            return HelperMethods.GetEntities().GetPatientSurveyStatus(PatientId, OrganizationId, PracticeId, ProviderId, SurveyId, FromDate, ToDate, PathwayID).AsEnumerable().Select(x => new PatientSurveyStatus_model
            {
                ID = x.ID,
                PatientSurveyId = x.PatientSurveyID,
                ExternalTitle = x.PatientSurvey.Survey.ExternalTitle,
                Email = x.Email,
                Status = x.Status,
                CreatedDate = x.CreatedDate,
                Score = x.Score
            }).ToList();
        }

        public static PatientSurveyStatus_model GetPatientSurveyStatus(Guid PatientId, Guid OrganizationId, Guid PracticeId, Guid ProviderId, int SurveyId)
        {
            PatientSurvey survey = HelperMethods.GetEntities().GetEntity_PatientSurvey(PatientId, OrganizationId, PracticeId, ProviderId, SurveyId);

            if (survey != null)
            {
                PatientSurveyStatu statusdata = HelperMethods.GetEntities().GetPatientSurveyStatus(survey.ID);

                if (statusdata != null)
                {
                    return new PatientSurveyStatus_model
                    {
                        ID = statusdata.ID,
                        PatientSurveyId = statusdata.PatientSurveyID,
                        Email = statusdata.Email,
                        Status = statusdata.Status,
                        CreatedDate = statusdata.CreatedDate,
                        Score = statusdata.Score,
                        StartDate = statusdata.PatientSurvey.StartDate,
                        EndDate = statusdata.PatientSurvey.EndDate,
                    };
                }
                else
                {
                    return null;
                }
            }
            return null;
        }


        public static List<PatientSurvey_model> GetPatientSurveyStatusBy_PatientId_ProviderId(Guid PatientId, Guid ProviderID, bool isArchive)
        {
            try
            {
                List<PatientSurvey_model> SurveyList = HelperMethods.GetEntities().GetEntity_PatientSurveyStatus(PatientId, ProviderID).AsEnumerable().Select(x => new PatientSurvey_model
                {
                    ID = x.PatientSurvey.ID,
                    PatientSurveyStatusID = x.ID,
                    PatientID = x.PatientSurvey.PatientID,
                    ProviderID = x.PatientSurvey.ProviderID,
                    ProviderName = x.PatientSurvey.Provider.UserDetail.FirstName + " " + x.PatientSurvey.Provider.UserDetail.LastName,
                    SurveyID = x.PatientSurvey.SurveyID,
                    OrganizationID = x.PatientSurvey.OrganizationID,
                    OrganizationName = x.PatientSurvey.Organization.OrganizationName,
                    PracticeID = x.PatientSurvey.PracticeID,
                    PracticeName = x.PatientSurvey.Practice.PracticeName,
                    ExternalTitle = x.PatientSurvey.Survey.ExternalTitle,
                    Status = x.Status,
                    CreatedDate = x.CreatedDate,
                    StartDate = x.PatientSurvey.StartDate == null ? null : Convert.ToDateTime(x.PatientSurvey.StartDate).ToString("MM'/'dd'/'yyyy"),
                    EndDate = x.PatientSurvey.EndDate == null ? null : Convert.ToDateTime(x.PatientSurvey.EndDate).ToString("MM'/'dd'/'yyyy"),
                    Score = PatientSurveyStatusClass.ConvertJSONScore(x.Score, x.PatientSurvey.Survey.ExternalTitle)
                }).ToList();

                DateTime dt = DateTime.Now.AddMonths(-3);

                if (isArchive)
                {
                    SurveyList = SurveyList.Where(x => x.CreatedDate < dt).ToList();
                }
                else
                {
                    SurveyList = SurveyList.Where(x => x.CreatedDate >= dt).ToList();
                }

                return SurveyList;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        public static string ConvertJSONScore(string score, string extTitle)
        {
            try
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                string strscore = "";
                int rf = 0;

                if (extTitle.Trim() == "Preventive ePROMs for Population Health Management by GPs ™")
                {
                    Factor[] scoreList = js.Deserialize<Factor[]>(score);

                    for (int i = 0; i < scoreList.Count(); i++)
                    {
                        string[] splitVal = scoreList[i].Value.Split('_');

                        if (splitVal[1] == "risk factor")
                        {
                            rf++;
                        }
                        else
                        {
                            strscore = "No Risk Factor.";
                        }
                    }

                    if (rf == 0)
                    {
                        strscore = "No Risk Factor";
                    }
                    else
                    {
                        strscore = rf + " Risk Factor Exist";
                    }
                }
                else
                {
                    Score[] scoreList = js.Deserialize<Score[]>(score);

                    if (scoreList.Count() == 1)
                    {
                        strscore = scoreList[0].Value.ToString();
                    }
                    else if (scoreList.Count() > 1)
                    {
                        for (int i = 0; i < scoreList.Count(); i++)
                        {
                            if (scoreList[i].Title.IndexOf("Physical Health") > -1)
                            {
                                strscore += "PH " + scoreList[i].Value.ToString() + " - ";
                            }
                            else if (scoreList[i].Title.IndexOf("Mental Health") > -1)
                            {
                                strscore += "MH " + scoreList[i].Value.ToString();
                            }
                        }
                    }
                }

                return strscore;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return "";
            }
        }

        #endregion GetPatientSurveyStatus

        #region CheckTodayPatientSurveyStatusExist

        public static string CheckTodayDate_PatientSurveyStatusExist(Guid PatientSurveyID)
        {
            try
            {
                string ReturnMsg = "";
                bool isActive = HelperMethods.GetEntities().CheckPatientSurvey_Active(PatientSurveyID);
                bool isTodayDateExist = false;

                if (isActive)
                {
                    isTodayDateExist = HelperMethods.GetEntities().CheckTodayDate_PatientSurveyStatusExist(PatientSurveyID);

                    if (isTodayDateExist)
                    {
                        ReturnMsg = "You have already Submitted this ePROM and cannot submit another ePROM like this one today.";
                    }
                }
                else
                {
                    ReturnMsg = "ePROM is Inactive. Please contact to your Provider to Active ePROM";
                }
                return ReturnMsg;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return "";
            }
        }

        #endregion CheckTodayPatientSurveyStatusExist

        #region CheckSurveyIsValidToSubmit

        public static bool CheckSurveyIsValidToSubmit(Guid PatientSurveyID, DateTime? StartDate, DateTime? EndDate, DateTime? CompletedDate, string Status)
        {
            try
            {
                bool isValid = true;

                bool isActive = HelperMethods.GetEntities().CheckPatientSurvey_Active(PatientSurveyID);
                if (!isActive)
                {
                    isValid = false;
                }
                else
                {
                    bool isCompletedInRange = ((CompletedDate != null) && (Status.ToLower() == "completed")) ? PatientSurveyClass.CheckSurveyCompletedDate(StartDate, EndDate, CompletedDate) : false;
                    bool isTodayDateExist = HelperMethods.GetEntities().CheckTodayDate_PatientSurveyStatusExist(PatientSurveyID);
                    DateTime? toDayDate = Utilities.ConvertServerCurrentDateTimeToClientDateTime(DateTime.Now).Value.Date;

                    if (toDayDate < StartDate.Value.Date)
                    {
                        isValid = false;
                    }
                    else if (isCompletedInRange)
                    {
                        isValid = false;
                    }
                    else
                    {
                        bool isInRange = PatientSurveyClass.CheckSurveyIsInValidDate(StartDate, EndDate);

                        if (!isInRange)
                        {
                            isValid = false;
                        }
                    }
                }
                return isValid;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return false;
            }
        }

        #endregion CheckSurveyIsValidToSubmit

        #endregion PatientSurveyStatus
    }

    public static class PracticeClass
    {
        #region ManagePracticeDetail

        public static string ManagePracticeDetail(Practice_custom_model _objPractice)
        {
            string isCreate = "0";
            Guid? OrganizationUserId = Providers.GetUserIdByUserName(_objPractice.CurrentUserName);
            Guid? userid = Providers.GetUserIdByUserName(_objPractice.User.UserName);

            var ud = new UserDetail
            {
                ID = (Guid)userid,
                UserName = _objPractice.User.UserName,
                Email = _objPractice.User.UserName,
                FirstName = _objPractice.User.FirstName,
                LastName = _objPractice.User.LastName,
                MiddleName = _objPractice.User.MiddleName,
                DOB = _objPractice.User.DOB,
                Gender = _objPractice.User.Gender
            };

            _objPractice.Address.UserId = userid;

            Addresses.ManageAddress((Guid)userid, _objPractice.Address);

            if (_objPractice.Contact != null)
            {
                var contact = new Contact
                {
                    Phone = _objPractice.Contact.Phone,
                    Mobile = _objPractice.Contact.Mobile,
                    Mobile2 = _objPractice.Contact.Mobile2,
                    FAX = _objPractice.Contact.FAX,
                    Pager = _objPractice.Contact.Pager,
                    Emergency = _objPractice.Contact.Emergency,
                    EmailBusiness = _objPractice.Contact.EmailBusiness,
                    EmailPersonal = _objPractice.Contact.EmailPersonal,
                    Skype = _objPractice.Contact.Skype,
                    LinkedIN = _objPractice.Contact.LinkedIN,
                    Website = _objPractice.Contact.Website,
                    OtherFieldsJSON = _objPractice.Contact.OtherFieldsJSON,
                    Description = _objPractice.Contact.Description,
                    IsActive = true,
                    UserId = userid
                };

                HelperMethods.GetEntities().ManageContact((Guid)userid, contact);
            }

            var PracticeID = Guid.NewGuid();
            if (_objPractice.ID == null || _objPractice.ID == Guid.Empty)
            {
                Guid? OrganizationID = Guid.Empty;
                if (OrganizationUserId == Guid.Empty)
                {
                    OrganizationID = _objPractice.OrganizationID;
                }
                else
                {
                    OrganizationID = HelperMethods.GetEntities().GetOrganizationsByUserID((Guid)OrganizationUserId).ID;
                }

                var practice = new Practice
                {
                    ID = PracticeID,
                    PracticeName = _objPractice.Practice.PracticeName,
                    OrganizationId = OrganizationID,
                    SalutationID = _objPractice.Practice.SalutationID,
                    IsActive = true,
                    UserId = (Guid)userid,
                    CreatedDate = Utilities.ConvertServerCurrentDateTimeToClientDateTime(DateTime.Now),
                    CreatedBy = (Guid)userid
                };
                isCreate = "1";
                HelperMethods.GetEntities().Entity_CreatePractice(practice);
            }
            else
            {
                PracticeID = _objPractice.ID;
                var Practice = new Practice
                {
                    ID = _objPractice.ID,
                    PracticeName = _objPractice.Practice.PracticeName,
                    SalutationID = _objPractice.Practice.SalutationID,
                    //UserId = (Guid)userid,
                    IsActive = true,
                    ModifiedDate = Utilities.ConvertServerCurrentDateTimeToClientDateTime(DateTime.Now),
                    CreatedBy = (Guid)userid
                };
                isCreate = "0";
                HelperMethods.GetEntities().Entity_UpdatePractice(Practice);
            }
            HelperMethods.GetEntities().Entity_UpdateuserDetail(ud);
            return PracticeID.ToString() + "_" + isCreate;
        }

        #endregion ManagePracticeDetail

        #region GetPracticeDetail
        public static Practice_custom_model GetPracticeDetail(Guid UserId)
        {
            if (UserId != null && UserId != Guid.Empty)
            {
                var user = HelperMethods.GetEntities().GetUserDetailsByUserId((Guid)UserId);
                var practice = HelperMethods.GetEntities().GetPracticeByUserID((Guid)UserId);
                return new Practice_custom_model
                {
                    OrganizationID = practice == null ? null : practice.OrganizationId,
                    Contact = Providers.GetContactDetails(int.MinValue, UserId),
                    Address = Addresses.GetAddressDetails(long.MinValue, (Guid)UserId),
                    User = new User_model { UserName = user.UserName, FirstName = user.FirstName, MiddleName = user.MiddleName, LastName = user.LastName, DOB = user.DOB, Gender = user.Gender, Email = user.Email },
                    Practice = practice == null ? null : new Practice_Model { ID = practice.ID, PracticeName = practice.PracticeName, OrganizationId = practice == null ? null : practice.OrganizationId, SalutationID = practice.SalutationID, UserId = practice.UserId, IsActive = (bool)practice.IsActive },
                    ID = practice == null ? Guid.Empty : practice.ID
                };
            }
            return null;
        }
        #endregion ManagePracticeDetail

        #region GetOrganization

        public static List<Practice_Model> GetPracticeListBy_OrganizationID(string UserName, string strOrganizationID)
        {
            Guid? OrganizationID = Guid.Empty;

            if (strOrganizationID == null || strOrganizationID == "")
            {
                Guid? UserId = Providers.GetUserIdByUserName(UserName);
                OrganizationID = OrganizationsClass.GetOrganizationIdByUserID(UserId);
            }
            else
            {
                OrganizationID = new Guid(strOrganizationID);
            }
            return HelperMethods.GetEntities().GetPracticeList(OrganizationID).AsEnumerable().Select(x => new Practice_Model
            {
                ID = x.ID,
                PracticeName = x.PracticeName,
                UserId = x.UserId,
                OrganizationId = x.OrganizationId,
                SalutationID = x.SalutationID,
                IsActive = (bool)x.IsActive,
                User = new User_model { UserName = x.UserDetail.UserName, FirstName = x.UserDetail.FirstName, MiddleName = x.UserDetail.MiddleName, LastName = x.UserDetail.LastName, DOB = x.UserDetail.DOB, Gender = x.UserDetail.Gender },
                Address = Addresses.GetAddressDetails(long.MinValue, (Guid)x.UserId),
                Contact = Providers.GetContactDetails(int.MinValue, (Guid)x.UserId)
            }).ToList();
        }


        public static List<Practice_Model> GetRemainingPracticeListBy_OrganizationID(string UserName, string strOrganizationID, Guid? ProviderId)
        {
            Guid? OrganizationID = Guid.Empty;

            if (strOrganizationID == null || strOrganizationID == "")
            {
                Guid? UserId = Providers.GetUserIdByUserName(UserName);
                OrganizationID = OrganizationsClass.GetOrganizationIdByUserID(UserId);
            }
            else
            {
                OrganizationID = new Guid(strOrganizationID);
            }
            return HelperMethods.GetEntities().GetRemainingPracticeList(OrganizationID, ProviderId).AsEnumerable().Select(x => new Practice_Model
            {
                ID = x.ID,
                PracticeName = x.PracticeName,
                UserId = x.UserId,
                OrganizationId = x.OrganizationId,
                SalutationID = x.SalutationID,
                IsActive = (bool)x.IsActive,
                User = new User_model { UserName = x.UserDetail.UserName, FirstName = x.UserDetail.FirstName, MiddleName = x.UserDetail.MiddleName, LastName = x.UserDetail.LastName, DOB = x.UserDetail.DOB, Gender = x.UserDetail.Gender },
                Address = Addresses.GetAddressDetails(long.MinValue, (Guid)x.UserId),
                Contact = Providers.GetContactDetails(int.MinValue, (Guid)x.UserId)
            }).ToList();
        }

        #endregion GetOrganization

        #region GetOrganizationPracticeByProviderId

        public static List<Practice_Model> GetOrganizationPracticeByProviderId(Guid OrganizationId, Guid ProviderId)
        {
            return HelperMethods.GetEntities().GetOrganizationPracticeByProviderId(OrganizationId, ProviderId).AsEnumerable().Select(x => new Practice_Model
            {
                ID = x.ID,
                PracticeName = x.PracticeName,
                UserId = x.UserId,
                OrganizationId = x.OrganizationId,
                SalutationID = x.SalutationID,
                IsActive = (bool)x.IsActive
            }).ToList();
        }

        #endregion GetOrganizationPracticeByProviderId
    }

    public static class PracticeRoleClass
    {
        #region PracticeRole

        #region ManagePracticeRole

        public static bool ManagePracticeRole(PracticeRole_Model _objPracticeRole)
        {
            try
            {
                Guid? OrganizationID = OrganizationsClass.GetOrganizationIdByUserID(_objPracticeRole.UserId);
                if (OrganizationID == Guid.Empty)
                {
                    OrganizationID = _objPracticeRole.OrganizationId;
                }

                if (_objPracticeRole.RoleID == 0 || _objPracticeRole.RoleID == int.MinValue)
                {
                    var practiceRole = new PracticeRole
                    {
                        RoleName = _objPracticeRole.RoleName,
                        OrganizationId = OrganizationID,
                        PracticeId = _objPracticeRole.PracticeId,
                        IsActive = true,
                        CreatedDate = Utilities.ConvertServerCurrentDateTimeToClientDateTime(DateTime.Now),
                        CreatedBy = (Guid)_objPracticeRole.UserId
                    };
                    return HelperMethods.GetEntities().Entity_CreatePracticeRole(practiceRole);
                }
                else
                {
                    var practiceRole = new PracticeRole
                    {
                        RoleId = _objPracticeRole.RoleID,
                        RoleName = _objPracticeRole.RoleName,
                        OrganizationId = OrganizationID,
                        PracticeId = _objPracticeRole.PracticeId,
                        IsActive = true
                    };
                    HelperMethods.GetEntities().Entity_UpdatePracticeRole(practiceRole);
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion ManagePracticeRole

        #region GetPracticeRole

        public static List<PracticeRole_Model> GetPracticeRole(Guid OrganizationId, Guid UserId, Guid PracticeId)
        {
            try
            {
                Guid? OrganizationID = OrganizationsClass.GetOrganizationIdByUserID(UserId);
                if (OrganizationID == Guid.Empty)
                {
                    OrganizationID = OrganizationId;
                }

                List<PracticeRole_Model> roleList = HelperMethods.GetEntities().GetEntity_PracticeRoleList(OrganizationID
                    , PracticeId).AsEnumerable().Select(x => new PracticeRole_Model
                    {
                        RoleID = x.RoleId,
                        RoleName = x.RoleName,
                        OrganizationId = x.OrganizationId,
                        PracticeId = x.PracticeId,
                        IsActive = x.IsActive
                    }).ToList();

                return roleList;

            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion GetPracticeRole

        #region DeletePracticeRole

        public static bool DeletePracticeRole(int RoleId)
        {
            try
            {
                return HelperMethods.GetEntities().DeleteEntity_PracticeRole(RoleId);
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion DeletePracticeRole

        #region CheckPracticeRoleExist

        public static bool CheckPracticeRoleExist(Guid PracticeId, string RoleName, int RoleID, Guid UserId, Guid OrganizationId)
        {
            try
            {
                Guid? OrganizationID = OrganizationsClass.GetOrganizationIdByUserID(UserId);
                if (OrganizationID == Guid.Empty)
                {
                    OrganizationID = OrganizationId;
                }

                return HelperMethods.GetEntities().CheckEntity_PracticeRoleExist(OrganizationID, PracticeId, RoleName, RoleID);
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion CheckPracticeRoleExist

        #endregion PracticeRole
    }

    public static class ProviderPractice
    {
        #region ProviderPractice

        public static List<ProviderPractice_model> GetProviderPracticeByProviderId(string ProviderId)
        {
            return HelperMethods.GetEntities().GetProviderPracticeByProviderId(new Guid(ProviderId)).Select(x => new ProviderPractice_model
            {
                Id = x.ID,
                ProviderID = (Guid)x.ProviderId,
                PracticeId = (Guid)x.PracticeId,
                ProviderOrganizationId = (Guid)x.ProviderOrganizationId,
                CreatedDate = x.CreatedDate,
                ModifiedDate = x.ModifiedDate,
                CreatedBy = x.CreatedBy,
                ModifiedBy = x.ModifiedBy
            }).ToList();
        }

        public static string CreateProviderPractice(ProviderPractice_model Practice)
        {
            var ObjPractice = new DAL.ProviderPractice
            {
                ID = Guid.NewGuid(),
                ProviderId = Practice.ProviderID,
                PracticeId = Practice.PracticeId,
                ProviderOrganizationId = Practice.ProviderOrganizationId,
                CreatedDate = Utilities.ConvertServerCurrentDateTimeToClientDateTime(DateTime.Now),
            };
            return HelperMethods.GetEntities().SaveEntity_ProviderPractice(ObjPractice);
        }

        public static string DeleteProviderPractice(string ProviderPracticeId, Guid PracticeID)
        {
            try
            {
                bool isExist = HelperMethods.GetEntities().CheckExists_PatientProviderOrganizations(Guid.Empty, PracticeID);
                if (!isExist)
                {
                    HelperMethods.GetEntities().DeleteProviderPractice(new Guid(ProviderPracticeId));
                    return "Practice Deleted Successfully!";
                }
                else
                {
                    return "Practice is in use!";
                }
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return "";
            }
        }

        #endregion ProviderPractice
    }

    public static class ProviderPracticeRoleClass
    {
        #region ProviderPracticeRole

        public static bool SaveProviderPracticeRole(ProviderPracticeRole_custom_model ProviderPracticeRole)
        {
            if (ProviderPracticeRole != null)
            {
                Guid ProviderPracticeID = ProviderPracticeRole.ProviderPracticeId;

                var ProviderPracticeRoleObj = ProviderPracticeRole.ProviderPracticeRoleList.Select(x => new DAL.ProviderPracticeRole
                {
                    RoleID = x.RoleID,
                    ProviderPracticeID = x.ProviderPracticeID
                }).ToList();

                return HelperMethods.GetEntities().SaveEntity_ProviderPracticeRole(ProviderPracticeRoleObj, ProviderPracticeID);
            }
            return false;
        }

        #endregion ProviderPracticeRole
    }

    public static class ProviderPatientThirdPartyAppClass
    {
        #region ProviderPatientThirdPartyApp

        #region SaveProviderPatientThirdPartyApp

        public static bool SaveProviderPatientThirdPartyApp(ProviderPatientThirdPartyApp_model entity)
        {
            Guid PatientSurveyID = Guid.Empty;
            //var patientSurvey = PatientSurveyClass.GetEntityByPatient_Org_Practice_Provider_Survey_PatientSurvey(entity.PatientID, entity.OrganizationID, entity.PracticeID, entity.ProviderID, entity.SurveyID);


            PatientSurvey patientSurvey = HelperMethods.GetEntities().GetEntity_PatientSurvey(entity.PatientID, entity.OrganizationID, entity.PracticeID, entity.ProviderID, entity.SurveyID);
            if (patientSurvey != null)
            {
                PatientSurveyID = patientSurvey.ID;
            }

            if (PatientSurveyID != Guid.Empty)
            {
                bool exist = HelperMethods.GetEntities().CheckProviderPatientThirdPartyAppExistByAppID(PatientSurveyID, entity.ThirdPartyAppID);

                if (!exist)
                {
                    var app = new ProviderPatientThirdPartyApp
                    {
                        ThirdPartyAppID = entity.ThirdPartyAppID,
                        ProviderID = entity.ProviderID,
                        PatientSurveyID = PatientSurveyID,
                        IsActive = true,
                        CreatedDate = Utilities.ConvertServerCurrentDateTimeToClientDateTime(DateTime.Now)
                    };
                    return HelperMethods.GetEntities().SaveEntity_ProviderPatientThirdPartyApp(app);
                }
            }

            return false;
        }
        #endregion SaveProviderPatientThirdPartyApp

        #region GetProviderPatientThirdPartyApp

        public static List<ProviderPatientThirdPartyApp_model> GetProviderPatientThirdPartyApp(Guid PatientID, Guid OrganizationID, Guid PracticeID, Guid ProviderID, int SurveyID)
        {
            if (PatientID != null && SurveyID > 0)
            {
                Guid PatientSurveyID = Guid.Empty;

                PatientSurvey patientSurvey = HelperMethods.GetEntities().GetEntity_PatientSurvey(PatientID, OrganizationID, PracticeID, ProviderID, SurveyID);
                if (patientSurvey != null)
                {
                    PatientSurveyID = patientSurvey.ID;
                }

                return HelperMethods.GetEntities().GetProviderPatientThirdPartyApp(PatientSurveyID).Select(x => new ProviderPatientThirdPartyApp_model
                {
                    ID = x.ID,
                    AppName = x.ThirdPartyApp.AppName,
                    URL = x.ThirdPartyApp.URL,
                    ThirdPartyAppID = (int)x.ThirdPartyAppID,
                    ProviderID = (Guid)x.ProviderID,
                    PatientID = PatientID,
                    SurveyID = SurveyID,
                    PatientSurveyID = PatientSurveyID,
                    IsActive = x.IsActive,
                    OrganizationName = x.PatientSurvey.Organization.OrganizationName,
                    PracticeName = x.PatientSurvey.Practice.PracticeName
                }).ToList();
            }
            else
            {
                return null;
            }
        }

        public static List<ProviderPatientThirdPartyApp_model> GetPatientThirdPartyApp(Guid PatientID, int SurveyID)
        {
            List<ProviderPatientThirdPartyApp_model> AppList = new List<ProviderPatientThirdPartyApp_model>();

            if (PatientID != null && SurveyID > 0)
            {
                Guid PatientSurveyID = Guid.Empty;

                List<PatientSurvey_model> patientSurvey = PatientSurveyClass.GetEntityByPatient_Survey_PatientSurvey(PatientID, SurveyID);
                if (patientSurvey != null && patientSurvey.Count > 0)
                {
                    for (int i = 0; i < patientSurvey.Count; i++)
                    {
                        PatientSurveyID = patientSurvey[i].ID;

                        var List = HelperMethods.GetEntities().GetProviderPatientThirdPartyApp(PatientSurveyID).Select(x => new ProviderPatientThirdPartyApp_model
                        {
                            ID = x.ID,
                            AppName = x.ThirdPartyApp.AppName,
                            URL = x.ThirdPartyApp.URL,
                            ThirdPartyAppID = (int)x.ThirdPartyAppID,
                            ProviderID = (Guid)x.ProviderID,
                            PatientID = PatientID,
                            SurveyID = SurveyID,
                            PatientSurveyID = PatientSurveyID,
                            OrganizationID = x.PatientSurvey.OrganizationID,
                            PracticeID = x.PatientSurvey.PracticeID,
                            IsActive = x.IsActive
                        }).FirstOrDefault();

                        AppList.Add(List);
                    }
                }

                return AppList;
            }
            else
            {
                return null;
            }
        }

        #endregion GetProviderPatientThirdPartyApp

        #region DeleteProviderPatientThirdPartyApp
        public static bool DeletePatientThirdPartyApp(int ThirdPartyAppId, Guid ProviderID, Guid OrganizationID, Guid PracticeID, Guid PatientID, int SurveyID)
        {
            try
            {
                Guid PatientSurveyID = Guid.Empty;
                var patientSurvey = PatientSurveyClass.GetEntityByPatient_Org_Practice_Provider_Survey_PatientSurvey(PatientID, OrganizationID, PracticeID, ProviderID, SurveyID);
                if (patientSurvey != null)
                {
                    PatientSurveyID = patientSurvey.ID;
                }

                return HelperMethods.GetEntities().DeleteEntity_PatientThirdPartyApp(ThirdPartyAppId, PatientSurveyID);
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return false;
            }
        }

        #endregion DeleteProviderPatientThirdPartyApp

        #endregion ProviderPatientThirdPartyApp
    }

    public static class SalutationClass
    {
        #region Salutation

        public static List<Salutation_Model> GetSalutationList()
        {
            return HelperMethods.GetEntities().GetSalutationList().Select(x => new Salutation_Model
            {
                ID = x.ID,
                SalutationName = x.SalutationName,
                IsActive = x.IsActive
            }).ToList();
        }
        #endregion Salutation
    }

    public static class Pathways
    {
        #region CreatePathway

        public static Guid CreatePathway(Pathway_model _category)
        {
            var category = new Pathway
            {
                ID = Guid.NewGuid(),
                PathwayName = _category.PathwayName,
                Description = _category.Description,
                IsActive = _category.IsActive,
                CreatedOn = Utilities.ConvertServerCurrentDateTimeToClientDateTime(DateTime.Now)
            };
            return HelperMethods.GetEntities().SaveEntity_Pathway(category);
        }

        #endregion CreatePathway

        #region GetPathwayById

        public static Pathway_model GetPathwayById(Guid id)
        {
            Pathway category = HelperMethods.GetEntities().GetEntityById_Pathways(id);
            try
            {
                return new Pathway_model
                {
                    ID = category.ID,
                    PathwayName = category.PathwayName,
                    Description = category.Description,
                    IsActive = category.IsActive,
                    CreatedOn = Utilities.ConvertServerCurrentDateTimeToClientDateTime(DateTime.Now)
                };
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        #endregion GetPathwayById

        #region GetPathwayList

        public static List<Pathway_model> GetPathwayList()
        {
            List<Pathway_model> category = HelperMethods.GetEntities().GetEntity_PathwaysList().ToList().AsEnumerable().Select(x =>
             new Pathway_model
             {
                 ID = x.ID,
                 PathwayName = x.PathwayName,
                 Description = x.Description,
                 IsActive = x.IsActive,
                 CreatedOn = Utilities.ConvertServerCurrentDateTimeToClientDateTime(DateTime.Now)
             }).ToList();

            return category;
        }

        #endregion GetPathwayList

        #region DeletePathway
        public static string DeletePathwayById(Guid id)
        {
            string str = "";
            bool isExistsinPatient = HelperMethods.GetEntities().CheckPathwayExistsInPatientSurvey(id);
            if (!isExistsinPatient)
            {
                HelperMethods.GetEntities().DeleteEntity_Pathway(id);
                str = "Successfully Deleted!";
            }
            else
            {
                str = "Child Record Exists";
            }
            return str;
        }

        public static string DeleteMultiplePathway(string Ids)
        {
            string str = "";
            string[] values = Ids.Split(',');
            int length = values.Length;

            for (int i = 0; i < length; i++)
            {
                if (values[i] != "")
                {
                    Guid ID = new Guid(values[i]);

                    bool isExistsinPatient = HelperMethods.GetEntities().CheckPathwayExistsInPatientSurvey(ID);
                    if (!isExistsinPatient)
                    {
                        HelperMethods.GetEntities().DeleteEntity_Pathway(ID);
                        str = "Successfully Deleted!";
                    }
                    else
                    {
                        str = "Child Record Exists";
                    }
                }
            }
            return str;
        }

        #endregion DeletePathway

        #region UpdatePathway

        public static void UpdatePathway(Pathway_model _category)
        {
            Pathway category = HelperMethods.GetEntities().GetEntityById_Pathways((Guid)_category.ID);
            category.PathwayName = _category.PathwayName;
            category.Description = _category.Description;
            category.IsActive = _category.IsActive;

            HelperMethods.GetEntities().UpdateEntity_Pathway(category);
        }

        public static string UpdatePathwayIsActiveStatus(string Ids, bool Status)
        {
            string str = "";
            string[] values = Ids.Split(',');
            int length = values.Length;

            for (int i = 0; i < length; i++)
            {
                if (values[i] != "")
                {
                    Guid ID = new Guid(values[i]);
                    str = HelperMethods.GetEntities().UpdateEntityIsActiveStatus_Pathway(ID, Status);
                }
            }
            return str;
        }

        #endregion UpdatePathway

        #region SearchFilterPathway
        public static usp_search_PathwayByFilterCustom_model SearchFilterPathway(int TotalCount, int? StartIndex = -1, int? EndIndex = -1, string SearchString = null, bool? IsActive = null)
        {
            if (SearchString == "undefined")
                SearchString = null;

            List<usp_search_PathwayByFilter_Result> list = HelperMethods.GetEntities().GetEntityBySearchFilter_Pathway(out TotalCount, StartIndex, EndIndex, SearchString, IsActive).ToList();

            var objList = list.ToList().AsEnumerable().Select(x =>
            new usp_search_PathwayByFilter_Result_model
            {
                RowNo = x.RowNo,
                ID = x.ID,
                PathwayName = x.PathwayName,
                Description = x.Description,
                CreatedDate = x.CreatedOn,
                IsActive = x.IsActive
            }).ToList();

            return (new usp_search_PathwayByFilterCustom_model { PathwaySearchFilterList = objList, TotalCount = TotalCount });

        }

        #endregion SearchFilterPathway
    }

    public static class SetDefaultTime
    {
        #region GetDays

        public static int getDays(int SurveyID)
        {
            return HelperMethods.GetEntities().getDays(SurveyID);
        }

        public static string updateDays(int SurveyID, int Days)
        {
            return HelperMethods.GetEntities().updateDays(SurveyID, Days);
        }

        #endregion
    }

    public static class LinkedEHR
    {
        public static ConsumerDetail getConsumerDetails(string IHINo)
        {
            ConsumerDetail cd = new ConsumerDetail();

            var consumer = HelperMethods.GetEntities().getConsumerDetails(IHINo);

            if (consumer != null)
            {
                int ContactID = Convert.ToInt32(consumer.ContactID);

                cd.Firstname = consumer.Firstname;
                cd.Middlename = consumer.Middlename;
                cd.Lastname = consumer.Lastname;
                cd.DOB = consumer.DOB;
                cd.Gender = consumer.Gender;
                cd.Line1 = consumer.Line1;
                cd.Line2 = consumer.Line2;
                cd.Suburb = consumer.Suburb;
                cd.StateID = consumer.StateID;
                cd.Pincode = consumer.Pincode;
                cd.Email = consumer.Email;
                cd.IHI = consumer.IHI;

                cd.SID = HelperMethods.GetEntities().GetStatesByCode(cd.StateID);

                var addr = HelperMethods.GetEntities().getConsumerDetailAddrs(ContactID);

                if (addr != null)
                {
                    cd.country = addr.country;
                    cd.telecom = addr.telecom;
                }
            }
            else
            {
                cd = null;
            }

            return cd;
        }
    }

    public static class CINT
    {
        public static List<CINTScore> CINTScore(CINTScore objCINT)
        {
            List<CINTScore> lstCINTScore = new List<CINTScore>();

            try
            {
                objCINT.CreatedDate = DateTime.Now;
                int ID = HelperMethods.GetEntities().AddCINTScore(objCINT);

                lstCINTScore = HelperMethods.GetEntities().GetCINTScore(ID);
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
            }

            return lstCINTScore;
        }
    }
}