using EmailReminderService.Entity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Timers;
using System.Web;
using System.Web.Script.Serialization;

namespace EmailReminderService
{
    public partial class EmailReminderService : ServiceBase
    {
        Timer emialReminderServiceTimer = null;
        Timer axoimServiceTimer = null;
        public EmailReminderService()
        {
            Log.ServicLog("======================================================================");
            Log.ServicLog("Initialize");
            Log.ServicLog("Email Reminder Duration Time :  " + ConfigurationManager.AppSettings["EmailReminderDuration"].ToString() + " min");
            Log.ServicLog("Axiom Duration Time :  " + ConfigurationManager.AppSettings["AxiomDuration"].ToString() + " min");


            emialReminderServiceTimer = new Timer();
            emialReminderServiceTimer.Elapsed += EmialReminderServiceTimer_Elapsed;
            emialReminderServiceTimer.Interval = Convert.ToDouble(ConfigurationManager.AppSettings["EmailReminderDuration"].ToString()) * 60 * 1000;
            emialReminderServiceTimer.Enabled = true;
            emialReminderServiceTimer.Start();

            axoimServiceTimer = new Timer();
            axoimServiceTimer.Elapsed += AxoimServiceTimer_Elapsed;
            axoimServiceTimer.Interval = Convert.ToDouble(ConfigurationManager.AppSettings["AxiomDuration"].ToString()) * 60 * 1000;
            axoimServiceTimer.Enabled = true;
            axoimServiceTimer.Start();

            InitializeComponent();
        }

        private void EmialReminderServiceTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Log.ServicLog(DateTime.Now.ToString() + " Email Reminder TIMER ELAPSED");
            ServiceExecution(); //Email Reminder Service
        }

