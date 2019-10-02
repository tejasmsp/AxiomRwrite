using Axiom.Common;
using AXIOM.Common;
using Axiom.Entity;
using Axiom.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.SqlClient;
using System.Data;


namespace Axiom.Web.API
{
    [RoutePrefix("api")]
    public class AttorneyApicontroller : ApiController
    {
        #region Initialization
        private readonly GenericRepository<CountyEntity> _repository = new GenericRepository<CountyEntity>();

        //temp        
        #endregion

        [HttpPost]
        [Route("AttorneySearch")]
        public TableParameter<AttorneySearchEntity> AttorneySearch(TableParameter<AttorneySearchEntity> tableParameter, int PageIndex, string SearchValue = "", string FirmID = "",
                string FirmName = "",
                string AttorneyFirstName = "",
                string AttorneyLastName = "")
        {

            tableParameter.PageIndex = PageIndex;
            string sortColumn = tableParameter.SortColumn.Desc ? tableParameter.SortColumn.Column + " desc" : tableParameter.SortColumn.Column + " asc";
            string searchValue = SearchValue == null ? string.Empty : SearchValue.Trim();
            var response = new ApiResponse<AttorneySearchEntity>();
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
                new SqlParameter{ParameterName = "AttorneyFirstName",DbType = DbType.String,Value =  (object)AttorneyFirstName ?? (object)DBNull.Value },
                new SqlParameter{ParameterName = "AttorneyLastName",DbType = DbType.String,Value = (object)AttorneyLastName ?? (object)DBNull.Value }
                };



                var result = _repository.ExecuteSQL<AttorneySearchEntity>("AttorneySearch", param).ToList();
                response.Success = true;
                response.Data = result;

                int totalRecords = 0;
                if (response != null && response.Data.Count > 0)
                {
                    totalRecords = response.Data[0].TotalRecords;
                }

                return new TableParameter<AttorneySearchEntity>
                {
                    aaData = (List<AttorneySearchEntity>)response.Data,
                    iTotalRecords = totalRecords,
                    iTotalDisplayRecords = totalRecords
                };

            }
            catch (Exception ex)
            {

                response.Message.Add(ex.Message);
            }

