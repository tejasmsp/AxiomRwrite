using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aspose.Words;
using Aspose.Words.Drawing;
using System.Drawing.Printing;
using System.IO;
using QuickFormService.Entity;
using System.Net.Mail;
using System.Net;
using System.Net.Configuration;
using Aspose.Pdf.Facades;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace QuickFormService
{
    public static class Utility
    {
        public static void InsertWatermarkText(Aspose.Words.Document doc, string watermarkText)
        {
            // Create a watermark shape. This will be a WordArt shape. 
            // You are free to try other shape types as watermarks.
            Shape watermark = new Shape(doc, ShapeType.TextPlainText);

            // Set up the text of the watermark.
            watermark.TextPath.Text = watermarkText;
            watermark.TextPath.FontFamily = "Arial";
            watermark.Width = 500;
            watermark.Height = 100;
            // Text will be directed from the bottom-left to the top-right corner.
            watermark.Rotation = -40;
            //   watermark.BehindText
            // Remove the following two lines if you need a solid black text.
            watermark.Fill.Color = System.Drawing.Color.LightGray; // Try LightGray to get more Word-style watermark
            watermark.StrokeColor = System.Drawing.Color.LightGray; // Try LightGray to get more Word-style watermark
            doc.FirstSection.Body.FirstParagraph.ParagraphFormat.Shading.BackgroundPatternColor = System.Drawing.Color.Empty;
            //   doc.FirstSection.Body.Paragraphs[10].ParagraphFormat.Shading.BackgroundPatternColor = Color.Empty; 
            // Place the watermark in the page center.
            watermark.RelativeHorizontalPosition = RelativeHorizontalPosition.Page;
            watermark.RelativeVerticalPosition = RelativeVerticalPosition.Page;
            watermark.WrapType = WrapType.None;
            watermark.VerticalAlignment = Aspose.Words.Drawing.VerticalAlignment.Center;
            watermark.HorizontalAlignment = Aspose.Words.Drawing.HorizontalAlignment.Center;

            // Create a new paragraph and append the watermark to this paragraph.
            Paragraph watermarkPara = new Paragraph(doc);
            watermarkPara.AppendChild(watermark);

            ArrayList hfTypes = new ArrayList();
            hfTypes.Add(HeaderFooterType.HeaderPrimary);
            hfTypes.Add(HeaderFooterType.HeaderFirst);
            hfTypes.Add(HeaderFooterType.HeaderEven);
            foreach (Section sect in doc.Sections)
            {
                foreach (HeaderFooterType hftype in hfTypes)
                {

                    if (sect.HeadersFooters[hftype] == null)
                    {
                        //If there is no header of the specified type in the current section we should create new header
                        if (hftype == HeaderFooterType.HeaderPrimary ||
                            hftype == HeaderFooterType.HeaderFirst && sect.PageSetup.DifferentFirstPageHeaderFooter ||
                            hftype == HeaderFooterType.HeaderEven && sect.PageSetup.OddAndEvenPagesHeaderFooter)
                        {
                            Aspose.Words.HeaderFooter hf = new Aspose.Words.HeaderFooter(doc, hftype);
                            //Insert clone of watermarkPar into the header
                            hf.AppendChild(watermarkPara.Clone(true));
                            sect.HeadersFooters.Add(hf);
                        }
                    }
                    else
                    {
                        //If the header of specified type exists then just insert watermark
                        sect.HeadersFooters[hftype].AppendChild(watermarkPara.Clone(true));
                    }
                }
            }

        }

        public static void PrintDocument(Document doc, int Id)
        {
            try
            {
                string printerName = "";
                printerName = ConfigurationManager.AppSettings["PrinterName"];
                bool printerInstalled = false;
                try
                {
                    IEnumerator enumerator = PrinterSettings.InstalledPrinters.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        string printer = Convert.ToString(enumerator.Current);
                        if (string.Compare(printer.Trim().ToUpper(), printerName.Trim().ToUpper(), false) == 0)
                        {
                            printerInstalled = true;
                        }
                    }
                }
                finally
                {
                    IEnumerator enumerator = null;
                    if (enumerator is IDisposable)
                    {
                        (enumerator as IDisposable).Dispose();
                    }
                }
                if (printerInstalled)
                {
                    License license = new License();
                    license.SetLicense("Aspose.Words.lic");
                    doc.Print(printerName);
                    DbAccess.UpdatePrintStatus(Id);
                }
                else
                {
                    Log.ServicLog(printerName + " Not installed.");
                }
            }
            catch (Exception ex)
            {
                Log.ServicLog(ex.Message);
                Log.ServicLog("---------------- QUICKFORMID : " + Id + " ---------------------");
                Log.ServicLog(ex.StackTrace.ToString());
            }
        }

        public static void EmailDocument(Document doc, int Id, List<string> fileName, string email, MemoryStream[] msList, EmailDetails ed, string additionalEmail, int IsRevised, string IsRevisedText, bool isMultiple)
        {
            try
            {
                MemoryStream msTemp = new MemoryStream();
                if (IsRevised == 1)
                {
                    InsertWatermarkText(doc, IsRevisedText);
                }
                doc.Save(msTemp, SaveFormat.Pdf);
                msTemp.Position = 0L;
                msList[msList.Count() - 1] = msTemp;
                DbAccess.UpdateEmailStatus(Id);
                string body = string.Empty;
                string htmlfilePath = AppDomain.CurrentDomain.BaseDirectory + "HtmlTemplates\\AuthorizationEmail.html";
                using (StreamReader reader = new StreamReader((htmlfilePath)))
                {
                    body = reader.ReadToEnd();
                }
                body = body.Replace("{CAUSECAPTION}", ed.Caption);
                body = body.Replace("{PATIENTNAME}", ed.PatientName);
                body = body.Replace("{EXNAME}", ed.AccExeName);
                body = body.Replace("{EXEMAIL}", ed.AccExeEmail);

                string subject = string.IsNullOrEmpty(ed.Caption) ? "Quick Forms Document" : ed.Caption;
                SendMailTest(fileName, subject, body, email, true, isMultiple, "AxiomSupport@axiomcopy.com", msList, "autoemail@axiomcopy.com,tejaspadia@gmail.com", additionalEmail);
                DbAccess.UpdateEmailStatus(Id);
            }
            catch (Exception ex)
            {
                Log.ServicLog(ex);
                Log.ServicLog("---------------- QUICKFORMID : " + Id + " ---------------------");
                Log.ServicLog(ex.Message);
                Log.ServicLog(ex.StackTrace.ToString());
            }
        }

        public static SmtpClient GetSMTP()
        {
            SmtpClient smtpClient;
            try
            {
                SmtpSection section = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
                if (section == null)
                {
                    smtpClient = null;
                }
                else
                {
                    int port = section.Network.Port;
                    string host = section.Network.Host;
                    string password = section.Network.Password;
                    string userName = section.Network.UserName;
                    string clientDomain = section.Network.ClientDomain;
                    SmtpClient smtpClient1 = new SmtpClient(host, port)
                    {
                        Credentials = new NetworkCredential(userName, password, clientDomain)
                    };
                    smtpClient = smtpClient1;
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return smtpClient;
           
        }

        public static void SendMailTest(List<string> fileName, string subject, string body, string SendTo, bool isHTMl, bool isMultiple, string SendFrom = "AxiomSupport@axiomcopy.com", System.IO.MemoryStream[] attachments = null, string bcc = "", string cc = "", string MergeFileName = "")
        {
            string QAEmail = Convert.ToString(ConfigurationManager.AppSettings["QAEmail"]);
            bool isQATesting = Convert.ToBoolean(ConfigurationManager.AppSettings["isQATesting"]);

            SmtpClient smtp = null;
            MailMessage mm = null;
            int counter = 0;
            try
            {
                mm = new MailMessage();
                smtp = GetSMTP();
                MailAddress from = new MailAddress(SendFrom, "Axiom Support");

                mm.From = from;
                mm.Body = body;

                if (isQATesting)
                    mm.Subject = subject + "Actual Email To be Send To[" + SendTo + "] - QuickForm";
                else
                    mm.Subject = subject;
                 
                if (!string.IsNullOrEmpty(SendTo))
                {
                    if (!string.IsNullOrEmpty(QAEmail) && isQATesting)
                        mm.To.Add(QAEmail);
                    else
                    {
                        string[] toEmail = SendTo.Split(',').Select(x => x.Trim()).Distinct().ToArray();
                        foreach (var email in toEmail)
                        {
                            mm.To.Add(email);
                        }
                    }
                }
                
                if (isQATesting)
                {
                    
                }
                
                //if (mm.To == null)
                //{
                //    mm.To.Add(SendTo);
                //    cc = string.Empty;
                //    bcc = string.Empty;
                //}
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
                    //Aspose.Pdf.License license = new Aspose.Pdf.License();
                    //license.Embedded = true;
                    //license.SetLicense("Aspose.Pdf.lic");
                    //MemoryStream pdfStream = new MemoryStream();
                    //PdfFileEditor pdfEditor = new PdfFileEditor();
                    //pdfEditor.Concatenate(attachments, pdfStream);
                    //Attachment attachment1 = new Attachment(pdfStream, MergeFileName);
                    //mm.Attachments.Add(attachment1);

                    int msCounter = 0;
                    PdfSharp.Pdf.PdfDocument outputDocument = new PdfSharp.Pdf.PdfDocument();
                    PdfSharp.Pdf.PdfDocument inputDocument = new PdfSharp.Pdf.PdfDocument();
                    MemoryStream mstype;
                    foreach (MemoryStream ms in attachments)
                    {
                        if (ms != null)
                        {
                            mstype = new MemoryStream();
                            if (fileName[msCounter].Contains(".jpg") || fileName[msCounter].Contains(".bmp") || fileName[msCounter].Contains(".png"))
                            {
                                PdfSharp.Pdf.PdfDocument pdfsharpdoc = new PdfSharp.Pdf.PdfDocument();
                                pdfsharpdoc.Pages.Add(new PdfSharp.Pdf.PdfPage());

                                PdfSharp.Drawing.XGraphics xgr = PdfSharp.Drawing.XGraphics.FromPdfPage(pdfsharpdoc.Pages[0]);
                                PdfSharp.Drawing.XImage img = PdfSharp.Drawing.XImage.FromStream(ms);

                                xgr.DrawImage(img,0,0,595, 842);
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
                    string tempFile = AppDomain.CurrentDomain.BaseDirectory + "Templates/MainFile.pdf";
                    if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "Templates"))
                    {
                        Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "Templates");
                    }
                    if (File.Exists(tempFile))
                        File.Delete(tempFile);
                    if (outputDocument.PageCount > 0)
                    {
                        outputDocument.Save(tempFile);
                        MemoryStream msAttach = new MemoryStream();
                        using (System.IO.FileStream fstream = System.IO.File.OpenRead(tempFile))
                        {
                            msAttach.SetLength(fstream.Length);
                            fstream.Read(msAttach.GetBuffer(), 0, (int)fstream.Length);
                            msAttach.Position = 0L;
                        }
                        mm.Attachments.Add(new Attachment(msAttach, MergeFileName, "application/pdf"));
                    }
                }
                else
                {
                    try
                    {
                        foreach (var attachment in attachments)
                        {
                            if (attachment != null)
                                mm.Attachments.Add(new Attachment(attachment, fileName[counter].Replace(".doc", ".pdf")));

                            counter++;
                            Log.ServicLog("Send Mail =" + SendTo + "File Name is: " + fileName[counter]);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.ServicLog(ex.ToString());
                    }
                    
                }
                smtp.Send(mm);
                
                mm.Dispose();
                smtp.Dispose();
            }
            catch (Exception ex)
            {
                Log.ServicLog("FROM EMAIL");
                Log.ServicLog(ex.Message);
                Log.ServicLog(ex.StackTrace.ToString());
                Log.ServicLog(ex.ToString());
                Log.ServicLog("FROM EMAIL END");
            }
        }


        public static void FaxDocument(Document doc, int Id, List<string> fileName, string fax, string name, MemoryStream[] msList)
        {

            int msCounter = 0;

            try
            {

                int counter = 0;
                Aspose.Words.License license = new Aspose.Words.License();
                license.SetLicense("Aspose.Words.lic");
                System.IO.MemoryStream msTemp = new System.IO.MemoryStream();
                doc.Save(msTemp, SaveFormat.Pdf);


                string tempFile = AppDomain.CurrentDomain.BaseDirectory + "MainFile.pdf";
                if (System.IO.File.Exists(tempFile))
                {
                    System.IO.File.Delete(tempFile);
                }

                msTemp.Position = 0;

                PdfSharp.Pdf.PdfDocument outputDocument = new PdfSharp.Pdf.PdfDocument();
                PdfSharp.Pdf.PdfDocument inputDocument = new PdfSharp.Pdf.PdfDocument();
                inputDocument = PdfSharp.Pdf.IO.PdfReader.Open(msTemp, PdfSharp.Pdf.IO.PdfDocumentOpenMode.Import);
                foreach (PdfSharp.Pdf.PdfPage page in inputDocument.Pages)
                {
                    outputDocument.AddPage(page); //throws the exception !!!
                }

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
                            {
                                outputDocument.AddPage(page);
                            }
                        }
                        else if (fileName[msCounter].Contains(".doc") || fileName[msCounter].Contains(".docx"))
                        {
                            Document docWord = new Document(ms);
                            docWord.Save(msImage, SaveFormat.Pdf);
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
                    }
                    msCounter++;
                }

                outputDocument.Save(tempFile);

                MemoryStream msSendFax = new MemoryStream();
                using (FileStream fs = new FileStream(tempFile, FileMode.Open, System.IO.FileAccess.Read))
                {
                    byte[] bytes = new byte[fs.Length];
                    fs.Read(bytes, 0, (int)fs.Length);
                    msSendFax.Write(bytes, 0, (int)fs.Length);
                }
                msSendFax.Position = 0;

                //PrintDocuments.SendMailTest("Documents", "Quick forms Document", "Please find the enclosed attachment for Quick forms", "tejas.padia@gmail.com", "AxiomSupport@axiomcopy.com", msSendFax);

                string base64String = Convert.ToBase64String(msSendFax.ToArray());
                string xmlFileName = GenrateXMLDocument(base64String, fileName[counter], fax, name);
                SendFax(xmlFileName);
                DbAccess.UpdateFaxStatus(Id);
            }
            catch (Exception ex)
            {
                Log.ServicLog("---------------- QUICKFORMID : " + Id + " ---------------------");
                Log.ServicLog("---------------- QUICKFORMID : " + fileName[msCounter].ToString() + " ---------------------");
                Log.ServicLog(ex.Message);
                Log.ServicLog(ex.StackTrace.ToString());
            }
        }

        private static void SendFax(string xmlFileName)
        {
            try
            {
                ServicePointManager.Expect100Continue = false;
                
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });
                
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                    | SecurityProtocolType.Tls11
                    | SecurityProtocolType.Tls12
                    | SecurityProtocolType.Ssl3;
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
                Log.ServicLog(ex.Message);
                Log.ServicLog(ex.StackTrace.ToString());
            }
        }
        private static string GenrateXMLDocument(string base64String, string attachmentName, string faxNo, string name)
        {
            string GenrateXMLDocument;
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
                Log.ServicLog(ex.Message);
                Log.ServicLog(ex.StackTrace.ToString());
                throw ex;
            }
            return GenrateXMLDocument;
        }

    }
}