        private void AxoimServiceTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Log.ServicLog(DateTime.Now.ToString() + " Axiom Service TIMER ELAPSED");
            SendMonthlyEmail();
            AxiomServiceExecution();
        }


        //================================Email Reminder Service==================================
        public void ServiceExecution_BackUp()
     {
            Log.ServicLog("------------------- EMAIL REMINDER SERVICE EXECUTED " + DateTime.Now + "---------------------------");
            try
            {
                var dataList = DbAccess.GetProposalFees();
                if (dataList == null)
                    return;
                string proposalBaseFeeURL = ConfigurationManager.AppSettings["ProposalBaseFeeURL"].ToString();
                foreach (var item in dataList)
                {
                    var patientName = DbAccess.GetOrderPatient(item.OrderNo);
                    var billAtty = DbAccess.GetBillAttorney(item.OrderNo);
                    string strBillAtty = string.Empty;
                    if (billAtty != null)
                    {
                        if (!string.IsNullOrEmpty(billAtty.BillingAttorneyId.Trim()))
                            strBillAtty = billAtty.BillingAttorneyId.Trim();
                        else
                            strBillAtty = billAtty.OrderingAttorney.Trim();
                    }

                    var attorney = DbAccess.GetBillAttorney(strBillAtty);

                    //Get Fee Approval Attrorney Email
                    string FeeApprovalEmails = DbAccess.GetFeeApprovalAttorneyEmail(item.OrderNo);
                    if (!String.IsNullOrEmpty(FeeApprovalEmails))
                    {
                        if (attorney != null)
                        {
                            if (!String.IsNullOrEmpty(attorney.Email))
                            {
                                attorney.Email = attorney.Email + "," + FeeApprovalEmails;
                            }
                            else
                            {
                                attorney.Email = FeeApprovalEmails;
                            }
                        }
                    }
                    //----------------------------------

                    var location = DbAccess.GetPartLocation(item.OrderNo, item.PartNo);
                    string subject = "Proposal Fees Reminder Request";
                    string body = string.Empty;
                    string htmlfilePath = AppDomain.CurrentDomain.BaseDirectory + "HtmlTemplates\\ProposalFees.html";
                    using (StreamReader reader = new StreamReader((htmlfilePath)))
                    {
                        body = reader.ReadToEnd();
                    }
                    string userFirstName = "", userLastName = "";
                    if (attorney != null)
                    {
                        if (!string.IsNullOrEmpty(attorney.FirstName))
                        {
                            userFirstName = Convert.ToString(attorney.FirstName).Trim();
                        }
                        if (!string.IsNullOrEmpty(attorney.LastName))
                        {
                            userLastName = Convert.ToString(attorney.LastName).Trim();
                        }
                    }

                    string strLoc = "";
                    if (location != null)
                    {
                        if (!string.IsNullOrEmpty(location.Name1))
                        {
                            strLoc += Convert.ToString(location.Name1).Trim().Replace(',', ' ');
                        }
                        if (!string.IsNullOrEmpty(location.LocID))
                        {
                            strLoc = strLoc + " (" + location.LocID + ")";
                        }
                    }

                    body = body.Replace("{UserName}", userFirstName + " " + userLastName);
                    body = body.Replace("{ORDERNO}", item.OrderNo.ToString());
                    body = body.Replace("{RECORDSOF}", patientName != null ? patientName.Name : "");
                    body = body.Replace("{LOCATION}", strLoc);
                    body = body.Replace("{PAGES}", Convert.ToString(item.Pages));
                    body = body.Replace("{COST}", Convert.ToString(item.Amount));

                    string accExecutiveName = "";
                    string accExecutiveEmail = "";
                    var accExecutive = DbAccess.GetClientAcctExecService(strBillAtty);

                    if (accExecutive != null)
                    {
                        accExecutiveName = Convert.ToString(accExecutive.Name);
                        accExecutiveEmail = Convert.ToString(accExecutive.Email);
                    }
                    else
                    {
                        accExecutiveName = "Leah Boroski";
                        accExecutiveEmail = "leah.boroski@axiomcopy.com";
                    }
                    string locName1 = "";
                    string locId = "";
                    if (location != null)
                    {
                        locName1 = location.Name1;
                        locId = location.LocID;
                    }
                    string strQueryString = accExecutiveEmail + "," + accExecutiveName + "," + item.OrderNo + "," +
                        HttpUtility.HtmlEncode(Convert.ToString(locName1).Replace(',', ' ')) +
                        " (" + HttpUtility.HtmlEncode(locId) + ")" + "," + item.Pages +
                        "," + item.Amount + "," + Convert.ToString(item.ProposalFeeID) + "," +
                       Convert.ToString(item.PartNo);

                    string strApproveLink     = HttpUtility.UrlEncode(EncryptDecrypt.Encrypt(strQueryString));
                    string strNotApprovedLink = HttpUtility.UrlEncode(EncryptDecrypt.Encrypt(strQueryString));
                    string strEditScopeLink   = HttpUtility.UrlEncode(EncryptDecrypt.Encrypt(strQueryString));

                    //body = body.Replace("{APPROVELINK}", "https://www.axiomcopyonline.com/Clients/ProposalFeeApproval.aspx?type=" + HttpUtility.UrlEncode(EncryptDecrypt.Encrypt("A")) + "&value=" + strApproveLink);
                    //body = body.Replace("{NOTAPPROVELINK}", "https://www.axiomcopyonline.com/Clients/ProposalFeeApproval.aspx?type=" + HttpUtility.UrlEncode(EncryptDecrypt.Encrypt("N")) + "&value=" + strNotApprovedLink);
                    //body = body.Replace("{EDITSCOPELINK}", "https://www.axiomcopyonline.com/Clients/ProposalFeeApproval.aspx?type=" + HttpUtility.UrlEncode(EncryptDecrypt.Encrypt("E")) + "&value=" + strEditScopeLink);


                    body = body.Replace("{APPROVELINK}", proposalBaseFeeURL + "/ProposalFeeApproval?type=" + HttpUtility.UrlEncode(EncryptDecrypt.Encrypt("A")) + "&value=" + strApproveLink);
                    body = body.Replace("{NOTAPPROVELINK}", proposalBaseFeeURL + "/ProposalFeeApproval?type=" + HttpUtility.UrlEncode(EncryptDecrypt.Encrypt("N")) + "&value=" + strNotApprovedLink);
                    body = body.Replace("{EDITSCOPELINK}", proposalBaseFeeURL + "/ProposalFeeApproval?type=" + HttpUtility.UrlEncode(EncryptDecrypt.Encrypt("E")) + "&value=" + strEditScopeLink);

                    if (attorney != null && !string.IsNullOrEmpty(attorney.Email))  //&& Utility.isValidEmail(attorney.Email)
                    {
                         Utility.SendMailBilling(subject, body, attorney.Email, true, "AxiomSupport@axiomcopy.com", null, "autharchive@axiomcopy.com", "tejaspadia@gmail.com,autoemail@axiomcopy.com");
                    } 
                     DbAccess.UpdateProposalData(item.OrderNo, item.PartNo, item.ProposalFeeID);
                    Log.ServicLog("----------------------------------------");
                    Log.ServicLog("Email sent : " + System.DateTime.Now);
                    Log.ServicLog("Order No " + item.OrderNo.ToString());
                }


            }
            catch (Exception ex)
            {
                Log.ServicLog("----------- EXCEPTION -------------------");
                Log.ServicLog(Convert.ToString(ex.Message));
                Log.ServicLog(Convert.ToString(ex.StackTrace));
                Log.ServicLog(Convert.ToString(ex.InnerException));
                Log.ServicLog("--------------------------------------------");
            }
            finally
            {
                Log.ServicLog("------------------- EMAIL REMINDER SERVICE EXECUTION FINISHED " + DateTime.Now + "---------------------------");
            } 
        }

        public void ServiceExecution()
        {
            Log.ServicLog("------------------- EMAIL REMINDER SERVICE EXECUTED " + DateTime.Now + "---------------------------");
            try
            {
                var dataList = DbAccess.GetProposalFees();
                if (dataList == null)
                    return;
                string proposalBaseFeeURL = ConfigurationManager.AppSettings["ProposalBaseFeeURL"].ToString();
                foreach (var item in dataList)
                {
                    
                    string strBillAtty = string.Empty;
                    string attorneyEmail = string.Empty;
                    string attorneyFirstName = string.Empty;
                    string attorneyLastName = string.Empty;
                    //dynamic attorney = null;
                    if (item.HasBillingAttorney==true)
                    {
                        strBillAtty = item.BillingAttorneyId;
                        attorneyEmail = item.BillingAttorneyEmail;
                        attorneyFirstName = item.BillingAttorneyFirstName;
                        attorneyLastName = item.BillingAttorneyLastName;
                    }
                    else
                    {
                        strBillAtty = item.OrderingAttorney;
                        attorneyEmail = item.OrderingAttorneyEmail;
                        attorneyFirstName = item.OrderingAttorneyFirstName;
                        attorneyLastName = item.OrderingAttorneyLastName;
                    }
                    //----------------------------------

                    //var location = DbAccess.GetPartLocation(item.OrderNo, item.PartNo);
                    string subject = "Proposal Fees Reminder Request";
                    string body = string.Empty;
                    string htmlfilePath = AppDomain.CurrentDomain.BaseDirectory + "HtmlTemplates\\ProposalFees.html";
                    using (StreamReader reader = new StreamReader((htmlfilePath)))
                    {
                        body = reader.ReadToEnd();
                    }
                    string userFirstName = attorneyFirstName, userLastName = attorneyLastName;
                    //if (attorney != null)
                    //{
                    //    if (!string.IsNullOrEmpty(attorney.FirstName))
                    //    {
                    //        userFirstName = Convert.ToString(attorney.FirstName).Trim();
                    //    }
                    //    if (!string.IsNullOrEmpty(attorney.LastName))
                    //    {
                    //        userLastName = Convert.ToString(attorney.LastName).Trim();
                    //    }
                    //}

                    string strLoc = "";
                    if (item != null)
                    {
                        if (!string.IsNullOrEmpty(item.locationName1))
                        {
                            strLoc += Convert.ToString(item.locationName1).Trim().Replace(',', ' ');
                        }
                        if (!string.IsNullOrEmpty(item.LocID))
                        {
                            strLoc = strLoc + " (" + item.LocID + ")";
                        }
                    }

                    body = body.Replace("{UserName}", userFirstName + " " + userLastName);
                    body = body.Replace("{ORDERNO}", item.OrderNo.ToString());
                    //body = body.Replace("{RECORDSOF}", patientName != null ? patientName.Name : "");
                    body = body.Replace("{RECORDSOF}", item != null ? item.OrderPatientName : "");
                    body = body.Replace("{LOCATION}", strLoc);
                    body = body.Replace("{PAGES}", Convert.ToString(item.Pages));
                    body = body.Replace("{COST}", Convert.ToString(item.Amount));

                    string accExecutiveName = item.ClientAttorneyName;
                    string accExecutiveEmail = item.ClientAttorneyEmail;
                     
                    string locName1 = "";
                    string locId = "";
                    if (item != null)
                    {
                        locName1 = item.locationName1;
                        locId = item.LocID;
                    }
                    string strQueryString = accExecutiveEmail + "," + accExecutiveName + "," + item.OrderNo + "," 
                        + HttpUtility.HtmlEncode(Convert.ToString(locName1).Replace(',', ' ')) 
                        + " (" + HttpUtility.HtmlEncode(locId) + ")" + "," 
                        + item.Pages 
                        + "," + item.Amount + "," + Convert.ToString(item.ProposalFeeID) 
                        + "," + Convert.ToString(item.PartNo);

                    strQueryString = HttpUtility.UrlEncode(EncryptDecrypt.Encrypt(strQueryString));

                    body = body.Replace("{APPROVELINK}", proposalBaseFeeURL + "/ProposalFeeApproval?type=" + HttpUtility.UrlEncode(EncryptDecrypt.Encrypt("A")) + "&value=" + strQueryString);
                    body = body.Replace("{NOTAPPROVELINK}", proposalBaseFeeURL + "/ProposalFeeApproval?type=" + HttpUtility.UrlEncode(EncryptDecrypt.Encrypt("N")) + "&value=" + strQueryString);
                    body = body.Replace("{EDITSCOPELINK}", proposalBaseFeeURL + "/ProposalFeeApproval?type=" + HttpUtility.UrlEncode(EncryptDecrypt.Encrypt("E")) + "&value=" + strQueryString);
                    var emailList = new List<string>();
                    if (!string.IsNullOrEmpty(attorneyEmail))
                    {
                        emailList.Add(attorneyEmail);
                    }
                    if (!string.IsNullOrEmpty(item.AssistantEmail))
                    {
                        emailList.Add(item.AssistantEmail);
                    }
                    attorneyEmail = string.Join(",", emailList);
                    if (!string.IsNullOrEmpty(attorneyEmail))  //&& Utility.isValidEmail(attorney.Email)
                    {
                        
                        Utility.SendMailBilling(subject, body, attorneyEmail, true, "AxiomSupport@axiomcopy.com", null, "autharchive@axiomcopy.com", "tejaspadia@gmail.com,autoemail@axiomcopy.com");
                    }

                    DbAccess.UpdateProposalData(item.OrderNo, item.PartNo, item.ProposalFeeID);
               
                     
                    Log.ServicLog("----------------------------------------");
                    Log.ServicLog("Email sent : " + System.DateTime.Now);
                    Log.ServicLog("Order No " + item.OrderNo.ToString());
                } 
            }
            catch (Exception ex)
            {
                Log.ServicLog("----------- EXCEPTION -------------------");
                Log.ServicLog(Convert.ToString(ex.Message));
                Log.ServicLog(Convert.ToString(ex.StackTrace));
                Log.ServicLog(Convert.ToString(ex.InnerException));
                Log.ServicLog("--------------------------------------------");
            }
            finally
            {
                Log.ServicLog("------------------- EMAIL REMINDER SERVICE EXECUTION FINISHED " + DateTime.Now + "---------------------------");
            } 
        }

        //==================================Axiom Service==========================================
        public void SendMonthlyEmail()
        {
            Log.ServicLog("------------------- AXIOM SERVICE EXECUTED " + DateTime.Now + "---------------------------");

            try
            {
                List<int> lstOrder = new List<int>();
                var mailSentList = DbAccess.GetAuthorizationToBeCalledOnEmail(0, 1);
                if (mailSentList != null)
                {
                    foreach (var item in mailSentList)
                    {
                        int orderNo = Convert.ToInt32(item.OrderNo);
                        if (!lstOrder.Contains(orderNo))
                        {
                            var patientName = DbAccess.GetOrderPatient(orderNo);

                            if (patientName == null)
                                continue;
                            var emailDetail = DbAccess.GetBillAttorney(patientName.OrderingAttorney);
                            if (emailDetail == null)
                                continue;
                            var lstPartNo = new List<PartDetail>();
                            foreach (var dataItem in mailSentList)
                            {
                                if (dataItem.OrderNo == orderNo)
                                {
                                    lstPartNo.Add(new PartDetail
                                    {
                                        PartNo = dataItem.PartNo
                                    });
                                }
                            }

                            string body = string.Empty;
                            string htmlfilePath = AppDomain.CurrentDomain.BaseDirectory + "HtmlTemplates\\AuthorizationEmailMonthly.html";
                            using (StreamReader reader = new StreamReader((htmlfilePath)))
                            {
                                body = reader.ReadToEnd();
                            }
                            string userfName = "", userlName = "";
                            if (!string.IsNullOrEmpty(emailDetail.FirstName))
                            {
                                userfName = emailDetail.FirstName.Trim();
                            }
                            if (!string.IsNullOrEmpty(emailDetail.LastName))
                            {
                                userlName = emailDetail.LastName.Trim();
                            }
                            body = body.Replace("{UserName}", userfName + " " + userlName);
                            body = body.Replace("{ORDERNO}", Convert.ToString(item.OrderNo));
                            body = body.Replace("{PATIENTNAME}", Convert.ToString(patientName.Name));
                            StringBuilder sb = new StringBuilder();
                            if (lstPartNo.Count > 0)
                            {
                                foreach (var part in lstPartNo)
                                {
                                    int pno = Convert.ToInt32(part.PartNo);
                                    var lc = DbAccess.GetPartLocation(orderNo, pno);
                                    if (lc != null)
                                    {
                                        sb.Append("<tr><td width='50'  style='text-align:center;'>" + pno.ToString() + "</td>");
                                        sb.Append("<td>" + lc.Name1 + "  (" + lc.LocID + ") </td></tr>");
                                    }
                                    else
                                    {
                                        sb.Append("<tr><td width='50'  style='text-align:center;'>" + pno.ToString() + "</td><td></td></tr>");
                                    }

                                }
                            }
                            if (string.IsNullOrEmpty(emailDetail.Email.Trim()) || Utility.isValidEmail(emailDetail.Email) == false)
                            {
                                Log.ServicLog("-------------ORDER ATTORNEY EMAIL IS NOT VALID OR EMPTY FROM MONTHLY---------------");
                                Log.ServicLog("Order No : " + item.OrderNo.ToString());
                                Log.ServicLog("Attorney Name : " + emailDetail.FirstName.Trim() + " " + emailDetail.LastName.Trim() + " (" + patientName.OrderingAttorney.Trim() + ")");
                                Log.ServicLog("Email : " + emailDetail.Email.Trim());
                                Log.ServicLog("------------------------------------------------------------------------");
                            }
                            else
                            {
                                string additionalEmail = string.Empty;
                                var AdditionalContacts = DbAccess.GetNotificationEmails(orderNo, patientName.OrderingAttorney);
                                if (AdditionalContacts != null)
                                {
                                    foreach (var itemContact in AdditionalContacts)
                                    {
                                        additionalEmail += itemContact.AssistantEmail + ",";
                                    }
                                }
                                additionalEmail = additionalEmail.Trim(',');
                                body = body.Replace("{LOCATION}", sb.ToString());

                                if (string.IsNullOrEmpty(additionalEmail))
                                {
                                    Utility.SendMailBilling("Monthly Reminder - Authorization To Be Called On", body, emailDetail.Email, true, "AxiomSupport@axiomcopy.com", null, "autharchive@axiomcopy.com,tejas.padia@gmail.com,autoemail@axiomcopy.com");
                                }
                                else
                                {
                                    Utility.SendMailBilling("Monthly Reminder - Authorization To Be Called On", body, additionalEmail, true, "AxiomSupport@axiomcopy.com", null, "autharchive@axiomcopy.com,tejas.padia@gmail.com,autoemail@axiomcopy.com");
                                }
                                foreach (var part in lstPartNo)
                                {
                                    DbAccess.UpdateAuthorizationToBeCalledOnEmail(orderNo, part.PartNo, 1);
                                }
                                lstOrder.Add(Convert.ToInt32(item.OrderNo));
                                Log.ServicLog("-------- MONTHLY EMAIL ----------------------");
                                Log.ServicLog("Email sent to " + emailDetail.Email + " : " + System.DateTime.Now);
                                Log.ServicLog("Order No " + item.OrderNo.ToString());
                                Log.ServicLog("Attorney Name : " + emailDetail.FirstName.Trim() + " " + emailDetail.LastName.Trim() + " (" + patientName.OrderingAttorney.Trim() + ")");
                            }

                            Log.ServicLog('\n' + "-------------- AXIOM SERVICE EXECUTION FINISHED --" + System.DateTime.Now + "-----------------------------");


                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Log.ServicLog("----------- EXCEPTION -------------------");
                Log.ServicLog(Convert.ToString(ex.Message));
                Log.ServicLog(Convert.ToString(ex.StackTrace));
                Log.ServicLog(Convert.ToString(ex.InnerException));
                Log.ServicLog("--------------------------------------------");
            }
        }


        public void AxiomServiceExecution()
        {

            try
            {
                List<int> lstOrder = new List<int>();
                Log.ServicLog('\n' + "-----------------------------" + System.DateTime.Now + "-------------------------------------");

                var followUpDay = ConfigurationManager.AppSettings["FollowUPDays"].ToString();
                var mailSent = DbAccess.GetAuthorizationToBeCalledOnEmail(Convert.ToInt32(followUpDay), 0);
                if (mailSent != null)
                {
                    foreach (var item in mailSent)
                    {
                        int orderNo = Convert.ToInt32(item.OrderNo);
                        if (!lstOrder.Contains(orderNo))
                        {
                            var patientName = DbAccess.GetOrderPatient(orderNo);
                            if (patientName == null)
                                continue;

                            var emailDetail = DbAccess.GetAttornyEmailidForAuthorization(orderNo);
                            if (emailDetail == null)
                            {
                                string strbody = "Could not found records for order number " + item.OrderNo + " from OppAtty table.";
                                Utility.SendMailBilling("Email Not Sent - Order No : " + item.OrderNo, strbody, "autharchive@axiomcopy.com", true, "AxiomSupport@axiomcopy.com", null, "tejaspadia@gmail.com,autoemail@axiomcopy.com");
                                continue;
                            }
                            var lstPartNo = new List<PartDetail>();
                            foreach (var dataItem in mailSent)
                            {
                                if (dataItem.OrderNo == orderNo)
                                {
                                    lstPartNo.Add(new PartDetail
                                    {
                                        PartNo = dataItem.PartNo
                                    });
                                }
                            }

                            string body = string.Empty;
                            string htmlfilePath = AppDomain.CurrentDomain.BaseDirectory + "HtmlTemplates\\AuthorizationEmail.html";
                            using (StreamReader reader = new StreamReader((htmlfilePath)))
                            {
                                body = reader.ReadToEnd();
                            }
                            body = body.Replace("{UserName}", emailDetail.FirstName.Trim() + " " + emailDetail.LastName.Trim());
                            body = body.Replace("{ORDERNO}", Convert.ToString(orderNo));
                            body = body.Replace("{PATIENTNAME}", Convert.ToString(patientName.Name));
                            StringBuilder sb = new StringBuilder();

                            if (lstPartNo.Count > 0)
                            {
                                foreach (var part in lstPartNo)
                                {
                                    int pno = Convert.ToInt32(part.PartNo);
                                    var lc = DbAccess.GetPartLocation(orderNo, pno);
                                    if (lc != null)
                                    {
                                        sb.Append("<tr><td width='50'  style='text-align:center;'>" + pno.ToString() + "</td>");
                                        sb.Append("<td>" + lc.Name1 + "  (" + lc.LocID + ") </td></tr>");
                                    }
                                    else
                                    {
                                        sb.Append("<tr><td width='50'  style='text-align:center;'>" + pno.ToString() + "</td><td></td></tr>");
                                    }
                                }
                            }

                            if (string.IsNullOrEmpty(emailDetail.Email.Trim()) || Utility.isValidEmail(emailDetail.Email) == false)
                            {
                                Log.ServicLog("-------------PATIENT ATTORNEY EMAIL IS NOT VALID OR EMPTY---------------");
                                Log.ServicLog("Order No : " + orderNo.ToString());
                                Log.ServicLog("Attorney Name : " + emailDetail.FirstName.Trim() + " " + emailDetail.LastName.Trim() + " (" + emailDetail.AttyID.Trim() + ")");
                                Log.ServicLog("Email : " + emailDetail.Email.Trim());
                                Log.ServicLog("------------------------------------------------------------------------");

                            }

                            else
                            {
                                try
                                {
                                    body = body.Replace("{LOCATION}", sb.ToString());
                                    Utility.SendMailBilling("Authorization To Be Called On", body, emailDetail.Email, true, "AxiomSupport@axiomcopy.com", null, "autharchive@axiomcopy.com,autoemail@axiomcopy.com");
                                    foreach (var part in lstPartNo)
                                    {
                                        DbAccess.UpdateAuthorizationToBeCalledOnEmail(orderNo, part.PartNo, 0);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Log.ServicLog("----------------- EXCEPTION -------------------");
                                    Log.ServicLog(ex.ToString());
                                    Log.ServicLog("-----------------------------------------------");
                                }
                                lstOrder.Add(Convert.ToInt32(item.OrderNo));
                                Log.ServicLog("----------------------------------------");
                                Log.ServicLog("Email sent to " + emailDetail.Email.Trim() + " : " + System.DateTime.Now);
                                Log.ServicLog("Order No " + orderNo.ToString());
                                Log.ServicLog("Attorney Name : " + emailDetail.FirstName.Trim() + " " + emailDetail.LastName.Trim() + " (" + emailDetail.AttyID.Trim() + ")");


                                Log.ServicLog("----------------------------------------");
                                Log.ServicLog("Email sent to " + emailDetail.Email.Trim() + " : " + System.DateTime.Now);
                                Log.ServicLog("Order No " + orderNo.ToString());
                                Log.ServicLog("Attorney Name : " + emailDetail.FirstName.Trim() + " " + emailDetail.LastName.Trim() + " (" + emailDetail.AttyID.Trim() + ")");
                            }
                            foreach (var part in lstPartNo)
                            {
                                Guid value = new Guid("7ABB0EFB-88A9-4699-B359-7F17216A8258");
                                string notes = "Contacted patients attorney for Authorization.";
                                DbAccess.InsertOrderNotes(orderNo, part.PartNo, notes, notes, value.ToString());
                            }
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                Log.ServicLog("----------- EXCEPTION -------------------");
                Log.ServicLog(Convert.ToString(ex.Message));
                Log.ServicLog(Convert.ToString(ex.StackTrace));
                Log.ServicLog(Convert.ToString(ex.InnerException));
                Log.ServicLog("--------------------------------------------");
            }
        }


        protected override void OnStart(string[] args)
        {
            Log.ServicLog("Service Started");
            SendMonthlyEmail();
            AxiomServiceExecution();
            ServiceExecution();
        }


        protected override void OnStop()
        {
            Log.ServicLog("Service Stopped");

        }
    }
}
