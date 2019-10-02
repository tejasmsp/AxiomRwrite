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
using System.Web;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;
using NReco.PdfGenerator;
using System.Net.Http.Headers;

namespace Axiom.Web.API
{

    public class PostDataEntity
    {
        public string htmlString { get; set; }
    }

    [RoutePrefix("api")]
    public class PrintInvoiceApiController : ApiController
    {
        #region Initialization
        private readonly GenericRepository<OrderPartEntity> _repository = new GenericRepository<OrderPartEntity>();
        #endregion
        //InsertOrderDocument
        #region Method


        [HttpGet]
        [Route("PrintInvoiceDetail")]
        public ApiResponse<PrintInvoiceDetailEntity> PrintInvoiceDetail(string InvoiceNumber)
        {
            var response = new ApiResponse<PrintInvoiceDetailEntity>();
            try
            {
                SqlParameter[] param = { new SqlParameter("InvoiceNumber", (object)InvoiceNumber ?? (object)DBNull.Value) };

                var result = _repository.ExecuteSQL<PrintInvoiceDetailEntity>("PrintInvoiceDetail", param).ToList();

                if (result == null)
                {
                    result = new List<PrintInvoiceDetailEntity>();
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
        [Route("PrintInvoice")]
        public ApiResponse<PrintInvoiceEntity> PrintInvoice(string InvoiceNumber,int OrderId,int PartNo,bool PrintAll=false)
        {
            var response = new ApiResponse<PrintInvoiceEntity>();
            try
            {
                SqlParameter[] param = {
                                 new SqlParameter("InvoiceNumbers", (object)InvoiceNumber ?? (object)DBNull.Value)
                                ,new SqlParameter("OrderId", (object)OrderId ?? (object)DBNull.Value)
                                ,new SqlParameter("PartNo", (object)PartNo ?? (object)DBNull.Value)
                                ,new SqlParameter("PrintAll", (object)PrintAll ?? (object)DBNull.Value)
                               };

                var result = _repository.ExecuteSQL<PrintInvoiceEntity>("PrintInvoice", param).ToList();

                if (result == null)
                {
                    result = new List<PrintInvoiceEntity>();
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
        [Route("DownloadInvoice")]
        public HttpResponseMessage DownloadInvoice(PostDataEntity strHTML)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
            string fileNames = "Invoice_"+ DateTime.Now.ToString("yyyyMMddHHmmss"); 
            
            try
            {
                byte[] bytes;// = new byte[file.Length];                
                var htmlToPdf = new HtmlToPdfConverter();
                if (!String.IsNullOrEmpty(strHTML.htmlString))
                {
                    strHTML.htmlString = strHTML.htmlString.Replace("break-after: page", "page-break-after:always");
                }
                bytes = htmlToPdf.GeneratePdf(strHTML.htmlString);                
                response.Content = new ByteArrayContent(bytes);

                response.Content.Headers.Clear();
                response.Content.Headers.Add("Content-Disposition", "attachment; filename=" + fileNames + ".pdf");
                response.Content.Headers.Add("Content-Length", bytes.Length.ToString());
                response.Content.Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping("application/pdf"));

                return response;
            }
            catch (Exception ex)
            {
                // response.Message.Add(ex.Message);
            }
            return response;
        }

        #endregion
    }
}
