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
    public class OrderFirmAttorneyApiController : ApiController
    {
        #region Initialization
        private readonly GenericRepository<AttorneyEntity> _repository = new GenericRepository<AttorneyEntity>();
        #endregion

        #region Method
        [HttpGet]
        [Route("GetOrderFirmAttorneyByOrderId")]
        public ApiResponse<OrderFirmAttorneyEntity> GetOrderFirmAttorneyByOrderId(string OrderId)
        {
            var response = new ApiResponse<OrderFirmAttorneyEntity>();

            try
            {
                SqlParameter[] param = { new SqlParameter("orderId", (object)OrderId ?? (object)DBNull.Value) };
                var result = _repository.ExecuteSQL<OrderFirmAttorneyEntity>("GetOrderFirmAttorneyByOrderId", param).ToList();

                if (result == null)
                {
                    result = new List<OrderFirmAttorneyEntity>();
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
        [Route("GetOrderFirmAttorneyByOrderFirmAttorneyId")]
        public ApiResponse<OrderFirmAttorneyEntity> GetOrderFirmAttorneyByOrderFirmAttorneyId(string OrderFirmAttorneyId)
        {
            var response = new ApiResponse<OrderFirmAttorneyEntity>();

            try
            {
                SqlParameter[] param = { new SqlParameter("orderFirmAttorneyId", (object)OrderFirmAttorneyId ?? (object)DBNull.Value) };
                var result = _repository.ExecuteSQL<OrderFirmAttorneyEntity>("GetOrderFirmAttorneyByOrderFirmAttorneyId", param).ToList();

                if (result == null)
                {
                    result = new List<OrderFirmAttorneyEntity>();
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
        [Route("GetWaiverDetailByOrderFirmAttorneyId")]
        public ApiResponse<WaiverEntity> GetWaiverDetailByOrderFirmAttorneyId(string OrderFirmAttorneyId, long OrderId)
        {
            var response = new ApiResponse<WaiverEntity>();

            try
            {
                SqlParameter[] param = { new SqlParameter("OrderFirmAttorneyId", (object)OrderFirmAttorneyId ?? (object)DBNull.Value)
                                        ,new SqlParameter("OrderId", (object)OrderId ?? (object)DBNull.Value)
                                       };
                var result = _repository.ExecuteSQL<WaiverEntity>("GetWaiverDetailByOrderFirmAttorneyId", param).ToList();

                if (result == null)
                {
                    result = new List<WaiverEntity>();
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
        [Route("GetAttorneyByAttyId")]
        public ApiResponse<OrderFirmAttorneyEntity> GetAttorneyByAttyId(string attyId)
        {
            var response = new ApiResponse<OrderFirmAttorneyEntity>();

            try
            {
                SqlParameter[] param = { new SqlParameter("attyId", (object)attyId ?? (object)DBNull.Value)
                                       };
                var result = _repository.ExecuteSQL<OrderFirmAttorneyEntity>("GetAttorneyByAttyId", param).ToList();

                if (result == null)
                {
                    result = new List<OrderFirmAttorneyEntity>();
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
        [Route("SaveWaiver")]
        public BaseApiResponse SaveWaiver(long OrderId, List<WaiverEntity> waiverlist)
        {
            var response = new BaseApiResponse();

            try
            {
                string xmlData = ConvertToXml<WaiverEntity>.GetXMLString(waiverlist);

                SqlParameter[] param = { new SqlParameter("xmlData", (object)xmlData ?? (object)DBNull.Value)
                                        ,new SqlParameter("OrderId", (object)OrderId ?? (object)DBNull.Value)
                                       };
                var result = _repository.ExecuteSQL<long>("InsertUpdateWaiver", param).FirstOrDefault();

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
        [Route("InsertUpdateOrderFirmAttorney")]
        public BaseApiResponse InsertUpdateOrderFirmAttorney(OrderFirmAttorneyEntity model)
        {
            var response = new BaseApiResponse();
            try
            {
                SqlParameter[] param1 = {new SqlParameter("OrderFirmAttorneyId", (object)model.OrderFirmAttorneyId ?? (object)DBNull.Value)
                                        ,new SqlParameter("OrderId", (object)model.OrderId ?? (object)DBNull.Value)
                                        ,new SqlParameter("AttyID", (object)model.AttyId ?? (object)DBNull.Value)
                                        ,new SqlParameter("FirmID", (object)model.FirmID ?? (object)DBNull.Value)
                                        ,new SqlParameter("AttorneyFor", (object)model.AttorneyFor ?? (object)DBNull.Value)
                                        ,new SqlParameter("IsPatientAttorney", (object)model.IsPatientAttorney ?? (object)DBNull.Value)
                                        ,new SqlParameter("OppSide", (object)model.OppSide ?? (object)DBNull.Value)
                                        ,new SqlParameter("Represents", (object)model.Represents ?? (object)DBNull.Value)
                                        ,new SqlParameter("Notes", (object)model.Notes ?? (object)DBNull.Value)
                                        ,new SqlParameter("CreatedBy", (object)model.EmpId ?? (object)DBNull.Value)
                                        ,new SqlParameter("UserAccessId ", (object)model.UserAccessId  ?? (object)DBNull.Value)
                };
                var result = _repository.ExecuteSQL<long>("InsertUpdateOrderFirmAttorney", param1).FirstOrDefault();
                if(result != 0)
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

        [HttpGet]
        [Route("ChangeOrderAttorneyStatus")]
        public BaseApiResponse ChangeOrderAttorneyStatus(long OrderId,string AttyId,bool IsDisabled, int UserAccessId)
        {
            var response = new BaseApiResponse();

            try
            {
                SqlParameter[] param = { new SqlParameter("OrderId", (object)OrderId ?? (object)DBNull.Value)
                                        ,new SqlParameter("AttyId", (object)AttyId ?? (object)DBNull.Value)
                                        ,new SqlParameter("IsDisabled", (object)IsDisabled ?? (object)DBNull.Value)
                                        ,new SqlParameter("UserAccessId", (object)UserAccessId ?? (object)DBNull.Value)
                                       };
                var result = _repository.ExecuteSQL<int>("ChangeOrderAttorneyStatus", param).FirstOrDefault();

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
        #endregion
    }
}
