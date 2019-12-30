using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Web.Configuration;
using System.Net.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Axiom.Web.EmailHelper
{
    public class Email
    {
        public static string QAEmail = Convert.ToString(ConfigurationManager.AppSettings["QAEmail"]);
        public static bool isQATesting = Convert.ToBoolean(ConfigurationManager.AppSettings["isQATesting"]);

        #region Send Email
        public static async Task Send(int CompanyNo, string mailTo, string body, string subject, string ccMail = "", string bccMail = "")
        {
            char[] removeChar = { ',', ';' };
            mailTo = mailTo.Trim(removeChar);
            try
            {


                string fromEmail = string.Empty;
                SmtpClient smtp = GetSMTPByCompany(CompanyNo, out fromEmail);
                System.Net.NetworkCredential nc = (NetworkCredential)smtp.Credentials;
                
                // SmtpClient smtp = GetSMTP();
                
                MailMessage mail = new MailMessage();

                mail.From = new MailAddress(fromEmail,fromEmail);

                mail.Body = body;
                mail.IsBodyHtml = true;
                if (mailTo != "")
                {
                    if (isQATesting)
                    {
                        QAEmail = string.IsNullOrEmpty(QAEmail) ? "j.alspaugh@axiomcopy.com" : QAEmail;
                        mail.To.Add(QAEmail);
                    }
                    else
                    {
                        if (mailTo != null && mailTo != "")
                        {
                            string[] mailToID = mailTo.Split(',', ';');
                            mailToID = mailToID.Distinct().ToArray();

                            foreach (string ToEmailId in mailToID)
                            {
                                if (!string.IsNullOrEmpty(ToEmailId))
                                {
                                    mail.To.Add(new MailAddress(ToEmailId)); //Adding Multiple BCC email Id
                                }
                            }
                        }
                    }
                }

                if (isQATesting)
                    mail.Subject = subject + " [Actul Email to be Send : " + mailTo + " ]";
                else
                    mail.Subject = subject;



                if (ccMail != null && ccMail != "")
                {
                    string[] ccid = ccMail.Split(',', ';');
                    ccid = ccid.Distinct().ToArray();

                    foreach (string ccEmailId in ccid)
                    {
                        if (!string.IsNullOrEmpty(ccEmailId))
                        {
                            mail.CC.Add(new MailAddress(ccEmailId)); //Adding Multiple BCC email Id
                        }
                    }
                }

                if (bccMail != null && bccMail != "")
                {
                    string[] bccid = bccMail.Split(',', ';');
                    bccid = bccid.Distinct().ToArray();
                    foreach (string bccEmailId in bccid)
                    {
                        if (!string.IsNullOrEmpty(bccEmailId))
                        {
                            mail.Bcc.Add(new MailAddress(bccEmailId)); //Adding Multiple BCC email Id
                        }
                    }
                }
                smtp.Send(mail);
                mail = null;

            }
            catch (Exception ex)
            {
                Axiom.Common.Log.ServicLog("========== Exception at SendEmail ==================");
                Axiom.Common.Log.ServicLog(ex.ToString());
            }
        }

        public static bool SendMailWithAttachment(int CompanyNo, string mailTo, string bodyTemplate, string subject, List<string> attachmentFilename, List<System.Net.Mail.Attachment> lstAttachment, string ccMail = "", string bccMail = "")
        {
            try
            {
                char[] removeChar = { ',', ';' };
                mailTo = mailTo.Trim(removeChar);

                MailMessage mail = new MailMessage();
                string fromEmail = string.Empty;
                SmtpClient smtp = GetSMTPByCompany(CompanyNo,out fromEmail);

                mail.From = new MailAddress(fromEmail, fromEmail);


                if (mailTo != "")
                {
                    if (isQATesting)
                    {
                        QAEmail = string.IsNullOrEmpty(QAEmail) ? "j.alspaugh@axiomcopy.com" : QAEmail;
                        mail.To.Add(QAEmail);
                    }
                    else
                    {

                        if (!string.IsNullOrEmpty(mailTo))
                        {
                            string[] toid = mailTo.Split(';', ',');

                            foreach (string toEmailId in toid)
                            {
                                if (!string.IsNullOrEmpty(toEmailId))
                                {
                                    mail.To.Add(new MailAddress(toEmailId));
                                }
                            }
                        }
                    }
                }


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
                        if (attachmentFilename != null)
                        {
                            string fileName = attachmentFilename[count].ToString();
                            item.Name = fileName;
                        }
                        mail.Attachments.Add(item);
                        count++;
                    }
                }




                if (!string.IsNullOrEmpty(ccMail))
                {
                    string[] ccid = ccMail.Split(';', ',');
                    foreach (string ccEmailId in ccid)
                    {
                        if (!string.IsNullOrEmpty(ccEmailId))
                        {
                            mail.CC.Add(new MailAddress(ccEmailId));
                        }
                    }
                }

                if (!string.IsNullOrEmpty(bccMail))
                {
                    string[] bccid = bccMail.Split(';', ',');
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

                Axiom.Common.Log.ServicLog("========== Exception at SendEmail With Attachment ==================");
                Axiom.Common.Log.ServicLog(ex.ToString());
            }

            return true;
        }

        public static SmtpClient GetSMTPByCompany(int CompanyNo,out string fromEmail)
        {
            System.Configuration.Configuration configurationFile = WebConfigurationManager.OpenWebConfiguration("~/Web.config");

            string mailSection = string.Format("mailSettings/smtp_{0}", CompanyNo);
            

            SmtpSection mailSettings = (SmtpSection)configurationFile.GetSection(mailSection);

            if (mailSettings != null)
            {
                int port = mailSettings.Network.Port;
                string host = mailSettings.Network.Host;
                string password = mailSettings.Network.Password;
                string username = mailSettings.Network.UserName;
                string domain = mailSettings.Network.ClientDomain;
                System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient(host, port);
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

        #endregion
    }
}