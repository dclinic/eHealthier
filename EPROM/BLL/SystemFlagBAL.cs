using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
namespace BAL
{
    public class SystemFlagBAL
    {
        Entities objEntities = null;
        public SystemFlagBAL()
        {
            objEntities = new Entities();
        }

        public List<SystemFlag> GetAllSystemFlags()
        {
            List<SystemFlag> objSystemFlag = null;
            try
            {
                objSystemFlag = objEntities.GetEntity_SystemFlag();
            }
            catch (Exception)
            {
                objSystemFlag = null;
            }
            return objSystemFlag;
        }

        public string IsAlreadyReqForgetPwd(string Email, bool isRegister)
        {
            try
            {
                return objEntities.IsAlreadyReqForgetPwd(Email, isRegister);
            }
            catch (Exception)
            {
                return "";
            }
        }

        public string GenerateToken(string Email, string Token, bool isRegister)
        {
            try
            {
                return objEntities.GenerateToken(Email, Token, isRegister);
            }
            catch (Exception)
            {
                return "";
            }
        }

        public string ValidateToken(string Token)
        {
            try
            {
                return objEntities.ValidateToken(Token);
            }
            catch (Exception)
            {
                return "";
            }
        }

        public string DeleteToken(string Email, string Token, bool isRegister)
        {
            try
            {
                return objEntities.DeleteToken(Email, Token, isRegister);
            }
            catch (Exception)
            {
                return "";
            }
        }

        public bool CheckIsTokenExists(string Email, bool isRegister)
        {
            try
            {
                return objEntities.CheckIsTokenExists(Email, isRegister);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
