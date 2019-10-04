using Axiom.Common;
using Axiom.Data.Repository;
using Axiom.Entity;
using AXIOM.Common;
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
    public class LoginApiController : ApiController
    {

        #region Initialization
        private readonly GenericRepository<LoginUserEntity> _repository = new GenericRepository<LoginUserEntity>();
        #endregion


        #region Methods

        [HttpPost]
        [Route("LoginUser")]
        public ApiResponse<LoginUserEntity> LoginUser(LoginUserEntity model)
        {
            var response = new ApiResponse<LoginUserEntity>();

            try
            {
                SqlParameter[] param = { new SqlParameter("Email", (object)model.Email ?? (object)DBNull.Value)
                                        ,new SqlParameter("Password", (object)Security.Encrypt(model.Password) ?? (object)DBNull.Value)
                                        ,new SqlParameter("CompNo", (object)ProjectSession.CompanyUserDetail.CompNo ?? (object)DBNull.Value)
                };
                var result = _repository.ExecuteSQL<LoginUserEntity>("LoginUser", param).ToList();
                if (result == null)
                {
                    result = new List<LoginUserEntity>();
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

        [Route("GetUserType")]
        public ApiResponse<string> GetUserType(string UserID)
        {
            var response = new ApiResponse<string>();

            try
            {
                SqlParameter[] param = { new SqlParameter("UserID", (object)UserID ?? (object)DBNull.Value) };
                var result = _repository.ExecuteSQL<string>("GetUserType", param).ToList();
                if (result == null)
                {
                    result = new List<string>();
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
        [Route("GetUserDetailByEmail")]
        public ApiResponse<LoginUserEntity> GetUserDetailByEmail(LoginUserEntity model)
        {
            var response = new ApiResponse<LoginUserEntity>();

            try
            {
                SqlParameter[] param = { new SqlParameter("Email", (object)model.Email ?? (object)DBNull.Value) };

                var result = _repository.ExecuteSQL<LoginUserEntity>("GetUserDetailByEmail", param).ToList();
                if (result == null)
                {
                    result = new List<LoginUserEntity>();
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
        [Route("UpdateNewGereatedPassword")]
        public ApiResponse<int> UpdateNewGereatedPassword(LoginUserEntity model)
        {
            var response = new ApiResponse<int>();

            try
            {
                SqlParameter[] param = { new SqlParameter("UserAccessId", (object)model.UserAccessId ?? (object)DBNull.Value)
                                        ,new SqlParameter("Password", (object)model.Password ?? (object)DBNull.Value)
                                        };

                var result = _repository.ExecuteSQL<int>("UpdateNewGereatedPassword", param).FirstOrDefault();
                if (result == 1)
                {
                    response.Success = true;
                }
                else
                {
                    response.Success = false;
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
