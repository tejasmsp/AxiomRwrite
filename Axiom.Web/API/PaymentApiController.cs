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
    public class PaymentApiController : ApiController
    {
        #region Initialization
        private readonly GenericRepository<PaymentEntity> _repository = new GenericRepository<PaymentEntity>();
        #endregion

        [HttpGet]
        [Route("GetPaymentList")]
        public ApiResponse<PaymentEntity> GetPaymentList(string pmtDate, int PmtNo)
        {
            var response = new ApiResponse<PaymentEntity>();

            try
            {
                SqlParameter[] param = { new SqlParameter("PmtDate", (object)pmtDate ?? (object)DBNull.Value),
                new SqlParameter("PmtNo", (object)PmtNo ?? (object)DBNull.Value)};
                var result = _repository.ExecuteSQL<PaymentEntity>("GetPaymentList", param).ToList();
                if (result == null)
                {
                    result = new List<PaymentEntity>();
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
        [Route("UpdateCounty")]
        public BaseApiResponse InsertUpdatePayment(PaymentEntity model)
        {
            var response = new BaseApiResponse();
            try
            {
                SqlParameter[] param = {
                                         new SqlParameter("PmtNo", (object)model.PmtNo ?? (object)DBNull.Value)
                                        ,new SqlParameter("PaidBy", (object)model.PaidBy ?? (object)DBNull.Value)
                                        ,new SqlParameter("ChkNo", (object)model.ChkNo ?? (object)DBNull.Value)
                                        ,new SqlParameter("PmtDate", (object)model.PmtNo ?? (object)DBNull.Value)
                                        ,new SqlParameter("ChkAmt", (object)model.ChkAmt ?? (object)DBNull.Value)
                                        };
                var result = _repository.ExecuteSQL<int>("InsertUpdatePayment", param).FirstOrDefault();
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
        [Route("DeletePayment")]
        public BaseApiResponse DeletePayment(int PmtNo = 0)
        {
            var response = new BaseApiResponse();
            try
            {
                SqlParameter[] param = { new SqlParameter("PmtNo", (object)PmtNo ?? (object)DBNull.Value) };
                var result = _repository.ExecuteSQL<int>("DeletePayment", param).FirstOrDefault();
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


    }
}