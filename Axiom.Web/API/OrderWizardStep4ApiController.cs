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
    public class OrderWizardStep4ApiController : ApiController
    {

        #region Initialization
        private readonly GenericRepository<OrderWizardStep4> _repository = new GenericRepository<OrderWizardStep4>();
        #endregion

        #region Step 3 Database Methods
        [HttpGet]
        [Route("GetOrderWizardStep4Details")]
        public ApiResponse<OrderWizardStep4> GetOrderWizardStep4Details(long orderId = 0)
        {
            var response = new ApiResponse<OrderWizardStep4>();
            var result = new List<OrderWizardStep4>();
            try
            {
                SqlParameter[] param = { new SqlParameter("OrderId", (object)orderId ?? (object)DBNull.Value) };
                result = _repository.ExecuteSQL<OrderWizardStep4>("GetOrderWizardStep4Details", param).ToList();

                if (result != null && result.Count > 0)
                {
                    response.Data = result;
                }
                else
                {
                    result.Add(new OrderWizardStep4());
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
        [Route("InsertOrUpdateOrderWizardStep4")]
        public BaseApiResponse InsertOrUpdateOrderWizardStep4(OrderWizardStep4 model)
        {
            var response = new BaseApiResponse();

            try
            {
                SqlParameter[] param = {
                                         new SqlParameter("OrderId", (object)model.OrderId ?? (object)DBNull.Value)
                                        ,new SqlParameter("CaseTypeId", (object)model.CaseTypeId ?? (object)DBNull.Value)
                                        ,new SqlParameter("Caption1", (object)model.Caption1 ?? (object)DBNull.Value)
                                        ,new SqlParameter("VsText1", (object)model.VsText1 ?? (object)DBNull.Value)
                                        ,new SqlParameter("Caption2", (object)model.Caption2 ?? (object)DBNull.Value)
                                        ,new SqlParameter("VsText2", (object)model.VsText2 ?? (object)DBNull.Value)
                                        ,new SqlParameter("Caption3", (object)model.Caption3 ?? (object)DBNull.Value)
                                        ,new SqlParameter("VsText3", (object)model.VsText3 ?? (object)DBNull.Value)
                                        ,new SqlParameter("CauseNo", (object)model.CauseNo ?? (object)DBNull.Value)
                                        ,new SqlParameter("ClientMatterNo", (object)model.ClaimMatterNo ?? (object)DBNull.Value)
                                        ,new SqlParameter("TrialDate", (object)model.TrialDate ?? (object)DBNull.Value)
                                        ,new SqlParameter("State", (object)model.State ?? (object)DBNull.Value)
                                        ,new SqlParameter("IsStateOrFedral", (object)model.IsStateOrFedral ?? (object)DBNull.Value)
                                        ,new SqlParameter("County", (object)model.County ?? (object)DBNull.Value)
                                        ,new SqlParameter("Court", (object)model.Court ?? (object)DBNull.Value)
                                        ,new SqlParameter("District", (object)model.District ?? (object)DBNull.Value)
                                        ,new SqlParameter("Division", (object)model.Division ?? (object)DBNull.Value)
                                        ,new SqlParameter("BillingDateOfLoss", (object)model.BillingDateOfLoss ?? (object)DBNull.Value)
                                        ,new SqlParameter("CreatedBy", (object)model.EmpId ?? (object)DBNull.Value)
                                        ,new SqlParameter("UserAccessId ", (object)model.UserAccessId  ?? (object)DBNull.Value)
                };
                var result = _repository.ExecuteSQL<long>("InsertOrUpdateOrderWizardStep4", param).FirstOrDefault();

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
