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
    public class MessageApiController : ApiController
    {
        #region Initialization
        private readonly GenericRepository<MessageEntity> _repository = new GenericRepository<MessageEntity>();

        //temp        
        #endregion

        [HttpGet]
        [Route("GetCustomMessageList")]
        public ApiResponse<MessageEntity> GetCustomMessageList(int districtid = 0)
        {
            var response = new ApiResponse<MessageEntity>();

            try
            {
                var result = _repository.ExecuteSQL<MessageEntity>("GetCustomMessageList").ToList();
                if (result == null)
                {
                    result = new List<MessageEntity>();
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
        [Route("InsertCustomMessage")]
        public BaseApiResponse InsertCustomMessage(MessageEntity model)
        {
            var response = new BaseApiResponse();
            try
            {

                SqlParameter[] param = { new SqlParameter("name", (object)model.Name ?? (object)DBNull.Value)
                                        , new SqlParameter("customMessage", (object)model.CustomMessage?? (object)DBNull.Value)
                                        , new SqlParameter("isActive", (object)model.IsActive ?? (object)DBNull.Value)
                                        , new SqlParameter("createdBy", (object)model.CreatedBy?? (object)DBNull.Value)
                                        };
                var result = _repository.ExecuteSQL<int>("InsertCustomMessage", param).FirstOrDefault();
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
        [Route("UpdateCustomMessage")]
        public BaseApiResponse UpdateCustomMessage(MessageEntity model)
        {
            var response = new BaseApiResponse();
            try
            {

                SqlParameter[] param = {new SqlParameter("id", (object)model.ID ?? (object)DBNull.Value)
                                        ,new SqlParameter("name", (object)model.Name ?? (object)DBNull.Value)
                                        , new SqlParameter("customMessage", (object)model.CustomMessage?? (object)DBNull.Value)
                                        , new SqlParameter("isActive", (object)model.IsActive ?? (object)DBNull.Value)
                                        , new SqlParameter("updatedBy", (object)model.CreatedBy ?? (object)DBNull.Value)
                                        };
                var result = _repository.ExecuteSQL<int>("UpdateCustomMessage", param).FirstOrDefault();
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

        [HttpGet]
        [Route("GetCustomMessageById")]
        public ApiResponse<MessageEntity> GetCustomMessageById(int id = 0)
        {
            var response = new ApiResponse<MessageEntity>();
            try
            {
                SqlParameter[] param = { new SqlParameter("id", (object)id ?? (object)DBNull.Value)
                                };
                var result = _repository.ExecuteSQL<MessageEntity>("GetCustomMessageById", param).ToList();
                if (result == null)
                {
                    result = new List<MessageEntity>();
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
        [Route("DeleteCustomMessage")]
        public BaseApiResponse DeleteCustomMessage(int id = 0)
        {
            var response = new BaseApiResponse();
            try
            {
                SqlParameter[] param = { new SqlParameter("districtid", (object)id ?? (object)DBNull.Value) };
                var result = _repository.ExecuteSQL<int>("DeleteCustomMessage", param).FirstOrDefault();
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

        [HttpGet]
        [Route("GetCustomMessageForClient")]
        public ApiResponse<MessageEntity> GetCustomMessageForClient()
        {
            var response = new ApiResponse<MessageEntity>();

            try
            {
                var result = _repository.ExecuteSQL<MessageEntity>("GetCustomMessageForClient").ToList();
                if (result == null)
                {
                    result = new List<MessageEntity>();
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
        [Route("GetUserNotification")]
        public ApiResponse<UserNotificationEntity> GetUserNotification(int? UserAccessId)
        {
            var response = new ApiResponse<UserNotificationEntity>();
            try
            {
                SqlParameter[] param = { new SqlParameter("UserAccessId", (object)UserAccessId ?? (object)DBNull.Value) };
                var result = _repository.ExecuteSQL<UserNotificationEntity>("GetUserNotification", param).ToList();
                if (result == null)
                {
                    result = new List<UserNotificationEntity>();
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
        [Route("UpdateUserNotification")]
        public BaseApiResponse UpdateUserNotification(int? UserAccessId,int? ID)
        {
            var response = new BaseApiResponse();
            try
            {
                SqlParameter[] param = {
                                     new SqlParameter("UserAccessId", (object)UserAccessId ?? (object)DBNull.Value)
                                     ,new SqlParameter("ID", (object)ID ?? (object)DBNull.Value)
                };
                var result = _repository.ExecuteSQL<int>("UpdateUserNotification", param).FirstOrDefault();
                if (result>0)
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

        [HttpGet]
        [Route("InsertNotificationReadByUser")]
        public ApiResponse<CustomMessageViewEntity> InsertNotificationReadByUser(string UserID)
        {
            var response = new ApiResponse<CustomMessageViewEntity>();

            try
            {
                SqlParameter[] param = { new SqlParameter("UserID", (object)UserID ?? (object)DBNull.Value) };
                var result = _repository.ExecuteSQL<CustomMessageViewEntity>("InsertNotificationReadByUser",param).ToList();
                if (result == null)
                {
                    result = new List<CustomMessageViewEntity>();
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
        [Route("GetNotificationReadByUser")]
        public ApiResponse<CustomMessageViewEntity> GetNotificationReadByUser(string UserID)
        {
            var response = new ApiResponse<CustomMessageViewEntity>();

            try
            {
                SqlParameter[] param = { new SqlParameter("UserID", (object)UserID ?? (object)DBNull.Value) };
                var result = _repository.ExecuteSQL<CustomMessageViewEntity>("GetNotificationReadByUser", param).ToList();
                if (result == null)
                {
                    result = new List<CustomMessageViewEntity>();
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
    }
}