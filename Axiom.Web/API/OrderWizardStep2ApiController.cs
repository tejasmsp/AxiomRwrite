using Axiom.Common;
using Axiom.Data.Repository;
using Axiom.Entity;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Axiom.Web.API
{
    [RoutePrefix("api")]
    public class OrderWizardStep2ApiController : ApiController
    {

        #region Initialization
        private readonly GenericRepository<OrderWizardStep2> _repository = new GenericRepository<OrderWizardStep2>();
        #endregion

        #region Step 1 Database Methods
        [HttpGet]
        [Route("GetOrderWizardStep2Details")]
        public ApiResponse<OrderWizardStep2> GetOrderWizardStep2Details(long orderId = 0)
        {
            var response = new ApiResponse<OrderWizardStep2>();
            var result = new List<OrderWizardStep2>();
            try
            {
                SqlParameter[] param = { new SqlParameter("OrderId", (object)orderId ?? (object)DBNull.Value) };
                result = _repository.ExecuteSQL<OrderWizardStep2>("GetOrderWizardStep2Details", param).ToList();

                if (result != null && result.Count > 0)
                {
                    response.Data = result;
                }else
                {
                    result.Add(new OrderWizardStep2());
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
        [Route("InsertOrUpdateOrderWizardStep2")]
        public BaseApiResponse InsertOrUpdateOrderWizardStep2(OrderWizardStep2 model)
        {
            var response = new BaseApiResponse();

            try
            {
                SqlParameter[] param = {
                                         new SqlParameter("OrderId", (object)model.OrderId ?? (object)DBNull.Value)
                                        ,new SqlParameter("BillToOrderingFirm", (object)model.BillToOrderingFirm ?? (object)DBNull.Value)
                                        ,new SqlParameter("BillingFirmId", (object)model.BillingFirmId ?? (object)DBNull.Value)
                                        ,new SqlParameter("BillingAttorneyId", (object)model.BillingAttorneyId ?? (object)DBNull.Value)
                                        ,new SqlParameter("BillingClaimNo", (object)model.BillingClaimNo ?? (object)DBNull.Value)
                                        ,new SqlParameter("BillingInsured", (object)model.BillingInsured ?? (object)DBNull.Value)
                                        ,new SqlParameter("CreatedBy", (object)model.EmpId ?? (object)DBNull.Value)
                                        ,new SqlParameter("UserAccessId ", (object)model.UserAccessId  ?? (object)DBNull.Value)
                };
                var result = _repository.ExecuteSQL<long>("InsertOrUpdateOrderWizardStep2", param).FirstOrDefault();

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

        [HttpGet]
        [Route("SelectFirmForBillingStep2")]
        public ApiResponse<FirmForBilling> SelectFirmForBillingStep2()
        {
            var response = new ApiResponse<FirmForBilling>();
            var result = new List<FirmForBilling>();
            try
            {
                result = _repository.ExecuteSQL<FirmForBilling>("GetFirmForBillingStep2").ToList();
                if (result != null && result.Count > 0)
                {
                    response.Data = result;
                }
                else
                {
                    result.Add(new FirmForBilling());
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
        [Route("GetStateFirmStep2")]
        public ApiResponse<FirmForBilling> GetStateFirmStep2()
        {
            var response = new ApiResponse<FirmForBilling>();
            var result = new List<FirmForBilling>();
            try
            {
                result = _repository.ExecuteSQL<FirmForBilling>("GetStateFirmStep2").ToList();
                if (result != null && result.Count > 0)
                {
                    response.Data = result;
                }
                else
                {
                    result.Add(new FirmForBilling());
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
    }
}
