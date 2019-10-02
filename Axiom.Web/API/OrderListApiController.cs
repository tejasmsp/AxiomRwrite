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
using AXIOM.Common;
using System.Data;
using System.Configuration;
using System.IO;
using System.Threading;

namespace Axiom.Web.API
{
    [RoutePrefix("api")]
    public class OrderListApiController : ApiController
    {
        #region Initialization
        private readonly GenericRepository<OrderListEntity> _repository = new GenericRepository<OrderListEntity>();
        #endregion

        #region DatabaseOperations
        [HttpGet]
        [Route("GetOrderList")]
        public ApiResponse<OrderListEntity> GetOrderList(int? OrderID)
        {
            var response = new ApiResponse<OrderListEntity>();

            try
            {
                SqlParameter[] param = { new SqlParameter("OrderID", (object)OrderID ?? (object)DBNull.Value) };
                var result = _repository.ExecuteSQL<OrderListEntity>("GetOrderList", param).ToList();
                // var result = _repository.ExecuteSQL<AttorneyUsersEntity>("AttorneyUserGetList").ToList();
                if (result == null)
                {
                    result = new List<OrderListEntity>();
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
        [Route("GetOrderListForClient")]
        public TableParameter<OrderListEntity> GetOrderListForClient(TableParameter<OrderListEntity> tableParameter, int PageIndex,int UserAccessId, string UserId, string SearchValue = "", string OrderID = "",
              string AttorneyID = "",
              string RecordsOf = "",
              string Caption = "",
              string Cause = "",
              string Claim = "",
              string ClaimMatterNo = "", bool HideArchived = true)
        {

            tableParameter.PageIndex = PageIndex;
            string sortColumn = tableParameter.SortColumn.Desc ? tableParameter.SortColumn.Column + " desc" : tableParameter.SortColumn.Column + " asc";
            string searchValue = SearchValue == null ? string.Empty : SearchValue.Trim();
            var response = new ApiResponse<OrderListEntity>();
            try
            {
                SqlParameter[] param = { new SqlParameter("Keyword", (object)searchValue ?? (object)DBNull.Value),
                                         new SqlParameter("PageIndex", (object)tableParameter.PageIndex ?? (object)DBNull.Value),
                                         new SqlParameter("PageSize", (object)tableParameter != null ? tableParameter.iDisplayLength : 10),
                                         new SqlParameter("SortBy", (object)sortColumn ?? (object)DBNull.Value),
                                         new SqlParameter("OrderID", (object)OrderID ?? (object)DBNull.Value),
                                         new SqlParameter("AttorneyID", (object)AttorneyID ?? (object)DBNull.Value),
                                         new SqlParameter("RecordsOf", (object)RecordsOf ?? (object)DBNull.Value),
                                         new SqlParameter("Caption", (object)Caption ?? (object)DBNull.Value),
                                         new SqlParameter("Cause", (object)Cause ?? (object)DBNull.Value),
                                         new SqlParameter("Claim", (object)Claim ?? (object)DBNull.Value),
                                         new SqlParameter("ClaimMatterNo", (object)ClaimMatterNo ?? (object)DBNull.Value),
                                         new SqlParameter("HideArchived", (object)HideArchived ?? (object)DBNull.Value),
                                         new SqlParameter("UserId", (object)UserId ?? (object)DBNull.Value),
                                         new SqlParameter("UserAccessId", (object)UserAccessId ?? (object)DBNull.Value)
                                       };
                            
                var result = _repository.ExecuteSQL<OrderListEntity>("GetOrderListForClient", param).ToList();
                response.Success = true;
                response.Data = result;

                int totalRecords = 0;
                if (response != null && response.Data.Count > 0)
                {
                    totalRecords = response.Data[0].TotalRecords;
                }

                return new TableParameter<OrderListEntity>
                {
                    aaData = (List<OrderListEntity>)response.Data,
                    iTotalRecords = totalRecords,
                    iTotalDisplayRecords = totalRecords
                };

            }
            catch (Exception ex)
            {

                response.Message.Add(ex.Message);
            }

            return new TableParameter<OrderListEntity>();


        }

        [HttpPost]
        [Route("ArchiveOrder")]
        public BaseApiResponse ArchiveOrder(OrderWizardStep1 model)
        {
            var response = new BaseApiResponse();
            try
            {
                SqlParameter[] param = {
                                          new SqlParameter("OrderId", (object)model.OrderId ?? (object)DBNull.Value)
                                         ,new SqlParameter("UpdatedBy", (object)model.EmpId ?? (object)DBNull.Value)
                                        ,new SqlParameter("UserAccessId", (object)model.UserAccessId  ?? (object)DBNull.Value)
                };
                var result = _repository.ExecuteSQL<long>("ArchiveOrder", param).FirstOrDefault();
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
        [Route("DeleteDraftOrder")]
        public BaseApiResponse DeleteDraftOrder(int OrderId)
        {
            var response = new BaseApiResponse();
            try
            {

                string serverPath = ConfigurationManager.AppSettings["UploadRoot"].ToString();
                string subFolder = Convert.ToString(OrderId);
                serverPath += subFolder;
                if (Directory.Exists(serverPath))
                {

                    Directory.Delete(serverPath, true);
                }
                SqlParameter[] param = { new SqlParameter("OrderId", (object)OrderId?? (object)DBNull.Value)};
                var result = _repository.ExecuteSQL<int>("DeleteDraftOrder", param).FirstOrDefault();

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



        private static void DeleteDir(string dir)
        {
            try
            {
                Thread.Sleep(1);
                Directory.Delete(dir, true);
            }
            catch (IOException)
            {
                DeleteDir(dir);
            }
            catch (UnauthorizedAccessException)
            {
                DeleteDir(dir);
            }
        }
        #endregion
    }
}