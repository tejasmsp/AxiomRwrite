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
    public class OrderWizardStep5ApiController : ApiController
    {
        #region Initialization
        private readonly GenericRepository<OrderWizardStep5> _repository = new GenericRepository<OrderWizardStep5>();
        #endregion


        #region Step 1 Database Methods
        [HttpGet]
        [Route("GetOrderWizardStep5AttorneyRecords")]
        public ApiResponse<OrderWizardStep5> GetOrderWizardStep5AttorneyRecords(long orderId = 0)
        {
            var response = new ApiResponse<OrderWizardStep5>();
            var result = new List<OrderWizardStep5>();
            try
            {
                SqlParameter[] param = { new SqlParameter("OrderId", (object)orderId ?? (object)DBNull.Value) };
                result = _repository.ExecuteSQL<OrderWizardStep5>("GetOrderWizardStep5AttorneyRecords", param).ToList();

                if (result == null)
                {
                    result = new List<OrderWizardStep5>();
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
        [Route("DeleteOppositeAttorney")]
        public BaseApiResponse DeleteOppositeAttorney(long OrderFirmAttorneyId)
        {
            var response = new BaseApiResponse();

            try
            {
                SqlParameter[] param = { new SqlParameter("OrderFirmAttorneyId", (object)OrderFirmAttorneyId ?? (object)DBNull.Value) };
                var result = _repository.ExecuteSQL<int>("DeleteOppositeAttorney", param).FirstOrDefault();

                if (result > 0)
                {
                    response.Success = true;
                    response.InsertedId = result;
                }
                else
                {
                    response.Success = false;
                    response.InsertedId = 0;
                }
            }
            catch (Exception ex)
            {
                response.Message.Add(ex.Message);
            }

            return response;
        }



        [HttpPost]
        [Route("InsertOrUpdateOrderWizardStep5")]
        public BaseApiResponse InsertOrUpdateOrderWizardStep5(OrderWizardStep5 model)
        {
            var response = new BaseApiResponse();

            try
            {
                SqlParameter[] param = {
                                         new SqlParameter("OrderFirmAttorneyId", (object)model.OrderFirmAttorneyId ?? (object)DBNull.Value)
                                        ,new SqlParameter("OrderId", (object)model.OrderId ?? (object)DBNull.Value)
                                        ,new SqlParameter("AttyID", (object)model.AttyID ?? (object)DBNull.Value)
                                        ,new SqlParameter("FirmID", (object)model.FirmID ?? (object)DBNull.Value)
                                        ,new SqlParameter("AttorneyFor", (object)model.AttorneyFor ?? (object)DBNull.Value)
                                        ,new SqlParameter("IsPatientAttorney", (object)model.IsPatientAttorney ?? (object)DBNull.Value)
                                        ,new SqlParameter("OppSide", (object)model.OppSide ?? (object)DBNull.Value)
                                        ,new SqlParameter("Represents", (object)model.Represents ?? (object)DBNull.Value)
                                        ,new SqlParameter("Notes", (object)model.Notes ?? (object)DBNull.Value)
                                        ,new SqlParameter("CreatedBy", (object)model.EmpId ?? (object)DBNull.Value)
                                        ,new SqlParameter("UserAccessId ", (object)model.UserAccessId  ?? (object)DBNull.Value)
                };
                var result = _repository.ExecuteSQL<long>("InsertOrUpdateOrderWizardStep5", param).FirstOrDefault();

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
        [Route("GetAttorneyListWithSearch")]
        public ApiResponse<AttorneyUsersEntity> GetAttorneyListWithSearch(string SearchCriteria, int CompNo, int SearchCondition = 1, string SearchText = "", int OrderId = 0)
        {
            var response = new ApiResponse<AttorneyUsersEntity>();

            try
            {
                SqlParameter[] param = { new SqlParameter("SearchCriteria", (object)SearchCriteria ?? (object)DBNull.Value),
                                         new SqlParameter("SearchCondition", (object)SearchCondition ?? (object)DBNull.Value),
                                         new SqlParameter("SearchText", (object)SearchText ?? (object)DBNull.Value),
                                         new SqlParameter("OrderId", (object)OrderId ?? (object)DBNull.Value),
                                          new SqlParameter("CompanyNo", (object)CompNo ?? (object)DBNull.Value)
                };
                var result = _repository.ExecuteSQL<AttorneyUsersEntity>("GetAttorneyListWithSearch", param).ToList();

                if (result == null)
                {
                    result = new List<AttorneyUsersEntity>();
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
        [Route("InsertNewFirmFromStep5")]
        public BaseApiResponse InsertNewFirmFromStep5(FirmEntity modal)
        {
            var response = new BaseApiResponse();

            try
            {
                SqlParameter[] param = { new SqlParameter("FirmName", (object)modal.FirmName ?? (object)DBNull.Value),
                                         new SqlParameter("City", (object)modal.City ?? (object)DBNull.Value),
                                         new SqlParameter("CreatedBy", (object) modal.EntBy?? (object)DBNull.Value),
                                         new SqlParameter("CompanyNo", (object) modal.CompanyNo?? (object)DBNull.Value),
                                                         };
                var result = _repository.ExecuteSQL<string>("InsertNewFirmFromStep5", param).FirstOrDefault();

                if (result != null)
                {
                    response.Success = true;
                    response.str_ResponseData = result;
                }

            }
            catch (Exception ex)
            {
                response.Message.Add(ex.Message);
            }

            return response;

        }
        [HttpPost]
        [Route("InsertNewAttorneyFromStep5")]
        public BaseApiResponse InsertNewAttorneyFromStep5(AttorneyEntity modal)
        {
            var response = new BaseApiResponse();
            try
            {
                SqlParameter[] param = { new SqlParameter("FirmName", (object)modal.FirstName ?? (object)DBNull.Value),
                                         new SqlParameter("City", (object)modal.LastName ?? (object)DBNull.Value),
                                         new SqlParameter("FirmID", (object) modal.FirmID?? (object)DBNull.Value),
                                         new SqlParameter("AreaCode1", (object) modal.AreaCode1?? (object)DBNull.Value),
                                         new SqlParameter("PhoneNo", (object) modal.PhoneNo?? (object)DBNull.Value),
                                         new SqlParameter("AreaCode2", (object) modal.AreaCode2?? (object)DBNull.Value),
                                         new SqlParameter("FaxNo", (object) modal.FaxNo?? (object)DBNull.Value),
                                         new SqlParameter("Email", (object) modal.Email?? (object)DBNull.Value),
                                         new SqlParameter("StateBarNo", (object) modal.StateBarNo?? (object)DBNull.Value),
                                         new SqlParameter("CreatedBy", (object) modal.CreatedBy?? (object)DBNull.Value),
                                         new SqlParameter("CompanyNo", (object) modal.CompanyNo?? (object)DBNull.Value),
                                                         };
                var result = _repository.ExecuteSQL<string>("InsertNewAttorneyFromStep5", param).FirstOrDefault();

                if (result != null)
                {
                    response.Success = true;
                    response.str_ResponseData = result;
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
