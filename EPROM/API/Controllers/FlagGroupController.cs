using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BLL;

namespace API.Controllers
{
    [RoutePrefix("FlagGroup")]
    public class FlagGroupController : ApiController
    {
        [System.Web.Http.HttpPost]
        public int Post(FlagGroup_model flaggroup)
        {
            return FlagGroups.CreateFlagGroup(flaggroup);
        }

        [System.Web.Http.HttpPost]
        public void Put(FlagGroup_model flaggroup)
        {
            FlagGroups.UpdateFlagGroup(flaggroup);
        }

        [System.Web.Http.HttpDelete]
        public string Delete(int id)
        {
            return FlagGroups.DeleteFlagGroupById(id);
        }

        [System.Web.Http.HttpDelete]
        public string DeleteMultiple()
        {
            string Ids = Request.Headers.GetValues("Ids").FirstOrDefault();
            return FlagGroups.DeleteMultiple_FlagGroup(Ids);
        }

        [System.Web.Http.HttpGet]
        public string Get()
        {
            return JsonConvert.SerializeObject(FlagGroups.GetFlagGroup());
        }

        [System.Web.Http.HttpGet]
        public string GetFlagGroupSearch(int? StartIndex = -1, int? EndIndex = -1, string SearchString = null, bool? IsActive = null)
        {
            int TotalCount = 0;
            return JsonConvert.SerializeObject(FlagGroups.SearchFilter_FlagGroup(TotalCount, StartIndex, EndIndex, SearchString, IsActive));
        }

        [System.Web.Http.HttpGet]
        public string GetById(int id)
        {
            return JsonConvert.SerializeObject(FlagGroups.GetFlagGroupById(id));
        }

        [System.Web.Http.HttpPost]
        public string UpdateStatus()
        {
            string Ids = Request.Headers.GetValues("Ids").FirstOrDefault();
            bool status = Convert.ToBoolean(Request.Headers.GetValues("Status").FirstOrDefault());

            return FlagGroups.UpdateIsActiveStatus_FlagGroup(Ids, status);
        }

        [System.Web.Http.HttpGet]
        public string CheckDisplayOrderIsExistOrNot(int DisplayOrder)
        {
            return JsonConvert.SerializeObject(FlagGroups.CheckDisplayOrderExist_FlagGroup(DisplayOrder));
        }
    }
}

