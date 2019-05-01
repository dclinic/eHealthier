using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BLL;
using Newtonsoft.Json;

namespace API.Controllers
{
    [RoutePrefix("SystemFlag")]
    public class SystemFlagController : ApiController
    {
        [System.Web.Http.HttpPost]
        public int Post(SystemFlags_model SystemFlag)
        {
            return SystemFlags.CreateSystemFlag(SystemFlag);
        }

        [System.Web.Http.HttpPost]
        public void Put(SystemFlags_model SystemFlag)
        {
            SystemFlags.UpdateSystemFlag(SystemFlag);
        }

        [System.Web.Http.HttpDelete]
        public string Delete(int id)
        {
            return SystemFlags.DeleteSystemFlagById(id);
        }

        [System.Web.Http.HttpDelete]
        public string DeleteMultiple()
        {
            string Ids = Request.Headers.GetValues("Ids").FirstOrDefault();
            return SystemFlags.DeleteMultipleSystemFlag(Ids);
        }

        [System.Web.Http.HttpGet]
        public string Get()
        {
            return JsonConvert.SerializeObject(SystemFlags.GetSystemFlag());
        }

        [System.Web.Http.HttpGet]
        public string GetSystemFlagSearch(int? StartIndex = -1, int? EndIndex = -1, string SearchString = null, bool? IsActive = null)
        {
            int TotalCount = 0;
            return JsonConvert.SerializeObject(SystemFlags.SearchFilterSystemFlag(TotalCount, StartIndex, EndIndex, SearchString, IsActive));
        }

        [System.Web.Http.HttpGet]
        public string GetById(int id)
        {
            return JsonConvert.SerializeObject(SystemFlags.GetSystemFlagById(id));
        }

        [System.Web.Http.HttpPost]
        public string UpdateStatus()
        {
            string Ids = Request.Headers.GetValues("Ids").FirstOrDefault();
            bool status = Convert.ToBoolean(Request.Headers.GetValues("Status").FirstOrDefault());

            return SystemFlags.UpdateSystemFlagIsActiveStatus(Ids, status);
        }

        [System.Web.Http.HttpGet]
        public string CheckDisplayOrderIsExistOrNot(int DisplayOrder)
        {
            return JsonConvert.SerializeObject(SystemFlags.CheckDisplayOrderExistOrNot(DisplayOrder));
        }

        //[System.Web.Http.HttpGet]
        //public List<SystemFlags_model> GetEntityById_FlagGroupId(int FlagGroupId)
        //{
        //    return SystemFlags.GetEntityById_FlagGroupId(FlagGroupId);
        //}
    }
}
