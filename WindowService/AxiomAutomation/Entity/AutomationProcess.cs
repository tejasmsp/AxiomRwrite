using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Timers;
using System.Configuration;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using AxiomAutomation.Entity;
using System.Text.RegularExpressions;

namespace AxiomAutomation.Entity
{
    public class AutomationProcess
    {

        private AutomationProcess()
        {
        }

        public AutomationProcess(string _documentRoot, int _RecordType, string _BillFirm
                                    , string _ClaimNo, string _AttyName, string _AttyID
                                   , bool _displaySSN, FileType _docFileType, PartDetail _part, OperationInitiatedFrom _operationInitiatedFrom)
        {
            documentRoot = _documentRoot;
            RecordType = _RecordType;
            BillFirm = _BillFirm;
            ClaimNo = _ClaimNo;
            AttyName = _AttyName;
            AttyID = _AttyID;
            DisplaySSN = _displaySSN;
            DocFileType = _docFileType;
            Part = _part;
            OperationInitiatedFrom = _operationInitiatedFrom;
        }
        private string documentRoot { get; set; }// = ConfigurationManager.AppSettings["DocumentRoot"].ToString();
        private int RecordType { get; set; }
        private string BillFirm { get; set; }
        private string ClaimNo { get; set; }
        private string AttyName { get; set; }
        private string AttyID { get; set; }
        private bool DisplaySSN { get; set; } 
        private FileType DocFileType { get; set; }
        private PartDetail Part { get; set; }
        private OperationInitiatedFrom OperationInitiatedFrom { get; set; }

        #region NewChangesByAkash


