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
    public class MiscellaneousChargesApiController : ApiController
    {
        #region Initialization
        private readonly GenericRepository<MiscellaneousChargesEntity> _repository = new GenericRepository<MiscellaneousChargesEntity>();
        #endregion

        #region DatabaseOperations
        [HttpGet]
        [Route("GetMiscDescById")]
        public ApiResponse<MiscellaneousChargesEntity> GetMiscDescById(string MemberID)
        {
            var response = new ApiResponse<MiscellaneousChargesEntity>();
            try
            {


                SqlParameter[] param = { new SqlParameter("MemberID", (object)MemberID ?? (object)DBNull.Value) };

                var result = _repository.ExecuteSQL<MiscellaneousChargesEntity>("GetMiscDescriptionById", param).ToList();

                if (result == null)
                {
                    result = new List<MiscellaneousChargesEntity>();
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
        [Route("GetMiscellaneousCharges")]
        public ApiResponse<MiscellaneousChargesEntity> GetMiscellaneousCharges(int MiscId)
        {
            var response = new ApiResponse<MiscellaneousChargesEntity>();
            try
            {


                SqlParameter[] param = { new SqlParameter("MemberID", (object)MiscId ?? (object)DBNull.Value) };

                var result = _repository.ExecuteSQL<MiscellaneousChargesEntity>("GetMiscellaneousCharges", param).ToList();

                if (result == null)
                {
                    result = new List<MiscellaneousChargesEntity>();
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
        [Route("InsertMiscCharges")]
        public BaseApiResponse InsertMiscCharges(MiscellaneousChargesEntity model)
        {
            var response = new BaseApiResponse();

            try
            {
                SqlParameter[] param = {new SqlParameter("MemberID", (object)model.MemberID ?? (object)DBNull.Value)
                                        ,new SqlParameter("MiscDesc", (object)model.MiscDesc ?? (object)DBNull.Value)
                                        ,new SqlParameter("RegularFee", (object)model.RegularFee ?? (object)DBNull.Value)
                                        ,new SqlParameter("DiscountFee", (object)model.DiscountFee ?? (object)DBNull.Value)                                     
                                        ,new SqlParameter("CreatedBy", (object)model.CreatedBy ?? (object)DBNull.Value)
                };
                var result = _repository.ExecuteSQL<int>("InsertMiscCharges", param).FirstOrDefault();

                if (result > 0)
                {
                    response.Success = true;                    
                }
                else
                {
                    response.Success = false;
                    response.Message.Add("Member and Misc Description already assigned.");
                }
            }
            catch (Exception ex)
            {
                response.Message.Add(ex.Message);
            }

            return response;
        }
        [HttpPost]
        [Route("UpdateMiscCharges")]
        public BaseApiResponse UpdateMiscCharges(MiscellaneousChargesEntity model)
        {
            var response = new BaseApiResponse();

            try
            {
                SqlParameter[] param = {new SqlParameter("FeeNo", (object)model.FeeNo ?? (object)DBNull.Value)
                                        ,new SqlParameter("MemberID", (object)model.MemberID ?? (object)DBNull.Value)
                                        ,new SqlParameter("MiscDesc", (object)model.MiscDesc ?? (object)DBNull.Value)
                                        ,new SqlParameter("RegularFee", (object)model.RegularFee ?? (object)DBNull.Value)
                                        ,new SqlParameter("DiscountFee", (object)model.DiscountFee ?? (object)DBNull.Value)
                                        ,new SqlParameter("CreatedBy", (object)model.CreatedBy ?? (object)DBNull.Value)
                };
                var result = _repository.ExecuteSQL<int>("UpdateMiscCharges", param).FirstOrDefault();

                if (result > 0)
                {
                    response.Success = true;
                }
                else
                {
                    response.Success = false;
                    response.Message.Add("Member and Misc Description already assigned.");
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