using BLL;
using DAL;
using ePRom.Filters;
using OfficeOpenXml;
using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using WebMatrix.WebData;

namespace ePRom.Controllers
{
    [InitializeSimpleMembership]
    public class CINTController : Controller
    {
        public ActionResult PROMIS10()
        {
            Entities.SetCookie("", "Gender");
            Entities.SetCookie("", "Age");
            Entities.SetCookie("", "PostCode");

            string email = "ehealthier.patient02@gmail.com";
            string parameterName = Request.QueryString.Keys[0];
            string parameterValue = Request.QueryString[parameterName];

            bool isLogin = WebSecurity.Login(email, "123456", true);

            Entities.SetCookie(email, "emailCINT");
            Entities.SetCookie(parameterName, "IDName");
            Entities.SetCookie(parameterValue, "IDValue");
            Entities.SetCookie("PROMIS10", "ePROMsType");
            Entities.SetCookie("122967872", "SurveyMonkeyID");
            Entities.SetCookie("162624632", "CollectorID");
            Entities.SetCookie("https://www.research.net/r/MURRAYPHN_PROMIS10?email=" + email + "&uniqueId=" + parameterValue, "ePROMsLink");

            return View();
        }

        public ActionResult POPN()
        {
            Entities.SetCookie("", "Gender");
            Entities.SetCookie("", "Age");
            Entities.SetCookie("", "PostCode");

            string email = "ehealthier.patient02@gmail.com";
            string parameterName = Request.QueryString.Keys[0];
            string parameterValue = Request.QueryString[parameterName];

            bool isLogin = WebSecurity.Login(email, "123456", true);

            Entities.SetCookie(email, "emailCINT");
            Entities.SetCookie(parameterName, "IDName");
            Entities.SetCookie(parameterValue, "IDValue");
            Entities.SetCookie("POPN", "ePROMsType");
            Entities.SetCookie("122969220", "SurveyMonkeyID");
            Entities.SetCookie("162625072", "CollectorID");
            Entities.SetCookie("https://www.research.net/r/MURRAYPHN_POPULATION_HEALTH?email=" + email + "&uniqueId=" + parameterValue, "ePROMsLink");

            return View();
        }

        [Authorize(Roles = "patient")]
        public ActionResult CompleteCINTEprom()
        {
            return View();
        }

        public void ExportToExcel()
        {
            try
            {
                string dirPath = Server.MapPath("~/Content/CINT");
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }

                DateTime dt = DateTime.Now.Date;
                string fname = dt.ToString("yyyyMMdd") + ".xlsx";
                string filePath = Path.Combine(dirPath, fname);

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                var file = new FileInfo(filePath);

                using (var package = new ExcelPackage(file))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("CINT Survey");

                    worksheet.Cells[1, 1].Value = "ePROMs Type";
                    worksheet.Cells[1, 2].Value = "ID Name";
                    worksheet.Cells[1, 3].Value = "ID Value";
                    worksheet.Cells[1, 4].Value = "Gender";
                    worksheet.Cells[1, 5].Value = "Age";
                    worksheet.Cells[1, 6].Value = "Post Code";
                    worksheet.Cells[1, 7].Value = "Score";
                    worksheet.Cells[1, 8].Value = "Survey Monkey ID";
                    worksheet.Cells[1, 9].Value = "Responded ID";

                    DateTime todayDate = DateTime.Now.Date;
                    var lstCINTScore = HelperMethods.GetEntities().GetAllCINTScore();
                    lstCINTScore = lstCINTScore.Where(x => x.CreatedDate.Value.Date == todayDate).ToList();

                    int i = 2;
                    foreach (var item in lstCINTScore)
                    {
                        worksheet.Cells[i, 1].Value = item.ePROMsType;
                        worksheet.Cells[i, 2].Value = item.IDName;
                        worksheet.Cells[i, 3].Value = item.IDValue;
                        worksheet.Cells[i, 4].Value = item.Gender;
                        worksheet.Cells[i, 5].Value = item.Age;
                        worksheet.Cells[i, 6].Value = item.PostCode;
                        worksheet.Cells[i, 7].Value = item.Score;
                        worksheet.Cells[i, 8].Value = item.SurveyMonkeyID;
                        worksheet.Cells[i, 9].Value = item.RespondedID;

                        i++;
                    }

                    worksheet.Column(1).AutoFit();
                    worksheet.Column(2).AutoFit();
                    worksheet.Column(3).AutoFit();
                    worksheet.Column(4).AutoFit();
                    worksheet.Column(5).AutoFit();
                    worksheet.Column(6).AutoFit();
                    worksheet.Column(7).AutoFit();
                    worksheet.Column(8).AutoFit();
                    worksheet.Column(9).AutoFit();

                    package.Save();
                }
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
            }
        }
    }
}