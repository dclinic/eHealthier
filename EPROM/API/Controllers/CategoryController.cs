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
    [RoutePrefix("category")]
    public class CategoryController : ApiController
    {
        [System.Web.Http.HttpPost]
        public int Post(SurveyCategory_model category)
        {
            return SurveyCategoriers.CreateCategory(category);
        }


        [System.Web.Http.HttpGet]
        public string GetById(short id)
        {
            return JsonConvert.SerializeObject(SurveyCategoriers.GetSurveyCategoryById(id));
        }

        [System.Web.Http.HttpGet]
        public string GetSubCategoryById(short id)
        {
            return JsonConvert.SerializeObject(SurveyCategoriers.GetSubCategoryById(id));
        }

        [System.Web.Http.HttpGet]
        public string Get()
        {
            return JsonConvert.SerializeObject(SurveyCategoriers.GetCategoryList());
        }

        [System.Web.Http.HttpGet]
        public string GetList(string id)
        {
            short CatId = 0;
            if (id != "undefined" && id != null && id != "")
                CatId = Convert.ToInt16(id);

            return JsonConvert.SerializeObject(SurveyCategoriers.GetCategory(CatId));
        }

        [System.Web.Http.HttpGet]
        public string GetCategorySearch(int? StartIndex = -1, int? EndIndex = -1, string SearchString = null, bool? IsActive = null)
        {
            int TotalCount = 0;

            return JsonConvert.SerializeObject(SurveyCategoriers.SearchFilterCategory(TotalCount, StartIndex, EndIndex, SearchString, IsActive));
        }

        [System.Web.Http.HttpPost]
        public void Put(SurveyCategory_model category)
        {
            SurveyCategoriers.UpdateSurveyCategory(category);
        }

        [System.Web.Http.HttpPost]
        public string UpdateStatus()
        {
            string Ids = Request.Headers.GetValues("Ids").FirstOrDefault();
            bool status = Convert.ToBoolean(Request.Headers.GetValues("Status").FirstOrDefault());

            return SurveyCategoriers.UpdateCategoryIsActiveStatus(Ids, status);
        }

        [System.Web.Http.HttpDelete]
        public string Delete(short id)
        {
            return SurveyCategoriers.DeleteSurveyCategoryById(id);
        }


        [System.Web.Http.HttpDelete]
        public string DeleteMultiple()
        {
            string Ids = Request.Headers.GetValues("Ids").FirstOrDefault();
            return SurveyCategoriers.DeleteMultipleSurveyCategory(Ids);
        }
    }
}
