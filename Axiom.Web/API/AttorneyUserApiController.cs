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
using System.IO;
using System.Text;
using System.Configuration;

namespace Axiom.Web.API
{
    [RoutePrefix("api")]
    public class AttorneyUserApiController : ApiController
    {
        #region Initialization
        private readonly GenericRepository<AttorneyUsersEntity> _repository = new GenericRepository<AttorneyUsersEntity>();
        #endregion

        #region DatabaseOperations
        [HttpGet]
        [Route("GetAttorneyUsers")]
        public ApiResponse<AttorneyUsersEntity> GetAttorneyUsers(string attorneyUserId, int CompanyNo)
        {
            var response = new ApiResponse<AttorneyUsersEntity>();

            try
            {
                SqlParameter[] param = { new SqlParameter("AttorneyUserId", (object)attorneyUserId ?? (object)DBNull.Value),
                new SqlParameter("CompanyNo", (object)CompanyNo ?? (object)DBNull.Value)};
                var result = _repository.ExecuteSQL<AttorneyUsersEntity>("GetAttorneyUsers", param).ToList();

                if (result == null)
                {
                    result = new List<AttorneyUsersEntity>();
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
        [Route("GetAttorneyListWithSearchCriteria")]
        public ApiResponse<AttorneyUsersEntity> GetAttorneyListWithSearchCriteria(string SearchCriteria, string SearchCondition, string SearchText = "")
        {
            var response = new ApiResponse<AttorneyUsersEntity>();

            try
            {
                SqlParameter[] param = { new SqlParameter("SearchCriteria", (object)SearchCriteria ?? (object)DBNull.Value),
                                         new SqlParameter("SearchCondition", (object)SearchCondition ?? (object)DBNull.Value),
                                         new SqlParameter("SearchText", (object)SearchText ?? (object)DBNull.Value)
                };
                var result = _repository.ExecuteSQL<AttorneyUsersEntity>("GetAttorneyListWithSearchCriteria", param).ToList();

                if (result == null)
                {
                    result = new List<AttorneyUsersEntity>();
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
        [Route("DeleteAttorneyUser")]
        public BaseApiResponse DeleteAttorneyUser(string attorneyUserId)
        {
            var response = new BaseApiResponse();

            try
            {
                SqlParameter[] param = { new SqlParameter("AttorneyUserId", (object)attorneyUserId ?? (object)DBNull.Value) };
                var result = _repository.ExecuteSQL<string>("DeleteAttorneyUser", param).FirstOrDefault();

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

        [HttpGet]
        [Route("ActivateInactiveAttorneyUser")]
        public BaseApiResponse ActivateInactiveAttorneyUser(string attorneyUserId)
        {
            var response = new BaseApiResponse();

            try
            {
                SqlParameter[] param = { new SqlParameter("AttorneyUserId", (object)attorneyUserId ?? (object)DBNull.Value) };
                var result = _repository.ExecuteSQL<string>("ActivateInactiveAttorneyUser", param).FirstOrDefault();

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
        [Route("InsertAttorneyUser")]
        public BaseApiResponse InsertAttorneyUser(AttorneyUsersEntity model)
        {
            var response = new BaseApiResponse();
            string RandomPassword = Axiom.Common.CommonHelper.CreateRandomPassword(8);
            CompanyDetailForEmailEntity objCompany = CommonFunction.CompanyDetailForEmail(model.CompanyNo);
            try
            {
                SqlParameter[] param = { new SqlParameter("Email", (object)model.Email ?? (object)DBNull.Value)
                                        , new SqlParameter("FirstName", (object)model.FirstName ?? (object)DBNull.Value)
                                        , new SqlParameter("LastName", (object)model.LastName ?? (object)DBNull.Value)
                                        , new SqlParameter("AttorneyEmployeeTypeId", (object)model.AttorneyEmployeeTypeId ?? (object)DBNull.Value)
                                        , new SqlParameter("Password", (object)Security.Encrypt(RandomPassword) ?? (object)DBNull.Value)
                                        , new SqlParameter("CreatedBy", (object)model.CreatedBy ?? (object)DBNull.Value)
                                        , new SqlParameter("CompanyNo", (object)model.CompanyNo ?? (object)DBNull.Value)
                                        };
                var result = _repository.ExecuteSQL<string>("InsertAttorneyUser", param).FirstOrDefault();
                if (result != string.Empty)
                {
                    // SEND WELCOME EMAIL TO USER ATTORNEY
                    StringBuilder body = new StringBuilder();
                    string htmlfilePath = AppDomain.CurrentDomain.BaseDirectory + "/MailTemplate/WelcomeNewUser.html";
                    using (StreamReader reader = new StreamReader((htmlfilePath)))
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
                    EmailHelper.Email.Send(model.Email, body.ToString(), subject, "", "tejaspadia@gmail.com,j.alspaugh@axiomcopy.com");

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
        [Route("UpdateAttorneyUser")]
        public BaseApiResponse UpdateAttorneyUser(AttorneyUsersEntity model)
        {
            var response = new BaseApiResponse();
            try
            {
                SqlParameter[] param = { new SqlParameter("Email", (object)model.Email ?? (object)DBNull.Value)
                                        , new SqlParameter("FirstName", (object)model.FirstName ?? (object)DBNull.Value)
                                        , new SqlParameter("LastName", (object)model.LastName ?? (object)DBNull.Value)
                                        , new SqlParameter("AttorneyEmployeeTypeId", (object)model.AttorneyEmployeeTypeId ?? (object)DBNull.Value)
                                        , new SqlParameter("UserId", (object)model.AttorneyUserId ?? (object)DBNull.Value)
                                        , new SqlParameter("CreatedBy", (object)model.CreatedBy ?? (object)DBNull.Value)
                                        , new SqlParameter("CompanyNo", (object)model.CompanyNo ?? (object)DBNull.Value)
                                        };
                var result = _repository.ExecuteSQL<string>("UpdateAttorneyUser", param).FirstOrDefault();
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