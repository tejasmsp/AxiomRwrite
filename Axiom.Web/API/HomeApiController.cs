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
        public ApiResponse<HomeEntity> GetActionDateStatus()
        {
            var response = new ApiResponse<HomeEntity>();

            try
            {
                var result = _repository.ExecuteSQL<HomeEntity>("GetActionDateStatus").ToList();
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

        #endregion
    }
}