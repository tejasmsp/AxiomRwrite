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


            string BtnType = string.Empty;
            string accExecutiveEmail = string.Empty;
            string accExecutiveName = string.Empty;
            string orderno = string.Empty;
            string location = string.Empty;
            string pages = string.Empty;
            string amount = string.Empty;
            string partno = string.Empty;
            string AttorneyNm = string.Empty;
            string AttyID = string.Empty;
            string FileVersionID = string.Empty;

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
            try
            {
                AttyID = querystring[8].ToString();
                FileVersionID = querystring[9].ToString();
            }
            catch (Exception ex)
            {

            }
            int CompanyNo = 0;
            CompanyDetailForEmailEntity objCompany = new CompanyDetailForEmailEntity();
            string currentUrl = Request.Url.AbsoluteUri;
            var response = homeApiController.GetCompanyNoBySiteUrl(currentUrl);
            CompanyNo = response.Data[0];
            objCompany = CommonFunction.CompanyDetailForEmail(CompanyNo);

            if (BtnType.ToLower() == "n")
            {
                string subject = "Billing Proposal Reply - Reject " + Convert.ToString(orderno) + "-" + Convert.ToString(partno);
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

                EmailHelper.Email.Send(CompanyNo: objCompany.CompNo
                    , mailTo: accExecutiveEmail
                    , body: body
                    , subject: subject
                    , ccMail: ""
                    , bccMail: "autharchive@axiomcopy.com,tejaspadia@gmail.com");

                UpdateWaiver(orderno, partno, AttyID, true);
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
            obj.AttyID = AttyID;
            obj.CompanyNo = CompanyNo.ToString();
            obj.CompanyName = objCompany.CompName;
            obj.LogoPath = objCompany.Logopath;
            obj.FileVersionID = FileVersionID;

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

            string subject = "Billing Proposal Reply - Approved " + Convert.ToString(obj.OrderNo) + "-" + Convert.ToString(obj.PartNo);
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

                EmailHelper.Email.Send(CompanyNo: objCompany.CompNo
                    , mailTo: obj.AccExecutiveEmail
                    , body: body
                    , subject: subject
                    , ccMail: ""
                    , bccMail: "autharchive@axiomcopy.com,tejaspadia@gmail.com");

                UpdateWaiver(obj.OrderNo, obj.PartNo, obj.AttyID, false);


                List<SoldAttorneyEntity> lstSoldAtty = new List<SoldAttorneyEntity>();
                SoldAttorneyEntity objSoldAtty = new SoldAttorneyEntity();
                objSoldAtty.AttyId = obj.AttyID;
                objSoldAtty.AttyType = "Ordering";
                lstSoldAtty.Add(objSoldAtty);

                BillingApiController ba = new BillingApiController();
                ba.GenerateInvoice(Convert.ToInt64(obj.OrderNo), Convert.ToInt32(obj.PartNo), obj.AttyID, objCompany.CompNo, lstSoldAtty,0,0,true);
                string DownLoadLink = GenerateDownloadLink(Convert.ToInt32(obj.FileVersionID), objCompany);
                objView.DownloadLink = DownLoadLink;
                objView.CompanyNo = CompanyNo.ToString();
                objView.CompanyName = objCompany.CompName;
                objView.LogoPath = objCompany.Logopath;
            }
            catch (Exception ex)
            {

            }
            return View("Index", objView);
        }


        public string GenerateDownloadLink(int FileVersionID, CompanyDetailForEmailEntity objCompany)
        {
            string returnLink = string.Empty;
            string siteURL = objCompany.SiteURL + "api/DownloadDocument?value=";
            FileVersionEntity FileEntity = new FileVersionEntity();
            SqlParameter[] param = {
                        new SqlParameter("FileVersionID", (object)FileVersionID ?? (object)DBNull.Value)};
            var result = _repository.ExecuteSQL<FileVersionEntity>("GetFileDetailByFileVersionID", param).FirstOrDefault();

            if (result != null)
            {
                string currentdate = DateTime.Now.AddDays(30).ToString();
                string link = result.FileDiskName + "," + result.FileName + "," + result.OrderNo.ToString() + "," + result.PartNo.ToString() + "," + currentdate;
                returnLink = siteURL + HttpUtility.UrlEncode(EncryptDecrypt.Encrypt(link));
            }

            return returnLink;
            // return new Document().DownloadFileFromServer(FileEntity.FileDiskName, FileEntity.FileName, FileEntity.OrderNo, FileEntity.PartNo);
        }


        public void UpdateWaiver(string OrderNo, string PartNo, string AttyID, bool Waiver)
        {
            try
            {
                SqlParameter[] param = {  new SqlParameter("OrderNo", (object)OrderNo?? (object)DBNull.Value)
                                                 ,new SqlParameter("PartNo",(object)PartNo?? (object)DBNull.Value)
                                                 ,new SqlParameter("AttyID",(object)AttyID?? (object)DBNull.Value)
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