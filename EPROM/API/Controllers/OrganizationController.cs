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
    public class OrganizationController : ApiController
    {
        [System.Web.Http.HttpPost]
        public bool UpdateOrganizationDetail(Organizations_custom_model organization)
        {
            return OrganizationsClass.ManageOrganizationDetail(organization);
        }

        [System.Web.Http.HttpGet]
        public Organizations_custom_model GetOrganizationDetails(string UserName)
        {
            return OrganizationsClass.GetOrganizationDetail(UserName);
        }

        [System.Web.Http.HttpGet]
        public List<Organization_Model> GetOrganizationByProviderId(string UserName)
        {
            string strProviderId = ProviderOrganizationClass.GetProviderIdFromUserName(UserName);
            if (strProviderId != null && strProviderId != "")
            {
                Guid ProviderId = new Guid(strProviderId);
                return ProviderOrganizationClass.GetOrganizationByProviderId(ProviderId);
            }
            else
            {
                return null;
            }
        }
    }
}
