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
using System.Text;
using System.IO;
using System.Configuration;

namespace Axiom.Web.API
{
    [RoutePrefix("api")]
    public class EmployeeApiController : ApiController
    {
        #region Initialization
        private readonly GenericRepository<EmployeeEntity> _repository = new GenericRepository<EmployeeEntity>();
        #endregion

        #region DatabaseOperations
        [HttpGet]
        [Route("GetEmployeeList")]
        public ApiResponse<EmployeeEntity> GetEmployeeList()
        {
            var response = new ApiResponse<EmployeeEntity>();
            try
            {
                var result = _repository.ExecuteSQL<EmployeeEntity>("GetEmployeeList").ToList();
                if (result == null)
                {
                    result = new List<EmployeeEntity>();
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
        [Route("GetEmployeeById")]
        public ApiResponse<EmployeeEntity> GetEmployeeById(string UserId)
        {
            var response = new ApiResponse<EmployeeEntity>();
            try
            {
                SqlParameter[] param = { new SqlParameter("userId", (object)UserId ?? (object)DBNull.Value) };
                var result = _repository.ExecuteSQL<EmployeeEntity>("GetEmployeeById", param).ToList();
                if (result == null)
                {
                    result = new List<EmployeeEntity>();
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
        [Route("InsertEmployee")]
        public BaseApiResponse InsertEmployee(EmployeeEntity model)
        {
            var response = new BaseApiResponse();
            string RandomPassword = Axiom.Common.CommonHelper.CreateRandomPassword(8);
            try
            {
                SqlParameter[] param = {  new SqlParameter("email", (object)model.Email ?? (object)DBNull.Value)
                                        , new SqlParameter("firstName", (object)model.FirstName ?? (object)DBNull.Value)
                                        , new SqlParameter("lastName", (object)model.LastName ?? (object)DBNull.Value)
                                        , new SqlParameter("password", (object)Security.Encrypt(RandomPassword) ?? (object)DBNull.Value)
                                        , new SqlParameter("departmentId", (object)model.DepartmentId ?? (object)DBNull.Value)
                                        , new SqlParameter("isLockedOut", (object)model.IsLockedOut ?? (object)DBNull.Value)
                                        //, new SqlParameter("isAdmin", (object)model.IsAdmin ?? (object)DBNull.Value)
                                        //, new SqlParameter("isDocumentAdmin", (object)model.IsDocumentAdmin ?? (object)DBNull.Value)
                                        , new SqlParameter("isApproved", (object)model.IsApproved ?? (object)DBNull.Value)
                                        , new SqlParameter("SelectedRoles", (object)model.SelectedRoles ?? (object)DBNull.Value)
                                        , new SqlParameter("createdBy", (object)model.CreatedBy ?? (object)DBNull.Value)
                                        , new SqlParameter("CompanyNo", (object)model.CompanyNo ?? (object)DBNull.Value)
                                        };

                var result = _repository.ExecuteSQL<string>("InsertEmployee", param).FirstOrDefault();
                if (result != string.Empty)
                {
                    CompanyDetailForEmailEntity objCompany = CommonFunction.CompanyDetailForEmail(model.CompanyNo);
                    System.Text.StringBuilder body = new StringBuilder();
                    string htmlfilePath = AppDomain.CurrentDomain.BaseDirectory + "/MailTemplate/WelcomeNewUserInternal.html";
                    using (System.IO.StreamReader reader = new StreamReader((htmlfilePath)))
                    {
                        body.Append(reader.ReadToEnd());
                    }

                    body = body.Replace("{FirstName}", model.FirstName);
                    body = body.Replace("{LastName}", model.LastName);
                    body = body.Replace("{Email}", model.Email);
                    body = body.Replace("{Password}", RandomPassword);
                    body = body.Replace("{Link}", ConfigurationManager.AppSettings["LiveSiteURL"].ToString());
                    body = body.Replace("{LogoURL}", objCompany.Logopath);
                    body = body.Replace("{ThankYou}", objCompany.ThankYouMessage);
                    body = body.Replace("{CompanyName}", objCompany.CompName);
                    body = body.Replace("{Link}", objCompany.SiteURL);

                    string subject = "Welcome To " + objCompany.CompName + " Requisition";                    
                    EmailHelper.Email.Send(model.Email, body.ToString(), subject, "", "tejaspadia@gmail.com,autharchive@axiomcopy.com");

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
        [Route("UpdateEmployee")]
        public BaseApiResponse UpdateEmployee(EmployeeEntity model)
        {
            var response = new BaseApiResponse();
            try
            {
                SqlParameter[] param = { new SqlParameter("userId", (object)model.UserId ?? (object)DBNull.Value)
                                        , new SqlParameter("email", (object)model.Email ?? (object)DBNull.Value)
                                        , new SqlParameter("firstName", (object)model.FirstName ?? (object)DBNull.Value)
                                        , new SqlParameter("lastName", (object)model.LastName ?? (object)DBNull.Value)
                                        , new SqlParameter("departmentId", (object)model.DepartmentId ?? (object)DBNull.Value)
                                        , new SqlParameter("isLockedOut", (object)model.IsLockedOut ?? (object)DBNull.Value)
                                        //, new SqlParameter("isAdmin", (object)model.IsAdmin ?? (object)DBNull.Value)
                                        //, new SqlParameter("isDocumentAdmin", (object)model.IsDocumentAdmin ?? (object)DBNull.Value)
                                        , new SqlParameter("isApproved", (object)model.IsApproved ?? (object)DBNull.Value)
                                        , new SqlParameter("SelectedRoles", (object)model.SelectedRoles ?? (object)DBNull.Value)
                                        , new SqlParameter("createdBy", (object)model.CreatedBy ?? (object)DBNull.Value)
                                        , new SqlParameter("CompanyNo", (object)model.CompanyNo ?? (object)DBNull.Value)


                                        };
                var result = _repository.ExecuteSQL<string>("UpdateEmployee", param).FirstOrDefault();
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

        #endregion
    }
}