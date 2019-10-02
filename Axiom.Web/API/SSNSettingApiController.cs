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
    public class SSNSettingApiController : ApiController
    {
        #region Initialization
        private readonly GenericRepository<SSNSettings> _repository = new GenericRepository<SSNSettings>();
        #endregion

        #region DatabaseOperations
        [HttpGet]
        [Route("GetSSNSettingList")]
        public ApiResponse<SSNSettings> GetSSNSettingList()
        {
            var response = new ApiResponse<SSNSettings>();

            try
            {
                var result = _repository.ExecuteSQL<SSNSettings>("GetSSNSettingList").ToList();
                
                if (result == null)
                {
                    result = new List<SSNSettings>();
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
        [Route("UpdateSSNSetting")]
        public BaseApiResponse UpdateSSNSetting(SSNSettings model)
        {
            var response = new BaseApiResponse();
            try
            {
                SqlParameter[] param = { new SqlParameter("SettingID", (object)model.SettingID ?? (object)DBNull.Value)
                                        , new SqlParameter("SettingValue", (object)model.SettingValue ?? (object)DBNull.Value)
                                        };
                var result = _repository.ExecuteSQL<int>("UpdateSSNSetting", param).FirstOrDefault();
                if (result > 0)
                {
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