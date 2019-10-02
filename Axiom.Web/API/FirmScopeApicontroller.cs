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
    public class FirmScopeApicontroller : ApiController
    {
        #region Initialization
        private readonly GenericRepository<CountyEntity> _repository = new GenericRepository<CountyEntity>();

        //temp        
        #endregion

        [HttpGet]
        [Route("GetFirmScopeList")]
        public ApiResponse<FirmScopeEntity> GetFirmScopeList(string FirmID = "", int RecTypeID = 0)
        {
            var response = new ApiResponse<FirmScopeEntity>();

            try
            {
                SqlParameter[] param = {
                                          new SqlParameter("FirmID", (object)FirmID ?? (object)DBNull.Value)
                                        , new SqlParameter("RecTypeID", (object)RecTypeID ?? (object)DBNull.Value)
                    };
                var result = _repository.ExecuteSQL<FirmScopeEntity>("GetFirmDefaultScope", param).ToList();
                if (result == null)
                {
                    result = new List<FirmScopeEntity>();
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
        [Route("GetScopeByRecType")]
        public ApiResponse<ScopeEntity> GetScopeByRecType(int RecType)
        {
            var response = new ApiResponse<ScopeEntity>();
            try
            {
                SqlParameter[] param = { new SqlParameter("RecType", (object)RecType ?? (object)DBNull.Value)  };
                var result = _repository.ExecuteSQL<ScopeEntity>("GetScopeByRecType", param).ToList();
                if (result == null)
                {
                    result = new List<ScopeEntity>();
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
        [Route("InsertFirmDefaultScope")]
        public BaseApiResponse InsertFirmDefaultScope(FirmScopeEntity model)
        {
            var response = new BaseApiResponse();
            try
            {

                SqlParameter[] param = {new SqlParameter("firmId", (object)model.FirmID?? (object)DBNull.Value)
                                        ,new SqlParameter("recTypeId", (object)model.RecTypeID?? (object)DBNull.Value)
                                        , new SqlParameter("recTypeName", (object)model.RecTypeName ?? (object)DBNull.Value)
                                        , new SqlParameter("scope", (object)model.Scope ?? (object)DBNull.Value)
                                        , new SqlParameter("createdBy", (object)model.CreatedBy ?? (object)DBNull.Value)
                                        };
                var result = _repository.ExecuteSQL<int>("InsertFirmDefaultScope", param).FirstOrDefault();
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
        [Route("UpdateFirmDefaultScope")]
        public BaseApiResponse UpdateFirmDefaultScope(FirmScopeEntity model)
        {
            var response = new BaseApiResponse();
            try
            {

                SqlParameter[] param = { new SqlParameter("mapId", (object)model.MapID?? (object)DBNull.Value)
                                        ,new SqlParameter("firmId", (object)model.FirmID?? (object)DBNull.Value)
                                        ,new SqlParameter("recTypeId", (object)model.RecTypeID?? (object)DBNull.Value)
                                        ,new SqlParameter("recTypeName", (object)model.RecTypeName ?? (object)DBNull.Value)
                                        ,new SqlParameter("scope", (object)model.Scope ?? (object)DBNull.Value)
                                        , new SqlParameter("updatedBy", (object)model.CreatedBy ?? (object)DBNull.Value)
                                        };
                var result = _repository.ExecuteSQL<int>("UpdateFirmDefaultScope", param).FirstOrDefault();
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
        [Route("GetFirmScopeById")]
        public ApiResponse<FirmScopeEntity> GetFirmScopeById(int mapId=0)
        {
            var response = new ApiResponse<FirmScopeEntity>();
            try
            {

                SqlParameter[] param = { new SqlParameter("mapId", (object)mapId?? (object)DBNull.Value)};
                var result = _repository.ExecuteSQL<FirmScopeEntity>("GetFirmScopeById", param).ToList();
                if (result == null)
                {
                    result = new List<FirmScopeEntity>();
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
        [Route("DeleteFirmScope")]
        public BaseApiResponse DeleteFirmScope(int mapId = 0)
        {
            var response = new BaseApiResponse();
            try
            {
                SqlParameter[] param = { new SqlParameter("mapId", (object)mapId ?? (object)DBNull.Value) };
                var result = _repository.ExecuteSQL<int>("DeleteFirmScope", param).FirstOrDefault();
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