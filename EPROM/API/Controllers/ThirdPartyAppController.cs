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
    public class ThirdPartyAppController : ApiController
    {
        [System.Web.Http.HttpPost]
        public int Post(ThirdPartyApp_model app)
        {
            return ThirdPartyAppClass.CreateThirdPartyApp(app);
        }


        [System.Web.Http.HttpGet]
        public string GetById(short id)
        {
            return JsonConvert.SerializeObject(ThirdPartyAppClass.GetThirdPartyAppById(id));
        }


        [System.Web.Http.HttpGet]
        public string Get()
        {
            return JsonConvert.SerializeObject(ThirdPartyAppClass.GetThirdPartyAppList());
        }

        [System.Web.Http.HttpGet]
        public string GetThirdPartyAppSearch(int? StartIndex = -1, int? EndIndex = -1, string SearchString = null, bool? IsActive = null)
        {
            int TotalCount = 0;

            return JsonConvert.SerializeObject(ThirdPartyAppClass.SearchFilterThirdPartyApp(TotalCount, StartIndex, EndIndex, SearchString, IsActive));
        }

        [System.Web.Http.HttpPost]
        public void Update(ThirdPartyApp_model app)
        {
            ThirdPartyAppClass.UpdateThirdPartyApp(app);
        }

        [System.Web.Http.HttpPost]
        public string UpdateStatus()
        {
            string Ids = Request.Headers.GetValues("Ids").FirstOrDefault();
            bool status = Convert.ToBoolean(Request.Headers.GetValues("Status").FirstOrDefault());

            return ThirdPartyAppClass.UpdateThirdPartyAppIsActiveStatus(Ids, status);
        }

        [System.Web.Http.HttpDelete]
        public string Delete(short id)
        {
            return ThirdPartyAppClass.DeleteThirdPartyAppById(id);
        }


        [System.Web.Http.HttpDelete]
        public string DeleteMultiple()
        {
            string Ids = Request.Headers.GetValues("Ids").FirstOrDefault();
            return ThirdPartyAppClass.DeleteMultipleThirdPartyApp(Ids);
        }

        [System.Web.Http.HttpGet]
        public string GetThirdPartyAppByCategoryID(short SurveyID)
        {
            return JsonConvert.SerializeObject(ThirdPartyAppClass.GetThirdPartyAppByCategoryID(SurveyID));
        }

    }
}