        public void DoRequireOperationOnDocuments(RequestDocuments docitem, int OrderNo, int PartNo, string filetype,
                                        Location location, int partListCount, CompanyDetailForEmailEntity objCompany, bool isProcessServer)
        {
            Log.ServicLog("Order NO :" + OrderNo + "  Part NO:" + PartNo);
            string docpath = docitem.DocumentPath.ToString().Trim().Replace(">", "/");
            string docNameDB = OrderNo.ToString() + "_" + PartNo.ToString() + "_" + docitem.DocumentName;

            if (docitem.DocumentName.ToString().Split('_').Length == 3)
                docitem.DocumentName = docitem.DocumentName.ToString().Split('_')[2].ToUpper();
            else if (docitem.DocumentName.ToString().Split('_').Length == 4)
                docitem.DocumentName = docitem.DocumentName.ToString().Split('_')[3].ToUpper();
            else
                docitem.DocumentName = docitem.DocumentName.ToString().Split('_')[1].ToUpper();
            string[] folders = docitem.DocumentPath.Split('>');

            if (folders.Contains("Custodian Letters") || folders.Contains("Subpoenas"))
                filetype = "pdf";

            string query = string.Empty;
            QueryType pdt = DbAccess.GetDocumentType(docitem.DocumentName, folders[0].ToString());

            query = ReplaceQueryAccordingToQueryType(pdt, OrderNo, PartNo, folders, docNameDB, partListCount);
            var dtQueryList = DbAccess.ExecuteSQLQuery(query);

            MemoryStream ms = new MemoryStream();

            ReplaceSubQueryAccordingToQueryType(pdt, OrderNo, PartNo, folders, docNameDB, partListCount, docitem, ref dtQueryList);

            string filePath = Path.Combine(documentRoot, docitem.DocumentPath.ToString().Trim().Replace(">", "\\"), docitem.DocumentName.Trim());
            Aspose.Words.Document doc = new Aspose.Words.Document();
            try
            {
                #region Add Company Wise logo 
                doc = Axiom.Common.CommonHelper.InsertHeaderLogo(filePath, objCompany.LogoPath);
                #endregion
            }
            catch (Exception ex)
            {
                Log.ServicLog("---------- 379 AT CREATING NEW OBJECT OF Aspose.Word.Document. FilePath = " + filePath.ToString());
                Log.ServicLog(ex.ToString());
            }

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
                Log.ServicLog("----------- EXCEPTION at 399 -------------------");
                Log.ServicLog(Convert.ToString(ex.Message));
                Log.ServicLog(Convert.ToString(ex.StackTrace));
                Log.ServicLog(Convert.ToString(ex.InnerException));
                Log.ServicLog("--------------------------------------------");
            }
            doc.MailMerge.Execute(dtQueryList);
            doc.Save(ms, Aspose.Words.SaveFormat.Pdf);
            if (RecordType > 0 && !string.IsNullOrEmpty(BillFirm))
            {
                string TStamp = DateTime.Now.ToString("yyyyMMddHHmm");
                string _storageRoot = string.Empty;
                string FilePath = string.Empty;
                if (BillFirm == "GRANCO01" || BillFirm == "HANOAA01")
                {
                    if (BillFirm == "GRANCO01")
                    {
                        _storageRoot = ConfigurationManager.AppSettings["GrangeRoot"].ToString();
                    }
                    else if (BillFirm == "HANOAA01")
                    {
                        _storageRoot = ConfigurationManager.AppSettings["HanoverRoot"].ToString();
                    }
                    DirectoryInfo dis = new DirectoryInfo(_storageRoot);
                    if (!dis.Exists)
                    { dis.Create(); }

                    if (BillFirm == "GRANCO01")
                    {
                        FilePath = _storageRoot + string.Format("{0}-{1}-{2}-{3}", ClaimNo, TStamp, OrderNo, PartNo + "." + filetype);
                    }
                    else if (BillFirm == "HANOAA01")
                    {
                        FilePath = _storageRoot + string.Format("{0}_{1}_{2}_{3}-{4}", ClaimNo, AttyName, TStamp, OrderNo, PartNo + "." + filetype);
                    }

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
                    ms.Close();
                }
            }
            string outputFileName = "printforms";
            outputFileName = Path.GetFileNameWithoutExtension(Convert.ToString(docNameDB).Replace(" ", "-")) + "." + filetype;
            Guid gid = Guid.NewGuid();
            if (PartNo > 0 || (pdt == QueryType.AttorneyForms || pdt == QueryType.Confirmation || pdt == QueryType.Waiver))
            {
                if (pdt == QueryType.AttorneyForms)
                {
                    string attorney = "";
                    if (docNameDB.ToString().Split('_').Length == 4)
                        attorney = docNameDB.ToString().Split('_')[2].ToUpper();
                    outputFileName = string.Format("{0}-{1}-{2}-{3}{4}", new object[] { OrderNo, PartNo, attorney, Path.GetFileNameWithoutExtension(filePath).Replace(" ", "-"), "." + filetype }).Replace(",", "-");
                }
                else
                    outputFileName = string.Format("{0}-{1}-{2}{3}", new object[] { OrderNo, PartNo, Path.GetFileNameWithoutExtension(filePath).Replace(" ", "-"), "." + filetype }).Replace(",", "-");

            }
            try //Order Level File
            {
                if (!Directory.Exists(Path.Combine(ConfigurationManager.AppSettings["UploadRoot"].ToString(), Convert.ToString(OrderNo), Convert.ToString(PartNo))))
                    Directory.CreateDirectory(Path.Combine(ConfigurationManager.AppSettings["UploadRoot"].ToString(), Convert.ToString(OrderNo), Convert.ToString(PartNo)));
                using (FileStream file = new FileStream(Path.Combine(ConfigurationManager.AppSettings["UploadRoot"].ToString(), Convert.ToString(OrderNo), Convert.ToString(PartNo), gid.ToString() + "." + filetype), FileMode.Create, FileAccess.Write))
                {
                    ms.WriteTo(file);
                    file.Close();
                }
            }
            catch (Exception ex)
            {
                Log.ServicLog("----------- EXCEPTION at 489-------------------");
                Log.ServicLog(Convert.ToString(ex.Message));
                Log.ServicLog(Convert.ToString(ex.StackTrace));
                Log.ServicLog(Convert.ToString(ex.InnerException));
                Log.ServicLog("--------------------------------------------");
            }
            finally
            {
                if (OperationInitiatedFrom.AutomationService == OperationInitiatedFrom || (OperationInitiatedFrom.WebForm == OperationInitiatedFrom &&  Part.isSup != true ) )
                {
                    var objFile = new FileEnity();
                    objFile.OrderNo = OrderNo;
                    objFile.PartNo = PartNo;
                    objFile.FileName = outputFileName.Replace("_", "-");
                    objFile.FileTypeId = (int)DocFileType;
                    objFile.FileDiskName = gid + "." + filetype;
                    DbAccess.InsertFile(objFile);
                }
            }
            ms.Dispose();
            ProcessOrderLocation(OrderNo, location, docNameDB, PartNo, pdt, objCompany, doc, isProcessServer);
            Log.ServicLog("========================================================================");
        }

