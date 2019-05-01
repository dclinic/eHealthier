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
    public class OrganizationTypeController : ApiController
    {
        [System.Web.Http.HttpPost]
        public int Post(OrganizationTypeModel OTM)
        {
            return OT.CreateOT(OTM);
        }

        [System.Web.Http.HttpGet]
        public string GetById(int ID)
        {
            return JsonConvert.SerializeObject(OT.GetOTByID(ID));
        }

        [System.Web.Http.HttpGet]
        public string Get()
        {
            return JsonConvert.SerializeObject(OT.GetOTList());
        }

        [System.Web.Http.HttpGet]
        public string GetOTSearch(int? StartIndex = -1, int? EndIndex = -1, string SearchString = null, bool? IsActive = null)
        {
            int TotalCount = 0;
            return JsonConvert.SerializeObject(OT.SearchFilterOT(TotalCount, StartIndex, EndIndex, SearchString, IsActive));
        }

        [System.Web.Http.HttpPost]
        public void Update(OrganizationTypeModel ut)
        {
            OT.UpdateOT(ut);
        }

        [System.Web.Http.HttpPost]
        public string UpdateStatus()
        {
            string Ids = Request.Headers.GetValues("Ids").FirstOrDefault();
            bool status = Convert.ToBoolean(Request.Headers.GetValues("Status").FirstOrDefault());
            return OT.UpdateOTIsActiveStatus(Ids, status);
        }

        [System.Web.Http.HttpDelete]
        public string Delete(int ID)
        {
            return OT.DeleteOTById(ID);
        }

        [System.Web.Http.HttpDelete]
        public string DeleteMultiple()
        {
            string Ids = Request.Headers.GetValues("Ids").FirstOrDefault();
            return OT.DeleteMultipleOT(Ids);
        }
    }
}