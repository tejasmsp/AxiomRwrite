using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Axiom.Data.Repository;
using Axiom.Entity;
using Axiom.Web.API;

namespace Axiom.Web.Controllers
{
    public class BillingProposalController : Controller
    {
        public HomeApiController homeApiController = new HomeApiController();
        private readonly GenericRepository<Entity.BillingProposalReply> _repository = new GenericRepository<Entity.BillingProposalReply>();

        // GET: BillingProposal
        [AllowAnonymous]
        [HttpGet]
        [Route("BillingProposal")]
        public ActionResult Index(string type, string value)
        {
            BillingProposalReply obj = new BillingProposalReply();

            string BtnType;
            string accExecutiveEmail;
            string accExecutiveName;
            string orderno;
            string location;
            string pages;
            string amount;
            string partno;
            string AttorneyNm;
            BtnType = (HttpUtility.UrlDecode(EncryptDecrypt.Decrypt(type)));
            string[] querystring = HttpUtility.UrlDecode(EncryptDecrypt.Decrypt(value)).Split(',');
            //strQueryString = accExecutiveEmail + "," + accExecutiveName + "," + orderNo + "," + location.Name1 + " " + location.Name1 + "(" + location.LocID + ")" + "," + strPages + "," + strAmount;
            accExecutiveEmail = querystring[0].ToString();
            accExecutiveName = querystring[1].ToString();
            orderno = querystring[2].ToString();
            location = querystring[3].ToString();
            pages = querystring[4].ToString();
            amount = querystring[5].ToString();
            partno = querystring[6].ToString();
            AttorneyNm = querystring[7].ToString();

            if (BtnType.ToLower() == "n")
            {
                string currentUrl = Request.Url.AbsoluteUri;
                var response = homeApiController.GetCompanyNoBySiteUrl(currentUrl);
                int CompanyNo = response.Data[0];

                CompanyDetailForEmailEntity objCompany = CommonFunction.CompanyDetailForEmail(CompanyNo);

                string subject = "Billing Proposal Reply - Reject";
                string body = string.Empty;
                using (System.IO.StreamReader reader = new System.IO.StreamReader(AppDomain.CurrentDomain.BaseDirectory + "/MailTemplate/WaiverBillingReplyRejected.html"))
                {
                    body = reader.ReadToEnd();
                }

                body = body.Replace("{UserName}", accExecutiveName);
                body = body.Replace("{ORDERNO}", orderno);
                body = body.Replace("{LOCATION}", location);
                body = body.Replace("{PAGES}", pages);
                body = body.Replace("{COST}", amount);
                body = body.Replace("{ATTYINFO}", AttorneyNm); //23052016
                body = body.Replace("{PARTNO}", partno);

                body = body.Replace("{LogoURL}", objCompany.Logopath);
                body = body.Replace("{ThankYou}", objCompany.ThankYouMessage);
                body = body.Replace("{CompanyName}", objCompany.CompName);
                body = body.Replace("{Link}", objCompany.SiteURL);

                EmailHelper.Email.Send(accExecutiveEmail, body, subject, "", "tejaspadia@gmail.com,j.alspaugh@axiomcopy.com");
                UpdateWaiver(orderno, partno, true);
            }

            obj.AccExecutiveEmail = accExecutiveEmail;
            obj.AccExecutiveName = accExecutiveName;
            obj.OrderNo = orderno;
            obj.Location = location;
            obj.Pages = pages;
            obj.Amount = amount;
            obj.AttorneyNm = AttorneyNm;
            obj.PartNo = partno;
            obj.BtnType = BtnType.ToLower();

            return View(obj);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("UpdateBillingProposal")]
        public ActionResult UpdateBillingProposal(BillingProposalReply obj)
        {
            BillingProposalReply objView = new BillingProposalReply();

            string currentUrl = Request.Url.AbsoluteUri;
            var response = homeApiController.GetCompanyNoBySiteUrl(currentUrl);
            int CompanyNo = response.Data[0];

            CompanyDetailForEmailEntity objCompany = CommonFunction.CompanyDetailForEmail(CompanyNo);

            string subject = "Billing Proposal Reply - Approved";
            string body = string.Empty;
            try
            {
                using (System.IO.StreamReader reader = new System.IO.StreamReader(AppDomain.CurrentDomain.BaseDirectory + "/MailTemplate/WaiverBillingReply.html"))
                {
                    body = reader.ReadToEnd();
                }

                body = body.Replace("{UserName}", obj.AccExecutiveName);
                body = body.Replace("{ORDERNO}", obj.OrderNo);
                body = body.Replace("{LOCATION}", obj.Location);
                body = body.Replace("{PAGES}", obj.Pages);
                body = body.Replace("{COST}", obj.Amount);
                body = body.Replace("{ATTYINFO}", obj.AttorneyNm); //23052016
                body = body.Replace("{PARTNO}", obj.PartNo);
                body = body.Replace("{DOCPREF}", obj.DocPreference ? "Digital" : "Hard Copy");
                body = body.Replace("{COMMENTS}", obj.Comment);

                body = body.Replace("{LogoURL}", objCompany.Logopath);
                body = body.Replace("{ThankYou}", objCompany.ThankYouMessage);
                body = body.Replace("{CompanyName}", objCompany.CompName);
                body = body.Replace("{Link}", objCompany.SiteURL);

                EmailHelper.Email.Send(obj.AccExecutiveEmail, body, subject, "", "j.alspaugh@axiomcopy.com,tejaspadia@gmail.com");

                UpdateWaiver(obj.OrderNo, obj.PartNo, false);
            }
            catch (Exception ex)
            {

            }
            return View("Index", objView);
        }
        public void UpdateWaiver(string OrderNo, string PartNo, bool Waiver)
        {
            try
            {
                SqlParameter[] param = {  new SqlParameter("OrderNo", (object)OrderNo?? (object)DBNull.Value)
                                                 ,new SqlParameter("PartNo",(object)PartNo?? (object)DBNull.Value)
                                                 ,new SqlParameter("Waiver",(object)Waiver?? (object)DBNull.Value)
                                         };
                var result = _repository.ExecuteSQL<int>("SendMailForInvoiceUpdateWaiver", param).FirstOrDefault();
            }
            catch (Exception ex)
            {

            }



        }
    }
}