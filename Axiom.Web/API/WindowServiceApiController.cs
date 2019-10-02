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
using System.IO;
using System.Data;
using System.Reflection;
using System.Web;
using System.Security.Cryptography;
using System.Net.Http.Headers;
using AXIOM.Common;
using System.Text;

namespace Axiom.Web.API
{
    [RoutePrefix("api")]
    public class BillingApiController : ApiController
    {
        #region Initialization
        private readonly GenericRepository<SSNSettings> _repository = new GenericRepository<SSNSettings>();
        #endregion


        [HttpGet]
        [Route("SendEmailForBill")]
        public ApiResponse<SendEmailForInvoice> SendEmailForBill(string AttyID, string InvoiceNumber, string OrderNumber, string PartNumber, string Location, string LocationName, string Patient,string UserGuid)
        {
            var response = new ApiResponse<SendEmailForInvoice>();
            // var OrderDetail = new ApiResponse<PrintInvoiceEmailBillOrderDetail>
            try
            {
                SqlParameter[] param = {
                    new SqlParameter("AttyID", (object)AttyID ?? (object)DBNull.Value)
                    ,new SqlParameter("InvoiceNumber", (object)InvoiceNumber ?? (object)DBNull.Value)
                    ,new SqlParameter("OrderNumber", (object)OrderNumber ?? (object)DBNull.Value)
                };

                var result = _repository.ExecuteSQL<SendEmailForInvoice>("SendMailForInvoice", param).ToList();

                // var OrderDetail = _repository.ExecuteSQL<PrintInvoiceEmailBillOrderDetail>("InvoiceEmailOrderDetail", new SqlParameter("InvoiceNumber", (object)InvoiceNumber ?? (object)DBNull.Value)).ToList();

                SqlParameter[] param1 = { new SqlParameter("UserId", (object)UserGuid ?? (object)DBNull.Value) };
                var AccountExec = _repository.ExecuteSQL<AccountExecutive>("GetClientAcctExec", param1).FirstOrDefault();

                string accExecutiveName = string.Empty;
                string accExecutiveEmail = string.Empty;

                if (AccountExec != null)
                {
                    accExecutiveName = Convert.ToString(AccountExec.Name);
                    accExecutiveEmail = Convert.ToString(AccountExec.Email);
                }
                else
                {
                    accExecutiveName = "Leah Boroski";
                    accExecutiveEmail = "leah.boroski@axiomcopy.com";
                }

                if (result == null)
                {
                    result = new List<SendEmailForInvoice>();
                }

                foreach (SendEmailForInvoice item in result)
                {
                    string subject = "Billing Proposal";
                    string body = string.Empty;
                    using (System.IO.StreamReader reader = new System.IO.StreamReader(HttpContext.Current.Server.MapPath("~/MailTemplate/WaiverBillingProposal.html")))
                    {
                        body = reader.ReadToEnd();
                    }

                    string strAmount = item.IsPatientAttorney ? item.InvAmtForPatientAttorney.ToString("F") : item.InvAmt.ToString("F");

                    body = body.Replace("{UserName}", Convert.ToString(item.AttorneyName).Trim());


                    body = body.Replace("{Attorney}", Convert.ToString(item.OrderingAttorneyName));
                    body = body.Replace("{Firm}", Convert.ToString(item.OrderingFirmName).Trim());
                    body = body.Replace("{caption}", Convert.ToString(item.Caption1).Trim());
                    body = body.Replace("{RECORDSOF}", Convert.ToString(item.PatientName).Trim());


                    body = body.Replace("{ORDERNO}", item.OrderNo.ToString());

                    body = body.Replace("{LOCATION}", Convert.ToString(item.LocationName).Replace(',', ' ') + " (" + Convert.ToString(item.LocID) + ")");
                    //  body = body.Replace("{RecordType}", location.Descr.Trim());
                    body = body.Replace("{RecordType}", Convert.ToString(item.InvHdr));//INVHdr OF Item TABLE
                    body = body.Replace("{PAGES}", Convert.ToString(item.Pages));
                    body = body.Replace("{COST}", strAmount);


                    

                    string AttorneyNm = item.AttorneyName + " (" + Convert.ToString(item.AttorneyEmail).Trim() + ")";
                    //Guid value = new Guid(Membership.GetUser().ProviderUserKey.ToString());

                    //Guid value = new Guid("3668BE61-8089-42AC-AC30-8334BE2C7410");
                    //retval.CreateErrorResponse(retval.message + value.ToString() + Membership.GetUser().ProviderUserKey.ToString());

                    //GetClientAcctExecResult accExecutive = mr.GetClientAcctExec(value).FirstOrDefault<GetClientAcctExecResult>();
                    //if (accExecutive != null)
                    //{
                    //    accExecutiveName = Convert.ToString(accExecutive.Name);
                    //    accExecutiveEmail = Convert.ToString(accExecutive.Email);
                    //}
                    //else
                    //{
                    //    accExecutiveName = "Leah Boroski";
                    //    accExecutiveEmail = "leah.boroski@axiomcopy.com";
                    //}

                    string orderNo = item.OrderNo.ToString();
                    string strPages = item.Pages.ToString();
                    
                    
                    string WaiverID = string.Empty;


                    string strApproveLink, strNotApprovedLink, strEditScopeLink, strQueryString;
                    strQueryString = accExecutiveEmail + "," + accExecutiveName + "," + orderNo + "," + item.LocationName.Replace(',', ' ') + " (" + item.LocID + ")" + "," + strPages + "," + strAmount + "," + item.PartNo.ToString() + "," + AttorneyNm;

                    strApproveLink = HttpUtility.UrlEncode(EncryptDecrypt.Encrypt(strQueryString));
                    strNotApprovedLink = HttpUtility.UrlEncode(EncryptDecrypt.Encrypt(strQueryString));
                    strEditScopeLink = HttpUtility.UrlEncode(EncryptDecrypt.Encrypt(strQueryString));

                    string BaseLink = "https://axiomcopyonline.com/BillingProposal?type=";
                    // string BaseLink = "http://localhost:8086/BillingProposal?type=";

                    // FOR LOCAL MACHINE
                    //body = body.Replace("{YESLINK}", "http://localhost:51617/WaiverBilling.aspx?type=" + HttpUtility.UrlEncode(EncryptDecrypt.Encrypt("Y")) + "&value=" + strApproveLink);
                    //body = body.Replace("{NOLINK}", "http://localhost:51617/WaiverBilling.aspx?type=" + HttpUtility.UrlEncode(EncryptDecrypt.Encrypt("N")) + "&value=" + strNotApprovedLink);


                    // FOR DEMO SITE
                    //body = body.Replace("{YESLINK}", "http://192.168.0.22:8080/Clients/WaiverBilling.aspx?type=" + HttpUtility.UrlEncode(EncryptDecrypt.Encrypt("Y")) + "&value=" + strApproveLink);
                    //body = body.Replace("{NOLINK}", "http://192.168.0.22:8080/Clients/WaiverBilling.aspx?type=" + HttpUtility.UrlEncode(EncryptDecrypt.Encrypt("N")) + "&value=" + strNotApprovedLink);


                    // FOR LIVE SITE
                    //body = body.Replace("{YESLINK}", "https://www.axiomcopyonline.com/Clients/WaiverBilling.aspx?type=" + HttpUtility.UrlEncode(EncryptDecrypt.Encrypt("Y")) + "&value=" + strApproveLink);
                    //body = body.Replace("{NOLINK}", "https://www.axiomcopyonline.com/Clients/WaiverBilling.aspx?type=" + HttpUtility.UrlEncode(EncryptDecrypt.Encrypt("N")) + "&value=" + strNotApprovedLink);

                    body = body.Replace("{YESLINK}", BaseLink + HttpUtility.UrlEncode(EncryptDecrypt.Encrypt("Y")) + "&value=" + strApproveLink);
                    body = body.Replace("{NOLINK}", BaseLink + HttpUtility.UrlEncode(EncryptDecrypt.Encrypt("N")) + "&value=" + strNotApprovedLink);

                    //Add Attorney Email

                    //var OrderFirmAttrorneyEmailIds = GetAttorneyEmailListByOrderId(OrderNo);
                    //if (!String.IsNullOrEmpty(OrderFirmAttrorneyEmailIds))
                    //{
                    //    item.Email = item.Email + "," + OrderFirmAttrorneyEmailIds;
                    //}
                    //item.Email = item.Email.Trim(',');

                    EmailHelper.Email.Send(item.AttorneyEmail, body, subject, "", "tejaspadia@gmail.com");

                    //AxiomEmail.SendMailBilling(subject, body, "sejal.k@shaligraminfotech.com", true, "AxiomSupport@axiomcopy.com", null, "");
                    // AxiomEmail.SendMailBilling(subject, body, waiver.Email.ToString(), true, "AxiomSupport@axiomcopy.com", null, "tejaspadia@gmail.com,autoemail@axiomcopy.com");

                }

                #region Comment
                //foreach (var item in result)
                //{
                //    if (item.Type != "Attorney")
                //        continue;
                //    string subject = "Your Records Are Available";
                //    string Sendto = item.Email;

                //    System.Text.StringBuilder body = new System.Text.StringBuilder();
                //    using (System.IO.StreamReader reader = new System.IO.StreamReader(HttpContext.Current.Server.MapPath("~/MailTemplate/BillingRecords.html")))
                //    {
                //        body.Append(reader.ReadToEnd());
                //    }

                //    body = body.Replace("{UserName}", "Hello " + item.FirstName.Trim() + " " + item.LastName.Trim() + ",");
                //    body = body.Replace("{LOCATION}", LocationName + " (" + Location + ")");
                //    body = body.Replace("{PATIENT}", Patient);
                //    body = body.Replace("{CLAIMNO}", Convert.ToString(OrderDetail[0].BillingClaimNo));
                //    body = body.Replace("{ORDERNO}", OrderNumber + "-" + PartNumber);
                //    body = body.Replace("{InvHdr}", Convert.ToString(OrderDetail[0].InvHdr));
                //    body = body.Replace("{Pages}", Convert.ToString(OrderDetail[0].Pages));
                //    body = body.Replace("{LINK}", "o=" + OrderNumber + "&p=" + PartNumber);

                //    // EmailHelper.Email.Send(item.Email, body.ToString(), subject, "", "j.alspaugh@axiomcopy.com", "tejas.p@cementdigital.com");
                //    EmailHelper.Email.Send(item.Email, body.ToString(),subject, "tejaspadia@gmail.com", "");
                //}
                //foreach (var item in result)
                //{
                //    if (item.Type == "Attorney")
                //        continue;
                //    string subject = "Your Records Are Available";
                //    string Sendto = item.Email;

                //    System.Text.StringBuilder body = new System.Text.StringBuilder();
                //    using (System.IO.StreamReader reader = new System.IO.StreamReader(HttpContext.Current.Server.MapPath("~/MailTemplate/BillingRecords.html")))
                //    {
                //        body.Append(reader.ReadToEnd());
                //    }

                //    body = body.Replace("{UserName}", "Hello " + item.FirstName.Trim() + " " + item.LastName.Trim() + ",");
                //    body = body.Replace("{LOCATION}", LocationName + " (" + Location + ")");
                //    body = body.Replace("{PATIENT}", Patient);
                //    body = body.Replace("{CLAIMNO}", Convert.ToString(OrderDetail[0].BillingClaimNo));
                //    body = body.Replace("{ORDERNO}", OrderNumber + "-" + PartNumber);
                //    body = body.Replace("{InvHdr}", Convert.ToString(OrderDetail[0].InvHdr));
                //    body = body.Replace("{Pages}", Convert.ToString(OrderDetail[0].Pages));
                //    body = body.Replace("{LINK}", "o=" + OrderNumber + "&p=" + PartNumber);

                //    // EmailHelper.Email.Send(item.Email, body.ToString(), subject, "", "j.alspaugh@axiomcopy.com", "tejas.p@cementdigital.com");
                //    EmailHelper.Email.Send(item.Email, body.ToString(), subject, "tejaspadia@gmail.com", "");
                //}
                #endregion


                response.Success = true;
                response.Data = result;
            }
            catch (Exception ex)
            {

            }
            return response;
        }

