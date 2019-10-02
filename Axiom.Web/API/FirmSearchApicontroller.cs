using Axiom.Common;
using Axiom.Data.Repository;
using Axiom.Entity;
using AXIOM.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Http;

namespace Axiom.Web.API
{
    [RoutePrefix("api")]
    public class FirmSearchApicontroller : ApiController
    {
        #region Initialization
        private readonly GenericRepository<FirmEntity> _repository = new GenericRepository<FirmEntity>();
        #endregion

        [HttpPost]
        [Route("SearchFirmList")]
        public TableParameter<FirmList> SearchFirmList(TableParameter<FirmEntity> tableParameter, int PageIndex, string SearchValue = "", string FirmID = "",
                string FirmName = "",
                string Address = "",
                string City = "",
                string State = "",
                string ParentFirm = "")
        {

            tableParameter.PageIndex = PageIndex;         
            string sortColumn = tableParameter.SortColumn.Desc ? tableParameter.SortColumn.Column + " desc" : tableParameter.SortColumn.Column + " asc";
            string searchValue = SearchValue == null ? string.Empty : SearchValue.Trim();
            var response = new ApiResponse<FirmList>();
            try
            {
                SqlParameter[] param =
                    {

                  new SqlParameter  {
                     ParameterName = "Keyword",
                     DbType = DbType.String,
                     Value = searchValue
                 },new SqlParameter
                 {
                     ParameterName = "PageIndex",
                     DbType = DbType.Int32,
                     Value = tableParameter.PageIndex
                 }, new SqlParameter
                 {
                     ParameterName = "PageSize",
                     DbType = DbType.Int32,
                     Value = (object)tableParameter != null ? tableParameter.iDisplayLength : 10
                 },
                new SqlParameter
                {
                    ParameterName = "SortBy",
                    DbType = DbType.String,
                    Value =sortColumn
                },
                new SqlParameter{ParameterName = "FirmID",DbType = DbType.String,Value = (object)FirmID ?? (object)DBNull.Value  },
                new SqlParameter{ParameterName = "FirmName",DbType = DbType.String,Value = (object)FirmName ?? (object)DBNull.Value  },
                new SqlParameter{ParameterName = "Address",DbType = DbType.String,Value = (object)Address ?? (object)DBNull.Value  },
                new SqlParameter{ParameterName = "City",DbType = DbType.String,Value =  (object)City ?? (object)DBNull.Value },
                new SqlParameter{ParameterName = "State",DbType = DbType.String,Value = (object)State ?? (object)DBNull.Value  },
                new SqlParameter{ParameterName = "AssociatedFirm",DbType = DbType.String,Value = (object)ParentFirm ?? (object)DBNull.Value  }
                };

                var result = _repository.ExecuteSQL<FirmList>("GetFirmList", param).ToList();
                response.Success = true;
                response.Data = result;

                int totalRecords = 0;
                if (response != null && response.Data.Count > 0)
                {
                    totalRecords = response.Data[0].TotalRecords;
                }

                return new TableParameter<FirmList>
                {
                    aaData = (List<FirmList>)response.Data,
                    iTotalRecords = totalRecords,
                    iTotalDisplayRecords = totalRecords
                };

            }
            catch (Exception ex)
            {

                response.Message.Add(ex.Message);
            }
            return new TableParameter<FirmList>();

        }
    }
}