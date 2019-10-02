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
    public class DepartmentApiController : ApiController
    {
        #region Initialization
        private readonly GenericRepository<DepartmentEntity> _repository = new GenericRepository<DepartmentEntity>();
        #endregion

        #region DatabaseOperations
        [HttpGet]
        [Route("GetDepartmentList")]
        public ApiResponse<DepartmentEntity> GetDepartmentList(int? DepartmentID)
        {
            var response = new ApiResponse<DepartmentEntity>();

            try
            {
                SqlParameter[] param = { new SqlParameter("DepartmentId", (object)DepartmentID ?? (object)DBNull.Value) };
                var result = _repository.ExecuteSQL<DepartmentEntity>("GetDepartmentList", param).ToList();
                // var result = _repository.ExecuteSQL<AttorneyUsersEntity>("AttorneyUserGetList").ToList();
                if (result == null)
                {
                    result = new List<DepartmentEntity>();
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
        [Route("InsertDepartment")]
        public BaseApiResponse InsertDepartment(DepartmentEntity model)
        {
            var response = new BaseApiResponse();
            try
            {
                SqlParameter[] param = { new SqlParameter("Department", (object)model.Department ?? (object)DBNull.Value)
                                        , new SqlParameter("CreatedBy", (object)model.CreatedBy ?? (object)DBNull.Value)
                                        , new SqlParameter("isActive", (object)model.isActive ?? (object)DBNull.Value)
                                        };
                var result = _repository.ExecuteSQL<string>("InsertDepartment", param).FirstOrDefault();
                if (result != string.Empty)
                {
                    response.str_ResponseData = result;
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
        [Route("UpdateDepartment")]
        public BaseApiResponse UpdateDepartment(DepartmentEntity model)
        {
            var response = new BaseApiResponse();
            try
            {
                SqlParameter[] param = { new SqlParameter("DepartmentID", (object)model.DepartmentId ?? (object)DBNull.Value)
                                        , new SqlParameter("Department", (object)model.Department ?? (object)DBNull.Value)
                                        , new SqlParameter("UpdatedBy", (object)model.UpdatedBy ?? (object)DBNull.Value)
                                        , new SqlParameter("IsActive", (object)model.isActive ?? (object)DBNull.Value)

                                        };
                var result = _repository.ExecuteSQL<string>("UpdateDepartment", param).FirstOrDefault();
                if (!string.IsNullOrEmpty(result))
                {
                    response.str_ResponseData = result;
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
        [Route("DeleteDepartment")]
        public BaseApiResponse DeleteDepartment(int DepartmentID)
        {
            var response = new BaseApiResponse();
            try
            {
                SqlParameter[] param = { new SqlParameter("DepartmentID", (object)DepartmentID ?? (object)DBNull.Value) };
                var result = _repository.ExecuteSQL<string>("DeleteDepartment", param).FirstOrDefault();
                if (!string.IsNullOrEmpty(result))
                {
                    response.str_ResponseData = result;
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
        [Route("UpdateDepartmentSortOrder")]
        public ApiResponse<DepartmentEntity> UpdateDepartmentSortOrder(int? DepartmentID, string Direction)
        {
            var response = new ApiResponse<DepartmentEntity>();

            try
            {
                SqlParameter[] param = { new SqlParameter("DepartmentId", (object)DepartmentID ?? (object)DBNull.Value)
                                        ,new SqlParameter("SortOrder", (object)Direction ?? (object)DBNull.Value) };
                var result = _repository.ExecuteSQL<DepartmentEntity>("UpdateDepartmentSortOrder", param).ToList();
                // var result = _repository.ExecuteSQL<AttorneyUsersEntity>("AttorneyUserGetList").ToList();
                if (result == null)
                {
                    result = new List<DepartmentEntity>();
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