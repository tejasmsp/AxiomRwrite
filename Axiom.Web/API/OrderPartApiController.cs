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
using Axiom.Web.EmailHelper;
using System.Web;
using System.IO;
using System.Net.Http.Headers;
using System.Data;

namespace Axiom.Web.API
{
    [RoutePrefix("api")]
    public class OrderPartApiController : ApiController
    {
        #region Initialization
        private readonly GenericRepository<OrderPartEntity> _repository = new GenericRepository<OrderPartEntity>();
        #endregion

        #region Method
        [HttpGet]
        [Route("GetPartListByOrderId")]
        public ApiResponse<OrderPartEntity> GetPartListByOrderId(long OrderId = 0, int PartNo = 0)
        {
            var response = new ApiResponse<OrderPartEntity>();

            try
            {
                SqlParameter[] param = {
                    new SqlParameter("orderId", (object)OrderId ?? (object)DBNull.Value)
                    ,new SqlParameter("partNo", (object)PartNo ?? (object)DBNull.Value)
                };
                var result = _repository.ExecuteSQL<OrderPartEntity>("GetPartListByOrderId", param).ToList();

                if (result == null)
                {
                    result = new List<OrderPartEntity>();
                }

                response.Success = true;
                response.Data = result;
            }
            catch (Exception ex)
            {
                response.Message.Add(ex.Message);
            }

            return response;

        }

        [HttpGet]
        [Route("GetLocationByLocIDForPart")]
        public ApiResponse<OrderPartEntity> GetLocationByLocIDForPart(string LocationId)
        {
            var response = new ApiResponse<OrderPartEntity>();
            try
            {
                SqlParameter[] param = { new SqlParameter("LocationId", (object)LocationId ?? (object)DBNull.Value) };
                var result = _repository.ExecuteSQL<OrderPartEntity>("GetLocationById", param).ToList();
                if (result == null)
                {
                    result = new List<OrderPartEntity>();
                }
                response.Success = true;
                response.Data = result;
            }
            catch (Exception ex)
            {
                response.Message.Add(ex.Message);
            }
            return response;
        }

