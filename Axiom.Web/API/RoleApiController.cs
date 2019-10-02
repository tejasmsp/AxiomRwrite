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
    public class RoleApiController : ApiController
    {
        #region Initialization
        private readonly GenericRepository<RoleEntity> _repository = new GenericRepository<RoleEntity>();
        #endregion


        #region Role Database Methods
        [HttpGet]
        [Route("GetRole")]
        public ApiResponse<RoleEntity> GetRole(int roleAccessId=0)
        {
            var response = new ApiResponse<RoleEntity>();

            try
            {
                SqlParameter[] param = {new SqlParameter("RoleAccessId", (object)roleAccessId ?? (object)DBNull.Value) };
                var result = _repository.ExecuteSQL<RoleEntity>("GetRole", param).ToList();

                if (result == null)
                {
                    result = new List<RoleEntity>();
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
        [Route("InsertRole")]
        public BaseApiResponse InsertRole(RoleEntity model)
        {
            var response = new BaseApiResponse();
            try
            {
                SqlParameter[] param = { 
                                          new SqlParameter("RoleName", (object)model.RoleName ?? (object)DBNull.Value)
                                        , new SqlParameter("Description", (object)model.Description ?? (object)DBNull.Value)
                                        , new SqlParameter("IsActive", (object)model.IsActive ?? (object)DBNull.Value)
                                        , new SqlParameter("CreatedBy", (object)model.CreatedBy ?? (object)DBNull.Value)
                                        };

                var result = _repository.ExecuteSQL<string>("InsertRole", param).FirstOrDefault();
                if (String.IsNullOrEmpty(result))
                {
                    response.Success = true;
                }else 
                {
                    response.Message.Add(result);
                }
            }
            catch (Exception ex)
            {
                response.Message.Add(ex.Message);
            }

            return response;
        }


        [HttpPost]
        [Route("UpdateRole")]
        public BaseApiResponse UpdateRole(RoleEntity model)
        {
            var response = new BaseApiResponse();
            try
            {
                SqlParameter[] param = { new SqlParameter("RoleAccessId", (object)model.RoleAccessId ?? (object)DBNull.Value)
                                        , new SqlParameter("RoleId", (object)model.RoleId ?? (object)DBNull.Value)
                                        , new SqlParameter("RoleName", (object)model.RoleName ?? (object)DBNull.Value)
                                        , new SqlParameter("Description", (object)model.Description ?? (object)DBNull.Value)
                                        , new SqlParameter("IsActive", (object)model.IsActive ?? (object)DBNull.Value)
                                        , new SqlParameter("CreatedBy", (object)model.CreatedBy ?? (object)DBNull.Value)
                                        };
                var result = _repository.ExecuteSQL<string>("UpdateRole", param).FirstOrDefault();

                if (String.IsNullOrEmpty(result))
                {
                    response.Success = true;
                }
                else
                {
                    response.Message.Add(result);
                }
            }
            catch (Exception ex)
            {
                response.Message.Add(ex.Message);
            }

            return response;
        }

        #endregion

        #region Role Rights Database Methods        

        [HttpGet]
        [Route("GetRoleRights")]
        public ApiResponse<RoleRightsEntity> GetRoleRights(int RoleAccessId, int UserAccessId)
        {
            var response = new ApiResponse<RoleRightsEntity>();

            try
            {
                SqlParameter[] param = { new SqlParameter("RoleAccessId", (object)RoleAccessId ?? (object)DBNull.Value)
                                        ,new SqlParameter("UserAccessId", (object)UserAccessId ?? (object)DBNull.Value)};

                var result = _repository.ExecuteSQL<RoleRightsEntity>("GetRoleRights", param).ToList();

                if (result == null)
                {
                    result = new List<RoleRightsEntity>();
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
        [Route("AddOrUpdateRoleConfiguration")]
        public BaseApiResponse AddOrUpdateRoleConfiguration(int UserAccessId, List<RoleRightsEntity> RoleRightsCollection)
        {
            var response = new BaseApiResponse();
            string getRoleRightsxmlstr = ConvertToXml<RoleRightsEntity>.GetXMLString(RoleRightsCollection);

            try
            {
                SqlParameter[] param = { new SqlParameter("RoleRightsCollection", (object)getRoleRightsxmlstr ?? (object)DBNull.Value)
                                        , new SqlParameter("UserAccessId", (object)UserAccessId ?? (object)DBNull.Value)};

                var result = _repository.ExecuteSQL<int>("AddOrUpdateRoleRights", param).FirstOrDefault();

                response.Success = result > 0;
                response.InsertedId = result;

            }
            catch (Exception ex)
            {
                response.Message.Add(ex.Message);
            }

            return response;
        }

        #endregion

        #region User Permission
        [HttpGet]
        [Route("GetUserPermissions")]
        public ApiResponse<RoleRightsEntity> GetUserPermissions(int UserAccessId)
        {
            var response = new ApiResponse<RoleRightsEntity>();

            try
            {
                SqlParameter[] param = { new SqlParameter("UserAccessId", (object)UserAccessId ?? (object)DBNull.Value) };
                var result = _repository.ExecuteSQL<RoleRightsEntity>("GetUserPermissionsByUserAccessId", param).ToList();

                if (result == null)
                {
                    result = new List<RoleRightsEntity>();
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
        #endregion
    }
}
