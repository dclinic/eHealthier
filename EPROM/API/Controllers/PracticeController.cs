using BLL;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace API.Controllers
{
    public class PracticeController : ApiController
    {
        [HttpPost]
        public string UpdatePracticeDetail(Practice_custom_model practice)
        {
            return PracticeClass.ManagePracticeDetail(practice);
        }

        [HttpGet]
        public Practice_custom_model GetPracticeDetail(Guid UserId)
        {
            return PracticeClass.GetPracticeDetail(UserId);
        }

        [HttpGet]
        public List<Practice_Model> GetPracticeListBy_OrganizationID(string UserName, string OrganizationID)
        {
            return PracticeClass.GetPracticeListBy_OrganizationID(UserName, OrganizationID);
        }

        [HttpGet]
        public Guid? GetUserIdByUserName(string UserName)
        {
            return Providers.GetUserIdByUserName(UserName);
        }

        [HttpPost]
        public bool ManagePracticeRole(PracticeRole_Model practiceRole)
        {
            return PracticeRoleClass.ManagePracticeRole(practiceRole);
        }

        [HttpGet]
        public List<PracticeRole_Model> GetPracticeRole(Guid OrganizationId, Guid UserId, Guid PracticeId)
        {
            return PracticeRoleClass.GetPracticeRole(OrganizationId, UserId, PracticeId);
        }

        [HttpDelete]
        public bool DeletePracticeRole(int RoleId)
        {
            return PracticeRoleClass.DeletePracticeRole(RoleId);
        }

        [HttpGet]
        public bool CheckPracticeRoleExist(Guid PracticeId, string RoleName, int RoleID, Guid UserId, Guid OrganizationId)
        {
            return PracticeRoleClass.CheckPracticeRoleExist(PracticeId, RoleName, RoleID, UserId, OrganizationId);
        }

        [HttpGet]
        public List<Practice_Model> GetOrganizationPracticeByProviderId(string UserName, Guid OrganizationId)
        {
            string strProviderId = ProviderOrganizationClass.GetProviderIdFromUserName(UserName);
            if (strProviderId != null && strProviderId != "")
            {
                Guid ProviderId = new Guid(strProviderId);
                return PracticeClass.GetOrganizationPracticeByProviderId(OrganizationId, ProviderId);
            }
            else
            {
                return null;
            }
        }
    }
}
