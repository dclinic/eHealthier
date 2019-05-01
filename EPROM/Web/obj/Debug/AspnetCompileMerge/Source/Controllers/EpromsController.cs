using System;
using System.Linq;
using System.Web.Mvc;
using System.IO;
using BLL;
using System.Data;
using Microsoft.VisualBasic.FileIO;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Packaging;
using System.Text;
using System.Reflection;
using DAL;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace ePRom.Controllers
{
    public class EpromsController : Controller
    {
        [Authorize(Roles = "admin")]
        public ActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToRoute("AdminLogin");
            }
            else
            {
                return View();
            }
        }

        [Authorize(Roles = "admin")]
        public ActionResult Create()
        {
            return View();
        }


        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        [Authorize(Roles = "admin")]
        public ActionResult Edit(int id)
        {
            return View();
        }


        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        [Authorize(Roles = "admin")]
        public ActionResult Delete(int id)
        {
            return View();
        }

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public string UploadEpromFile()
        {
            try
            {
                string SurveyId = Request.Headers.GetValues("surveyId").FirstOrDefault();
                string DirectoryName = "~/Content/Files/";
                string DirectoryPath = Server.MapPath(DirectoryName);

                if (!Directory.Exists(DirectoryPath))
                {
                    Directory.CreateDirectory(DirectoryPath);
                }

                if (System.Web.HttpContext.Current.Request.Files.Count > 0)
                {
                    var file = System.Web.HttpContext.Current.Request.Files[0];
                    if (file.ContentLength > 0)
                    {
                        var extension = Path.GetExtension(file.FileName);
                        var fileName = Guid.NewGuid().ToString();
                        var filepath = Server.MapPath("/Content/Files/") + fileName + extension;
                        file.SaveAs(filepath);
                        return fileName + extension;
                    }
                }
                return "";
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return "";
            }
        }


        [HttpPost]
        public string ReadCsvFile(string FileName, string Title)
        {
            try
            {
                string Returnval = "";
                EpromScore objEpromScore = new EpromScore();

                string xmlPath = Server.MapPath("/Content/Xml/EpromStaticValue.xml");
                objEpromScore.ReadXmlFile(xmlPath);

                string filePath = Server.MapPath("/Content/Files/") + FileName;
                if (System.IO.File.Exists(filePath))
                {
                    int pos = filePath.LastIndexOf(".") + 1;
                    string extension = filePath.Substring(pos, filePath.Length - pos);

                    if (extension.ToLower() == "csv")
                    {
                        DataTable tblCsv = GenerateTableFromCsv(filePath);

                        if (Title == "PROMIS Global 10")
                        {
                            Returnval = objEpromScore.Create_PROMISG10_ScoreTable(tblCsv);
                        }
                        else if (Title == "Orebro Musculoskeletal Pain Questionnaire")
                        {
                            Returnval = objEpromScore.Calculate_OMPQ_Score(tblCsv);
                        }
                        else if (Title == "Hip disability and Osteoarthritis Outcome Score (HOOS)")
                        {
                            Returnval = objEpromScore.Calculate_HOOS_Score(tblCsv);
                        }
                        else if (Title == "Neck Disability Index - Vernon and Mior Cervical Spine Questionnaire")
                        {
                            Returnval = objEpromScore.Calculate_NeckPain_Score(tblCsv);
                        }
                        else if (Title == "KOOS-Physical Function Shortform (KOOS-PS)")
                        {
                            Returnval = objEpromScore.Calculate_KOOS_Score(tblCsv);
                        }
                        else if (Title == "Kessler Psychological Distress Scale (K10)")
                        {
                            Returnval = objEpromScore.Calculate_K10_Score(tblCsv);
                        }
                        else if (Title == "QuickDASH Questionnaire")
                        {
                            Returnval = objEpromScore.Calculate_QDASH_Score(tblCsv);
                        }
                        else if (Title == "Modified Oswestry Low Back Disability Questionnaire")
                        {
                            Returnval = objEpromScore.Calculate_Oswestry_Score(tblCsv);
                        }
                        else
                        {
                            Returnval = "";
                        }
                    }
                    else if (extension.ToLower() == "xlsx")
                    {
                        DataTable dtExcel = new DataTable();
                        if (Title == "PROMIS Global 10")
                        {
                            dtExcel = GenerateTableFromExcel(filePath, "PROMIS G10 Data");
                            Returnval = objEpromScore.Create_PROMISG10_ScoreTable(dtExcel);
                        }
                        else if (Title == "Orebro Musculoskeletal Pain Questionnaire")
                        {
                            dtExcel = GenerateTableFromExcel(filePath, "OMPQ Data");
                            Returnval = objEpromScore.Calculate_OMPQ_Score(dtExcel);
                        }
                        else if (Title == "Hip disability and Osteoarthritis Outcome Score (HOOS)")
                        {
                            dtExcel = GenerateTableFromExcel(filePath, "HOOS Data");
                            Returnval = objEpromScore.Calculate_HOOS_Score(dtExcel);
                        }
                        else if (Title == "Neck Disability Index - Vernon and Mior Cervical Spine Questionnaire")
                        {
                            dtExcel = GenerateTableFromExcel(filePath, "Neck Pain Data");
                            Returnval = objEpromScore.Calculate_NeckPain_Score(dtExcel);
                        }
                        else if (Title == "KOOS-Physical Function Shortform (KOOS-PS)")
                        {
                            dtExcel = GenerateTableFromExcel(filePath, "KOOS Data");
                            Returnval = objEpromScore.Calculate_KOOS_Score(dtExcel);
                        }
                        else if (Title == "Kessler Psychological Distress Scale (K10)")
                        {
                            dtExcel = GenerateTableFromExcel(filePath, "K10 Data");
                            Returnval = objEpromScore.Calculate_K10_Score(dtExcel);
                        }
                        else if (Title == "QuickDASH Questionnaire")
                        {
                            dtExcel = GenerateTableFromExcel(filePath, "QDASH Data");
                            Returnval = objEpromScore.Calculate_QDASH_Score(dtExcel);
                        }
                        else if (Title == "Modified Oswestry Low Back Disability Questionnaire")
                        {
                            dtExcel = GenerateTableFromExcel(filePath, "Oswestry Data");
                            Returnval = objEpromScore.Calculate_Oswestry_Score(dtExcel);
                        }
                        else
                        {
                            Returnval = "";
                        }
                    }
                    return Returnval;
                }
                return "";
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return "";
            }
        }

        public DataTable GenerateTableFromCsv(string FilePath)
        {
            try
            {
                DataTable tblCsv = new DataTable();
                string csvData = System.IO.File.ReadAllText(FilePath);

                TextFieldParser parser = new TextFieldParser(FilePath);
                parser.HasFieldsEnclosedInQuotes = true;
                parser.SetDelimiters(",");

                string[] fields;

                int index = 0;

                while (!parser.EndOfData)
                {
                    if (index > 0)
                        tblCsv.Rows.Add();

                    fields = parser.ReadFields();
                    int count = 0;
                    for (int i = 0; i < fields.Count(); i++)
                    {
                        if (index == 0)
                        {
                            tblCsv.Columns.Add(fields[i]);
                        }
                        else
                        {
                            tblCsv.Rows[tblCsv.Rows.Count - 1][count] = fields[i];
                            count++;
                        }
                    }
                    index++;
                }

                return tblCsv;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        public DataTable GenerateTableFromExcel(string FilePath, string SheetName)
        {
            try
            {
                DataTable dtData = new DataTable();
                Microsoft.Office.Interop.Excel.Application oXL = null;
                Microsoft.Office.Interop.Excel.Workbook oWB = null;
                Microsoft.Office.Interop.Excel.Worksheet oSheet = null;
                Microsoft.Office.Interop.Excel.Range oRng = null;

                oXL = new Microsoft.Office.Interop.Excel.Application();

                //Getting a WorkBook object
                oWB = oXL.Workbooks.Open(FilePath, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value);

                oSheet = (Microsoft.Office.Interop.Excel.Worksheet)oWB.Sheets[SheetName];

                StringBuilder sb = new StringBuilder();
                //Total Column Count
                int jValue = oSheet.UsedRange.Cells.Columns.Count;
                //Total Row count   
                int iValue = oSheet.UsedRange.Cells.Rows.Count;

                for (int i = 1; i <= iValue; i++)
                {
                    int count = 0;
                    if (i > 1)
                    {
                        dtData.Rows.Add();
                    }
                    for (int j = 1; j <= jValue; j++)
                    {
                        if (i == 1)
                        {
                            oRng = (Microsoft.Office.Interop.Excel.Range)oSheet.Cells[i, j];
                            string strValue = oRng.Text.ToString();
                            dtData.Columns.Add(strValue);
                        }
                        else
                        {
                            oRng = (Microsoft.Office.Interop.Excel.Range)oSheet.Cells[i, j];
                            dtData.Rows[dtData.Rows.Count - 1][count] = oRng.Text;
                            count++;
                        }
                    }
                }

                //Release the Excel objects                
                oWB.Close(false, System.Reflection.Missing.Value, System.Reflection.Missing.Value);
                oXL.Workbooks.Close();
                oXL.Quit();
                oXL = null;
                oWB = null;
                oSheet = null;
                oRng = null;
                GC.GetTotalMemory(false);
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.GetTotalMemory(true);
                //Release the Excel objects               

                return dtData;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return null;
            }
        }

        private string GetValue(SpreadsheetDocument doc, Cell cell)
        {
            string value = cell.CellValue.InnerText;
            if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
            {
                return doc.WorkbookPart.SharedStringTablePart.SharedStringTable.ChildElements.GetItem(int.Parse(value)).InnerText;
            }
            return value;
        }

        [HttpPost]
        public string PatientScore(ResponseCustomClass dataList)
        {
            try
            {
                ScoreCalculation objEpromScore = new ScoreCalculation();

                string xmlPath = Server.MapPath("/Content/Xml/EpromStaticValue.xml");
                objEpromScore.ReadXmlFile(xmlPath);

                return objEpromScore.GetEpromScore(dataList);

            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return "";
            }
        }

        public ActionResult DefaultTime()
        {
            return View();
        }
    }
}
