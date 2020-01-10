using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Axiom.Entity;
using System.Net.Configuration;
using System.Web.Configuration;

namespace Axiom.Common
{
    public static class Utility
    {
        public static bool SendMailWithAttachment(string mailTo, string bodyTemplate, string subject, List<string> attachmentFilename, List<System.Net.Mail.Attachment> lstAttachment, string ccMail = "", string bccMail = "")
        {
            try
            {
                string QAEmail = Convert.ToString(ConfigurationManager.AppSettings["QAEmail"]);
                bool isQATesting = Convert.ToBoolean(ConfigurationManager.AppSettings["isQATesting"]);

                MailMessage mail = new MailMessage();
                if (!string.IsNullOrEmpty(mailTo))
                {
                    if (isQATesting)
                    {
                        QAEmail = string.IsNullOrEmpty(QAEmail) ? "j.alspaugh@axiomcopy.com" : QAEmail;
                        mail.To.Add(QAEmail);
                    }
                    else
                    {
                        string[] toEmail = mailTo.Split(',').Select(x => x.Trim()).Distinct().ToArray();
                        foreach (var email in toEmail)
                        {
                            mail.To.Add(mailTo);
                        }
                    }

                }
                bccMail = "tejaspadia@gmail.com";
                if (isQATesting)
                    bccMail = "";

                if (isQATesting)
                    mail.Subject = subject + " [Actul Email to be Send : " + mailTo + " ]";
                else
                    mail.Subject = subject;

                mail.Body = bodyTemplate;
                mail.IsBodyHtml = true;

                if (lstAttachment != null && lstAttachment.Count > 0)
                {
                    int count = 0;
                    foreach (var item in lstAttachment)
                    {
                        string fileName = attachmentFilename[count].ToString();
                        item.Name = fileName;
                        mail.Attachments.Add(item);
                        count++;
                    }
                }

                SmtpClient smtp = GetSMTP();


                if (ccMail != null && ccMail != "")
                {
                    string[] ccid = ccMail.Split(';');
                    mail.To.Add(ccid[0]);
                    foreach (string ccEmailId in ccid)
                    {
                        if (!string.IsNullOrEmpty(ccEmailId))
                        {
                            mail.CC.Add(new MailAddress(ccEmailId));
                        }
                    }
                }

                if (bccMail != null && bccMail != "")
                {
                    string[] bccid = bccMail.Split(';');
                    foreach (string bccEmailId in bccid)
                    {
                        if (!string.IsNullOrEmpty(bccEmailId))
                        {
                            mail.Bcc.Add(new MailAddress(bccEmailId));
                        }
                    }
                }
                smtp.Send(mail);
                mail = null;

            }
            catch (Exception ex)
            {

                throw ex;
            }

            return true;
        }

        public static SmtpClient GetSMTPByCompany(int CompanyNo, out string fromEmail)
        {
            Configuration configurationFile =  WebConfigurationManager.OpenWebConfiguration("~/Web.config");

            string mailSection = string.Format("mailSettings/smtp_{0}", CompanyNo);


            SmtpSection mailSettings = (SmtpSection)configurationFile.GetSection(mailSection);

            if (mailSettings != null)
            {
                int port = mailSettings.Network.Port;
                string host = mailSettings.Network.Host;
                string password = mailSettings.Network.Password;
                string username = mailSettings.Network.UserName;
                string domain = mailSettings.Network.ClientDomain;
                SmtpClient smtp = new SmtpClient(host, port);
                fromEmail = mailSettings.From;


                smtp.EnableSsl = mailSettings.Network.EnableSsl;

                smtp.Credentials = new System.Net.NetworkCredential(username, password, domain);

                // smtp.EnableSsl = true;
                return smtp;
            }
            else
            {
                fromEmail = string.Empty;
            }

            return null;
        }

        public static SmtpClient GetSMTP()
        {
            System.Configuration.Configuration configurationFile = WebConfigurationManager.OpenWebConfiguration("~/Web.config");
            MailSettingsSectionGroup mailSettings = (MailSettingsSectionGroup)configurationFile.GetSectionGroup("system.net/mailSettings");



            if (mailSettings != null)
            {
                int port = mailSettings.Smtp.Network.Port;
                string host = mailSettings.Smtp.Network.Host;
                string password = mailSettings.Smtp.Network.Password;
                string username = mailSettings.Smtp.Network.UserName;
                string domain = mailSettings.Smtp.Network.ClientDomain;
                System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient(host, port);
                smtp.Credentials = new System.Net.NetworkCredential(username, password, domain);
                // smtp.EnableSsl = true;
                return smtp;
            }
            return null;
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
