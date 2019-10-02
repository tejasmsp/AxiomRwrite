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
    public class MiscChargesApiController : ApiController
    {
        #region Initialization
        private readonly GenericRepository<MiscChargesEntity> _repository = new GenericRepository<MiscChargesEntity>();
        #endregion

        #region Method
        [HttpGet]
        [Route("GetMiscChargesByOrderId")]
        public ApiResponse<MiscChargesEntity> GetMiscChargesByOrderId(long OrderId=0,int PartNo=0)
        {
            var response = new ApiResponse<MiscChargesEntity>();

            try
            {
                SqlParameter[] param = { new SqlParameter("orderId", (object)OrderId ?? (object)DBNull.Value)
                                        ,new SqlParameter("partNo", (object)PartNo ?? (object)DBNull.Value)
                };
                var result = _repository.ExecuteSQL<MiscChargesEntity>("GetMiscChargesByOrderId", param).ToList();

                if (result == null)
                {
                    result = new List<MiscChargesEntity>();
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
        [Route("GetMiscChrgsDropDown")]
        public ApiResponse<MiscChargesEntity> GetMiscChrgsDropDown()
        {
            var response = new ApiResponse<MiscChargesEntity>();

            try
            {
                //SqlParameter[] param = { new SqlParameter("orderId", (object)OrderId ?? (object)DBNull.Value) };
                var result = _repository.ExecuteSQL<MiscChargesEntity>("GetMiscChrgsDropDown").ToList();

                if (result == null)
                {
                    result = new List<MiscChargesEntity>();
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
        [Route("GetMiscChargeAttorneyDropDown")]
        public ApiResponse<AttorneyEntity> GetMiscChargeAttorneyDropDown(long OrderId)
        {
            var response = new ApiResponse<AttorneyEntity>();

            try
            {
                SqlParameter[] param = { new SqlParameter("orderId", (object)OrderId ?? (object)DBNull.Value) };
                var result = _repository.ExecuteSQL<AttorneyEntity>("GetMiscChargeAttorneyDropDown",param).ToList();

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

        [HttpPost]
        [Route("SaveMiscCharges")]
        public BaseApiResponse SaveMiscCharges(MiscChargesEntity model )
        {
            var response = new BaseApiResponse();

            try
            {
                SqlParameter[] param = {  new SqlParameter("MiscChrgId", (object)model.MiscChrgId ?? (object)DBNull.Value)
                                         ,new SqlParameter("OrderId", (object)model.OrderId ?? (object)DBNull.Value)
                                         ,new SqlParameter("PartNoStr", (object)model.PartNoStr ?? (object)DBNull.Value)
                                         ,new SqlParameter("BillToAttorneyName", (object)model.BillToAttorneyName ?? (object)DBNull.Value)
                                         ,new SqlParameter("Descr", (object)model.Descr ?? (object)DBNull.Value)
                                         ,new SqlParameter("Units", (object)model.Units?? (object)DBNull.Value)
                                         ,new SqlParameter("RegFee", (object)model.RegFee ?? (object)DBNull.Value)
                                         ,new SqlParameter("Amount", (object)model.Amount ?? (object)DBNull.Value)
                                         ,new SqlParameter("EmpId", (object)model.EmpId ?? (object)DBNull.Value)
                };

                var result = _repository.ExecuteSQL<int>("SaveMiscCharges", param).FirstOrDefault();
                if (result > 0)
                {
                    response.Success = true;
                    response.InsertedId = result;
                }

            }
            catch (Exception ex)
            {
                response.Message.Add(ex.Message);
            }

            return response;

        }

        [HttpGet]
        [Route("GetMiscChargesByMiscChrgId")]
        public ApiResponse<MiscChargesEntity> GetMiscChargesByMiscChrgId(int MiscChrgId)
        {
            var response = new ApiResponse<MiscChargesEntity>();

            try
            {
                SqlParameter[] param = { new SqlParameter("MiscChrgId", (object)MiscChrgId ?? (object)DBNull.Value) };
                var result = _repository.ExecuteSQL<MiscChargesEntity>("GetMiscChargesByMiscChrgId", param).ToList();

                if (result == null)
                {
                    result = new List<MiscChargesEntity>();
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
        [Route("DeleteMiscChargesByMiscChrgId")]
        public BaseApiResponse DeleteMiscChargesByMiscChrgId(int MiscChrgId)
        {
            var response = new BaseApiResponse();
            try
            {
                SqlParameter[] param = {  new SqlParameter("MiscChrgId", (object)MiscChrgId ?? (object)DBNull.Value)};

                var result = _repository.ExecuteSQL<int>("DeleteMiscChargesByMiscChrgId", param).FirstOrDefault();
                if (result == 1)
                {
                    response.Success = true;
                    response.InsertedId = result;
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