        [HttpPost]
        [Route("InsertOrderPart")]
        public ApiResponse<long> InsertOrderPart(OrderPartEntity model)
        {
            var response = new ApiResponse<long>();

            try
            {
                int IsAdmin = model.RoleName == "Administrator" ? 1 : 0;
                SqlParameter[] param = {
                    new SqlParameter("OrderId", (object)model.OrderId ?? (object)DBNull.Value)
                    ,new SqlParameter("LocID", (object)model.LocID ?? (object)DBNull.Value)
                    ,new SqlParameter("Contact", (object)model.Contact ?? (object)DBNull.Value)
                    ,new SqlParameter("RecType", (object)model.RecordTypeId ?? (object)DBNull.Value)
                    ,new SqlParameter("Notes", (object)model.Notes ?? (object)DBNull.Value)
                    ,new SqlParameter("Scope", (object)model.Scope ?? (object)DBNull.Value)
                    ,new SqlParameter("CreatedBy", (object)model.EmpId ?? (object)DBNull.Value)
                    ,new SqlParameter("IsAdmin",(object) IsAdmin  ?? (object)DBNull.Value)
                };

                var result = _repository.ExecuteSQL<long>("InsertOrderPart", param).FirstOrDefault();

                if (result > 0)
                {
                    response.Success = true;
                    response.str_ResponseData = result.ToString();
                }



            }
            catch (Exception ex)
            {
                response.Message.Add(ex.Message);
            }

            return response;

        }
        [HttpPost]
        [Route("UpdateOrderPart")]
        public BaseApiResponse UpdateOrderPart(OrderPartEntity model)
        {
            var response = new BaseApiResponse();

            try
            {
                SqlParameter[] param = {
                    new SqlParameter("OrderId", (object)model.OrderId ?? (object)DBNull.Value)
                    ,new SqlParameter("PartNo", (object)model.PartNo ?? (object)DBNull.Value)
                    ,new SqlParameter("LocID", (object)model.LocID ?? (object)DBNull.Value)
                    ,new SqlParameter("RecType", (object)model.RecordTypeId ?? (object)DBNull.Value)
                    ,new SqlParameter("Notes", (object)model.Notes ?? (object)DBNull.Value)
                    ,new SqlParameter("Scope", (object)model.Scope ?? (object)DBNull.Value)
                    ,new SqlParameter("CreatedBy", (object)model.EmpId ?? (object)DBNull.Value)
                };
                var result = _repository.ExecuteSQL<int>("UpdateOrderPart", param).FirstOrDefault();

                if (result == 1)
                {
                    response.Success = true;
                    response.str_ResponseData = result.ToString();
                }
            }
            catch (Exception ex)
            {
                response.Message.Add(ex.Message);
            }

            return response;

        }
        [HttpPost]
        [Route("AddUpdateChronolgy")]
        public BaseApiResponse AddUpdateChronolgy(int OrderId, int PartNo, string userGuid, bool IsChronology)
        {
            var response = new BaseApiResponse();

            try
            {
                Guid CreatedByGuid = new Guid(userGuid);
                SqlParameter[] param = {
                    new SqlParameter("OrderId", (object)OrderId ?? (object)DBNull.Value)
                    ,new SqlParameter("PartNo", (object)PartNo ?? (object)DBNull.Value)
                    ,new SqlParameter("CreatedBy", (object)CreatedByGuid?? (object)DBNull.Value)
                    ,new SqlParameter("IsChronology", (object)IsChronology?? (object)DBNull.Value)
                };
                var result = _repository.ExecuteSQL<bool>("AddUpdateChronolgy", param).FirstOrDefault();
                response.str_ResponseData = result.ToString();
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message.Add(ex.Message);
            }

            return response;

        }
        [HttpGet]
        [Route("GetLocationFees")]
        public ApiResponse<OrderPartLocationFees> GetLocationFees(int OrderNo, int PartNo)
        {
            var response = new ApiResponse<OrderPartLocationFees>();
            try
            {
                SqlParameter[] param = { new SqlParameter("OrderNo", (object)OrderNo ?? (object)DBNull.Value),
                new SqlParameter("PartNo", (object)PartNo ?? (object)DBNull.Value)};
                var result = _repository.ExecuteSQL<OrderPartLocationFees>("GetLocationFees", param).ToList();
                if (result == null)
                {
                    result = new List<OrderPartLocationFees>();
                }
                response.Success = true;
                response.Data = result;
            }
            catch (Exception ex)
            {
                response.Message.Add(ex.Message);
            }
            return response;
        }
        [HttpPost]
        [Route("InsertUpdateLocationFeesChecks")]
        public BaseApiResponse InsertUpdateLocationFeesChecks(OrderPartLocationFees model)
        {
            var response = new BaseApiResponse();

            try
            {
                SqlParameter[] param = {
                    new SqlParameter("ChkID", (object)model.ChkID ?? (object)DBNull.Value)
                    ,new SqlParameter("CheckNo", (object)model.ChkNo==null?"":model.ChkNo ?? (object)DBNull.Value)
                    ,new SqlParameter("ChkAmt", (object)model.Amount ?? (object)DBNull.Value)
                    ,new SqlParameter("ChngBy", (object)model.ChngBy ?? (object)DBNull.Value)
                    ,new SqlParameter("FirmID", (object)model.FirmID==null?"":model.FirmID ?? (object)DBNull.Value)
                    ,new SqlParameter("Memo", (object)model.Memo==null?"":model.Memo ?? (object)DBNull.Value)
                    ,new SqlParameter("OrderNo", (object)model.OrderNo ?? (object)DBNull.Value)
                    ,new SqlParameter("PartNo", (object)model.PartNo ?? (object)DBNull.Value)
                    ,new SqlParameter("ChkDate", (object)model.IssueDate ?? (object)DBNull.Value)
                    ,new SqlParameter("ToBePrint", (object)model.ToBePrint ?? (object)DBNull.Value)

                };
                var result = _repository.ExecuteSQL<int>("InsertUpdateLocationFeesChecks", param).FirstOrDefault();
                if (result > 0)
                {
                    response.Success = true;
                }
            }
            catch (Exception ex)
            {
                response.Message.Add(ex.Message);
            }

            return response;

        }
        [HttpPost]
        [Route("UpdateLocationFeesVoidChecks")]
        public BaseApiResponse UpdateLocationFeesVoidChecks(int ChkID)
        {
            var response = new BaseApiResponse();

            try
            {
                SqlParameter[] param = {
                    new SqlParameter("ChkID", (object)ChkID ?? (object)DBNull.Value)
                };
                var result = _repository.ExecuteSQL<int>("UpdateLocationFeesVoidChecks", param).FirstOrDefault();
                if (result > 0)
                {
                    response.Success = true;
                }
            }
            catch (Exception ex)
            {
                response.Message.Add(ex.Message);
            }

            return response;

        }
        [HttpPost]
        [Route("DeleteLocationFeesChecks")]
        public BaseApiResponse DeleteLocationFeesChecks(int ChkID)
        {
            var response = new BaseApiResponse();

            try
            {
                SqlParameter[] param = {
                    new SqlParameter("ChkID", (object)ChkID ?? (object)DBNull.Value)
                };
                var result = _repository.ExecuteSQL<int>("DeleteLocationFeesChecks", param).FirstOrDefault();
                if (result > 0)
                {
                    response.Success = true;
                }
            }
            catch (Exception ex)
            {
                response.Message.Add(ex.Message);
            }

            return response;

        }

