using Axiom.Common;
using Axiom.Data.Repository;
using Axiom.Entity;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Http;

namespace Axiom.Web.API
{
    [RoutePrefix("api")]
    public class CompanyApicontroller : ApiController
    {
        #region Initialization
        private readonly GenericRepository<LocationEntity> _repository = new GenericRepository<LocationEntity>();
        #endregion

        #region Method
        [HttpGet]
        [Route("GetCompanyList")]
        public ApiResponse<CompanyEntity> GetCompanyList()
        {
            var response = new ApiResponse<CompanyEntity>();
            try
            {
                var result = _repository.ExecuteSQL<CompanyEntity>("GetCompanyList").ToList();
                if (result == null)
                {
                    result = new List<CompanyEntity>();
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


        [HttpGet]
        [Route("GetCompanyDetailById")]
        public ApiResponse<CompanyEntity> GetCompanyDetailById(int CompNo)
        {
            var response = new ApiResponse<CompanyEntity>();

            try
            {
                SqlParameter[] param = { new SqlParameter("compNo", (object)CompNo ?? (object)DBNull.Value) };
                var result = _repository.ExecuteSQL<CompanyEntity>("GetCompanyDetailById", param).ToList();
                if (result == null)
                {
                    result = new List<CompanyEntity>();
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
        [Route("InsertCompany")]
        public BaseApiResponse InsertCompany(CompanyEntity model)
        {
            var response = new BaseApiResponse();
            try
            {
                SqlParameter[] param = {
                                         new SqlParameter("compID",(object)model.CompID??(object)DBNull.Value)
                                        ,new SqlParameter("compName",(object)model.CompName??(object)DBNull.Value)
                                        ,new SqlParameter("street1",(object)model.Street1??(object)DBNull.Value)
                                        ,new SqlParameter("street2",(object)model.Street2??(object)DBNull.Value)
                                        ,new SqlParameter("city",(object)model.City??(object)DBNull.Value)
                                        ,new SqlParameter("state",(object)model.State??(object)DBNull.Value)
                                        ,new SqlParameter("zip",(object)model.Zip??(object)DBNull.Value)
                                        ,new SqlParameter("areaCode1",(object)model.AreaCode1??(object)DBNull.Value)
                                        ,new SqlParameter("phoneNo",(object)model.PhoneNo??(object)DBNull.Value)
                                        ,new SqlParameter("areaCode2",(object)model.AreaCode2??(object)DBNull.Value)
                                        ,new SqlParameter("faxNo",(object)model.FaxNo??(object)DBNull.Value)
                                        ,new SqlParameter("email",(object)model.Email??(object)DBNull.Value)
                                        ,new SqlParameter("taxID",(object)model.TaxID??(object)DBNull.Value)
                                        ,new SqlParameter("remitNo",(object)model.RemitNo??(object)DBNull.Value)
                                        ,new SqlParameter("chkNo",(object)model.ChkNo??(object)DBNull.Value)
                                        ,new SqlParameter("aRNo",(object)model.ARNo??(object)DBNull.Value)
                                        ,new SqlParameter("refundNo",(object)model.RefundNo??(object)DBNull.Value)
                                        ,new SqlParameter("fCNo",(object)model.FCNo??(object)DBNull.Value)
                                        ,new SqlParameter("dcntNo",(object)model.DcntNo??(object)DBNull.Value)
                                        ,new SqlParameter("debtNo",(object)model.DebtNo??(object)DBNull.Value)
                                        ,new SqlParameter("payrollNo",(object)model.PayrollNo??(object)DBNull.Value)
                                        ,new SqlParameter("lateDays",(object)model.LateDays??(object)DBNull.Value)
                                        ,new SqlParameter("onInv",(object)model.OnInv??(object)DBNull.Value)
                                        ,new SqlParameter("onStmt",(object)model.OnStmt??(object)DBNull.Value)
                                        ,new SqlParameter("showPage",(object)model.ShowPage??(object)DBNull.Value)
                                        ,new SqlParameter("dueDays",(object)model.DueDays??(object)DBNull.Value)
                                        ,new SqlParameter("preprtInv",(object)model.PreprtInv??(object)DBNull.Value)
                                        ,new SqlParameter("preprtStmt",(object)model.PreprtStmt??(object)DBNull.Value)
                                        ,new SqlParameter("incomeNo",(object)model.IncomeNo??(object)DBNull.Value)
                                        ,new SqlParameter("term",(object)model.Term??(object)DBNull.Value)
                                        ,new SqlParameter("notes",(object)model.Notes??(object)DBNull.Value)
                                        ,new SqlParameter("createdBy",(object)model.CreatedBy??(object)DBNull.Value)
                                        ,new SqlParameter("siteURL",(object)model.SiteURL??(object)DBNull.Value)
                                        ,new SqlParameter("AllowedURL",(object)model.AllowedURL??(object)DBNull.Value)
                                        };
                var result = _repository.ExecuteSQL<int>("InsertCompany", param).FirstOrDefault();
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
        [Route("UpdateCompanyDetail")]
        public BaseApiResponse UpdateCompanyDetail(CompanyEntity model)
        {
            var response = new BaseApiResponse();
            try
            {
                SqlParameter[] param = { new SqlParameter("compNo",(object)model.CompNo??(object)DBNull.Value)
                                        ,new SqlParameter("compID",(object)model.CompID.Trim()??(object)DBNull.Value)
                                        ,new SqlParameter("compName",(object)model.CompName.Trim()??(object)DBNull.Value)
                                        ,new SqlParameter("street1",(object)model.Street1.Trim()??(object)DBNull.Value)
                                        ,new SqlParameter("street2",(object)model.Street2.Trim()??(object)DBNull.Value)
                                        ,new SqlParameter("city",(object)model.City.Trim()??(object)DBNull.Value)
                                        ,new SqlParameter("state",(object)model.State.Trim()??(object)DBNull.Value)
                                        ,new SqlParameter("zip",(object)model.Zip.Trim()??(object)DBNull.Value)
                                        ,new SqlParameter("areaCode1",(object)model.AreaCode1.Trim()??(object)DBNull.Value)
                                        ,new SqlParameter("phoneNo",(object)model.PhoneNo.Trim()??(object)DBNull.Value)
                                        ,new SqlParameter("areaCode2",(object)model.AreaCode2.Trim()??(object)DBNull.Value)
                                        ,new SqlParameter("faxNo",(object)model.FaxNo.Trim()??(object)DBNull.Value)
                                        ,new SqlParameter("email",(object)model.Email.Trim()??(object)DBNull.Value)
                                        ,new SqlParameter("taxID",(object)model.TaxID.Trim()??(object)DBNull.Value)
                                        ,new SqlParameter("remitNo",(object)model.RemitNo??(object)DBNull.Value)
                                        ,new SqlParameter("chkNo",(object)model.ChkNo??(object)DBNull.Value)
                                        ,new SqlParameter("aRNo",(object)model.ARNo??(object)DBNull.Value)
                                        ,new SqlParameter("refundNo",(object)model.RefundNo??(object)DBNull.Value)
                                        ,new SqlParameter("fCNo",(object)model.FCNo??(object)DBNull.Value)
                                        ,new SqlParameter("dcntNo",(object)model.DcntNo??(object)DBNull.Value)
                                        ,new SqlParameter("debtNo",(object)model.DebtNo??(object)DBNull.Value)
                                        ,new SqlParameter("payrollNo",(object)model.PayrollNo??(object)DBNull.Value)
                                        ,new SqlParameter("lateDays",(object)model.LateDays??(object)DBNull.Value)
                                        ,new SqlParameter("onInv",(object)model.OnInv??(object)DBNull.Value)
                                        ,new SqlParameter("onStmt",(object)model.OnStmt??(object)DBNull.Value)
                                        ,new SqlParameter("showPage",(object)model.ShowPage??(object)DBNull.Value)
                                        ,new SqlParameter("dueDays",(object)model.DueDays??(object)DBNull.Value)
                                        ,new SqlParameter("preprtInv",(object)model.PreprtInv??(object)DBNull.Value)
                                        ,new SqlParameter("preprtStmt",(object)model.PreprtStmt??(object)DBNull.Value)
                                        ,new SqlParameter("incomeNo",(object)model.IncomeNo??(object)DBNull.Value)
                                        ,new SqlParameter("term",(object)model.Term??(object)DBNull.Value)
                                        ,new SqlParameter("notes",(object)model.Notes??(object)DBNull.Value)
                                        ,new SqlParameter("updatedBy",(object)model.CreatedBy??(object)DBNull.Value)                                        
                                        ,new SqlParameter("AllowedURL",(object)model.AllowedURL??(object)DBNull.Value)
                                        
                                        };
                var result = _repository.ExecuteSQL<int>("UpdateCompanyDetail", param).FirstOrDefault();
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
        [Route("DeleteCompany")]
        public BaseApiResponse DeleteCompany(int CompNo)
        {
            var response = new BaseApiResponse();
            try
            {
                SqlParameter[] param = {
                                        new SqlParameter("compNo",(object)CompNo??(object)DBNull.Value)
                                        };
                var result = _repository.ExecuteSQL<int>("DeleteCompany", param).FirstOrDefault();
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