            return new TableParameter<AttorneySearchEntity>();

        }


        [HttpGet]
        [Route("GetAttorneyByAttyIdForAttorney")]
        public ApiResponse<AttorneyMasterEntity> GetAttorneyByAttyIdForAttorney(string AttyID)
        {
            var response = new ApiResponse<AttorneyMasterEntity>();

            try
            {
                SqlParameter[] param = { new SqlParameter("AttyID", (object)AttyID ?? (object)DBNull.Value) };
                var result = _repository.ExecuteSQL<AttorneyMasterEntity>("GetAttorneyByAttyIdForAttorney", param).ToList();
                if (result == null)
                {
                    result = new List<AttorneyMasterEntity>();
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
        [Route("InsertAttorney")]
        public BaseApiResponse InsertAttorney(AttorneyMasterEntity model, string LoginEmpId, int UserID)
        {
            var response = new BaseApiResponse();
            try
            {
                string XmlAssistantContact = "";
                if (model.AttorneyAssistantContactList != null && model.AttorneyAssistantContactList.Count > 0)
                {

                    XmlAssistantContact = ConvertToXml<AttorneyAssistantContact>.GetXMLString(model.AttorneyAssistantContactList);
                }
                string xmlData = ConvertToXml<AttorneyMasterEntity>.GetXMLString(new List<AttorneyMasterEntity>() { model });

                SqlParameter[] param = {
                                         new SqlParameter("LoginEmpId", LoginEmpId ?? (object)DBNull.Value),
                                         new SqlParameter("UserID", (object)UserID ?? (object)DBNull.Value ),
                                         new SqlParameter("xmlDataString", (object)xmlData ?? (object)DBNull.Value),
                                         new SqlParameter("xmlAssistantContact", (object)XmlAssistantContact ?? (object)DBNull.Value)
                                       };
                var result = _repository.ExecuteSQL<int>("InsertAttorney", param).FirstOrDefault();
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
        [Route("UpdateAttorney")]
        public ApiResponse<AttorneyMasterEntity> UpdateAttorney(AttorneyMasterEntity model, string LoginEmpId, int UserID)
        {
            var response = new ApiResponse<AttorneyMasterEntity>();
            try
            {
                string XmlAssistantContact = "";
                if(model.AttorneyAssistantContactList!=null && model.AttorneyAssistantContactList.Count>0)
                {
                    
                    XmlAssistantContact = ConvertToXml<AttorneyAssistantContact>.GetXMLString(model.AttorneyAssistantContactList);
                }
                string xmlData = ConvertToXml<AttorneyMasterEntity>.GetXMLString(new List<AttorneyMasterEntity>() { model });

                SqlParameter[] param = {  new SqlParameter("LoginEmpId", (object)LoginEmpId ?? (object)DBNull.Value)
                                         ,new SqlParameter("UserID", (object)UserID ?? (object)DBNull.Value)
                                         ,new SqlParameter("LocID", (object)model.AttyID ?? (object)DBNull.Value)
                                         ,new SqlParameter("xmlDataString", (object)xmlData ?? (object)DBNull.Value)
                                         ,new SqlParameter("xmlAssistantContact", (object)XmlAssistantContact ?? (object)DBNull.Value)
                                        };

                var result = _repository.ExecuteSQL<int>("UpdateAttorney", param).FirstOrDefault();
                if (result >  0)
                {
                    // result = new List<AttorneyMasterEntity>();
                    response.Success = true;
                }

                return response;
            }
            catch (Exception ex)
            {
                response.Message.Add(ex.Message);
            }
            return response;
        }

        [HttpGet]
        [Route("GetAttorneyFormsList")]
        public ApiResponse<AttorneyFormEntity> GetAttorneyFormsList(string AttyID, string FormType )
        {
            var response = new ApiResponse<AttorneyFormEntity>();

            try
            {
                SqlParameter[] param = {    new SqlParameter("AttyID", (object)AttyID ?? (object)DBNull.Value),
                                            new SqlParameter("FormType", (object)FormType ?? (object)DBNull.Value) };
                var result = _repository.ExecuteSQL<AttorneyFormEntity>("GetAttorneyFormsList", param).ToList();
                if (result == null)
                {
                    result = new List<AttorneyFormEntity>();
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
        [Route("InsertAttorneyForm")]
        public BaseApiResponse InsertAttorneyForm(AttorneyFormEntity model)
        {
            var response = new BaseApiResponse();
            try
            {

                SqlParameter[] param = {
                                         new SqlParameter("AttyID", (object)model.AttyID ?? (object)DBNull.Value),
                                         new SqlParameter("FormType", (object)model.FormType ?? (object)DBNull.Value),
                                         new SqlParameter("CreatedBy", (object)model.CreatedBy ?? (object)DBNull.Value),
                                         new SqlParameter("FolderPath", (object)model.FolderPath ?? (object)DBNull.Value),
                                         new SqlParameter("FolderName", (object)model.FolderName ?? (object)DBNull.Value),
                                         new SqlParameter("DocFileName", (object)model.DocFileName ?? (object)DBNull.Value)
                                       };
                var result = _repository.ExecuteSQL<int>("InsertAttorneyForm", param).FirstOrDefault();
                if (result > 0)
                {
                    response.Success = true;
                }
                else if (result == -1)
                {
                    response.Success = false;
                    response.Message.Add("The document already exists for this location.");
                }
                else if (result == -2)
                {
                    response.Success = false;
                    response.Message.Add("The document does not exist in the DB(table: documents).");
                }
            }
            catch (Exception ex)
            {
                response.Message.Add(ex.Message);
            }
            return response;

        }


        [HttpPost]
        [Route("DeleteAttorneyForm")]
        public BaseApiResponse DeleteAttorneyForm(int AttyFormID)
        {
            var response = new BaseApiResponse();
            try
            {

                SqlParameter[] param = {
                                         new SqlParameter("AttyFormID", (object)AttyFormID ?? (object)DBNull.Value)

                                       };
                var result = _repository.ExecuteSQL<int>("DeleteAttorneyForm", param).FirstOrDefault();
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

        [HttpGet]
        [Route("GetAttorneyAssistantContactList")]
        public ApiResponse<AttorneyAssistantContact> GetAttorneyAssistantContactList(string AttyID, int UserAccessId)
        {
            var response = new ApiResponse<AttorneyAssistantContact>();

            try
            {
                SqlParameter[] param = {    new SqlParameter("AttyID", (object)AttyID ?? (object)DBNull.Value),
                                            new SqlParameter("UserAccessId", (object)UserAccessId ?? (object)DBNull.Value) };
                var result = _repository.ExecuteSQL<AttorneyAssistantContact>("GetAttorneyAssistantContactList", param).ToList();
                if (result != null && result.Count>0)
                {
                    response.Data = result;
                }
                else
                {
                    result = new List<AttorneyAssistantContact>();
                    result.Add(new AttorneyAssistantContact());
                    response.Data = result;
                }
                response.Success = true;
               
            }
            catch (Exception ex)
            {
                response.Message.Add(ex.Message);
            }

            return response;

        }
    }
}