        private string ReplaceQueryAccordingToQueryType(QueryType pdt, int OrderNo, int PartNo, string[] folders, string docNameDB, int partListCount)
        {
            if (pdt == QueryType.FaceSheet || pdt == QueryType.FaceSheetNew)
            {
                pdt = QueryType.FaceSheetNew;
            }

            string query = DbAccess.GetQueryByQueryTypeId((int)pdt, "Query");
            query = ReplaceOrderPartNo(query, OrderNo, PartNo);
            switch (pdt)
            {
                case QueryType.Common:
                    bool isMichigan = folders.Contains("Michigan");
                    bool isRush = docNameDB.Contains("RUSH");

                    if (!string.IsNullOrEmpty(query))
                    {
                        if (isMichigan && isRush)
                            query = query.Replace("--Michigan and Rush", " ,DATENAME(DW, DATEADD(day,7,GETDATE())) as [DayName],DATENAME(MM, DATEADD(day,7,GETDATE())) + RIGHT(CONVERT(VARCHAR(12), DATEADD(day,7,GETDATE()), 107), 9) AS BigDate ");
                        else if (isMichigan && !isRush)
                            query = query.Replace("--Michigan and Rush", " ,DATENAME(DW, DATEADD(day,14,GETDATE())) as [DayName],DATENAME(MM, DATEADD(day,14,GETDATE())) + RIGHT(CONVERT(VARCHAR(12), DATEADD(day,14,GETDATE()), 107), 9) AS BigDate ");
                    }
                    break;
                case QueryType.Confirmation:
                    query = query.Replace("%%PartCnt%%", Convert.ToString(partListCount));
                    break;
                case QueryType.FaceSheet:
                case QueryType.FaceSheetNew:
                case QueryType.AttorneyForms:
                    query = query.Replace("%%ATTYNO%%", AttyID);
                    break;
                case QueryType.StatusProgressReports:
                    query = query.Replace("%%PartCnt%%", Convert.ToString(partListCount));
                    break;
                case QueryType.StatusLetters:
                case QueryType.Waiver:
                case QueryType.Interrogatories:
                case QueryType.TargetSheets:
                case QueryType.CerticicationNOD:
                case QueryType.AttorneyOfRecords:
                case QueryType.CollectionLetters:
                case QueryType.Notices:
                    break;
                default:
                    break;
            }
            if (!DisplaySSN)
                query = ReplaceSSN(query);
            return query;

        }

