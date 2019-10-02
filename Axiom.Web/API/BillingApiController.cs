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
using System.Security.Cryptography;
using System.Net.Http.Headers;
using AXIOM.Common;
using System.Text;

namespace Axiom.Web.API
{
    [RoutePrefix("api")]
    public class WindowServiceApiController : ApiController
    {
        #region Initialization
        private readonly GenericRepository<SSNSettings> _repository = new GenericRepository<SSNSettings>();
        #endregion


        [HttpGet]
        [Route("QuickFormGetRecords")]
        public ApiResponse<PrintInvoiceEmailBill> QuickFormGetRecords(string AttyID, string InvoiceNumber, string OrderNumber, string PartNumber, string Location, string LocationName, string Patient)
        {
            var response = new ApiResponse<PrintInvoiceEmailBill>();
            

            try
            {
                SqlParameter[] param = {
                    new SqlParameter("AttyID", (object)AttyID ?? (object)DBNull.Value)
                    ,new SqlParameter("InvoiceNumber", (object)InvoiceNumber ?? (object)DBNull.Value)
                    ,new SqlParameter("OrderNumber", (object)OrderNumber ?? (object)DBNull.Value)
                };

                var result = _repository.ExecuteSQL<PrintInvoiceEmailBill>("InvoiceGetEmailList", param).ToList();

                var OrderDetail = _repository.ExecuteSQL<PrintInvoiceEmailBillOrderDetail>("InvoiceEmailOrderDetail", new SqlParameter("InvoiceNumber", (object)InvoiceNumber ?? (object)DBNull.Value)).ToList();
                
                if (result == null)
                {
                    result = new List<PrintInvoiceEmailBill>();
                }

                foreach (var item in result)
                {
                    if (item.Type != "Attorney")
                        continue;
                    string subject = "Your Records Are Available";
                    string Sendto = item.Email;

                    System.Text.StringBuilder body = new System.Text.StringBuilder();
                    using (System.IO.StreamReader reader = new System.IO.StreamReader(HttpContext.Current.Server.MapPath("~/MailTemplate/BillingRecords.html")))
                    {
                        body.Append(reader.ReadToEnd());
                    }

                    body = body.Replace("{UserName}", "Hello " + item.FirstName.Trim() + " " + item.LastName.Trim() + ",");
                    body = body.Replace("{LOCATION}", LocationName + " (" + Location + ")");
                    body = body.Replace("{PATIENT}", Patient);
                    body = body.Replace("{CLAIMNO}", Convert.ToString(OrderDetail[0].BillingClaimNo));
                    body = body.Replace("{ORDERNO}", OrderNumber + "-" + PartNumber);
                    body = body.Replace("{InvHdr}", Convert.ToString(OrderDetail[0].InvHdr));
                    body = body.Replace("{Pages}", Convert.ToString(OrderDetail[0].Pages));
                    body = body.Replace("{LINK}", "o=" + OrderNumber + "&p=" + PartNumber);

                    // EmailHelper.Email.Send(item.Email, body.ToString(), subject, "", "j.alspaugh@axiomcopy.com", "tejas.p@cementdigital.com");
                    EmailHelper.Email.Send(mailTo:item.Email,body: body.ToString(),subject: subject,ccMail: "j.alspaugh@axiomcopy.com", bccMail: "tejaspadia@gmail.com");
                }

                foreach (var item in result)
                {
                    if (item.Type == "Attorney")
                        continue;
                    string subject = "Your Records Are Available";
                    string Sendto = item.Email;

                    System.Text.StringBuilder body = new System.Text.StringBuilder();
                    using (System.IO.StreamReader reader = new System.IO.StreamReader(HttpContext.Current.Server.MapPath("~/MailTemplate/BillingRecords.html")))
                    {
                        body.Append(reader.ReadToEnd());
                    }

                    body = body.Replace("{UserName}", "Hello " + item.FirstName.Trim() + " " + item.LastName.Trim() + ",");
                    body = body.Replace("{LOCATION}", LocationName + " (" + Location + ")");
                    body = body.Replace("{PATIENT}", Patient);
                    body = body.Replace("{CLAIMNO}", Convert.ToString(OrderDetail[0].BillingClaimNo));
                    body = body.Replace("{ORDERNO}", OrderNumber + "-" + PartNumber);
                    body = body.Replace("{InvHdr}", Convert.ToString(OrderDetail[0].InvHdr));
                    body = body.Replace("{Pages}", Convert.ToString(OrderDetail[0].Pages));
                    body = body.Replace("{LINK}", "o=" + OrderNumber + "&p=" + PartNumber);

                    // EmailHelper.Email.Send(item.Email, body.ToString(), subject, "", "j.alspaugh@axiomcopy.com", "tejas.p@cementdigital.com");
                    EmailHelper.Email.Send(item.Email, body.ToString(), subject, "", "tejaspadia@gmail.com");
                }

                response.Success = true;
                response.Data = result;
            }
            catch (Exception ex)
            {

            }
            return response;
        }

        [HttpGet]
        [Route("GetInvMsg")]
        public ApiResponse<InvMsgEntity> GetInvMsg()
        {
            var response = new ApiResponse<InvMsgEntity>();
            try
            {           
                var result = _repository.ExecuteSQL<InvMsgEntity>("GetInvMsg").ToList();

                if (result == null)
                {
                    result = new List<InvMsgEntity>();
                }
                else
                {
                    response.Data = result;
                }
                response.Success = true;                
            }
            catch (Exception ex)
            {
                response.Message.Add(ex.Message);
            }
            return response;
        }

    }    
}