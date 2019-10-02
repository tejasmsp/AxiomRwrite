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
    public class OrderWizardStep3ApiController : ApiController
    {

        #region Initialization
        private readonly GenericRepository<OrderWizardStep3> _repository = new GenericRepository<OrderWizardStep3>();
        #endregion

        #region Step 3 Database Methods
        [HttpGet]
        [Route("GetOrderWizardStep3Details")]
        public ApiResponse<OrderWizardStep3> GetOrderWizardStep3Details(long orderId = 0)
        {
            var response = new ApiResponse<OrderWizardStep3>();
            var result = new List<OrderWizardStep3>();
            try
            {
                SqlParameter[] param = { new SqlParameter("OrderId", (object)orderId ?? (object)DBNull.Value) };
                result = _repository.ExecuteSQL<OrderWizardStep3>("GetOrderWizardStep3Details", param).ToList();

                if (result != null && result.Count > 0)
                {
                    response.Data = result;
                }
                else
                {
                    result.Add(new OrderWizardStep3());
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
        [Route("InsertOrUpdateOrderWizardStep3")]
        public BaseApiResponse InsertOrUpdateOrderWizardStep3(OrderWizardStep3 model)
        {
            var response = new BaseApiResponse();

            try
            {
                SqlParameter[] param = {
                                         new SqlParameter("OrderId", (object)model.OrderId ?? (object)DBNull.Value)
                                        ,new SqlParameter("RecordsOf", (object)model.RecordsOf ?? (object)DBNull.Value)
                                        ,new SqlParameter("SSN", (object)model.SSN ?? (object)DBNull.Value)
                                        ,new SqlParameter("DateOfBirth", (object)model.DateOfBirth ?? (object)DBNull.Value)
                                        ,new SqlParameter("DateOfDeath", (object)model.DateOfDeath ?? (object)DBNull.Value)
                                        ,new SqlParameter("Address1", (object)model.Address1 ?? (object)DBNull.Value)
                                        ,new SqlParameter("Address2", (object)model.Address2 ?? (object)DBNull.Value)
                                        ,new SqlParameter("City", (object)model.City ?? (object)DBNull.Value)
                                        ,new SqlParameter("State", (object)model.State ?? (object)DBNull.Value)
                                        ,new SqlParameter("Zip", (object)model.ZipCode ?? (object)DBNull.Value)
                                        ,new SqlParameter("PatientTypeId", (object)model.PatientTypeId ?? (object)DBNull.Value)
                                        ,new SqlParameter("CreatedBy", (object)model.EmpId ?? (object)DBNull.Value)
                                        ,new SqlParameter("UserAccessId ", (object)model.UserAccessId  ?? (object)DBNull.Value)
                };
                var result = _repository.ExecuteSQL<long>("InsertOrUpdateOrderWizardStep3", param).FirstOrDefault();

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
