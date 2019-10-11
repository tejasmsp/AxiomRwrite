using Axiom.Data.Repository;
using Axiom.Entity;
using Axiom.Web.API;
using Axiom.Web.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Axiom.Web.Controllers
{
    public class HomeController : Controller
    {
        public HomeApiController homeApiController = new HomeApiController();
        public RoleApiController roleApiController = new RoleApiController();
        private readonly GenericRepository<ProposalFeesApprovalModel> _repository = new GenericRepository<ProposalFeesApprovalModel>();

        public ActionResult Index()
        {
            if (ProjectSession.LoggedInUserDetail != null && !String.IsNullOrEmpty(ProjectSession.LoggedInUserDetail.UserId))
            {
                SetUserPermissions();

                return View();
                //if (ProjectSession.LoggedInUserDetail.RoleName.Contains("Administrator") || ProjectSession.LoggedInUserDetail.RoleName.Contains("DocumentAdmin"))
                //{
                //    return View();
                //}
                //else
                //{
                //    return RedirectToAction("Dashboard", "Home");
                //}
            }
            else
            {
                //if (Convert.ToString(Request.Url).Contains("Login/ResetPassword"))
                //{
                //    ResetPasswordEntity resetmodel = new ResetPasswordEntity();
                //    return View("Login/ResetPassword.cshtml", resetmodel);
                //}
                TempData["ReturnUrl"] = Convert.ToString(Request.Url);
                return RedirectToAction("Index", "Login");
            }
        }

        [HttpGet]
        [Route("Dashboard")]
        public ActionResult Dashboard()
        {
            if (ProjectSession.LoggedInUserDetail != null && !String.IsNullOrEmpty(ProjectSession.LoggedInUserDetail.UserId))
            {
                SetUserPermissions();
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        [AllowAnonymous]
        [HttpGet]
        [Route("ProposalFeeApproval")]
        public ActionResult ProposalFeeApproval(string type, string value)
        {
            var model = new ProposalFeesApprovalModel();
            try
            {

                if (!string.IsNullOrEmpty(value))
                {
                    string[] querystring = HttpUtility.UrlDecode(EncryptDecrypt.Decrypt(Convert.ToString(value))).Split(',');

                    model.accExecutiveEmail = Convert.ToString(querystring[0]);
                    model.accExecutiveName = Convert.ToString(querystring[1]);
                    model.orderno = Convert.ToString(querystring[2]);
                    model.location = Convert.ToString(querystring[3]);
                    model.pages = Convert.ToString(querystring[4]);
                    model.amount = Convert.ToString(querystring[5]);
                    model.proposalFeesID = Convert.ToString(querystring[6]);
                    model.part = Convert.ToString(querystring[7]);
                    model.Newamount = model.amount;
                    model.Newpages = model.pages;
                    model.operation = HttpUtility.UrlDecode(EncryptDecrypt.Decrypt(type)).ToString().ToLower();
                    model.isUpdated = false;
                    if (!string.IsNullOrEmpty(type))
                    {
                        if (model.operation == "a")
                            SendEmail(model, true);
                        else if (model.operation == "n")
                            SendEmail(model, false);
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return View(model);
        }
        private void SendEmail(ProposalFeesApprovalModel model, bool status)
        {
            string currentUrl = Request.Url.AbsoluteUri;
            var response = homeApiController.GetCompanyNoBySiteUrl(currentUrl);
            int CompanyNo = response.Data[0];

            CompanyDetailForEmailEntity objCompany = CommonFunction.CompanyDetailForEmail(CompanyNo);

            string subject = "Proposal Fees Request Status";
            string body = string.Empty;
            using (System.IO.StreamReader reader = new System.IO.StreamReader(AppDomain.CurrentDomain.BaseDirectory + "/MailTemplate/ProposalFeesReply.html"))
            {
                body = reader.ReadToEnd();
            }
            if (status)
                body = body.Replace("{STATUS}", "APPROVED");
            else
                body = body.Replace("{STATUS}", "NOT APPROVED");


            body = body.Replace("{UserName}", model.accExecutiveName);
            body = body.Replace("{ORDERNO}", model.orderno);
            body = body.Replace("{PARTNO}", model.part);
            body = body.Replace("{LOCATION}", model.location);
            body = body.Replace("{PAGES}", model.pages);
            body = body.Replace("{COST}", model.amount);
            body = body.Replace("{LogoURL}", objCompany.Logopath);
            body = body.Replace("{ThankYou}", objCompany.ThankYouMessage);
            body = body.Replace("{CompanyName}", objCompany.CompName);
            body = body.Replace("{Link}", objCompany.SiteURL);

            EmailHelper.Email.Send(model.accExecutiveEmail, body, subject, "", "tejaspadia@gmail.com;autoemail@axiomcopy.com");
            var feeStatus = status ? 1 : 2;
            SqlParameter[] sqlparam = {  new SqlParameter("ProposalFeesID", (object)model.proposalFeesID ?? (object)DBNull.Value),
                                new SqlParameter("FeesStatus", (object)feeStatus ?? (object)DBNull.Value),
                                new SqlParameter("Comments", (object)model.comment ?? (object)DBNull.Value)
                            };
            _repository.ExecuteSQL<int>("UpdateProposalFeeApproval", sqlparam).FirstOrDefault();

        }

        [AllowAnonymous]
        [HttpPost]
        [Route("UpdateProposalFeeApproval")]
        public ActionResult UpdateProposalFeeApproval(ProposalFeesApprovalModel model)
        {
            try
            {
                string currentUrl = Request.Url.AbsoluteUri;
                var response = homeApiController.GetCompanyNoBySiteUrl(currentUrl);
                int CompanyNo = response.Data[0];

                CompanyDetailForEmailEntity objCompany = CommonFunction.CompanyDetailForEmail(CompanyNo);

                string subject = "Proposal Fees Request Status - Edit Scope";
                string body = string.Empty;
                using (System.IO.StreamReader reader = new System.IO.StreamReader(AppDomain.CurrentDomain.BaseDirectory + "/MailTemplate/ProposalFeesReplyEditScope.html"))
                {
                    body = reader.ReadToEnd();
                }

                body = body.Replace("{UserName}", model.accExecutiveName);
                body = body.Replace("{ORDERNO}", model.orderno);
                body = body.Replace("{LOCATION}", model.location);
                body = body.Replace("{PAGES}", model.pages);
                body = body.Replace("{COST}", model.amount);

                body = body.Replace("{EDITEDPAGES}", model.Newpages);
                body = body.Replace("{EDITEDCOST}", model.Newamount);
                body = body.Replace("{COMMENTS}", model.comment);
                body = body.Replace("{LogoURL}", objCompany.Logopath);
                body = body.Replace("{ThankYou}", objCompany.ThankYouMessage);
                body = body.Replace("{CompanyName}", objCompany.CompName);
                body = body.Replace("{Link}", objCompany.SiteURL);

                EmailHelper.Email.Send(model.accExecutiveEmail, body, subject, "", "tejaspadia@gmail.com;autoemail@axiomcopy.com");

                SqlParameter[] saveparam = {  new SqlParameter("ProposalFeesID", (object)model.proposalFeesID ?? (object)DBNull.Value),
                                new SqlParameter("FeesStatus", (object)3 ?? (object)DBNull.Value),
                                new SqlParameter("Comments", (object)model.comment ?? (object)DBNull.Value)
                            };
                var proposalId = _repository.ExecuteSQL<int>("UpdateProposalFeeApproval", saveparam).FirstOrDefault();
                if (proposalId > 0)
                {
                    model.isUpdated = true;
                }
            }
            catch (Exception ex)
            {

            }
            return View("~/Views/Home/ProposalFeeApproval.cshtml", model);
        }

        [HttpGet]
        [Route("AccessDenied")]
        public ActionResult AccessDenied()
        {
            return View();
        }



        public void SetUserPermissions()
        {
            var RolePermissions = roleApiController.GetUserPermissions(ProjectSession.LoggedInUserDetail.UserAccessId.Value);
            if (RolePermissions != null && RolePermissions.Data.Count > 0)
            {
                ProjectSession.LoggedInUserDetail.Permissions = RolePermissions.Data.ToList();
            }
            else
            {
                ProjectSession.LoggedInUserDetail.Permissions = new List<Entity.RoleRightsEntity>();
            }

        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }


    }
}