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
    public class CountyApicontroller : ApiController
    {
        #region Initialization
        private readonly GenericRepository<CountyEntity> _repository = new GenericRepository<CountyEntity>();

        //temp        
        #endregion

        [HttpGet]
        [Route("GetCountyList")]
        public ApiResponse<CountyEntity> GetCountyList(int countyId = 0)
        {
            var response = new ApiResponse<CountyEntity>();

            try
            {
                SqlParameter[] param = { new SqlParameter("countyId", (object)countyId ?? (object)DBNull.Value) };
                var result = _repository.ExecuteSQL<CountyEntity>("GetCountyList", param).ToList();
                if (result == null)
                {
                    result = new List<CountyEntity>();
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
        [Route("InsertCounty")]
        public BaseApiResponse InsertCounty(CountyEntity model)
        {
            var response = new BaseApiResponse();
            try
            {

                SqlParameter[] param = { new SqlParameter("countyName", (object)model.CountyName ?? (object)DBNull.Value)
                                        , new SqlParameter("stateID", (object)model.StateId ?? (object)DBNull.Value)                                                                               
                                        , new SqlParameter("isActive", (object)model.IsActive ?? (object)DBNull.Value)
                                        , new SqlParameter("createdBy", (object)model.CreatedBy ?? (object)DBNull.Value)
                                        };
                var result = _repository.ExecuteSQL<int>("InsertCounty", param).FirstOrDefault();
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
        [Route("UpdateCounty")]
        public BaseApiResponse UpdateCounty(CountyEntity model)
        {
            var response = new BaseApiResponse();
            try
            {

                SqlParameter[] param = {new SqlParameter("countyID", (object)model.CountyId ?? (object)DBNull.Value)
                                        ,new SqlParameter("countyName", (object)model.CountyName ?? (object)DBNull.Value)
                                        , new SqlParameter("stateID", (object)model.StateId ?? (object)DBNull.Value)
                                        , new SqlParameter("isActive", (object)model.IsActive ?? (object)DBNull.Value)
                                        , new SqlParameter("updatedBy", (object)model.CreatedBy ?? (object)DBNull.Value)
                                        };
                var result = _repository.ExecuteSQL<int>("UpdateCounty", param).FirstOrDefault();
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
        [Route("DeleteCounty")]
        public BaseApiResponse DeleteCounty(int countyId=0 )
        {
            var response = new BaseApiResponse();
            try
            {
                SqlParameter[] param = { new SqlParameter("countyId", (object)countyId ?? (object)DBNull.Value)};
                var result = _repository.ExecuteSQL<int>("DeleteCounty", param).FirstOrDefault();
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
        [Route("CheckUniqueCounty")]
        public BaseApiResponse CheckUniqueCounty(CountyEntity model)
        {
            var response = new BaseApiResponse();
            try
            {

                SqlParameter[] param = {
                    new SqlParameter("countyId", (object)model.CountyId ?? (object)DBNull.Value)
                                        ,new SqlParameter("countyName", (object)model.CountyName ?? (object)DBNull.Value)
                                        , new SqlParameter("stateId", (object)model.StateId ?? (object)DBNull.Value)
                                        };
                var result = _repository.ExecuteSQL<int>("CheckUniqueCounty", param).FirstOrDefault();
                if (result == 0)
                {
                    response.Success = false;
                }
                else
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