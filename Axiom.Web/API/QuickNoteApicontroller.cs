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
    public class QuickNoteApicontroller : ApiController
    {
        #region Initialization
        private readonly GenericRepository<CountyEntity> _repository = new GenericRepository<CountyEntity>();

        //temp        
        #endregion

        [HttpGet]
        [Route("GetQuickNotesList")]
        public ApiResponse<QuickNotesEntity> GetQuickNotesList(int partstatusid = 0)
        {
            var response = new ApiResponse<QuickNotesEntity>();

            try
            {
                SqlParameter[] param = { new SqlParameter("partstatusid", (object)partstatusid ?? (object)DBNull.Value) };
                var result = _repository.ExecuteSQL<QuickNotesEntity>("GetQuickNotesList", param).ToList();
                if (result == null)
                {
                    result = new List<QuickNotesEntity>();
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
        [Route("InsertQuickNotes")]
        public BaseApiResponse InsertQuickNotes(QuickNotesEntity model)
        {
            var response = new BaseApiResponse();
            try
            {
                SqlParameter[] param = { new SqlParameter("partstatus", (object)model.PartStatus ?? (object)DBNull.Value)
                                        , new SqlParameter("note", (object)model.Note ?? (object)DBNull.Value)                                       
                                        , new SqlParameter("ishidden", (object)!model.IsHidden ?? (object)DBNull.Value)
                                        , new SqlParameter("partstatusgroupid", (object)model.PartStatusGroupId ?? (object)DBNull.Value)
                                        , new SqlParameter("createdBy", (object)model.CreatedBy ?? (object)DBNull.Value)
                                        };
                var result = _repository.ExecuteSQL<int>("InsertQuickNotes", param).FirstOrDefault();
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

        [HttpPost]
        [Route("UpdateQuickNotes")]
        public BaseApiResponse UpdateQuickNotes(QuickNotesEntity model)
        {
            var response = new BaseApiResponse();
            try
            {

                SqlParameter[] param = {new SqlParameter("partstatusid", (object)model.PartStatusId?? (object)DBNull.Value)
                                        ,new SqlParameter("partstatus", (object)model.PartStatus ?? (object)DBNull.Value)
                                        , new SqlParameter("note", (object)model.Note ?? (object)DBNull.Value)
                                        , new SqlParameter("ishidden", (object)!model.IsHidden?? (object)DBNull.Value)
                                        , new SqlParameter("partstatusgroupid", (object)model.PartStatusGroupId ?? (object)DBNull.Value)
                                         , new SqlParameter("updatedBy", (object)model.CreatedBy ?? (object)DBNull.Value)
                                        };
                var result = _repository.ExecuteSQL<int>("UpdateQuickNotes", param).FirstOrDefault();
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
       
    }
}