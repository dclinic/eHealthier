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
    public class IndicatorsController : ApiController
    {
        [System.Web.Http.HttpPost]
        public int Post(Indicators_model indicator)
        {
            return Indicators.CreateIndicators(indicator);
        }


        [System.Web.Http.HttpGet]
        public string GetById(short id)
        {
            return JsonConvert.SerializeObject(Indicators.GetIndicatorById(id));
        }


        [System.Web.Http.HttpGet]
        public string Get()
        {
            return JsonConvert.SerializeObject(Indicators.GetIndicatorList());
        }

        [System.Web.Http.HttpGet]
        public string GetIndicatorSearch(int? StartIndex = -1, int? EndIndex = -1, string SearchString = null, bool? IsActive = null)
        {
            int TotalCount = 0;

            return JsonConvert.SerializeObject(Indicators.SearchFilterIndicator(TotalCount, StartIndex, EndIndex, SearchString, IsActive));
        }

        [System.Web.Http.HttpPost]
        public void Update(Indicators_model indicator)
        {
            Indicators.UpdateIndicator(indicator);
        }

        [System.Web.Http.HttpPost]
        public string UpdateStatus()
        {
            string Ids = Request.Headers.GetValues("Ids").FirstOrDefault();
            bool status = Convert.ToBoolean(Request.Headers.GetValues("Status").FirstOrDefault());

            return Indicators.UpdateIndicatorIsActiveStatus(Ids, status);
        }

        [System.Web.Http.HttpDelete]
        public string Delete(short id)
        {
            return Indicators.DeleteIndicatorById(id);
        }


        [System.Web.Http.HttpDelete]
        public string DeleteMultiple()
        {
            string Ids = Request.Headers.GetValues("Ids").FirstOrDefault();
            return Indicators.DeleteMultipleIndicator(Ids);
        }
    }
}