        [HttpPost]
        [Route("GetPrintCheckIIFFiles")]
        public HttpResponseMessage GetPrintCheckIIFFiles(DateTime fromDate, DateTime toDate, string checkNo, string checkId)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);

            try
            {
                SqlParameter[] param = { new SqlParameter("dtFrom", (object) fromDate.ToString("MM/dd/yyyy") ?? (object)DBNull.Value),
                   new SqlParameter("dtTo", (object)toDate.ToString("MM/dd/yyyy") ?? (object)DBNull.Value)
                };
                var result = _repository.ExecuteSQL<LocationFeeCheckIIF>("GetLocationFeeCheckIIF", param).ToList();
                if (!string.IsNullOrEmpty(checkId) && !string.IsNullOrEmpty(checkNo))
                {
                    int chkno = Convert.ToInt32(checkNo);
                    string[] s = checkId.Split(',');
                    for (int i = 0; i < s.Length; i++)
                    {
                        UpdateCheckNo(Convert.ToInt32(s[i]), chkno);
                        chkno = chkno + 1;
                    }
                }
                if (!string.IsNullOrEmpty(checkId))
                {
                    var chkIdList = checkId.Split(',').Select(n => Convert.ToInt64(n)).ToArray();
                    result = result.Where(x => chkIdList.Contains(x.ChkID)).ToList();
                }
                else
                {
                    List<string> list = result.Select(x => string.IsNullOrEmpty(x.ChkNo) ? "0" : x.ChkNo).ToList();
                    string[] greatervalue = list.Where(x => Convert.ToInt64(x) >= Convert.ToInt64(checkNo)).OrderByDescending(x => x).ToArray();
                    result = result.Where(x => greatervalue.Contains(x.ChkNo)).OrderBy(x => x.OrderNo).ThenBy(x => x.PartNo).ToList();
                }

                var dt = Common.CommonHelper.ToDataTable(result);
                foreach (DataRow dr in dt.Rows)
                {
                    dr["AmountInWords"] = NumWords.FormatAmount(NumWords.Convert(Convert.ToDecimal(dr["ChkAmt"])));
                }
                Aspose.Words.License license = new Aspose.Words.License();
                license.SetLicense("Aspose.Words.lic");
                string fileName = "ChequeFormat.doc";
                string path = HttpContext.Current.Server.MapPath("~/PrintFolder/" + fileName);
                MemoryStream ms = new MemoryStream();
                Aspose.Words.Document doc = new Aspose.Words.Document(path);
                doc.MailMerge.Execute(dt);
                System.Collections.IEnumerator enumerator = dt.Rows.GetEnumerator();
                doc.Save(ms, Aspose.Words.SaveFormat.Pdf);

                string PDFpath = HttpContext.Current.Server.MapPath("~/PrintCheck/");
                string fileNames = DateTime.Now.ToString("yyyy-MM-dd");
                string FullPathWithFileName = Path.Combine(PDFpath, fileNames + ".pdf");
                if (File.Exists(FullPathWithFileName))
                    File.Delete(FullPathWithFileName);

                FileStream file = new FileStream(FullPathWithFileName, FileMode.CreateNew, FileAccess.ReadWrite);
                ms.WriteTo(file);
                file.Close();
                ms.Close();

                byte[] bytes = File.ReadAllBytes(FullPathWithFileName);
                using (FileStream stream = new FileStream(FullPathWithFileName, FileMode.Create))
                {
                    stream.Write(bytes, 0, bytes.Length);
                }
                response.Content = new ByteArrayContent(bytes);

                response.Content.Headers.Clear();
                response.Content.Headers.Add("Content-Disposition", "attachment; filename=" + fileNames + ".pdf");
                response.Content.Headers.Add("Content-Length", bytes.Length.ToString());
                response.Content.Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping("application/pdf"));

            }
            catch (Exception ex)
            {


            }
            return response;
        }

        private int UpdateCheckNo(int ChkID, int ChkNo)
        {
            SqlParameter[] param = {
                    new SqlParameter("ChkID", (object)ChkID ?? (object)DBNull.Value),
                    new SqlParameter("ChkNo", (object)ChkNo ?? (object)DBNull.Value)
                };
            return _repository.ExecuteSQL<int>("UpdateCheckIIFFiles", param).FirstOrDefault();

        }

        [HttpPost]
        [Route("GetGenerateIIFFiles")]
        public HttpResponseMessage GetGenerateIIFFiles(DateTime fromDate, DateTime toDate, string checkNo, string checkId)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
            try
            {
                SqlParameter[] param = { new SqlParameter("dtFrom", (object) fromDate.ToString("MM/dd/yyyy") ?? (object)DBNull.Value),
                   new SqlParameter("dtTo", (object)toDate.ToString("MM/dd/yyyy") ?? (object)DBNull.Value)
                };
                var result = _repository.ExecuteSQL<LocationFeeCheckIIF>("GetLocationFeeCheckIIF", param).ToList();
                if (!string.IsNullOrEmpty(checkId))
                {
                    var chkIdList = checkId.Split(',').Select(x => Convert.ToInt64(x)).ToArray();
                    result = result.Where(x => chkIdList.Contains(x.ChkID)).ToList();
                }
                else
                {
                    List<string> list = result.Select(x => string.IsNullOrEmpty(x.ChkNo) ? "0" : x.ChkNo).ToList();
                    string[] greatervalue = list.Where(x => Convert.ToInt64(x) >= Convert.ToInt64(checkNo)).OrderByDescending(x => x).ToArray();
                    result = result.Where(x => greatervalue.Contains(x.ChkNo)).ToList();
                }
                MemoryStream memoryStream = new MemoryStream();
                StreamWriter sw = new StreamWriter(memoryStream);
                int i;
                sw.WriteLine("\"!TRNS\"" + ",\"TRNSTYPE\"" + ",\"DATE\"" + ",\"ACCNT\"" + ",\"NAME\"" + ",\"AMOUNT\"" + ",\"DOCNUM\""
                    + ",\"TOPRINT\"" + ",\"ADDR1\"" + ",\"ADDR2\"" + ",\"ADDR3\"", "\r\n");
                sw.WriteLine("\"!SPL\"" + ",\"TRNSTYPE\"" + ",\"DATE\"" + ",\"ACCNT\"" + ",\"NAME\"" + ",\"AMOUNT\"", "\r\n");
                sw.WriteLine("\"!ENDTRNS\"");
                DataTable dt = Common.CommonHelper.ToDataTable(result);
                foreach (DataRow iif in dt.Rows)
                {
                    object[] array = iif.ItemArray;
                    string startiif = "\"TRNS\",\"CHECK\",";
                    string splitiif = "\"SPL\",\"CHECK\",";
                    string endiif = "\"ENDTRNS\"";
                    for (i = 0; i < iif.ItemArray.Length; i++)
                    {
                        if (i == 1)
                            sw.Write(startiif + "\"{0}\",", array[i].ToString());
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
                            sw.Write("\"-{0}\",", array[i].ToString());
                        else if (i == 0)
                            sw.Write("");
                        else
                            sw.Write("\"{0}\",", array[i].ToString());
                    }
                    sw.Write(Environment.NewLine);
                }
                sw.Flush();
                memoryStream.Seek(0, SeekOrigin.Begin);
                byte[] bytesInStream = memoryStream.ToArray();

                MemoryStream ms = new MemoryStream(bytesInStream);
                string path = HttpContext.Current.Server.MapPath(@"~/IIFFile");
                string fileName = DateTime.Now.ToString("yyyy-MM-dd");
                string FullPathWithFileName = Path.Combine(path, fileName + ".iif");
                if (File.Exists(FullPathWithFileName))
                    File.Delete(FullPathWithFileName);
                FileStream file = new FileStream(FullPathWithFileName, FileMode.CreateNew, FileAccess.ReadWrite);
                ms.WriteTo(file);
                file.Close();
                ms.Close();
                byte[] bytes = File.ReadAllBytes(FullPathWithFileName);
                using (FileStream stream = new FileStream(FullPathWithFileName, FileMode.Create))
                {
                    stream.Write(bytes, 0, bytes.Length);
                }
                response.Content = new ByteArrayContent(bytes);

                response.Content.Headers.Clear();
                response.Content.Headers.Add("Content-Disposition", "attachment; filename=" + fileName + ".iif");
                response.Content.Headers.Add("Content-Length", bytes.Length.ToString());
                response.Content.Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping("application/octet-stream"));

            }
            catch (Exception ex)
            {

            }
            return response;

        }

        [HttpPost]
        [Route("CancelPartSendEmail")]
        public BaseApiResponse CancelPartSendEmail(string OrderID, string csvPartNo, string userGuid, int companyNo)
        {
            var response = new BaseApiResponse();

            try
            {
                string accExecutiveName = string.Empty;
                string accExecutiveEmail = string.Empty;

                Guid CreatedByGuid = new Guid(userGuid);
                SqlParameter[] param = {
                    new SqlParameter("OrderID", (object)OrderID ?? (object)DBNull.Value)
                    ,new SqlParameter("csvPartNo", (object)csvPartNo ?? (object)DBNull.Value)
                    ,new SqlParameter("UserId", (object)userGuid?? (object)DBNull.Value)
                };

                var locationInfo = _repository.ExecuteSQL<CancelPartSendEmailEntity>("GetCancelPartEmailInfo", param).ToList();
                //response.str_ResponseData = result.ToString();

                if (locationInfo != null && locationInfo.Any())
                {
                    accExecutiveName = locationInfo.Select(x => x.AccExecutiveName).FirstOrDefault();
                    accExecutiveEmail = locationInfo.Select(x => x.AccExecutiveEmail).FirstOrDefault();
                }
                else
                {
                    accExecutiveName = "Leah Boroski";
                    accExecutiveEmail = "leah.boroski@axiomcopy.com";
                }

                System.Text.StringBuilder htmlBody = new System.Text.StringBuilder();

                CompanyDetailForEmailEntity objCompany = CommonFunction.CompanyDetailForEmail(companyNo);
                string htmlfilePath = AppDomain.CurrentDomain.BaseDirectory + "MailTemplate\\CancelPart.html";
                using (System.IO.StreamReader reader = new System.IO.StreamReader((htmlfilePath)))
                {
                    htmlBody.Append(reader.ReadToEnd());
                }

                htmlBody.Replace("{USERNAME}", accExecutiveName);
                htmlBody.Replace("{ORDERNO}", OrderID);

                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                foreach (var loc in locationInfo)
                {
                    string strLink = string.Format(System.Web.HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + "/PartDetail?OrderId={0}&PartNo={1}", OrderID, loc.PartNo);// "https://www.axiomcopyonline.com/Clients/Secured/Internal/PartDetail.aspx?o=" + OrderID + "&p=" + loc.PartNo.ToString();
                    sb.Append("<tr><td width='50'  style='text-align:center;'>" + loc.PartNo.ToString() + "</td>");
                    sb.Append("<td>" + loc.Name1 + "  (" + loc.LocID + ") </td>");
                    sb.Append(@"<td>&nbsp;&nbsp;<a style='color:#9b272a; text-decoration:none !important;' title='Click here to view part detail' target='_blank' href='" + strLink + "'>View Detail</a>&nbsp;&nbsp;</td></tr>");
                }

                htmlBody.Replace("{LOCATION}", sb.ToString());
                htmlBody.Replace("{LogoURL}", objCompany.Logopath);
                htmlBody.Replace("{ThankYou}", objCompany.ThankYouMessage);
                htmlBody.Replace("{CompanyName}", objCompany.CompName);
                htmlBody.Replace("{Link}", objCompany.SiteURL);

                //AxiomEmail.SendMailBilling("Cancel Part Reqeust", htmlBody.ToString(), accExecutiveEmail, true, "axiomsupport@axiomcopy.com", null, "tejaspadia@gmail.com,autoemail");
                Email.Send(accExecutiveEmail, htmlBody.ToString(), "Cancel Part Reqeust", "autharchive@axiomcopy.com", "tejaspadia@gmail.com");
                
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message.Add(ex.Message);
            }
            return response;
        }

        public class CancelPartSendEmailEntity
        {
            public string Name1 { get; set; }
            public string Name2 { get; set; }
            public int PartNo { get; set; }
            public string LocID { get; set; }
            public string AccExecutiveName { get; set; }
            public string AccExecutiveEmail { get; set; }
        }
        #endregion
    }
}
