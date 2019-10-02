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
    public class AccountExecutiveApiController : ApiController
    {
        #region Initialization
        private readonly GenericRepository<AccountExecutive> _repository = new GenericRepository<AccountExecutive>();
        #endregion

        #region DatabaseOperations
        [HttpGet]
        [Route("GetClientAccountExecutive")]
        public ApiResponse<AccountExecutive> GetClientAccountExecutive(Guid UserId)
        {
            var response = new ApiResponse<AccountExecutive>();

            try
            {
                SqlParameter[] param = { new SqlParameter("UserId", (object)UserId ?? (object)DBNull.Value) };
                var result = _repository.ExecuteSQL<AccountExecutive>("GetClientAcctExec", param).ToList();
                // var result = _repository.ExecuteSQL<AttorneyUsersEntity>("AttorneyUserGetList").ToList();
                if (result == null)
                {
                    result = new List<AccountExecutive>();
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