        private void ReplaceSubQueryAccordingToQueryType(QueryType pdt, int OrderNo, int PartNo, string[] folders, string docNameDB, int partListCount, RequestDocuments docitem, ref DataTable dtQueryList)
        {
            string subquery = string.Empty;
            var dtsubQuery = new DataTable();
            StringBuilder partInfo = new StringBuilder();
            StringBuilder partInfo2 = new StringBuilder();

            subquery = DbAccess.GetQueryByQueryTypeId(Convert.ToInt32(pdt), "SubQuery");
            if (!string.IsNullOrEmpty(subquery) && dtQueryList != null)
            {
                subquery = ReplaceOrderPartNo(subquery, OrderNo, PartNo);
                if (!DisplaySSN)
                    subquery = ReplaceSSN(subquery);

                if (pdt == QueryType.AttorneyForms)
                {
                    subquery = subquery.Replace("%%ATTYNO%%", AttyID);
                }
                if (pdt != QueryType.AttorneyOfRecords)
                {
                    dtsubQuery = DbAccess.ExecuteSQLQuery(subquery);
                }

            }
            else
            {
                return;
            }

            if (pdt == QueryType.Confirmation || pdt == QueryType.TargetSheets || pdt == QueryType.StatusProgressReports)
            {

                partInfo.Append("_____________________________________________________________________________\n\r");
                for (int a = 0; a < dtsubQuery.Rows.Count; a++)
                {
                    string partInfoText = Convert.ToString(dtsubQuery.Rows[a]["PartInfo1"]);
                    if (pdt == QueryType.Confirmation && docitem.DocumentName.ToUpper().Equals("ORDER CONFIRMATION.DOC"))
                        partInfoText = partInfoText.Replace("scopehere", Convert.ToString(dtsubQuery.Rows[a]["scope"]));
                    else
                        partInfoText = partInfoText.Replace("scopehere", "");
                    partInfo.Append(partInfoText + "\n\r");
                    partInfo2.Append(Convert.ToString(dtsubQuery.Rows[a]["LocationHeader"]) + "\n");
                }

                AddColumWithDefultValue(dtQueryList, "PartInfo", typeof(string), Convert.ToString(partInfo));
                AddColumWithDefultValue(dtQueryList, "PartInfo2", typeof(string), Convert.ToString(partInfo2));


            }
            else if (pdt == QueryType.Waiver)
            {
                StringBuilder locationInfo = new StringBuilder();

                for (int b = 0; b < dtsubQuery.Rows.Count; b++)
                {
                    locationInfo.Append(Convert.ToString(dtsubQuery.Rows[b]["Location1"]) + '\n');
                }

                AddColumWithDefultValue(dtQueryList, "Selected_Part", typeof(string), Convert.ToString(PartNo));
                AddColumWithDefultValue(dtQueryList, "Location1", typeof(string), Convert.ToString(locationInfo));
            }
            else if (pdt == QueryType.CerticicationNOD)
            {
                StringBuilder attyInfo = new StringBuilder();
                attyInfo.Append('\n');

                for (int c = 0; c < dtsubQuery.Rows.Count; c++)
                {
                    attyInfo.Append(Convert.ToString(dtsubQuery.Rows[c]["Attorneys"]) + "\n");
                }

                AddColumWithDefultValue(dtQueryList, "Attorneys", typeof(string), Convert.ToString(attyInfo));
            }
            else if (pdt == QueryType.AttorneyOfRecords)
            {

                string[] strQuery = null;
                StringBuilder attyInfo2 = new StringBuilder();
                attyInfo2.Append('\n');
                if (!string.IsNullOrEmpty(subquery))
                {
                    strQuery = subquery.Split(new string[] { "--Split--" }, StringSplitOptions.RemoveEmptyEntries);
                    dtsubQuery = DbAccess.ExecuteSQLQuery(strQuery[0]);
                    for (int d = 0; d < dtsubQuery.Rows.Count; d++)
                    {
                        attyInfo2.Append(Convert.ToString(dtsubQuery.Rows[d]["AttyInfo"]));
                    }
                    attyInfo2.Append('\n');

                    dtsubQuery = DbAccess.ExecuteSQLQuery(strQuery[1]);
                    for (int e = 0; e < dtsubQuery.Rows.Count; e++)
                    {
                        attyInfo2.Append(Convert.ToString(dtsubQuery.Rows[e]["AttyInfo"]) + "\n");
                    }
                }
                AddColumWithDefultValue(dtQueryList, "AttyInfo", typeof(string), Convert.ToString(attyInfo2));

            }
            else if (pdt == QueryType.AttorneyForms)
            {
                StringBuilder locationInfo2 = new StringBuilder();

                for (int f = 0; f < dtsubQuery.Rows.Count; f++)
                {
                    locationInfo2.Append(Convert.ToString(dtsubQuery.Rows[f]["Location1"]) + "\n");
                }

                AddColumWithDefultValue(dtQueryList, "Selected_Part", typeof(string), Convert.ToString(PartNo));
                AddColumWithDefultValue(dtQueryList, "Location1", typeof(string), Convert.ToString(locationInfo2));
            }
        }
         
