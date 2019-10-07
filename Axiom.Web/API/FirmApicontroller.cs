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
    public class FirmApicontroller : ApiController
    {
        #region Initialization
        private readonly GenericRepository<FirmEntity> _repository = new GenericRepository<FirmEntity>();
        #endregion

        [HttpPost]
        [Route("GetFirmList")]
        public TableParameter<FirmList> GetFirmList(TableParameter<FirmEntity> tableParameter, int PageIndex, string SearchValue = "", string FirmID = "",
                string FirmName = "",
                string Address = "",
                string City = "",
                string State = "",
                string ParentFirm = "")
        {

            tableParameter.PageIndex = PageIndex;
            string sortColumn = tableParameter.SortColumn.Desc ? tableParameter.SortColumn.Column + " desc" : tableParameter.SortColumn.Column + " asc";
            string searchValue = SearchValue == null ? string.Empty : SearchValue.Trim();


            sortColumn = sortColumn.Replace("Address", "Street1");

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



            //var response = new ApiResponse<FirmEntity>();

            //try
            //{
            //    SqlParameter[] param = { new SqlParameter("FirmId", (object)FirmId ?? (object)DBNull.Value) };
            //    var result = _repository.ExecuteSQL<FirmEntity>("GetFirmList", param).ToList();
            //    if (result == null)
            //    {
            //        result = new List<FirmEntity>();
            //    }

            //    response.Success = true;
            //    response.Data = result;
            //}
            //catch (Exception ex)
            //{
            //    response.Message.Add(ex.Message);
            //}

            //return response;

        }


        [HttpGet]
        [Route("GetFirmById")]
        public ApiResponse<FirmEntity> GetFirmById(string FirmId)
        {
            var response = new ApiResponse<FirmEntity>();

            try
            {
                SqlParameter[] param = { new SqlParameter("FirmId", (object)FirmId ?? (object)DBNull.Value) };
                var result = _repository.ExecuteSQL<FirmEntity>("GetFirmById", param).ToList();
                if (result == null)
                {
                    result = new List<FirmEntity>();
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
        [Route("UpdateAssociatedFirmDefaultBill")]
        public BaseApiResponse UpdateAssociatedFirmDefaultBill(string FirmID, string AssociatedFirmID)
        {
            var response = new BaseApiResponse();
            try
            {
                SqlParameter[] param = {
                                         new SqlParameter("FirmID", (object)FirmID ?? (object)DBNull.Value)
                                        ,new SqlParameter("AssociatedFirmID", (object)AssociatedFirmID ?? (object)DBNull.Value)
                                       };
                var result = _repository.ExecuteSQL<int>("UpdateAssociatedFirmDefaultBill", param).FirstOrDefault();
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
        [Route("InsertFirm")]
        public BaseApiResponse InsertFirm(FirmEntity model, string EmpID, string UserID)
        {
            var response = new BaseApiResponse();
            try
            {
                string xmlData = ConvertToXml<FirmEntity>.GetXMLString(new List<FirmEntity>() { model });

                SqlParameter[] param = {
                                         new SqlParameter("LoginEmpId", (object)EmpID ?? (object)DBNull.Value)
                                        ,new SqlParameter("UserID", (object)UserID ?? (object)DBNull.Value)
                                        ,new SqlParameter("xmlDataString", (object)xmlData ?? (object)DBNull.Value)
                                       };
                var result = _repository.ExecuteSQL<int>("InsertFirm", param).FirstOrDefault();
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
        [Route("UpdateFirm")]
        public ApiResponse<FirmEntity> UpdateFirm(FirmEntity model, string EmpID, string UserID)
        {
            var response = new ApiResponse<FirmEntity>();
            try
            {

                string xmlData = ConvertToXml<FirmEntity>.GetXMLString(new List<FirmEntity>() { model });

                SqlParameter[] param = {
                                new SqlParameter("LoginEmpId", (object)EmpID ?? (object)DBNull.Value)
                                ,new SqlParameter("UserID", (object)UserID ?? (object)DBNull.Value)
                                ,new SqlParameter("FirmID", (object)model.FirmID ?? (object)DBNull.Value)
                                ,new SqlParameter("xmlDataString", (object)xmlData ?? (object)DBNull.Value)};

                var result = _repository.ExecuteSQL<FirmEntity>("UpdateFirm", param).ToList();
                if (result == null)
                {
                    result = new List<FirmEntity>();
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
        [Route("DeleteFirm")]
        public BaseApiResponse DeleteFirm(string FirmId)
        {
            var response = new BaseApiResponse();
            try
            {
                SqlParameter[] param = { new SqlParameter("FirmID", (object)FirmId ?? (object)DBNull.Value) };
                var result = _repository.ExecuteSQL<int>("DeleteFirm", param).FirstOrDefault();
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
        [Route("SaveFirmMonthlyBilling")]
        public ApiResponse<FirmEntity> SaveFirmMonthlyBilling(string FirmId, string Name, string Email)
        {
            var response = new ApiResponse<FirmEntity>();

            try
            {
                SqlParameter[] param = { new SqlParameter("FirmId", (object)FirmId ?? (object)DBNull.Value)
                                            ,new SqlParameter("Name", (object)Name ?? (object)DBNull.Value)
                                            ,new SqlParameter("Email", (object)Email ?? (object)DBNull.Value)
                };
                var result = _repository.ExecuteSQL<FirmEntity>("SaveFirmMonthlyBilling", param).ToList();
                if (result == null)
                {
                    result = new List<FirmEntity>();
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

        #region --- ASSOCIATED FIRM --- 

        [HttpPost]
        [Route("InsertAssociatedFirm")]
        public BaseApiResponse InsertAssociatedFirm(string ParentFirmID, string AssocicatedFirmID)
        {
            var response = new BaseApiResponse();
            try
            {
                SqlParameter[] param = {
                                         new SqlParameter("FirmID", ParentFirmID  ?? (object)DBNull.Value),
                                         new SqlParameter("AssociatedFirmID", (object)AssocicatedFirmID ?? (object)DBNull.Value)
                                       };
                var result = _repository.ExecuteSQL<int>("InsertAssociatedFirm", param).FirstOrDefault();
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
        [Route("GetAssociatedFirmList")]
        public ApiResponse<AssociatedFirmEntity> GetAssociatedFirmList(string FirmId)
        {
            var response = new ApiResponse<AssociatedFirmEntity>();

            try
            {
                SqlParameter[] param = { new SqlParameter("FirmId", (object)FirmId ?? (object)DBNull.Value) };
                var result = _repository.ExecuteSQL<AssociatedFirmEntity>("GetAssociatedFirmList", param).ToList();
                if (result == null)
                {
                    result = new List<AssociatedFirmEntity>();
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
        [Route("DeleteAssociatedFirm")]
        public BaseApiResponse DeleteAssociatedFirm(string ParentFirmID, string AssocicatedFirmID)
        {
            var response = new BaseApiResponse();
            try
            {
                SqlParameter[] param = { new SqlParameter("FirmID", (object)ParentFirmID ?? (object)DBNull.Value)
                                        ,new SqlParameter("AssociatedFirmID", (object)AssocicatedFirmID ?? (object)DBNull.Value)
                };
                var result = _repository.ExecuteSQL<int>("DeleteAssociatedFirm", param).FirstOrDefault();
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

        #region --- MEMBER OF ID --- 
        [HttpGet]
        [Route("GetMemberOfIDList")]
        public ApiResponse<MemberOfIDEntity> GetMemberOfIDList(string FirmId)
        {
            var response = new ApiResponse<MemberOfIDEntity>();

            try
            {
                SqlParameter[] param = { new SqlParameter("FirmId", (object)FirmId ?? (object)DBNull.Value) };
                var result = _repository.ExecuteSQL<MemberOfIDEntity>("GetMemberOfIDList", param).ToList();
                if (result == null)
                {
                    result = new List<MemberOfIDEntity>();
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
        [Route("InsertMemberOfID")]
        public BaseApiResponse InsertMemberOfID(string FirmID, string CompanyID, string MemberID)
        {
            var response = new BaseApiResponse();
            try
            {
                SqlParameter[] param = {
                                         new SqlParameter("FirmID", (object)FirmID  ?? (object)DBNull.Value),
                                         new SqlParameter("CompanyID", (object)CompanyID ?? (object)DBNull.Value),
                                         new SqlParameter("MemberID", (object)MemberID ?? (object)DBNull.Value)
                                       };
                var result = _repository.ExecuteSQL<string>("InsertMemberOfID", param).FirstOrDefault();
                if (result != string.Empty)
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
        [Route("DeleteMemberOfID")]
        public BaseApiResponse DeleteMemberOfID(string FirmBillingRateID)
        {
            var response = new BaseApiResponse();
            try
            {
                SqlParameter[] param = { new SqlParameter("ID", (object)FirmBillingRateID ?? (object)DBNull.Value) };
                var result = _repository.ExecuteSQL<int>("DeleteMemberOfID", param).FirstOrDefault();
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
        [Route("GetAdditionalContacts")]
        public ApiResponse<AdditionalContactEntity> GetAdditionalContacts(string FirmId, string Type)
        {
            var response = new ApiResponse<AdditionalContactEntity>();

            try
            {
                SqlParameter[] param = { new SqlParameter("FirmId", (object)FirmId ?? (object)DBNull.Value)
                                        ,new SqlParameter("Type", (object)Type ?? (object)DBNull.Value)
                                        };
                var result = _repository.ExecuteSQL<AdditionalContactEntity>("GetAdditionalContactsByFirmID", param).ToList();
                if (result == null)
                {
                    result = new List<AdditionalContactEntity>();
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
        [Route("SaveAdditionalContacts")]
        public ApiResponse<AdditionalContactEntity> SaveAdditionalContacts(AdditionalContactEntity modal)
        {
            var response = new ApiResponse<AdditionalContactEntity>();

            try
            {
                SqlParameter[] param = { new SqlParameter("FirmID", (object)modal.FirmID ?? (object)DBNull.Value)
                        ,new SqlParameter("FirstName", (object)modal.FirstName ?? (object)DBNull.Value)
                        ,new SqlParameter("LastName", (object)modal.LastName ?? (object)DBNull.Value)
                        ,new SqlParameter("Email", (object)modal.Email ?? (object)DBNull.Value)
                        ,new SqlParameter("ContactFor", (object)modal.ContactFor ?? (object)DBNull.Value)

                };
                var result = _repository.ExecuteSQL<int>("SaveAdditionalContacts", param).FirstOrDefault();
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
        [Route("DeleteAdditionalContact")]
        public BaseApiResponse DeleteAdditionalContact(string ContactID)
        {
            var response = new BaseApiResponse();
            try
            {
                SqlParameter[] param = { new SqlParameter("ContactID", (object)ContactID ?? (object)DBNull.Value) };
                var result = _repository.ExecuteSQL<int>("DeleteAdditionalContacts", param).FirstOrDefault();
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
        [Route("GetFirmFormsList")]
        public ApiResponse<FirmForm> GetFirmFormsList(string FirmID, bool isRequestform, bool isFaceSheet)
        {
            var response = new ApiResponse<FirmForm>();

            try
            {
                SqlParameter[] param = { new SqlParameter("FirmID", (object)FirmID ?? (object)DBNull.Value)
                                        ,new SqlParameter("isFaceSheet", (object)isFaceSheet ?? (object)DBNull.Value)
                                        ,new SqlParameter("isRequestform", (object)isRequestform ?? (object)DBNull.Value) };
                var result = _repository.ExecuteSQL<FirmForm>("GetFirmFormsList", param).ToList();
                if (result == null)
                {
                    result = new List<FirmForm>();
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
        [Route("InsertFirmForm")]
        public BaseApiResponse InsertFirmForm(FirmForm model)
        {
            var response = new BaseApiResponse();
            try
            {

                SqlParameter[] param = {
                                         new SqlParameter("FirmID", (object)model.FirmID ?? (object)DBNull.Value),
                                         new SqlParameter("IsRequestForm", (object)model.IsRequestForm ?? (object)DBNull.Value),
                                         new SqlParameter("IsFaceSheet", (object)model.IsFaceSheet ?? (object)DBNull.Value),
                                         new SqlParameter("CreatedBy", (object)model.CreatedBy ?? (object)DBNull.Value),
                                         new SqlParameter("FolderPath", (object)model.FolderPath ?? (object)DBNull.Value),
                                         new SqlParameter("FolderName", (object)model.FolderName ?? (object)DBNull.Value),
                                         new SqlParameter("DocFileName", (object)model.DocFileName ?? (object)DBNull.Value)
                                       };
                var result = _repository.ExecuteSQL<int>("InsertFirmForm", param).FirstOrDefault();
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
        [Route("DeleteFirmForm")]
        public BaseApiResponse DeleteFirmForm(int FirmFormID)
        {
            var response = new BaseApiResponse();
            try
            {

                SqlParameter[] param = {
                                         new SqlParameter("FirmformID", (object)FirmFormID ?? (object)DBNull.Value)

                                       };
                var result = _repository.ExecuteSQL<int>("DeleteFirmForm", param).FirstOrDefault();
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