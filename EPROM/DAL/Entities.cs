using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;
using System.Web;
using Newtonsoft.Json;

namespace DAL
{
    public class Entities
    {
        public ShEhrDbEntities dbModel = null;

        public EPROMDBEntities db = null;

        #region Admin

        #region Token

        public string IsAlreadyReqForgetPwd(string Email, bool isRegister)
        {
            try
            {
                DateTime? TodayDate = Common.Utilities.ConvertServerCurrentDateTimeToClientDateTime(DateTime.Now);

                var db = GetDataContext();
                Token T = (from token in db.Tokens
                           where token.Email.Equals(Email) && DbFunctions.TruncateTime(token.ExpiryDate) > DbFunctions.TruncateTime(TodayDate) && token.IsUsed.Equals(false) && token.IsRegister == isRegister
                           select token).FirstOrDefault();

                if (T != null)
                {
                    T.ExpiryDate = TodayDate.Value.AddDays(1);
                    T.ModifiedDate = TodayDate.Value;
                    db.SaveChanges();
                    return T.Token1.ToString();
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

        public string GenerateToken(string Email, string Token, bool isRegister)
        {
            try
            {
                var db = GetDataContext();
                DateTime? TodayDate = Common.Utilities.ConvertServerCurrentDateTimeToClientDateTime(DateTime.Now);

                Token T = new Token();
                T.Email = Email;
                T.ID = Guid.NewGuid();
                T.Token1 = Token;
                T.IsRegister = isRegister;
                T.ExpiryDate = TodayDate.Value.AddDays(1);
                T.IsUsed = false;
                T.CreatedDate = TodayDate.Value;
                db.Tokens.Add(T);
                db.SaveChanges();
                return T.Token1.ToString();
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return "";
            }
        }

        public string ValidateToken(string Token)
        {
            try
            {
                var db = GetDataContext();

                DateTime? TodayDate = Common.Utilities.ConvertServerCurrentDateTimeToClientDateTime(DateTime.Now);
                Token T = (from token in db.Tokens
                           where token.Token1.Equals(Token) && token.IsUsed.Equals(false) && DbFunctions.TruncateTime(token.ExpiryDate) > DbFunctions.TruncateTime(TodayDate)
                           select token).FirstOrDefault();
                if (T != null)
                {
                    return T.Email.ToString();
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

        public bool CheckIsTokenExists(string Email, bool isRegister)
        {
            try
            {
                var db = GetDataContext();
                Token T = (from token in db.Tokens
                           where token.Email.Equals(Email) && token.IsRegister == isRegister
                           select token).FirstOrDefault();

                if (T != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return false;
            }
        }

        public string DeleteToken(string Email, string Token, bool isRegister)
        {
            try
            {
                var db = GetDataContext();
                Token entity = (from token in db.Tokens
                                where token.Email.Equals(Email) && token.Token1.Equals(Token) && token.IsRegister == isRegister
                                select token).FirstOrDefault();
                if (entity != null)
                {
                    string email = entity.Email;
                    db.Entry(entity).State = System.Data.Entity.EntityState.Deleted;
                    db.SaveChanges();
                    return email;
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

        #endregion Token

        #region SystemFlags

        #region GetSystemFlag

        public SystemFlag GetEntityById_SystemFlag(int id)
        {
            try
            {
                var db = GetDataContext();
                return db.SystemFlags.FirstOrDefault(x => x.ID == id);
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        public List<SystemFlag> GetEntitySystemFlag_ByFlagGroupId(int FlagGroupId)
        {
            try
            {
                var db = GetDataContext();
                return db.SystemFlags.Where(x => x.FlagGroupID == FlagGroupId && x.IsActive == true).ToList();
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        public List<SystemFlag> GetEntity_SystemFlag()
        {
            try
            {
                var db = GetDataContext();
                return db.SystemFlags.Where(x => x.IsActive == true).OrderBy(x => x.SystemFlagName).ToList();
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        #endregion GetSystemFlag

        #region CreateSystemFlags

        public int SaveEntity_SystemFlags(SystemFlag entity)
        {
            try
            {
                var db = GetDataContext();
                db.Entry(entity).State = System.Data.Entity.EntityState.Added;
                db.SaveChanges();
                return entity.ID;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return 0;
            }
        }

        public short GetEntityDisplayOrderAutoGenerate_SystemFlag()
        {
            var db = GetDataContext();

            var obj = db.SystemFlags.OrderByDescending(x => x.DisplayOrder).Select(x => x.DisplayOrder).FirstOrDefault();

            if (obj > 0)
            {
                return Convert.ToInt16(obj + 1);
            }
            else
            {
                return 1;
            }
        }

        public int GetEntityCheckDisplayOrderExistOrNot(int DisplayOrder)
        {
            var db = GetDataContext();
            var obj = db.SystemFlags.Where(x => x.DisplayOrder == DisplayOrder).Select(x => x.DisplayOrder).FirstOrDefault();
            return Convert.ToInt16(obj);
        }

        #endregion CreateSystemFlags

        #region UpdateSystemFlag
        public void UpdateEntity_SystemFlag(SystemFlag _entity)
        {
            try
            {
                var db = GetDataContext();
                SystemFlag entity = db.SystemFlags.FirstOrDefault(x => x.ID == _entity.ID);
                entity.SystemFlagName = _entity.SystemFlagName;
                entity.FlagGroupID = _entity.FlagGroupID;
                entity.DefaultValue = _entity.DefaultValue;
                entity.Value = _entity.Value;
                entity.DisplayOrder = _entity.DisplayOrder;
                entity.IsActive = _entity.IsActive;
                entity.ModifiedDate = _entity.ModifiedDate;
                db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
            }
        }

        public string UpdateEntityIsActiveStatus_SystemFlag(int ID, bool Status)
        {
            var db = GetDataContext();
            SystemFlag entity = db.SystemFlags.FirstOrDefault(x => x.ID == ID);
            entity.IsActive = Status;
            db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return "1";
        }

        #endregion UpdateSystemFlag

        #region DeleteSystemFlag

        public void DeleteEntity_SystemFlag(int id)
        {
            try
            {
                var db = GetDataContext();
                SystemFlag entity = db.SystemFlags.FirstOrDefault(x => x.ID == id);
                db.Entry(entity).State = System.Data.Entity.EntityState.Deleted;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
            }
        }

        #endregion DeleteSystemFlag

        #region SearchFilterSystemFlag

        public List<usp_search_SystemFlagByFilter_Result> GetEntityBySearchFilter_SystemFlag(out int totalrecords, int? StartIndex = -1, int? EndIndex = -1, string SearchString = null, bool? IsActive = null)
        {
            totalrecords = 0;
            try
            {
                var db = GetDataContext();

                ObjectParameter TotalRecords = new ObjectParameter("TotalRecords", typeof(int));
                TotalRecords.Value = 0;

                var obj = db.usp_search_SystemFlagByFilter(StartIndex != -1 ? StartIndex + 1 : null, EndIndex != -1 ? EndIndex : null, SearchString, IsActive, TotalRecords).ToList();
                totalrecords = Convert.ToInt32(TotalRecords.Value);
                return obj;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        #endregion SearchFilterSystemFlag

        #endregion SystemFlags

        #region FlagGroups

        #region Get FlagGroups

        public List<FlagGroup> GetEntityListById_FlagGroups(int id)
        {
            var db = GetDataContext();
            return db.FlagGroups.Where(x => x.ID == id && x.IsActive == true).ToList();
        }

        public List<FlagGroup> GetEntity_FlagGroup()
        {
            var db = GetDataContext();
            return db.FlagGroups.Where(x => x.IsActive == true).ToList();
        }

        public FlagGroup GetEntityById_FlagGroup(int id)
        {
            try
            {
                var db = GetDataContext();
                return db.FlagGroups.FirstOrDefault(x => x.ID == id);
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        public SystemFlag GetEntityBySystemFlagGroupID_FlagGroup(int id)
        {
            var db = GetDataContext();
            return db.SystemFlags.FirstOrDefault(x => x.FlagGroupID == id);
        }

        public short GetEntityDisplayOrderAutoGenerate_FlagGroups()
        {
            var db = GetDataContext();

            var obj = db.FlagGroups.OrderByDescending(x => x.DisplayOrder).Select(x => x.DisplayOrder).FirstOrDefault();

            if (obj > 0)
            {
                return Convert.ToInt16(obj + 1);
            }
            else
            {
                return 1;
            }
        }

        public int GetEntity_CheckDisplayOrderExist_FlagGroups(int DisplayOrder)
        {
            var db = GetDataContext();
            var obj = db.FlagGroups.Where(x => x.DisplayOrder == DisplayOrder).Select(x => x.DisplayOrder).FirstOrDefault();
            return Convert.ToInt16(obj);
        }

        #endregion Get FlagGroups

        #region Create FlagGroup

        public int SaveEntity_FlagGroups(FlagGroup entity)
        {
            try
            {
                var db = GetDataContext();
                db.Entry(entity).State = System.Data.Entity.EntityState.Added;
                db.SaveChanges();
                return entity.ID;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return 0;
            }
        }

        #endregion Create FlagGroup

        #region Update FlagGroup

        public void UpdateEntity_FlagGroups(FlagGroup _entity)
        {
            try
            {
                var db = GetDataContext();
                FlagGroup entity = db.FlagGroups.FirstOrDefault(x => x.ID == _entity.ID);
                entity.FlagGroupName = _entity.FlagGroupName;
                entity.Description = _entity.Description;
                entity.DisplayOrder = _entity.DisplayOrder;
                entity.IsActive = _entity.IsActive;
                entity.ModifiedDate = _entity.ModifiedDate;
                db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
            }
        }

        public string UpdateEntityIsActiveStatus_FlagGroup(int ID, bool Status)
        {
            var db = GetDataContext();
            FlagGroup entity = db.FlagGroups.FirstOrDefault(x => x.ID == ID);
            entity.IsActive = Status;
            db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return "1";
        }

        #endregion Update FlagGroup

        #region Delete FlagGroups

        public void DeleteEntity_FlagGroups(int id)
        {
            try
            {
                var db = GetDataContext();
                FlagGroup entity = db.FlagGroups.FirstOrDefault(x => x.ID == id);
                db.Entry(entity).State = System.Data.Entity.EntityState.Deleted;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
            }
        }

        #endregion Delete FlagGroups

        #region SearchFilter FlagGroups

        public List<usp_search_FlagGroupByFilter_Result> GetEntityBySearchFilter_FlagGroups(out int totalrecords, int? StartIndex = -1, int? EndIndex = -1, string SearchString = null, bool? IsActive = null)
        {
            totalrecords = 0;
            try
            {
                var db = GetDataContext();

                ObjectParameter TotalRecords = new ObjectParameter("TotalRecords", typeof(int));
                TotalRecords.Value = 0;

                var obj = db.usp_search_FlagGroupByFilter(StartIndex != -1 ? StartIndex + 1 : null, EndIndex != -1 ? EndIndex : null, SearchString, IsActive, TotalRecords).ToList();

                totalrecords = Convert.ToInt32(TotalRecords.Value);
                return obj;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        #endregion SearchFilter FlagGroups

        #endregion FlagGroups

        #region PatientCategory

        #region GetPatientCategory

        public PatientCategory GetEntityById_PatientCategories(short id)
        {
            try
            {
                var db = GetDataContext();
                return db.PatientCategories.FirstOrDefault(x => x.ID == id);
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        public List<PatientCategory> GetEntity_PatientCategoriesList()
        {
            try
            {
                var db = GetDataContext();
                return db.PatientCategories.Where(x => x.IsActive == true).OrderBy(x => x.PatientCategoryName).ToList();
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        #endregion GetPatientCategory

        #region Create PatientCategory

        public int SaveEntity_PatientCategory(PatientCategory entity)
        {
            try
            {
                var db = GetDataContext();
                db.Entry(entity).State = System.Data.Entity.EntityState.Added;
                db.SaveChanges();
                return entity.ID;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return 0;
            }
        }

        #endregion Create PatientCategory

        #region UpdatePatientCategory

        public void UpdateEntity_PatientCategory(PatientCategory _entity)
        {
            try
            {
                var db = GetDataContext();
                PatientCategory entity = db.PatientCategories.FirstOrDefault(x => x.ID == _entity.ID);
                entity.PatientCategoryName = _entity.PatientCategoryName;
                entity.Description = _entity.Description;
                entity.IsActive = _entity.IsActive;
                db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
            }
        }

        public string UpdateEntityIsActiveStatus_PatientCategory(int ID, bool Status)
        {
            var db = GetDataContext();
            PatientCategory entity = db.PatientCategories.FirstOrDefault(x => x.ID == ID);

            entity.IsActive = Status;
            db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return "1";
        }

        #endregion UpdatePatientCategory

        #region Delete PatientCategory

        public void DeleteEntity_PatientCategory(short id)
        {
            try
            {
                var db = GetDataContext();
                PatientCategory entity = db.PatientCategories.FirstOrDefault(x => x.ID == id);
                db.Entry(entity).State = System.Data.Entity.EntityState.Deleted;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
            }
        }

        #endregion Delete PatientCategory

        #region SearchFilterPatientCategory
        public List<usp_search_PatientCategoryByFilter_Result> GetEntityBySearchFilter_PatientCategory(out int totalrecords, int? StartIndex = -1, int? EndIndex = -1, string SearchString = null, bool? IsActive = null)
        {
            totalrecords = 0;
            try
            {
                var db = GetDataContext();

                ObjectParameter TotalRecords = new ObjectParameter("TotalRecords", typeof(int));
                TotalRecords.Value = 0;

                var obj = db.usp_search_PatientCategoryByFilter(StartIndex != -1 ? StartIndex + 1 : null, EndIndex != -1 ? EndIndex : null, SearchString, IsActive, TotalRecords).ToList();
                totalrecords = Convert.ToInt32(TotalRecords.Value);
                return obj;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        #endregion SearchFilterPatientCategory

        #endregion PatientCategory

        #region SurveyCategory

        #region GetSurveyCategory
        public SurveyCategory GetEntityById_SurveyCategories(short id)
        {
            try
            {
                var db = GetDataContext();
                return db.SurveyCategories.FirstOrDefault(x => x.ID == id);
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        public List<SurveyCategory> GetEntity_SurveyCategoriesList(short id)
        {
            try
            {
                var db = GetDataContext();
                return db.SurveyCategories.Where(x => x.IsActive == true && x.ID != id && x.ParentSurveyCategoryID == null).OrderBy(x => x.SurvayCategoryName).ToList();
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        public List<SurveyCategory> GetEntity_CategoriesList()
        {
            try
            {
                var db = GetDataContext();
                return db.SurveyCategories.Where(x => x.IsActive == true && x.ParentSurveyCategoryID == null).OrderBy(x => x.SurvayCategoryName).ToList();
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        public List<SurveyCategory> GetEntity_SubSurveyCategoriesList(short id)
        {
            try
            {
                var db = GetDataContext();
                return db.SurveyCategories.Where(x => x.IsActive == true && x.ParentSurveyCategoryID == id).OrderBy(x => x.SurvayCategoryName).ToList();
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        #endregion GetSurveyCategory

        #region Create SurveyCategory

        public int SaveEntity_SurveyCategory(SurveyCategory entity)
        {
            try
            {
                var db = GetDataContext();
                db.Entry(entity).State = System.Data.Entity.EntityState.Added;
                db.SaveChanges();
                return entity.ID;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return 0;
            }
        }

        #endregion Create SurveyCategory

        #region UpdateSurveyCategory

        public void UpdateEntity_SurveyCategory(SurveyCategory _entity)
        {
            try
            {
                var db = GetDataContext();
                SurveyCategory entity = db.SurveyCategories.FirstOrDefault(x => x.ID == _entity.ID);
                entity.SurvayCategoryName = _entity.SurvayCategoryName;
                entity.Description = _entity.Description;
                entity.ParentSurveyCategoryID = _entity.ParentSurveyCategoryID;
                entity.IsActive = _entity.IsActive;
                db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
            }
        }

        public string UpdateEntityIsActiveStatus_SurveyCategory(int ID, bool Status)
        {
            var db = GetDataContext();
            SurveyCategory entity = db.SurveyCategories.FirstOrDefault(x => x.ID == ID);

            entity.IsActive = Status;
            db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return "1";
        }

        #endregion UpdateSurveyCategory

        #region Delete SurveyCategory

        public void DeleteEntity_SurveyCategory(short id)
        {
            try
            {
                var db = GetDataContext();
                SurveyCategory entity = db.SurveyCategories.FirstOrDefault(x => x.ID == id);
                db.Entry(entity).State = System.Data.Entity.EntityState.Deleted;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
            }
        }

        #endregion Delete SurveyCategory

        #region CheckSurveyCategory

        public bool CheckSurveyCategoryExist_Survey(short id)
        {
            try
            {
                var db = GetDataContext();
                var obj = db.Surveys.Where(x => x.SurveyCategoryID == id || x.SurveySubCategoryID == id).FirstOrDefault();
                if (obj == null)
                    return false;
                else
                    return true;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return false;
            }
        }
        #endregion CheckSurveyCategory


        #region SearchFilterSurveyCategory
        public List<usp_search_SurveyCategoryByFilter_Result> GetEntityBySearchFilter_SurveyCategory(out int totalrecords, int? StartIndex = -1, int? EndIndex = -1, string SearchString = null, bool? IsActive = null)
        {
            totalrecords = 0;
            try
            {
                var db = GetDataContext();

                ObjectParameter TotalRecords = new ObjectParameter("TotalRecords", typeof(int));
                TotalRecords.Value = 0;

                var obj = db.usp_search_SurveyCategoryByFilter(StartIndex != -1 ? StartIndex + 1 : null, EndIndex != -1 ? EndIndex : null, SearchString, IsActive, TotalRecords).ToList();
                totalrecords = Convert.ToInt32(TotalRecords.Value);
                return obj;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        #endregion SearchFilterCategory

        #endregion SurveyCategory

        #region ThirdPartyApp

        #region GetThirdPartyApp

        public ThirdPartyApp GetEntityById_ThirdPartyApp(int id)
        {
            try
            {
                var db = GetDataContext();
                return db.ThirdPartyApps.FirstOrDefault(x => x.ID == id);
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        public List<ThirdPartyApp> GetEntity_ThirdPartyAppList()
        {
            try
            {
                var db = GetDataContext();
                return db.ThirdPartyApps.Where(x => x.IsActive == true).OrderBy(x => x.AppName).ToList();
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        public List<ThirdPartyApp> GetEntity_ThirdPartyAppByCategoryID(short? SurveyCategoryID, short? SurveySubCategoryID)
        {
            try
            {
                var db = GetDataContext();
                if (SurveySubCategoryID != null && SurveySubCategoryID > 0)
                {
                    return db.ThirdPartyApps.Where(x => x.IsActive == true && x.SurveyCategoryID == SurveyCategoryID && x.SurveySubCategoryID == SurveySubCategoryID).OrderBy(x => x.AppName).ToList();
                }
                else
                {
                    return db.ThirdPartyApps.Where(x => x.IsActive == true && x.SurveyCategoryID == SurveyCategoryID).OrderBy(x => x.AppName).ToList();
                }
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        #endregion GetThirdPartyApp

        #region Create ThirdPartyApp

        public int SaveEntity_ThirdPartyApp(ThirdPartyApp entity)
        {
            try
            {
                var db = GetDataContext();
                db.Entry(entity).State = System.Data.Entity.EntityState.Added;
                db.SaveChanges();
                return entity.ID;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return 0;
            }
        }

        #endregion Create ThirdPartyApp

        #region Update ThirdPartyApp

        public void UpdateEntity_ThirdPartyApp(ThirdPartyApp _entity)
        {
            try
            {
                var db = GetDataContext();
                ThirdPartyApp entity = db.ThirdPartyApps.FirstOrDefault(x => x.ID == _entity.ID);
                entity.AppName = _entity.AppName;
                entity.URL = _entity.URL;
                entity.SurveyCategoryID = _entity.SurveyCategoryID;
                entity.SurveySubCategoryID = _entity.SurveySubCategoryID;
                entity.Address = _entity.Address;
                entity.Email = _entity.Email;
                entity.MobileNo = _entity.MobileNo;
                entity.PhoneNo = _entity.PhoneNo;
                entity.IsActive = _entity.IsActive;
                entity.ModifiedDate = Common.Utilities.ConvertServerCurrentDateTimeToClientDateTime(DateTime.Now);
                db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
            }
        }

        public string UpdateEntityIsActiveStatus_ThirdPartyApp(int ID, bool Status)
        {
            var db = GetDataContext();
            ThirdPartyApp entity = db.ThirdPartyApps.FirstOrDefault(x => x.ID == ID);

            entity.IsActive = Status;
            db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return "1";
        }

        #endregion Update ThirdPartyApp

        #region Delete ThirdPartyApp

        public void DeleteEntity_ThirdPartyApp(int id)
        {
            try
            {
                var db = GetDataContext();
                ThirdPartyApp entity = db.ThirdPartyApps.FirstOrDefault(x => x.ID == id);
                db.Entry(entity).State = System.Data.Entity.EntityState.Deleted;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
            }
        }

        #endregion Delete ThirdPartyApp

        #region SearchFilterThirdPartyApp
        public List<usp_search_ThirdPartyAppByFilter_Result> GetEntityBySearchFilter_ThirdPartyApp(out int totalrecords, int? StartIndex = -1, int? EndIndex = -1, string SearchString = null, bool? IsActive = null)
        {
            totalrecords = 0;
            try
            {
                var db = GetDataContext();

                ObjectParameter TotalRecords = new ObjectParameter("TotalRecords", typeof(int));
                TotalRecords.Value = 0;

                var obj = db.usp_search_ThirdPartyAppByFilter(StartIndex != -1 ? StartIndex + 1 : null, EndIndex != -1 ? EndIndex : null, SearchString, IsActive, TotalRecords).ToList();
                totalrecords = Convert.ToInt32(TotalRecords.Value);
                return obj;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        #endregion SearchFilterThirdPartyApp

        #region CheckProviderPatient_ThirdPartyApp

        public bool CheckProviderPatient_ThirdPartyApp(int ThirdPartyAppID)
        {
            try
            {
                var db = GetDataContext();
                var obj = db.ProviderPatientThirdPartyApps.FirstOrDefault(x => x.ThirdPartyAppID == ThirdPartyAppID);
                if (obj == null)
                    return false;
                else
                    return true;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return false;
            }
        }

        #endregion CheckPatientCategory

        #endregion ThirdPartyApp

        #region Survey

        #region GetSurveyBySurveyCategoryID
        public List<Survey> GetEntityBy_SurveyCategoryId_Survey(short id, short subId)
        {
            try
            {
                var db = GetDataContext();

                if (subId > 0)
                {
                    return db.Surveys.Where(x => x.SurveyCategoryID == id && x.SurveySubCategoryID == subId && x.IsActive == true).ToList();
                }
                else
                {
                    return db.Surveys.Where(x => x.SurveyCategoryID == id && x.SurveySubCategoryID == null && x.IsActive == true).ToList();
                }
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        #endregion GetSurveyBySurveyCategoryID

        #region GetSurveyByExternalID
        public Survey GetEntityBy_ExternalID_Survey(string ExternalID)
        {
            try
            {
                var db = GetDataContext();
                return db.Surveys.Where(x => x.ExternalID == ExternalID && x.IsActive == true).FirstOrDefault();
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        #endregion GetSurveyByExternalID

        #region GetSurveyNameById
        public Survey GetSurveyDetails_ById_Survey(int id)
        {
            try
            {
                var db = GetDataContext();
                return db.Surveys.FirstOrDefault(x => x.ID == id);
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        #endregion GetSurveyNameById

        #region GetSurvey
        public Survey GetEntityById_Survey(short id)
        {
            try
            {
                var db = GetDataContext();
                return db.Surveys.FirstOrDefault(x => x.ID == id);
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        public List<Survey> GetEntity_SurveyList()
        {
            try
            {
                var db = GetDataContext();
                return db.Surveys.Where(x => x.IsActive == true).OrderBy(x => x.Title).ToList();
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        #endregion GetSurvey

        #region Create Survey

        public int SaveEntity_Survey(Survey entity)
        {
            try
            {
                var db = GetDataContext();
                db.Entry(entity).State = EntityState.Added;
                db.SaveChanges();
                return entity.ID;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return 0;
            }
        }

        #endregion Create Survey

        #region UpdateSurveyFileName

        public string UpdateSurveyFileName_Survey(int ID, string FileName)
        {
            var db = GetDataContext();
            Survey entity = db.Surveys.FirstOrDefault(x => x.ID == ID);

            if (entity != null)
            {
                entity.FileName = FileName;
                db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return "1";
            }
            return "";
        }

        #endregion UpdateSurveyFileName

        #region UpdateSurvey

        public void UpdateEntity_Survey(Survey _entity)
        {
            try
            {
                var db = GetDataContext();
                Survey entity = db.Surveys.FirstOrDefault(x => x.ID == _entity.ID);
                entity.Title = _entity.Title;
                entity.ExternalTitle = _entity.ExternalTitle;
                entity.ExternalID = _entity.ExternalID;
                entity.CollectorID = _entity.CollectorID;
                entity.ContentCode = _entity.ContentCode;
                entity.URL = _entity.URL;
                entity.CreatedByUserID = _entity.CreatedByUserID;
                entity.SurveyCategoryID = _entity.SurveyCategoryID;
                entity.SurveySubCategoryID = _entity.SurveySubCategoryID;
                entity.SurveyTypeID = _entity.SurveyTypeID;
                entity.StartDate = _entity.StartDate;
                entity.EndDate = _entity.EndDate;
                entity.Language = _entity.Language;
                entity.IsPublish = _entity.IsPublish;
                entity.Description = _entity.Description;
                entity.FileName = _entity.FileName;
                entity.IsActive = _entity.IsActive;
                entity.NormValue = _entity.NormValue;
                entity.Days = _entity.Days;
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
            }
        }

        public string UpdateEntityIsActiveStatus_Survey(int ID, bool Status)
        {
            var db = GetDataContext();
            Survey entity = db.Surveys.FirstOrDefault(x => x.ID == ID);
            if (entity != null)
            {
                entity.IsActive = Status;
                db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return "1";
            }
            return "";
        }

        #endregion UpdateSurvey

        #region Delete Survey

        public void DeleteEntity_Survey(short id)
        {
            try
            {
                var db = GetDataContext();
                Survey entity = db.Surveys.FirstOrDefault(x => x.ID == id);
                db.Entry(entity).State = System.Data.Entity.EntityState.Deleted;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
            }
        }

        #endregion Delete Survey

        #region SearchFilterSurvey
        public List<usp_search_SurveyByFilter_Result> GetEntityBySearchFilter_Survey(out int totalrecords, int? StartIndex = -1, int? EndIndex = -1, string SearchString = null, bool? IsActive = null)
        {
            totalrecords = 0;
            try
            {
                var db = GetDataContext();

                ObjectParameter TotalRecords = new ObjectParameter("TotalRecords", typeof(int));
                TotalRecords.Value = 0;

                var obj = db.usp_search_SurveyByFilter(StartIndex != -1 ? StartIndex + 1 : null, EndIndex != -1 ? EndIndex : null, SearchString, IsActive, TotalRecords).ToList();
                totalrecords = Convert.ToInt32(TotalRecords.Value);
                return obj;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        #endregion SearchFilter

        #endregion Survey

        #region SurvetType

        public List<SurveyType> GetEntity_SurveyTypes()
        {
            try
            {
                var db = GetDataContext();
                return db.SurveyTypes.Where(x => x.IsActive == true).ToList();
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        #endregion SurvetType

        #region OT
        public int SaveEntity_OT(OrganizationType entity)
        {
            try
            {
                var db = GetDataContext();
                db.Entry(entity).State = EntityState.Added;
                db.SaveChanges();
                return entity.ID;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return 0;
            }
        }

        public OrganizationType GetEntityById_OT(int ID)
        {
            try
            {
                var db = GetDataContext();
                return db.OrganizationTypes.FirstOrDefault(x => x.ID == ID);
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        public List<OrganizationType> GetEntity_OTList()
        {
            try
            {
                var db = GetDataContext();
                return db.OrganizationTypes.Where(x => x.IsActive == true).OrderBy(x => x.OrganizationType1).ToList();
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        public List<usp_search_OrganizationTypeByFilter_Result> GetEntityBySearchFilter_OT(out int totalrecords, int? StartIndex = -1, int? EndIndex = -1, string SearchString = null, bool? IsActive = null)
        {
            totalrecords = 0;
            try
            {
                var db = GetDataContext();

                ObjectParameter TotalRecords = new ObjectParameter("TotalRecords", typeof(int));
                TotalRecords.Value = 0;

                var obj = db.usp_search_OrganizationTypeByFilter(StartIndex != -1 ? StartIndex + 1 : null, EndIndex != -1 ? EndIndex : null, SearchString, IsActive, TotalRecords).ToList();
                totalrecords = Convert.ToInt32(TotalRecords.Value);
                return obj;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        public OrganizationType GetEntityById_OTs(int ID)
        {
            try
            {
                var db = GetDataContext();
                return db.OrganizationTypes.FirstOrDefault(x => x.ID == ID);
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        public void UpdateEntity_OT(OrganizationType _entity)
        {
            try
            {
                var db = GetDataContext();
                OrganizationType entity = db.OrganizationTypes.FirstOrDefault(x => x.ID == _entity.ID);
                entity.OrganizationType1 = _entity.OrganizationType1;
                entity.IsActive = _entity.IsActive;
                db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
            }
        }

        public string UpdateEntityIsActiveStatus_OT(int ID, bool Status)
        {
            var db = GetDataContext();
            OrganizationType entity = db.OrganizationTypes.FirstOrDefault(x => x.ID == ID);

            entity.IsActive = Status;
            db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return "1";
        }

        public void DeleteEntity_OT(int ID)
        {
            try
            {
                var db = GetDataContext();
                OrganizationType entity = db.OrganizationTypes.FirstOrDefault(x => x.ID == ID);
                db.Entry(entity).State = System.Data.Entity.EntityState.Deleted;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
            }
        }
        #endregion

        #region UT
        public int SaveEntity_UT(UserType entity)
        {
            try
            {
                var db = GetDataContext();
                db.Entry(entity).State = EntityState.Added;
                db.SaveChanges();
                return entity.ID;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return 0;
            }
        }

        public UserType GetEntityById_UT(int ID)
        {
            try
            {
                var db = GetDataContext();
                return db.UserTypes.FirstOrDefault(x => x.ID == ID);
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        public List<UserType> GetEntity_UTList()
        {
            try
            {
                var db = GetDataContext();
                return db.UserTypes.Where(x => x.IsActive == true).OrderBy(x => x.UserType1).ToList();
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        public List<usp_search_UserTypeByFilter_Result> GetEntityBySearchFilter_UT(out int totalrecords, int? StartIndex = -1, int? EndIndex = -1, string SearchString = null, bool? IsActive = null)
        {
            totalrecords = 0;
            try
            {
                var db = GetDataContext();

                ObjectParameter TotalRecords = new ObjectParameter("TotalRecords", typeof(int));
                TotalRecords.Value = 0;

                var obj = db.usp_search_UserTypeByFilter(StartIndex != -1 ? StartIndex + 1 : null, EndIndex != -1 ? EndIndex : null, SearchString, IsActive, TotalRecords).ToList();
                totalrecords = Convert.ToInt32(TotalRecords.Value);
                return obj;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        public UserType GetEntityById_UTs(int ID)
        {
            try
            {
                var db = GetDataContext();
                return db.UserTypes.FirstOrDefault(x => x.ID == ID);
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        public void UpdateEntity_UT(UserType _entity)
        {
            try
            {
                var db = GetDataContext();
                UserType entity = db.UserTypes.FirstOrDefault(x => x.ID == _entity.ID);
                entity.UserType1 = _entity.UserType1;
                entity.IsActive = _entity.IsActive;
                db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
            }
        }

        public string UpdateEntityIsActiveStatus_UT(int ID, bool Status)
        {
            var db = GetDataContext();
            UserType entity = db.UserTypes.FirstOrDefault(x => x.ID == ID);

            entity.IsActive = Status;
            db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return "1";
        }

        public void DeleteEntity_UT(int ID)
        {
            try
            {
                var db = GetDataContext();
                UserType entity = db.UserTypes.FirstOrDefault(x => x.ID == ID);
                db.Entry(entity).State = System.Data.Entity.EntityState.Deleted;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
            }
        }
        #endregion

        #region Indicators

        #region GetIndicators

        public Indicator GetEntityById_Indicators(short id)
        {
            try
            {
                var db = GetDataContext();
                return db.Indicators.FirstOrDefault(x => x.ID == id);
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        public List<Indicator> GetEntity_IndicatorsList()
        {
            try
            {
                var db = GetDataContext();
                return db.Indicators.Where(x => x.IsActive == true).OrderBy(x => x.IndicatorName).ToList();
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        #endregion GetIndicators

        #region CreateIndicators

        public int SaveEntity_Indicators(Indicator entity)
        {
            try
            {
                var db = GetDataContext();
                db.Entry(entity).State = System.Data.Entity.EntityState.Added;
                db.SaveChanges();
                return entity.ID;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return 0;
            }
        }

        #endregion CreateIndicators

        #region UpdateIndicators

        public void UpdateEntity_Indicators(Indicator _entity)
        {
            try
            {
                var db = GetDataContext();
                Indicator entity = db.Indicators.FirstOrDefault(x => x.ID == _entity.ID);
                entity.IndicatorName = _entity.IndicatorName;
                entity.Description = _entity.Description;
                entity.IsActive = _entity.IsActive;
                db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
            }
        }

        public string UpdateEntityIsActiveStatus_Indicators(int ID, bool Status)
        {
            var db = GetDataContext();
            Indicator entity = db.Indicators.FirstOrDefault(x => x.ID == ID);

            entity.IsActive = Status;
            db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return "1";
        }

        #endregion UpdateIndicators

        #region DeleteIndicators

        public void DeleteEntity_Indicators(short id)
        {
            try
            {
                var db = GetDataContext();
                Indicator entity = db.Indicators.FirstOrDefault(x => x.ID == id);
                db.Entry(entity).State = System.Data.Entity.EntityState.Deleted;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
            }
        }

        #endregion DeleteIndicators

        #region SearchFilterIndicators
        public List<usp_search_IndicatorsByFilter_Result> GetEntityBySearchFilter_Indicators(out int totalrecords, int? StartIndex = -1, int? EndIndex = -1, string SearchString = null, bool? IsActive = null)
        {
            totalrecords = 0;
            try
            {
                var db = GetDataContext();

                ObjectParameter TotalRecords = new ObjectParameter("TotalRecords", typeof(int));
                TotalRecords.Value = 0;

                var obj = db.usp_search_IndicatorsByFilter(StartIndex != -1 ? StartIndex + 1 : null, EndIndex != -1 ? EndIndex : null, SearchString, IsActive, TotalRecords).ToList();
                totalrecords = Convert.ToInt32(TotalRecords.Value);
                return obj;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        #endregion SearchFilterIndicators

        #region CheckIndicator

        public bool CheckIndicatorExistsInPatientIndicators(short IndicatorId)
        {
            try
            {
                var db = GetDataContext();
                var objIndicators = db.PatientIndicators.FirstOrDefault(x => x.IndicatorID == IndicatorId);
                if (objIndicators == null)
                    return false;
                else
                    return true;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return false;
            }
        }

        #endregion CheckIndicator

        #endregion Indicators

        #region eProm's Set Default Time

        public int getDays(int SurveyID)
        {
            int days = 0;
            try
            {
                var db = GetDataContext();

                var defaultTime = db.DefaultDays.Where(x => x.SurveyID == SurveyID).FirstOrDefault();

                if (defaultTime != null)
                {
                    days = defaultTime.Days ?? 0;
                }

                return days;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public void addDays(int SurveyID)
        {
            try
            {
                var db = GetDataContext();

                var defaultTime = new DefaultDay();

                defaultTime.SurveyID = SurveyID;
                defaultTime.Days = 0;

                db.DefaultDays.Add(defaultTime);

                db.SaveChanges();
            }
            catch (Exception)
            {

            }
        }

        public string updateDays(int SurveyID, int Days)
        {
            string strVal = "unsuccess";

            try
            {
                var db = GetDataContext();

                var defaultTime = db.DefaultDays.Where(x => x.SurveyID == SurveyID).FirstOrDefault();

                if (defaultTime != null)
                {
                    defaultTime.Days = Days;

                    db.SaveChanges();

                    strVal = "success";
                }

                return strVal;
            }
            catch (Exception)
            {
                return strVal;
            }
        }

        #endregion

        #endregion Admin

        #region Private
        public EPROMDBEntities GetDataContext()
        {
            return new EPROMDBEntities();
        }

        public ShEhrDbEntities GetShEhrDbContext()
        {
            return new ShEhrDbEntities();
        }

        #endregion Private

        #region Role

        public List<webpages_Roles> GetEntity_Role()
        {
            try
            {
                var db = GetDataContext();
                return db.webpages_Roles.OrderBy(x => x.RoleName).ToList();
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        #endregion

        #region User

        public Guid? GetUserIdByUserName(string userName)
        {
            try
            {
                var db = GetDataContext();
                return db.UserDetails.Where(x => x.UserName.Trim().Equals(userName.Trim(), StringComparison.OrdinalIgnoreCase)).Select(x => x.ID).FirstOrDefault();
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return new Guid();
            }
        }

        public UserDetail GetUserDetailsByUserId(Guid? UserId)
        {
            try
            {
                var db = GetDataContext();
                return db.UserDetails.Where(x => x.ID == UserId).FirstOrDefault();
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        public bool Entity_UpdateuserDetail(UserDetail entity)
        {
            try
            {
                var db = GetDataContext();
                var ud = db.UserDetails.Where(x => x.ID.Equals(entity.ID)).FirstOrDefault();
                if (ud != null)
                {
                    ud.FirstName = entity.FirstName ?? ud.FirstName;
                    ud.LastName = entity.LastName ?? ud.LastName;
                    ud.MiddleName = entity.MiddleName ?? ud.MiddleName;
                    ud.DOB = entity.DOB ?? ud.DOB;
                    ud.Gender = entity.Gender ?? ud.Gender;
                }

                db.Entry(ud).State = EntityState.Modified;
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return false;
            }
        }
        #endregion User

        #region Providers

        public string SaveEntity_Providers(Provider entity)
        {
            try
            {
                var db = GetDataContext();
                db.Entry(entity).State = System.Data.Entity.EntityState.Added;
                db.SaveChanges();
                return entity.ID.ToString();
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return "";
            }
        }

        public Guid? UpdateEntity_Providers(Provider objEntity)
        {
            Guid? returnValue = null;
            try
            {
                var db = GetDataContext();
                Provider entity = db.Providers.FirstOrDefault(x => x.ID == objEntity.ID);
                if (entity == null)
                {
                    return null;
                }

                entity.AddressID = objEntity.AddressID ?? entity.AddressID;
                entity.ContactID = objEntity.ContactID ?? entity.ContactID;
                entity.SalutationID = objEntity.SalutationID ?? entity.SalutationID;
                entity.Description = objEntity.Description ?? entity.Description;
                entity.HealthProviderIdentifier = objEntity.HealthProviderIdentifier ?? entity.HealthProviderIdentifier;
                entity.MedicareProviderNumber = objEntity.MedicareProviderNumber ?? entity.MedicareProviderNumber;
                entity.IsActive = objEntity.IsActive;
                entity.ModifiedDate = objEntity.ModifiedDate ?? entity.ModifiedDate;
                entity.ModifiedBy = objEntity.ModifiedBy ?? entity.ModifiedBy;
                entity.PHN = objEntity.PHN ?? entity.PHN;
                entity.LHN = objEntity.LHN ?? entity.LHN;
                entity.LHD = objEntity.LHD ?? entity.LHD;
                entity.ProviderTypeID = objEntity.ProviderTypeID ?? entity.ProviderTypeID;

                var ud = db.UserDetails.AsEnumerable().Where(x => x.ID.Equals(entity.UserID)).FirstOrDefault();
                if (ud != null)
                {
                    ud.UserName = objEntity.UserDetail.UserName ?? ud.UserName;
                    ud.FirstName = objEntity.UserDetail.FirstName ?? ud.FirstName;
                    ud.LastName = objEntity.UserDetail.LastName ?? ud.LastName;
                    ud.MiddleName = objEntity.UserDetail.MiddleName ?? ud.MiddleName;
                    ud.Email = objEntity.UserDetail.Email ?? ud.Email;
                    ud.DOB = objEntity.UserDetail.DOB ?? ud.DOB;
                    ud.Gender = objEntity.UserDetail.Gender ?? ud.Gender;
                }

                db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                returnValue = entity.ID;
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
            return returnValue;
        }

        public List<ProviderType> GetProviderTypes()
        {
            List<ProviderType> objList = new List<ProviderType>();
            try
            {
                var db = GetDataContext();
                objList = db.ProviderTypes.Where(x => x.IsActive == true).ToList();
                if (objList == null)
                {
                    return null;
                }
                else
                {
                    return objList;
                }
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        public string GetLastProviderCode()
        {
            try
            {
                var db = GetDataContext();
                if (db.Providers.Count() > 0)
                {
                    var Code = (from P in db.Providers orderby P.Code descending select P.Code).FirstOrDefault();
                    if (Code != null)
                    {
                        var SubCode = Code.Substring(1, Code.Length - 1);
                        if (SubCode != null && SubCode != "")
                        {
                            var length = SubCode.Length;
                            string number = Convert.ToString(Convert.ToInt32(SubCode) + 1);
                            var zero = "";
                            var count = length - number.Length;
                            for (int i = 0; i < count; i++)
                            {
                                zero += "0";
                            }
                            SubCode = "P" + zero + number;
                        }
                        return SubCode;
                    }
                    else
                    {
                        return "P00001";
                    }
                }
                else
                {
                    return "P00001";
                }
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return "";
            }
        }

        public Provider CheckExistingProvider(Guid? UserID)
        {
            var returnValue = new Provider();
            try
            {
                var db = GetDataContext();
                returnValue = db.Providers.AsEnumerable().Where(x => x.UserID.Equals(UserID)).FirstOrDefault();
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
            }
            return returnValue;
        }

        public bool CheckExistingUser(Guid? UserID)
        {
            try
            {
                var db = GetDataContext();
                UserDetail entity = db.UserDetails.AsEnumerable().Where(x => x.ID.Equals(UserID)).FirstOrDefault();
                if (entity == null)
                    return false;
                else
                    return true;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return false;
            }
        }

        public bool ChangeEmailVerificationStatus(Guid? UserID)
        {
            bool returnValue = false;
            try
            {
                var db = GetDataContext();
                UserDetail entity = db.UserDetails.AsEnumerable().Where(x => x.ID.Equals(UserID)).FirstOrDefault();
                if (entity == null)
                {
                    return returnValue;
                }

                entity.EmailConfirmed = true;

                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return false;
            }
        }

        public bool? GetEmailVerificationStatus(Guid? UserID)
        {
            bool? returnValue = false;
            try
            {
                var db = GetDataContext();
                returnValue = db.UserDetails.AsEnumerable().Where(x => x.ID.Equals(UserID)).Select(x => x.EmailConfirmed).FirstOrDefault();

                if (returnValue == null)
                    return false;
                else
                    return returnValue;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return false;
            }
        }

        public Guid? GetProviderID(Guid userID)
        {
            Guid? returnValue = null;
            try
            {
                var db = new EPROMDBEntities();
                returnValue = db.Providers.AsEnumerable().Where(x => x.UserID.Equals(userID)).Select(x => x.ID).FirstOrDefault();
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
            }
            return returnValue;
        }

        public string UpdateProviderTypeID(Guid userID, short providerTypeID)
        {
            try
            {
                var db = new EPROMDBEntities();
                var obj = db.Providers.AsEnumerable().Where(x => x.UserID.Equals(userID) && x.ProviderTypeID == null).FirstOrDefault();
                if (obj != null)
                {
                    obj.ProviderTypeID = providerTypeID;

                    db.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return "1";
                }
                return "";
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return "";
            }
        }

        public int? Save_Contact(Contact entity)
        {
            try
            {
                var db = GetDataContext();

                db.Entry(entity).State = EntityState.Added;
                db.SaveChanges();

                return entity.ID;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return 0;
            }
        }

        public int? ManageProviderContactDetails(Guid userId, Contact entity)
        {
            try
            {
                var db = GetDataContext();
                var contactId = db.Providers.AsEnumerable().Where(x => x.UserID.Equals(userId)).Select(x => x.ContactID).FirstOrDefault();
                if (contactId != null)
                {
                    var objCN = db.Contacts.AsEnumerable().Where(x => x.ID.Equals(contactId)).FirstOrDefault();
                    if (objCN != null)
                    {
                        objCN.Phone = entity.Phone ?? objCN.Phone;
                        objCN.Mobile = entity.Mobile ?? objCN.Mobile;
                        objCN.Mobile2 = entity.Mobile2 ?? objCN.Mobile2;
                        objCN.FAX = entity.FAX ?? objCN.FAX;
                        objCN.Pager = entity.Pager ?? objCN.Pager;
                        objCN.Emergency = entity.Emergency ?? objCN.Emergency;
                        objCN.EmailBusiness = entity.EmailBusiness ?? objCN.EmailBusiness;
                        objCN.EmailPersonal = entity.EmailPersonal ?? objCN.EmailPersonal;
                        objCN.Skype = entity.Skype ?? objCN.Skype;
                        objCN.LinkedIN = entity.LinkedIN ?? objCN.LinkedIN;
                        objCN.Website = entity.Website ?? objCN.Website;
                        objCN.OtherFieldsJSON = entity.OtherFieldsJSON ?? objCN.OtherFieldsJSON;
                        objCN.Description = entity.Description ?? objCN.Description;
                        objCN.IsActive = entity.IsActive;
                        objCN.UserId = entity.UserId;
                        objCN.ModifiedDate = entity.ModifiedDate ?? objCN.ModifiedDate;
                        objCN.ModifiedBy = entity.ModifiedBy ?? objCN.ModifiedBy;
                    }

                    db.SaveChanges();
                    return (int)contactId;
                }
                else
                {
                    db.Entry(entity).State = System.Data.Entity.EntityState.Added;
                    db.SaveChanges();

                    return entity.ID;
                }
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return 0;
            }
        }

        public int? ManageContact(Guid userId, Contact entity)
        {
            try
            {
                var db = GetDataContext();

                var contactId = db.Contacts.AsEnumerable().Where(x => x.UserId.Equals(userId)).Select(x => x.ID).FirstOrDefault();
                if (contactId > 0)
                {
                    var objCN = db.Contacts.AsEnumerable().Where(x => x.ID.Equals(contactId)).FirstOrDefault();
                    if (objCN != null)
                    {
                        objCN.Phone = entity.Phone ?? objCN.Phone;
                        objCN.Mobile = entity.Mobile ?? objCN.Mobile;
                        objCN.Mobile2 = entity.Mobile2 ?? objCN.Mobile2;
                        objCN.FAX = entity.FAX ?? objCN.FAX;
                        objCN.Pager = entity.Pager ?? objCN.Pager;
                        objCN.Emergency = entity.Emergency ?? objCN.Emergency;
                        objCN.EmailBusiness = entity.EmailBusiness ?? objCN.EmailBusiness;
                        objCN.EmailPersonal = entity.EmailPersonal ?? objCN.EmailPersonal;
                        objCN.Skype = entity.Skype ?? objCN.Skype;
                        objCN.LinkedIN = entity.LinkedIN ?? objCN.LinkedIN;
                        objCN.Website = entity.Website ?? objCN.Website;
                        objCN.OtherFieldsJSON = entity.OtherFieldsJSON ?? objCN.OtherFieldsJSON;
                        objCN.Description = entity.Description ?? objCN.Description;
                        objCN.IsActive = entity.IsActive;
                        objCN.UserId = entity.UserId;
                        db.Entry(objCN).State = System.Data.Entity.EntityState.Modified;
                    }
                    else
                    {
                        db.Entry(entity).State = System.Data.Entity.EntityState.Added;
                    }
                }
                else
                {
                    db.Entry(entity).State = System.Data.Entity.EntityState.Added;
                    db.SaveChanges();

                    return entity.ID;
                }

                db.SaveChanges();
                return (int)contactId;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return 0;
            }
        }

        public short? GetProviderTypeID(Guid userID)
        {
            short? returnValue = null;
            try
            {
                var db = new EPROMDBEntities();
                returnValue = db.Providers.AsEnumerable().Where(x => x.UserID.Equals(userID)).Select(x => x.ProviderTypeID).FirstOrDefault();
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
            }
            return returnValue;
        }

        public Provider GetProviderDetails(Guid? providerId)
        {
            try
            {
                var db = GetDataContext();
                return db.Providers.AsEnumerable().Where(x => x.ID.Equals(providerId)).FirstOrDefault();
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        public Contact GetContactDetails(int? contactID)
        {
            try
            {
                var db = GetDataContext();
                return db.Contacts.AsEnumerable().Where(x => x.ID.Equals(contactID)).FirstOrDefault();
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        public Contact GetContactDetails_ByUserId(Guid? UserId)
        {
            try
            {
                var db = GetDataContext();
                return db.Contacts.AsEnumerable().Where(x => x.UserId.Equals(UserId)).FirstOrDefault();
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        public string getProviderEmailID(Guid ProviderId)
        {
            string email = string.Empty;

            try
            {
                var db = GetDataContext();
                var provider = db.Providers.Where(x => x.ID == ProviderId).FirstOrDefault();

                if (provider != null)
                {
                    var usr = db.UserDetails.Where(x => x.ID == provider.UserID).FirstOrDefault();

                    if (usr != null)
                    {
                        email = usr.Email;
                    }
                }
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
            }

            return email;
        }

        #endregion Providers

        #region General

        public static string CustomeWebRequest(string requestUrl, string method = "POST")
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUrl);
                request.Method = method;
                request.ContentLength = 0;
                request.ContentType = "application/json";
                WebResponse response = request.GetResponse();
                string result = new StreamReader(response.GetResponseStream()).ReadToEnd();
                string returnvalue = new JavaScriptSerializer().Deserialize<string>(result);
                return returnvalue;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return "";
            }
        }

        #endregion General

        #region Address

        public long? Save_Address(Address entity)
        {
            try
            {
                var db = GetDataContext();

                db.Entry(entity).State = EntityState.Added;
                db.SaveChanges();

                return entity.ID;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return 0;
            }
        }

        public long? ManageProviderAddress(Guid userId, Address entity)
        {
            try
            {
                var db = GetDataContext();
                var addressId = db.Providers.AsEnumerable().Where(x => x.UserID.Equals(userId)).Select(x => x.AddressID).FirstOrDefault();
                if (addressId != null)
                {
                    var objAdd = db.Addresses.AsEnumerable().Where(x => x.ID.Equals(addressId)).FirstOrDefault();
                    if (objAdd != null)
                    {
                        objAdd.Line1 = entity.Line1 ?? objAdd.Line1;
                        objAdd.Line2 = entity.Line2 ?? objAdd.Line2;
                        objAdd.CountryID = entity.CountryID ?? objAdd.CountryID;
                        objAdd.StateID = entity.StateID ?? objAdd.StateID;
                        objAdd.ZipCode = entity.ZipCode ?? objAdd.ZipCode;
                    }

                    db.SaveChanges();
                    return (long)addressId;
                }
                else
                {
                    db.Entry(entity).State = System.Data.Entity.EntityState.Added;
                    db.SaveChanges();

                    return entity.ID;
                }
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return 0;
            }
        }

        public long? ManageAddress(Guid userId, Address entity)
        {
            try
            {
                var db = GetDataContext();
                var addressId = db.Addresses.AsEnumerable().Where(x => x.UserId.Equals(userId)).Select(x => x.ID).FirstOrDefault();
                if (addressId > 0)
                {
                    var objAdd = db.Addresses.AsEnumerable().Where(x => x.ID.Equals(addressId)).FirstOrDefault();
                    if (objAdd != null)
                    {
                        objAdd.Line1 = entity.Line1 ?? objAdd.Line1;
                        objAdd.Line2 = entity.Line2 ?? objAdd.Line2;
                        objAdd.CountryID = entity.CountryID ?? objAdd.CountryID;
                        objAdd.StateID = entity.StateID ?? objAdd.StateID;
                        objAdd.ZipCode = entity.ZipCode ?? objAdd.ZipCode;
                        objAdd.Suburb = entity.Suburb ?? objAdd.Suburb;
                        objAdd.UserId = entity.UserId ?? objAdd.UserId;
                        db.Entry(objAdd).State = System.Data.Entity.EntityState.Modified;
                    }
                    else
                    {
                        db.Entry(entity).State = System.Data.Entity.EntityState.Added;
                    }

                    db.SaveChanges();
                    return (long)addressId;
                }
                else
                {
                    db.Entry(entity).State = System.Data.Entity.EntityState.Added;
                    db.SaveChanges();

                    return entity.ID;
                }
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return 0;
            }
        }

        public List<Country> GetCountries()
        {
            List<Country> objList = new List<Country>();
            try
            {
                var db = GetDataContext();
                objList = db.Countries.Where(x => x.IsActive == true).ToList();

                if (objList == null)
                    return null;
                else
                    return objList;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        public int GetStatesByCode(string code)
        {
            try
            {
                int SID = 0;

                var db = GetDataContext();
                var st = db.States.Where(x => x.Code == code && x.IsActive == true).FirstOrDefault();

                if (st != null)
                {
                    SID = st.ID;
                }

                return SID;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return 0;
            }
        }

        public List<State> GetStates()
        {
            List<State> objList = new List<State>();
            try
            {
                var db = GetDataContext();
                objList = db.States.Where(x => x.IsActive == true).ToList();
                if (objList == null)
                    return null;
                else
                    return objList;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }

        }

        public Address GetAddressDetails(long? addressID)
        {
            try
            {
                var db = GetDataContext();
                return db.Addresses.AsEnumerable().Where(x => x.ID.Equals(addressID)).FirstOrDefault();
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        public bool Entity_UpdateAddress(Address entity)
        {
            try
            {
                var db = GetDataContext();
                var ad = db.Addresses.Where(x => x.ID.Equals(entity.ID)).FirstOrDefault();
                if (ad != null)
                {
                    ad.ZipCode = entity.ZipCode;
                    ad.StateID = entity.StateID;
                    ad.CountryID = entity.CountryID;
                    ad.Line1 = entity.Line1;
                    ad.Line2 = entity.Line2;
                    ad.ModifiedDate = entity.ModifiedDate;
                    ad.Suburb = entity.Suburb;
                }
                db.Entry(ad).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return false;
            }
        }

        public Address GetAddressDetails_ByUserId(Guid? UserId)
        {
            try
            {
                var db = GetDataContext();
                return db.Addresses.AsEnumerable().Where(x => x.UserId.Equals(UserId)).FirstOrDefault();
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }


        #endregion Address

        #region SecretQuestions

        #region GetSystemFlag

        public List<SecretQuestion> GetEntity_SecretQuestion()
        {
            try
            {
                var db = GetDataContext();
                return db.SecretQuestions.Where(x => x.IsActive == true).ToList();
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        public string CheckSecretQuestionAnswer(Guid userId, int QuestionId, string answer)
        {
            try
            {
                var db = GetDataContext();
                var objanswer = db.UserSecretQuestions.AsEnumerable().Where(x => x.UserID.Equals(userId) && x.SecretQuestionID == QuestionId && x.Answer.Equals(answer)).FirstOrDefault();

                if (objanswer == null)
                    return "Answer is not Correct";
                else
                    return "";

            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return "";
            }
        }

        #endregion GetSystemFlag

        #endregion

        #region Cookie

        public static void SetCookie(string value, string cookieName, int cookieExpireDay = 1)
        {
            try
            {
                DateTime? TodayDate = DateTime.Now;

                HttpCookie myCookie = new HttpCookie(cookieName);
                myCookie.Value = value;
                myCookie.Expires = TodayDate.Value.AddHours(cookieExpireDay);
                HttpContext.Current.Response.Cookies.Add(myCookie);
                HttpContext.Current.Response.SetCookie(myCookie);
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
            }
        }

        #endregion Cookie

        #region Patient

        #region CheckPatientCategory

        public bool CheckPatientCategoryExistsInPatients(short PatientCategoryId)
        {
            try
            {
                var db = GetDataContext();
                var objPatient = db.Patients.FirstOrDefault(x => x.PatientCategoryID == PatientCategoryId);
                if (objPatient == null)
                    return false;
                else
                    return true;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return false;
            }
        }

        #endregion CheckPatientCategory

        #region GetPatient

        public Guid? GetPatientID(Guid userID)
        {
            Guid? returnValue = null;
            try
            {
                var db = new EPROMDBEntities();
                returnValue = db.Patients.AsEnumerable().Where(x => x.UserID.Equals(userID)).Select(x => x.ID).FirstOrDefault();
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
            }
            return returnValue;
        }

        public Patient GetEntityById_Patient(Guid id)
        {
            try
            {
                var db = GetDataContext();
                return db.Patients.FirstOrDefault(x => x.ID == id);
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        public List<Patient> GetEntity_Patient()
        {
            try
            {
                var db = GetDataContext();
                return db.Patients.Where(x => x.IsActive == true).ToList();
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        public List<PatientProvider> GetProviderPatientDetailByUserID(Guid? userid, Guid? OrganizationId, Guid? PracticeId)
        {
            try
            {
                var db = GetDataContext();
                var obj = (from a in db.PatientProviders
                           join b in db.Patients on a.PatientID equals b.ID
                           where a.ProviderID == userid && a.OrganizationID == OrganizationId && a.PracticeID == PracticeId && b.IsActive == true
                           select a).ToList();
                return obj;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        public List<PatientProvider> GetProviderPatientDetailBySeletedPatient(Guid? userid, Guid? OrganizationId, Guid? PracticeId, string patientName)
        {
            try
            {
                var db = GetDataContext();
                var obj = (from a in db.PatientProviders
                           join b in db.Patients on a.PatientID equals b.ID
                           where a.ProviderID == userid && a.OrganizationID == OrganizationId && a.PracticeID == PracticeId && b.IsActive == true && (a.Patient.UserDetail.FirstName.ToLower().Contains(patientName.ToLower()) || a.Patient.UserDetail.LastName.ToLower().Contains(patientName.ToLower()) || a.Patient.IHINumber.ToLower().Contains(patientName.ToLower()) || a.Patient.MedicareNumber.ToLower().Contains(patientName.ToLower()) || a.Patient.Contact.Mobile.ToLower().Contains(patientName.ToLower()) || a.Patient.Contact.EmailPersonal.ToLower().Contains(patientName.ToLower()))
                           select a).ToList();
                return obj;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        public PatientProvider GetPatientProviderByIds(Guid? PatientId, Guid? ProviderId, Guid? OrganizationId, Guid? PracticeId)
        {
            try
            {
                var db = GetDataContext();
                return db.PatientProviders.Where(x => x.PatientID == PatientId && x.ProviderID == ProviderId && x.OrganizationID == OrganizationId && x.PracticeID == PracticeId).FirstOrDefault();
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        public List<PatientProvider> GetPatientProviderListByPatientID(Guid? PatientId)
        {
            try
            {
                var db = GetDataContext();
                return db.PatientProviders.Where(x => x.PatientID == PatientId).ToList();
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        public Patient GetPatientDetailsByUserID(Guid? userid)
        {
            try
            {
                var db = GetDataContext();
                return db.Patients.Where(x => x.ID == userid && x.IsActive == true).FirstOrDefault();
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        public bool CheckPatientExist(string IHINumber, string MedicareNumber, Guid ProviderId, Guid OrganizationId, Guid PracticeId)
        {
            try
            {
                var db = GetDataContext();
                var obj = (from a in db.Patients
                           join b in db.PatientProviders on a.ID equals b.PatientID
                           where a.IHINumber.Equals(IHINumber, StringComparison.OrdinalIgnoreCase) && a.MedicareNumber.Equals(MedicareNumber, StringComparison.OrdinalIgnoreCase) && b.ProviderID == ProviderId && b.OrganizationID == OrganizationId && b.PracticeID == PracticeId
                           select b).FirstOrDefault();

                if (obj == null)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return false;
            }
        }

        public Patient SearchPatientDetailByUniqueNumber(string IHINumber, string MedicareNumber)
        {
            try
            {
                var db = GetDataContext();

                Patient entity = db.Patients.Where(x => x.IHINumber.Equals(IHINumber, StringComparison.OrdinalIgnoreCase) || x.MedicareNumber.Equals(MedicareNumber, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

                return entity;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        #endregion GetPatient

        #region Create Patient

        public bool SaveEntity_Patient(Patient entity)
        {
            try
            {
                var db = GetDataContext();
                db.Entry(entity).State = System.Data.Entity.EntityState.Added;
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return false;
            }
        }

        #endregion Create Patient

        #region Create PatientProvider

        public bool SaveEntity_PatientProvider(PatientProvider entity)
        {
            try
            {
                var db = GetDataContext();
                db.Entry(entity).State = System.Data.Entity.EntityState.Added;
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return false;
            }
        }

        public bool CheckPatientProviderExist(Guid? ProviderId, Guid? PatientId, Guid? OrganizationId, Guid? PracticeId)
        {
            try
            {
                var db = GetDataContext();
                var obj = db.PatientProviders.Where(x => x.ProviderID == ProviderId && x.PatientID == PatientId && x.OrganizationID == OrganizationId && x.PracticeID == PracticeId).FirstOrDefault();

                if (obj == null)
                    return false;
                else
                    return true;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return false;
            }
        }

        #endregion Create PatientProvider

        #region UpdatePatient

        public void UpdateEntity_Patient(Patient _entity)
        {
            try
            {
                var db = GetDataContext();
                Patient entity = db.Patients.FirstOrDefault(x => x.ID == _entity.ID);
                entity.ContactID = _entity.ContactID;
                entity.Code = _entity.Code;
                entity.PatientCategoryID = _entity.PatientCategoryID;
                entity.AddressID = _entity.AddressID;
                entity.Description = _entity.Description;
                entity.IHINumber = _entity.IHINumber;
                entity.MedicareNumber = _entity.MedicareNumber;
                entity.Salutation = _entity.Salutation;
                entity.IsActive = _entity.IsActive;
                entity.ModifiedDate = _entity.ModifiedDate;
                entity.ModifiedBy = _entity.ModifiedBy;
                db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
            }
        }

        public void UpdateStatus_Patient(Patient _entity)
        {
            try
            {
                var db = GetDataContext();
                Patient entity = db.Patients.FirstOrDefault(x => x.ID == _entity.ID);
                entity.IsActive = _entity.IsActive;
                entity.ModifiedDate = Common.Utilities.ConvertServerCurrentDateTimeToClientDateTime(DateTime.Now);
                entity.ModifiedBy = _entity.ModifiedBy;
                db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
            }
        }

        #endregion UpdatePatient      

        #region Check Duplicate

        public bool CheckExistingMedicure(string MedicareNumber)
        {
            try
            {
                var db = GetDataContext();
                Patient entity = db.Patients.Where(x => x.MedicareNumber.Equals(MedicareNumber)).FirstOrDefault();
                if (entity == null)
                    return false;
                else
                    return true;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return false;
            }
        }

        public bool CheckExistingIHINumber(string IHINumber)
        {
            try
            {
                var db = GetDataContext();
                Patient entity = db.Patients.Where(x => x.IHINumber.Equals(IHINumber)).FirstOrDefault();
                if (entity == null)
                    return false;
                else
                    return true;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return false;
            }
        }

        #endregion Check Duplicate

        #endregion

        #region PatientSurvey

        #region GetPatientSurvey

        public List<PatientSurvey> GetEntityByPatient_Provider_Org_Practice_ID_PatientSurvey(Guid PatientId, Guid ProviderID, Guid OrganizationID, Guid PracticeID, bool isAllPatient, bool IsCompleted)
        {
            try
            {
                var db = GetDataContext();
                if (!IsCompleted)
                {
                    if (isAllPatient && PatientId == Guid.Empty)
                    {
                        return db.PatientSurveys.Include("PatientSurvey_Pathway_PatientSurveyStatus").Where(x => x.OrganizationID == OrganizationID && x.PracticeID == PracticeID).ToList();
                    }
                    else
                    {
                        return db.PatientSurveys.Include("PatientSurvey_Pathway_PatientSurveyStatus").Where(x => x.PatientID == PatientId && x.OrganizationID == OrganizationID && x.PracticeID == PracticeID).ToList();
                    }
                }
                else
                {
                    if (isAllPatient && PatientId == Guid.Empty)
                    {
                        var objList = (from a in db.PatientSurveys
                                       join b in db.PatientSurveyStatus on a.ID equals b.PatientSurveyID
                                       where a.OrganizationID == OrganizationID
                                       && a.PracticeID == PracticeID
                                       && b.Status.Equals("completed", StringComparison.OrdinalIgnoreCase)
                                       orderby b.CreatedDate
                                       select a.ID).ToList();

                        return db.PatientSurveys.Where(x => objList.Contains(x.ID)).ToList();
                    }
                    else
                    {
                        var objList = (from a in db.PatientSurveys
                                       join b in db.PatientSurveyStatus on a.ID equals b.PatientSurveyID
                                       where a.PatientID == PatientId
                                       && a.OrganizationID == OrganizationID
                                       && a.PracticeID == PracticeID
                                       && b.Status.Equals("completed", StringComparison.OrdinalIgnoreCase)
                                       orderby b.CreatedDate
                                       select a.ID).ToList();

                        return db.PatientSurveys.Where(x => objList.Contains(x.ID)).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        public List<PatientSurvey> GetEntityByPatient_Org_Practice_ID_PatientSurvey(Guid PatientId, Guid ProviderID, Guid OrganizationID, Guid PracticeID, bool isAllPatient, bool IsCompleted)
        {
            try
            {
                var db = GetDataContext();
                if (!IsCompleted)
                {
                    if (isAllPatient && PatientId == Guid.Empty)
                    {
                        return db.PatientSurveys.Include("PatientSurvey_Pathway_PatientSurveyStatus").Where(x => x.ProviderID == ProviderID && x.OrganizationID == OrganizationID && x.PracticeID == PracticeID).ToList();
                    }
                    else
                    {
                        return db.PatientSurveys.Include("PatientSurvey_Pathway_PatientSurveyStatus").Where(x => x.PatientID == PatientId && x.ProviderID == ProviderID && x.OrganizationID == OrganizationID && x.PracticeID == PracticeID).ToList();
                    }
                }
                else
                {
                    if (isAllPatient && PatientId == Guid.Empty)
                    {
                        var objList = (from a in db.PatientSurveys
                                       join b in db.PatientSurveyStatus on a.ID equals b.PatientSurveyID
                                       where a.ProviderID == ProviderID
                                       && a.OrganizationID == OrganizationID
                                       && a.PracticeID == PracticeID
                                       && b.Status.Equals("completed", StringComparison.OrdinalIgnoreCase)
                                       orderby b.CreatedDate
                                       select a.ID).ToList();

                        return db.PatientSurveys.Where(x => objList.Contains(x.ID)).ToList();
                    }
                    else
                    {
                        var objList = (from a in db.PatientSurveys
                                       join b in db.PatientSurveyStatus on a.ID equals b.PatientSurveyID
                                       where a.PatientID == PatientId
                                       && a.ProviderID == ProviderID
                                       && a.OrganizationID == OrganizationID
                                       && a.PracticeID == PracticeID
                                       && b.Status.Equals("completed", StringComparison.OrdinalIgnoreCase)
                                       orderby b.CreatedDate
                                       select a.ID).ToList();

                        return db.PatientSurveys.Where(x => objList.Contains(x.ID)).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        public List<PatientSurvey> GetEntity_PatientSurvey_PatientID(Guid PatientId, Guid ProviderId, bool isCompleted)
        {
            try
            {
                var db = GetDataContext();
                if (!isCompleted)
                {
                    if (ProviderId != Guid.Empty)
                    {
                        return db.PatientSurveys.Where(x => x.PatientID == PatientId && x.ProviderID == ProviderId).ToList();
                    }
                    else
                    {
                        return db.PatientSurveys.Where(x => x.PatientID == PatientId).ToList();
                    }
                }
                else
                {
                    if (ProviderId != Guid.Empty)
                    {
                        var objList = (from a in db.PatientSurveys
                                       join b in db.PatientSurveyStatus on a.ID equals b.PatientSurveyID
                                       where a.PatientID == PatientId
                                       && a.ProviderID == ProviderId
                                       && b.Status.Equals("completed", StringComparison.OrdinalIgnoreCase)
                                       orderby b.CreatedDate
                                       select a.ID).ToList();

                        return db.PatientSurveys.Where(x => objList.Contains(x.ID)).ToList();
                    }
                    else
                    {
                        var objList = (from a in db.PatientSurveys
                                       join b in db.PatientSurveyStatus on a.ID equals b.PatientSurveyID
                                       where a.PatientID == PatientId
                                       && b.Status.Equals("completed", StringComparison.OrdinalIgnoreCase)
                                       orderby b.CreatedDate
                                       select a.ID).ToList();


                        return db.PatientSurveys.Where(x => objList.Contains(x.ID)).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        public PatientSurvey GetEntity_PatientSurvey(Guid? PatientId, Guid? OrganizationId, Guid? PracticeId, Guid? ProviderId, int? SurveyId)
        {
            try
            {
                var db = GetDataContext();
                return db.PatientSurveys.Where(x => x.PatientID == PatientId && x.SurveyID == SurveyId && x.OrganizationID == OrganizationId && x.PracticeID == PracticeId && x.ProviderID == ProviderId).FirstOrDefault();
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        public List<PatientSurvey> GetEntity_PatientSurveyList(Guid? OrganizationId, Guid? PracticeId, Guid? ProviderId, int? SurveyId)
        {
            try
            {
                var db = GetDataContext();
                return db.PatientSurveys.Where(x => x.SurveyID == SurveyId && x.OrganizationID == OrganizationId && x.PracticeID == PracticeId && x.ProviderID == ProviderId).ToList();
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        public List<PatientSurvey> GetEntityByPatientIdSurveyID_PatientSurvey(Guid? PatientId, int SurveyId)
        {
            try
            {
                var db = GetDataContext();
                return db.PatientSurveys.Where(x => x.PatientID == PatientId && x.SurveyID == SurveyId).ToList();
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        public List<PatientSurvey> GetEntityByID_PatientSurveyList(Guid? PatientSurveyID)
        {
            try
            {
                var db = GetDataContext();
                return db.PatientSurveys.Where(x => x.ID == PatientSurveyID).ToList();
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        #endregion GetPatientSurvey

        #region CheckPatientSurvey_Active

        public bool CheckPatientSurvey_Active(Guid PatientSurveyId)
        {
            try
            {
                var db = GetDataContext();

                bool isActive = false;

                var objPatientStats = db.PatientSurveys.Where(x => x.ID == PatientSurveyId && x.IsActive == true).FirstOrDefault();

                if (objPatientStats != null)
                {
                    isActive = true;
                }
                else
                {
                    isActive = false;
                }
                return isActive;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return false;
            }
        }

        #endregion CheckPatientSurvey_Active

        #region Create PatientSurvey

        public string AddPatientSurveyEProms(Guid organizationID, Guid practiceID, Guid providerID, Guid patientID, string surveyID)
        {
            var db = GetDataContext();
            string[] ids = surveyID.Split(',');

            for (int i = 0; i < ids.Length; i++)
            {
                int SID = Convert.ToInt32(ids[i].ToString());

                var isExist = db.PatientSurveys.Where(x => x.OrganizationID == organizationID && x.PracticeID == practiceID && x.ProviderID == providerID && x.PatientID == patientID && x.SurveyID == SID).FirstOrDefault();

                if (isExist == null)
                {
                    Guid id = Guid.NewGuid();

                    PatientSurvey tblPS = new PatientSurvey();

                    tblPS.ID = id;
                    tblPS.OrganizationID = organizationID;
                    tblPS.PracticeID = practiceID;
                    tblPS.ProviderID = providerID;
                    tblPS.PatientID = patientID;
                    tblPS.SurveyID = SID;
                    tblPS.StartDate = DateTime.Now;
                    tblPS.EndDate = DateTime.Now;
                    tblPS.IsActive = true;
                    tblPS.CreatedDate = DateTime.Now;
                    tblPS.IsSend = false;

                    db.PatientSurveys.Add(tblPS);
                    db.SaveChanges();
                }
            }

            return "";
        }

        public string SaveEntity_PatientSurvey(PatientSurvey entity)
        {
            try
            {
                var db = GetDataContext();
                db.Entry(entity).State = EntityState.Added;
                db.SaveChanges();
                return entity.ID.ToString();
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return "";
            }
        }

        #endregion Create PatientSurvey

        #region UpdatePatientSurvey

        public string UpdateEntity_PatientSurvey(PatientSurvey _entity)
        {
            try
            {
                var db = GetDataContext();

                PatientSurvey entity = db.PatientSurveys.Where(x => x.ProviderID == _entity.ProviderID && x.OrganizationID == _entity.OrganizationID && x.PracticeID == _entity.PracticeID && x.PatientID == _entity.PatientID && x.SurveyID == _entity.SurveyID).FirstOrDefault();

                if (entity != null)
                {
                    entity.StartDate = _entity.StartDate;
                    entity.EndDate = _entity.EndDate;
                    entity.IsActive = _entity.IsActive;
                    entity.ModifiedDate = DateTime.Now;
                    entity.IsSend = _entity.IsSend;
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    entity = new PatientSurvey();
                    entity.PatientID = _entity.PatientID;
                    entity.SurveyID = _entity.SurveyID;
                    entity.OrganizationID = _entity.OrganizationID;
                    entity.PracticeID = _entity.PracticeID;
                    entity.ProviderID = _entity.ProviderID;
                    entity.StartDate = _entity.StartDate;
                    entity.EndDate = _entity.EndDate;
                    entity.IsActive = _entity.IsActive;
                    entity.CreatedDate = DateTime.Now;
                    entity.IsSend = _entity.IsSend;
                    db.PatientSurveys.Add(entity);
                    db.SaveChanges();
                }
                return entity.ID.ToString();
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return "";
            }
        }

        #endregion UpdatePatientSurvey

        #region Delete PatientSurvey

        public void DeleteEntity_PatientSurvey(Guid Id)
        {
            try
            {
                var db = GetDataContext();
                PatientSurvey entity = db.PatientSurveys.FirstOrDefault(x => x.ID == Id);
                db.Entry(entity).State = System.Data.Entity.EntityState.Deleted;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
            }
        }

        public void DeleteExistPatientSurvey(Guid PatientId)
        {
            try
            {
                if (PatientId != null)
                {
                    var db = GetDataContext();
                    var objList = db.PatientSurveys.AsEnumerable().Where(x => x.PatientID.Equals(PatientId) && x.IsActive == true).ToList();

                    if (objList != null)
                    {
                        for (var i = 0; i < objList.Count(); i++)
                        {
                            db.Entry(objList[i]).State = System.Data.Entity.EntityState.Deleted;
                            db.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
            }
        }

        #endregion Delete SurveyCategory

        #region CheckPatientSurveyExist

        public bool CheckPatientSurveyExist(Guid? ProviderId, Guid? OrganizationId, Guid? PracticeId, Guid? PatientId, int SurveyId)
        {
            var db = GetDataContext();
            var obj = new PatientSurvey();

            obj = db.PatientSurveys.Where(x => x.ProviderID == ProviderId && x.OrganizationID == OrganizationId && x.PracticeID == PracticeId && x.PatientID == PatientId && x.SurveyID == SurveyId).FirstOrDefault();

            if (obj != null)
                return true;
            else
                return false;
        }

        public bool CheckPatientSurveyExistBySurveyID(short SurveyId)
        {
            var db = GetDataContext();
            var obj = new PatientSurvey();

            obj = db.PatientSurveys.Where(x => x.SurveyID == SurveyId).FirstOrDefault();

            if (obj == null)
                return false;
            else
                return true;

        }
        #endregion CheckPatientSurveyExist

        #region GetSurveyMonkeyEprom

        public string GetSurveyMonkeyEproms(string ApiKey, string AccessToken)
        {
            var token = "Bearer " + AccessToken;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://api.surveymonkey.net/v3/surveys?api_key=" + ApiKey);
            request.Method = "GET";
            request.ContentType = "application/json";
            request.Headers.Add("Authorization", token);
            request.Accept = "*/*";
            request.UseDefaultCredentials = true;
            WebResponse response = request.GetResponse();

            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            string result = new StreamReader(response.GetResponseStream()).ReadToEnd();
            var jsonObject = serializer.DeserializeObject(result);

            return JsonConvert.SerializeObject(jsonObject);

        }

        public string GetSurveyMonkeyCollectorIDBySurveyID(string ApiKey, string AccessToken, string surveyID)
        {
            var token = "Bearer " + AccessToken;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://api.surveymonkey.net/v3/surveys/" + surveyID + "/collectors?api_key=" + ApiKey);
            request.Method = "GET";
            request.ContentType = "application/json";
            request.Headers.Add("Authorization", token);
            request.Accept = "*/*";
            request.UseDefaultCredentials = true;
            WebResponse response = request.GetResponse();

            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            string result = new StreamReader(response.GetResponseStream()).ReadToEnd();
            var jsonObject = serializer.DeserializeObject(result);

            return JsonConvert.SerializeObject(jsonObject);

        }

        public string GetSurveyMonkey_SurveyDetails(string ApiKey, string AccessToken, string surveyID)
        {
            var token = "Bearer " + AccessToken;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://api.surveymonkey.net/v3/surveys/" + surveyID + "/details?api_key=" + ApiKey);
            request.Method = "GET";
            request.ContentType = "application/json";
            request.Headers.Add("Authorization", token);
            request.Accept = "*/*";
            request.UseDefaultCredentials = true;
            WebResponse response = request.GetResponse();

            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            string result = new StreamReader(response.GetResponseStream()).ReadToEnd();
            var jsonObject = serializer.DeserializeObject(result);

            return JsonConvert.SerializeObject(jsonObject);

        }

        public string GetSurveyMonkeyResponseBy_CollectorID(string ApiKey, string AccessToken, string CollectorID)
        {
            var token = "Bearer " + AccessToken;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://api.surveymonkey.net/v3/collectors/" + CollectorID + "/responses/bulk?api_key=" + ApiKey + "&sort_order=DESC");
            request.Method = "GET";
            request.ContentType = "application/json";
            request.Headers.Add("Authorization", token);
            request.Accept = "*/*";
            request.UseDefaultCredentials = true;
            WebResponse response = request.GetResponse();

            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            string result = new StreamReader(response.GetResponseStream()).ReadToEnd();
            var jsonObject = serializer.DeserializeObject(result);

            return JsonConvert.SerializeObject(jsonObject);

        }

        public string GetSurveyMonkeyCollectorDetails_CollectorID(string ApiKey, string AccessToken, string CollectorID)
        {
            var token = "Bearer " + AccessToken;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://api.surveymonkey.net/v3/collectors/" + CollectorID + "?api_key=" + ApiKey);
            request.Method = "GET";
            request.ContentType = "application/json";
            request.Headers.Add("Authorization", token);
            request.Accept = "*/*";
            request.UseDefaultCredentials = true;
            WebResponse response = request.GetResponse();

            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            string result = new StreamReader(response.GetResponseStream()).ReadToEnd();
            var jsonObject = serializer.DeserializeObject(result);

            return JsonConvert.SerializeObject(jsonObject);

        }

        public string GetSurveyMonkeyPagesBySurveyID(string ApiKey, string SurveyID, string AccessToken)
        {
            var token = "Bearer " + AccessToken;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://api.surveymonkey.net/v3/surveys/" + SurveyID + "/pages?api_key=" + ApiKey);
            request.Method = "GET";
            request.ContentType = "application/json";
            request.Headers.Add("Authorization", token);
            request.Accept = "*/*";
            request.UseDefaultCredentials = true;
            WebResponse response = request.GetResponse();

            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            string result = new StreamReader(response.GetResponseStream()).ReadToEnd();
            var jsonObject = serializer.DeserializeObject(result);

            return JsonConvert.SerializeObject(jsonObject);
        }

        public string GetSurveyMonkeyQuestionsListByPageID(string ApiKey, string AccessToken, string SurveyID, string PageID)
        {
            var token = "Bearer " + AccessToken;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://api.surveymonkey.net/v3/surveys/" + SurveyID + "/pages/" + PageID + "/questions?api_key=" + ApiKey);
            request.Method = "GET";
            request.ContentType = "application/json";
            request.Headers.Add("Authorization", token);
            request.Accept = "*/*";
            request.UseDefaultCredentials = true;
            WebResponse response = request.GetResponse();

            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            string result = new StreamReader(response.GetResponseStream()).ReadToEnd();
            var jsonObject = serializer.DeserializeObject(result);

            return JsonConvert.SerializeObject(jsonObject);
        }

        #endregion GetSurveyMonkeyEprom               

        #endregion PatientSurvey

        #region SecrectQuestion
        public UserSecretQuestion getUserQuestionByUserID(Guid UserID)
        {
            try
            {
                var db = GetDataContext();
                return db.UserSecretQuestions.Where(x => x.UserID == UserID).FirstOrDefault();
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }



        public bool Entity_UpdateUserSecretQuestion(UserSecretQuestion entity)
        {
            try
            {
                var db = GetDataContext();
                var ud = db.UserSecretQuestions.Where(x => x.ID.Equals(entity.ID)).FirstOrDefault();
                if (ud != null)
                {
                    ud.SecretQuestionID = entity.SecretQuestionID;
                    ud.Answer = entity.Answer;
                }

                db.Entry(ud).State = EntityState.Modified;
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return false;
            }
        }

        #endregion

        #region contact
        public bool Entity_UpdatecontactDetail(Contact entity)
        {
            try
            {
                var db = GetDataContext();
                var ct = db.Contacts.Where(x => x.ID.Equals(entity.ID)).FirstOrDefault();
                if (ct != null)
                {
                    ct.Mobile = entity.Mobile;
                    ct.Mobile2 = entity.Mobile2;
                }

                db.Entry(ct).State = EntityState.Modified;
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return false;
            }
        }
        #endregion

        #region PatientIndicators

        #region GetPatientIndicators

        public List<PatientIndicator> GetEntityById_PatientIndicators(Guid PatientId)
        {
            try
            {
                var db = GetDataContext();
                return db.PatientIndicators.Where(x => x.PatientID == PatientId && x.IsActive == true).ToList();
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        #endregion GetPatientIndicators

        #region Create PatientIndicators

        public string SaveEntity_PatientIndicators(PatientIndicator entity)
        {
            try
            {
                var db = GetDataContext();
                db.Entry(entity).State = EntityState.Added;
                db.SaveChanges();
                return "Patient Indicator Added Successfully!";
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return "";
            }
        }

        #endregion Create PatientIndicators

        #region UpdatePatientIndicators

        public string UpdateEntity_PatientIndicators(PatientIndicator _entity)
        {
            try
            {
                var db = GetDataContext();
                PatientIndicator entity = db.PatientIndicators.FirstOrDefault(x => x.ID == _entity.ID);
                if (entity != null)
                {
                    entity.PatientID = _entity.PatientID;
                    entity.IndicatorID = _entity.IndicatorID;
                    entity.StartDate = _entity.StartDate;
                    entity.EndDate = _entity.EndDate;
                    entity.Frequency = _entity.Frequency;
                    entity.Unit = _entity.Unit;
                    entity.Goal = _entity.Goal;
                    entity.Comments = _entity.Comments;
                    entity.IsActive = _entity.IsActive;
                    entity.Status = _entity.Status;
                    entity.ModifiedDate = Common.Utilities.ConvertServerCurrentDateTimeToClientDateTime(DateTime.Now);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();

                    return "Patient Indicators Updated Successfully!";
                }
                return "";
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return "";
            }
        }

        #endregion UpdatePatientIndicators

        #region Delete PatientIndicators

        public void DeleteEntity_PatientIndicators(int Id)
        {
            try
            {
                var db = GetDataContext();
                PatientIndicator entity = db.PatientIndicators.FirstOrDefault(x => x.ID == Id);
                db.Entry(entity).State = System.Data.Entity.EntityState.Deleted;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
            }
        }

        #endregion Delete PatientIndicators

        #region CheckPatientIndicatorExist

        public bool CheckPatientIndicatorExist(Guid PatientId, short IndicatorID)
        {
            var db = GetDataContext();
            var obj = db.PatientIndicators.Where(x => x.PatientID == PatientId && x.IndicatorID == IndicatorID).FirstOrDefault();
            if (obj == null)
                return false;
            else
                return true;

        }

        #endregion CheckPatientIndicatorExist
        #endregion PatientIndicators

        #region PatientSuggestion

        #region GetPatientSuggestion

        public PatientSuggestion GetEntityByPatientSurveyId_PatientSuggestion(Guid PatientSurveyId)
        {
            try
            {
                var db = GetDataContext();
                return db.PatientSuggestions.Where(x => x.PatientSurveyID == PatientSurveyId && x.IsActive == true).FirstOrDefault();
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        #endregion GetPatientSuggestion

        #region Create PatientSuggestion

        public string SaveEntity_PatientSuggestion(PatientSuggestion entity)
        {
            try
            {
                var db = GetDataContext();
                db.Entry(entity).State = System.Data.Entity.EntityState.Added;
                db.SaveChanges();
                return "1";
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return "";
            }
        }

        #endregion Create PatientSuggestion

        #region UpdatePatientSuggestion

        public string UpdateEntity_PatientSuggestion(PatientSuggestion _entity)
        {
            try
            {
                var db = GetDataContext();
                PatientSuggestion entity = db.PatientSuggestions.FirstOrDefault(x => x.ID == _entity.ID);
                if (entity != null)
                {
                    entity.PatientSurveyID = _entity.PatientSurveyID;
                    entity.Suggestions = _entity.Suggestions;
                    entity.IsActive = _entity.IsActive;
                    entity.ModifiedDate = Common.Utilities.ConvertServerCurrentDateTimeToClientDateTime(DateTime.Now);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();

                    return "1";
                }
                return "0";
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return "";
            }
        }

        #endregion UpdatePatientSuggestion

        #region CheckPatientSuggestionExist

        public bool CheckPatientSuggestionExist(Guid PatientSurveyId)
        {
            var db = GetDataContext();
            var obj = db.PatientSuggestions.Where(x => x.PatientSurveyID == PatientSurveyId).FirstOrDefault();
            if (obj == null)
                return false;
            else
                return true;

        }

        #endregion CheckPatientSuggestionExist

        #endregion PatientSuggestion

        #region Organizations

        #region GetOrganizationList  

        public List<Organization> GetOrganizationList()
        {
            try
            {
                var db = GetDataContext();
                return db.Organizations.Where(x => x.IsActive == true).ToList();
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        #endregion GetOrganizationList

        #region GetOrganizationsByUserID  

        public Organization GetOrganizationsByUserID(Guid? UserID)
        {
            try
            {
                var db = GetDataContext();
                return db.Organizations.Where(x => x.UserId == UserID && x.IsActive == true).FirstOrDefault();
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        #endregion GetOrganizationsByUserID

        #region CreateOrganizations 

        public bool Entity_CreateOrganizations(Organization entity)
        {
            try
            {
                var db = GetDataContext();
                db.Entry(entity).State = System.Data.Entity.EntityState.Added;
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return false;
            }
        }

        #endregion CreateOrganizations

        #region UpdateOrganizations 

        public bool Entity_UpdateOrganizations(Organization entity)
        {
            try
            {
                var db = GetDataContext();
                Organization _entity = db.Organizations.FirstOrDefault(x => x.ID == entity.ID);
                if (_entity != null)
                {
                    _entity.ID = entity.ID;
                    _entity.OrganizationName = entity.OrganizationName;
                    _entity.SalutationID = entity.SalutationID;
                    _entity.IsActive = entity.IsActive;
                    _entity.ModifiedBy = entity.UserId;
                    _entity.ModifiedDate = Common.Utilities.ConvertServerCurrentDateTimeToClientDateTime(DateTime.Now);
                    db.Entry(_entity).State = EntityState.Modified;
                    db.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return false;
            }
        }

        #endregion UpdateOrganizations

        #region GetOrganizationsByID  

        public Organization GetOrganizationsByID(Guid ID)
        {
            try
            {
                var db = GetDataContext();
                return db.Organizations.Where(x => x.ID == ID && x.IsActive == true).FirstOrDefault();
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        #endregion GetOrganizationsByID

        #endregion Organizations

        #region ProviderOrganization

        #region GetProviderOrganizations

        public List<ProviderOrganization> GetEntityByProviderID_ProviderOrganizations(Guid ProviderId)
        {
            try
            {
                var db = GetDataContext();
                return db.ProviderOrganizations.Where(x => x.ProviderID == ProviderId && x.IsActive == true).ToList();
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        #endregion GetProviderOrganizations

        #region Create ProviderOrganization

        public string SaveEntity_ProviderOrganization(ProviderOrganization entity)
        {
            try
            {
                var db = GetDataContext();
                db.Entry(entity).State = System.Data.Entity.EntityState.Added;
                db.SaveChanges();
                return "Organization Added Successfully!";
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return "";
            }
        }

        #endregion Create ProviderOrganization

        #region Update ProviderOrganization

        public string UpdateEntity_ProviderOrganization(ProviderOrganization _entity)
        {
            try
            {
                var db = GetDataContext();
                ProviderOrganization entity = db.ProviderOrganizations.FirstOrDefault(x => x.OrganizationID == _entity.OrganizationID && x.ProviderID == _entity.ProviderID);
                if (entity != null)
                {
                    entity.OrganizationID = _entity.OrganizationID;
                    entity.ProviderID = _entity.ProviderID;
                    entity.Designation = _entity.Designation;
                    entity.Description = _entity.Description;
                    entity.StartDate = _entity.StartDate;
                    entity.EndDate = _entity.EndDate;
                    entity.IsActive = _entity.IsActive;
                    entity.ModifiedDate = Common.Utilities.ConvertServerCurrentDateTimeToClientDateTime(DateTime.Now);
                    db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    return "Organization Updated Successfully!";
                }
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
            }
            return "";
        }

        #endregion Update ProviderOrganization

        #region Delete ProviderOrganization

        public void DeleteEntity_ProviderOrganization(Guid Id)
        {
            try
            {
                var db = GetDataContext();
                List<ProviderPracticeRole> RoleList = new List<ProviderPracticeRole>();
                List<ProviderPractice> ProviderPracticeEntity = db.ProviderPractices.Where(x => x.ProviderOrganizationId == Id).ToList();
                if (ProviderPracticeEntity != null)
                {
                    var ProviderPracticeIds = ProviderPracticeEntity.Select(x => x.ID).ToList();
                    if (ProviderPracticeIds != null)
                    {
                        for (int i = 0; i < ProviderPracticeIds.Count; i++)
                        {
                            Guid ProviderPracticeId = ProviderPracticeIds[i];
                            ProviderPracticeRole RoleEntity = db.ProviderPracticeRoles.FirstOrDefault(x => x.ProviderPracticeID == ProviderPracticeId);

                            if (RoleEntity != null)
                            {
                                db.Entry(RoleEntity).State = System.Data.Entity.EntityState.Deleted;
                                db.SaveChanges();
                            }
                        }

                        for (int i = 0; i < ProviderPracticeEntity.Count; i++)
                        {
                            Guid ProviderPracticeId = ProviderPracticeEntity[i].ID;
                            ProviderPractice PracticeEntity = db.ProviderPractices.FirstOrDefault(x => x.ID == ProviderPracticeId);
                            if (PracticeEntity != null)
                            {
                                db.Entry(PracticeEntity).State = System.Data.Entity.EntityState.Deleted;
                                db.SaveChanges();
                            }
                        }
                    }
                }
                ProviderOrganization entity = db.ProviderOrganizations.FirstOrDefault(x => x.ID == Id);
                if (entity != null)
                {
                    db.Entry(entity).State = System.Data.Entity.EntityState.Deleted;
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
            }
        }

        #endregion Delete ProviderOrganization

        #region CheckProviderOrganizations

        public bool CheckExists_ProviderOrganizations(Guid OrganizationID, Guid ProviderId)
        {
            try
            {
                bool isExists = false;
                var db = GetDataContext();
                var obj = db.ProviderOrganizations.Where(x => x.OrganizationID == OrganizationID && x.ProviderID == ProviderId && x.IsActive == true).FirstOrDefault();

                if (obj != null)
                    isExists = true;
                else
                    isExists = false;

                return isExists;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return false;
            }
        }

        public bool CheckExists_PatientProviderOrganizations(Guid OrganizationID, Guid PracticeID)
        {
            try
            {
                bool isExists = false;
                var db = GetDataContext();
                var obj = new PatientProvider();

                if (OrganizationID != Guid.Empty)
                {
                    obj = db.PatientProviders.Where(x => x.OrganizationID == OrganizationID).FirstOrDefault();
                }
                else if (PracticeID != Guid.Empty)
                {
                    obj = db.PatientProviders.Where(x => x.PracticeID == PracticeID).FirstOrDefault();
                }

                if (obj != null)
                    isExists = true;
                else
                    isExists = false;

                return isExists;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return false;
            }
        }

        #endregion GetProviderOrganizations


        #region GetProviderOrganizationsByProviderId

        public List<Organization> GetProviderOrganizationsByProviderId(Guid ProviderId)
        {
            try
            {
                var db = GetDataContext();
                List<Organization> OrgList = (from a in db.ProviderOrganizations
                                              join b in db.Organizations on a.OrganizationID equals b.ID
                                              where a.ProviderID == ProviderId && b.IsActive == true
                                              select b).ToList();
                return OrgList;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        #endregion GetProviderOrganizationsByProviderId

        #endregion ProviderOrganization    

        #region PatientSurveyStatus     

        #region Create_PatientSurveyStatus  

        public Guid Create_PatientSurveyStatus(PatientSurveyStatu entity)
        {
            try
            {
                var db = GetDataContext();
                db.Entry(entity).State = EntityState.Added;
                db.SaveChanges();
                return entity.ID;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return new Guid();
            }
        }

        #endregion Create_PatientSurveyStatus

        #region Update_PatientSurveyStatus  

        public string Update_PatientSurveyStatus(PatientSurveyStatu _entity)
        {
            try
            {
                var db = GetDataContext();
                PatientSurveyStatu entity = db.PatientSurveyStatus.FirstOrDefault(x => x.ID == _entity.ID);
                if (entity != null)
                {
                    entity.Status = _entity.Status;
                    entity.Score = _entity.Score;

                    db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    //Pathway functionality related work.
                    //So update the PatientSurveyStatusID in table "PatientSurvey_Pathway_PatientSurveyStatus"

                    PatientSurvey_Pathway_PatientSurveyStatus objEntity = db.PatientSurvey_Pathway_PatientSurveyStatus.Where(x => x.PatientSurveyID == entity.PatientSurveyID && x.PathwayID != null && x.PatientSurveyStatusID == null).FirstOrDefault();

                    Guid guidOutput_ID;

                    if (objEntity != null)
                    {
                        bool isValid = Guid.TryParse(objEntity.ID.ToString(), out guidOutput_ID);

                        if (isValid && guidOutput_ID != null && guidOutput_ID != new Guid("00000000-0000-0000-0000-000000000000"))
                        {
                            // so now update the PatientSurveyStatusID in table "PatientSurvey_Pathway_PatientSurveyStatus"

                            objEntity.PatientSurveyStatusID = entity.ID;
                            db.Entry(objEntity).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }
                    }

                    return "1";
                }

                return "0";
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return "0";
            }
        }

        #endregion Update_PatientSurveyStatus

        #region GetPatientSurveyStatus

        public List<PatientSurveyStatu> GetPatientSurveyStatus(Guid PatientId, Guid OrganizationId, Guid PracticeId, Guid ProviderId, int SurveyId, DateTime? FromDate, DateTime? ToDate, string PathwayID)
        {
            try
            {
                var db = GetDataContext();
                DateTime? TodayDate = Common.Utilities.ConvertServerCurrentDateTimeToClientDateTime(DateTime.Now);

                Guid PathwayIDGuid = string.IsNullOrEmpty(PathwayID) || PathwayID == "undefined" || PathwayID == "-1" ? Guid.Empty : new Guid(PathwayID);

                if (PatientId != Guid.Empty && FromDate != null && ToDate != null)
                {
                    if (PathwayIDGuid != Guid.Empty)
                    {
                        //return new List<PatientSurveyStatu>();
                        //return (from a in db.PatientSurveys
                        //        join b in db.PatientSurveyStatus on new {a.ID} equals new { ID = (Guid)b.PatientSurveyID } into b_join
                        //        from b in b_join.DefaultIfEmpty()
                        //        where
                        //              (from c in db.PatientSurvey_Pathway_PatientSurveyStatus
                        //               where
                        //                     a.ID == c.PatientSurveyID &&
                        //                     b.ID == c.PatientSurveyStatusID &&
                        //                     c.PathwayID != null &&
                        //                     c.PatientSurveyStatusID != null
                        //               select new
                        //               {
                        //                   c
                        //               }).Single() != null &&
                        //          b.Status.ToLower() == "completed".ToLower()
                        //          && a.OrganizationID == OrganizationId
                        //        && a.PracticeID == PracticeId
                        //        && a.ProviderID == ProviderId
                        //        && a.SurveyID == SurveyId
                        //        && DbFunctions.TruncateTime(b.CreatedDate) >= DbFunctions.TruncateTime(FromDate)
                        //        && DbFunctions.TruncateTime(b.CreatedDate) <= DbFunctions.TruncateTime(ToDate)
                        //        orderby b.CreatedDate
                        //        select b).ToList();


                        return (from a in db.PatientSurveys
                                join b in db.PatientSurveyStatus on new { ID = a.ID } equals new { ID = (Guid)b.PatientSurveyID } into b_join
                                from b in b_join.DefaultIfEmpty()
                                join c in db.PatientSurvey_Pathway_PatientSurveyStatus
                                      on new { a.ID, Column1 = b.ID }
                                  equals new { ID = c.PatientSurveyID, Column1 = (Guid)c.PatientSurveyStatusID } into c_join
                                from c in c_join.DefaultIfEmpty()
                                where a.PatientID == PatientId
                                && a.OrganizationID == OrganizationId
                                && a.PracticeID == PracticeId
                                && a.ProviderID == ProviderId
                                && a.SurveyID == SurveyId
                                && b.Status.ToLower() == "completed".ToLower()
                                && c.PathwayID != null
                                && c.PathwayID == PathwayIDGuid
                                && DbFunctions.TruncateTime(b.CreatedDate) >= DbFunctions.TruncateTime(FromDate)
                                && DbFunctions.TruncateTime(b.CreatedDate) <= DbFunctions.TruncateTime(ToDate)
                                && c.PatientSurveyStatusID != null
                                orderby b.CreatedDate
                                select b).ToList();
                    }
                    else
                    {
                        return (from a in db.PatientSurveys
                                join b in db.PatientSurveyStatus on a.ID equals b.PatientSurveyID
                                where a.PatientID == PatientId
                                && a.OrganizationID == OrganizationId
                                && a.PracticeID == PracticeId
                                && a.ProviderID == ProviderId
                                && a.SurveyID == SurveyId
                                && DbFunctions.TruncateTime(b.CreatedDate) >= DbFunctions.TruncateTime(FromDate)
                                && DbFunctions.TruncateTime(b.CreatedDate) <= DbFunctions.TruncateTime(ToDate)
                                && b.Status.ToLower() == "completed"
                                orderby b.CreatedDate
                                select b).ToList();
                    }
                }
                else if (PatientId != Guid.Empty && FromDate != null && ToDate == null)
                {
                    if (PathwayIDGuid != Guid.Empty)
                    {
                        return (from a in db.PatientSurveys
                                join b in db.PatientSurveyStatus on new { ID = a.ID } equals new { ID = (Guid)b.PatientSurveyID } into b_join
                                from b in b_join.DefaultIfEmpty()
                                join c in db.PatientSurvey_Pathway_PatientSurveyStatus
                                      on new { a.ID, Column1 = b.ID }
                                  equals new { ID = c.PatientSurveyID, Column1 = (Guid)c.PatientSurveyStatusID } into c_join
                                from c in c_join.DefaultIfEmpty()
                                where a.PatientID == PatientId
                                && a.OrganizationID == OrganizationId
                                && a.PracticeID == PracticeId
                                && a.ProviderID == ProviderId
                                && a.SurveyID == SurveyId
                                && DbFunctions.TruncateTime(b.CreatedDate) >= DbFunctions.TruncateTime(FromDate)
                                && b.Status.ToLower() == "completed".ToLower()
                                && c.PathwayID != null
                                && c.PathwayID == PathwayIDGuid
                                && c.PatientSurveyStatusID != null
                                orderby b.CreatedDate
                                select b).ToList();
                    }
                    else
                    {
                        return (from a in db.PatientSurveys
                                join b in db.PatientSurveyStatus on a.ID equals b.PatientSurveyID
                                where a.PatientID == PatientId
                                && a.OrganizationID == OrganizationId
                                && a.PracticeID == PracticeId
                                && a.ProviderID == ProviderId
                                && a.SurveyID == SurveyId
                                && DbFunctions.TruncateTime(b.CreatedDate) >= DbFunctions.TruncateTime(FromDate)
                                && DbFunctions.TruncateTime(b.CreatedDate) <= DbFunctions.TruncateTime(TodayDate)
                                && b.Status.ToLower() == "completed"
                                orderby b.CreatedDate
                                select b).ToList();
                    }
                }
                else if (PatientId != Guid.Empty && FromDate == null && ToDate != null)
                {
                    if (PathwayIDGuid != Guid.Empty)
                    {
                        return (from a in db.PatientSurveys
                                join b in db.PatientSurveyStatus on new { ID = a.ID } equals new { ID = (Guid)b.PatientSurveyID } into b_join
                                from b in b_join.DefaultIfEmpty()
                                join c in db.PatientSurvey_Pathway_PatientSurveyStatus
                                      on new { a.ID, Column1 = b.ID }
                                  equals new { ID = c.PatientSurveyID, Column1 = (Guid)c.PatientSurveyStatusID } into c_join
                                from c in c_join.DefaultIfEmpty()
                                where a.PatientID == PatientId
                                && a.OrganizationID == OrganizationId
                                && a.PracticeID == PracticeId
                                && a.ProviderID == ProviderId
                                && a.SurveyID == SurveyId
                                && DbFunctions.TruncateTime(b.CreatedDate) <= DbFunctions.TruncateTime(ToDate)
                                && b.Status.ToLower() == "completed".ToLower()
                                && c.PathwayID != null
                                && c.PathwayID == PathwayIDGuid
                                && c.PatientSurveyStatusID != null
                                orderby b.CreatedDate
                                select b).ToList();
                    }
                    else
                    {
                        return (from a in db.PatientSurveys
                                join b in db.PatientSurveyStatus on a.ID equals b.PatientSurveyID
                                where a.PatientID == PatientId
                                && a.OrganizationID == OrganizationId
                                && a.PracticeID == PracticeId
                                && a.ProviderID == ProviderId
                                && a.SurveyID == SurveyId
                                && DbFunctions.TruncateTime(b.CreatedDate) <= DbFunctions.TruncateTime(ToDate)
                                && b.Status.ToLower() == "completed"
                                orderby b.CreatedDate
                                select b).ToList();
                    }
                }
                else if (PatientId != Guid.Empty && FromDate == null && ToDate == null)
                {
                    if (PathwayIDGuid != Guid.Empty)
                    {
                        return (from a in db.PatientSurveys
                                join b in db.PatientSurveyStatus on new { ID = a.ID } equals new { ID = (Guid)b.PatientSurveyID } into b_join
                                from b in b_join.DefaultIfEmpty()
                                join c in db.PatientSurvey_Pathway_PatientSurveyStatus
                                      on new { a.ID, Column1 = b.ID }
                                  equals new { ID = c.PatientSurveyID, Column1 = (Guid)c.PatientSurveyStatusID } into c_join
                                from c in c_join.DefaultIfEmpty()
                                where a.PatientID == PatientId
                                && a.OrganizationID == OrganizationId
                                && a.PracticeID == PracticeId
                                && a.ProviderID == ProviderId
                                && a.SurveyID == SurveyId
                                && b.Status.ToLower() == "completed".ToLower()
                                && c.PathwayID != null
                                && c.PathwayID == PathwayIDGuid
                                && c.PatientSurveyStatusID != null
                                orderby b.CreatedDate
                                select b).ToList();
                    }
                    else
                    {
                        return (from a in db.PatientSurveys
                                join b in db.PatientSurveyStatus on a.ID equals b.PatientSurveyID
                                where a.PatientID == PatientId
                                && a.OrganizationID == OrganizationId
                                && a.PracticeID == PracticeId
                                && a.ProviderID == ProviderId
                                && a.SurveyID == SurveyId
                                && b.Status.ToLower() == "completed"
                                orderby b.CreatedDate
                                select b).ToList();
                    }
                }
                else if (PatientId == Guid.Empty && FromDate != null && ToDate != null)
                {
                    if (PathwayIDGuid != Guid.Empty)
                    {
                        return (from a in db.PatientSurveys
                                join b in db.PatientSurveyStatus on new { ID = a.ID } equals new { ID = (Guid)b.PatientSurveyID } into b_join
                                from b in b_join.DefaultIfEmpty()
                                join c in db.PatientSurvey_Pathway_PatientSurveyStatus
                                      on new { a.ID, Column1 = b.ID }
                                  equals new { ID = c.PatientSurveyID, Column1 = (Guid)c.PatientSurveyStatusID } into c_join
                                from c in c_join.DefaultIfEmpty()
                                where a.OrganizationID == OrganizationId
                                && a.PracticeID == PracticeId
                                && a.ProviderID == ProviderId
                                && a.SurveyID == SurveyId
                                && DbFunctions.TruncateTime(b.CreatedDate) >= DbFunctions.TruncateTime(FromDate)
                                && DbFunctions.TruncateTime(b.CreatedDate) <= DbFunctions.TruncateTime(ToDate)
                                && b.Status.ToLower() == "completed".ToLower()
                                && c.PathwayID != null
                                && c.PathwayID == PathwayIDGuid
                                && c.PatientSurveyStatusID != null
                                orderby b.CreatedDate
                                select b).ToList();
                    }
                    else
                    {
                        return (from a in db.PatientSurveys
                                join b in db.PatientSurveyStatus on a.ID equals b.PatientSurveyID
                                where a.OrganizationID == OrganizationId
                                && a.PracticeID == PracticeId
                                && a.ProviderID == ProviderId
                                && a.SurveyID == SurveyId
                                && DbFunctions.TruncateTime(b.CreatedDate) >= DbFunctions.TruncateTime(FromDate)
                                && DbFunctions.TruncateTime(b.CreatedDate) <= DbFunctions.TruncateTime(ToDate)
                                && b.Status.ToLower() == "completed"
                                orderby b.CreatedDate
                                select b).ToList();
                    }
                }
                else if (PatientId == Guid.Empty && FromDate != null && ToDate == null)
                {
                    if (PathwayIDGuid != Guid.Empty)
                    {
                        return (from a in db.PatientSurveys
                                join b in db.PatientSurveyStatus on new { ID = a.ID } equals new { ID = (Guid)b.PatientSurveyID } into b_join
                                from b in b_join.DefaultIfEmpty()
                                join c in db.PatientSurvey_Pathway_PatientSurveyStatus
                                      on new { a.ID, Column1 = b.ID }
                                  equals new { ID = c.PatientSurveyID, Column1 = (Guid)c.PatientSurveyStatusID } into c_join
                                from c in c_join.DefaultIfEmpty()
                                where a.OrganizationID == OrganizationId
                                && a.PracticeID == PracticeId
                                && a.ProviderID == ProviderId
                                && a.SurveyID == SurveyId
                                && DbFunctions.TruncateTime(b.CreatedDate) >= DbFunctions.TruncateTime(FromDate)
                                && b.Status.ToLower() == "completed".ToLower()
                                && c.PathwayID != null
                                && c.PathwayID == PathwayIDGuid
                                && c.PatientSurveyStatusID != null
                                orderby b.CreatedDate
                                select b).ToList();
                    }
                    else
                    {
                        return (from a in db.PatientSurveys
                                join b in db.PatientSurveyStatus on a.ID equals b.PatientSurveyID
                                where a.OrganizationID == OrganizationId
                                && a.PracticeID == PracticeId
                                && a.ProviderID == ProviderId
                                && a.SurveyID == SurveyId
                                && DbFunctions.TruncateTime(b.CreatedDate) >= DbFunctions.TruncateTime(FromDate)
                                && DbFunctions.TruncateTime(b.CreatedDate) <= DbFunctions.TruncateTime(TodayDate)
                                && b.Status.ToLower() == "completed"
                                orderby b.CreatedDate
                                select b).ToList();
                    }
                }
                else if (PatientId == Guid.Empty && FromDate == null && ToDate != null)
                {
                    if (PathwayIDGuid != Guid.Empty)
                    {
                        return (from a in db.PatientSurveys
                                join b in db.PatientSurveyStatus on new { ID = a.ID } equals new { ID = (Guid)b.PatientSurveyID } into b_join
                                from b in b_join.DefaultIfEmpty()
                                join c in db.PatientSurvey_Pathway_PatientSurveyStatus
                                      on new { a.ID, Column1 = b.ID }
                                  equals new { ID = c.PatientSurveyID, Column1 = (Guid)c.PatientSurveyStatusID } into c_join
                                from c in c_join.DefaultIfEmpty()
                                where a.OrganizationID == OrganizationId
                                && a.PracticeID == PracticeId
                                && a.ProviderID == ProviderId
                                && a.SurveyID == SurveyId
                                && DbFunctions.TruncateTime(b.CreatedDate) <= DbFunctions.TruncateTime(ToDate)
                                && b.Status.ToLower() == "completed".ToLower()
                                && c.PathwayID != null
                                && c.PathwayID == PathwayIDGuid
                                && c.PatientSurveyStatusID != null
                                orderby b.CreatedDate
                                select b).ToList();
                    }
                    else
                    {
                        return (from a in db.PatientSurveys
                                join b in db.PatientSurveyStatus on a.ID equals b.PatientSurveyID
                                where a.OrganizationID == OrganizationId
                                && a.PracticeID == PracticeId
                                && a.ProviderID == ProviderId
                                && a.SurveyID == SurveyId
                                && DbFunctions.TruncateTime(b.CreatedDate) <= DbFunctions.TruncateTime(ToDate)
                                && b.Status.ToLower() == "completed"
                                orderby b.CreatedDate
                                select b).ToList();
                    }


                }
                else if (PatientId == Guid.Empty)
                {
                    if (PathwayIDGuid != Guid.Empty)
                    {
                        return (from a in db.PatientSurveys
                                join b in db.PatientSurveyStatus on new { ID = a.ID } equals new { ID = (Guid)b.PatientSurveyID } into b_join
                                from b in b_join.DefaultIfEmpty()
                                join c in db.PatientSurvey_Pathway_PatientSurveyStatus
                                      on new { a.ID, Column1 = b.ID }
                                  equals new { ID = c.PatientSurveyID, Column1 = (Guid)c.PatientSurveyStatusID } into c_join
                                from c in c_join.DefaultIfEmpty()
                                where a.OrganizationID == OrganizationId
                                && a.PracticeID == PracticeId
                                && a.ProviderID == ProviderId
                                && a.SurveyID == SurveyId
                                && b.Status.ToLower() == "completed".ToLower()
                                && c.PathwayID != null
                                && c.PathwayID == PathwayIDGuid
                                && c.PatientSurveyStatusID != null
                                orderby b.CreatedDate
                                select b).ToList();
                    }
                    else
                    {
                        return (from a in db.PatientSurveys
                                join b in db.PatientSurveyStatus on a.ID equals b.PatientSurveyID
                                where a.OrganizationID == OrganizationId
                                && a.PracticeID == PracticeId
                                && a.ProviderID == ProviderId
                                && a.SurveyID == SurveyId
                                && b.Status.ToLower() == "completed"
                                orderby b.CreatedDate
                                select b).ToList();
                    }
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }

        }
        #region PathwayFunctionality
        public int Check_PatientSurvey_Pathway_PatientSurveyStatus(Guid? providerID, Guid? organizationID, Guid? practiceID, Guid? patientID, int surveyID, Guid PatientSurveyID, Guid? PathwayID)
        {
            var db = GetDataContext();

            int count = db.PatientSurvey_Pathway_PatientSurveyStatus.Where(x => x.PatientSurveyID == PatientSurveyID).Count();

            return count;
        }

        public Guid Check_PatientSurvey_Pathway_PatientSurveyStatus_WithOut_Status(Guid? providerID, Guid? organizationID, Guid? practiceID, Guid? patientID, int surveyID, Guid PatientSurveyID, Guid? PathwayID)
        {
            var db = GetDataContext();

            PatientSurvey_Pathway_PatientSurveyStatus obj = db.PatientSurvey_Pathway_PatientSurveyStatus.Where(x => x.PatientSurveyID == PatientSurveyID && x.PatientSurveyStatusID == null).OrderByDescending(x => x.CreatedOn).FirstOrDefault();

            if (obj != null)
            {
                return obj.ID;
            }
            else
            {
                return Guid.Empty;
            }
        }
        public Guid SaveEntity_PatientSurvey_Pathway_PatientSurveyStatus(PatientSurvey_Pathway_PatientSurveyStatus entity)
        {
            try
            {
                var db = GetDataContext();
                db.Entry(entity).State = System.Data.Entity.EntityState.Added;
                db.SaveChanges();
                return entity.ID;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return new Guid();
            }
        }

        public string UpdateEntity_PatientSurvey_Pathway_PatientSurveyStatus(PatientSurvey_Pathway_PatientSurveyStatus _entity)
        {
            try
            {
                var db = GetDataContext();
                PatientSurvey_Pathway_PatientSurveyStatus entity = db.PatientSurvey_Pathway_PatientSurveyStatus.FirstOrDefault(x => x.ID == _entity.ID);
                if (entity != null)
                {
                    if (entity.PathwayID != _entity.PathwayID)
                    {
                        entity.PathwayID = _entity.PathwayID;
                    }
                    db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    return "Eproms Updated Successfully!";
                }
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
            }
            return "";
        }
        #endregion


        public List<PatientSurveyStatu> GetPatientSurveyStatusData(Guid? PatientSurveyID, DateTime? FromDate)
        {
            try
            {
                var db = GetDataContext();

                DateTime? TodayDate = Common.Utilities.ConvertServerCurrentDateTimeToClientDateTime(DateTime.Now);

                return db.PatientSurveyStatus.Where(x => x.PatientSurveyID == PatientSurveyID && DbFunctions.TruncateTime(x.CreatedDate) >= DbFunctions.TruncateTime(FromDate) && DbFunctions.TruncateTime(x.CreatedDate) <= DbFunctions.TruncateTime(TodayDate) && x.Status.ToLower() == "completed").OrderBy(x => x.CreatedDate).ToList();
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }

        }

        public PatientSurveyStatu GetPatientSurveyStatus(Guid? PatientSurveyID)
        {
            try
            {
                var db = GetDataContext();
                return db.PatientSurveyStatus.Where(x => x.PatientSurveyID == PatientSurveyID).OrderByDescending(x => x.CreatedDate).FirstOrDefault();
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }

        }

        public PatientSurveyStatu GetPatientSurveyStatusById(Guid? ID)
        {
            try
            {
                var db = GetDataContext();
                return db.PatientSurveyStatus.Where(x => x.ID == ID).OrderBy(x => x.Status).FirstOrDefault();
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }

        }

        public List<PatientSurveyStatu> GetEntity_PatientSurveyStatus(Guid PatientId, Guid ProviderId)
        {
            try
            {
                var db = GetDataContext();

                if (ProviderId != Guid.Empty)
                {
                    var objList = (from a in db.PatientSurveys
                                   join b in db.PatientSurveyStatus on a.ID equals b.PatientSurveyID
                                   where a.PatientID == PatientId
                                   && a.ProviderID == ProviderId
                                   && b.Status.Equals("completed", StringComparison.OrdinalIgnoreCase)
                                   orderby a.Survey.ExternalTitle
                                   select b).ToList();

                    return objList;
                }
                else
                {
                    var objList = (from a in db.PatientSurveys
                                   join b in db.PatientSurveyStatus on a.ID equals b.PatientSurveyID
                                   where a.PatientID == PatientId
                                   && b.Status.Equals("completed", StringComparison.OrdinalIgnoreCase)
                                   orderby a.Survey.ExternalTitle
                                   select b).ToList();


                    return objList;
                }
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }


        #endregion GetPatientSurveyStatus

        #region CheckTodayPatientSurveyStatusExist

        public bool CheckTodayDate_PatientSurveyStatusExist(Guid PatientSurveyID)
        {
            try
            {
                var db = GetDataContext();

                bool isExists = false;
                DateTime? TodayDate = Common.Utilities.ConvertServerCurrentDateTimeToClientDateTime(DateTime.Now);

                var objPatientStats = db.PatientSurveyStatus.Where(x => x.PatientSurveyID == PatientSurveyID && DbFunctions.TruncateTime(x.CreatedDate) == DbFunctions.TruncateTime(TodayDate) && x.Status.Equals("completed", StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

                if (objPatientStats != null)
                {
                    isExists = true;
                }
                else
                {
                    isExists = false;
                }
                return isExists;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return false;
            }
        }

        #endregion CheckTodayPatientSurveyStatusExist

        #region CheckPatientSurveyExist

        public bool CheckPatientSurveyExist(Guid PatientSurveyID)
        {
            try
            {
                var db = GetDataContext();

                bool isExists = false;

                var objPatientStats = db.PatientSurveyStatus.AsEnumerable().Where(x => x.PatientSurveyID == PatientSurveyID).FirstOrDefault();

                if (objPatientStats != null)
                {
                    isExists = true;
                }
                else
                {
                    isExists = false;
                }
                return isExists;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return false;
            }
        }

        #endregion CheckPatientSurveyExist

        #endregion PatientSurveyStatus

        #region Practice

        #region CreatePractice 

        public bool Entity_CreatePractice(Practice entity)
        {
            try
            {
                var db = GetDataContext();
                db.Entry(entity).State = System.Data.Entity.EntityState.Added;
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return false;
            }
        }

        #endregion CreatePractice 

        #region UpdatePractice 

        public bool Entity_UpdatePractice(Practice entity)
        {
            try
            {
                var db = GetDataContext();
                Practice _entity = db.Practices.FirstOrDefault(x => x.ID == entity.ID);
                if (_entity != null)
                {
                    _entity.ID = entity.ID;
                    _entity.PracticeName = entity.PracticeName;
                    _entity.SalutationID = entity.SalutationID;
                    //  _entity.OrganizationId = entity.OrganizationId;                    
                    _entity.IsActive = entity.IsActive;
                    _entity.ModifiedBy = entity.UserId;
                    _entity.ModifiedDate = Common.Utilities.ConvertServerCurrentDateTimeToClientDateTime(DateTime.Now);
                    db.Entry(_entity).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return false;
            }
        }

        #endregion UpdatePractice

        #region GetPracticeByUserID  

        public Practice GetPracticeByUserID(Guid UserID)
        {
            try
            {
                var db = GetDataContext();
                return db.Practices.Where(x => x.UserId == UserID && x.IsActive == true).FirstOrDefault();
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        #endregion GetPracticeByUserID

        #region GetPracticeByID  

        public Practice GetPracticeByID(Guid PracticeID)
        {
            try
            {
                var db = GetDataContext();
                return db.Practices.Where(x => x.ID == PracticeID && x.IsActive == true).FirstOrDefault();
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        #endregion GetPracticeByID

        #region GetPracticeList  

        public List<Practice> GetPracticeList(Guid? OrgID)
        {
            try
            {
                var db = GetDataContext();
                return db.Practices.Where(x => x.IsActive == true && x.OrganizationId == OrgID).ToList();
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        public List<Practice> GetRemainingPracticeList(Guid? OrgID, Guid? ProviderID)
        {
            try
            {
                var db = GetDataContext();

                var list = db.ProviderPractices.Where(x => x.ProviderId == ProviderID).Select(x => x.PracticeId).ToList();
                if (list != null && list.Count > 0)
                {
                    return db.Practices.Where(x => x.OrganizationId == OrgID && x.IsActive == true && (!list.Contains(x.ID))).ToList();
                }
                else
                {
                    return db.Practices.Where(x => x.OrganizationId == OrgID && x.IsActive == true).ToList();
                }
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }
        #endregion GetPracticeList        

        #endregion Practice

        #region PracticeRole

        #region CreatePracticeRole

        public bool Entity_CreatePracticeRole(PracticeRole entity)
        {
            try
            {
                var db = GetDataContext();
                db.Entry(entity).State = System.Data.Entity.EntityState.Added;
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return false;
            }
        }

        #endregion CreatePracticeRole 

        #region UpdatePracticeRole 

        public bool Entity_UpdatePracticeRole(PracticeRole entity)
        {
            try
            {
                var db = GetDataContext();
                PracticeRole _entity = db.PracticeRoles.FirstOrDefault(x => x.RoleId == entity.RoleId);
                if (_entity != null)
                {
                    _entity.RoleId = entity.RoleId;
                    _entity.RoleName = entity.RoleName;
                    _entity.PracticeId = entity.PracticeId;
                    _entity.OrganizationId = entity.OrganizationId;
                    _entity.IsActive = entity.IsActive;
                    db.Entry(_entity).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return false;
            }
        }

        #endregion UpdatePracticeRole

        #region DeletePracticeRole

        public bool DeleteEntity_PracticeRole(int RoleId)
        {
            try
            {
                var db = GetDataContext();
                PracticeRole entity = db.PracticeRoles.FirstOrDefault(x => x.RoleId == RoleId);
                db.Entry(entity).State = EntityState.Deleted;
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return false;
            }
        }

        #endregion DeletePracticeRole

        #region GetPracticeRoleList  

        public List<PracticeRole> GetEntity_PracticeRoleList(Guid? OrgID, Guid? PracticeID)
        {
            try
            {
                var db = GetDataContext();
                return db.PracticeRoles.Where(x => x.IsActive == true && x.OrganizationId == OrgID &&
                x.PracticeId == PracticeID).ToList();
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        #endregion GetPracticeRoleList        

        #region CheckPracticeRole

        public bool CheckEntity_PracticeRoleExist(Guid? OrgID, Guid? PracticeID, string RoleName, int RoleId)
        {
            try
            {
                var db = GetDataContext();
                var obj = db.PracticeRoles.Where(x => x.IsActive == true && x.OrganizationId == OrgID &&
                 x.PracticeId == PracticeID && x.RoleName == RoleName && x.RoleId != RoleId).ToList();

                if (obj != null && obj.Count == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return false;
            }
        }

        #endregion CheckPracticeRole        

        #endregion PracticeRole

        #region ProviderPractice

        #region GetOrganizationPracticeByProviderId
        public List<Practice> GetOrganizationPracticeByProviderId(Guid OrganizationId, Guid ProviderId)
        {
            try
            {
                var db = GetDataContext();
                List<Practice> PracticeList = (from a in db.ProviderOrganizations
                                               join b in db.ProviderPractices on a.ID equals b.ProviderOrganizationId
                                               join c in db.Practices on b.PracticeId equals c.ID
                                               where b.ProviderId == ProviderId && a.OrganizationID == OrganizationId && c.IsActive == true
                                               select c).ToList();

                return PracticeList;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }
        #endregion GetOrganizationPracticeByProviderId

        #region GetProviderPractiveByProviderId
        public List<ProviderPractice> GetProviderPracticeByProviderId(Guid ProviderId)
        {
            try
            {
                var db = GetDataContext();
                return db.ProviderPractices.Where(x => x.ProviderOrganization.ProviderID == ProviderId).ToList();
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }
        #endregion GetProviderPractiveByProviderId

        #region SaveEntity_ProviderPractice
        public string SaveEntity_ProviderPractice(ProviderPractice entity)
        {
            try
            {
                var db = GetDataContext();
                db.Entry(entity).State = EntityState.Added;
                db.SaveChanges();
                return "Provider practice added successfully!";
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return "";
            }
        }
        #endregion SaveEntity_ProviderPractice

        #region DeleteProviderPractice
        public bool DeleteProviderPractice(Guid ProviderPracticeId)
        {
            try
            {
                var db = GetDataContext();

                List<ProviderPracticeRole> roleEntity = db.ProviderPracticeRoles.Where(x => x.ProviderPracticeID == ProviderPracticeId).ToList();
                if (roleEntity != null)
                {
                    db.ProviderPracticeRoles.RemoveRange(roleEntity);
                }
                ProviderPractice entity = db.ProviderPractices.FirstOrDefault(x => x.ID == ProviderPracticeId);
                if (entity != null)
                {
                    db.Entry(entity).State = System.Data.Entity.EntityState.Deleted;
                }
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return false;
            }
        }
        #endregion DeleteProviderPractice

        #endregion ProviderPractice

        #region ProviderPracticeRole

        public bool SaveEntity_ProviderPracticeRole(List<ProviderPracticeRole> entity, Guid ProviderPracticeID)
        {
            try
            {
                var db = GetDataContext();
                List<ProviderPracticeRole> Roles = db.ProviderPracticeRoles.Where(r => r.ProviderPracticeID == ProviderPracticeID).ToList();
                if (Roles != null)
                {
                    db.ProviderPracticeRoles.RemoveRange(Roles);
                }
                db.ProviderPracticeRoles.AddRange(entity);
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return false;
            }
        }

        public List<ProviderPracticeRole> GetEntity_ProviderPracticeRole(Guid ProviderPracticeID)
        {
            try
            {
                var db = GetDataContext();
                List<ProviderPracticeRole> RoleList = db.ProviderPracticeRoles.Where(r => r.ProviderPracticeID == ProviderPracticeID).ToList();

                return RoleList;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }

        }
        #endregion ProviderPracticeRole

        #region ProviderPatientThirdPartyApp

        #region CreateProviderPatientThirdPartyApp

        public bool SaveEntity_ProviderPatientThirdPartyApp(ProviderPatientThirdPartyApp entity)
        {
            try
            {
                var db = GetDataContext();
                db.Entry(entity).State = EntityState.Added;
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return false;
            }
        }

        #endregion CreateProviderPatientThirdPartyApp  

        #region GetProviderPatientThirdPartyApp

        public List<ProviderPatientThirdPartyApp> GetProviderPatientThirdPartyApp(Guid? PatientSurveyID)
        {
            try
            {
                var db = GetDataContext();
                return db.ProviderPatientThirdPartyApps.Where(x => x.PatientSurveyID == PatientSurveyID && x.IsActive == true).ToList();
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        #endregion GetProviderPatientThirdPartyApp          

        #region Delete ProviderPatientThirdPartyApp

        public bool DeleteEntity_PatientThirdPartyApp(int ThirdPartyAppId, Guid PatientSurveyID)
        {
            try
            {
                var db = GetDataContext();

                ProviderPatientThirdPartyApp entity = db.ProviderPatientThirdPartyApps.FirstOrDefault(x => x.ThirdPartyAppID == ThirdPartyAppId && x.PatientSurveyID == PatientSurveyID);
                db.Entry(entity).State = EntityState.Deleted;

                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return false;
            }
        }

        #endregion Delete ProviderPatientThirdPartyApp

        public bool CheckProviderPatientThirdPartyAppExist(Guid PatientSurveyId)
        {
            var db = GetDataContext();
            var obj = db.ProviderPatientThirdPartyApps.Where(x => x.PatientSurveyID == PatientSurveyId).FirstOrDefault();
            if (obj == null)
                return false;
            else
                return true;

        }

        public bool CheckProviderPatientThirdPartyAppExistByAppID(Guid PatientSurveyId, int AppID)
        {
            var db = GetDataContext();
            var obj = db.ProviderPatientThirdPartyApps.Where(x => x.PatientSurveyID == PatientSurveyId && x.ThirdPartyAppID == AppID).FirstOrDefault();
            if (obj == null)
                return false;
            else
                return true;

        }

        #endregion ProviderPatientThirdPartyApp

        #region Salutation        

        #region GetSalutationList  

        public List<Salutation> GetSalutationList()
        {
            try
            {
                var db = GetDataContext();
                return db.Salutations.Where(x => x.IsActive == true).ToList();
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        #endregion GetSalutationList        

        #endregion Salutation

        #region Pathway

        #region GetPathway

        public Pathway GetEntityById_Pathways(Guid id)
        {
            try
            {
                var db = GetDataContext();
                return db.Pathways.FirstOrDefault(x => x.ID == id);
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        public List<Pathway> GetEntity_PathwaysList()
        {
            try
            {
                var db = GetDataContext();
                return db.Pathways.Where(x => x.IsActive == true).OrderBy(x => x.PathwayName).ToList();
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        public List<Pathway> GetPathwayList(Guid guidProviderId)
        {
            try
            {
                var db = GetDataContext();
                return db.Pathways.Where(x => x.IsActive == true).OrderBy(x => x.PathwayName).ToList();
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        #endregion GetPathway

        #region Create Pathway

        public Guid SaveEntity_Pathway(Pathway entity)
        {
            try
            {
                var db = GetDataContext();
                db.Entry(entity).State = System.Data.Entity.EntityState.Added;
                db.SaveChanges();
                return entity.ID;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return new Guid();
            }
        }

        #endregion Create Pathway

        #region UpdatePathway

        public void UpdateEntity_Pathway(Pathway _entity)
        {
            try
            {
                var db = GetDataContext();
                Pathway entity = db.Pathways.FirstOrDefault(x => x.ID == _entity.ID);
                entity.PathwayName = _entity.PathwayName;
                entity.Description = _entity.Description;
                entity.IsActive = _entity.IsActive;
                db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
            }
        }

        public string UpdateEntityIsActiveStatus_Pathway(Guid ID, bool Status)
        {
            var db = GetDataContext();
            Pathway entity = db.Pathways.FirstOrDefault(x => x.ID == ID);

            entity.IsActive = Status;
            db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return "1";
        }

        #endregion UpdatePathway

        #region Delete Pathway

        public void DeleteEntity_Pathway(Guid id)
        {
            try
            {
                var db = GetDataContext();
                Pathway entity = db.Pathways.FirstOrDefault(x => x.ID == id);
                db.Entry(entity).State = System.Data.Entity.EntityState.Deleted;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
            }
        }

        #endregion Delete Pathway


        public bool CheckPathwayExistsInPatientSurvey(Guid PathwayId)
        {
            try
            {
                var db = GetDataContext();
                var objPatient = db.PatientSurvey_Pathway_PatientSurveyStatus.FirstOrDefault(x => x.PathwayID == PathwayId);
                if (objPatient == null)
                    return false;
                else
                    return true;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return false;
            }
        }

        #region SearchFilterPathway
        public List<usp_search_PathwayByFilter_Result> GetEntityBySearchFilter_Pathway(out int totalrecords, int? StartIndex = -1, int? EndIndex = -1, string SearchString = null, bool? IsActive = null)
        {
            totalrecords = 0;
            try
            {
                var db = GetDataContext();

                ObjectParameter TotalRecords = new ObjectParameter("TotalRecords", typeof(int));
                TotalRecords.Value = 0;

                var obj = db.usp_search_PathwayByFilter(StartIndex != -1 ? StartIndex + 1 : null, EndIndex != -1 ? EndIndex : null, SearchString, IsActive, TotalRecords).ToList();
                totalrecords = Convert.ToInt32(TotalRecords.Value);
                return obj;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        #endregion SearchFilterPathway

        #endregion Pathway

        #region Linked EHR

        public GetConsumerDetail_Result getConsumerDetails(string IHINo)
        {
            var dbModel = GetShEhrDbContext();
            return dbModel.GetConsumerDetail(IHINo).FirstOrDefault();
        }

        public GetConsumerDetailAddr_Result getConsumerDetailAddrs(int ContactID)
        {
            var dbModel = GetShEhrDbContext();
            var addr = dbModel.GetConsumerDetailAddr(ContactID).Where(x => x.type == "Telephone").FirstOrDefault();

            if (addr != null)
            {
                if (addr.telecom == null)
                {
                    addr = dbModel.GetConsumerDetailAddr(ContactID).Where(x => x.type == "Mobile").FirstOrDefault();
                }
            }
            else
            {
                addr = dbModel.GetConsumerDetailAddr(ContactID).Where(x => x.type == "Mobile").FirstOrDefault();
            }

            return addr;
        }

        #endregion

        #region CINT

        public int AddCINTScore(CINTScore objCINT)
        {
            try
            {
                var db = GetDataContext();
                db.Entry(objCINT).State = EntityState.Added;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
            }

            return objCINT.ID;
        }

        public List<CINTScore> GetCINTScore(int ID)
        {
            List<CINTScore> lstCINTScore = new List<CINTScore>();

            try
            {
                var db = GetDataContext();
                lstCINTScore = db.CINTScores.Where(x => x.ID == ID).ToList();
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
            }

            return lstCINTScore;
        }

        public List<CINTScore> GetAllCINTScore()
        {
            List<CINTScore> lstCINTScore = new List<CINTScore>();

            try
            {
                var db = GetDataContext();
                lstCINTScore = db.CINTScores.ToList();
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
            }

            return lstCINTScore;
        }

        #endregion CINT
    }
}
