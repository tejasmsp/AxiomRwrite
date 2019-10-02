using Axiom.Common;
using Axiom.Data.Repository;
using Axiom.Entity;
using AXIOM.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Axiom.Web.API
{
    [RoutePrefix("api")]
    public class AccountReceivableApiController : ApiController
    {

        #region Initialization
        private readonly GenericRepository<OrderWizardStep2> _repository = new GenericRepository<OrderWizardStep2>();
        #endregion

        #region Step 1 Database Methods

        [HttpGet]
        [Route("GetAccountReceivableListBySearch")]
        public ApiResponse<AccountReceivable> GetAccountReceivableListBySearch(string FirmID,string FirmName, string CheckType, string CheckNo, int? InvoiceNo, int UserAccessId)
        {
            var response = new ApiResponse<AccountReceivable>();
            var result = new List<AccountReceivable>();
            try
            {
                SqlParameter[] param = {
                                        //new SqlParameter("FirmID", (object)FirmID ?? (object)DBNull.Value)
                                        new SqlParameter("FirmName", (object)FirmName ?? (object)DBNull.Value)
                                        ,new SqlParameter("CheckType", (object)CheckType ?? (object)DBNull.Value)
                                        ,new SqlParameter("CheckNo", (object)CheckNo ?? (object)DBNull.Value)
                                        ,new SqlParameter("InvoiceNo", (object)InvoiceNo ?? (object)DBNull.Value)
                                        ,new SqlParameter("UserAccessId", (object)UserAccessId ?? (object)DBNull.Value)

                                        };
                result = _repository.ExecuteSQL<AccountReceivable>("GetAccountReceivableListBySearch", param).ToList();

                if (result != null && result.Count > 0)
                {
                    response.Data = result;
                }
                else
                {
                    result = new List<AccountReceivable>();
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
        [HttpGet]
        [Route("GetAccountReceivable")]
        public ApiResponse<AccountReceivable> GetAccountReceivable(int ArID=0)
        {
            var response = new ApiResponse<AccountReceivable>();
            var result = new List<AccountReceivable>();
            try
            {
                SqlParameter[] param = { new SqlParameter("ArID", (object)ArID ?? (object)DBNull.Value) };
                result = _repository.ExecuteSQL<AccountReceivable>("GetAccountReceivable", param).ToList();

                if (result != null && result.Count > 0)
                {
                    response.Data = result;
                }else
                {
                    result = new List<AccountReceivable>();
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
        [Route("InsertOrUpdateAccountReceivable")]
        public BaseApiResponse InsertOrUpdateAccountReceivable(AccountReceivable model)
        {
            var response = new BaseApiResponse();

            try
            {
                SqlParameter[] param = {
                                         new SqlParameter("ArID", (object)model.ArID ?? (object)DBNull.Value)
                                        ,new SqlParameter("CheckType", (object)model.CheckType ?? (object)DBNull.Value)
                                        ,new SqlParameter("FirmName", (object)model.FirmName ?? (object)DBNull.Value)
                                        ,new SqlParameter("CheckNumber", (object)model.CheckNumber ?? (object)DBNull.Value)
                                        ,new SqlParameter("CheckAmount", (object)model.CheckAmount ?? (object)DBNull.Value)
                                        ,new SqlParameter("Note", (object)model.Note ?? (object)DBNull.Value)
                                        ,new SqlParameter("CreatedBy", (object)model.UserAccessId ?? (object)DBNull.Value)                                   
                };
                var result = _repository.ExecuteSQL<int>("InsertOrUpdateAccountReceivable", param).FirstOrDefault();

                if (result > 0)
                {
                    response.Success = true;
                    response.ArID = result;
                }
                else
                {
                    response.Success = false;
                    response.Message.Add("Check Amount Should be less than or equal to total Invoice.");
                }
            }
            catch (Exception ex)
            {
                response.Message.Add(ex.Message);
            }

            return response;
        }

        [HttpPost]
        [Route("InsertAccountReceivableInvoice")]
        public BaseApiResponse InsertAccountReceivableInvoice(AccountReceivableInvoice model)
        {
            var response = new BaseApiResponse();

            try
            {
                
                SqlParameter[] param = {
                                        new SqlParameter("CheckId", (object)model.ArID ?? (object)DBNull.Value)
                                        ,new SqlParameter("InvoiceId", (object)model.InvNo ?? (object)DBNull.Value)
                                        ,new SqlParameter("Note", (object)model.Note ?? (object)DBNull.Value)
                                        ,new SqlParameter("Payment", (object)model.InvoicePayableAmount ?? (object)DBNull.Value)
                                        ,new SqlParameter("FirmId", (object)"" ?? (object)DBNull.Value)
                                        ,new SqlParameter("CreatedBy", (object)model.CreatedBy ?? (object)DBNull.Value)

                                        };
                var result = 0;
                  result = _repository.ExecuteSQL<int>("InsertCheckInvoicePaymentForNormalCheck", param).FirstOrDefault();

                if (result > 0)
                {
                    response.Success = true;
                  
                }
                else if (result == -1)
                {
                    response.Success = false;
                    response.Message.Add("Total Of Invoice Amount Should be less than or equal to Check Amount.");
                }
                else
                {
                    response.Success = false;
                    response.Message.Add("Invalid Invoice No");
                }
            }
            catch (Exception ex)
            {
                response.Message.Add(ex.Message);
            }

            return response;
        }
        [HttpPost]
        [Route("UpdateAccountReceivableInvoice")]
        public BaseApiResponse UpdateAccountReceivableInvoice(AccountReceivableInvoice model)
        {
            var response = new BaseApiResponse();

            try
            {
                SqlParameter[] param = {new SqlParameter("CheckInvoiceId", (object)model.CheckInvoiceId ?? (object)DBNull.Value)                               
                                        ,new SqlParameter("Note", (object)model.Note ?? (object)DBNull.Value)
                                        ,new SqlParameter("NewPaymentAmount", (object)model.InvoicePayableAmount ?? (object)DBNull.Value)                                          
                                         ,new SqlParameter("CreatedBy", (object)model.CreatedBy ?? (object)DBNull.Value)

                                        };
                var result = _repository.ExecuteSQL<int>("EditInvoicePayment", param).FirstOrDefault();

                if (result > 0)
                {
                    response.Success = true;

                }
                else if(result==-1)
                {
                    response.Success = false;
                    response.Message.Add("Total Of Invoice Amount Should be less than or equal to Check Amount.");
                }
                else
                {
                    response.Success = false;
                    response.Message.Add("Invalid Invoice No");
                }
            }
            catch (Exception ex)
            {
                response.Message.Add(ex.Message);
            }

            return response;
        }

        [HttpPost]
        [Route("InsertAccountReceivableInvoiceForCreditCheck")]
        public BaseApiResponse InsertAccountReceivableInvoiceForCreditCheck(AccountReceivableInvoice model)
        {
            var response = new BaseApiResponse();

            try
            {

                SqlParameter[] param = {
                                        new SqlParameter("CheckId", (object)model.ArID ?? (object)DBNull.Value)
                                        ,new SqlParameter("InvoiceId", (object)model.InvNo ?? (object)DBNull.Value)
                                        ,new SqlParameter("Note", (object)model.Note ?? (object)DBNull.Value)
                                        ,new SqlParameter("Payment", (object)model.InvoicePayableAmount ?? (object)DBNull.Value)
                                        ,new SqlParameter("FirmId", (object)"" ?? (object)DBNull.Value)
                                        ,new SqlParameter("CreatedBy", (object)model.CreatedBy ?? (object)DBNull.Value)

                                        };
                var result = 0;
                result = _repository.ExecuteSQL<int>("InsertCheckInvoicePaymentForCreditCheck", param).FirstOrDefault();

                if (result > 0)
                {
                    response.Success = true;

                }
                else if (result == -1)
                {
                    response.Success = false;
                    response.Message.Add("Total Of Invoice Amount Should be less than or equal to Check Amount.");
                }
                else
                {
                    response.Success = false;
                    response.Message.Add("Invalid Invoice No");
                }
            }
            catch (Exception ex)
            {
                response.Message.Add(ex.Message);
            }

            return response;
        }
        [HttpPost]
        [Route("UpdateAccountReceivableInvoiceForCreditCheck")]
        public BaseApiResponse UpdateAccountReceivableInvoiceForCreditCheck(AccountReceivableInvoice model)
        {
            var response = new BaseApiResponse();

            try
            {
                SqlParameter[] param = {new SqlParameter("CheckInvoiceId", (object)model.CheckInvoiceId ?? (object)DBNull.Value)
                                        ,new SqlParameter("CheckId", (object)model.ArID ?? (object)DBNull.Value)
                                        ,new SqlParameter("InvoiceId", (object)model.InvNo ?? (object)DBNull.Value)
                                        ,new SqlParameter("Note", (object)model.Note ?? (object)DBNull.Value)
                                        ,new SqlParameter("Payment", (object)model.InvoicePayableAmount ?? (object)DBNull.Value)
                                         ,new SqlParameter("CreatedBy", (object)model.CreatedBy ?? (object)DBNull.Value)

                                        };
                var result = _repository.ExecuteSQL<int>("UpdateAccountReceivableInvoiceForCreditCheck", param).FirstOrDefault();

                if (result > 0)
                {
                    response.Success = true;

                }
                else if (result == -1)
                {
                    response.Success = false;
                    response.Message.Add("Total Of Invoice Amount Should be less than or equal to Check Amount.");
                }
                else
                {
                    response.Success = false;
                    response.Message.Add("Invalid Invoice No");
                }
            }
            catch (Exception ex)
            {
                response.Message.Add(ex.Message);
            }

            return response;
        }

        [HttpPost]
        [Route("InsertCheckInvoicePaymentForDebitCheck")]
        public BaseApiResponse InsertCheckInvoicePaymentForDebitCheck(AccountReceivableInvoice model)
        {
            var response = new BaseApiResponse();

            try
            {

                SqlParameter[] param = {
                                        new SqlParameter("CheckId", (object)model.ArID ?? (object)DBNull.Value)
                                        ,new SqlParameter("InvoiceId", (object)model.InvNo ?? (object)DBNull.Value)
                                        ,new SqlParameter("Note", (object)model.Note ?? (object)DBNull.Value)
                                        ,new SqlParameter("Payment", (object)model.InvoicePayableAmount ?? (object)DBNull.Value)
                                        ,new SqlParameter("FirmId", (object)"" ?? (object)DBNull.Value)
                                        ,new SqlParameter("CreatedBy", (object)model.CreatedBy ?? (object)DBNull.Value)

                                        };
                var result = 0;
                result = _repository.ExecuteSQL<int>("InsertCheckInvoicePaymentForDebitCheck", param).FirstOrDefault();

                if (result > 0)
                {
                    response.Success = true;

                }
                else if (result == -1)
                {
                    response.Success = false;
                    response.Message.Add("Total Of Invoice Amount Should be less than or equal to Check Amount.");
                }
                else
                {
                    response.Success = false;
                    response.Message.Add("Invalid Invoice No");
                }
            }
            catch (Exception ex)
            {
                response.Message.Add(ex.Message);
            }

            return response;
        }
        [HttpPost]
        [Route("UpdateCheckInvoicePaymentForDebitCheck")]
        public BaseApiResponse UpdateCheckInvoicePaymentForDebitCheck(AccountReceivableInvoice model)
        {
            var response = new BaseApiResponse();

            try
            {
                SqlParameter[] param = {new SqlParameter("CheckInvoiceId", (object)model.CheckInvoiceId ?? (object)DBNull.Value)
                                        ,new SqlParameter("CheckId", (object)model.ArID ?? (object)DBNull.Value)
                                        ,new SqlParameter("InvoiceId", (object)model.InvNo ?? (object)DBNull.Value)
                                        ,new SqlParameter("Note", (object)model.Note ?? (object)DBNull.Value)
                                        ,new SqlParameter("Payment", (object)model.InvoicePayableAmount ?? (object)DBNull.Value)
                                         ,new SqlParameter("CreatedBy", (object)model.CreatedBy ?? (object)DBNull.Value)

                                        };
                var result = _repository.ExecuteSQL<int>("UpdateCheckInvoicePaymentForDebitCheck", param).FirstOrDefault();

                if (result > 0)
                {
                    response.Success = true;

                }
                else if (result == -1)
                {
                    response.Success = false;
                    response.Message.Add("Total Of Invoice Amount Should be less than or equal to Check Amount.");
                }
                else
                {
                    response.Success = false;
                    response.Message.Add("Invalid Invoice No");
                }
            }
            catch (Exception ex)
            {
                response.Message.Add(ex.Message);
            }

            return response;
        }

        [HttpPost]
        [Route("DeleteInvoicePayment")]
        public BaseApiResponse DeleteInvoicePayment(int CheckInvoiceId,int CreatedBy)
        {
            var response = new BaseApiResponse();

            try
            {
                SqlParameter[] param = {new SqlParameter("CheckInvoiceId", (object)CheckInvoiceId ?? (object)DBNull.Value)
                                        ,new SqlParameter("CreatedBy", (object)CreatedBy ?? (object)DBNull.Value)                                        
                                        };
                var result = _repository.ExecuteSQL<int>("DeleteInvoicePayment", param).FirstOrDefault();

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
        [Route("GetAccountReceivableInvoice")]
        public ApiResponse<AccountReceivableInvoice> GetAccountReceivableInvoice(int ArID = 0,int UserId=0)
        {
            var response = new ApiResponse<AccountReceivableInvoice>();
            var result = new List<AccountReceivableInvoice>();
            try
            {
                SqlParameter[] param = { new SqlParameter("ArID", (object)ArID ?? (object)DBNull.Value),
                                        new SqlParameter("UserId", (object)UserId ?? (object)DBNull.Value)};
                result = _repository.ExecuteSQL<AccountReceivableInvoice>("GetAccountReceivableInvoice", param).ToList();

                if (result != null && result.Count > 0)
                {
                    response.Data = result;
                }
                else
                {
                    result = new List<AccountReceivableInvoice>();
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
        [HttpGet]
        [Route("GetAccountReceivableInvoicesById")]
        public ApiResponse<AccountReceivableInvoice> GetAccountReceivableInvoicesById(int InvoiceId = 0)
        {
            var response = new ApiResponse<AccountReceivableInvoice>();
            var result = new List<AccountReceivableInvoice>();
            try
            {
                SqlParameter[] param = { new SqlParameter("InvoiceId", (object)InvoiceId ?? (object)DBNull.Value) };
                result = _repository.ExecuteSQL<AccountReceivableInvoice>("GetAccountReceivableInvoicesById", param).ToList();

                if (result != null && result.Count > 0)
                {
                    response.Data = result;
                }
                else
                {
                    result = new List<AccountReceivableInvoice>();
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

        [Route("GetInvoicesList")]
        public ApiResponse<InvoiceListEntity> GetInvoicesList(int UserId)
        {
            var response = new ApiResponse<InvoiceListEntity>();
            var result = new List<InvoiceListEntity>();
            try
            {
                SqlParameter[] param = {new SqlParameter("UserId", (object)UserId ?? (object)DBNull.Value)                                       

                                        };
                result = _repository.ExecuteSQL<InvoiceListEntity>("GetInvoiceListForCheck", param).ToList();

                if (result != null && result.Count > 0)
                {
                    response.Data = result;
                }
                else
                {
                    result = new List<InvoiceListEntity>();
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
        [Route("GetVoidAndAllInVoices")]
        public TableParameter<InvoiceListEntity> GetVoidAndAllInVoices(TableParameter<InvoiceListEntity> tableParameter, int PageIndex, int? InvoiceNo, int? OrderNo,int UserId, string BilledAttorney, string SoldAttorney, bool? VoidInvoices,
                                    bool? AllInvoices, string FirmID, string FirmName)
        {

            tableParameter.PageIndex = PageIndex;
            string sortColumn = tableParameter.SortColumn.Desc ? tableParameter.SortColumn.Column + " desc" : tableParameter.SortColumn.Column + " asc";

            var response = new ApiResponse<InvoiceListEntity>();
            try
            {
                SqlParameter[] param =
                    {
                       new SqlParameter{ParameterName = "FirmID ", DbType = DbType.String,Value = (object)FirmID  ?? (object)DBNull.Value  },
                       new SqlParameter{ParameterName = "FirmName", DbType = DbType.String,Value = (object)FirmName ?? (object)DBNull.Value  },
                       new SqlParameter{ParameterName = "VoidInvoices", DbType = DbType.Boolean,Value = (object)VoidInvoices ?? (object)DBNull.Value  },
                       new SqlParameter{ParameterName = "AllInvoices", DbType = DbType.Boolean,Value = (object)AllInvoices ?? (object)DBNull.Value  },
                       new SqlParameter{ParameterName = "InvoiceNo", DbType = DbType.Int32,Value = (object)InvoiceNo ?? (object)DBNull.Value  },
                       new SqlParameter{ParameterName = "UserId", DbType = DbType.Int32,Value = (object)UserId ?? (object)DBNull.Value  },
                      new SqlParameter{ParameterName = "OrderNo", DbType = DbType.Int32,Value = (object)OrderNo ?? (object)DBNull.Value  },
                      new SqlParameter{ ParameterName = "PageSize", DbType = DbType.Int32, Value = (object)tableParameter != null ? tableParameter.iDisplayLength : 10},
                     new SqlParameter {ParameterName = "SortBy",DbType = DbType.String,Value =sortColumn},
                     new SqlParameter{ParameterName = "PageIndex", DbType = DbType.Int32,Value = (object)PageIndex ?? (object)DBNull.Value },
                   new SqlParameter{ParameterName = "BilledAttorney", DbType = DbType.String,Value = (object)BilledAttorney ?? (object)DBNull.Value  },
                   new SqlParameter{ParameterName = "SoldAttorney", DbType = DbType.String,Value = (object)SoldAttorney ?? (object)DBNull.Value  }
              
                };

                var result = _repository.ExecuteSQL<InvoiceListEntity>("GetVoidAndAllInvoices", param).ToList();
                response.Success = true;
                response.Data = result;

                int totalRecords = 0;
                if (response != null && response.Data.Count > 0)
                {
                    totalRecords = response.Data[0].TotalRecords;
                }

                return new TableParameter<InvoiceListEntity>
                {
                    aaData = (List<InvoiceListEntity>)response.Data,
                    iTotalRecords = totalRecords,
                    iTotalDisplayRecords = totalRecords
                };

            }
            catch (Exception ex)
            {

                response.Message.Add(ex.Message);
            }

            return new TableParameter<InvoiceListEntity>();

        }




        [HttpGet]
        [Route("GetVoidInvoicesBySearch")]
        public ApiResponse<VoidInvoiceEntity> GetVoidInvoicesBySearch(string FirmID,string FirmName,bool? VoidInvoices, bool? AllInvoices, int? InvoiceNo,int UserId)
        {
            var response = new ApiResponse<VoidInvoiceEntity>();
            var result = new List<VoidInvoiceEntity>();
            try
            {
                SqlParameter[] param = { new SqlParameter("FirmID", (object)FirmID ?? (object)DBNull.Value)
                                       , new SqlParameter("FirmName", (object)FirmName ?? (object)DBNull.Value)
                                        ,new SqlParameter("VoidInvoices", (object)VoidInvoices ?? (object)DBNull.Value)
                                        ,new SqlParameter("AllInvoices", (object)AllInvoices ?? (object)DBNull.Value)                                       
                                        ,new SqlParameter("InvoiceNo", (object)InvoiceNo ?? (object)DBNull.Value)
                                        ,new SqlParameter("UserId", (object)UserId ?? (object)DBNull.Value)};
                result = _repository.ExecuteSQL<VoidInvoiceEntity>("GetVoidInvoicesBySearch",param).ToList();

                if (result != null && result.Count > 0)
                {
                    response.Data = result;
                }
                else
                {
                    result = new List<VoidInvoiceEntity>();
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
        [Route("SetInvoiceStatus")]
        public BaseApiResponse SetInvoiceStatus(int InvNo,int Status)
        {
            var response = new BaseApiResponse();

            try
            {
                SqlParameter[] param = {new SqlParameter("InvNo", (object)InvNo?? (object)DBNull.Value)
                                        ,new SqlParameter("Status", (object)Status?? (object)DBNull.Value)                                        
                                        };
                var result = _repository.ExecuteSQL<int>("SetInvoiceStatus", param).FirstOrDefault();

                if (result > 0)
                {
                    response.Success = true;

                }
                else
                {
                    response.Success = false;
                    response.Message.Add("Total Of Invoice Amount Should be less than Check Amount.");
                }
            }
            catch (Exception ex)
            {
                response.Message.Add(ex.Message);
            }

            return response;
        }

        [Route("BounceAndCancelCheck")]
        public BaseApiResponse BounceAndCancelCheck(int ArID, int CreatedBy,string CheckType)
        {
            var response = new BaseApiResponse();

            try
            {
                SqlParameter[] param = {new SqlParameter("InvNo", (object)ArID?? (object)DBNull.Value)
                                            ,new SqlParameter("CreatedBy", (object)CreatedBy?? (object)DBNull.Value)
                                             ,new SqlParameter("CheckType", (object)CheckType?? (object)DBNull.Value)
                                            };
                var result = _repository.ExecuteSQL<int>("BounceORCancelCheck", param).FirstOrDefault();

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
        [Route("GetChangeLogOfCheck")]
        public ApiResponse<ChangeLogCheckEntity> GetChangeLogOfCheck(int ArID)
        {
            var response = new ApiResponse<ChangeLogCheckEntity>();
            var result = new List<ChangeLogCheckEntity>();
            try
            {
                SqlParameter[] param = { new SqlParameter("ArID", (object)ArID ?? (object)DBNull.Value)};
                result = _repository.ExecuteSQL<ChangeLogCheckEntity>("GetChangeLogOfCheck", param).ToList();

                if (result != null && result.Count > 0)
                {
                    response.Data = result;
                }
                else
                {
                    result = new List<ChangeLogCheckEntity>();
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

        [Route("GetARInvoiceChangeLogByInvoiceId")]
        public ApiResponse<ChangeLogInvoiceEntity> GetARInvoiceChangeLogByInvoiceId(int ArID,int InvNo)
        {
            var response = new ApiResponse<ChangeLogInvoiceEntity>();
            var result = new List<ChangeLogInvoiceEntity>();
            try
            {
                SqlParameter[] param = { new SqlParameter("ArID", (object)ArID ?? (object)DBNull.Value),
                                        new SqlParameter("InvNo", (object)InvNo ?? (object)DBNull.Value)};
                result = _repository.ExecuteSQL<ChangeLogInvoiceEntity>("GetARInvoiceChangeLogByInvoiceId", param).ToList();

                if (result != null && result.Count > 0)
                {
                    response.Data = result;
                }
                else
                {
                    result = new List<ChangeLogInvoiceEntity>();
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

        [HttpGet]
        [Route("GetInvoiceDetailByInvoiceId")]
        public ApiResponse<InvoiceListEntity> GetInvoiceDetailByInvoiceId(int InvoiceNo=0)
        {
            var response = new ApiResponse<InvoiceListEntity>();
            var result = new List<InvoiceListEntity>();
            try
            {
                SqlParameter[] param = { new SqlParameter("InvoiceNo", (object)InvoiceNo ?? (object)DBNull.Value)};
                result = _repository.ExecuteSQL<InvoiceListEntity>("GetInvoiceDetailByInvoiceId", param).ToList();

                if (result != null && result.Count > 0)
                {
                    response.Data = result;
                }
                else
                {
                    result = new List<InvoiceListEntity>();
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
