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
    public class DistrictApiController : ApiController
    {
        #region Initialization
        private readonly GenericRepository<DistrictEntity> _repository = new GenericRepository<DistrictEntity>();

        //temp        
        #endregion

        [HttpGet]
        [Route("GetDistrictList")]
        public ApiResponse<DistrictEntity> GetDistrictList(int districtid = 0)
        {
            var response = new ApiResponse<DistrictEntity>();

            try
            {
                SqlParameter[] param = { new SqlParameter("districtid", (object)districtid ?? (object)DBNull.Value) };
                var result = _repository.ExecuteSQL<DistrictEntity>("GetDistrictList", param).ToList();
                if (result == null)
                {
                    result = new List<DistrictEntity>();
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
        [Route("InsertDistrict")]
        public BaseApiResponse InsertDistrict(DistrictEntity model)
        {
            var response = new BaseApiResponse();
            try
            {

                SqlParameter[] param = { new SqlParameter("districtname", (object)model.DistrictName ?? (object)DBNull.Value)
                                        , new SqlParameter("stateid", (object)model.StateId ?? (object)DBNull.Value)
                                        , new SqlParameter("isactive", (object)model.IsActive ?? (object)DBNull.Value)
                                        , new SqlParameter("createdBy", (object)model.CreatedBy ?? (object)DBNull.Value)
                                        };
                var result = _repository.ExecuteSQL<int>("InsertDistrict", param).FirstOrDefault();
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
        [Route("UpdateDistrict")]
        public BaseApiResponse UpdateDistrict(DistrictEntity model)
        {
            var response = new BaseApiResponse();
            try
            {

                SqlParameter[] param = {new SqlParameter("districtid", (object)model.DistrictID ?? (object)DBNull.Value)
                                        ,new SqlParameter("districtname", (object)model.DistrictName ?? (object)DBNull.Value)
                                        , new SqlParameter("stateid", (object)model.StateId ?? (object)DBNull.Value)
                                        , new SqlParameter("isactive", (object)model.IsActive ?? (object)DBNull.Value)
                                        , new SqlParameter("updatedby", (object)model.CreatedBy ?? (object)DBNull.Value)
                                        };
                var result = _repository.ExecuteSQL<int>("UpdateDistrict", param).FirstOrDefault();
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
        [Route("CheckUniqueDistrict")]
        public BaseApiResponse CheckUniqueDistrict(DistrictEntity model)
        {
            var response = new BaseApiResponse();
            try
            {

                SqlParameter[] param = { new SqlParameter("DistrictID", (object)model.DistrictID ?? (object)DBNull.Value)
                                        ,new SqlParameter("DistrictName", (object)model.DistrictName ?? (object)DBNull.Value)
                                        , new SqlParameter("StateID", (object)model.StateId ?? (object)DBNull.Value)
                                        };
                var result = _repository.ExecuteSQL<int>("CheckUniqueDistrict", param).FirstOrDefault();
                if (result == 0)
                {
                    response.Success = false;
                }
                else
                {
                    response.Success = true ;
                }
            }
            catch (Exception ex)
            {
                response.Message.Add(ex.Message);
            }
            return response;
        }


        [HttpPost]
        [Route("DeleteDistrict")]
        public BaseApiResponse DeleteDistrict(int districtid = 0)
        {
            var response = new BaseApiResponse();
            try
            {
                SqlParameter[] param = { new SqlParameter("districtid", (object)districtid ?? (object)DBNull.Value) };
                var result = _repository.ExecuteSQL<int>("DeleteDistrict", param).FirstOrDefault();
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