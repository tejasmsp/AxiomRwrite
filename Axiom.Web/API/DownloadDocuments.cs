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
using System.Net.Http.Headers;
using Ionic.Zip;

namespace Axiom.Web.API
{
    [RoutePrefix("api")]
    public class DownloadDocumentApiController : ApiController
    {
        #region Initialization
        private readonly GenericRepository<OrderPartEntity> _repository = new GenericRepository<OrderPartEntity>();
        #endregion
        //InsertOrderDocument
        #region Method

        [HttpGet]
        [Route("DownloadDocument")]
        public HttpResponseMessage DownloadDocument(string value)
        {
            string fileDiskName = string.Empty;
            string fileName = string.Empty;
            int orderNo = 0;
            int partNo = 0;
            DateTime dtNow;
            string[] querystring = HttpUtility.UrlDecode(EncryptDecrypt.Decrypt(value)).Split(',');
            fileDiskName = querystring[0];
            fileName = querystring[1];
            orderNo = Convert.ToInt32(querystring[2]);
            partNo = Convert.ToInt32(querystring[3]);
            dtNow = Convert.ToDateTime(querystring[4]);
            if (DateTime.Now > dtNow)
            {
                return this.Request.CreateResponse(HttpStatusCode.NotFound, "Your Record is Expired.");
            }

            try
            {
                if (!string.IsNullOrEmpty(value))
                {

                    return DownloadFileFromServer(fileDiskName, fileName, orderNo, partNo);
                }
                return this.Request.CreateResponse(HttpStatusCode.NotFound, "File not found.");
            }
            catch (Exception ex)
            {
                return this.Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        public HttpResponseMessage DownloadFileFromServer(string fileDiskName, string originalFileName, int orderNo, int partNo = 0)
        {
            string fileType = Path.GetExtension(originalFileName);

            if (!string.IsNullOrEmpty(fileType) && (fileType.ToUpper() == ".DOC" || fileType.ToUpper() == ".DOCX"))
            {
                originalFileName = Path.GetFileNameWithoutExtension(originalFileName) + ".pdf";
            }
            string filePath = ConfigurationManager.AppSettings["UploadRoot"].ToString();
            if (partNo > 0)
            {
                filePath += "/" + orderNo + "/" + partNo + "/" + fileDiskName;

            }
            else
            {
                filePath += "/" + orderNo + "/" + fileDiskName;
            }

            using (MemoryStream ms = new MemoryStream())
            {
                using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    byte[] bytes = new byte[file.Length];
                    file.Read(bytes, 0, (int)file.Length);
                    ms.Write(bytes, 0, (int)file.Length);

                    HttpResponseMessage httpResponseMessage = new HttpResponseMessage();
                    httpResponseMessage.Content = new ByteArrayContent(bytes.ToArray());
                    httpResponseMessage.Content.Headers.Add("x-filename", originalFileName);
                    httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    httpResponseMessage.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                    httpResponseMessage.Content.Headers.ContentDisposition.FileName = originalFileName;
                    httpResponseMessage.StatusCode = HttpStatusCode.OK;
                    return httpResponseMessage;
                }
            }
        }
        #endregion
    }
}
