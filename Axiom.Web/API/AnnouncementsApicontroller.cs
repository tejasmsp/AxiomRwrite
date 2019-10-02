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
    public class AnnouncementsApicontroller : ApiController
    {
        #region Initialization
        private readonly GenericRepository<CountyEntity> _repository = new GenericRepository<CountyEntity>();

        //temp        
        #endregion

        [HttpGet]
        [Route("GetDailyAnnouncement")]
        public ApiResponse<string> GetDailyAnnouncement()
        {
            var response = new ApiResponse<string>();

            try
            {
                var result = _repository.ExecuteSQL<string>("GetDailyAnnouncement").FirstOrDefault();
                if (result != null)
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
        [Route("InsertDailyAnnouncement")]
        public BaseApiResponse InsertDailyAnnouncement(string strAnnouncement)
        {
            var response = new BaseApiResponse();
            try
            {

                SqlParameter[] param = {
                            new SqlParameter("announcement", (object)strAnnouncement?? (object)DBNull.Value)
                                        };
                var result = _repository.ExecuteSQL<int>("InsertDailyAnnouncement", param).FirstOrDefault();
                if (result > 0)
                {
                    response.Success = true;
                    response.str_ResponseData = result.ToString();
                }
            }
            catch (Exception ex)
            {
                response.Message.Add(ex.Message);
            }
            return response;
        }

    }
}