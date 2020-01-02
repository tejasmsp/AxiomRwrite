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


namespace AxiomAutomation
{
    public partial class AxiomAutomation : ServiceBase
    {
        public static int IsRevised = 0;
        public static string IsRevisedText = "";
        public static string BillFirm = "";
        public static string ClaimNo = "";
        public static int RecordType = 0;
        public static string AttyName = "";
        public static string AttyID = "";
        System.Timers.Timer serviceTimer = null;
        public string time1 = string.Empty;
        public string time2 = string.Empty;
        public string time3 = string.Empty;
        public string time4 = string.Empty;
        Aspose.Words.License license = new Aspose.Words.License();
        string documentRoot = string.Empty;

        public AxiomAutomation()
        {
            license.SetLicense("Aspose.Words.lic");
            documentRoot = ConfigurationManager.AppSettings["DocumentRoot"].ToString();
            Log.ServicLog("======================================================================");
            Log.ServicLog("Initialize");
            Log.ServicLog("Duration Time :  " + ConfigurationManager.AppSettings["Duration"].ToString() + " min");
            InitializeComponent();
            serviceTimer = new System.Timers.Timer();
            serviceTimer.Elapsed += serviceTimer_Elapsed;
            serviceTimer.Interval = Convert.ToDouble(ConfigurationManager.AppSettings["Duration"].ToString()) * 60 * 1000;
            serviceTimer.Enabled = true;

            time1 = ConfigurationManager.AppSettings["Time1"].ToString();
            time2 = ConfigurationManager.AppSettings["Time2"].ToString();
            time3 = ConfigurationManager.AppSettings["Time3"].ToString();
            time4 = ConfigurationManager.AppSettings["Time4"].ToString();

            serviceTimer.Start();

        }

        void serviceTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Log.ServicLog(DateTime.Now.ToString() + " TIMER ELAPSED");
            //AutomationMain();            
            if (DateTime.Now.ToString("HH") == time1 || DateTime.Now.ToString("HH") == time2 || DateTime.Now.ToString("HH") == time3 || DateTime.Now.ToString("HH") == time4)
                AutomationMain();
        }

        protected override void OnStart(string[] args)
        {
            Log.ServicLog(DateTime.Now.ToString() + " SERVICE STARTED");
            AutomationMain();
        }

        protected override void OnStop()
        {
        }


        #region NewChangesByAkash
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

