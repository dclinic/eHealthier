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
    public class PatientCategoryController : ApiController
    {
        [System.Web.Http.HttpPost]
        public int Post(PatientCategory_model category)
        {
            return PatientCategoriers.CreatePatientCategory(category);
        }


        [System.Web.Http.HttpGet]
        public string GetById(short id)
        {
            return JsonConvert.SerializeObject(PatientCategoriers.GetPatientCategoryById(id));
        }


        [System.Web.Http.HttpGet]
        public string Get()
        {
            return JsonConvert.SerializeObject(PatientCategoriers.GetPatientCategoryList());
        }

        [System.Web.Http.HttpGet]
        public string GetPatientCategorySearch(int? StartIndex = -1, int? EndIndex = -1, string SearchString = null, bool? IsActive = null)
        {
            int TotalCount = 0;

            return JsonConvert.SerializeObject(PatientCategoriers.SearchFilterPatientCategory(TotalCount, StartIndex, EndIndex, SearchString, IsActive));
        }

        [System.Web.Http.HttpPost]
        public void Update(PatientCategory_model category)
        {
            PatientCategoriers.UpdatePatientCategory(category);
        }

        [System.Web.Http.HttpPost]
        public string UpdateStatus()
        {
            string Ids = Request.Headers.GetValues("Ids").FirstOrDefault();
            bool status = Convert.ToBoolean(Request.Headers.GetValues("Status").FirstOrDefault());

            return PatientCategoriers.UpdatePatientCategoryIsActiveStatus(Ids, status);
        }

        [System.Web.Http.HttpDelete]
        public string Delete(short id)
        {
            return PatientCategoriers.DeletePatientCategoryById(id);
        }


        [System.Web.Http.HttpDelete]
        public string DeleteMultiple()
        {
            string Ids = Request.Headers.GetValues("Ids").FirstOrDefault();
            return PatientCategoriers.DeleteMultiplePatientCategory(Ids);
        }
    }
}
