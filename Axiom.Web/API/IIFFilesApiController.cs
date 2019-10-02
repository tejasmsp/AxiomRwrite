using Axiom.Common;
using Axiom.Entity;
using Axiom.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.SqlClient;
using System.IO;
using System.Data;
using System.Reflection;
using System.Web;
using System.Net.Http.Headers;
using AXIOM.Common;
using System.Text;

namespace Axiom.Web.API
{
    [RoutePrefix("api")]
    public class IIFFilesApiController : ApiController
    {
        #region Initialization
        private readonly GenericRepository<SSNSettings> _repository = new GenericRepository<SSNSettings>();
        #endregion

        #region DatabaseOperations

        [HttpPost]
        [Route("IIFGenerateCheckList")]
        public TableParameter<IIFCheckListEntity> IIFGenerateCheckList(TableParameter<IIFCheckListEntity> tableParameter, int PageIndex, string FromDate = "",
                string ToDate = "",bool ToBePrint = false)
        {

            tableParameter.PageIndex = PageIndex;
            string sortColumn = tableParameter.SortColumn.Desc ? tableParameter.SortColumn.Column + " desc" : tableParameter.SortColumn.Column + " asc";
            string searchValue = string.Empty;
            var response = new ApiResponse<IIFCheckListEntity>();
            try
            {
                SqlParameter[] param =
                    {

                  new SqlParameter  {
                     ParameterName = "Keyword",
                     DbType = DbType.String,
                     Value = searchValue
                 },new SqlParameter
                 {
                     ParameterName = "PageIndex",
                     DbType = DbType.Int32,
                     Value = tableParameter.PageIndex
                 }, new SqlParameter
                 {
                     ParameterName = "PageSize",
                     DbType = DbType.Int32,
                     Value = (object)tableParameter != null ? tableParameter.iDisplayLength : 10
                 },
                new SqlParameter
                {
                    ParameterName = "SortBy",
                    DbType = DbType.String,
                    Value =sortColumn
                },
                new SqlParameter{ParameterName = "FromDate",DbType = DbType.Date,Value = (object)Convert.ToDateTime(FromDate) ?? (object)DBNull.Value  },
                new SqlParameter{ParameterName = "ToDate",DbType = DbType.Date,Value = (object)Convert.ToDateTime(ToDate) ?? (object)DBNull.Value  },
                new SqlParameter{ParameterName = "ToBePrint",DbType = DbType.Boolean,Value = (object)ToBePrint ?? (object)DBNull.Value  }
                //new SqlParameter{ParameterName = "City",DbType = DbType.String,Value =  (object)City ?? (object)DBNull.Value },
                //new SqlParameter{ParameterName = "State",DbType = DbType.String,Value = (object)State ?? (object)DBNull.Value  },
                //new SqlParameter{ParameterName = "AssociatedFirm",DbType = DbType.String,Value = (object)ParentFirm ?? (object)DBNull.Value  }
                };

                var result = _repository.ExecuteSQL<IIFCheckListEntity>("IIFGenerateCheckList", param).ToList();
                response.Success = true;
                response.Data = result;

                int totalRecords = 0;
                if (response != null && response.Data.Count > 0)
                {
                    totalRecords = response.Data[0].TotalRecords;
                }

                return new TableParameter<IIFCheckListEntity>
                {
                    aaData = (List<IIFCheckListEntity>)response.Data,
                    iTotalRecords = totalRecords,
                    iTotalDisplayRecords = totalRecords
                };

            }
            catch (Exception ex)
            {

                response.Message.Add(ex.Message);
            }

            return new TableParameter<IIFCheckListEntity>();
        }

        [HttpPost]
        [Route("PrintCheckIIFFiles")]
        public HttpResponseMessage PrintCheckIIFFiles(string fromDate,string toDate,string checkID,int checkNo)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);

