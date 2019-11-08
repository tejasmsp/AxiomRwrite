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
using System.Web;

namespace Axiom.Web.API
{
    [RoutePrefix("api")]
    public class ProposalFeesApiController : ApiController
    {
        #region Initialization
        private readonly GenericRepository<OrderPartEntity> _repository = new GenericRepository<OrderPartEntity>();
        #endregion

        #region Method
        [HttpGet]
        [Route("GetProposalFees")]
        public ApiResponse<ProposalFeesEntity> GetProposalFees(int OrderId = 0, int PartNo = 0)
        {
            var response = new ApiResponse<ProposalFeesEntity>();
            try
            {
                SqlParameter[] param = {
                                         new SqlParameter("orderId", (object)OrderId ?? (object)DBNull.Value)
                                        ,new SqlParameter("partNo", (object)PartNo ?? (object)DBNull.Value)
                };
                var result = _repository.ExecuteSQL<ProposalFeesEntity>("GetProposalFees", param).ToList();
                if (result == null)
                {
                    result = new List<ProposalFeesEntity>();
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
        [Route("GetProposalFeesCalculation")]
        public ApiResponse<ProposalFeesEntity> GetProposalFeesCalculation(int OrderId = 0, int PartNo = 0, int RecordTypeId = 0, int PageNo = 0)
        {
            var response = new ApiResponse<ProposalFeesEntity>();
            try
            {
                SqlParameter[] param = {
                                         new SqlParameter("OrderNo", (object)OrderId ?? (object)DBNull.Value)
                                        ,new SqlParameter("PartNo", (object)PartNo ?? (object)DBNull.Value)
                                        ,new SqlParameter("AttyType", "Ordering")
                                        ,new SqlParameter("RecordType", (object)RecordTypeId ?? (object)DBNull.Value)
                                        ,new SqlParameter("Pages", (object)PageNo ?? (object)DBNull.Value)
                };
                var result = _repository.ExecuteSQL<ProposalFeesEntity>("CalculateProposalFees", param).ToList();
                if (result == null)
                {
                    result = new List<ProposalFeesEntity>();
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
        [Route("VerifyProposalFees")]
        public BaseApiResponse VerifyProposalFees(ProposalFeesEntity model)
        {
            var response = new BaseApiResponse();

            try
            {

                SqlParameter[] saveparam = {  new SqlParameter("OrderNo", (object)model.OrderNo ?? (object)DBNull.Value),
                                new SqlParameter("PartNo", (object)model.PartNo ?? (object)DBNull.Value),
                                new SqlParameter("Pages", (object)model.Pages ?? (object)DBNull.Value),
                                new SqlParameter("Amount", (object)Convert.ToDecimal(model.Amount) ?? (object)DBNull.Value),
                                new SqlParameter("Descr", (object)model.Descr ?? (object)DBNull.Value),
                                new SqlParameter("EntBy", (object)model.EntBy ?? (object)DBNull.Value),
                                new SqlParameter("ISRemindEmaiDate", (object)false ?? (object)DBNull.Value),
                            };
                var proposalId = _repository.ExecuteSQL<int>("SaveProposalFee", saveparam).FirstOrDefault();
                if (proposalId > 0)
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
        [Route("SaveProposalFees")]
        public BaseApiResponse SaveProposalFees(ProposalFeesEntity model)
        {
            var response = new BaseApiResponse();

            //This url will come from company details.
            //string LiveSiteURL = System.Configuration.ConfigurationManager.AppSettings["LiveSiteURL"].ToString();
            try
            {
                SqlParameter[] param = { new SqlParameter("OrderNo", (object)model.OrderNo ?? (object)DBNull.Value) };
                var proposalOrderDetail = _repository.ExecuteSQL<ProposalOrderDetails>("GetProposalOrderDetails", param).FirstOrDefault();
                if (proposalOrderDetail != null)
                {
                    if (string.IsNullOrEmpty(proposalOrderDetail.Email.Trim()))
                    {
                        response.Message.Add("Email Address not found for Attorney - " + Convert.ToString(proposalOrderDetail.OrderingAttorney));
                        return response;
                    }
                    else
                    {
                        SqlParameter[] locationParam = { new SqlParameter("OrderNo", (object)model.OrderNo ?? (object)DBNull.Value),
                            new SqlParameter("PratNo", (object)model.PartNo ?? (object)DBNull.Value),
                        };
                        var location = _repository.ExecuteSQL<LocationEntity>("GetLocationByOrderPartNo", locationParam).FirstOrDefault();

                        //Save Proposal 
                        SqlParameter[] saveparam = {  new SqlParameter("OrderNo", (object)model.OrderNo ?? (object)DBNull.Value),
                                new SqlParameter("PartNo", (object)model.PartNo ?? (object)DBNull.Value),
                                new SqlParameter("Pages", (object)model.Pages ?? (object)DBNull.Value),
                                new SqlParameter("Amount", (object)Convert.ToDecimal(model.Amount) ?? (object)DBNull.Value),
                                new SqlParameter("Descr", (object)model.Descr ?? (object)DBNull.Value),
                                new SqlParameter("EntBy", (object)model.EntBy ?? (object)DBNull.Value),
                                new SqlParameter("ISRemindEmaiDate", (object)true ?? (object)DBNull.Value),
                            };
                        var proposalId = _repository.ExecuteSQL<int>("SaveProposalFee", saveparam).FirstOrDefault();
                        if (proposalId > 0)
                        {
                            string subject = "Proposal Fees Request " + Convert.ToString(model.OrderNo) + "-" + Convert.ToString(model.PartNo);
                            string body = string.Empty;

                            CompanyDetailForEmailEntity objCompany = CommonFunction.CompanyDetailForEmail(model.CompanyNo);

                            using (System.IO.StreamReader reader = new System.IO.StreamReader(AppDomain.CurrentDomain.BaseDirectory + "/MailTemplate/ProposalFees.html"))
                            {
                                body = reader.ReadToEnd();
                            }
                            body = body.Replace("{UserName}", proposalOrderDetail.FirstName.Trim() + " " + proposalOrderDetail.LastName.Trim());
                            body = body.Replace("{ORDERNO}", Convert.ToString(model.OrderNo));
                            body = body.Replace("{RECORDSOF}", proposalOrderDetail.PatientName);
                            body = body.Replace("{LOCATION}", location.Name1.Trim() + " " + location.Name2.Trim() + " (" + location.LocID + ")");
                            body = body.Replace("{PAGES}", Convert.ToString(model.Pages));
                            body = body.Replace("{DESC}", model.Descr);
                            body = body.Replace("{COST}", Convert.ToString(model.Amount));
                            body = body.Replace("{LogoURL}", objCompany.Logopath);
                            body = body.Replace("{ThankYou}", objCompany.ThankYouMessage);
                            body = body.Replace("{CompanyName}", objCompany.CompName);
                            body = body.Replace("{Link}", objCompany.SiteURL);

                            string accExecutiveName = "Admin";
                            string accExecutiveEmail = "Fees@axiomcopy.com";

                            string strApproveLink, strNotApprovedLink, strEditScopeLink, strQueryString;
                            strQueryString = accExecutiveEmail + "," + accExecutiveName + "," + model.OrderNo + "," + location.Name1.Replace(',', ' ') + " (" + location.LocID + ")" + "," + model.Pages + "," + model.Amount +
                                "," + proposalId.ToString() + "," + model.PartNo.ToString();
                            strApproveLink = HttpUtility.UrlEncode(EncryptDecrypt.Encrypt(strQueryString));
                            strNotApprovedLink = HttpUtility.UrlEncode(EncryptDecrypt.Encrypt(strQueryString));
                            strEditScopeLink = HttpUtility.UrlEncode(EncryptDecrypt.Encrypt(strQueryString));

                            body = body.Replace("{APPROVELINK}", objCompany.SiteURL + "/ProposalFeeApproval?type=" + HttpUtility.UrlEncode(EncryptDecrypt.Encrypt("A")) + "&value=" + strApproveLink);
                            body = body.Replace("{NOTAPPROVELINK}", objCompany.SiteURL + "/ProposalFeeApproval?type=" + HttpUtility.UrlEncode(EncryptDecrypt.Encrypt("N")) + "&value=" + strNotApprovedLink);
                            body = body.Replace("{EDITSCOPELINK}", objCompany.SiteURL + "/ProposalFeeApproval?type=" + HttpUtility.UrlEncode(EncryptDecrypt.Encrypt("E")) + "&value=" + strEditScopeLink);




                            SqlParameter[] sqlparameter = { new SqlParameter("OrderNo", (object)model.OrderNo ?? (object)DBNull.Value),
                            new SqlParameter("AttyID", (object)proposalOrderDetail.OrderingAttorney ?? (object)DBNull.Value)};
                            var additionalContacts = _repository.ExecuteSQL<NotificationEmailEntity>("GetProposalUserNotification", sqlparameter).ToList();
                            string additionalEmail = string.Empty;

                            if (additionalContacts != null && additionalContacts.Count > 0)
                            {
                                foreach (var item in additionalContacts)
                                {
                                    additionalEmail += item.AssistantEmail + ",";
                                }
                                additionalEmail = additionalEmail.Trim(',');
                            }

                            string orderFeesEmails = "";
                            SqlParameter[] orderfeesparam = { new SqlParameter("OrderNo", (object)model.OrderNo ?? (object)DBNull.Value),
                            new SqlParameter("IsOrderConfirm", (object)false ?? (object)DBNull.Value),
                            new SqlParameter("IsFeeApproval", (object)true ?? (object)DBNull.Value)};

                            var orderFeesEmailList = _repository.ExecuteSQL<string>("GetAttorneyEmailList", orderfeesparam).ToList();
                            if (orderFeesEmailList != null && orderFeesEmailList.Count > 0)
                            {
                                foreach (var item in orderFeesEmailList)
                                {
                                    orderFeesEmails += item + ",";
                                }
                                orderFeesEmails = orderFeesEmails.Remove(orderFeesEmails.Length - 1);
                            }
                            string emailIds = "";
                            if (string.IsNullOrEmpty(additionalEmail))
                            {
                                if (!string.IsNullOrEmpty(proposalOrderDetail.Email))
                                    emailIds = proposalOrderDetail.Email;
                                if (!string.IsNullOrEmpty(emailIds))
                                    emailIds += ",";
                                emailIds += orderFeesEmails;
                                EmailHelper.Email.Send(emailIds, body, subject, "", "tejaspadia@gmail.com;autoemail@axiomcopy.com");
                            }
                            else
                            {
                                emailIds = additionalEmail;
                                if (!string.IsNullOrEmpty(emailIds))
                                    emailIds += ",";
                                emailIds += orderFeesEmails;
                                EmailHelper.Email.Send(emailIds, body, subject, "", "tejaspadia@gmail.com;autoemail@axiomcopy.com");
                            }
                        }
                        response.Success = true;

                    }
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