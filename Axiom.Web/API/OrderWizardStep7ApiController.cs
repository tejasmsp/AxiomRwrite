using Axiom.Common;
using Axiom.Entity;
using Axiom.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Data.SqlClient;
using System.Web;
using System.IO;
using System.Configuration;

namespace Axiom.Web.API
{
    [RoutePrefix("api")]
    public class OrderWizardStep7ApiController : ApiController
    {
        #region Initialization
        private readonly GenericRepository<OrderDocumentEntity> _repository = new GenericRepository<OrderDocumentEntity>();
        #endregion

        #region DatabaseOperations
        [HttpGet]
        [Route("GetDocumentList")]
        public ApiResponse<OrderDocumentEntity> GetDocumentList(long? OrderID = 0, int PartNo = 0)
        {
            var response = new ApiResponse<OrderDocumentEntity>();
            try
            {
                SqlParameter[] param = { new SqlParameter("OrderID", (object)OrderID ?? (object)DBNull.Value)
                                        ,new SqlParameter("PartNo", (object)PartNo?? (object)DBNull.Value)
                                       };
                var result = _repository.ExecuteSQL<OrderDocumentEntity>("GetOrderDocumentList", param).ToList();
                if (result == null)
                {
                    result = new List<OrderDocumentEntity>();
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
        [Route("UploadOrderDocument")]
        public ApiResponse<OrderDocumentEntity> UploadOrderDocument(long OrderId, string CreatedBy, int? UserAccessId)
        {
            var response = new ApiResponse<OrderDocumentEntity>();
            try
            {
                if (HttpContext.Current.Request.Files.AllKeys.Any())
                {
                    var httpPostedFile = HttpContext.Current.Request.Files[0];
                    string tempStorageDirectory = ConfigurationManager.AppSettings["TempStorageDirectory"].ToString();
                    //string tempStorageDirectory = System.Web.Hosting.HostingEnvironment.MapPath(System.Configuration.ConfigurationManager.AppSettings["TempStorageDirectory"]);
                    if (tempStorageDirectory.EndsWith("\\"))
                        tempStorageDirectory = tempStorageDirectory + OrderId;
                    else
                        tempStorageDirectory = tempStorageDirectory + "\\" + OrderId;

                    if (!Directory.Exists(tempStorageDirectory))
                    {
                        Directory.CreateDirectory(tempStorageDirectory);
                    }
                    string FileExtension = System.IO.Path.GetExtension(httpPostedFile.FileName);
                    string OrderGuid = Guid.NewGuid().ToString();
                    string FileSavePath = tempStorageDirectory + "\\" + OrderGuid + FileExtension;
                    httpPostedFile.SaveAs(FileSavePath);

                    string FileDBPath = FileSavePath.Replace("~", "");

                    SqlParameter[] param = {  new SqlParameter("OrderId", (object)OrderId ?? (object)DBNull.Value)
                                             ,new SqlParameter("PartNo",(object)0?? (object)DBNull.Value)
                                             ,new SqlParameter("Title", (object)httpPostedFile.FileName ?? (object)DBNull.Value)
                                             ,new SqlParameter("FilePath", (object)FileDBPath ?? (object)DBNull.Value)
                                             ,new SqlParameter("FileTypeId",(object) 0?? (object)DBNull.Value)
                                             ,new SqlParameter("RecordTypeId",(object) 0?? (object)DBNull.Value)
                                             ,new SqlParameter("CreatedBy", (object)CreatedBy ?? (object)DBNull.Value)
                                             ,new SqlParameter("UserAccessId", (object)UserAccessId ?? (object)DBNull.Value) };
                    // var result = _repository.ExecuteSQL<long>("AddOrderDocument", param).FirstOrDefault();
                    var result = _repository.ExecuteSQL<long>("InsertOrderDocument", param).FirstOrDefault();
                    #region Save as order level document 
                    try
                    {
                        var data = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(HttpContext.Current.Request.Form[0]);
                        Guid UserGuid = Guid.Parse(Convert.ToString(data["UserGuid"]));
                        

                        int FileTypeId = 18; //other
                        int RecordTypeId = 0; //other
                        SqlParameter[] paramOrderLevelDoc = {  new SqlParameter("OrderId", (object)Convert.ToInt32(OrderId) ?? (object)DBNull.Value)
                                                 ,new SqlParameter("PartNo",(object)Convert.ToInt32(0)?? (object)DBNull.Value)
                                                 ,new SqlParameter("FileName", (object)httpPostedFile.FileName ?? (object)DBNull.Value)
                                                 ,new SqlParameter("FileTypeId",(object)Convert.ToInt32(FileTypeId)?? (object)DBNull.Value)
                                                 ,new SqlParameter("IsPublic",(object)Convert.ToBoolean(true)?? (object)DBNull.Value)
                                                 ,new SqlParameter("RecordTypeId",(object) Convert.ToInt32(RecordTypeId) ?? (object)DBNull.Value)
                                                 ,new SqlParameter("FileDiskName", (object)(OrderGuid + FileExtension) ?? (object)DBNull.Value)
                                                 ,new SqlParameter("PageNo", (object)Convert.ToInt32(0) ?? (object)DBNull.Value)
                                                 ,new SqlParameter("CreatedBy", (object)UserGuid?? (object)DBNull.Value) };

                        var resultOrderLevelDoc = _repository.ExecuteSQL<int>("InsertFile", paramOrderLevelDoc).FirstOrDefault();
                        string tempPath = ConfigurationManager.AppSettings["UploadRoot"].ToString();
                        string subFolder = tempPath + "\\" + OrderId.ToString();
                        if (!Directory.Exists(subFolder))
                        {
                            Directory.CreateDirectory(subFolder);
                        }
                        if (Directory.Exists(subFolder))
                        {
                            File.Copy(FileSavePath, subFolder + "\\" + OrderGuid + FileExtension, true);
                        }
                    }
                    catch (Exception ex)
                    {


                    }


                    #endregion
                    if (result > 0)
                    {
                        response.lng_InsertedId = result;
                        response.Success = true;
                    }
                }
            }
            catch (Exception ex)
            {
                response.Message.Add(ex.Message);
            }
            return response;
        }

        [HttpPost]
        [Route("DeleteOrderDocument")]
        public BaseApiResponse DeleteOrderDocument(long? OrderDocumentId)
        {
            var response = new BaseApiResponse();
            try
            {
                SqlParameter[] param = { new SqlParameter("OrderDocumentId", (object)OrderDocumentId ?? (object)DBNull.Value) };
                var result = _repository.ExecuteSQL<int>("DeleteOrderDocument", param).FirstOrDefault();
                if (result > 0)
                {
                    response.Success = true;
                    response.lng_InsertedId = result;
                }
            }
            catch (Exception ex)
            {
                response.Message.Add(ex.Message);
            }
            return response;
        }

        #endregion
    }
}