            try
            {
                SqlParameter[] param = { new SqlParameter("FromDate", (object)fromDate ?? (object)DBNull.Value)
                                        , new SqlParameter("ToDate", (object)toDate ?? (object)DBNull.Value)
                                        , new SqlParameter("CheckID", (object)checkID ?? (object)DBNull.Value)
                                        , new SqlParameter("CheckNumber", (object)checkNo ?? (object)DBNull.Value)
                                        };
                var result = _repository.ExecuteSQL<IIFPrintCheckEntity>("PrintCheckIIFFiles ", param).ToList();
                if (result.Count > 0 )
                {
                    

                    DataTable dt = new DataTable();
                    dt = Common.CommonHelper.ToDataTable(result);
                    Aspose.Words.License license = new Aspose.Words.License();
                    license.SetLicense(HttpContext.Current.Server.MapPath("~/ProjectLicFiles/" + "Aspose.Words.lic"));
                    string fileName = "ChequeFormat.doc";
                    string path = HttpContext.Current.Server.MapPath("~/PrintCheck/" + fileName);

                    MemoryStream ms = new MemoryStream();

                    Aspose.Words.Document doc = new Aspose.Words.Document(path);
                   // doc.MailMerge.RemoveEmptyParagraphs = true;
                    doc.MailMerge.Execute(dt);
                    System.Collections.IEnumerator enumerator = dt.Rows.GetEnumerator();


                    //   doc.Save(ms,SaveFormat.Docx);

                    doc.Save(ms, Aspose.Words.SaveFormat.Pdf);

                    string PDFpath = HttpContext.Current.Server.MapPath("~/PrintCheck/");
                    string fileNames = DateTime.Now.ToString("yyyy-MM-dd");
                    string FullPathWithFileName = Path.Combine(PDFpath, fileNames + ".pdf");
                    if (System.IO.File.Exists(FullPathWithFileName))
                    {
                        System.IO.File.Delete(FullPathWithFileName);
                    }
                    FileStream file = new FileStream(FullPathWithFileName, FileMode.CreateNew, FileAccess.ReadWrite);
                    
                    ms.WriteTo(file);

                    file.Close();
                    ms.Close();

                    byte[] bytes;// = new byte[file.Length];
                    bytes = File.ReadAllBytes(FullPathWithFileName); //ms.ToArray();


                    using (FileStream stream = new FileStream(FullPathWithFileName, FileMode.Create))
                    {
                        stream.Write(bytes, 0, bytes.Length);
                    }

                    //Read the File into a Byte Array.

                    //Set the Response Content.
                    response.Content = new ByteArrayContent(bytes);

                    response.Content.Headers.Clear();
                    response.Content.Headers.Add("Content-Disposition", "attachment; filename=" + fileNames+ ".pdf");
                    response.Content.Headers.Add("Content-Length", bytes.Length.ToString());
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping("application/pdf"));

                   

                    //Set the Response Content Length.
                    //response.Content.Headers.ContentLength = bytes.LongLength;

                    //Set the Content Disposition Header Value and FileName.
                  //  response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                 //   response.Content.Headers.ContentDisposition.FileName = FullPathWithFileName;


                    //Set the File Content Type.
                   // response.Content.Headers.ContentType = "Application/pdf";// new MediaTypeHeaderValue(MimeMapping.GetMimeMapping(FullPathWithFileName));

                    return response;

                }



            }
            catch (Exception ex)
            {
                // response.Message.Add(ex.Message);
            }

            return response;
        }

        public static string FormatCSV(string input)
        {
            try
            {
                if (input == null)
                    return string.Empty;

                bool containsQuote = false;
                bool containsComma = false;
                int len = input.Length;
                for (int i = 0; i < len && (containsComma == false || containsQuote == false); i++)
                {
                    char ch = input[i];
                    if (ch == '"')
                        containsQuote = true;
                    else if (ch == ',')
                        containsComma = true;
                }

                if (containsQuote && containsComma)
                    input = input.Replace("\"", "\"\"");

                if (containsComma)
                    return "\"" + input + "\"";
                else
                    return input;
            }
            catch
            {
                throw;
            }
        }

        [HttpGet]
        [Route("GetIIFFileForDay")]
        public HttpResponseMessage GetIIFFileForDay(DateTime date,bool ToBePrint)
        {
            // var response = new ApiResponse<IIFFilesEntity>();
            // Create HTTP Response.
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
            try
            {
                SqlParameter[] param = {new SqlParameter("Date", (object)date ?? (object)DBNull.Value)
                                        ,new SqlParameter("ToBePrint", (object)ToBePrint ?? (object)DBNull.Value)};
                var result = _repository.ExecuteSQL<IIFFilesEntity>("GetIIfFileForDay", param).ToList();

                if (result == null)
                {
                    result = new List<IIFFilesEntity>();
                }

                MemoryStream memoryStream = new MemoryStream();
                StreamWriter sw = new StreamWriter(memoryStream);

                sw.WriteLine("\"!TRNS\"" + ",\"TRNSTYPE\"" + ",\"DATE\"" + ",\"ACCNT\"" + ",\"NAME\"" + ",\"AMOUNT\"" + ",\"DOCNUM\""
                        + ",\"TOPRINT\"" + ",\"ADDR1\"" + ",\"ADDR2\"" + ",\"ADDR3\"", "\r\n");

                sw.WriteLine("\"!SPL\"" + ",\"TRNSTYPE\"" + ",\"DATE\"" + ",\"ACCNT\"" + ",\"NAME\"" + ",\"AMOUNT\"", "\r\n");
                sw.WriteLine("\"!ENDTRNS\"");

                DataTable dt = new DataTable();
                int i;
                dt = Common.CommonHelper.ToDataTable(result);
                DataTable Dtable = null;
                if (dt.Rows.Count > 0)
                {
                    Dtable = dt.Copy();
                    Dtable.Columns.Add("TRNS").SetOrdinal(0);
                    Dtable.Columns.Add("CHECK").SetOrdinal(1);
                    Dtable.Columns.Remove("ChkID");
                    Dtable.Columns.Remove("Date");
                    Dtable.Columns.Remove("AcctNo");
                    Dtable.Columns.Remove("Secondname");
                    Dtable.Columns.Remove("checkamount");
                    foreach (DataRow iif in dt.Rows)
                    {

                        object[] array = iif.ItemArray;

                        string startiif = "\"TRNS\",\"CHECK\",";

                        string splitiif = "\"SPL\",\"CHECK\",";



                        string endiif = "\"ENDTRNS\"";


                        for (i = 0; i < iif.ItemArray.Length; i++)
                        {
                            if (i == 1)
                            {
                                sw.Write(startiif + "\"{0}\",", array[i].ToString());
                            }
                            else if (i == 10)
                            {
                                sw.Write(Environment.NewLine);
                                sw.Write(splitiif + "\"{0}\",", array[i].ToString());
                            }

                            else if (i == 13)
                            {

                                sw.Write("\"{0}\",", array[i].ToString());
                                sw.Write(Environment.NewLine);
                                sw.Write(endiif);
                            }


                            else if (i == 4)
                            {

                                sw.Write("\"-{0}\",", array[i].ToString());
                            }
                            else if (i == 0)
                            {
                                sw.Write("");
                            }
                            else
                            {

                                sw.Write("\"{0}\",", array[i].ToString());
                            }

                        }
                        sw.Write(Environment.NewLine);//display in next line
                    }
                }
                sw.Flush(); // added this line
                byte[] bytesInStream = memoryStream.ToArray(); // simpler way of converting to array        
                MemoryStream ms = new MemoryStream(bytesInStream);
                string path = HttpContext.Current.Server.MapPath(@"~/IIfFile");
                string fileName = DateTime.Now.ToString("yyyy-MM-ddhhmmss");
                string FullPathWithFileName = Path.Combine(path, fileName + ".iif");

                if (System.IO.File.Exists(FullPathWithFileName))
                {
                    System.IO.File.Delete(FullPathWithFileName);
                }
                FileStream file = new FileStream(FullPathWithFileName, FileMode.CreateNew, FileAccess.ReadWrite);
                ms.WriteTo(file);
                file.Close();





                //Set the File Path.
                string filePath = HttpContext.Current.Server.MapPath("~/IIFFile/") + fileName + ".iif";

                //Check whether File exists.
                if (!File.Exists(filePath))
                {
                    //Throw 404 (Not Found) exception if File not found.
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.ReasonPhrase = string.Format("File not found: {0} .", fileName);
                    throw new HttpResponseException(response);
                }

                //Read the File into a Byte Array.
                byte[] bytes = File.ReadAllBytes(filePath);

                //Set the Response Content.
                response.Content = new ByteArrayContent(bytes);

                //Set the Response Content Length.
                response.Content.Headers.ContentLength = bytes.LongLength;

                //Set the Content Disposition Header Value and FileName.
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                response.Content.Headers.ContentDisposition.FileName = fileName;

                //Set the File Content Type.
                response.Content.Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping(fileName));

                return response;
            }
            catch (Exception ex)
            {
                // response.Message.Add(ex.Message);
            }
            return response;
            // return response;
        }

        [HttpGet]
        [Route("GetIIFFileForDayCSV")]
        public HttpResponseMessage GetIIFFileForDayCSV(DateTime date)
        {
            // var response = new ApiResponse<IIFFilesEntity>();
            // Create HTTP Response.
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
            try
            {
                SqlParameter[] param = { new SqlParameter("Date", (object)date ?? (object)DBNull.Value)
                                        };
                var result = _repository.ExecuteSQL<IIFFilesEntity>("GetIIfFileForDay", param).ToList();

                if (result == null)
                {
                    result = new List<IIFFilesEntity>();
                }

                MemoryStream memoryStream = new MemoryStream();
                StreamWriter sw = new StreamWriter(memoryStream);

                

                DataTable dt = new DataTable();
                int i;
                dt = Common.CommonHelper.ToDataTable(result);
                DataTable Dtable = null;
                if (dt.Rows.Count > 0)
                {
                    Dtable = dt.Copy();
                    Dtable.Columns.Add("TRNS").SetOrdinal(0);
                    Dtable.Columns.Add("CHECK").SetOrdinal(1);
                    Dtable.Columns.Remove("ChkID");
                    Dtable.Columns.Remove("Date");
                    Dtable.Columns.Remove("AcctNo");
                    Dtable.Columns.Remove("Secondname");
                    Dtable.Columns.Remove("checkamount");                    
                }

                string CSVFileName = "csv_" + DateTime.Now.ToString("yyyy-MM-ddhhmmdd");
                string path = HttpContext.Current.Server.MapPath(@"~/IIfFile");
                string CSVPath = Path.Combine(path, CSVFileName + ".csv");

                StringBuilder sb = new StringBuilder();

                
                foreach (DataRow dr in Dtable.Rows)
                {
                    foreach (DataColumn dc in Dtable.Columns)
                    {
                        if (dc.ColumnName == "TRNS")
                        {
                            sb.Append(FormatCSV("TRNS") + ",");
                        }
                        else if (dc.ColumnName == "CHECK")
                        {
                            sb.Append(FormatCSV("CHECK") + ",");
                        }
                        else
                        {
                            sb.Append(FormatCSV(dr[dc.ColumnName].ToString()) + ",");
                        }
                    }
                    sb.Remove(sb.Length - 1, 1);
                    sw.WriteLine(sb.ToString());
                    sb.Clear();
                    // sb.AppendLine();
                }
                sw.Flush();
                byte[] bytesInStream = memoryStream.ToArray(); // simpler way of converting to array        
                
                MemoryStream ms = new MemoryStream(bytesInStream);

                FileStream file = new FileStream(CSVPath, FileMode.CreateNew, FileAccess.ReadWrite);
                ms.WriteTo(file);

                //StreamWriter swriter = new StreamWriter(CSVPath, false);
                //swriter.Write(sb);
                //sb.Clear();
                //swriter.Close();
                //swriter.Dispose();
                file.Close();
                ms.Close();


                //Set the File Path.
                string filePath = HttpContext.Current.Server.MapPath("~/IIFFile/") + CSVFileName + ".csv";

                //Check whether File exists.
                if (!File.Exists(filePath))
                {
                    //Throw 404 (Not Found) exception if File not found.
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.ReasonPhrase = string.Format("File not found: {0} .", CSVFileName);
                    throw new HttpResponseException(response);
                }

                //Read the File into a Byte Array.
                byte[] bytes = File.ReadAllBytes(filePath);

                //Set the Response Content.
                response.Content = new ByteArrayContent(bytes);

                //Set the Response Content Length.
                response.Content.Headers.ContentLength = bytes.LongLength;

                //Set the Content Disposition Header Value and FileName.
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                response.Content.Headers.ContentDisposition.FileName = CSVFileName;

                //Set the File Content Type.
                response.Content.Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping(CSVFileName));

                return response;
            }
            catch (Exception ex)
            {
                // response.Message.Add(ex.Message);
            }
            return response;
            // return response;
        }

        #endregion



    }
}