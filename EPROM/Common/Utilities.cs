using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Security;

namespace Common
{
    public class Utilities
    {
        #region Encryption/Decryption Methods

        /// <summary>
        /// This Method Encrypt data using AES Alghorithm
        /// </summary>
        /// <param name="plainText">String to Encrypt</param>
        /// <returns>It returns encrypted string if succeed, else returns null</returns>
        public static string Encrypt(string plainText)
        {
            try
            {
                AESCFB8ModeEncryption AES = new AESCFB8ModeEncryption();
                return AES.Encrypt(plainText);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// This Method Decrypt data using AES Alghorithm
        /// </summary>
        /// <param name="plainText">Encrypted String to Decrypt</param>
        /// <returns>It returns decrypted string if succeed, else returns null</returns>
        public static string Decrypt(string cipherText)
        {
            try
            {
                AESCFB8ModeEncryption AES = new AESCFB8ModeEncryption();
                return AES.Decrypt(cipherText);
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion Encryption/Decryption Methods

        #region Json Serialize/Deserialize

        public static string JSONSerialize(object obj)
        {
            try
            {
                return new JavaScriptSerializer().Serialize(obj);
                //return new JavaScriptSerializer { MaxJsonLength = Int32.MaxValue }.Serialize(obj);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static object JSONDeSerialize(string Value, Type type)
        {
            try
            {
                return new JavaScriptSerializer().Deserialize(Value, type);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string EncodeTo64(string toEncode)
        {
            try
            {
                byte[] toEncodeAsBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(toEncode);
                string returnValue = System.Convert.ToBase64String(toEncodeAsBytes);
                return returnValue;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string DecodeFrom64(string encodedData)
        {
            try
            {
                byte[] encodedDataAsBytes = System.Convert.FromBase64String(encodedData);
                string returnValue = System.Text.ASCIIEncoding.ASCII.GetString(encodedDataAsBytes);
                return returnValue;
            }
            catch (Exception)
            {
                return null;
            }

        }

        #endregion

        public string GetImagesInHTMLString(string htmlString, ref string CID)
        {
            try
            {
                string pattern = @"(?<=src="").*?(?="")";
                Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                //MatchCollection matches = rgx.Matches(htmlString);
                int i = 1;
                foreach (Match match in Regex.Matches(htmlString, pattern))
                {
                    string temp = "cid:" + i + "";
                    //string[] _temp = match.ToString().Split('=').ToArray();
                    string path = match.ToString().Replace(HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Host, "~") + "^" + i;
                    //Replace SRC
                    htmlString = htmlString.Replace(match.ToString(), temp);
                    CID += path + ",";
                    i++;
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }
            return htmlString;
        }

        #region MethodToCheckRole

        public static bool IsRoleSelected(string UserName)
        {
            string[] UserInProvider = Roles.FindUsersInRole("provider", UserName);
            string[] UserInOrg = Roles.FindUsersInRole("organization", UserName);
            string[] UserInPractice = Roles.FindUsersInRole("practice", UserName);

            if (UserInProvider.Length > 0 || UserInOrg.Length > 0 || UserInPractice.Length > 0)
                return true;
            else
                return false;
        }

        #endregion


        public static DateTime? ConvertServerCurrentDateTimeToClientDateTime(DateTime? dt)
        {
            try
            {
                if (dt.HasValue)
                {
                    dt = dt.Value.ToUniversalTime();

                    if (HttpContext.Current.Request.Headers["timezoneoffset"] != null && !string.IsNullOrEmpty(HttpContext.Current.Request.Headers.GetValues("timezoneoffset").FirstOrDefault()))
                    {
                        var timeOffSet = HttpContext.Current.Request.Headers.GetValues("timezoneoffset").FirstOrDefault();
                        if (timeOffSet != null)
                        {
                            var offset = int.Parse(timeOffSet.ToString());
                            if (offset >= 0)
                            {
                                dt = dt.Value.AddMinutes(-offset);
                            }
                            else
                            {
                                dt = dt.Value.AddMinutes((-1 * offset));
                            }
                            return dt;
                        }
                    }
                    return dt;
                }
                else
                {
                    return dt;
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return null;
            }           
        }

        public static DateTime? ConvertToDateTime(string Value)
        {
            DateTime? objDateTime = null;
            try
            {
                objDateTime = DateTime.Parse(Value, CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }
            return objDateTime;
        }
    }
}
