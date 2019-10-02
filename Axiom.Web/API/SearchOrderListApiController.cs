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

namespace Axiom.Web.API
{
    [RoutePrefix("api")]
    public class SearchOrderListApiController : ApiController
    {
        #region Initialization
        private readonly GenericRepository<OrderListEntity> _repository = new GenericRepository<OrderListEntity>();
        #endregion

        #region DatabaseOperations

        [HttpPost]
        [Route("GetOrderListForAdmin")]
        public TableParameter<SearchListEntity> GetOrderListForAdmin(TableParameter<SearchListEntity> tableParameter
                , int PageIndex
                , string SearchValue = ""
                , string OrderNo = ""
                , string PartNo = ""
                , string PatientName = ""
                , string LocationName = ""
                , string LocationState = ""
                , string PlaintiffAtty1 = ""
                , string PlaintiffAtty2 = ""
                , string PlaintiffAtty3 = ""
                , string PlaintiffAtty4 = ""
                , string PlaintiffAtty5 = ""
                , string OrderAtty = ""
                , string OrderFirm = ""
                , string RecordTypeId = ""
                , string AccountRep = ""
                , string Processor = ""
                , string AssignedTo = ""
                , string InternalStatus = ""
                , string Rush = ""
                , bool CallbackOnly = false
                , DateTime? DueDateFrom = null
                , DateTime? DueDateTo = null
                , DateTime? ActionDateFrom = null
                , DateTime? ActionDateTo = null
                , DateTime? CallbackDateFrom = null
                , DateTime? CallbackDateTo = null
                , string CompNo = null
                , string AreaCode = null
                , string FaxNo = null
                , bool ShowPartDetailWithOrder = true 
            )
        {

            tableParameter.PageIndex = PageIndex;
            string sortColumn = tableParameter.SortColumn.Desc ? tableParameter.SortColumn.Column + " desc" : tableParameter.SortColumn.Column + " asc";
            string searchValue = SearchValue == null ? string.Empty : SearchValue.Trim();
            var response = new ApiResponse<SearchListEntity>();
            try
            {
                SqlParameter[] param =
                    {

                 new SqlParameter{ ParameterName = "PageIndex",  Value = tableParameter.PageIndex }
                ,new SqlParameter { ParameterName = "PageSize",  Value = (object)tableParameter != null ? tableParameter.iDisplayLength : 10 }
                ,new SqlParameter { ParameterName = "SortBy",    Value =sortColumn }
                ,new SqlParameter  { ParameterName = "Keyword", DbType = DbType.String, Value = searchValue },
                new SqlParameter{ParameterName ="OrderNo"    ,Value = (object)OrderNo ?? (object)DBNull.Value},
                new SqlParameter{ParameterName ="PartNo" ,Value = (object)PartNo  ?? (object)DBNull.Value  },
                new SqlParameter{ParameterName ="PatientName"    ,Value = (object)PatientName     ?? (object)DBNull.Value  },
                new SqlParameter{ParameterName ="LocationName"   ,Value = (object)LocationName    ?? (object)DBNull.Value  },
                new SqlParameter{ParameterName ="LocationState"  ,Value = (object)LocationState   ?? (object)DBNull.Value  },
                new SqlParameter{ParameterName ="PlaintiffAtty1" ,Value = (object)PlaintiffAtty1  ?? ""  },
                new SqlParameter{ParameterName ="PlaintiffAtty2" ,Value = (object)PlaintiffAtty2  ?? ""  },
                new SqlParameter{ParameterName ="PlaintiffAtty3" ,Value = (object)PlaintiffAtty3  ?? ""  },
                new SqlParameter{ParameterName ="PlaintiffAtty4" ,Value = (object)PlaintiffAtty4  ?? ""  },
                new SqlParameter{ParameterName ="PlaintiffAtty5" ,Value = (object)PlaintiffAtty5  ?? ""  },
                new SqlParameter{ParameterName ="OrderAtty"  ,Value = (object)OrderAtty   ?? ""  },
                new SqlParameter{ParameterName ="OrderFirm"  ,Value = (object)OrderFirm   ?? ""  },
                new SqlParameter{ParameterName ="RecordTypeId"   ,Value = (object)RecordTypeId    ?? ""  },
                new SqlParameter{ParameterName ="AccountRep" ,Value = (object)AccountRep  ?? ""  },
                new SqlParameter{ParameterName ="Processor"  ,Value = (object)Processor   ?? ""  },
                new SqlParameter{ParameterName ="AssignedTo" ,Value = (object)AssignedTo  ?? ""  },
                new SqlParameter{ParameterName ="InternalStatus" ,Value = (object)InternalStatus  ?? ""  },
                new SqlParameter{ParameterName ="Rush"   ,Value = (object)Rush    ?? ""  },
                new SqlParameter{ParameterName ="CallbackOnly"   ,Value = (object)CallbackOnly    ?? (object)DBNull.Value  },
                new SqlParameter{ParameterName ="DueDateFrom"    ,Value = (object)DueDateFrom ?? (object)DBNull.Value  },
                new SqlParameter{ParameterName ="DueDateTo"  ,Value = (object)DueDateTo   ?? (object)DBNull.Value  },
                new SqlParameter{ParameterName ="ActionDateFrom" ,Value = (object)ActionDateFrom  ?? (object)DBNull.Value  },
                new SqlParameter{ParameterName ="ActionDateTo"   ,Value = (object)ActionDateTo    ?? (object)DBNull.Value  },
                new SqlParameter{ParameterName ="CallbackDateFrom"   ,Value = (object)CallbackDateFrom    ?? (object)DBNull.Value  },
                new SqlParameter{ParameterName ="CallbackDateTo" ,Value = (object)CallbackDateTo  ?? (object)DBNull.Value  },
                new SqlParameter{ParameterName ="CompNo" ,Value = (object)CompNo  ?? (object)DBNull.Value  },
                new SqlParameter{ParameterName ="AreaCode"   ,Value = (object)AreaCode    ?? (object)DBNull.Value  },
                new SqlParameter{ParameterName ="FaxNo"  ,Value = (object)FaxNo   ?? (object)DBNull.Value  },
                new SqlParameter{ParameterName ="ShowPartDetailWithOrder"  ,Value = (object)ShowPartDetailWithOrder   ?? (object)DBNull.Value  },

                };

                var result = _repository.ExecuteSQL<SearchListEntity>("GetOrderListForAdmin", param).ToList();
                response.Success = true;
                response.Data = result;

                int totalRecords = 0;
                if (response != null && response.Data.Count > 0)
                {
                    totalRecords = response.Data[0].TotalRecords;
                }
                return new TableParameter<SearchListEntity>
                {
                    aaData = (List<SearchListEntity>)response.Data,
                    iTotalRecords = totalRecords,
                    iTotalDisplayRecords = totalRecords
                };

            }
            catch (Exception ex)
            {
                response.Message.Add(ex.Message);
            }
            return new TableParameter<SearchListEntity>();
        }

        #endregion
    }
}