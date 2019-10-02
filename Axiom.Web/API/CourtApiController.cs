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
    public class CourtApiController : ApiController
    {
        #region Initialization
        private readonly GenericRepository<CourtEntity> _repository = new GenericRepository<CourtEntity>();
        #endregion

        #region DatabaseOperations
        [HttpGet]
        [Route("GetCourtList")]
        public ApiResponse<CourtEntity> GetCourtList(int? CourtId)
        {
            var response = new ApiResponse<CourtEntity>();

            try
            {
                SqlParameter[] param = { new SqlParameter("CourtId", (object)CourtId ?? (object)DBNull.Value) };
                var result = _repository.ExecuteSQL<CourtEntity>("GetCourtList", param).ToList();
                // var result = _repository.ExecuteSQL<AttorneyUsersEntity>("AttorneyUserGetList").ToList();
                if (result == null)
                {
                    result = new List<CourtEntity>();
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
        [Route("InsertCourt")]
        public BaseApiResponse InsertCourt(CourtEntity model)
        {
            var response = new BaseApiResponse();
            try
            {
                SqlParameter[] param = { new SqlParameter("CourtType", (object)model.CourtType ?? (object)DBNull.Value)
                                        , new SqlParameter("StateID", (object)model.StateID ?? (object)DBNull.Value)
                                        , new SqlParameter("DistrictID", (object)model.DistrictID ?? (object)DBNull.Value)
                                        , new SqlParameter("CountyID", (object)model.CountyID ?? (object)DBNull.Value)
                                        , new SqlParameter("CourtName", (object)model.CourtName ?? (object)DBNull.Value)
                                        , new SqlParameter("IsActive", (object)model.IsActive ?? (object)DBNull.Value)
                                        , new SqlParameter("CreatedBy", (object)model.CreatedBy ?? (object)DBNull.Value)
                                        };
                var result = _repository.ExecuteSQL<int>("InsertCourt", param).FirstOrDefault();
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
        [Route("UpdateCourt")]
        public BaseApiResponse UpdateCourt(CourtEntity model)
        {
            var response = new BaseApiResponse();
            try
            {
                SqlParameter[] param = { new SqlParameter("CourtID", (object)model.CourtID ?? (object)DBNull.Value)
                                        , new SqlParameter("CourtType", (object)model.CourtType ?? (object)DBNull.Value)
                                        , new SqlParameter("StateID", (object)model.StateID ?? (object)DBNull.Value)
                                        , new SqlParameter("DistrictID", (object)model.DistrictID ?? (object)DBNull.Value)
                                        , new SqlParameter("CountyID", (object)model.CountyID ?? (object)DBNull.Value)
                                        , new SqlParameter("CourtName", (object)model.CourtName ?? (object)DBNull.Value)
                                        , new SqlParameter("IsActive", (object)model.IsActive ?? (object)DBNull.Value)
                                        , new SqlParameter("UpdatedBy", (object)model.UpdatedBy ?? (object)DBNull.Value)
                                        };
                var result = _repository.ExecuteSQL<int>("UpdateCourt", param).FirstOrDefault();
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
        [Route("DeleteCourt")]
        public BaseApiResponse DeleteCourt(int CourtID)
        {
            var response = new BaseApiResponse();
            try
            {
                SqlParameter[] param = { new SqlParameter("CourtID", (object)CourtID ?? (object)DBNull.Value) };
                var result = _repository.ExecuteSQL<int>("DeleteCourt", param).FirstOrDefault();
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

        #endregion
    }
}