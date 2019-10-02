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
    public class UserAttorneyMappingApiController : ApiController
    {
        #region Initialization
        private readonly GenericRepository<UserAttorneyMappingEntity> _repository = new GenericRepository<UserAttorneyMappingEntity>();
        #endregion

        #region DatabaseOperations
        [HttpGet]
        [Route("GetUserAttorneyMapping")]
        public ApiResponse<UserAttorneyMappingEntity> GetUserAttorneyMapping(Guid UserId, bool selectOnlyCurrentAccessAttorney)
        {
            var response = new ApiResponse<UserAttorneyMappingEntity>();

            try
            {
                SqlParameter[] param = {new SqlParameter("UserId", (object)UserId ?? (object)DBNull.Value)
                                        , new SqlParameter("SelectOnlyCurrentAccessAttorney", (object)selectOnlyCurrentAccessAttorney ?? (object)DBNull.Value)
                                        };
                var result = _repository.ExecuteSQL<UserAttorneyMappingEntity>("UserAttorneyMappingGet", param).ToList();
                if (result == null)
                {
                    result = new List<UserAttorneyMappingEntity>();
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
        [Route("DeleteAttorneyUserMapping")]
        public BaseApiResponse DeleteAttorneyUserMapping(string attorneyUserId, string attorneyId)
        {
            var response = new BaseApiResponse();
            try
            {
                SqlParameter[] param = { new SqlParameter("AttorneyUserId", (object)attorneyUserId ?? (object)DBNull.Value)
                                        , new SqlParameter("AttorneyId", (object)attorneyId?? (object)DBNull.Value)
                                        };
                var result = _repository.ExecuteSQL<int>("DeleteAttorneyUserMapping", param).FirstOrDefault();
                if (result > 0)
                {
                    response.InsertedId = result;
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
        [Route("InsertAttorneyUserMapping")]
        public BaseApiResponse InsertAttorneyUserMapping(string attorneyUserId, string attorneyId)
        {
            var response = new BaseApiResponse();
            try
            {
                SqlParameter[] param = { new SqlParameter("AttorneyUserId", (object)attorneyUserId ?? (object)DBNull.Value)
                                        , new SqlParameter("AttorneyId", (object)attorneyId?? (object)DBNull.Value)
                                        };
                var result = _repository.ExecuteSQL<int>("InsertAttorneyUserMapping", param).FirstOrDefault();
                if (result > 0)
                {
                    response.InsertedId = result;
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
