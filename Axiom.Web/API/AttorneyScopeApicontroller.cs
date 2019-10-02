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
    public class AttorneyScopeApicontroller : ApiController
    {
        #region Initialization
        private readonly GenericRepository<CountyEntity> _repository = new GenericRepository<CountyEntity>();

        //temp        
        #endregion

        [HttpGet]
        [Route("GetAttorneyScopeList")]
        public ApiResponse<AttorneyScopeEntity> GetAttorneyScopeList()
        {
            var response = new ApiResponse<AttorneyScopeEntity>();

            try
            {
                var result = _repository.ExecuteSQL<AttorneyScopeEntity>("GetAttorneyDefaultScope").ToList();
                if (result == null)
                {
                    result = new List<AttorneyScopeEntity>();
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
        [Route("GetDefaultScope")]
        public ApiResponse<DefaultScopeEntity> GetDefaultScope()
        {
            var response = new ApiResponse<DefaultScopeEntity>();

            try
            {
                var result = _repository.ExecuteSQL<DefaultScopeEntity>("GetDefaultScope").ToList();
                if (result == null)
                {
                    result = new List<DefaultScopeEntity>();
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
        [Route("GetDefaultScopeById")]
        public ApiResponse<DefaultScopeEntity> GetDefaultScopeById(int ScopeID = 0)
        {
            var response = new ApiResponse<DefaultScopeEntity>();
            try
            {

                SqlParameter[] param = { new SqlParameter("ScopeID", (object)ScopeID ?? (object)DBNull.Value) };
                var result = _repository.ExecuteSQL<DefaultScopeEntity>("GetDefaultScopeById", param).ToList();
                if (result == null)
                {
                    result = new List<DefaultScopeEntity>();
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
        [Route("UpdateDefaultScope")]
        public BaseApiResponse UpdateDefaultScope(DefaultScopeEntity model)
        {
            var response = new BaseApiResponse();
            try
            {

                SqlParameter[] param = { new SqlParameter("ScopeID", (object)model.ScopeID?? (object)DBNull.Value)
                                        ,new SqlParameter("RecType", (object)model.RecType?? (object)DBNull.Value)
                                        ,new SqlParameter("ScopeDesc", (object)model.ScopeDesc?? (object)DBNull.Value)
                                         ,new SqlParameter("IsDefault", (object)model.IsDefault?? (object)DBNull.Value)
                                        };
                var result = _repository.ExecuteSQL<int>("UpdateDefaultScope", param).FirstOrDefault();
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
        [Route("InsertAttorneyScope")]
        public BaseApiResponse InsertAttorneyScope(AttorneyScopeEntity model)
        {
            var response = new BaseApiResponse();
            try
            {

                SqlParameter[] param = {new SqlParameter("attyId", (object)model.AttyID?? (object)DBNull.Value)
                                        ,new SqlParameter("recTypeId", (object)model.RecTypeID?? (object)DBNull.Value)
                                        , new SqlParameter("recTypeName", (object)model.RecTypeName ?? (object)DBNull.Value)
                                        , new SqlParameter("scope", (object)model.Scope ?? (object)DBNull.Value)
                                        , new SqlParameter("createdBy", (object)model.CreatedBy ?? (object)DBNull.Value)
                                        };
                var result = _repository.ExecuteSQL<int>("InsertAttorneyDefaultScope", param).FirstOrDefault();
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
        [Route("UpdateAttorneyScope")]
        public BaseApiResponse UpdateAttorneyScope(AttorneyScopeEntity model)
        {
            var response = new BaseApiResponse();
            try
            {

                SqlParameter[] param = { new SqlParameter("mapId", (object)model.MapID?? (object)DBNull.Value)
                                        ,new SqlParameter("attyId", (object)model.AttyID?? (object)DBNull.Value)
                                        ,new SqlParameter("recTypeId", (object)model.RecTypeID?? (object)DBNull.Value)
                                        ,new SqlParameter("recTypeName", (object)model.RecTypeName ?? (object)DBNull.Value)
                                        ,new SqlParameter("scope", (object)model.Scope ?? (object)DBNull.Value)
                                        ,new SqlParameter("updatedBy", (object)model.CreatedBy ?? (object)DBNull.Value)
                                        };
                var result = _repository.ExecuteSQL<int>("UpdateAttorneyDefaultScope", param).FirstOrDefault();
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

        [HttpGet]
        [Route("GetAttorneyScopeById")]
        public ApiResponse<AttorneyScopeEntity> GetAttorneyScopeById(int mapId=0)
        {
            var response = new ApiResponse<AttorneyScopeEntity>();
            try
            {

                SqlParameter[] param = { new SqlParameter("mapId", (object)mapId?? (object)DBNull.Value)};
                var result = _repository.ExecuteSQL<AttorneyScopeEntity>("GetAttorneyScopeById", param).ToList();
                if (result == null)
                {
                    result = new List<AttorneyScopeEntity>();
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
        [Route("DeleteAttorneyScope")]
        public BaseApiResponse DeleteAttorneyScope(int mapId = 0)
        {
            var response = new BaseApiResponse();
            try
            {
                SqlParameter[] param = { new SqlParameter("mapId", (object)mapId ?? (object)DBNull.Value) };
                var result = _repository.ExecuteSQL<int>("DeleteAttorneyScope", param).FirstOrDefault();
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