            return query;

        }
        
        private void DoRequireOperationOnDocuments(RequestDocuments docitem, int OrderNo, int PartNo, string filetype,
                                        Location location, int partListCount, CompanyDetailForEmailEntity objCompany, bool isProcessServer)
        {
            Log.ServicLog("Order NO :" + OrderNo + "  Part NO:" + PartNo);
            string docpath = docitem.DocumentPath.ToString().Trim().Replace(">", "/");
            string docNameDB = OrderNo.ToString() + "_" + PartNo.ToString() + "_" + docitem.DocumentName;

            bool displaySSN = true;

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

                doc = new Aspose.Words.Document(filePath);
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
                if (BillFirm == "GRANCO01")
                {
                    #region GRANCO01
                    _storageRoot = ConfigurationManager.AppSettings["GrangeRoot"].ToString();
                    DirectoryInfo dis = new DirectoryInfo(_storageRoot);
                    if (!dis.Exists)
                        dis.Create();
                    FilePath = _storageRoot + string.Format("{0}-{1}-{2}-{3}", ClaimNo, TStamp, OrderNo, PartNo + "." + filetype);
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
                else if (BillFirm == "HANOAA01")
                {
                    #region HANOAA01
                    _storageRoot = ConfigurationManager.AppSettings["HanoverRoot"].ToString();

                    DirectoryInfo dis = new DirectoryInfo(_storageRoot);
                    if (!dis.Exists)
                        dis.Create();

                    FilePath = _storageRoot + string.Format("{0}_{1}_{2}_{3}-{4}", ClaimNo, AttyName, TStamp, OrderNo, PartNo + "." + filetype);
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
                    ms = null;

                    #endregion
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
                var objFile = new FileEnity();
                objFile.OrderNo = OrderNo;
                objFile.PartNo = PartNo;
                objFile.FileName = outputFileName.Replace("_", "-");
                objFile.FileTypeId = 3;
                objFile.FileDiskName = gid + "." + filetype;
                DbAccess.InsertFile(objFile);
            }
            ms.Dispose();
            #region Location
            var objOrderDetail = DbAccess.GetOrderDetailByOrderNo(OrderNo);
            string OrderAttorney = string.Empty;
            if (location != null)
            {
                //foreach (var location in locationList)
                {
                    if (!string.IsNullOrEmpty(location.SendRequest))
                    {
                        string[] strsplit = location.SendRequest.Split(',');

                        foreach (var action in strsplit)
                        {
                            if (action == "0" || action == "2")
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


                                // var objAccExecutive = DbAccess.GetAccntRepDetail(acctrep);
                                //if (objAccExecutive != null)
                                //{
                                //    ed.AccExeName = objAccExecutive.Name;
                                //    ed.AccExeEmail = objAccExecutive.Email;
                                //}
                                //else
                                //{
                                //    ed.AccExeName = "Josh Sanford";
                                //    ed.AccExeEmail = "Josh.Sanford@axiomcopy.com";
                                //}
                                string additionalEmail = string.Empty;
                                if (pdt == QueryType.Confirmation)
                                {
                                    var notificationList = DbAccess.GetAssistantContactNotification(OrderNo);
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
                                    // IF EMAIL LOCATION EMAIL IS NOT FOUND THEN SEND MAIL TO JACK
                                    if (string.IsNullOrEmpty(location.Email))
                                    {
                                        Log.ServicLog("=========== Email not found ================ ");
                                        string subject = "[Axiom Automation] Email Not Found for Order No " + OrderNo + "-" + PartNo + " Location ID : " + location.LocID;
                                        string body = "We have not found Email for " + OrderNo + "-" + PartNo;
                                        body += "\n Location ID : " + location.LocID;
                                        body += "\n Location Name : " + location.Name1 + " " + location.Name2;
                                        Utility.SendMailWithAttachment("j.alspaugh@axiomcopy.com", body, subject, null, null, "tejaspadia@gmail.com", "");
                                    }
                                    else
                                    {
                                        EmailDocument(doc, fileNames, location.Email, msFile, ed, additionalEmail, true, pdfDocName, objCompany);
                                        Log.ServicLog("Email Sent to : " + location.Email);
                                        string partNotes = string.Empty;
                                        CreateNoteString(OrderNo, PartNo, "Input or sent via Email.", "SYSTEM", ref partNotes, false, false);
                                    }
                                }
                                if (action == "0")
                                {
                                    string faxNumber = location.AreaCode3 + location.FaxNo;
                                    faxNumber = faxNumber.Replace("-", "").Replace(" ", "");
                                    if (string.IsNullOrEmpty(faxNumber))
                                    {
                                        Log.ServicLog("=========== Fax Number not found ================ ");
                                        string subject = "[Axiom Automation] Fax Number Not Found for Order No " + OrderNo + "-" + PartNo + " Location ID : " + location.LocID;
                                        string body = "We have not found Fax Number for " + OrderNo + "-" + PartNo;
                                        body += "\n Location ID : " + location.LocID;
                                        body += "\n Location Name : " + location.Name1 + " " + location.Name2;
                                        Utility.SendMailWithAttachment("j.alspaugh@axiomcopy.com", body, subject, null, null, "tejaspadia@gmail.com", "");
                                    }
                                    else
                                    {
                                        FaxDocument(0, fileNames, faxNumber, location.Name1, msFile);
                                        Log.ServicLog("Fax Sent to : " + faxNumber);
                                        string partNotes = string.Empty;
                                        CreateNoteString(OrderNo, PartNo, "Input or sent via Fax.", "SYSTEM", ref partNotes, false, false);
                                    }
                                }
                                foreach (MemoryStream item in msFile)
                                {
                                    if (item != null)
                                        item.Dispose();
                                }
                                #endregion
                            }
                            else if (action == "1")
                            {
                                Log.ServicLog("Mail - Standard Folder");
                                #region MAIL
                                string MailPath = ConfigurationManager.AppSettings["MailPath"].ToString();
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
                                                Log.ServicLog("============ Exception  641 =============");
                                                Log.ServicLog(ex.ToString());
                                                Log.ServicLog(ex.StackTrace);
                                                Log.ServicLog(ex.Source);
                                                Log.ServicLog(".doc at 643");
                                                Log.ServicLog(fileNames[msCounter]);
                                            }

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
                                            }

                                        }
                                        else
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
                                string strNote = "Input or Sent via Mail.";
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

                            else if (action == "3" || action == "4") //UPLOAD & PROCESS SERVER
                            {
                                isProcessServer = true;
                                string AsgnTo = "";
                                if (action == "3")
                                {
                                    AsgnTo = "UTILRE";
                                    Log.ServicLog("Upload");
                                }
                                else if (action == "4")
                                {

                                    Log.ServicLog("Process Server");
                                    AsgnTo = "REQUES";
                                }
                                DbAccess.UpdateQuickFormOrderPart(OrderNo, PartNo, AsgnTo);
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
                                                Log.ServicLog("============ Exception 783 =============");
                                                Log.ServicLog(ex.ToString());
                                                Log.ServicLog(ex.StackTrace);
                                                Log.ServicLog(ex.Source);
                                                Log.ServicLog(".doc at 776");
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
                                                Log.ServicLog("============ Exception 804 =============");
                                                Log.ServicLog(ex.ToString());
                                                Log.ServicLog(ex.StackTrace);
                                                Log.ServicLog(ex.Source);
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
                                                    Log.ServicLog(ex.ToString());
                                                    Log.ServicLog(ex.StackTrace);
                                                    Log.ServicLog(ex.Source);
                                                    Log.ServicLog(".doc at 768");
                                                    Log.ServicLog(fileNames[msCounter]);
                                                }
                                            }
                                            mstype.Dispose();
                                        }
                                        else
                                        {
                                            try
                                            {

                                                inputDocument = PdfSharp.Pdf.IO.PdfReader.Open(msCombine, PdfSharp.Pdf.IO.PdfDocumentOpenMode.Import);
                                                // inputDocument = PdfSharp.Pdf.IO.PdfReader.Open(msCombine,PdfSharp.Pdf.IO.PdfDocumentOpenMode.ReadOnly);
                                                foreach (PdfSharp.Pdf.PdfPage page in inputDocument.Pages)
                                                    outputDocument.AddPage(page);
                                            }
                                            catch (Exception ex)
                                            {
                                                Log.ServicLog("============ Exception 840 =============");
                                                Log.ServicLog(ex.ToString());
                                                Log.ServicLog(ex.StackTrace);
                                                Log.ServicLog(ex.Source);
                                                Log.ServicLog(".doc at 835");
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
                                DbAccess.InsertMiscChgOrderPart(OrderNo, PartNo);

                                //var fee = DbAccess.GetFee();
                                //if (fee != null)
                                outputDocument.Dispose();
                                inputDocument.Dispose();


                                string partNotes = string.Empty;
                                CreateNoteString(OrderNo, PartNo, "Input or Sent via Certified Mail.", "SYSTEM", ref partNotes, false, false);

                                #endregion

                                foreach (MemoryStream item in msFile)
                                {
                                    if (item != null)
                                        item.Dispose();
                                }
                                mstype = null;
                            }

                        }

                    }

                }
            }

            #endregion
            Log.ServicLog("========================================================================");
        }

        private void ReplaceSubQueryAccordingToQueryType(QueryType pdt, int OrderNo, int PartNo, string[] folders, string docNameDB, int partListCount, RequestDocuments docitem, ref DataTable dtQueryList)
        {
            string subquery = string.Empty;
            var dtsubQuery = new DataTable();
            StringBuilder partInfo = new StringBuilder();
            StringBuilder partInfo2 = new StringBuilder();

            if (pdt == QueryType.Confirmation || pdt == QueryType.TargetSheets || pdt == QueryType.StatusProgressReports)
            {
                subquery = DbAccess.GetQueryByQueryTypeId(Convert.ToInt32(pdt), "SubQuery");
                if (!string.IsNullOrEmpty(subquery))
                {
                    subquery = ReplaceOrderPartNo(subquery, OrderNo, PartNo);

                    dtsubQuery = DbAccess.ExecuteSQLQuery(subquery);
                    partInfo.Append("_____________________________________________________________________________\n\r");
                    for (int a = 0; a < dtsubQuery.Rows.Count; a++)
                    {
                        string partInfoText;
                        if (pdt == QueryType.Confirmation && docitem.DocumentName.ToUpper().Equals("ORDER CONFIRMATION.DOC"))
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
                subquery = DbAccess.GetQueryByQueryTypeId(Convert.ToInt32(pdt), "SubQuery");
                if (!string.IsNullOrEmpty(subquery))
                {
                    subquery = ReplaceOrderPartNo(subquery, OrderNo, PartNo);
                    dtsubQuery = DbAccess.ExecuteSQLQuery(subquery);
                    for (int b = 0; b < dtsubQuery.Rows.Count; b++)
                        locationInfo.Append(Convert.ToString(dtsubQuery.Rows[b]["Location1"]) + '\n');

                }
                var dt3 = dtQueryList;
                dt3.Columns.Add(new DataColumn("Selected_Part", typeof(string)));
                dt3.Columns.Add(new DataColumn("Location1", typeof(string)));
                for (int k = 0; k < dt3.Rows.Count; k++)
                {
                    DataRow dr4 = dt3.Rows[k];
                    dr4["Selected_Part"] = PartNo;
                    dr4["Location1"] = locationInfo;
                }
            }
            else if (pdt == QueryType.CerticicationNOD)
            {
                StringBuilder attyInfo = new StringBuilder();
                attyInfo.Append('\n');
                subquery = DbAccess.GetQueryByQueryTypeId(Convert.ToInt32(pdt), "SubQuery");
                if (!string.IsNullOrEmpty(subquery))
                {
                    subquery = ReplaceOrderPartNo(subquery, OrderNo, PartNo);
                    dtsubQuery = DbAccess.ExecuteSQLQuery(subquery);
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
                subquery = DbAccess.GetQueryByQueryTypeId(Convert.ToInt32(pdt), "SubQuery");
                string[] strQuery = null;
                StringBuilder attyInfo2 = new StringBuilder();
                attyInfo2.Append('\n');
                if (!string.IsNullOrEmpty(subquery))
                {
                    strQuery = subquery.Split(new string[] { "--Split--" }, StringSplitOptions.RemoveEmptyEntries);
                    subquery = ReplaceOrderPartNo(strQuery[0], OrderNo, PartNo);
                    dtsubQuery = DbAccess.ExecuteSQLQuery(subquery);
                    for (int d = 0; d < dtsubQuery.Rows.Count; d++)
                        attyInfo2.Append(System.Runtime.CompilerServices.RuntimeHelpers.GetObjectValue(Convert.ToString(dtsubQuery.Rows[d]["AttyInfo"])));

                    attyInfo2.Append('\n');
                    subquery = ReplaceOrderPartNo(strQuery[1], OrderNo, PartNo);
                    dtsubQuery = DbAccess.ExecuteSQLQuery(subquery);
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
                subquery = DbAccess.GetQueryByQueryTypeId(Convert.ToInt32(pdt), "SubQuery");
                if (!string.IsNullOrEmpty(subquery))
                {
                    subquery = ReplaceOrderPartNo(subquery, OrderNo, PartNo).Replace("%%ATTYNO%%", AttyID);
                    dtsubQuery = DbAccess.ExecuteSQLQuery(subquery);
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
        }
        #endregion


        public void AutomationMain()
        {

            CompanyDetailForEmailEntity objCompany = new CompanyDetailForEmailEntity();



            try
            {
                Log.ServicLog(DateTime.Now.ToString() + "======================= Automation Execution Start ===========================");
                var partList = DbAccess.GetPartList();

                if (partList != null && partList.Count > 0)
                {
                    foreach (var pt in partList)
                    {
                        try
                        {
                            int OrderNo = pt.OrderNo;
                            int PartNo = pt.PartNo;

                            objCompany = DbAccess.CompanyDetailForEmail("CompanyDetailForEmailByOrderNo", pt.OrderNo).FirstOrDefault();


                            bool isProcessServer = false;
                            var location = DbAccess.GetPartLocation(pt.LocID);

                            Log.ServicLog(OrderNo + " " + PartNo);

                            var Attorney = DbAccess.GetAttorneyByOrder(OrderNo);
                            if (Attorney != null)
                            {
                                AttyName = Attorney.AttorneyName;
                                AttyID = Attorney.AttyID;
                            }

                            #region Main Code

                            string filetype = "pdf";
                            var docResult = DbAccess.GetDocsForRequest(OrderNo, Convert.ToString(PartNo));
                            if (docResult != null && docResult.Count > 0)
                            {
                                foreach (var docitem in docResult)
                                {
                                    DoRequireOperationOnDocuments(docitem, OrderNo, PartNo, filetype, location, partList.Count, objCompany, isProcessServer);
                                }

                            }
                            string AsgnTo;
                            DateTime CallBack = DateTime.Now.AddDays(14);
                            if (!isProcessServer)
                            {
                                 AsgnTo = "CBLIST";
                            }
                            else
                            {
                                AsgnTo = "UTILRE";
                            }

                            DbAccess.UpdateOrderPart(OrderNo, PartNo, AsgnTo, CallBack);
                          

                            if (location != null)
                            {

                                if (location.ReqAuthorization == true || location.FeeAmountSendRequest == true)
                                {
                                    DbAccess.UpdateOrderPart(OrderNo, PartNo, "UTILRE", Convert.ToDateTime(pt.CallBack));
                                    string partNotes = string.Empty;
                                    CreateNoteString(OrderNo, PartNo, "Assign to In Office Request.", "SYSTEM", ref partNotes, false, false);
                                }

                            }
                            #endregion Main Code

                        }
                        catch (Exception ex)
                        {
                            Log.ServicLog(DateTime.Now.ToString() + " ----------- EXCEPTION 933 -------------------");
                            Log.ServicLog(Convert.ToString(ex.Message));
                            Log.ServicLog(Convert.ToString(ex.StackTrace));
                            Log.ServicLog(Convert.ToString(ex.InnerException));
                            Log.ServicLog("--------------------------------------------");
                        }


                    }
                }
                GC.Collect();
                Log.ServicLog(DateTime.Now.ToString() + "============================ Automation Execution Complete ============================ ");
            }
            catch (Exception ex)
            {
                Log.ServicLog(DateTime.Now.ToString() + " ----------- EXCEPTION 948  -------------------");
                Log.ServicLog(Convert.ToString(ex.Message));
                Log.ServicLog(Convert.ToString(ex.StackTrace));
                Log.ServicLog(Convert.ToString(ex.InnerException));
                Log.ServicLog("--------------------------------------------");
            }
        }
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

        public void EmailDocument(Aspose.Words.Document doc, List<string> fileName, string email, MemoryStream[] msList, EmailDetails ed, string additionalEmail, bool isMultiple, string MargeFileName, CompanyDetailForEmailEntity objCompany)
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

        public void FaxDocument(int Id, List<string> fileName, string fax, string name, MemoryStream[] msList)
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
