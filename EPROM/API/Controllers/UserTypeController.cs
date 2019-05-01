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
    public class UserTypeController : ApiController
    {
        [System.Web.Http.HttpPost]
        public int Post(UserTypeModel UTM)
        {
            return UT.CreateUT(UTM);
        }

        [System.Web.Http.HttpGet]
        public string GetById(int ID)
        {
            return JsonConvert.SerializeObject(UT.GetUTByID(ID));
        }

        [System.Web.Http.HttpGet]
        public string Get()
        {
            return JsonConvert.SerializeObject(UT.GetUTList());
        }

        [System.Web.Http.HttpGet]
        public string GetUTSearch(int? StartIndex = -1, int? EndIndex = -1, string SearchString = null, bool? IsActive = null)
        {
            int TotalCount = 0;
            return JsonConvert.SerializeObject(UT.SearchFilterUT(TotalCount, StartIndex, EndIndex, SearchString, IsActive));
        }

        [System.Web.Http.HttpPost]
        public void Update(UserTypeModel ut)
        {
            UT.UpdateUT(ut);
        }

        [System.Web.Http.HttpPost]
        public string UpdateStatus()
        {
            string Ids = Request.Headers.GetValues("Ids").FirstOrDefault();
            bool status = Convert.ToBoolean(Request.Headers.GetValues("Status").FirstOrDefault());
            return UT.UpdateUTIsActiveStatus(Ids, status);
        }

        [System.Web.Http.HttpDelete]
        public string Delete(short id)
        {
            return UT.DeleteUTById(id);
        }
        
        [System.Web.Http.HttpDelete]
        public string DeleteMultiple()
        {
            string Ids = Request.Headers.GetValues("Ids").FirstOrDefault();
            return UT.DeleteMultipleUT(Ids);
        }
    }
}