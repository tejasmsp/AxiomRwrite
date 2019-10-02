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
using System.Web;
using AXIOM.Common;

namespace Axiom.Web.API
{
    [RoutePrefix("api")]
    public class UserProfileApiController : ApiController
    {
        #region Initialization
        private readonly GenericRepository<UserMasterEntity> _repository = new GenericRepository<UserMasterEntity>();
        #endregion

        #region DatabaseOperations
        [HttpGet]
        [Route("GetUserMasterList")]
        public ApiResponse<UserMasterEntity> GetUserMasterList(int UserAccessID)
        {
            var response = new ApiResponse<UserMasterEntity>();

            try
            {
                SqlParameter[] param = { new SqlParameter("UserAccessID", (object)UserAccessID ?? (object)DBNull.Value) };
                var result = _repository.ExecuteSQL<UserMasterEntity>("GetUserMasterList", param).ToList();
                if (result == null)
                {
                    result = new List<UserMasterEntity>();
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
        [Route("UploadFile")]
        public ApiResponse<UserMasterEntity> UploadFile(int UserAccessID)
        {
            var response = new ApiResponse<UserMasterEntity>();
            try
            {
                if (HttpContext.Current.Request.Files.AllKeys.Any())
                {
                    // Get the uploaded image from the Files collection
                    var httpPostedFile = HttpContext.Current.Request.Files[0];
                    byte[] data;
                    string[] ext = httpPostedFile.FileName.Split('.');
                    string[] type = { "jpg", "png", "jpeg", "bmp" };
                    if (type.Any(ext[ext.Length - 1].Contains))
                    {
                        using (System.IO.Stream inputStream = httpPostedFile.InputStream)
                        {
                            System.IO.MemoryStream memoryStream = inputStream as System.IO.MemoryStream;
                            if (memoryStream == null)
                            {
                                memoryStream = new System.IO.MemoryStream();
                                inputStream.CopyTo(memoryStream);
                            }
                            data = memoryStream.ToArray();
                        }

                        SqlParameter[] param = {new SqlParameter("UserAccessID", (object)UserAccessID ?? (object)DBNull.Value)
                                        , new SqlParameter("Photo", (object)data ?? (object)DBNull.Value)};
                        var result = _repository.ExecuteSQL<UserMasterEntity>("UpdateUserMasterImage", param).ToList();

                        if (result == null)
                        {
                            result = new List<UserMasterEntity>();
                        }
                        response.Success = true;
                        response.Data = result;
                    }
                    else
                    {
                        response.Success = true;
                        response.Data = new List<UserMasterEntity>();
                    }                    
                }
            }
            catch (Exception ex)
            {
                response.Message.Add(ex.Message);
            }
            return response;
        }
        [HttpPost]
        [Route("UpdateUserMaster")]
        public ApiResponse<UserMasterEntity> UpdateUserMaster(UserMasterEntity modal)
        {
            var response = new ApiResponse<UserMasterEntity>();

            try
            {
                // Get the uploaded image from the Files collection
                //var httpPostedFile = System.Web.HttpContext.Current.Request.Files[0];
                //byte[] data;
                //using (System.IO.Stream inputStream = httpPostedFile.InputStream)
                //{
                //    System.IO.MemoryStream memoryStream = inputStream as System.IO.MemoryStream;
                //    if (memoryStream == null)
                //    {
                //        memoryStream = new System.IO.MemoryStream();
                //        inputStream.CopyTo(memoryStream);
                //    }
                //    data = memoryStream.ToArray();
                //}


                SqlParameter[] param = {new SqlParameter("UserAccessID", (object)modal.UserAccessId ?? (object)DBNull.Value)
                                        , new SqlParameter("FirstName", (object)modal.FirstName ?? (object)DBNull.Value)
                                        , new SqlParameter("LastName", (object)modal.LastName ?? (object)DBNull.Value)
                                        // , new SqlParameter("Photo", (object)data ?? (object)DBNull.Value)
                                        };
                var result = _repository.ExecuteSQL<UserMasterEntity>("UpdateUserMaster", param).ToList();
                if (result == null)
                {
                    result = new List<UserMasterEntity>();
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
        [Route("UserMasterUpdatePassword")]
        public ApiResponse<UserMasterEntity> UserMasterUpdatePassword(UserMasterEntity modal)
        {
            var response = new ApiResponse<UserMasterEntity>();

            try
            {
                SqlParameter[] param = {new SqlParameter("UserAccessID", (object)modal.UserAccessId ?? (object)DBNull.Value)
                                        , new SqlParameter("Password", (object)Security.Encrypt(modal.Password) ?? (object)DBNull.Value)
                                        };
                var result = _repository.ExecuteSQL<UserMasterEntity>("UserMasterUpdatePassword", param).ToList();
                if (result == null)
                {
                    result = new List<UserMasterEntity>();
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
