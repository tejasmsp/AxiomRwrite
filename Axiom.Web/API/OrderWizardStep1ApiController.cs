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
using System.Text;
using System.Configuration;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Data;
using System.Reflection;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Net.Mail;
using System.Net.Configuration;

namespace Axiom.Web.API
{
    [RoutePrefix("api")]
    public class OrderWizardStep1ApiController : ApiController
    {
        #region Initialization
        private readonly GenericRepository<AttorneyEntity> _repository = new GenericRepository<AttorneyEntity>();
        #endregion

        #region Drop Down
        [HttpGet]
        [Route("GetAttorneyList")]
        public ApiResponse<AttorneyEntity> GetAttorneyList(string userId)
        {
            var response = new ApiResponse<AttorneyEntity>();

            try
            {
                SqlParameter[] param = { new SqlParameter("userId", (object)userId ?? (object)DBNull.Value) };
                var result = _repository.ExecuteSQL<AttorneyEntity>("GetAttorneyList", param).ToList();

                if (result == null)
                {
                    result = new List<AttorneyEntity>();
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
        [Route("GetAttorneyForCode")]
        public ApiResponse<CodeEntity> GetAttorneyForCode()
        {
            var response = new ApiResponse<CodeEntity>();

            try
            {

                var result = _repository.ExecuteSQL<CodeEntity>("GetAttorneyForCode").ToList();

                if (result == null)
                {
                    result = new List<CodeEntity>();
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
        [Route("GetNotificationList")]
        public ApiResponse<NotificationEmailEntity> GetNotificationList(string attyId, int orderId)
        {
            var response = new ApiResponse<NotificationEmailEntity>();

            try
            {
                SqlParameter[] param = { new SqlParameter("AttyId", (object)attyId ?? (object)DBNull.Value)
                , new SqlParameter("OrderId", (object)orderId ?? (object)DBNull.Value)     };
                var result = _repository.ExecuteSQL<NotificationEmailEntity>("GetNotificationList", param).ToList();

                if (result == null)
                {
                    result = new List<NotificationEmailEntity>();
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
        #endregion

        #region Step 1 Database Methods
        [HttpGet]
        [Route("GetOrderWizardStep1Details")]
        public ApiResponse<OrderWizardStep1> GetOrderWizardStep1Details(long orderId = 0)
        {
            var response = new ApiResponse<OrderWizardStep1>();
            var result = new List<OrderWizardStep1>();
            try
            {
                if (orderId > 0)
                {
                    SqlParameter[] param = { new SqlParameter("OrderId", (object)orderId ?? (object)DBNull.Value) };
                    result = _repository.ExecuteSQL<OrderWizardStep1>("GetOrderWizardStep1Details", param).ToList();

                    if (result != null && result.Count > 0)
                    {
                        response.Data = result;
                    }
                }
                else
                {
                    result.Add(new OrderWizardStep1());
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
        [Route("InsertOrderWizardStep1")]
        public BaseApiResponse InsertOrderWizardStep1(OrderWizardStep1 model)
        {
            var response = new BaseApiResponse();

            try
            {
                SqlParameter[] param = { new SqlParameter("AttorneyFor", (object)model.AttorneyFor ?? (object)DBNull.Value)
                                        ,new SqlParameter("OrderingAttorney", (object)model.OrderingAttorney ?? (object)DBNull.Value)
                                        ,new SqlParameter("Represents", (object)model.Represents ?? (object)DBNull.Value)
                                        ,new SqlParameter("IsFromClient", (object)model.IsFromClient ?? (object)DBNull.Value)
                                        ,new SqlParameter("CreatedBy", (object)model.EmpId ?? (object)DBNull.Value)
                                        ,new SqlParameter("UserAccessId", (object)model.UserAccessId  ?? (object)DBNull.Value)
                                         ,new SqlParameter("OrderingFirmID", (object)model.OrderingFirmID  ?? (object)DBNull.Value)
                                         ,new SqlParameter("CompanyNo", (object)model.CompanyNo  ?? (object)DBNull.Value)
                };
                var result = _repository.ExecuteSQL<long>("InsertOrderWizardStep1", param).FirstOrDefault();

                if (result > 0)
                {
                    response.Success = true;
                    response.lng_InsertedId = result;
                    InsertUpdateNotification(model.NotificationEmail, model.OrderingAttorney, response.lng_InsertedId);
                }
            }
            catch (Exception ex)
            {
                response.Message.Add(ex.Message);
            }

            return response;
        }

        [HttpPost]
        [Route("InsertUpdateNotification")]
        public BaseApiResponse InsertUpdateNotification(List<NotificationEmailEntity> NotificationEmail, string OrderingAttorney, long? orderId)
        {
            var response = new BaseApiResponse();

            string xmlData = ConvertToXml<NotificationEmailEntity>.GetXMLString(NotificationEmail);

            try
            {
                SqlParameter[] param = { new SqlParameter("xmlData", (object)xmlData ?? (object)DBNull.Value)
                            ,new SqlParameter("OrderingAttorney", (object)OrderingAttorney ?? (object)DBNull.Value)
                             ,new SqlParameter("OrderId", (object)orderId ?? (object)DBNull.Value)
                };
                var result = _repository.ExecuteSQL<long>("[InsertUpdateNotification]", param).FirstOrDefault();

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

        [HttpPost]
        [Route("UpdateOrderWizardStep1")]
        public BaseApiResponse UpdateOrderWizardStep1(OrderWizardStep1 model)
        {
            var response = new BaseApiResponse();

            try
            {
                SqlParameter[] param = {
                                         new SqlParameter("OrderId", (object)model.OrderId ?? (object)DBNull.Value)
                                        ,new SqlParameter("AttorneyFor", (object)model.AttorneyFor ?? (object)DBNull.Value)
                                        ,new SqlParameter("OrderingAttorney", (object)model.OrderingAttorney ?? (object)DBNull.Value)
                                        ,new SqlParameter("Represents", (object)model.Represents ?? (object)DBNull.Value)
                                        ,new SqlParameter("IsFromClient", (object)model.IsFromClient ?? (object)DBNull.Value)
                                        ,new SqlParameter("UpdatedBy", (object)model.EmpId ?? (object)DBNull.Value)
                                        ,new SqlParameter("UserAccessId", (object)model.UserAccessId  ?? (object)DBNull.Value)
                                        ,new SqlParameter("OrderingFirmID", (object)model.OrderingFirmID  ?? (object)DBNull.Value)
                };

                var result = _repository.ExecuteSQL<long>("UpdateOrderWizardStep1", param).FirstOrDefault();

                if (result > 0)
                {
                    response.Success = true;
                    response.lng_InsertedId = result;
                    InsertUpdateNotification(model.NotificationEmail, model.OrderingAttorney, response.lng_InsertedId);
                }
            }
            catch (Exception ex)
            {
                response.Message.Add(ex.Message);
            }

            return response;
        }

        [HttpPost]
        [Route("SubmitOrder")]
        public async Task<BaseApiResponse> SubmitOrder(OrderWizardStep1 model)
        {
            var response = new BaseApiResponse();

            SqlParameter[] paramStatus = { new SqlParameter("OrderNo", (object)model.OrderId ?? (object)DBNull.Value) };
            bool isPartAddLater = _repository.ExecuteSQL<bool>("GetOrderStatus", paramStatus).FirstOrDefault();

            try
            {
                if (model.OrderId > 0)
                {
                    SqlParameter[] Sqlparam = { new SqlParameter("OrderId", (object)model.OrderId ?? (object)DBNull.Value) };
                    var orderResult = _repository.ExecuteSQL<OrderWizardStep1>("GetOrderWizardStep1Details", Sqlparam).ToList();
                    if (orderResult != null && orderResult.Count > 0)
                    {
                        model.SubmitStatus = orderResult[0].SubmitStatus;
                    }
                    SqlParameter[] param = {new SqlParameter("OrderId", (object)model.OrderId ?? (object)DBNull.Value)
                                         ,new SqlParameter("UpdatedBy", (object)model.EmpId ?? (object)DBNull.Value)
                                        ,new SqlParameter("UserAccessId", (object)model.UserAccessId  ?? (object)DBNull.Value)};

                    var result = _repository.ExecuteSQL<int>("SubmitOrder", param).FirstOrDefault();

                    if (result > 0)
                    {


                        //Upload Location Documents on Final Submit
                        SqlParameter[] SqlFileparam = { new SqlParameter("OrderNo", (object)model.OrderId ?? (object)DBNull.Value),
                        new SqlParameter("PartNo", (object)0 ?? (object)DBNull.Value)};
                        var LocDocumentList = _repository.ExecuteSQL<LocationFilesModel>("GetLocationTempFilesForNewOrder", SqlFileparam).ToList();
                        if (LocDocumentList != null && LocDocumentList.Count > 0)
                        {
                            string sourcePath = ConfigurationManager.AppSettings["AttachPathLocal"].ToString();
                            string destinationPath = ConfigurationManager.AppSettings["UploadRoot"].ToString();
                            foreach (var item in LocDocumentList)
                            {
                                SqlParameter[] deleteattachparam = {
                                    new SqlParameter("OrderNo", (object)model.OrderId ?? (object)DBNull.Value),
                                    new SqlParameter("PartNo", (object)item.PartNo ?? (object)DBNull.Value),
                                    new SqlParameter("FileTypeId", (object) item.FileTypeId ?? (object)DBNull.Value),
                                    new SqlParameter("DeleteFromLocFile", (object) (int)0 ?? (object)DBNull.Value),
                                    new SqlParameter("LocFileId", (object) (int)0 ?? (object)DBNull.Value)
                                };
                                _repository.ExecuteSQL<int>("DeleteLocationFile", deleteattachparam).FirstOrDefault();
                            }
                            foreach (var docitem in LocDocumentList)
                            {

                                var destPath = destinationPath + "/" + docitem.OrderNo + "/" + docitem.PartNo;
                                string fileDiskName = new Document().MoveLocalToServerFile(docitem.FileName, docitem.BatchId, sourcePath, destPath, docitem.OrderNo, docitem.PartNo.ToString());
                                docitem.OrderNo = Convert.ToInt32(docitem.OrderNo);
                                docitem.FileDiskName = fileDiskName;
                                docitem.Pages = 0;
                                string xmlAttachData = ConvertToXml<LocationFilesModel>.GetXMLString(new List<LocationFilesModel>() { docitem });
                                SqlParameter[] attachparam = { new SqlParameter("xmlDataString", (object)xmlAttachData ?? (object)DBNull.Value) };
                                _repository.ExecuteSQL<int>("InsertFiles", attachparam).FirstOrDefault();
                            }
                            foreach (var docitem in LocDocumentList)
                            {
                                Document docobj = new Document();
                                docobj.DeleteAttchFile(docitem.FileName, docitem.BatchId, sourcePath);
                            }
                            SqlParameter[] deleteLocationFile = {
                                    new SqlParameter("OrderNo", (object)model.OrderId ?? (object)DBNull.Value)
                                };
                            _repository.ExecuteSQL<int>("DeleteLocationFileByOrderNo", deleteLocationFile).FirstOrDefault();                            
                        }

                        await new OrderProcess().OrderSummaryEmail(Convert.ToInt32(model.OrderId), model.UserEmail, model.CompanyNo, Convert.ToInt32(model.SubmitStatus));
                        await new OrderProcess().AddQuickformsForNewOrder(Convert.ToInt32(model.OrderId), Convert.ToBoolean(model.SubmitStatus), Convert.ToBoolean(model.SubmitStatus));

                        // await new OrderProcess().ESignature(Convert.ToInt32(model.OrderId), model.CompanyNo);

                        InsertWaiverForNewOrder(Convert.ToInt32(model.OrderId));

                        // UPDATE isAddedPart TO 0 OF ALL PART AFTER PROCESSING ORDER
                        SqlParameter[] paramAddedPart = { new SqlParameter("OrderNo", (object)model.OrderId ?? (object)DBNull.Value) };
                        int resultAddedPart = _repository.ExecuteSQL<int>("SubmitOrderUpdateisAddedPart", paramAddedPart).FirstOrDefault();

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
        [Route("InsertWaiverForNewOrder")]
        public void InsertWaiverForNewOrder(int OrderNo)
        {

            try
            {
                SqlParameter[] param = { new SqlParameter("OrderNo", (object)OrderNo ?? (object)DBNull.Value) };
                var result = _repository.ExecuteSQL<int>("InsertWaiverForNewOrder", param).FirstOrDefault();
            }
            catch (Exception ex)
            {

            }
        }

        #endregion
    }
}
