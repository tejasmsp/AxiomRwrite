using Axiom.Common;
using Axiom.Entity;
using Axiom.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Data.SqlClient;
using System.Text;
using System.Configuration;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Data;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Net.Mail;
using Axiom.Web.EmailHelper;
using System.Web;

namespace Axiom.Web.API
{
    public class OrderProcess
    {
        public HomeApiController homeApiController = new HomeApiController();
        private readonly GenericRepository<AttorneyEntity> _repository = new GenericRepository<AttorneyEntity>();

        public async Task ESignature(int OrderNo, int CompanyNo, string partCSVID = "")
        {
            CompanyDetailForEmailEntity objCompany = CommonFunction.CompanyDetailForEmail(CompanyNo);

            SqlParameter[] paramStatus = { new SqlParameter("OrderNo", (object)OrderNo ?? (object)DBNull.Value) };
            bool isPartAddLater = _repository.ExecuteSQL<bool>("GetOrderStatus", paramStatus).FirstOrDefault();

            await Task.Run(() =>
            {
                if (OrderNo > 0)
                {
                    //@isAddedPart
                    SqlParameter[] Sqlparam = { new SqlParameter("OrderNo", (object)OrderNo ?? (object)DBNull.Value) };
                    var attyResultList = _repository.ExecuteSQL<ESignOrderAttorney>("ESignGetOrderAttorney", Sqlparam).ToList();
                    if (attyResultList != null && attyResultList.Count > 0)
                    {
                        foreach (var item in attyResultList)
                        {
                            string EmailID = item.Email;
                            string AttorneyName = item.AttyName;
                            if (!string.IsNullOrEmpty(EmailID))
                            {
                                try
                                {
                                    StringBuilder body = new StringBuilder();
                                    string htmlfilePath = AppDomain.CurrentDomain.BaseDirectory + "/MailTemplate/LinktoPatientAttorney.html";
                                    using (StreamReader reader = new StreamReader((htmlfilePath)))
                                    {
                                        body.Append(reader.ReadToEnd());
                                    }
                                    body = body.Replace("{AttorneyName}", AttorneyName);
                                    body = body.Replace("{LogoURL}", objCompany.Logopath);
                                    body = body.Replace("{ThankYou}", objCompany.ThankYouMessage);
                                    body = body.Replace("{CompanyName}", objCompany.CompName);
                                    body = body.Replace("{Link}", objCompany.SiteURL);

                                    var fileList = new List<FileEntity>();


                                    SqlParameter[] Sqlfileparam = {
                                            new SqlParameter("OrderNo", (object)OrderNo ?? (object)DBNull.Value)
                                            ,new SqlParameter("isAddedPart", (object)isPartAddLater ?? (object)DBNull.Value)
                                    };
                                    fileList = _repository.ExecuteSQL<FileEntity>("ESignGetFileList", Sqlfileparam).ToList();

                                    //if (partCSVID.Trim().Length == 0)
                                    //{
                                    //    SqlParameter[] Sqlfileparam = { new SqlParameter("OrderNo", (object)OrderNo ?? (object)DBNull.Value) };
                                    //    fileList = _repository.ExecuteSQL<FileEntity>("ESignGetFileList", Sqlfileparam).ToList();
                                    //}
                                    //else
                                    //{
                                    //    SqlParameter[] Sqlfileparam = { new SqlParameter("OrderNo", (object)OrderNo ?? (object)DBNull.Value)
                                    //        ,new SqlParameter("PartNo", (object)partCSVID ?? (object)DBNull.Value)
                                    //    };
                                    //    fileList = _repository.ExecuteSQL<FileEntity>("ESignGetFileList", Sqlfileparam).ToList();
                                    //}

                                    string Link = "";
                                    if (fileList != null && fileList.Count > 0)
                                    {
                                        foreach (var singlefile in fileList)
                                        {
                                            int FileID = singlefile.FileId;
                                            string FileName = singlefile.FileName;
                                            string increptID = EncryptDecrypt.Encrypt(FileID.ToString());
                                            string LinkURL = Convert.ToString(ConfigurationManager.AppSettings["SignatureLinkUrl"]);
                                            Link += "<tr><td><a href=\"" + LinkURL + "Viewer//default.aspx?FileID=" + System.Web.HttpUtility.UrlEncode(increptID) + "\">Click Here to show " + FileName + " </a></td></tr>";
                                        }
                                        body = body.Replace("{Link}", Link);
                                        try
                                        {
                                            List<Attachment> lst = new List<Attachment>();
                                            string attachmentPath = AppDomain.CurrentDomain.BaseDirectory + "/MailTemplate/GuideLine.PNG";
                                            Attachment inline = new Attachment(attachmentPath);
                                            lst.Add(inline);
                                            Email.SendMailWithAttachment(EmailID, body.ToString(), "Sign Document", null, lst, "autharchive@axiomcopy.com", "tejaspadia@gmail.com");
                                        }
                                        catch (Exception ex)
                                        {

                                        }
                                    }
                                }
                                catch (Exception ex)
                                {

                                }
                            }
                        }
                    }
                }


            });
        }