        private void ProcessOrderLocation(int OrderNo, Location location, string docNameDB, int PartNo
                                        , QueryType pdt, CompanyDetailForEmailEntity objCompany, Aspose.Words.Document doc, bool isProcessServer)
        {
            #region Location
            var objOrderDetail = DbAccess.GetOrderDetailByOrderNo(OrderNo);
            string OrderAttorney = string.Empty;
            if (location != null)
            {
                //foreach (var location in locationList)
                {
                    if (Part.isCreateAuthSup == true && Part.isAuth == true)
                    {
                        if (Part.LocID.Trim().ToUpper() != "AXIOMI001")
                        {
                            var AsgnTo = "AUTHSS";
                            var CallBack = DateTime.Now.AddDays(14);
                           DbAccess.UpdateOrderPart(OrderNo, PartNo, AsgnTo, DateTime.Now.AddDays(14));
                        }
                    }
                    else if (!string.IsNullOrEmpty(location.SendRequest))
                    {
                        string[] strsplit = location.SendRequest.Split(',');

                        foreach (var requesttype in strsplit)
                        {

                            SendRequestType sendRequestType = (SendRequestType)Enum.Parse(typeof(SendRequestType), requesttype);

                            if (sendRequestType == SendRequestType.Email || sendRequestType == SendRequestType.FaxNumber)
                            {
                                #region Email - FAX
                                MemoryStream[] msFile = new MemoryStream[10];
                                List<string> fileNames = new List<string>();
                                var pdfDocName = docNameDB.Replace("doc", "pdf").Replace("DOC", "pdf");
                                DbAccess.GetAttachFileFromDB(OrderNo, PartNo, ref fileNames, ref msFile);
                                EmailDetails ed = new EmailDetails();
                                ed.Caption = objOrderDetail.Caption;
                                ed.CauseNumber = objOrderDetail.CauseNo;
                                ed.PatientName = objOrderDetail.PatientName;
                                ed.AccExeName = string.IsNullOrEmpty(objOrderDetail.AccExeName) ? "Josh Sanford" : objOrderDetail.AccExeName;
                                ed.AccExeEmail = string.IsNullOrEmpty(objOrderDetail.AccExeEmail) ? "Josh.Sanford@axiomcopy.com" : objOrderDetail.AccExeEmail;


                                OrderAttorney = Convert.ToString(objOrderDetail.OrderingAttorney);

                                string additionalEmail = string.Empty;
                                if (pdt == QueryType.Confirmation)
                                {
                                    var notificationList = DbAccess.GetAssistantContactNotification(OrderNo);
                                    if (notificationList != null && notificationList.Count > 0)
                                    {
                                        /*
                                        notificationList = notificationList.Where(x => x.AttyID == OrderAttorney.Trim() && x.OrderConfirmation == true).ToList();
                                        if (notificationList.Count > 0)
                                        {
                                            foreach (var item in notificationList)
                                                additionalEmail += item.AssistantEmail + ",";

                                            additionalEmail = additionalEmail.Trim(',');
                                        }
                                        */
                                        additionalEmail = string.Join(",", notificationList.Where(x => x.AttyID == OrderAttorney.Trim() && x.OrderConfirmation == true).Select(x => x.AssistantEmail));
                                    }
                                }
                                if (sendRequestType == SendRequestType.FaxNumber || sendRequestType == SendRequestType.Email)
                                {
                                    string title = (sendRequestType == SendRequestType.Email ? "Email" : "Fax Number");
                                    string SendTo = (sendRequestType == SendRequestType.Email ? location.Email : string.Concat(location.AreaCode3, location.FaxNo).Replace("-", "").Replace(" ", ""));

                                    // IF EMAIL LOCATION (EMAIL OR Fax Number)  IS NOT FOUND THEN SEND MAIL OR FAX  TO JACK
                                    if (string.IsNullOrEmpty(SendTo))
                                    {
                                        Log.ServicLog("=========== " + title + " not found ================ ");
                                        string subject = "[Axiom Automation] " + title + " Not Found for Order No " + OrderNo + "-" + PartNo + " Location ID : " + location.LocID;
                                        string body = "We have not found " + title + " for " + OrderNo + "-" + PartNo;
                                        body += "\n Location ID : " + location.LocID;
                                        body += "\n Location Name : " + location.Name1 + " " + location.Name2;
                                        Utility.SendMailWithAttachment("j.alspaugh@axiomcopy.com", body, subject, null, null, "tejaspadia@gmail.com", "");
                                    }
                                    else
                                    {
                                        if (sendRequestType == SendRequestType.Email)
                                        {
                                            EmailDocument(doc, fileNames, SendTo, msFile, ed, additionalEmail, true, pdfDocName, objCompany);
                                        }
                                        else
                                        {
                                            FaxDocument(0, fileNames, SendTo, location.Name1, msFile);
                                        }

                                        Log.ServicLog(title + " Sent to : " + SendTo);
                                        string partNotes = string.Empty;
                                        CreateNoteString(OrderNo, PartNo, "Input or sent via " + title + ".", "SYSTEM", ref partNotes, false, false);
                                    }
                                }
                                foreach (MemoryStream item in msFile)
                                {
                                    if (item != null)
                                        item.Dispose();
                                }
                                #endregion
                            }
                            else if (sendRequestType == SendRequestType.Mail_StandardFolder || sendRequestType == SendRequestType.CertifiedMail)
                            {
                                string title = (sendRequestType == SendRequestType.Mail_StandardFolder ? "Mail - Standard Folder" : "Certified Mail");
                                Log.ServicLog(title);
                                #region MAIL
                                string MailPath = ConfigurationManager.AppSettings[sendRequestType == SendRequestType.Mail_StandardFolder ? "MailPath" : "CertifiedPath"].ToString();
                                if (!Directory.Exists(MailPath))
                                    Directory.CreateDirectory(MailPath);
                                MemoryStream[] msFile = new MemoryStream[10];
                                List<string> fileNames = new List<string>();
                                DbAccess.GetAttachFileFromDB(OrderNo, PartNo, ref fileNames, ref msFile);
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
                                            try
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
                                            catch (Exception ex)
                                            {
                                                Log.ServicLog("============ Exception " + title + "  641 =============");
                                                Log.ServicLog(ex.ToString());
                                                Log.ServicLog(ex.StackTrace);
                                                Log.ServicLog(ex.Source);
                                                Log.ServicLog(".doc at 643");
                                                Log.ServicLog(fileNames[msCounter]);
                                            }
                                            mstype.Dispose();

                                        }
                                        else if (fileNames[msCounter].Contains(".doc") || fileNames[msCounter].Contains(".docx"))
                                        {
                                            try
                                            {
                                                Aspose.Words.Document docWord = new Aspose.Words.Document(msCombine);
                                                docWord.Save(mstype, Aspose.Words.SaveFormat.Pdf);
                                                inputDocument = PdfSharp.Pdf.IO.PdfReader.Open(mstype, PdfSharp.Pdf.IO.PdfDocumentOpenMode.Import);
                                                foreach (PdfSharp.Pdf.PdfPage page in inputDocument.Pages)
                                                    outputDocument.AddPage(page);
                                            }
                                            catch (Exception ex)
                                            {
                                                Log.ServicLog("============ Exception  662=============");
                                                Log.ServicLog(ex.ToString());
                                                Log.ServicLog(ex.StackTrace);
                                                Log.ServicLog(ex.Source);
                                                Log.ServicLog(".doc at 648");
                                                Log.ServicLog(fileNames[msCounter]);



                                                Log.ServicLog(".doc at 750");
                                                Log.ServicLog(fileNames[msCounter]);
                                                try
                                                {
                                                    inputDocument = PdfSharp.Pdf.IO.PdfReader.Open(msCombine, PdfSharp.Pdf.IO.PdfDocumentOpenMode.ReadOnly);

                                                    foreach (PdfSharp.Pdf.PdfPage page in inputDocument.Pages)
                                                        outputDocument.AddPage(page);
                                                }
                                                catch (Exception ex1)
                                                {
                                                    Log.ServicLog("============ Exception  820 =============");
                                                    Log.ServicLog(ex1.ToString());
                                                    Log.ServicLog(ex1.StackTrace);
                                                    Log.ServicLog(ex1.Source);
                                                    Log.ServicLog(".doc at 768");
                                                    Log.ServicLog(fileNames[msCounter]);
                                                }

                                                mstype.Dispose();

                                            }

                                        }
                                        else if(!fileNames[msCounter].Contains(".html"))
                                        {
                                            try
                                            {
                                                inputDocument = PdfSharp.Pdf.IO.PdfReader.Open(msCombine, PdfSharp.Pdf.IO.PdfDocumentOpenMode.Import);
                                                foreach (PdfSharp.Pdf.PdfPage page in inputDocument.Pages)
                                                    outputDocument.AddPage(page);
                                            }
                                            catch (Exception ex)
                                            {
                                                Log.ServicLog("============ Exception 685 =============");
                                                Log.ServicLog(ex.ToString());
                                                Log.ServicLog(ex.StackTrace);
                                                Log.ServicLog(ex.Source);
                                                Log.ServicLog(".doc at 671");
                                                Log.ServicLog(fileNames[msCounter]);
                                            }

                                        }
                                    }
                                    msCounter++;
                                }
                                string FilePath = MailPath + string.Format("{0}-{1}", OrderNo.ToString(), PartNo.ToString() + ".pdf");
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
                                if (sendRequestType == SendRequestType.CertifiedMail)
                                {
                                    DbAccess.InsertMiscChgOrderPart(OrderNo, PartNo);
                                }

                                string strNote = string.Empty;
                                if (sendRequestType == SendRequestType.CertifiedMail)
                                {
                                    strNote = "Input or Sent via Certified Mail.";
                                }
                                else if (sendRequestType == SendRequestType.Mail_StandardFolder)
                                {
                                    strNote = "Input or Sent via Mail.";
                                }
                                string partNotes = string.Empty;
                                CreateNoteString(OrderNo, PartNo, strNote, "SYSTEM", ref partNotes, false, false);

                                outputDocument.Dispose();
                                inputDocument.Dispose();

                                foreach (MemoryStream item in msFile)
                                {
                                    if (item != null)
                                        item.Dispose();
                                }
                                mstype = null;

                                #endregion
                            }

                            else if (sendRequestType == SendRequestType.Upload || sendRequestType == SendRequestType.ProcessServer) //UPLOAD & PROCESS SERVER
                            {
                                isProcessServer = true;
                                string AsgnTo = "";
                                if (sendRequestType == SendRequestType.Upload)
                                {
                                    AsgnTo = "UTILRE";
                                    Log.ServicLog("Upload");
                                }
                                else if (sendRequestType == SendRequestType.ProcessServer)
                                {

                                    Log.ServicLog("Process Server");
                                    AsgnTo = "REQUES";
                                }
                                DbAccess.UpdateQuickFormOrderPart(OrderNo, PartNo, AsgnTo);
                            }

                        }

                    }
                    else
                    { 
                            if (location.ReqAuthorization == true || location.FeeAmountSendRequest == true || location.LinkRequest == true)
                            {
                            // DbAccess.UpdateOrderPart(OrderNo, part.PartNo, "UTILRE", Convert.ToDateTime(pt.CallBack));
                            DbAccess.UpdateOrderPart(OrderNo, PartNo, "UTILRE", DateTime.Now.AddDays(14));


                                string partNotes = string.Empty;
                                CreateNoteString(OrderNo, PartNo, "Assign to In Office Request.", "SYSTEM", ref partNotes, false, false);
                            }

                        
                    }

                }
            }

            #endregion
        }

        private void AddColumWithDefultValue(DataTable dataTable, string ColumnName, Type ColumnDatatype, dynamic defultValue)
        {
            DataColumn column = new DataColumn(ColumnName, ColumnDatatype);
            if (defultValue != null)
            {
                column.DefaultValue = defultValue;
            }
            dataTable.Columns.Add(column);
        }


        #endregion

        private string ConvertScopeHTMLToString(string str)
        {
            return Regex.Replace(str, "<.*?>", String.Empty);
        }
        private string ReplaceOrderPartNo(string query, int OrderNo, int PartNo)
        {
            return query.Replace("%%ORDERNO%%", Convert.ToString(OrderNo)).Replace("%%PARTNO%%", Convert.ToString(PartNo));
        }
        private string ReplaceSSN(string query)
        {
            return query.Replace("LTRIM(RTRIM(Orders.SSN))", " 'XXX-XX-' + SUBSTRING(LTRIM(RTRIM(Orders.SSN)),8,4) ");
        }
        public static void CreateNoteString(int OrderNo, int PartNo, string note, string empId, ref string databaseNoteField, bool mrAttyNote = false, bool trimNote = false)
        {
            Log.ServicLog(" ----------- Note Start -------------------");
            Log.ServicLog(note);
            try
            {
                if (string.IsNullOrEmpty(note.Trim()))
                    return;

                string noteString = mrAttyNote ? "" : Environment.NewLine + Environment.NewLine + "[ {0} - {1} ]" + Environment.NewLine + "{2}";
                DateTime dt = DateTime.Now;
                if (trimNote)
                    note = note.Substring(0, 250);

                noteString = string.Format(noteString, dt.ToString(), empId, note.Trim());

                if (string.IsNullOrEmpty(databaseNoteField) || mrAttyNote)
                    databaseNoteField = mrAttyNote ? noteString.Substring(0, 250) : noteString;
                else
                    databaseNoteField += noteString;

                var userId = new Guid("507D72AE-1E1F-4FC0-AEED-3962FF1DCEC8");
                DbAccess.InsertOrderNotes(OrderNo, PartNo, userId, note);
            }
            catch (Exception ex)
            {
                Log.ServicLog(DateTime.Now.ToString() + " ----------- EXCEPTION INSERT Note -------------------");
                Log.ServicLog(Convert.ToString(ex.Message));
                Log.ServicLog(Convert.ToString(ex.StackTrace));
                Log.ServicLog(Convert.ToString(ex.InnerException));
                Log.ServicLog("--------------------------------------------");
            }
            Log.ServicLog(" ----------- Note end -------------------");
        }

        private void EmailDocument(Aspose.Words.Document doc, List<string> fileName, string email, MemoryStream[] msList, EmailDetails ed, string additionalEmail, bool isMultiple, string MargeFileName, CompanyDetailForEmailEntity objCompany)
        {
            try
            {
                string body = string.Empty;
                string template = AppDomain.CurrentDomain.BaseDirectory + "Template\\AuthorizationEmail.html";
                using (StreamReader reader = new StreamReader((template)))
                {
                    body = reader.ReadToEnd();
                }
                body = body.Replace("{CAUSECAPTION}", ed.Caption);
                body = body.Replace("{PATIENTNAME}", ed.PatientName);
                body = body.Replace("{EXNAME}", ed.AccExeName);
                body = body.Replace("{EXEMAIL}", ed.AccExeEmail);

                string subject = string.IsNullOrEmpty(ed.Caption) ? "Quick Forms Document" : ed.Caption;
                AxiomAutomationEmail.SendMailTest(objCompany, fileName, subject, body, email, true, isMultiple, msList, "", additionalEmail, MargeFileName);
            }
            catch (Exception ex)
            {
                Log.ServicLog(DateTime.Now.ToString() + " ----------- EXCEPTION 1018-------------------");
                Log.ServicLog(Convert.ToString(ex.Message));
                Log.ServicLog(Convert.ToString(ex.StackTrace));
                Log.ServicLog(Convert.ToString(ex.InnerException));
                Log.ServicLog("--------------------------------------------");

            }
        }

        private void FaxDocument(int Id, List<string> fileName, string fax, string name, MemoryStream[] msList)
        {
            int msCounter = 0;
            try
            {
                int counter = 0;
                string tempFile = AppDomain.CurrentDomain.BaseDirectory + "MainFile.pdf";
                if (File.Exists(tempFile))
                    File.Delete(tempFile);
                PdfSharp.Pdf.PdfDocument outputDocument = new PdfSharp.Pdf.PdfDocument();
                PdfSharp.Pdf.PdfDocument inputDocument = new PdfSharp.Pdf.PdfDocument();

                MemoryStream msImage;
                foreach (MemoryStream ms in msList)
                {


                    if (ms != null)
                    {
                        try
                        {
                            msImage = new MemoryStream();
                            if (fileName[msCounter].Contains(".jpeg") || fileName[msCounter].Contains(".jpg") || fileName[msCounter].Contains(".bmp") || fileName[msCounter].Contains(".png"))
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
                                {
                                    outputDocument.AddPage(page);
                                }
                            }
                            else if (fileName[msCounter].Contains(".doc") || fileName[msCounter].Contains(".docx"))
                            {

                                Aspose.Words.Document docWord = new Aspose.Words.Document(ms);
                                docWord.Save(msImage, Aspose.Words.SaveFormat.Pdf);
                                inputDocument = PdfSharp.Pdf.IO.PdfReader.Open(msImage, PdfSharp.Pdf.IO.PdfDocumentOpenMode.Import);
                                foreach (PdfSharp.Pdf.PdfPage page in inputDocument.Pages)
                                {
                                    outputDocument.AddPage(page);
                                }
                            }
                            else
                            {
                                inputDocument = PdfSharp.Pdf.IO.PdfReader.Open(ms, PdfSharp.Pdf.IO.PdfDocumentOpenMode.Import);
                                foreach (PdfSharp.Pdf.PdfPage page in inputDocument.Pages)
                                {
                                    outputDocument.AddPage(page);
                                }
                            }
                            inputDocument.Dispose();
                            outputDocument.Dispose();
                        }
                        catch (Exception ex)
                        {
                            Log.ServicLog("============ Fax Exception 1093 =============");
                            Log.ServicLog(ex.ToString());
                            Log.ServicLog(ex.StackTrace);
                            Log.ServicLog(ex.Source);
                            Log.ServicLog(".doc at 1072");
                            Log.ServicLog(fileName[msCounter]);
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

                Log.ServicLog(DateTime.Now.ToString() + " ----------- EXCEPTION 1125  -------------------");
                Log.ServicLog(Convert.ToString(ex.Message));
                Log.ServicLog(Convert.ToString(ex.StackTrace));
                Log.ServicLog(Convert.ToString(ex.InnerException));
                Log.ServicLog("--------------------------------------------");
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
                str.Append("<name>" + senderName.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&apos;") + "</name>");
                str.Append("<email_address>" + senderEmail + "</email_address>");
                str.Append("</sender>");
                str.Append("<recipient>");
                str.Append("<name>" + name.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&apos;") + "</name>");
                str.Append("<fax_number>91" + faxNo + "</fax_number>");
                str.Append(" </recipient>");
                str.Append("<attachment>");
                str.Append("<location>inline</location>");
                str.Append("<name>" + attachmentName + "</name>");
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

                Log.ServicLog(DateTime.Now.ToString() + " ----------- EXCEPTION 1179 -------------------");
                Log.ServicLog(Convert.ToString(ex.Message));
                Log.ServicLog(Convert.ToString(ex.StackTrace));
                Log.ServicLog(Convert.ToString(ex.InnerException));
                Log.ServicLog("--------------------------------------------");

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

                Log.ServicLog(DateTime.Now.ToString() + " ----------- EXCEPTION 1219 -------------------");
                Log.ServicLog(Convert.ToString(ex.Message));
                Log.ServicLog(Convert.ToString(ex.StackTrace));
                Log.ServicLog(Convert.ToString(ex.InnerException));
                Log.ServicLog("--------------------------------------------");

            }
        }
    }
}
