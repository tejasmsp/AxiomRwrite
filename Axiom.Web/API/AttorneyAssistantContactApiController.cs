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



namespace Axiom.Web.API
{
    [RoutePrefix("api")]
    public class NotificationEmailEntityApiController : ApiController
    {
        #region Initialization
        private readonly GenericRepository<NotificationEmailEntity> _repository = new GenericRepository<NotificationEmailEntity>();
        #endregion

        #region Method
        [HttpGet]
        [Route("GetAssistantContactNotificationInformationByOrderId")]
        public ApiResponse<NotificationEmailEntity> GetAssistantContactNotificationInformationByOrderId(string OrderId)
        {
            var response = new ApiResponse<NotificationEmailEntity>();

            try
            {
                SqlParameter[] param = { new SqlParameter("orderId", (object)OrderId ?? (object)DBNull.Value) };
                var result = _repository.ExecuteSQL<NotificationEmailEntity>("GetAssistantContactNotificationInformationByOrderId", param).ToList();

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

        [HttpPost]
        [Route("UpdateAssistantContact")]
        public BaseApiResponse UpdateAssistantContact(string orderId,List<NotificationEmailEntity> AssistantContactList)
        {
            var response = new BaseApiResponse();

            string xmlData = ConvertToXml<NotificationEmailEntity>.GetXMLString(AssistantContactList);

            try
            {
                SqlParameter[] param = { new SqlParameter("xmlData", (object)xmlData ?? (object)DBNull.Value)
                            ,new SqlParameter("OrderingAttorney", (object)DBNull.Value)
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
        #endregion
    }
}
