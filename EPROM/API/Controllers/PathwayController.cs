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
    public class PathwayController : ApiController
    {
        [System.Web.Http.HttpPost]
        public Guid Post(Pathway_model category)
        {
            return Pathways.CreatePathway(category);
        }


        [System.Web.Http.HttpGet]
        public string GetById(Guid id)
        {
            return JsonConvert.SerializeObject(Pathways.GetPathwayById(id));
        }


        [System.Web.Http.HttpGet]
        public string Get()
        {
            return JsonConvert.SerializeObject(Pathways.GetPathwayList());
        }

        [System.Web.Http.HttpGet]
        public string GetPatientCategorySearch(int? StartIndex = -1, int? EndIndex = -1, string SearchString = null, bool? IsActive = null)
        {
            int TotalCount = 0;

            return JsonConvert.SerializeObject(Pathways.SearchFilterPathway(TotalCount, StartIndex, EndIndex, SearchString, IsActive));
        }

        [System.Web.Http.HttpPost]
        public void Update(Pathway_model category)
        {
            Pathways.UpdatePathway(category);
        }

        [System.Web.Http.HttpPost]
        public string UpdateStatus()
        {
            string Ids = Request.Headers.GetValues("Ids").FirstOrDefault();
            bool status = Convert.ToBoolean(Request.Headers.GetValues("Status").FirstOrDefault());

            return Pathways.UpdatePathwayIsActiveStatus(Ids, status);
        }

        [System.Web.Http.HttpDelete]
        public string Delete(Guid id)
        {
            return Pathways.DeletePathwayById(id);
        }


        [System.Web.Http.HttpDelete]
        public string DeleteMultiple()
        {
            string Ids = Request.Headers.GetValues("Ids").FirstOrDefault();
            return Pathways.DeleteMultiplePathway(Ids);
        }
    }
}
