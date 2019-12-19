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
    public class HomeApiController : ApiController
    {
        #region Initialization
        private readonly GenericRepository<HomeEntity> _repository = new GenericRepository<HomeEntity>();
        #endregion

        #region DatabaseOperations
        [HttpGet]
        [Route("GetActionDateStatus")]
        public ApiResponse<HomeEntity> GetActionDateStatus(int? CompanyNo)
        {
            var response = new ApiResponse<HomeEntity>();

            try
            {
                SqlParameter[] param = { new SqlParameter("CompanyNo", (object)CompanyNo ?? (object)DBNull.Value) };
                var result = _repository.ExecuteSQL<HomeEntity>("GetActionDateStatus", param).ToList();
                if (result == null)
                {
                    result = new List<HomeEntity>();
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
        [Route("GetCompanyNoBySiteUrl")]
        public ApiResponse<int> GetCompanyNoBySiteUrl(string SiteUrl)
        {
            var response = new ApiResponse<int>();
            try
            {
                SqlParameter[] param = { new SqlParameter("SiteUrl", (object)SiteUrl ?? (object)DBNull.Value) };
                var result = _repository.ExecuteSQL<int>("GetCompanyNoBySiteUrl", param).ToList();
                if (result == null)
                {
                    result = new List<int>();
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