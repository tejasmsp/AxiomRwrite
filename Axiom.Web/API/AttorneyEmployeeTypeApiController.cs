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
    public class AttornerEmployeeTypeController : ApiController
    {
        #region Initialization
        private readonly GenericRepository<AttornerEmployeeTypeEntity> _repository = new GenericRepository<AttornerEmployeeTypeEntity>();
        #endregion

        #region DatabaseOperations
        [HttpGet]
        [Route("GetAttorneyEmployeeType")]
        public ApiResponse<AttornerEmployeeTypeEntity> GetAttorneyEmployeeType()
        {
            var response = new ApiResponse<AttornerEmployeeTypeEntity>();

            try
            {
                var result = _repository.ExecuteSQL<AttornerEmployeeTypeEntity>("GetAttorneyEmployeeType").ToList();
                if (result == null)
                {
                    result = new List<AttornerEmployeeTypeEntity>();
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