        public async Task OrderSummaryEmail(int OrderId, string userEmail, int CompanyNo, int SubmitStatus = 0)
        {
            await Task.Run(() =>
            {
                SqlParameter[] param = { new SqlParameter("OrderID", (object)OrderId ?? (object)DBNull.Value) };
                var result = _repository.ExecuteSQL<OrderDetailEntity>("GetOrderDetails", param).ToList();

                CompanyDetailForEmailEntity objCompany = CommonFunction.CompanyDetailForEmail(CompanyNo);                

                if (result != null && result.Count > 0)
                {
                    var data = result[0];

                    //Order Confirmation Attorney Email
                    string orderConfirmEmails = "";
                    SqlParameter[] orderconfirmparam = { new SqlParameter("OrderNo", (object)OrderId ?? (object)DBNull.Value),
                    new SqlParameter("IsOrderConfirm", (object)true ?? (object)DBNull.Value)};
                    var orderConfirmEmailList = _repository.ExecuteSQL<string>("GetAttorneyEmailList", orderconfirmparam).ToList();
                    if (orderConfirmEmailList != null && orderConfirmEmailList.Count > 0)
                    {
                        foreach (var item in orderConfirmEmailList)
                        {
                            orderConfirmEmails += item + ",";
                        }
                        orderConfirmEmails = orderConfirmEmails.Remove(orderConfirmEmails.Length - 1);
                    }


                    if (!string.IsNullOrEmpty(userEmail))
                        userEmail = userEmail + ",";

                    if (!string.IsNullOrEmpty(orderConfirmEmails))
                        userEmail += orderConfirmEmails;

                    StringBuilder body = new StringBuilder();
                    string htmlfilePath = "";
                    bool isNewOrder = SubmitStatus == 1 ? false : true;
                    if (isNewOrder)
                    {
                        htmlfilePath = AppDomain.CurrentDomain.BaseDirectory + "/MailTemplate/OrderSummary/OrderSummaryPage.html";
                    }
                    else
                    {
                        htmlfilePath = AppDomain.CurrentDomain.BaseDirectory + "/MailTemplate/OrderSummary/Edit.html";
                    }
                    using (StreamReader reader = new StreamReader((htmlfilePath)))
                    {
                        body.Append(reader.ReadToEnd());
                    }
                    ////Ordering Information
                    body = body.Replace("##ORDATTY##", data.FirstName + " " + data.LastName);
                    body = body.Replace("##ORDATTYFOR##", data.AttorneyFor);
                    body = body.Replace("##ORDREPRESENT##", data.Represents);
                    ////Billing Information
                    body = body.Replace("##BILLTOORDFIRM##", Convert.ToString(data.BillToOrderingFirm));
                    body = body.Replace("##CLAIMNO##", data.BillingClaimNo);
                    body = body.Replace("##DATELOSS##", data.BillingDateOfLoss != null && data.BillingDateOfLoss.ToString().Contains("1/1/1900") ? "" : string.Format("{0:MM/dd/yyyy}", data.BillingDateOfLoss));
                    body = body.Replace("##INSURED##", data.BillingInsured);
                    ////Records Of
                    body = body.Replace("##Name##", data.PatientName);
                    body = body.Replace("##SSN##", data.SSN);
                    body = body.Replace("##DOB##", data.DateOfBirth != null && data.DateOfBirth.ToString().Contains("1/1/1900") ? "" : string.Format("{0:MM/dd/yyyy}", data.DateOfBirth));

                    body = body.Replace("##Death##", data.DateOfDeath != null && data.DateOfDeath.ToString().Contains("1/1/1900") ? "" : string.Format("{0:MM/dd/yyyy}", data.DateOfDeath));
                    body = body.Replace("##Address1##", data.Address1);
                    body = body.Replace("##Address2##", data.Address2);
                    body = body.Replace("##City##", data.City);
                    body = body.Replace("##State##", data.OPStateName);
                    body = body.Replace("##Zip##", data.ZipCode);
                    body = body.Replace("##ClientMATNO##", data.ClaimMatterNo);
                    body = body.Replace("##PatientType##", data.PatientType);
                    ////Case Information
                    body = body.Replace("##Style1##", data.Caption1);
                    body = body.Replace("##Style2##", data.VsText1);
                    body = body.Replace("##Style3##", data.Caption2);
                    body = body.Replace("##Style4##", data.VsText2);
                    body = body.Replace("##Style5##", data.Caption3);
                    body = body.Replace("##Style6##", data.VsText3);
                    body = body.Replace("##Style7##", "");
                    body = body.Replace("##CauseNO##", data.CauseNo);
                    body = body.Replace("##TrialDate##", data.TrialDate != null && data.TrialDate.ToString().Contains("1/1/1900") ? "" : string.Format("{0:MM/dd/yyyy}", data.TrialDate));
                    body = body.Replace("##State##", data.OrderCaseState);
                    body = body.Replace("##Rush##", Convert.ToBoolean(data.Rush) ? "Yes" : "No");
                    body = body.Replace("##District##", data.District);
                    body = body.Replace("##Division##", data.Division);
                    body = body.Replace("##County##", data.CountyName);
                    body = body.Replace("##Court##", data.Court);
                    body = body.Replace("##IMAGELINK##", objCompany.Logopath);

                    //Attorneys of Record
                    SqlParameter[] SQLparam = { new SqlParameter("OrderId", (object)OrderId ?? (object)DBNull.Value) };
                    var attorneyList = _repository.ExecuteSQL<OrderWizardStep5>("GetOrderWizardStep5AttorneyRecords", SQLparam).ToList();
                    if (attorneyList != null)
                    {
                        StringBuilder joinAttorneys = new StringBuilder();
                        foreach (var atty in attorneyList)
                        {
                            StringBuilder sbAttorney = new StringBuilder();
                            string htmlfilePathForAttorneys = AppDomain.CurrentDomain.BaseDirectory + "/MailTemplate/OrderSummary/OrderSummaryPageAttorneys.html";
                            using (StreamReader reader = new StreamReader((htmlfilePathForAttorneys)))
                            {
                                sbAttorney.Append(reader.ReadToEnd());
                            }
                            sbAttorney = sbAttorney.Replace("##ATTYFirm##", atty.FirmID);
                            sbAttorney = sbAttorney.Replace("##ATTYFIRSTNAME##", atty.AttorneyFirstName);
                            sbAttorney = sbAttorney.Replace("##ATTYLASTNAME##", atty.AttorneyLastName);
                            sbAttorney = sbAttorney.Replace("##ATTYADD1##", atty.Street1);
                            sbAttorney = sbAttorney.Replace("##ATTYADD2##", atty.Street2);
                            sbAttorney = sbAttorney.Replace("##ATTYCITY##", atty.City);
                            sbAttorney = sbAttorney.Replace("##ATTYSTATE##", atty.StateName);
                            if (!string.IsNullOrEmpty(atty.Zip))
                            {
                                if (atty.Zip.Length >= 5)
                                    sbAttorney = sbAttorney.Replace("##ATTYZIP##", atty.Zip.Substring(0, 5));
                                else
                                    sbAttorney = sbAttorney.Replace("##ATTYZIP##", atty.Zip.Substring(0, atty.Zip.Length));
                            }
                            else
                                sbAttorney = sbAttorney.Replace("##ATTYZIP##", "");

                            sbAttorney = sbAttorney.Replace("##ATTYPHONE##", atty.AreaCode1 + "-" + atty.PhoneNo);
                            sbAttorney = sbAttorney.Replace("##ATTYFAX##", atty.AreaCode2 + "-" + atty.FaxNo);
                            sbAttorney = sbAttorney.Replace("##ATTYSTATEBARNO##", atty.StateBarNo);
                            sbAttorney = sbAttorney.Replace("##ATTYAttorneyFor##", atty.AttorneyFor);
                            sbAttorney = sbAttorney.Replace("##ATTYRepresents##", atty.Represents);
                            sbAttorney = sbAttorney.Replace("##ATTYOPPSIDE##", atty.OppSide.ToString());
                            sbAttorney = sbAttorney.Replace("##ATTYORDATTORNEY##", "false");
                            sbAttorney = sbAttorney.Replace("##ATTYPatientAttorney##", atty.IsPatientAttorney.ToString());
                            sbAttorney = sbAttorney.Replace("##ATTYRemarks##", atty.Notes);

                            joinAttorneys = joinAttorneys.Append(sbAttorney);

                        }
                        body = body.Replace("##Attorneys##", joinAttorneys.ToString());

                        //Locations
                        int locationCount = 0;
                        int IsRequiredRecordType = 1;
                        SqlParameter[] SQLLocationparam =
                                {   new SqlParameter("OrderId", (object)OrderId ?? (object)DBNull.Value),
                                    new SqlParameter("IsRequiredRecordType", (object)IsRequiredRecordType ?? (object)DBNull.Value)
                                };
                        var locationList = new List<OrderWizardStep6>();

                        if (isNewOrder)
                        {
                            // NEW ORDER - GET ALL PARTS TO SEND ORDER SUMMARY EMAIL
                            locationList = _repository.ExecuteSQL<OrderWizardStep6>("GetOrderWizardStep6Location", SQLLocationparam).ToList();
                        }
                        else
                        {
                            // EDITED ORDER - GET ONLY NEWLY ADDED PARTS TO SEND ORDER SUMMARY EMAIL
                            locationList = _repository.ExecuteSQL<OrderWizardStep6>("OrderSummaryGetAddedPart", SQLLocationparam).ToList();

                        }


                        StringBuilder combineLocation = new StringBuilder();
                        foreach (var loc in locationList)
                        {
                            string fileName = string.Empty;

                            string htmlfilePathForLocations = AppDomain.CurrentDomain.BaseDirectory + "/MailTemplate/OrderSummary/OrderSummaryPageLocations.html";
                            StringBuilder sbLocations = new StringBuilder();
                            using (StreamReader reader = new StreamReader((htmlfilePathForLocations)))
                            {
                                sbLocations.Append(reader.ReadToEnd());
                            }

                            //    SqlParameter[] SQLScopeParam = { new SqlParameter("AttyID", (object)data.AttyID ?? (object)DBNull.Value),
                            //new SqlParameter("RecType", (object)locationList[locationCount].RecordTypeId ?? (object)DBNull.Value)};

                            //var resultLocation = _repository.ExecuteSQL<string>("GetDefaultScopeByAttyID", SQLScopeParam).ToList();
                            //var locScope = string.Empty;
                            //if (resultLocation != null && resultLocation.Count > 0)
                            //{
                            //    locScope = resultLocation[0];

                            //    if (locationList[locationCount].ScopeStartDate != null && !string.IsNullOrEmpty(locScope))
                            //    {
                            //        locScope = Regex.Replace(locScope, "FROM <<DOB>>", "FROM " + locationList[locationCount].ScopeStartDate.ToString().Trim(), RegexOptions.IgnoreCase);
                            //    }
                            //    if (data.DateOfBirth != null && !string.IsNullOrEmpty(locScope))
                            //    {
                            //        locScope = locScope.Replace("<<DOB>>", string.Format("{0:MM/dd/yyyy}", data.DateOfBirth));
                            //    }
                            //    if (locationList[locationCount].ScopeEndDate != null && !string.IsNullOrEmpty(locScope))
                            //    {
                            //        locScope = locScope.Replace("PRESENT", locationList[locationCount].ScopeEndDate.ToString().Trim());
                            //    }
                            //    if (!string.IsNullOrEmpty(locScope))
                            //    {
                            //        locScope = locScope.Replace("<<SSN>>", data.SSN.Trim());
                            //    }

                            //    if (!string.IsNullOrEmpty(locScope))
                            //    {
                            //        locScope = locScope.Replace("<<PATIENT>>", data.PatientName.Trim());
                            //    }

                            //    locScope = locScope + " " + loc.Notes;

                            //}

                            sbLocations = sbLocations.Replace("##LOCNAME1##", loc.Name1);
                            sbLocations = sbLocations.Replace("##LOCNAME2##", loc.Name2);
                            sbLocations = sbLocations.Replace("##LOCDEPT##", loc.Dept);
                            sbLocations = sbLocations.Replace("##LOCRECTYPE##", loc.RecordTypeDesc);
                            sbLocations = sbLocations.Replace("##LOCADDRESS1##", loc.Street1);
                            sbLocations = sbLocations.Replace("##LOCADDRESS2##", loc.Street2);
                            sbLocations = sbLocations.Replace("##LOCCITY##", loc.City);
                            sbLocations = sbLocations.Replace("##LOCSTATE##", loc.State);
                            sbLocations = sbLocations.Replace("##LOCZIP##", loc.Zip);
                            sbLocations = sbLocations.Replace("##LOCPHONE##", loc.AreaCode1 + "-" + loc.PhoneNo1);
                            sbLocations = sbLocations.Replace("##LOCFAX##", loc.AreaCode3 + "-" + loc.FaxNo);
                            sbLocations = sbLocations.Replace("##LOCREMARKS##", loc.Comment);
                            sbLocations = sbLocations.Replace("##LOCSCOPE##", loc.Scope);

                            if (data.BillToOrderingFirm == false && !string.IsNullOrEmpty(loc.RecordTypeDesc) && loc.RecordTypeDesc.Contains("Films") && !string.IsNullOrEmpty(data.FirmName) && data.FirmName.ToUpper().Contains("HANOVER"))
                            {
                                string htmltrtext = "<tr style=\"-webkit-box-sizing: border-box;-moz-box-sizing: border-box;box-sizing: border-box;page-break-inside: avoid;\" ><td style = \"-webkit-box-sizing: border-box;-moz-box-sizing: border-box;box-sizing: border-box;padding: 8px;line-height: 1.42857143;vertical-align: middle;border-top: none;border: 1px solid #ddd!important;border-top-color: #E6E9ED;white-space: nowrap;border-right: 0;background-color: #fff!important;width:35px;\" > Pre-Approval </td ><td style =\" -webkit-box-sizing: border-box;-moz-box-sizing: border-box;box-sizing: border-box;padding: 8px;line-height: 1.42857143;vertical-align: middle;border-top: 1px solid #ddd;border: 1px solid #ddd!important;border-top-color: #E6E9ED;border-right: 0;background-color: #fff!important;\" >Client has indicated that pre-approval to request films has been obtained<br></td></tr >";
                                sbLocations = sbLocations.Replace("##PreApprovalMsg##", htmltrtext);
                            }
                            else
                            {
                                sbLocations = sbLocations.Replace("##PreApprovalMsg##", "");
                            }
                            SqlParameter[] SQLLocParam = { new SqlParameter("OrderId", (object)OrderId ?? (object)DBNull.Value),
                         new SqlParameter("PartNo", (object)loc.PartNo ?? (object)DBNull.Value)};
                            var FileList = _repository.ExecuteSQL<string>("GetLocationFiles", SQLLocParam).ToList();
                            foreach (var file in FileList)
                            {
                                fileName += file;
                                fileName += "<br>";
                            }

                            sbLocations = sbLocations.Replace("##LocationFiles##", fileName);
                            combineLocation = combineLocation.Append(sbLocations);
                            locationCount++;
                        }
                        body = body.Replace("##locations##", combineLocation.ToString());
                        StringBuilder combinecanvas = new StringBuilder();
                        SqlParameter[] canvasparam = { new SqlParameter("OrderId", (object)OrderId ?? (object)DBNull.Value) };
                        var canvasList = _repository.ExecuteSQL<CanvasRequestEntity>("GetCanvasList", canvasparam).ToList();
                        foreach (var canvas in canvasList)
                        {
                            string fileName = string.Empty;
                            string htmlfilePathForcanvas = AppDomain.CurrentDomain.BaseDirectory + "/MailTemplate/OrderSummary/OrderSummaryPageCanvas.html";
                            StringBuilder sbCanvas = new StringBuilder();
                            using (StreamReader reader = new StreamReader((htmlfilePathForcanvas)))
                            {
                                sbCanvas.Append(reader.ReadToEnd());
                            }
                            sbCanvas = sbCanvas.Replace("##Package##", canvas.PkgName);
                            sbCanvas = sbCanvas.Replace("##Selection##", canvas.PkgDescription);
                            sbCanvas = sbCanvas.Replace("##ZipCode##", canvas.ZipCode);
                            combinecanvas = combinecanvas.Append(sbCanvas);
                        }
                        body = body.Replace("##Canvas##", combinecanvas.ToString());
                        StringWriter writer = new StringWriter(body);
                        MemoryStream ms = new MemoryStream();
                        StreamWriter w = new StreamWriter(ms);
                        w.Write(writer.ToString());
                        w.Flush();
                        string FilePath = ConfigurationManager.AppSettings["UploadRoot"].ToString();
                        Guid g = Guid.NewGuid();

                        string FilePathDoc = string.Format("{0}{1}\\", FilePath, OrderId);

                        string FilePathPdf = string.Format("{0}{1}\\", FilePath, OrderId);
                        DirectoryInfo dis = new DirectoryInfo(FilePathDoc);
                        if (!dis.Exists)
                        {
                            dis.Create();
                        }
                        if (isNewOrder)
                        {
                            FilePathDoc = FilePathDoc + string.Format("{0}_{1}", "NewOrderSummary", g + ".html");
                        }
                        else
                        {
                            FilePathDoc = FilePathDoc + string.Format("{0}_{1}", "EditOrderSummary", g + ".html");
                        }
                        FileStream fs = null;
                        fs = File.Create(FilePathDoc);
                        using (StreamWriter stream = new StreamWriter(fs))
                        {
                            stream.Write(writer);
                            stream.Close();
                        }

                        var objfile = new OrderWizardStep6Document();
                        objfile.FileName = isNewOrder ? "NewOrderSummary.html" : "EditOrderSummary.html";
                        objfile.OrderNo = OrderId;
                        objfile.PartNo = 0;
                        objfile.FileDiskName = string.Format("{0}_{1}", isNewOrder ? "NewOrderSummary" : "EditOrderSummary", g + ".html");
                        objfile.CreatedBy = Convert.ToString(new Guid("9037E8F5-35E3-4B50-B183-A53B31D4354C"));
                        objfile.RecordTypeId = 0;
                        objfile.Pages = 0;
                        objfile.FileTypeId = (int)FileType.Other;
                        string xmlAttachData = ConvertToXml<OrderWizardStep6Document>.GetXMLString(new List<OrderWizardStep6Document>() { objfile });
                        SqlParameter[] attachparam = { new SqlParameter("xmlDataString", (object)xmlAttachData ?? (object)DBNull.Value) };
                        _repository.ExecuteSQL<int>("InsertFiles", attachparam).FirstOrDefault();


                        System.Net.Mime.ContentType ct = new System.Net.Mime.ContentType(System.Net.Mime.MediaTypeNames.Text.Html);
                        System.Net.Mail.Attachment a = new System.Net.Mail.Attachment(new System.IO.MemoryStream(ms.GetBuffer(), 0, Convert.ToInt32(ms.Length), false), ct);
                        a.ContentDisposition.FileName = "OrderSummary.html";
                        StringBuilder strBody = new StringBuilder();
                        if (isNewOrder)
                        {
                            strBody.Append("Thank you for submitting a new order with us.");
                            strBody.Append("<br/>");
                            strBody.Append("<br/>");
                        }
                        else
                        {
                            strBody.Append("Thank you for submitting order with us.");
                            strBody.Append("<br/>");
                            strBody.Append("<br/>");
                        }
                        strBody.Append("<br/>");
                        strBody.Append(objCompany.ThankYouMessage);
                        //strBody.Append("<br/>");
                        //strBody.Append("Attached is your order summary.");
                        //strBody.Append("<br/>");
                        //strBody.Append("<br/>");
                        //strBody.Append("Best Regards,");
                        //strBody.Append("<br/>");
                        //strBody.Append("Axiom Requisition Copy Service");

                        List<System.Net.Mail.Attachment> attachmentList = new List<System.Net.Mail.Attachment>();
                        System.Net.Mail.Attachment attachment = new System.Net.Mail.Attachment(FilePathDoc);
                        attachmentList.Add(attachment);

                        try
                        {
                            List<string> attchmentFileList = new List<string>();
                            if (isNewOrder)
                            {
                                attchmentFileList.Add("NewOrderSummary.html");
                            }
                            else
                            {
                                attchmentFileList.Add("EditOrderSummary.html");
                            }
                            string subject = string.Empty;
                            subject = isNewOrder ? "New Order Summary " : "Edit Order Summary ";
                            subject = subject + Convert.ToString(OrderId);
                            // EmailHelper.Email.SendMailWithAttachment(userEmail, strBody.ToString(), isNewOrder ? "New Order Summary" : "Edit Order Summary", attchmentFileList, attachmentList, "", "");
                            EmailHelper.Email.SendMailWithAttachment(userEmail, strBody.ToString(), subject, attchmentFileList, attachmentList, "autharchive@axiomcopy.com", "tejaspadia@gmail.com");
                            //string illegal = "[^a-zA-Z0-9.-]";
                            //Regex reg = new Regex(illegal);
                            //string recsOf = NormalizeLength(data.PatientName, 60);
                            //recsOf = reg.Replace(recsOf, "_");


                            // EmailHelper.Email.SendMailWithAttachment(userEmail, strBody.ToString(), isNewOrder ? "New Order Summary" : "Edit Order Summary", attchmentFileList, attachmentList, "autharchive@axiomcopy.com", "tejaspadia@gmail.com");
                            //string illegal = "[^a-zA-Z0-9.-]";
                            //Regex reg = new Regex(illegal);
                            //string recsOf = NormalizeLength(data.PatientName, 60);
                            //recsOf = reg.Replace(recsOf, "_");

                            w.Dispose();
                            ms.Dispose();
                        }
                        catch (Exception ex)
                        {
                            Log.ServicLog(ex.ToString());
                        }

                    }

                }
            });
        }

