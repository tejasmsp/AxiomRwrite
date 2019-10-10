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
    public class OrderDetailMasterApiController : ApiController
    {
        #region Initialization
        private readonly GenericRepository<OrderPartEntity> _repository = new GenericRepository<OrderPartEntity>();
        #endregion

        #region Method
        [HttpGet]
        [Route("GetOrderCompanyDetail")]
        public ApiResponse<OrderCompanyEntity> GetOrderCompanyDetail(string OrderId)
        {
            var response = new ApiResponse<OrderCompanyEntity>();
            try
            {
                SqlParameter[] param = { new SqlParameter("orderId", (object)OrderId ?? (object)DBNull.Value) };
                var result = _repository.ExecuteSQL<OrderCompanyEntity>("GetOrderCompanyDetail", param).ToList();
                if (result == null)
                {
                    result = new List<OrderCompanyEntity>();
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
        [Route("GetClientInformation")]
        public ApiResponse<ClientInformation> GetClientInformation(string OrderId)
        {
            var response = new ApiResponse<ClientInformation>();
            try
            {
                SqlParameter[] param = { new SqlParameter("orderId", (object)OrderId ?? (object)DBNull.Value) };
                var result = _repository.ExecuteSQL<ClientInformation>("GetClientInformation", param).ToList();
                if (result == null)
                {
                    result = new List<ClientInformation>();
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
        [Route("GetPartInternalStatus")]
        public ApiResponse<InternalStatus> GetPartInternalStatus(int OrderId, int PartNo)
        {
            var response = new ApiResponse<InternalStatus>();
            try
            {
                SqlParameter[] param = { new SqlParameter("OrderId", (object)OrderId ?? (object)DBNull.Value),
                 new SqlParameter("PartNo", (object)PartNo ?? (object)DBNull.Value)};
                var result = _repository.ExecuteSQL<InternalStatus>("GetPartInternalStatus", param).ToList();
                if (result == null)
                {
                    result = new List<InternalStatus>();
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
        [Route("GetOrderDetailByOrderId")]
        public ApiResponse<OrderListEntity> GetOrderDetailByOrderId(int OrderId)
        {
            var response = new ApiResponse<OrderListEntity>();
            try
            {
                SqlParameter[] param = { new SqlParameter("OrderId", (object)OrderId ?? (object)DBNull.Value) };                 
                var result = _repository.ExecuteSQL<OrderListEntity>("GetOrderDetailByOrderId", param).ToList();
                if (result == null)
                {
                    result = new List<OrderListEntity>();
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
        [Route("GetOrderInformation")]
        public ApiResponse<BasicOrderInformation> GetOrderInformation(int OrderId)
        {
            var response = new ApiResponse<BasicOrderInformation>();
            try
            {
                SqlParameter[] param = { new SqlParameter("OrderNo", (object)OrderId ?? (object)DBNull.Value) };
                var result = _repository.ExecuteSQL<BasicOrderInformation>("GetOrderInformation", param).ToList();
                if (result == null)
                {
                    result = new List<BasicOrderInformation>();
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
        [Route("UpdateBasicOrderIformation")]
        public BaseApiResponse UpdateBasicOrderIformation(BasicOrderInformation objOrder)
        {
            var response = new BaseApiResponse();
            response.Success = true;
            try
            {
                SqlParameter[] param = {
                    new SqlParameter("OrderNo", (object)objOrder.OrderNo ?? (object)DBNull.Value)
                    ,new SqlParameter("PatientName", (object)objOrder.PatientName ?? (object)DBNull.Value)
                    ,new SqlParameter("SSN", (object)objOrder.SSN ?? (object)DBNull.Value)
                    ,new SqlParameter("DateOfBirth", (object)objOrder.DateOfBirth ?? (object)DBNull.Value)
                    ,new SqlParameter("DateOfDeath", (object)objOrder.DateOfDeath ?? (object)DBNull.Value)
                    ,new SqlParameter("DateOfLoss", (object)objOrder.DateOfLoss ?? (object)DBNull.Value)
                    ,new SqlParameter("CompanyNo", (object)objOrder.CompanyNo ?? (object)DBNull.Value)
                    ,new SqlParameter("BillingClaimNo", (object)objOrder.BillingClaimNo ?? (object)DBNull.Value)
                    
                };
                var result = _repository.ExecuteSQL<int>("UpdateOrderBasicInformation", param).FirstOrDefault();
                
                if (result == null)
                {
                    result = 0;
                    response.Success = false;
                }
                response.Success = true;                
            }
            catch (Exception ex)
            {
                response.Message.Add(ex.Message);
            }
            return response;
        }
        //UpdateOrderBasicInformation

        [HttpGet]
        [Route("UpdateBillToFirm")]
        public BaseApiResponse UpdateBillToFirm(int OrderID,string FirmID)
        {
            var response = new BaseApiResponse();
            response.Success = true;
            try
            {
                SqlParameter[] param = {
                    new SqlParameter("OrderNo", (object)OrderID ?? (object)DBNull.Value)
                    ,new SqlParameter("FirmID", (object)FirmID ?? (object)DBNull.Value)                    
                };
                var result = _repository.ExecuteSQL<int>("UpdateBillToFirm", param).FirstOrDefault();

                if (result == null)
                {
                    result = 0;
                    response.Success = false;
                }
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Message.Add(ex.Message);
            }
            return response;
        }
        [HttpGet]
        [Route("UpdateBillToAttorney")]
        public BaseApiResponse UpdateBillToAttorney(int OrderID, string AttyID)
        {
            var response = new BaseApiResponse();
            response.Success = true;
            try
            {
                SqlParameter[] param = {
                    new SqlParameter("OrderNo", (object)OrderID ?? (object)DBNull.Value)
                    ,new SqlParameter("AttyID", (object)AttyID ?? (object)DBNull.Value)
                };
                var result = _repository.ExecuteSQL<int>("UpdateBillToAttorney", param).FirstOrDefault();

                if (result == null)
                {
                    result = 0;
                    response.Success = false;
                }
                response.Success = true;
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
