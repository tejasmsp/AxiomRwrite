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
using System.Configuration;
using System.IO;
using System.Threading.Tasks;

namespace Axiom.Web.API
{
    [RoutePrefix("api")]
    public class OrderNoteApiController : ApiController
    {
        #region Initialization
        private readonly GenericRepository<OrderPartEntity> _repository = new GenericRepository<OrderPartEntity>();
        #endregion
        //InsertOrderDocument
        #region Method
        [HttpGet]
        [Route("GetOrderNotes")]
        public ApiResponse<OrderNoteEntity> GetOrderNotes(int OrderId=0 ,int PartNo=0)
        {
            var response = new ApiResponse<OrderNoteEntity>();
            try
            {
                SqlParameter[] param = {   new SqlParameter("OrderId", (object)OrderId?? (object)DBNull.Value)
                                          ,new SqlParameter("PartNo",(object)PartNo?? (object)DBNull.Value)
                                       };
                var result = _repository.ExecuteSQL<OrderNoteEntity>("GetOrderNotes", param).ToList();
                if (result == null)
                {
                    result = new List<OrderNoteEntity>();
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
        [Route("InsertOrderNotes")]
        public BaseApiResponse InsertOrderNotes(OrderNoteEntity modal)
        {
            var response = new BaseApiResponse();
            try
            {
                Guid UserId = new Guid(modal.UserId);
                SqlParameter[] param = {   new SqlParameter("OrderId", (object)modal.OrderId?? (object)DBNull.Value)
                                          ,new SqlParameter("PartNo",(object)modal.PartNo?? (object)DBNull.Value)
                                          ,new SqlParameter("NotesClient",(object)modal.NotesClient?? (object)DBNull.Value)
                                          ,new SqlParameter("NotesInternal",(object)modal.NotesInternal?? (object)DBNull.Value)
                                          ,new SqlParameter("UserId",(object)UserId?? (object)DBNull.Value)
                                       };
                var result = _repository.ExecuteSQL<int>("InsertOrderNotes", param).FirstOrDefault();
                if (result == 1)
                {
                    response.Success = true;
                    response.InsertedId = result;
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