        private string NormalizeLength(string value, int maxLength)
        {

            if ((value != null))
            {
                if (value.Length <= maxLength)
                {
                    return value;
                }
                return value.Substring(0, maxLength);
            }
            else
            {
                return string.Empty;
            }
        }

        public async Task AddQuickformsForNewOrder(int OrderNo, bool isEditOrder = false, bool isNewAddedPart = false, string partNumberCSV = "")
        {
            await Task.Run(() =>
            {
                try
                {
                    string documentRoot = Convert.ToString(ConfigurationManager.AppSettings["DocumentRoot"]);
                    List<QuickFormDocument> AuthRequestDocName = new List<QuickFormDocument>();
                    SqlParameter[] Sqlorderparam = { new SqlParameter("OrderId", (object)OrderNo ?? (object)DBNull.Value) };
                    var objOrderDetail = _repository.ExecuteSQL<QuickFormOrderDetail>("QuickFormGetOrderDetails", Sqlorderparam).FirstOrDefault();


                    var partList = new List<PartDetail>();

                    if (isNewAddedPart)
                    {
                        // OrderSummaryGetAddedPart
                        SqlParameter[] Sqlparam =   {new SqlParameter("OrderNo", (object)OrderNo ?? (object)DBNull.Value)
                                                        ,new SqlParameter("PartNo", (object)partNumberCSV ?? (object)DBNull.Value) };
                        partList = _repository.ExecuteSQL<PartDetail>("GetPartsByCsvID", Sqlparam).ToList();


                        // ADD UPLOADED FILES TO PART
                        //Upload Location Documents on Final Submit

                        SqlParameter[] SqlFileparam = { new SqlParameter("OrderNo", (object)OrderNo ?? (object)DBNull.Value)
                                // ,new SqlParameter("PartNo", (object)partNumberCSV ?? (object)DBNull.Value)
                        };
                        var LocDocumentList = _repository.ExecuteSQL<LocationFilesModel>("GetLocationTempFilesByCsvPart", SqlFileparam).ToList();
                        if (LocDocumentList != null && LocDocumentList.Count > 0)
                        {
                            string sourcePath = ConfigurationManager.AppSettings["AttachPathLocal"].ToString();
                            string destinationPath = ConfigurationManager.AppSettings["UploadRoot"].ToString();
                            foreach (var item in LocDocumentList)
                            {
                                SqlParameter[] deleteattachparam = {
                                    new SqlParameter("OrderNo", (object)OrderNo ?? (object)DBNull.Value),
                                    new SqlParameter("PartNo", (object)item.PartNo ?? (object)DBNull.Value),
                                    new SqlParameter("FileTypeId", (object) item.FileTypeId ?? (object)DBNull.Value),
                                    new SqlParameter("DeleteFromLocFile", (object) (int)0 ?? (object)DBNull.Value),
                                    new SqlParameter("LocFileId", (object) (int)0 ?? (object)DBNull.Value)
                                };
                                _repository.ExecuteSQL<int>("DeleteLocationFile", deleteattachparam).FirstOrDefault();
                            }
                            foreach (var docitem in LocDocumentList)
                            {

                                var destPath = destinationPath + "/" + docitem.OrderNo + "/" + docitem.PartNo;
                                string fileDiskName = new Document().MoveLocalToServerFile(docitem.FileName, docitem.BatchId, sourcePath, destPath, docitem.OrderNo, docitem.PartNo.ToString());
                                docitem.OrderNo = Convert.ToInt32(docitem.OrderNo);
                                docitem.FileDiskName = fileDiskName;
                                docitem.Pages = 0;
                                string xmlAttachData = ConvertToXml<LocationFilesModel>.GetXMLString(new List<LocationFilesModel>() { docitem });
                                SqlParameter[] attachparam = { new SqlParameter("xmlDataString", (object)xmlAttachData ?? (object)DBNull.Value) };
                                _repository.ExecuteSQL<int>("InsertFiles", attachparam).FirstOrDefault();
                            }
                            foreach (var docitem in LocDocumentList)
                            {
                                Document docobj = new Document();
                                docobj.DeleteAttchFile(docitem.FileName, docitem.BatchId, sourcePath);
                            }
                        }

                    }
                    else
                    {
                        SqlParameter[] Sqlparam = { new SqlParameter("OrderNo", (object)OrderNo ?? (object)DBNull.Value) };
                        partList = _repository.ExecuteSQL<PartDetail>("GetPartsByOrder", Sqlparam).ToList();
                    }

                    SqlParameter[] attyparam = { new SqlParameter("OrderId", (object)OrderNo ?? (object)DBNull.Value) };
                    var patientResult = _repository.ExecuteSQL<OrderWizardStep5>("GetOrderWizardStep5AttorneyRecords", attyparam).ToList();
                    var ispatientAttyList = patientResult.Where(x => x.IsPatientAttorney == true).ToList();
                    string emailToPatientAtty = "";

                    try
                    {
                        if (ispatientAttyList != null && ispatientAttyList.Count != 0)
                        {
                            foreach (var item in ispatientAttyList)
                            {
                                emailToPatientAtty += item.Email + ",";
                            }
                            emailToPatientAtty = emailToPatientAtty.Remove(emailToPatientAtty.Length - 1);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.ServicLog(ex.ToString());
                    }

                    string orderConfirmEmails = "";
                    SqlParameter[] orderconfirmparam = { new SqlParameter("OrderNo", (object)OrderNo ?? (object)DBNull.Value),
                    new SqlParameter("IsOrderConfirm", (object)true ?? (object)DBNull.Value)};
                    var orderConfirmEmailList = _repository.ExecuteSQL<string>("GetAttorneyEmailList", orderconfirmparam).ToList();
                    if (orderConfirmEmailList != null && orderConfirmEmailList.Count > 0)
                    {
                        foreach (var item in orderConfirmEmailList)
                        {
                            orderConfirmEmails += item + ",";
                        }
                        orderConfirmEmails = orderConfirmEmails.Remove(orderConfirmEmails.Length - 1);
                    }

                    int i = 0, RecordType = 0;

                    bool isProcessServer = false;
                    foreach (var part in partList)
                    {
                        // TO GET PART CONFIGURATION 

                        SqlParameter[] param = {
                                                            new SqlParameter("LocationId", (object)part.LocID ?? (object)DBNull.Value)
                                                        };
                        var locationList = _repository.ExecuteSQL<LocationEntity>("GetLocationById", param).FirstOrDefault();


                        if (part.isSup.HasValue && part.isSup.Value)
                            continue;

                        if (!string.IsNullOrEmpty(part.LocID) && part.LocID.Trim() != "CANVAS02" && part.isCreateAuthSup != null)
                        {
                            if (part.isBatchUpload)
                                continue;
                            if (part.isCreateAuthSup == true)
                                AuthRequestDocName = GetQuickFormAuthDocument(part.LocID, 1);
                            else
                                AuthRequestDocName = GetQuickFormAuthDocument(part.LocID, 0);

                            QuickFormEntity qf = new QuickFormEntity();
                            if (AuthRequestDocName == null || AuthRequestDocName.Count == 0)
                            {
                                i++;
                                qf.OrderNo = OrderNo;
                                qf.PartNo = Convert.ToString(part.PartNo);
                                if (part.isCreateAuthSup == true)
                                {
                                    qf.DocName = OrderNo + "_" + part.PartNo + "_" + "Medical Blank Authorization - NO ORAL COMMUNICATION.doc";
                                    qf.DocPath = "Custodian Letters>All Authorizations";
                                    qf.FileTypeID = (int)FileType.Authorization;
                                }
                                else
                                {
                                    qf.DocName = OrderNo + "_" + part.PartNo + "_" + "FAX Request.doc";
                                    if (objOrderDetail.State == "MI")
                                        qf.DocPath = "Subpoenas>State>Michigan";
                                    else
                                        qf.DocPath = "Custodian Letters>All Letter Request Forms>FOIA or Letter Requests";

                                    qf.FileTypeID = (int)FileType.Request;
                                }

                                if (part.isCreateAuthSup == true) // CREATE AUTH OR SUBPOENA
                                {
                                    if (part.isSup == true)
                                    {
                                        qf.Email = string.Empty;
                                        qf.IsEmail = 0;
                                    }
                                    else
                                    {
                                        if (!string.IsNullOrEmpty(objOrderDetail.Email))
                                        {
                                            qf.Email = objOrderDetail.Email;
                                            qf.IsEmail = 1;
                                        }
                                        else
                                        {
                                            qf.Email = string.Empty;
                                            qf.IsEmail = 0;
                                        }
                                    }
                                }
                                else // UPLOAD AUTH OR SUBPOENA
                                {
                                    // UPLOAD SUBPOENA
                                    if (part.isSup == true)
                                    {
                                        qf.Email = string.Empty;
                                        qf.FaxNo = string.Empty;
                                        qf.IsFax = 0;
                                        qf.IsEmail = 0;
                                    }
                                    else // UPLOAD AUTH
                                    {
                                        LocationEntity loc = GetLocationDetail(part.LocID);
                                        if (loc != null)
                                        {
                                            if (string.IsNullOrEmpty(loc.SendRequest))
                                            {
                                                qf.Email = loc.Email;
                                                qf.IsEmail = 1;
                                                qf.FaxNo = string.Empty;
                                                qf.IsFax = 0;
                                            }
                                            else
                                            {
                                                if (loc.SendRequest.IndexOf("0") >= 0)
                                                {
                                                    qf.FaxNo = loc.AreaCode3 + loc.FaxNo.Replace("-", "").Trim();
                                                    qf.Email = string.Empty;
                                                    qf.IsFax = 1;
                                                    qf.IsEmail = 0;
                                                }
                                                else if (loc.SendRequest.IndexOf("1") >= 0)
                                                {
                                                    qf.FaxNo = string.Empty;
                                                    qf.Email = string.Empty;
                                                    qf.IsEmail = 0;
                                                    qf.IsFax = 0;
                                                }
                                                else
                                                {
                                                    qf.Email = loc.Email;
                                                    qf.FaxNo = string.Empty;
                                                    qf.IsFax = 0;
                                                    qf.IsEmail = 1;
                                                }
                                            }
                                        }
                                    }
                                }
                                qf.UserId = Convert.ToString(new Guid("7ABB0EFB-88A9-4699-B359-7F17216A8258"));
                                qf.IsPrint = 0;
                                qf.printStatus = 0;
                                qf.EmailStatus = 0;
                                qf.faxStatus = 0;
                                qf.Pages = 0;
                                qf.Name = string.Empty;
                                qf.Address = string.Empty;
                                qf.IsSSN = false;
                                qf.IsRevised = string.Empty;
                                qf.isFromClient = true;
                                qf.IsPublic = true;
                                qf.UpdatedDate = null;
                            }
                            else
                            {
                                for (int i1 = 0; i1 < AuthRequestDocName.Count; i1++)
                                {
                                    i++;
                                    qf = new QuickFormEntity();
                                    qf.OrderNo = OrderNo;
                                    qf.PartNo = Convert.ToString(part.PartNo);
                                    if (part.isCreateAuthSup == true)
                                    {
                                        if (AuthRequestDocName != null)
                                        {
                                            qf.DocName = OrderNo + "_" + part.PartNo + "_" + AuthRequestDocName[i1].DocFileName;
                                            qf.DocPath = AuthRequestDocName[i1].FolderPath;
                                            qf.FileTypeID = (int)FileType.Authorization;
                                        }
                                    }
                                    else
                                    {
                                        if (AuthRequestDocName != null)
                                        {
                                            qf.DocName = OrderNo + "_" + part.PartNo + "_" + AuthRequestDocName[i1].DocFileName;
                                            qf.DocPath = AuthRequestDocName[i1].FolderPath;
                                            qf.FileTypeID = (int)FileType.Request;
                                        }
                                    }
                                    qf.CreateDocument = false;
                                    if (part.isCreateAuthSup == true)
                                    {
                                        // CREATE AUTH / SUP : SEND TO ATTORNEY IF ATTORNEY IS AVAILABLE OTHERWISE DONT SEND ANYWHERE
                                        // IN CASE OF SUP DONT DO ANYTHING JUST UPLOAD DOCS ON ORDER->PART
                                        if (part.isSup == true)
                                        {
                                            qf.Email = string.Empty;
                                            qf.IsEmail = 0;
                                        }
                                        else
                                        {
                                            if (!string.IsNullOrEmpty(objOrderDetail.Email))
                                            {
                                                qf.Email = objOrderDetail.Email;
                                                qf.IsEmail = 1;
                                            }
                                            else
                                            {
                                                qf.Email = string.Empty;
                                                qf.IsEmail = 0;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        // UPLOAD AUTH
                                        if (part.isSup == true)
                                        {
                                            qf.Email = string.Empty;
                                            qf.FaxNo = string.Empty;
                                            qf.IsFax = 0;
                                            qf.IsEmail = 0;
                                        }
                                        else
                                        {
                                            LocationEntity loc = GetLocationDetail(part.LocID);
                                            if (string.IsNullOrEmpty(loc.SendRequest))
                                            {
                                                qf.Email = loc.Email;
                                                qf.IsEmail = 1;
                                                qf.FaxNo = string.Empty;
                                                qf.IsFax = 0;
                                            }
                                            else
                                            {
                                                if (loc.SendRequest.IndexOf("0") >= 0)
                                                {
                                                    // SEND FAX
                                                    qf.FaxNo = loc.AreaCode3 + loc.FaxNo.Replace("-", "").Trim();
                                                    qf.Email = string.Empty;
                                                    qf.IsFax = 1;
                                                    qf.IsEmail = 0;
                                                }
                                                else if (loc.SendRequest.IndexOf("1") >= 0)
                                                {
                                                    // MAIL VIA POST
                                                    qf.FaxNo = string.Empty;
                                                    qf.Email = string.Empty;
                                                    qf.IsEmail = 0;
                                                    qf.IsFax = 0;
                                                }
                                                else
                                                {
                                                    // EMAIL
                                                    qf.Email = loc.Email;
                                                    qf.FaxNo = string.Empty;
                                                    qf.IsFax = 0;
                                                    qf.IsEmail = 1;
                                                }
                                            }
                                        }
                                    }
                                    qf.UserId = Convert.ToString(new Guid("7ABB0EFB-88A9-4699-B359-7F17216A8258"));
                                    qf.IsPrint = 0;
                                    qf.printStatus = 0;
                                    qf.EmailStatus = 0;
                                    qf.faxStatus = 0;
                                    qf.Pages = 0;
                                    qf.Name = string.Empty;
                                    qf.Address = string.Empty;
                                    qf.IsSSN = false;
                                    qf.IsRevised = string.Empty;
                                    qf.isFromClient = true;
                                    qf.IsPublic = true;
                                    qf.UpdatedDate = null;
                                }
                            }
                            #region --- DOCUMENT GENERATE PROCESS ---
                            string filetype = "pdf";
                            string attyName = (ConvertToString(objOrderDetail.FirstName).Trim()) + " " + (ConvertToString(objOrderDetail.LastName).Trim());
                            bool displaySSN = !string.IsNullOrEmpty(objOrderDetail.SSN) ? true : false;
                            qf.BillFirm = objOrderDetail.BillingFirmId;
                            qf.ClaimNo = objOrderDetail.BillingClaimNo;
                            qf.AttyName = attyName;
                            RecordType = Convert.ToInt32(part.RecType);
                            if (qf.DocPath.Contains("Subpoenas"))
                            {
                                string[] dname = Convert.ToString(qf.DocName).Split('_');
                                qf.DocName = qf.DocName.ToString().Replace(dname[0] + "_" + dname[1] + "_", "");
                            }
                            else
                            {
                                if (qf.DocName.ToString().Split('_').Length == 3)
                                    qf.DocName = qf.DocName.ToString().Split('_')[2].ToUpper();
                                else if (qf.DocName.ToString().Split('_').Length == 4)
                                    qf.DocName = qf.DocName.ToString().Split('_')[3].ToUpper();
                                else
                                    qf.DocName = qf.DocName.ToString().Split('_')[1].ToUpper();
                            }
                            string[] folders = qf.DocPath.ToString().Split('>');
                            var pdt = GetDocumentType(qf.DocName, folders[0].ToString());
                            string query = string.Empty;
                            if (pdt == QueryType.Common)
                            {
                                bool isMichigan = false;
                                bool isRush = false;
                                if (folders.Contains("Michigan"))
                                    isMichigan = true;
                                if (qf.DocName.Contains("RUSH"))
                                    isRush = true;
                                if (folders.Contains("Custodian Letters") || folders.Contains("Subpoenas"))
                                    filetype = "pdf";
                                query = GetQueryByQueryTypeId(1, "Query");
                                if (!string.IsNullOrEmpty(query))
                                {
                                    query = ReplaceOrderPartNo(query, OrderNo, Convert.ToString(part.PartNo));
                                    if (isMichigan && isRush)
                                        query = query.Replace("--Michigan and Rush", " ,DATENAME(DW, DATEADD(day,7,GETDATE())) as [DayName],DATENAME(MM, DATEADD(day,7,GETDATE())) + RIGHT(CONVERT(VARCHAR(12), DATEADD(day,7,GETDATE()), 107), 9) AS BigDate ");
                                    else if (isMichigan && !isRush)
                                        query = query.Replace("--Michigan and Rush", " ,DATENAME(DW, DATEADD(day,14,GETDATE())) as [DayName],DATENAME(MM, DATEADD(day,14,GETDATE())) + RIGHT(CONVERT(VARCHAR(12), DATEADD(day,14,GETDATE()), 107), 9) AS BigDate ");
                                }
                            }
                            else if (pdt == QueryType.Confirmation)
                            {
                                query = GetQueryByQueryTypeId(2, "Query");
                                if (!string.IsNullOrEmpty(query))
                                    query = ReplaceOrderPartNo(query, OrderNo, Convert.ToString(part.PartNo)).Replace("%%PartCnt%%", Convert.ToString(partList.Count));
                            }
                            else if (pdt == QueryType.FaceSheet)
                            {
                                query = GetQueryByQueryTypeId(14, "Query");
                                if (!string.IsNullOrEmpty(query))
                                {
                                    query = ReplaceOrderPartNo(query, OrderNo, Convert.ToString(part.PartNo)).Replace("%%ATTYNO%%", objOrderDetail.AttyID);
                                    if (!displaySSN)
                                        query = ReplaceSSN(query);
                                }
                            }
                            else if (pdt == QueryType.StatusLetters)
                            {
                                query = GetQueryByQueryTypeId(4, "Query");
                                if (!string.IsNullOrEmpty(query))
                                {
                                    query = ReplaceOrderPartNo(query, OrderNo, Convert.ToString(part.PartNo));
                                    if (!displaySSN)
                                        query = ReplaceSSN(query);
                                }
                            }
                            else if (pdt == QueryType.Waiver)
                            {
                                query = GetQueryByQueryTypeId(5, "Query");
                                if (!string.IsNullOrEmpty(query))
                                {
                                    query = ReplaceOrderPartNo(query, OrderNo, Convert.ToString(part.PartNo));
                                    if (!displaySSN)
                                        query = ReplaceSSN(query);
                                }
                            }
                            else if (pdt == QueryType.Interrogatories)
                            {
                                query = GetQueryByQueryTypeId(7, "Query");
                                if (!string.IsNullOrEmpty(query))
                                {
                                    query = ReplaceOrderPartNo(query, OrderNo, Convert.ToString(part.PartNo));
                                    if (!displaySSN)
                                        query = ReplaceSSN(query);
                                }
                            }
                            else if (pdt == QueryType.TargetSheets)
                            {
                                query = GetQueryByQueryTypeId(8, "Query");
                                if (!string.IsNullOrEmpty(query))
                                    query = ReplaceOrderPartNo(query, OrderNo, Convert.ToString(part.PartNo));
                            }
                            else if (pdt == QueryType.StatusProgressReports)
                            {
                                query = GetQueryByQueryTypeId(9, "Query");
                                if (!string.IsNullOrEmpty(query))
                                    query = ReplaceOrderPartNo(query, OrderNo, Convert.ToString(part.PartNo)).Replace("%%PartCnt%%", Convert.ToString(partList.Count));
                            }
                            else if (pdt == QueryType.CerticicationNOD)
                            {
                                query = GetQueryByQueryTypeId(10, "Query");
                                if (!string.IsNullOrEmpty(query))
                                    query = ReplaceOrderPartNo(query, OrderNo, Convert.ToString(part.PartNo));
                            }
                            else if (pdt == QueryType.AttorneyOfRecords)
                            {
                                query = GetQueryByQueryTypeId(6, "Query");
                                if (!string.IsNullOrEmpty(query))
                                    query = ReplaceOrderPartNo(query, OrderNo, Convert.ToString(part.PartNo));
                            }
                            else if (pdt == QueryType.CollectionLetters)
                            {
                                query = GetQueryByQueryTypeId(11, "Query");
                                if (!string.IsNullOrEmpty(query))
                                    query = ReplaceOrderPartNo(query, OrderNo, Convert.ToString(part.PartNo));
                            }
                            else if (pdt == QueryType.Notices)
                            {
                                query = GetQueryByQueryTypeId(12, "Query");
                                if (!string.IsNullOrEmpty(query))
                                {
                                    query = ReplaceOrderPartNo(query, OrderNo, Convert.ToString(part.PartNo));
                                    if (!displaySSN)
                                        query = ReplaceSSN(query);
                                }
                            }
                            else if (pdt == QueryType.AttorneyForms)
                            {
                                query = GetQueryByQueryTypeId(13, "Query");
                                if (!string.IsNullOrEmpty(query))
                                {
                                    query = ReplaceOrderPartNo(query, OrderNo, Convert.ToString(part.PartNo)).Replace("%%ATTYNO%%", objOrderDetail.AttyID);
                                    if (!displaySSN)
                                        query = ReplaceSSN(query);
                                }
                            }
                            MemoryStream ms = new MemoryStream();
                            var dtQueryList = ExecuteSQLQuery(query);


                            string subquery = string.Empty;
                            var dtsubQuery = new DataTable();
                            StringBuilder partInfo = new StringBuilder();
                            StringBuilder partInfo2 = new StringBuilder();

                            if (pdt == QueryType.Confirmation || pdt == QueryType.TargetSheets || pdt == QueryType.StatusProgressReports)
                            {
                                subquery = GetQueryByQueryTypeId(Convert.ToInt32(pdt), "SubQuery");
                                if (!string.IsNullOrEmpty(subquery))
                                {
                                    subquery = ReplaceOrderPartNo(subquery, OrderNo, Convert.ToString(part.PartNo));
                                    if (!displaySSN)
                                        subquery = ReplaceSSN(subquery);
                                    dtsubQuery = ExecuteSQLQuery(subquery);
                                    partInfo.Append("_____________________________________________________________________________\n\r");

                                    for (int a = 0; a < dtsubQuery.Rows.Count; a++)
                                    {
                                        string partInfoText;
                                        if (pdt == QueryType.Confirmation && qf.DocName.ToUpper().Equals("ORDER CONFIRMATION.DOC"))
                                            partInfoText = Convert.ToString(dtsubQuery.Rows[a]["PartInfo1"]).Replace("scopehere", Convert.ToString(dtsubQuery.Rows[a]["scope"]));
                                        else
                                            partInfoText = Convert.ToString(dtsubQuery.Rows[a]["PartInfo1"]).Replace("scopehere", "");
                                        partInfo.Append(partInfoText + "\n\r");
                                        partInfo2.Append(Convert.ToString(dtsubQuery.Rows[a]["LocationHeader"]) + "\n");
                                    }
                                }
                                var dt2 = dtQueryList;
                                DataColumn dc2 = new DataColumn("PartInfo", typeof(string));
                                DataColumn dc3 = new DataColumn("PartInfo2", typeof(string));
                                dc2.AllowDBNull = true;
                                dc3.AllowDBNull = true;
                                dt2.Columns.Add(dc2);
                                dt2.Columns.Add(dc3);
                                for (int j = 0; j < dt2.Rows.Count; j++)
                                {
                                    DataRow dr = dt2.Rows[j];
                                    dr["PartInfo"] = partInfo.ToString();
                                    dr["PartInfo2"] = partInfo2.ToString();
                                }
                            }
                            else if (pdt == QueryType.Waiver)
                            {
                                StringBuilder locationInfo = new StringBuilder();
                                subquery = GetQueryByQueryTypeId(5, "SubQuery");
                                if (!string.IsNullOrEmpty(subquery))
                                {
                                    subquery = ReplaceOrderPartNo(subquery, OrderNo, Convert.ToString(part.PartNo));
                                    dtsubQuery = ExecuteSQLQuery(subquery);
                                    for (int b = 0; b < dtsubQuery.Rows.Count; b++)
                                        locationInfo.Append(Convert.ToString(dtsubQuery.Rows[b]["Location1"]) + '\n');

                                }
                                var dt3 = dtQueryList;
                                dt3.Columns.Add(new DataColumn("Selected_Part", typeof(string)));
                                dt3.Columns.Add(new DataColumn("Location1", typeof(string)));
                                for (int k = 0; k < dt3.Rows.Count; k++)
                                {
                                    DataRow dr4 = dt3.Rows[k];
                                    dr4["Selected_Part"] = part.PartNo;
                                    dr4["Location1"] = locationInfo;
                                }
                            }
                            else if (pdt == QueryType.CerticicationNOD)
                            {
                                StringBuilder attyInfo = new StringBuilder();
                                attyInfo.Append('\n');
                                subquery = GetQueryByQueryTypeId(10, "SubQuery");
                                if (!string.IsNullOrEmpty(subquery))
                                {
                                    subquery = ReplaceOrderPartNo(subquery, OrderNo, Convert.ToString(part.PartNo));
                                    dtsubQuery = ExecuteSQLQuery(subquery);
                                    for (int c = 0; c < dtsubQuery.Rows.Count; c++)
                                        attyInfo.Append(Convert.ToString(dtsubQuery.Rows[c]["Attorneys"]) + "\n");
                                }
                                DataTable dt4 = dtQueryList;
                                dt4.Columns.Add(new DataColumn("Attorneys", typeof(string)));
                                for (int l = 0; l < dt4.Rows.Count; l++)
                                {
                                    DataRow dr5 = dt4.Rows[l];
                                    dr5["Attorneys"] = attyInfo;
                                }
                            }
                            else if (pdt == QueryType.AttorneyOfRecords)
                            {
                                subquery = GetQueryByQueryTypeId(6, "SubQuery");
                                string[] strQuery = null;
                                StringBuilder attyInfo2 = new StringBuilder();
                                attyInfo2.Append('\n');
                                if (!string.IsNullOrEmpty(subquery))
                                {
                                    strQuery = subquery.Split(new string[] { "--Split--" }, StringSplitOptions.RemoveEmptyEntries);
                                    subquery = ReplaceOrderPartNo(strQuery[0], OrderNo, Convert.ToString(part.PartNo));
                                    dtsubQuery = ExecuteSQLQuery(subquery);
                                    for (int d = 0; d < dtsubQuery.Rows.Count; d++)
                                        attyInfo2.Append(System.Runtime.CompilerServices.RuntimeHelpers.GetObjectValue(Convert.ToString(dtsubQuery.Rows[d]["AttyInfo"])));

                                    attyInfo2.Append('\n');
                                    subquery = ReplaceOrderPartNo(strQuery[1], OrderNo, Convert.ToString(part.PartNo));
                                    dtsubQuery = ExecuteSQLQuery(subquery);
                                    for (int e = 0; e < dtsubQuery.Rows.Count; e++)
                                        attyInfo2.Append(Convert.ToString(dtsubQuery.Rows[e]["AttyInfo"]) + "\n");
                                }
                                DataTable dt5 = dtQueryList;
                                dt5.Columns.Add(new DataColumn("AttyInfo", typeof(string)));
                                for (int m = 0; m < dt5.Rows.Count; m++)
                                {
                                    DataRow dr6 = dt5.Rows[m];
                                    dr6["AttyInfo"] = attyInfo2;
                                }
                            }
                            else if (pdt == QueryType.AttorneyForms)
                            {
                                StringBuilder locationInfo2 = new StringBuilder();
                                subquery = GetQueryByQueryTypeId(13, "SubQuery");
                                if (!string.IsNullOrEmpty(subquery))
                                {
                                    subquery = ReplaceOrderPartNo(subquery, OrderNo, Convert.ToString(part.PartNo)).Replace("%%ATTYNO%%", objOrderDetail.AttyID);
                                    dtsubQuery = ExecuteSQLQuery(subquery);
                                    for (int f = 0; f < dtsubQuery.Rows.Count; f++)
                                        locationInfo2.Append(Convert.ToString(dtsubQuery.Rows[f]["Location1"]) + "\n");
                                }
                                DataTable dt6 = dtQueryList;
                                dt6.Columns.Add(new DataColumn("Selected_Part", typeof(string)));
                                dt6.Columns.Add(new DataColumn("Location1", typeof(string)));
                                for (int n = 0; n < dt6.Rows.Count; n++)
                                {
                                    DataRow dr7 = dt6.Rows[n];
                                    dr7["Location1"] = locationInfo2;
                                }
                            }
                            Aspose.Words.License license = new Aspose.Words.License();
                            license.SetLicense("Aspose.Words.lic");
                            string filePath = Path.Combine(documentRoot, qf.DocPath.ToString().Trim().Replace(">", "\\"), qf.DocName.Trim());
                            Aspose.Words.Document doc = new Aspose.Words.Document(filePath);

                            try
                            {
                                DataColumnCollection columns = dtQueryList.Columns;
                                if (columns.Contains("Part_Scope"))
                                {
                                    foreach (DataRow dr in dtQueryList.Rows)
                                    {
                                        string str = Convert.ToString(dr["Part_Scope"]);
                                        if (!string.IsNullOrEmpty(str))
                                            dr["Part_Scope"] = ConvertScopeHTMLToString(str);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Log.ServicLog(ex.ToString());
                            }

                            doc.MailMerge.Execute(dtQueryList);
                            doc.Save(ms, Aspose.Words.SaveFormat.Pdf);

                            if (RecordType > 0 && !string.IsNullOrEmpty(objOrderDetail.BillingFirmId))
                            {
                                string TStamp = DateTime.Now.ToString("yyyyMMddHHmm");
                                string _storageRoot = string.Empty;
                                string FilePath = string.Empty;
                                if (objOrderDetail.BillingFirmId == "GRANCO01")
                                {
                                    #region GRANCO01
                                    _storageRoot = ConfigurationManager.AppSettings["GrangeRoot"].ToString();
                                    DirectoryInfo dis = new DirectoryInfo(_storageRoot);
                                    if (!dis.Exists)
                                        dis.Create();
                                    FilePath = _storageRoot + string.Format("{0}-{1}-{2}-{3}", objOrderDetail.BillingClaimNo, TStamp, OrderNo, part.PartNo + "." + filetype);
                                    int count = 1;
                                    string fileNameOnly = Path.GetFileNameWithoutExtension(FilePath);
                                    string extension = Path.GetExtension(FilePath);
                                    string path = Path.GetDirectoryName(FilePath);
                                    string newFullPath = FilePath;

                                    while (File.Exists(newFullPath))
                                    {
                                        string tempFileName = string.Format("{0}({1})", fileNameOnly, count++);
                                        newFullPath = Path.Combine(path, tempFileName + extension);
                                    }

                                    FileStream file = new FileStream(newFullPath, FileMode.Create, FileAccess.Write);
                                    ms.WriteTo(file);
                                    file.Close();
                                    #endregion
                                }
                                if (objOrderDetail.BillingFirmId == "HANOAA01")
                                {
                                    #region HANOAA01
                                    _storageRoot = ConfigurationManager.AppSettings["HanoverRoot"].ToString();

                                    System.IO.DirectoryInfo dis = new System.IO.DirectoryInfo(_storageRoot);
                                    if (!dis.Exists)
                                        dis.Create();

                                    FilePath = _storageRoot + string.Format("{0}_{1}_{2}_{3}-{4}", objOrderDetail.BillingClaimNo, attyName, TStamp, OrderNo, part.PartNo + "." + filetype);
                                    int count = 1;
                                    string fileNameOnly = Path.GetFileNameWithoutExtension(FilePath);
                                    string extension = Path.GetExtension(FilePath);
                                    string path = Path.GetDirectoryName(FilePath);
                                    string newFullPath = FilePath;
                                    while (File.Exists(newFullPath))
                                    {
                                        string tempFileName = string.Format("{0}({1})", fileNameOnly, count++);
                                        newFullPath = Path.Combine(path, tempFileName + extension);
                                    }

                                    FileStream file = new FileStream(newFullPath, FileMode.Create, FileAccess.Write);
                                    ms.WriteTo(file);
                                    file.Close();
                                    #endregion
                                }
                            }
                            string outputFileName = "printforms";
                            outputFileName = Path.GetFileNameWithoutExtension(Convert.ToString(qf.DocName).Replace(" ", "-")) + "." + filetype;
                            Guid gid = Guid.NewGuid();
                            if (part.PartNo > 0 || (pdt == QueryType.AttorneyForms || pdt == QueryType.Confirmation || pdt == QueryType.Waiver))
                            {
                                if (pdt == QueryType.AttorneyForms)
                                {
                                    string attorney = "";
                                    if (qf.DocName.ToString().Split('_').Length == 4)
                                        attorney = qf.DocName.ToString().Split('_')[2].ToUpper();
                                    outputFileName = string.Format("{0}-{1}-{2}-{3}{4}", new object[] { OrderNo, part.PartNo, attorney, Path.GetFileNameWithoutExtension(filePath).Replace(" ", "-"), "." + filetype }).Replace(",", "-");
                                }
                                else
                                    outputFileName = string.Format("{0}-{1}-{2}{3}", new object[] { OrderNo, part.PartNo, Path.GetFileNameWithoutExtension(filePath).Replace(" ", "-"), "." + filetype }).Replace(",", "-");

                            }

                            try
                            {
                                if (!Directory.Exists(Path.Combine(ConfigurationManager.AppSettings["UploadRoot"].ToString(), OrderNo.ToString(), part.PartNo.ToString())))
                                    Directory.CreateDirectory(Path.Combine(ConfigurationManager.AppSettings["UploadRoot"].ToString(), OrderNo.ToString(), part.PartNo.ToString()));
                                using (FileStream file2 = new FileStream(Path.Combine(ConfigurationManager.AppSettings["UploadRoot"].ToString(), OrderNo.ToString(), part.PartNo.ToString(), gid + "." + filetype), FileMode.Create, FileAccess.Write))
                                {
                                    ms.WriteTo(file2);
                                    file2.Close();
                                }
                            }
                            catch (Exception ex)
                            {
                                Log.ServicLog(ex.ToString());
                            }
                            finally
                            {
                                if (part.isSup != true)
                                {
                                    SqlParameter[] sqlParam = {  new SqlParameter("OrderId", (object)OrderNo ?? (object)DBNull.Value)
                                                 ,new SqlParameter("PartNo",(object)part.PartNo?? (object)DBNull.Value)
                                                 ,new SqlParameter("FileName", (object)outputFileName.Replace("_", "-") ?? (object)DBNull.Value)
                                                 ,new SqlParameter("FileTypeId",(object)(part.isCreateAuthSup==true?(int)FileType.Authorization_Blank: qf.FileTypeID)?? (object)DBNull.Value)
                                                 ,new SqlParameter("IsPublic",(object)Convert.ToBoolean(true)?? (object)DBNull.Value)
                                                 ,new SqlParameter("RecordTypeId",(object) Convert.ToInt32(0) ?? (object)DBNull.Value)
                                                 ,new SqlParameter("FileDiskName", (object)gid+"."+filetype ?? (object)DBNull.Value)
                                                 ,new SqlParameter("PageNo", (object)Convert.ToInt32(0) ?? (object)DBNull.Value)
                                                 ,new SqlParameter("CreatedBy", (object)new Guid("7ABB0EFB-88A9-4699-B359-7F17216A8258")?? (object)DBNull.Value)
                                                 };
                                    _repository.ExecuteSQL<int>("InsertFile", sqlParam).FirstOrDefault();
                                }
                            }



                            #endregion

                            // IF PART IS Create Subpoena then no note should be added. Only Subpoena is fine
                            // NO Documents Should be added.

                            #region  location

                            SqlParameter[] paramLocationList = {
                                                            new SqlParameter("OrderNo", (object)OrderNo ?? (object)DBNull.Value)
                                                            ,new SqlParameter("PartNo", (object)part.PartNo ?? (object)DBNull.Value)
                                                        };
                            var location = _repository.ExecuteSQL<SendRequestEntity>("GetSendRequestDetail", paramLocationList).FirstOrDefault();

                            //SqlParameter[] param = { new SqlParameter("LocationId", (object)part.LocID ?? (object)DBNull.Value) };
                            //var location = _repository.ExecuteSQL<LocationEntity>("GetLocationById", param).FirstOrDefault();
                            if (part.isCreateAuthSup == true && part.isAuth == true)
                            {
                                var AsgnTo = "AUTHSS";
                                var CallBack = DateTime.Now.AddDays(14);

                                // THIS LOGIC WILLNOT APPLY WHEN CREATING NEW ORDER
                                //if ((part.ChngDate == Convert.ToDateTime("1900-01-01 00:00:00")) || (pt.ChngDate == null) || (pt.ChngDate <= DateTime.Now.AddYears(-2)))
                                //{
                                //    AsgnTo = "FSTCAL";
                                //    CallBack = Convert.ToDateTime(part.CallBack);
                                //}

                                //DbAccess.UpdateOrderPart(OrderNo, PartNo, AsgnTo, CallBack);
                                UpdatePart(OrderNo, part.PartNo, AsgnTo, DateTime.Now.AddDays(14));
                            }
                            else if (location != null && !string.IsNullOrEmpty(location.SendRequest))
                            {
                                string[] strsplit = location.SendRequest.Split(',');
                                foreach (string action in strsplit)
                                {



                                    if (action == "0" || action == "2")
                                    {
                                        #region Email - FAX
                                        MemoryStream[] msFile = new MemoryStream[10];
                                        List<string> fileNames = new List<string>();
                                        var pdfDocName = qf.DocName.Replace("doc", "pdf").Replace("DOC", "pdf");
                                        GetAttachFileFromDB(OrderNo, part.PartNo, ref fileNames, ref msFile);
                                        EmailDetails ed = new EmailDetails();
                                        ed.Caption = objOrderDetail.Caption;
                                        ed.CauseNumber = objOrderDetail.CauseNo;
                                        ed.PatientName = objOrderDetail.PatientName;
                                        string OrderAttorney = objOrderDetail.OrderingAttorney;
                                        string acctrep = objOrderDetail.AcctRep;

                                        SqlParameter[] sqlparam = { new SqlParameter("AccntRep", (object)acctrep ?? (object)DBNull.Value) };
                                        var objAccExecutive = _repository.ExecuteSQL<AccntRepDetails>("GetAccntRepDetail", sqlparam).FirstOrDefault();

                                        if (objAccExecutive != null)
                                        {
                                            ed.AccExeName = objAccExecutive.Name;
                                            ed.AccExeEmail = objAccExecutive.Email;
                                        }
                                        else
                                        {
                                            ed.AccExeName = "Josh Sanford";
                                            ed.AccExeEmail = "Josh.Sanford@axiomcopy.com";
                                        }
                                        string additionalEmail = string.Empty;
                                        if (pdt == QueryType.Confirmation)
                                        {

                                            SqlParameter[] sqlparameter = { new SqlParameter("orderId", (object)OrderNo ?? (object)DBNull.Value) };
                                            var notificationList = _repository.ExecuteSQL<NotificationEmailEntity>("GetAssistantContactNotificationInformationByOrderId", sqlparameter).ToList();
                                            if (notificationList != null && notificationList.Count > 0)
                                            {
                                                notificationList = notificationList.Where(x => x.AttyID == OrderAttorney.Trim() && x.OrderConfirmation == true).ToList();
                                                if (notificationList.Count > 0)
                                                {
                                                    foreach (var item in notificationList)
                                                        additionalEmail += item.AssistantEmail + ",";

                                                    additionalEmail = additionalEmail.Trim(',');
                                                }
                                            }

                                        }
                                        if (action == "2")
                                        {
                                            if (string.IsNullOrEmpty(location.Email))
                                            {
                                                string subject = "[Axiom Automation] Email Not Found for Order No " + OrderNo + "-" + part.PartNo + " Location ID : " + location.LocID;
                                                string body = "We have not found Email for " + OrderNo + "-" + part.PartNo;
                                                body += "\n Location ID : " + location.LocID;
                                                body += "\n Location Name : " + location.Name1 + " " + location.Name2;
                                                EmailHelper.Email.Send("j.alspaugh@axiomcopy.com", body, subject, "tejaspadia@gmail.com", "");
                                            }
                                            else
                                            {
                                                EmailDocument(doc, fileNames, emailToPatientAtty, location.Email, orderConfirmEmails, msFile, ed, additionalEmail, true, pdfDocName);
                                                string partNotes = string.Empty;
                                                CreateNoteString(OrderNo, part.PartNo, "Input or Sent via Email.", "SYSTEM", ref partNotes, false, false);
                                            }
                                        }
                                        if (action == "0")
                                        {
                                            string faxNumber = location.AreaCode3 + location.FaxNo;
                                            faxNumber = faxNumber.Replace("-", "").Replace(" ", "");
                                            if (string.IsNullOrEmpty(faxNumber))
                                            {
                                                string subject = "[Axiom Automation] Fax Number Not Found for Order No " + OrderNo + "-" + part.PartNo + " Location ID : " + location.LocID;
                                                string body = "We have not found Fax Number for " + OrderNo + "-" + part.PartNo;
                                                body += "\n Location ID : " + location.LocID;
                                                body += "\n Location Name : " + location.Name1 + " " + location.Name2;
                                                EmailHelper.Email.Send("j.alspaugh@axiomcopy.com", body, subject, "tejaspadia@gmail.com", "");
                                            }
                                            else
                                            {
                                                FaxDocument(0, fileNames, faxNumber, location.Name1, msFile);
                                                string partNotes = string.Empty;
                                                CreateNoteString(OrderNo, part.PartNo, "Input or Sent via Fax.", "SYSTEM", ref partNotes, false, false);
                                            }
                                        }
                                        #endregion
                                    }
                                    else if (action == "1")
                                    {
                                        // MAIL VIA POST - PHYSICAL DOCUMENT TO BE SEND AFTER PRINT

                                        #region MAIL
                                        string MailPath = ConfigurationManager.AppSettings["MailPath"].ToString();
                                        if (!Directory.Exists(MailPath))
                                            Directory.CreateDirectory(MailPath);
                                        MemoryStream[] msFile = new MemoryStream[10];
                                        List<string> fileNames = new List<string>();
                                        GetAttachFileFromDB(OrderNo, part.PartNo, ref fileNames, ref msFile);
                                        int msCounter = 0;
                                        PdfSharp.Pdf.PdfDocument outputDocument = new PdfSharp.Pdf.PdfDocument();
                                        PdfSharp.Pdf.PdfDocument inputDocument = new PdfSharp.Pdf.PdfDocument();
                                        MemoryStream mstype;
                                        foreach (MemoryStream msCombine in msFile)
                                        {
                                            if (msCombine != null)
                                            {
                                                mstype = new MemoryStream();
                                                if (fileNames[msCounter].Contains(".jpg") || fileNames[msCounter].Contains(".jepg") || fileNames[msCounter].Contains(".bmp") || fileNames[msCounter].Contains(".png"))
                                                {
                                                    PdfSharp.Pdf.PdfDocument pdfsharpdoc = new PdfSharp.Pdf.PdfDocument();
                                                    pdfsharpdoc.Pages.Add(new PdfSharp.Pdf.PdfPage());

                                                    PdfSharp.Drawing.XGraphics xgr = PdfSharp.Drawing.XGraphics.FromPdfPage(pdfsharpdoc.Pages[0]);
                                                    PdfSharp.Drawing.XImage img = PdfSharp.Drawing.XImage.FromStream(msCombine);

                                                    xgr.DrawImage(img, 0, 0);
                                                    pdfsharpdoc.Save(mstype);
                                                    pdfsharpdoc.Close();

                                                    inputDocument = PdfSharp.Pdf.IO.PdfReader.Open(mstype, PdfSharp.Pdf.IO.PdfDocumentOpenMode.Import);
                                                    foreach (PdfSharp.Pdf.PdfPage page in inputDocument.Pages)
                                                        outputDocument.AddPage(page);
                                                }
                                                else if (fileNames[msCounter].Contains(".doc") || fileNames[msCounter].Contains(".docx"))
                                                {
                                                    Aspose.Words.Document docWord = new Aspose.Words.Document(msCombine);
                                                    docWord.Save(mstype, Aspose.Words.SaveFormat.Pdf);
                                                    inputDocument = PdfSharp.Pdf.IO.PdfReader.Open(mstype, PdfSharp.Pdf.IO.PdfDocumentOpenMode.Import);
                                                    foreach (PdfSharp.Pdf.PdfPage page in inputDocument.Pages)
                                                        outputDocument.AddPage(page);
                                                }
                                                else if (fileNames[msCounter].Contains(".html"))
                                                {

                                                }
                                                else
                                                {
                                                    inputDocument = PdfSharp.Pdf.IO.PdfReader.Open(msCombine, PdfSharp.Pdf.IO.PdfDocumentOpenMode.Import);
                                                    foreach (PdfSharp.Pdf.PdfPage page in inputDocument.Pages)
                                                        outputDocument.AddPage(page);
                                                }
                                            }
                                            msCounter++;
                                        }
                                        string FilePath = MailPath + string.Format("{0}-{1}", OrderNo.ToString(), part.PartNo.ToString() + ".pdf");
                                        int count = 1;
                                        string fileNameOnly = Path.GetFileNameWithoutExtension(FilePath);
                                        string extension = Path.GetExtension(FilePath);
                                        string path = Path.GetDirectoryName(FilePath);
                                        string newFullPath = FilePath;
                                        while (File.Exists(newFullPath))
                                        {
                                            string tempFileName = string.Format("{0}({1})", fileNameOnly, count++);
                                            newFullPath = Path.Combine(path, tempFileName + extension);
                                        }
                                        using (FileStream file2 = new FileStream(newFullPath, FileMode.Create, FileAccess.Write))
                                        {
                                            if (outputDocument.PageCount > 0)
                                                outputDocument.Save(file2);
                                            file2.Close();
                                        }
                                        string strNote = "Input or Sent via sent via Mail.";
                                        string partNotes = string.Empty;
                                        CreateNoteString(OrderNo, part.PartNo, strNote, "SYSTEM", ref partNotes, false, false);


                                        #endregion MAIL

                                    }
                                    else if (action == "3" || action == "4") //UPLOAD & PROCESS SERVER
                                    {
                                        // UTILRE : In Office Requests
                                        // REQUES : REQUESTS TO BE SENT
                                        isProcessServer = true;
                                        string AsgnTo = action == "3" ? "UTILRE" : "REQUES";
                                        SqlParameter[] SQLparam = {
                                                    new SqlParameter("OrderNo", (object)OrderNo ?? (object)DBNull.Value)
                                                    ,new SqlParameter("PartNo", (object)part.PartNo ?? (object)DBNull.Value)
                                                    ,new SqlParameter("AsgnTo", (object)AsgnTo ?? (object)DBNull.Value)
                                                };
                                        _repository.ExecuteSQL<int>("UpdateQuickFormOrderPart", SQLparam).FirstOrDefault();



                                    }
                                    else if (action == "5") //CERTIFIED MAIL
                                    {
                                        #region  Certified Mail
                                        string MailPath = ConfigurationManager.AppSettings["CertifiedPath"].ToString();
                                        if (!Directory.Exists(MailPath))
                                        {
                                            Directory.CreateDirectory(MailPath);
                                        }
                                        MemoryStream[] msFile = new MemoryStream[10];
                                        List<string> fileNames = new List<string>();
                                        GetAttachFileFromDB(OrderNo, part.PartNo, ref fileNames, ref msFile);
                                        int msCounter = 0;
                                        PdfSharp.Pdf.PdfDocument outputDocument = new PdfSharp.Pdf.PdfDocument();
                                        PdfSharp.Pdf.PdfDocument inputDocument = new PdfSharp.Pdf.PdfDocument();
                                        MemoryStream mstype;

                                        foreach (MemoryStream msCombine in msFile)
                                        {
                                            if (msCombine != null)
                                            {
                                                mstype = new MemoryStream();
                                                if (fileNames[msCounter].Contains(".jpeg") || fileNames[msCounter].Contains(".jpg") || fileNames[msCounter].Contains(".bmp") || fileNames[msCounter].Contains(".png"))
                                                {
                                                    PdfSharp.Pdf.PdfDocument pdfsharpdoc = new PdfSharp.Pdf.PdfDocument();
                                                    pdfsharpdoc.Pages.Add(new PdfSharp.Pdf.PdfPage());

                                                    PdfSharp.Drawing.XGraphics xgr = PdfSharp.Drawing.XGraphics.FromPdfPage(pdfsharpdoc.Pages[0]);
                                                    PdfSharp.Drawing.XImage img = PdfSharp.Drawing.XImage.FromStream(msCombine);

                                                    xgr.DrawImage(img, 0, 0);
                                                    pdfsharpdoc.Save(mstype);
                                                    pdfsharpdoc.Close();

                                                    inputDocument = PdfSharp.Pdf.IO.PdfReader.Open(mstype, PdfSharp.Pdf.IO.PdfDocumentOpenMode.Import);
                                                    foreach (PdfSharp.Pdf.PdfPage page in inputDocument.Pages)
                                                        outputDocument.AddPage(page);
                                                }
                                                else if (fileNames[msCounter].Contains(".doc") || fileNames[msCounter].Contains(".docx"))
                                                {
                                                    Aspose.Words.Document docWord = new Aspose.Words.Document(msCombine);
                                                    docWord.Save(mstype, Aspose.Words.SaveFormat.Pdf);
                                                    inputDocument = PdfSharp.Pdf.IO.PdfReader.Open(mstype, PdfSharp.Pdf.IO.PdfDocumentOpenMode.Import);
                                                    foreach (PdfSharp.Pdf.PdfPage page in inputDocument.Pages)
                                                        outputDocument.AddPage(page);
                                                }
                                                else if (fileNames[msCounter].Contains(".html"))
                                                {
                                                }
                                                else
                                                {
                                                    inputDocument = PdfSharp.Pdf.IO.PdfReader.Open(msCombine, PdfSharp.Pdf.IO.PdfDocumentOpenMode.Import);
                                                    foreach (PdfSharp.Pdf.PdfPage page in inputDocument.Pages)
                                                        outputDocument.AddPage(page);
                                                }
                                            }
                                            msCounter++;
                                        }

                                        string FilePath = MailPath + string.Format("{0}-{1}", OrderNo.ToString(), part.PartNo.ToString() + ".pdf");
                                        int count = 1;
                                        string fileNameOnly = Path.GetFileNameWithoutExtension(FilePath);
                                        string extension = Path.GetExtension(FilePath);
                                        string path = Path.GetDirectoryName(FilePath);
                                        string newFullPath = FilePath;
                                        while (File.Exists(newFullPath))
                                        {
                                            string tempFileName = string.Format("{0}({1})", fileNameOnly, count++);
                                            newFullPath = Path.Combine(path, tempFileName + extension);
                                        }
                                        using (FileStream file2 = new FileStream(newFullPath, FileMode.Create, FileAccess.Write))
                                        {
                                            if (outputDocument.PageCount > 0)
                                            {
                                                outputDocument.Save(file2);
                                            }
                                            file2.Close();
                                        }

                                        string partNotes = string.Empty;
                                        CreateNoteString(OrderNo, part.PartNo, "Input or Sent via Certified Mail.", "SYSTEM", ref partNotes, false, false);
                                        #endregion
                                        SqlParameter[] sqlparam = { new SqlParameter("OrderId",(object)OrderNo ??(object)DBNull.Value),
                                                        new SqlParameter("PatNo",(object)part.PartNo ??(object)DBNull.Value)};
                                        _repository.ExecuteSQL<int>("InsertMiscChargesForCertifiedMail", sqlparam).FirstOrDefault();
                                    }

                                }

                                if (!isProcessServer)
                                {
                                    var AsgnTo = "CBLIST";
                                    var CallBack = DateTime.Now.AddDays(14);

                                    // THIS LOGIC WILLNOT APPLY WHEN CREATING NEW ORDER
                                    //if ((part.ChngDate == Convert.ToDateTime("1900-01-01 00:00:00")) || (pt.ChngDate == null) || (pt.ChngDate <= DateTime.Now.AddYears(-2)))
                                    //{
                                    //    AsgnTo = "FSTCAL";
                                    //    CallBack = Convert.ToDateTime(part.CallBack);
                                    //}

                                    //DbAccess.UpdateOrderPart(OrderNo, PartNo, AsgnTo, CallBack);
                                    UpdatePart(OrderNo, part.PartNo, AsgnTo, DateTime.Now.AddDays(14));

                                }
                                if (locationList != null)
                                {

                                    if (locationList.ReqAuthorization == true || locationList.FeeAmountSendRequest == true || locationList.LinkRequest == true)
                                    {
                                        // DbAccess.UpdateOrderPart(OrderNo, part.PartNo, "UTILRE", Convert.ToDateTime(pt.CallBack));
                                        UpdatePart(OrderNo, part.PartNo, "UTILRE", DateTime.Now.AddDays(14));


                                        string partNotes = string.Empty;
                                        CreateNoteString(OrderNo, part.PartNo, "Assign to In Office Request.", "SYSTEM", ref partNotes, false, false);
                                    }

                                }
                            }
                            else
                            {
                                if (locationList != null)
                                {

                                    if (locationList.ReqAuthorization == true || locationList.FeeAmountSendRequest == true || locationList.LinkRequest == true)
                                    {
                                        // DbAccess.UpdateOrderPart(OrderNo, part.PartNo, "UTILRE", Convert.ToDateTime(pt.CallBack));
                                        UpdatePart(OrderNo, part.PartNo, "UTILRE", DateTime.Now.AddDays(14));


                                        string partNotes = string.Empty;
                                        CreateNoteString(OrderNo, part.PartNo, "Assign to In Office Request.", "SYSTEM", ref partNotes, false, false);
                                    }

                                }
                            }

                            #endregion
                        }
                    }

                }
                catch (Exception ex)
                {
                    Log.ServicLog(ex.ToString());
                }
            });
        }

        public void UpdatePart(int OrderNo, int PartNo, string AsgnTo, DateTime CallBack)
        {
            SqlParameter[] paramUpdatePart = {
                                                            new SqlParameter("OrderNo", (object)OrderNo ?? (object)DBNull.Value)
                                                            ,new SqlParameter("PartNo", (object)PartNo ?? (object)DBNull.Value)
                                                            ,new SqlParameter("AsgnTo", (object)AsgnTo ?? (object)DBNull.Value)
                                                            ,new SqlParameter("CallBack", (object) CallBack ?? (object)DBNull.Value)
                                                        };
            var result = _repository.ExecuteSQL<int>("ServiceAxiomAutoUpdatePart", paramUpdatePart).FirstOrDefault();
        }


        private void FaxDocument(int Id, List<string> fileName, string fax, string name, MemoryStream[] msList)
        {
            int msCounter = 0;
            try
            {
                int counter = 0;
                string tempFile = AppDomain.CurrentDomain.BaseDirectory + "/Templates/MainFile.pdf";
                if (File.Exists(tempFile))
                    File.Delete(tempFile);

                PdfSharp.Pdf.PdfDocument outputDocument = new PdfSharp.Pdf.PdfDocument();
                PdfSharp.Pdf.PdfDocument inputDocument = new PdfSharp.Pdf.PdfDocument();

                MemoryStream msImage;
                foreach (MemoryStream ms in msList)
                {
                    if (ms != null)
                    {
                        msImage = new MemoryStream();
                        if (fileName[msCounter].Contains(".jpg") || fileName[msCounter].Contains(".bmp") || fileName[msCounter].Contains(".png"))
                        {
                            PdfSharp.Pdf.PdfDocument pdfsharpdoc = new PdfSharp.Pdf.PdfDocument();
                            pdfsharpdoc.Pages.Add(new PdfSharp.Pdf.PdfPage());

                            PdfSharp.Drawing.XGraphics xgr = PdfSharp.Drawing.XGraphics.FromPdfPage(pdfsharpdoc.Pages[0]);
                            PdfSharp.Drawing.XImage img = PdfSharp.Drawing.XImage.FromStream(ms);

                            xgr.DrawImage(img, 0, 0);
                            pdfsharpdoc.Save(msImage);
                            pdfsharpdoc.Close();

                            inputDocument = PdfSharp.Pdf.IO.PdfReader.Open(msImage, PdfSharp.Pdf.IO.PdfDocumentOpenMode.Import);
                            foreach (PdfSharp.Pdf.PdfPage page in inputDocument.Pages)
                                outputDocument.AddPage(page);
                        }
                        else if (fileName[msCounter].Contains(".doc") || fileName[msCounter].Contains(".docx"))
                        {

                            Aspose.Words.Document docWord = new Aspose.Words.Document(ms);
                            docWord.Save(msImage, Aspose.Words.SaveFormat.Pdf);
                            inputDocument = PdfSharp.Pdf.IO.PdfReader.Open(msImage, PdfSharp.Pdf.IO.PdfDocumentOpenMode.Import);
                            foreach (PdfSharp.Pdf.PdfPage page in inputDocument.Pages)
                                outputDocument.AddPage(page);
                        }
                        else if (fileName[msCounter].Contains(".html"))
                        {

                        }
                        else
                        {
                            inputDocument = PdfSharp.Pdf.IO.PdfReader.Open(ms, PdfSharp.Pdf.IO.PdfDocumentOpenMode.Import);
                            foreach (PdfSharp.Pdf.PdfPage page in inputDocument.Pages)
                                outputDocument.AddPage(page);
                        }
                    }
                    msCounter++;
                }
                if (outputDocument.PageCount > 0)
                    outputDocument.Save(tempFile);

                MemoryStream msSendFax = new MemoryStream();
                using (FileStream fs = new FileStream(tempFile, FileMode.Open, System.IO.FileAccess.Read))
                {
                    byte[] bytes = new byte[fs.Length];
                    fs.Read(bytes, 0, (int)fs.Length);
                    msSendFax.Write(bytes, 0, (int)fs.Length);
                }
                msSendFax.Position = 0;

                string base64String = Convert.ToBase64String(msSendFax.ToArray());
                string xmlFileName = this.GenrateXMLDocument(base64String, fileName[counter], fax, name);
                SendFax(xmlFileName);

            }
            catch (Exception ex)
            {
                Log.ServicLog(ex.ToString());
            }
        }
        private string GenrateXMLDocument(string base64String, string attachmentName, string faxNo, string name)
        {
            string GenrateXMLDocument = string.Empty;
            try
            {
                string senderName = ConfigurationManager.AppSettings["FaxSenderName"];
                string senderEmail = ConfigurationManager.AppSettings["FaxEmail"];
                StringBuilder str = new StringBuilder();
                str.Append("<?xml version='1.0' encoding='UTF-8'?>");
                str.Append("<schedule_fax>");
                str.Append("<max_tries>3</max_tries>");
                str.Append("<priority>2</priority>");
                str.Append("<try_interval>333</try_interval>");
                str.Append("<receipt>never</receipt>");
                str.Append("<receipt_attachment>none</receipt_attachment>");
                str.Append("<cover_page>");
                str.Append(" <enabled>false</enabled>");
                str.Append("</cover_page>");
                str.Append("<sender>");
                str.Append("<name>" + Convert.ToString(senderName).Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&apos;") + "</name>");
                str.Append("<email_address>" + Convert.ToString(senderEmail) + "</email_address>");
                str.Append("</sender>");
                str.Append("<recipient>");
                str.Append("<name>" + Convert.ToString(name).Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&apos;") + "</name>");
                str.Append("<fax_number>91" + Convert.ToString(faxNo) + "</fax_number>");
                str.Append(" </recipient>");
                str.Append("<attachment>");
                str.Append("<location>inline</location>");
                str.Append("<name>" + Convert.ToString(attachmentName) + "</name>");
                str.Append("<content_type>application/pdf</content_type>");
                str.Append("<content_transfer_encoding>base64</content_transfer_encoding>");
                str.Append("<content>");
                str.Append(base64String);
                str.Append("</content>");
                str.Append("</attachment>");
                str.Append("</schedule_fax>");
                string fileName = Path.GetTempFileName();
                using (StreamWriter file = new StreamWriter(fileName, false))
                {
                    file.WriteLine(str);
                }
                GenrateXMLDocument = fileName;
            }
            catch (Exception ex)
            {
                Log.ServicLog(ex.ToString());
            }
            return GenrateXMLDocument;
        }
        private void SendFax(string xmlFileName)
        {
            try
            {
                ServicePointManager.Expect100Continue = false;
                ServicePointManager.ServerCertificateValidationCallback = ((object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) => true);
                string url = ConfigurationManager.AppSettings["FaxURL"];
                string userName = ConfigurationManager.AppSettings["FaxUserName"];
                string Password = ConfigurationManager.AppSettings["FaxPasword"];
                WebRequest request = WebRequest.Create(url);
                request.Credentials = new NetworkCredential(userName, Password);
                request.Method = "POST";
                request.ContentType = "application/xml";
                string fileName = Path.GetTempFileName();
                FileInfo fileSize = new FileInfo(xmlFileName);
                int len = checked((int)fileSize.Length);
                request.ContentLength = (long)len;
                Stream dataStream = request.GetRequestStream();
                StreamReader textIn = new StreamReader(new FileStream(xmlFileName, FileMode.Open, FileAccess.Read));
                string TextLines = textIn.ReadToEnd();
                byte[] byteArray = Encoding.UTF8.GetBytes(TextLines);
                dataStream.Write(byteArray, 0, byteArray.Length);
                textIn.Close();
                dataStream.Close();
                WebResponse response = request.GetResponse();
                response.Close();
            }
            catch (Exception ex)
            {
                Log.ServicLog(ex.ToString());
            }
        }


        private void CreateNoteString(int OrderNo, int Partno, string note, string empId, ref string databaseNoteField, bool mrAttyNote = false, bool trimNote = false)
        {
            try
            {
                if (string.IsNullOrEmpty(note.Trim()))
                    return;
                string noteString = mrAttyNote ? "" : Environment.NewLine + Environment.NewLine + "[ {0} - {1} ]" + Environment.NewLine + "{2}";
                DateTime dt = DateTime.Now;
                if (trimNote)
                    note = note.Substring(0, 250);

                noteString = string.Format(noteString, dt.ToString(), empId, note.Trim(Environment.NewLine.ToCharArray()));

                if (string.IsNullOrEmpty(databaseNoteField) || mrAttyNote)
                    databaseNoteField = mrAttyNote ? noteString.Substring(0, 250) : noteString;
                else
                    databaseNoteField += noteString;

                var userId = new Guid("7ABB0EFB-88A9-4699-B359-7F17216A8258");

                SqlParameter[] noteparam = {   new SqlParameter("OrderId", (object)OrderNo?? (object)DBNull.Value)
                                          ,new SqlParameter("PartNo",(object)Partno?? (object)DBNull.Value)
                                          ,new SqlParameter("NotesClient",(object)note?? (object)DBNull.Value)
                                          ,new SqlParameter("NotesInternal",(object)note?? (object)DBNull.Value)
                                          ,new SqlParameter("UserId",(object)userId.ToString()?? (object)DBNull.Value)};
                _repository.ExecuteSQL<int>("InsertOrderNotes_QuickForm", noteparam).FirstOrDefault();

                SqlParameter[] sqlparam = {   new SqlParameter("OrderNo", (object)OrderNo?? (object)DBNull.Value)
                                          ,new SqlParameter("PartNo",(object)Partno?? (object)DBNull.Value)
                                          ,new SqlParameter("AsgnTo",(object)""?? (object)DBNull.Value)
                                          ,new SqlParameter("UpdatePartNotes",(object)1?? (object)DBNull.Value)
                                          ,new SqlParameter("Notes",(object)databaseNoteField?? (object)DBNull.Value)};
                _repository.ExecuteSQL<int>("UpdateQuickFormOrderPart", sqlparam).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Log.ServicLog(ex.ToString());
            }
        }
        private void GetAttachFileFromDB(int OrderNo, int partno, ref List<string> AttachFileName, ref MemoryStream[] msFile)
        {
            try
            {
                SqlParameter[] Sqlorderparam = { new SqlParameter("OrderNumber", (object)OrderNo ?? (object)DBNull.Value),
                                                new SqlParameter("PartNumber", (object)partno ?? (object)DBNull.Value)};
                var attachedFileList = _repository.ExecuteSQL<QuickFormDocumentAttachmentListEntity>("QuickFormGetAttachFileList", Sqlorderparam).ToList();
                if (attachedFileList != null && attachedFileList.Count > 0)
                {
                    int counter = AttachFileName.Count();
                    string uploadRoot = Path.Combine(ConfigurationManager.AppSettings["UploadRoot"].ToString());
                    foreach (var item in attachedFileList)
                    {
                        string finalPath = "";
                        int partNo = item.PartNo;
                        if (partNo <= 0)
                            finalPath = string.Format(@"{0}{1}\{2}", uploadRoot, OrderNo, item.FileDiskName);
                        else
                            finalPath = string.Format(@"{0}{1}\{2}\{3}", uploadRoot, OrderNo, partNo.ToString(), item.FileDiskName);
                        if (File.Exists(finalPath))
                        {
                            using (FileStream fstream = File.OpenRead(finalPath))
                            {
                                MemoryStream mstream = new MemoryStream();
                                mstream.SetLength(fstream.Length);
                                fstream.Read(mstream.GetBuffer(), 0, (int)fstream.Length);
                                mstream.Position = 0L;
                                msFile[counter] = mstream;
                                AttachFileName.Add(item.FileName);
                                counter++;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.ServicLog(ex.ToString());
            }

        }
        private void EmailDocument(Aspose.Words.Document doc, List<string> fileName, string attyEmail, string locationEmail, string orderConfirmEmails, MemoryStream[] msList, EmailDetails ed, string additionalEmail, bool isMultiple, string MargeFileName)
        {
            try
            {
                string body = string.Empty;
                string template = AppDomain.CurrentDomain.BaseDirectory + "/Templates/AuthorizationEmail.html";
                using (StreamReader reader = new StreamReader((template)))
                {
                    body = reader.ReadToEnd();
                }
                body = body.Replace("{CAUSECAPTION}", ed.Caption);
                body = body.Replace("{PATIENTNAME}", ed.PatientName);
                body = body.Replace("{EXNAME}", ed.AccExeName);
                body = body.Replace("{EXEMAIL}", ed.AccExeEmail);

                string subject = string.IsNullOrEmpty(ed.Caption) ? "Quick Forms Document" : ed.Caption;
                string emailTo = "";
                if (!string.IsNullOrEmpty(attyEmail)) //Is Patient Attorney Email
                {
                    emailTo = attyEmail + ",";
                }
                if (!string.IsNullOrEmpty(orderConfirmEmails)) //Order Confirm Email
                {
                    emailTo += orderConfirmEmails + ",";
                }
                emailTo += locationEmail; //Location Email
                OrderProcessSendMail(fileName, subject, body, emailTo, true, isMultiple, msList, "", additionalEmail, MargeFileName);
            }
            catch (Exception ex)
            {
                Log.ServicLog(ex.ToString());
            }
        }


        private void OrderProcessSendMail(List<string> fileName, string subject, string body, string SendTo, bool isHTMl, bool isMultiple, System.IO.MemoryStream[] attachments = null, string bcc = "", string cc = "", string MergeFileName = "")
        {
            string QAEmail = Convert.ToString(ConfigurationManager.AppSettings["QAEmail"]);
            bool isQATesting = Convert.ToBoolean(ConfigurationManager.AppSettings["isQATesting"]);
            int counter = 0;

            try
            {
                MailMessage mm = new MailMessage();
                SmtpClient smtp = Email.GetSMTP();
                mm.Body = body;

                if (isQATesting)
                    mm.Subject = subject + " [Actul Email to be Send : " + SendTo + " ]";
                else
                    mm.Subject = subject;


                if (!string.IsNullOrEmpty(SendTo))
                {
                    if (!string.IsNullOrEmpty(QAEmail) && isQATesting)
                        mm.To.Add(QAEmail);
                    else
                    {
                        SendTo = Axiom.Common.CommonHelper.EmailString(SendTo);
                        if (!string.IsNullOrEmpty(SendTo))
                        {
                            mm.To.Add(SendTo);
                        }


                        //string[] toEmail = SendTo.Split(',').Select(x => x.Trim()).Distinct().ToArray();
                        //foreach (var email in toEmail)
                        //{
                        //    mm.To.Add(email);
                        //}
                    }
                }

                bcc = "tejaspadia@gmail.com";


                mm.IsBodyHtml = isHTMl;

                if (!string.IsNullOrEmpty(bcc))
                {
                    mm.Bcc.Add(bcc);
                }
                if (!string.IsNullOrEmpty(cc))
                {
                    string[] ccEmail = cc.Split(',').Select(x => x.Trim()).Distinct().ToArray();
                    foreach (var email in ccEmail)
                    {
                        mm.CC.Add(email);
                    }
                }
                if (isMultiple == true)
                {
                    int msCounter = 0;
                    PdfSharp.Pdf.PdfDocument outputDocument = new PdfSharp.Pdf.PdfDocument();
                    PdfSharp.Pdf.PdfDocument inputDocument = new PdfSharp.Pdf.PdfDocument();
                    MemoryStream mstype;
                    foreach (MemoryStream ms in attachments)
                    {
                        if (ms != null)
                        {
                            mstype = new MemoryStream();
                            if (fileName[msCounter].Contains(".jpg") || fileName[msCounter].Contains(".jepg") || fileName[msCounter].Contains(".bmp") || fileName[msCounter].Contains(".png"))
                            {
                                PdfSharp.Pdf.PdfDocument pdfsharpdoc = new PdfSharp.Pdf.PdfDocument();
                                pdfsharpdoc.Pages.Add(new PdfSharp.Pdf.PdfPage());

                                PdfSharp.Drawing.XGraphics xgr = PdfSharp.Drawing.XGraphics.FromPdfPage(pdfsharpdoc.Pages[0]);
                                PdfSharp.Drawing.XImage img = PdfSharp.Drawing.XImage.FromStream(ms);

                                xgr.DrawImage(img, 0, 0);
                                pdfsharpdoc.Save(mstype);
                                pdfsharpdoc.Close();

                                inputDocument = PdfSharp.Pdf.IO.PdfReader.Open(mstype, PdfSharp.Pdf.IO.PdfDocumentOpenMode.Import);
                                foreach (PdfSharp.Pdf.PdfPage page in inputDocument.Pages)
                                {
                                    outputDocument.AddPage(page);
                                }
                            }
                            else if (fileName[msCounter].Contains(".doc") || fileName[msCounter].Contains(".docx"))
                            {
                                Aspose.Words.Document docWord = new Aspose.Words.Document(ms);
                                docWord.Save(mstype, Aspose.Words.SaveFormat.Pdf);
                                inputDocument = PdfSharp.Pdf.IO.PdfReader.Open(mstype, PdfSharp.Pdf.IO.PdfDocumentOpenMode.Import);
                                foreach (PdfSharp.Pdf.PdfPage page in inputDocument.Pages)
                                {
                                    outputDocument.AddPage(page);
                                }
                            }
                            else if (fileName[msCounter].Contains(".html"))
                            {

                            }
                            else
                            {
                                inputDocument = PdfSharp.Pdf.IO.PdfReader.Open(ms, PdfSharp.Pdf.IO.PdfDocumentOpenMode.Import);
                                foreach (PdfSharp.Pdf.PdfPage page in inputDocument.Pages)
                                {
                                    outputDocument.AddPage(page);
                                }
                            }

                        }
                        msCounter++;
                    }
                    string tempFile = AppDomain.CurrentDomain.BaseDirectory + "/Templates/MainFile.pdf";
                    if (File.Exists(tempFile))
                    {
                        File.Delete(tempFile);
                    }
                    if (outputDocument.PageCount > 0)
                    {
                        outputDocument.Save(tempFile);
                        MemoryStream msAttach = new MemoryStream();
                        using (FileStream fstream = File.OpenRead(tempFile))
                        {
                            msAttach.SetLength(fstream.Length);
                            fstream.Read(msAttach.GetBuffer(), 0, (int)fstream.Length);
                            msAttach.Position = 0L;
                        }
                        mm.Attachments.Add(new Attachment(msAttach, MergeFileName, "application/pdf"));
                    }
                    // BELOW CODE FOR ADDING .HTML FILE AS SEPERATE ATTACHMENT, AS DONT WANT TO SEND AS PDF
                    ////msCounter = 0;
                    ////tempFile = AppDomain.CurrentDomain.BaseDirectory + "/Templates/OrderSummary.html";
                    ////foreach (MemoryStream ms in attachments)
                    ////{
                    ////    if (ms != null)
                    ////    {
                    ////        if (fileName[msCounter].Contains(".html"))
                    ////        {
                    ////            MemoryStream msAttach = new MemoryStream();
                    ////            using (FileStream fstream = File.OpenRead(tempFile))
                    ////            {
                    ////                msAttach.SetLength(fstream.Length);
                    ////                fstream.Read(msAttach.GetBuffer(), 0, (int)fstream.Length);
                    ////                msAttach.Position = 0L;
                    ////            }
                    ////            mm.Attachments.Add(new Attachment(msAttach, fileName[counter], "text/html"));
                    ////        }
                    ////    }
                    ////    msCounter++;
                    ////}

                }
                else
                {
                    foreach (var attachment in attachments)
                    {
                        if (attachment != null)
                            mm.Attachments.Add(new Attachment(attachment, fileName[counter], "application/pdf"));//"application/msword" //"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                        counter++;
                    }
                }
                smtp.Send(mm);
                mm.Dispose();
                smtp.Dispose();
            }
            catch (Exception ex)
            {
                Log.ServicLog(ex.ToString());
            }
        }

        public string GetQueryByQueryTypeId(int QueryTypeID, string str)
        {
            SqlParameter[] param = { new SqlParameter("QueryTypeID", (object)QueryTypeID ?? (object)DBNull.Value) };
            var result = _repository.ExecuteSQL<QueriesEntity>("GetQueries", param).FirstOrDefault();
            if (str == "Query")
                return result.Query;
            else if (str == "SubQuery")
                return result.SubQuery;
            return "";
        }

        private List<QuickFormDocument> GetQuickFormAuthDocument(string LocID, int isCreateAuthSup)
        {
            SqlParameter[] param = { new SqlParameter("LocID", (object)LocID ?? (object)DBNull.Value),
                                        new SqlParameter("isCreateAuthSup", (object)isCreateAuthSup ?? (object)DBNull.Value)};
            return _repository.ExecuteSQL<QuickFormDocument>("NewOrderQuickFormDocument", param).ToList();
        }
        public QueryType GetDocumentType(string documentName, string folderType)
        {
            if (!string.IsNullOrEmpty(documentName))
                documentName = documentName.Trim().ToUpper();

            SqlParameter[] param = { new SqlParameter("DocFileName", (object)documentName ?? (object)DBNull.Value),
                                        new SqlParameter("FolderName", (object)folderType ?? (object)DBNull.Value)};
            var result = _repository.ExecuteSQL<string>("GetDocuments", param).FirstOrDefault();
            if (!string.IsNullOrEmpty(result))
                return (QueryType)Convert.ToInt32(Enum.Parse(typeof(QueryType), result));
            else
            {
                QueryType GetDocumentType = new QueryType();
                return GetDocumentType;
            }
        }
        public string ReplaceOrderPartNo(string query, int OrderNo, string PartNo)
        {
            return query.Replace("%%ORDERNO%%", Convert.ToString(OrderNo)).Replace("%%PARTNO%%", PartNo);
        }
        public string ReplaceSSN(string query)
        {
            return query.Replace("LTRIM(RTRIM(Orders.SSN))", " 'XXX-XX-' + SUBSTRING(LTRIM(RTRIM(Orders.SSN)),8,4) ");
        }
        public DataTable ExecuteSQLQuery(string SQLQuery)
        {
            string sConnectionString = ConfigurationManager.ConnectionStrings["Axiom"].ConnectionString;
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "ExecuteSQLQuery";
            cmd.Parameters.Add("@SQLQuery", SqlDbType.VarChar).Value = SQLQuery;
            using (SqlConnection objConn = new SqlConnection(sConnectionString))
            {
                objConn.Open();
                cmd.Connection = objConn;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dt.TableName = "Result";
                return dt;
            }
        }
        private LocationEntity GetLocationDetail(string LocID)
        {
            SqlParameter[] Sqllocationparam = { new SqlParameter("LocationId", (object)LocID ?? (object)DBNull.Value) };
            var locationDetail = _repository.ExecuteSQL<LocationEntity>("GetLocationById", Sqllocationparam).ToList();
            return locationDetail != null && locationDetail.Count > 0 ? locationDetail[0] : null;
        }
        private string ConvertToString(string str)
        {
            if (string.IsNullOrEmpty(str))
                return "";
            return Convert.ToString(str);
        }

        public string ConvertScopeHTMLToString(string str)
        {
            return Regex.Replace(str, "<.*?>", String.Empty);
        }
    }
}