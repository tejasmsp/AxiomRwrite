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
using System.Net.Http.Headers;
using System.Web;

namespace Axiom.Web.API
{
    [RoutePrefix("api")]
    public class BatchInvoiceApiController : ApiController
    {
        #region Initialization
        private readonly GenericRepository<SSNSettings> _repository = new GenericRepository<SSNSettings>();
        #endregion

        #region DatabaseOperations
        [HttpGet]
        [Route("InvoiceBatchCheckLED")]
        public BaseApiResponse InvoiceBatchCheckLED(string FirmID)
        {
            var response = new BaseApiResponse();

            try
            {
                SqlParameter[] param = { new SqlParameter("FirmID", (object)FirmID ?? (object)DBNull.Value)
                                        };
                var result = _repository.ExecuteSQL<int>("InvoiceBatchCheckLED", param).FirstOrDefault();

                if (result == null)
                {
                    result = 0;
                }

                response.Success = true;
                response.str_ResponseData = Convert.ToBoolean(result) ? "true" : "false";
            }
            catch (Exception ex)
            {
                response.Message.Add(ex.Message);
            }
            return response;
        }

        [HttpGet]
        [Route("GenerateLEDESFile")]
        public HttpResponseMessage GenerateLEDESFile(string FirmID, string Caption, string ClaimNo, string AttyID, string FromDate, string ToDate, string InvNo, string SoldAtty)
        {

            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
            

            try
            {
                SqlParameter[] param = { new SqlParameter("FirmID", (object)(string.IsNullOrEmpty(FirmID) ? "":FirmID) ?? (object)DBNull.Value)
                        ,new SqlParameter("Caption", (object)(string.IsNullOrEmpty(Caption) ? "":Caption) ?? (object)DBNull.Value)
                        ,new SqlParameter("ClaimNo", (object)(string.IsNullOrEmpty(ClaimNo) ? "" : ClaimNo) ?? (object)DBNull.Value)
                        ,new SqlParameter("AttyID", (object)(string.IsNullOrEmpty(AttyID) ? "" : AttyID) ?? (object)DBNull.Value)
                        ,new SqlParameter("FromDate", (object)(string.IsNullOrEmpty(FromDate) ? "" : FromDate) ?? (object)DBNull.Value)
                        ,new SqlParameter("ToDate", (object)(string.IsNullOrEmpty(ToDate) ? "" : ToDate) ?? (object)DBNull.Value)
                        ,new SqlParameter("InvNo", (object)(string.IsNullOrEmpty(InvNo) ? "" : InvNo) ?? (object)DBNull.Value)
                        ,new SqlParameter("SoldAtty", (object)(string.IsNullOrEmpty(SoldAtty) ? "" : SoldAtty) ?? (object)DBNull.Value)
                };
                var result = _repository.ExecuteSQL<BatchInvoiceEntity>("GenerateLEDESFile", param).ToList();
                // var result = _repository.ExecuteSQL<AttorneyUsersEntity>("AttorneyUserGetList").ToList();
                if (result == null)
                {
                    result = new List<BatchInvoiceEntity>();
                }


                MemoryStream memoryStream = new MemoryStream();
                StreamWriter sw = new StreamWriter(memoryStream);
                sw.WriteLine("LEDES1998B[]");
                sw.WriteLine("INVOICE_DATE|INVOICE_NUMBER|CLIENT_ID|LAW_FIRM_MATTER_ID|INVOICE_TOTAL|BILLING_START_DATE|BILLING_END_DATE|INVOICE_DESCRIPTION|LINE_ITEM_NUMBER|EXP/FEE/INV_ADJ_TYPE|LINE_ITEM_NUMBER_OF_UNITS|LINE_ITEM_ADJUSTMENT_AMOUNT|LINE_ITEM_TOTAL|LINE_ITEM_DATE|LINE_ITEM_TASK_CODE|LINE_ITEM_EXPENSE_CODE|LINE_ITEM_ACTIVITY_CODE|TIMEKEEPER_ID|LINE_ITEM_DESCRIPTION|LAW_FIRM_ID|LINE_ITEM_UNIT_COST|TIMEKEEPER_NAME|TIMEKEEPER_CLASSIFICATION|CLIENT_MATTER_ID[]");
                int tempInvoice = 0;
                int invoice = 0;
                int counter = 1;

                foreach (BatchInvoiceEntity item in result)
                {
                    tempInvoice = Convert.ToInt32(item.InvNo);
                    if (tempInvoice != invoice)
                    {
                        counter = 1;
                    }
                    sw.Write(item.InvDate + '|' + item.InvNo + '|' + item.ClientID.Trim() + '|' + item.ClaimNo.Trim() + '|' + item.Balance + '|' + item.OrdDate + '|' + item.InvDate1 + '|' + item.InvDesc + '|' + counter.ToString() + '|' + item.Exp_Fees + '|' + item.Pages + '|' + item.AdjustmentAmnt + '|' + item.TotalAmnt + '|' + item.InvDate2 + '|' + item.TaskCode + '|' + Convert.ToString(item.ExpenseCode) + '|' + item.ActivityCode + '|' + item.TimeKeeperID + '|' + item.Description.Trim() + '|' + item.FirmID.Trim() + '|' + item.CopyRate + '|' + item.TimeKeeperName + '|' + item.Classification + '|' + item.ClientmatterID.Trim());
                    sw.Write("[]");
                    sw.Write(Environment.NewLine);
                    invoice = Convert.ToInt32(item.InvNo);
                    counter++;
                }
                sw.Flush();
                byte[] bytesInStream = memoryStream.ToArray();
                memoryStream.Close();

                string fileName = DateTime.Now.ToString("MMddyyyyss");

                response.Content = new ByteArrayContent(bytesInStream);

                response.Content.Headers.Clear();
                
                response.Content.Headers.Add("Content-Disposition", "attachment; filename=" + fileName + ".txt");
                response.Content.Headers.Add("Content-Length", bytesInStream.Length.ToString());
                response.Content.Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping("application/text"));

                
            }
            catch (Exception ex)
            {
                
            }

            return response;

        }


        #endregion
    }
}