        [HttpGet]
        [Route("GetBillToAttorneyByFirmId")]
        public ApiResponse<BillToAttorneyEntity> GetBillToAttorneyByFirmId(string FirmID)
        {

            var response = new ApiResponse<BillToAttorneyEntity>();

            try
            {
                SqlParameter[] param = { new SqlParameter("FirmId", (object)FirmID ?? (object)DBNull.Value)
                                        };
                var result = _repository.ExecuteSQL<BillToAttorneyEntity>("GetBillToAttorneyByFirmId", param).ToList();

                if (result == null)
                {
                    result = new List<BillToAttorneyEntity>();
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
        [Route("GetSoldToAttorneyByOrderNo")]
        public ApiResponse<BillToAttorneyEntity> GetSoldToAttorneyByOrderNo(string OrderNo, string PartNo)
        {
            var response = new ApiResponse<BillToAttorneyEntity>();

            try
            {
                SqlParameter[] param = { new SqlParameter("OrderNo", (object)OrderNo ?? (object)DBNull.Value)
                                        ,new SqlParameter("PartNo", (object)PartNo ?? (object)DBNull.Value)
                                        };
                var result = _repository.ExecuteSQL<BillToAttorneyEntity>("GetSoldToAttorneyByOrderNo", param).ToList();

                if (result == null)
                {
                    result = new List<BillToAttorneyEntity>();
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
        [Route("GetBillToAttorneyDetailsByOrderId")]
        public ApiResponse<BillToAttorneyDetailsEntity> GetBillToAttorneyDetailsByOrderId(string OrderNo, string PartNo)
        {

            var response = new ApiResponse<BillToAttorneyDetailsEntity>();

            try
            {
                SqlParameter[] param = { new SqlParameter("OrderId", (object)OrderNo ?? (object)DBNull.Value)
                                        ,new SqlParameter("PartNo", (object)PartNo ?? (object)DBNull.Value)
                                        };
                var result = _repository.ExecuteSQL<BillToAttorneyDetailsEntity>("GetBillToAttorneyDetailsByOrderId", param).ToList();

                if (result == null)
                {
                    result = new List<BillToAttorneyDetailsEntity>();
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
        [Route("GetSoldToAttorneyDetailsByOrderId")]
        public ApiResponse<SoldToAttorneyDetailsEntity> GetSoldToAttorneyDetailsByOrderId(string OrderNo, string PartNo)
        {

            var response = new ApiResponse<SoldToAttorneyDetailsEntity>();

            try
            {
                SqlParameter[] param = { new SqlParameter("OrderId", (object)OrderNo ?? (object)DBNull.Value)
                                        ,new SqlParameter("PartNo", (object)PartNo ?? (object)DBNull.Value)
                                        };
                var result = _repository.ExecuteSQL<SoldToAttorneyDetailsEntity>("GetSoldToAttorneyDetailsByOrderId", param).ToList();

                if (result == null)
                {
                    result = new List<SoldToAttorneyDetailsEntity>();
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

        //GetSoldToAttorneyDetailsByOrderId
        [HttpGet]
        [Route("GetAllInvoiceByOrderIdAndPartId")]
        public ApiResponse<BillInvoiceListEntity> GetAllInvoiceByOrderIdAndPartId(string OrderNo, string PartNo)
        {

            var response = new ApiResponse<BillInvoiceListEntity>();

            try
            {
                SqlParameter[] param = { new SqlParameter("OrderId", (object)OrderNo ?? (object)DBNull.Value)
                                        ,new SqlParameter("PartId", (object)PartNo ?? (object)DBNull.Value)
                };
                var result = _repository.ExecuteSQL<BillInvoiceListEntity>("GetAllInvoiceByOrderIdAndPartId", param).ToList();

                if (result == null)
                {
                    result = new List<BillInvoiceListEntity>();
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
        [Route("EditInvoice")]
        public ApiResponse<EditInvoiceEntity> EditInvoice(string InvoiceNumber)
        {

            var response = new ApiResponse<EditInvoiceEntity>();

            try
            {
                SqlParameter[] param = { new SqlParameter("InvoiceNo", (object)InvoiceNumber ?? (object)DBNull.Value)

                };
                var result = _repository.ExecuteSQL<EditInvoiceEntity>("EditInvoice", param).ToList();

                if (result == null)
                {
                    result = new List<EditInvoiceEntity>();
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
        [Route("GerRecordTypeListForBilling")]
        public ApiResponse<BillingRecordTypeListEntity> GerRecordTypeListForBilling()
        {

            var response = new ApiResponse<BillingRecordTypeListEntity>();

            try
            {
                var result = _repository.ExecuteSQL<BillingRecordTypeListEntity>("GerRecordTypeListForBilling").ToList();

                if (result == null)
                {
                    result = new List<BillingRecordTypeListEntity>();
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
        [Route("UpdateInvoice")]
        public ApiResponse<EditInvoiceEntity> UpdateInvoice(EditInvoiceEntity modal)
        {

            var response = new ApiResponse<EditInvoiceEntity>();

            try
            {
                SqlParameter[] param = {
                        new SqlParameter("InvoiceNumber", (object)modal.InvoiceNumber ?? (object)DBNull.Value)
                        ,new SqlParameter("OrigRate", (object)modal.OrigRate ?? (object)DBNull.Value)
                        ,new SqlParameter("CopyRate", (object)modal.CopyRate ?? (object)DBNull.Value)
                        ,new SqlParameter("Pages", (object)modal.Pages ?? (object)DBNull.Value)
                        ,new SqlParameter("StdFee1", (object)modal.StdFee1 ?? (object)DBNull.Value)
                        ,new SqlParameter("StdFee2", (object)modal.StdFee2 ?? (object)DBNull.Value)
                        ,new SqlParameter("StdFee4", (object)modal.StdFee4 ?? (object)DBNull.Value)
                        ,new SqlParameter("StdFee5", (object)modal.StdFee5 ?? (object)DBNull.Value)
                        ,new SqlParameter("StdFee6", (object)modal.StdFee6 ?? (object)DBNull.Value)
                        ,new SqlParameter("MiscCharge", (object)modal.MiscChrge ?? (object)DBNull.Value)
                        ,new SqlParameter("RcvdID", (object)modal.RcvdID ?? (object)DBNull.Value)
                        ,new SqlParameter("ItemNo", (object)modal.ItemNo ?? (object)DBNull.Value)
                        ,new SqlParameter("BillAtty", (object)modal.BillAtty ?? (object)DBNull.Value)
                };
                var result = _repository.ExecuteSQL<EditInvoiceEntity>("UpdateInvoice", param).ToList();

                if (result == null)
                {
                    result = new List<EditInvoiceEntity>();
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
        [Route("DeleteInvoice")]
        public BaseApiResponse DeleteInvoice(int invoiceNumber, int itemNumber)
        {
            var response = new BaseApiResponse();

            try
            {
                SqlParameter[] param = { new SqlParameter("InvoiceNumber", (object)invoiceNumber ?? (object)DBNull.Value)
                                        ,new SqlParameter("ItemNo", (object)itemNumber ?? (object)DBNull.Value)};

                var result = _repository.ExecuteSQL<int>("DeleteInvoice", param).FirstOrDefault();
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


        public string GetAttorneyEmailListByOrderId(Int64 OrderNo)
        {
            SqlParameter[] param = {
                        new SqlParameter("OrderNo", (object)OrderNo ?? (object)DBNull.Value)
                };
            var result = _repository.ExecuteSQL<string>("GetAttorneyEmailListByOrderId", param).FirstOrDefault();
            return result;
        }

        [HttpPost]
        [Route("GenerateInvoiceMultiple")]
        public ApiResponse<GenerateInvoiceEntity> GenerateInvoiceMultiple(List<GenerateInvoiceMultiple> OrderPartList)
        {
            var response = new ApiResponse<GenerateInvoiceEntity>();

            foreach (GenerateInvoiceMultiple item in OrderPartList)
            {
                BillingApiController bc = new BillingApiController();
                int OrderID = Convert.ToInt32(item.OrderId);
                int PartNo = Convert.ToInt32(item.PartNo);
                ApiResponse<BillToAttorneyDetailsEntity> objBillToDetails = bc.GetBillToAttorneyDetailsByOrderId(OrderID.ToString(), PartNo.ToString());
                ApiResponse<SoldToAttorneyDetailsEntity> objSoldToDetails = bc.GetSoldToAttorneyDetailsByOrderId(OrderID.ToString(), PartNo.ToString());

                ApiResponse<BillToAttorneyEntity> objBillToAttorney = new ApiResponse<BillToAttorneyEntity>();
                ApiResponse<BillToAttorneyEntity> objSoldtoAttorney = new ApiResponse<BillToAttorneyEntity>();

                if (objSoldToDetails.Data.Count > 0)
                {
                    objSoldtoAttorney = bc.GetSoldToAttorneyByOrderNo(OrderID.ToString(), PartNo.ToString());
                }

                if (objBillToDetails.Data.Count > 0)
                {
                    objBillToAttorney = bc.GetBillToAttorneyByFirmId(objBillToDetails.Data[0].BillingFirmID);
                }
                string strBilltoAttorney = objBillToAttorney.Data.Count > 0 ? objBillToAttorney.Data[0].AttyId : "";

                List<SoldAttorneyEntity> soldAttorneyList = new List<SoldAttorneyEntity>();


                foreach (var itemAttorney in objSoldtoAttorney.Data)
                {
                    soldAttorneyList.Add(new SoldAttorneyEntity { AttyId = itemAttorney.AttyId, AttyType = "Ordering" });
                }

                bc.GenerateInvoice(OrderID, PartNo, strBilltoAttorney, soldAttorneyList);
            }

            return response;
        }


        [HttpPost]
        [Route("GenerateInvoice")]
        public ApiResponse<GenerateInvoiceEntity> GenerateInvoice(Int64 OrderNo, int PartNo, string BillToAttorney, List<SoldAttorneyEntity> SoldAtty)
        {

            var response = new ApiResponse<GenerateInvoiceEntity>();

            string xmlData = ConvertToXml<SoldAttorneyEntity>.GetXMLString(SoldAtty);

            try
            {
                SqlParameter[] param = {
                        new SqlParameter("OrderNo", (object)OrderNo ?? (object)DBNull.Value)
                        ,new SqlParameter("PartNo", (object)PartNo ?? (object)DBNull.Value)
                        ,new SqlParameter("BillAttorney", (object)BillToAttorney ?? (object)DBNull.Value)
                        ,new SqlParameter("SoldAttorneyAndAttorneyType", (object)xmlData ?? (object)DBNull.Value)
                };
                var result = _repository.ExecuteSQL<GenerateInvoiceEntity>("GenerateInvoice", param).ToList();

                if (result.Count > 0)
                {
                    foreach (GenerateInvoiceEntity item in result)
                    {
                        string subject = "Billing Proposal";
                        string body = string.Empty;
                        using (System.IO.StreamReader reader = new System.IO.StreamReader(HttpContext.Current.Server.MapPath("~/MailTemplate/WaiverBillingProposal.html")))
                        {
                            body = reader.ReadToEnd();
                        }
                        if (!string.IsNullOrEmpty(item.FirstName))
                        {
                            body = body.Replace("{Attorney}", Convert.ToString(item.FirstName) + " " + Convert.ToString(item.LastName).Trim());
                            body = body.Replace("{Firm}", Convert.ToString(item.FirmName).Trim());
                        }
                        body = body.Replace("{caption}", Convert.ToString(item.Caption).Trim());
                        body = body.Replace("{RECORDSOF}", Convert.ToString(item.Patient).Trim());

                        body = body.Replace("{UserName}", Convert.ToString(item.WaiverAttyFirstName).Trim() + " " + Convert.ToString(item.WaiverAttyLastName).Trim());
                        body = body.Replace("{ORDERNO}", OrderNo.ToString());

                        body = body.Replace("{LOCATION}", Convert.ToString(item.LocationName).Replace(',', ' ') + " (" + Convert.ToString(item.LocID) + ")");
                        //  body = body.Replace("{RecordType}", location.Descr.Trim());
                        body = body.Replace("{RecordType}", Convert.ToString(item.Descr));//INVHdr OF Item TABLE
                        body = body.Replace("{PAGES}", Convert.ToString(item.Pages));
                        body = body.Replace("{COST}", Convert.ToString(item.TotalAmountForPatientAtty.ToString("F")));


                        string accExecutiveName = "Admin";
                        string accExecutiveEmail = "nrrpf@axiomcopy.com";

                        string AttorneyNm = Convert.ToString(item.WaiverAttyFirstName).Trim() + " " + Convert.ToString(item.WaiverAttyLastName).Trim() + " (" + Convert.ToString(item.Email).Trim() + ")";
                        //Guid value = new Guid(Membership.GetUser().ProviderUserKey.ToString());

                        //Guid value = new Guid("3668BE61-8089-42AC-AC30-8334BE2C7410");
                        //retval.CreateErrorResponse(retval.message + value.ToString() + Membership.GetUser().ProviderUserKey.ToString());

                        //GetClientAcctExecResult accExecutive = mr.GetClientAcctExec(value).FirstOrDefault<GetClientAcctExecResult>();
                        //if (accExecutive != null)
                        //{
                        //    accExecutiveName = Convert.ToString(accExecutive.Name);
                        //    accExecutiveEmail = Convert.ToString(accExecutive.Email);
                        //}
                        //else
                        //{
                        //    accExecutiveName = "Leah Boroski";
                        //    accExecutiveEmail = "leah.boroski@axiomcopy.com";
                        //}

                        string orderNo = OrderNo.ToString();
                        string strPages = item.Pages.ToString();
                        string strAmount = item.TotalAmountForPatientAtty.ToString();
                        string WaiverID = string.Empty;


                        string strApproveLink, strNotApprovedLink, strEditScopeLink, strQueryString;
                        strQueryString = accExecutiveEmail + "," + accExecutiveName + "," + orderNo + "," + item.LocationName.Replace(',', ' ') + " (" + item.LocID + ")" + "," + strPages + "," + strAmount + "," + PartNo.ToString() + "," + AttorneyNm;
                        strApproveLink = HttpUtility.UrlEncode(EncryptDecrypt.Encrypt(strQueryString));
                        strNotApprovedLink = HttpUtility.UrlEncode(EncryptDecrypt.Encrypt(strQueryString));
                        strEditScopeLink = HttpUtility.UrlEncode(EncryptDecrypt.Encrypt(strQueryString));


                        // FOR LOCAL MACHINE
                        //body = body.Replace("{YESLINK}", "http://localhost:51617/WaiverBilling.aspx?type=" + HttpUtility.UrlEncode(EncryptDecrypt.Encrypt("Y")) + "&value=" + strApproveLink);
                        //body = body.Replace("{NOLINK}", "http://localhost:51617/WaiverBilling.aspx?type=" + HttpUtility.UrlEncode(EncryptDecrypt.Encrypt("N")) + "&value=" + strNotApprovedLink);


                        // FOR DEMO SITE
                        //body = body.Replace("{YESLINK}", "http://192.168.0.22:8080/Clients/WaiverBilling.aspx?type=" + HttpUtility.UrlEncode(EncryptDecrypt.Encrypt("Y")) + "&value=" + strApproveLink);
                        //body = body.Replace("{NOLINK}", "http://192.168.0.22:8080/Clients/WaiverBilling.aspx?type=" + HttpUtility.UrlEncode(EncryptDecrypt.Encrypt("N")) + "&value=" + strNotApprovedLink);


                        // FOR LIVE SITE
                        body = body.Replace("{YESLINK}", "https://www.axiomcopyonline.com/Clients/WaiverBilling.aspx?type=" + HttpUtility.UrlEncode(EncryptDecrypt.Encrypt("Y")) + "&value=" + strApproveLink);
                        body = body.Replace("{NOLINK}", "https://www.axiomcopyonline.com/Clients/WaiverBilling.aspx?type=" + HttpUtility.UrlEncode(EncryptDecrypt.Encrypt("N")) + "&value=" + strNotApprovedLink);

                        //Add Attorney Email
                        var OrderFirmAttrorneyEmailIds = GetAttorneyEmailListByOrderId(OrderNo);
                        if (!String.IsNullOrEmpty(OrderFirmAttrorneyEmailIds))
                        {
                            item.Email = item.Email + "," + OrderFirmAttrorneyEmailIds;
                        }
                        item.Email = item.Email.Trim(',');

                        // EmailHelper.Email.Send(item.Email, body, subject, "j.alspaugh@axiomcopy.com", "tejas.p@cementdigital.com");

                        //AxiomEmail.SendMailBilling(subject, body, "sejal.k@shaligraminfotech.com", true, "AxiomSupport@axiomcopy.com", null, "");
                        // AxiomEmail.SendMailBilling(subject, body, waiver.Email.ToString(), true, "AxiomSupport@axiomcopy.com", null, "tejaspadia@gmail.com,autoemail@axiomcopy.com");

                    }
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
    }
    public class EncryptDecrypt
    {
        //Private strKey As [String] = "1234-13241324-1234-123"
        public static String strKey = "U2A9/R*41FD412+4-123";

        /// <summary>
        /// To encrypt the string data 
        /// </summary>
        /// <param name="strData"> string which user want to encrypt </param>
        /// <returns> Encrypted String </returns>
        /// <remarks></remarks>
        public static string Encrypt(string strData)
        {
            string strValue = "";
            if (!string.IsNullOrEmpty(strKey))
            {
                if (strKey.Length < 16)
                {
                    char c = "XXXXXXXXXXXXXXXX"[16];
                    strKey = strKey + strKey.Substring(0, 16 - strKey.Length);
                }

                if (strKey.Length > 16)
                {
                    strKey = strKey.Substring(0, 16);
                }

                // create encryption keys
                byte[] byteKey = Encoding.UTF8.GetBytes(strKey.Substring(0, 8));
                byte[] byteVector = Encoding.UTF8.GetBytes(strKey.Substring(strKey.Length - 8, 8));

                // convert data to byte array
                byte[] byteData = Encoding.UTF8.GetBytes(strData);

                // encrypt 
                DESCryptoServiceProvider objDES = new DESCryptoServiceProvider();
                MemoryStream objMemoryStream = new MemoryStream();
                CryptoStream objCryptoStream = new CryptoStream(objMemoryStream, objDES.CreateEncryptor(byteKey, byteVector), CryptoStreamMode.Write);
                objCryptoStream.Write(byteData, 0, byteData.Length);
                objCryptoStream.FlushFinalBlock();

                // convert to string and Base64 encode
                strValue = Convert.ToBase64String(objMemoryStream.ToArray());
            }
            else
            {
                strValue = strData;
            }

            return strValue;
        }

        /// <summary>
        /// To Decrypt the Data
        /// </summary>
        /// <param name="strData"> Encrypted string which user want to Decrypt. </param>
        /// <returns> Original string </returns>
        /// <remarks></remarks>
        public static string Decrypt(string strData)
        {
            string strValue = "";
            if (!string.IsNullOrEmpty(strKey))
            {
                // convert key to 16 characters for simplicity
                if (strKey.Length < 16)
                {
                    strKey = strKey + strKey.Substring(0, 16 - strKey.Length);

                }

                if (strKey.Length > 16)
                {
                    strKey = strKey.Substring(0, 16);

                }

                // create encryption keys
                byte[] byteKey = Encoding.UTF8.GetBytes(strKey.Substring(0, 8));
                byte[] byteVector = Encoding.UTF8.GetBytes(strKey.Substring(strKey.Length - 8, 8));

                if (!string.IsNullOrEmpty(strData))
                {
                    // convert data to byte array and Base64 decode
                    byte[] byteData = new byte[strData.Length + 1];
                    try
                    {
                        byteData = Convert.FromBase64String(strData);
                    }
                    catch
                    {
                        strValue = strData;
                    }

                    // decrypt
                    DESCryptoServiceProvider objDES = new DESCryptoServiceProvider();
                    MemoryStream objMemoryStream = new MemoryStream();
                    CryptoStream objCryptoStream = new CryptoStream(objMemoryStream, objDES.CreateDecryptor(byteKey, byteVector), CryptoStreamMode.Write);
                    objCryptoStream.Write(byteData, 0, byteData.Length);
                    objCryptoStream.FlushFinalBlock();

                    // convert to string
                    System.Text.Encoding objEncoding = System.Text.Encoding.UTF8;
                    strValue = objEncoding.GetString(objMemoryStream.ToArray());
                }
            }
            else
            {
                strValue = strData;
            }

            return strValue;
        }
    }
}