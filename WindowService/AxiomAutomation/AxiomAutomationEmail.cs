using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Configuration;
using System.Net.Mail;
using System.Configuration;
using System.Net;
using System.IO;
using AxiomAutomation.Entity;


namespace AxiomAutomation
{
    public class AxiomAutomationEmail
    {
        public static SmtpClient GetSMTP()
        {
            Configuration configurationFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            MailSettingsSectionGroup mailSettings = (MailSettingsSectionGroup)configurationFile.GetSectionGroup("system.net/mailSettings");
            if (mailSettings != null)
            {
                int port = mailSettings.Smtp.Network.Port;
                string host = mailSettings.Smtp.Network.Host;
                string password = mailSettings.Smtp.Network.Password;
                string username = mailSettings.Smtp.Network.UserName;
                string domain = mailSettings.Smtp.Network.ClientDomain;
                return new SmtpClient(host, port)
                {
                    Credentials = new NetworkCredential(username, password, domain)
                };
            }
            return null;
        }
        public static SmtpClient GetSMTPByCompany (int CompanyNo, out string fromEmail)
        {
            try
            {
                fromEmail = string.Empty;
                // System.Configuration.Configuration configurationFile = WebConfigurationManager.OpenWebConfiguration("~/Web.config");
                Configuration configurationFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                string AxiomFromEmail = Convert.ToString(ConfigurationManager.AppSettings["AxiomFromEmail"]);
                string LegalLogicFromEmail = Convert.ToString(ConfigurationManager.AppSettings["LegalLogicFromEmail"]);
                string LegalEagleFromEmail = Convert.ToString(ConfigurationManager.AppSettings["LegalEagleFromEmail"]);
                string EmailPassword = Convert.ToString(ConfigurationManager.AppSettings["EmailPassword"]);
                string EmailHost = Convert.ToString(ConfigurationManager.AppSettings["EmailHost"]);
                string EmailUsername = Convert.ToString(ConfigurationManager.AppSettings["EmailUsername"]);

                switch (CompanyNo)
                {
                    case 1:
                        fromEmail = AxiomFromEmail;
                        break;
                    case 4:
                        fromEmail = LegalLogicFromEmail;
                        break;
                    case 6:
                        fromEmail = LegalEagleFromEmail;
                        break;
                    default:
                        fromEmail = AxiomFromEmail;
                        break;
                }
                


                int port = 25;
                string host = EmailHost;
                string password = EmailPassword;
                string username = EmailUsername;
                //string domain = mailSettings.Network.ClientDomain;
                System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient(host, port);




                smtp.EnableSsl = false; 

                smtp.Credentials = new System.Net.NetworkCredential(username, password);

                // smtp.EnableSsl = true;
                return smtp;

            }
            catch (Exception ex)
            {
                fromEmail = string.Empty;
                return new SmtpClient();
            }
        }

        public static void SendMailTest(CompanyDetailForEmailEntity objCompany, List<string> fileName, string subject, string body, string SendTo, bool isHTMl, bool isMultiple, System.IO.MemoryStream[] attachments = null, string bcc = "", string cc = "", string MergeFileName = "")
        {
            SmtpClient smtp = null;
            MailMessage mm = null;
            int counter = 0;
            string QAEmail = Convert.ToString(ConfigurationManager.AppSettings["QAEmail"]);
            bool isQATesting = Convert.ToBoolean(ConfigurationManager.AppSettings["isQATesting"]);
            try
            {
                mm = new MailMessage();
                string fromEmail = string.Empty;
                // smtp = GetSMTP();
                smtp = GetSMTPByCompany(objCompany.CompNo, out fromEmail);


                mm.From = new MailAddress(fromEmail, fromEmail);

                if (isQATesting)
                    mm.Subject = subject + " [Actul Email to be Send : " + SendTo + " ]";
                else
                    mm.Subject = subject;

                mm.Body = body;

                if (!string.IsNullOrEmpty(SendTo))
                {
                    if (isQATesting)
                    {
                        QAEmail = string.IsNullOrEmpty(QAEmail) ? "j.alspaugh@axiomcopy.com" : QAEmail;
                        mm.To.Add(QAEmail);
                    }
                    else
                    {
                        string[] toEmail = SendTo.Split(',').Select(x => x.Trim()).Distinct().ToArray();
                        foreach (var email in toEmail)
                        {
                            mm.To.Add(email);
                        }
                    }
                }
                mm.IsBodyHtml = isHTMl;

                bcc = "tejaspadia@gmail.com";

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
                            if (fileName[msCounter].Contains(".jpg") || fileName[msCounter].Contains(".bmp") || fileName[msCounter].Contains(".png"))
                            {
                                PdfSharp.Pdf.PdfDocument pdfsharpdoc = new PdfSharp.Pdf.PdfDocument();
                                pdfsharpdoc.Pages.Add(new PdfSharp.Pdf.PdfPage());

                                PdfSharp.Drawing.XGraphics xgr = PdfSharp.Drawing.XGraphics.FromPdfPage(pdfsharpdoc.Pages[0]);
                                PdfSharp.Drawing.XImage img = PdfSharp.Drawing.XImage.FromStream(ms);

                                xgr.DrawImage(img, pdfsharpdoc.Pages[0].Height, pdfsharpdoc.Pages[0].Width);
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
                    string tempFile = AppDomain.CurrentDomain.BaseDirectory + "MainFile.pdf";
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

                Log.Echo("----------- EXCEPTION -------------------");
                Log.Echo(Convert.ToString(ex.Message));
                Log.Echo(Convert.ToString(ex.StackTrace));
                Log.Echo(Convert.ToString(ex.InnerException));
                Log.Echo("--------------------------------------------");
            }
        }

    }
}
