﻿using Axiom.Common;
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
    public class SettingApiController : ApiController
    {
        #region Initialization
        private readonly GenericRepository<LogDetailEntity> _repository = new GenericRepository<LogDetailEntity>();
        #endregion

        [HttpGet]
        [Route("GetLogList")]
        public ApiResponse<LogDetailEntity> GetLogList(string userId, string startDate, string endDate, int CompanyNo)
        {
            var response = new ApiResponse<LogDetailEntity>();
            try
            {
                SqlParameter[] param = {  new SqlParameter("userId", (object)userId?? (object)DBNull.Value)
                                                 ,new SqlParameter("bd",(object)startDate?? (object)DBNull.Value)
                                                 ,new SqlParameter("ed",(object)endDate?? (object)DBNull.Value)
                                                 ,new SqlParameter("CompanyNo",(object)CompanyNo?? (object)DBNull.Value)
                                         };
                var result = _repository.ExecuteSQL<LogDetailEntity>("GetEmployeeLogs", param).ToList();

                if (result == null)
                {
                    result = new List<LogDetailEntity>();
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
        [Route("GetDepartmentEmpoyeeLog")]
        public ApiResponse<LogFilterEntity> GetDepartmentEmpoyeeLog(string userId,string DepartmentId, string startDate, string endDate, int CompanyNo)
        {
            var response = new ApiResponse<LogFilterEntity>();
            try
            {
                SqlParameter[] param = {          new SqlParameter("userId", (object)userId?? (object)DBNull.Value)
                                                 ,new SqlParameter("DepartmentId", (object)DepartmentId?? (object)DBNull.Value)
                                                 ,new SqlParameter("FromDate",(object)startDate?? (object)DBNull.Value)
                                                 ,new SqlParameter("ToDate",(object)endDate?? (object)DBNull.Value)
                                                 ,new SqlParameter("CompanyNo",(object)CompanyNo?? (object)DBNull.Value)
                                         };
                var result = _repository.ExecuteSQL<LogFilterEntity>("GetDepartmentEmpoyeeLog", param).ToList();

                if (result == null)
                {
                    result = new List<LogFilterEntity>();
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
    }
}