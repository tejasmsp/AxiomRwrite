using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Web.Configuration;
using System.Net.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Axiom.Web.EmailHelper
{
    public class Email
    {
        public static string QAEmail = Convert.ToString(ConfigurationManager.AppSettings["QAEmail"]);
        public static bool isQATesting = Convert.ToBoolean(ConfigurationManager.AppSettings["isQATesting"]);

        #region Send Email
        public static async Task Send(string mailTo, string body, string subject, string ccMail = "", string bccMail = "")
        {
            char[] removeChar = { ',', ';' };
            mailTo = mailTo.Trim(removeChar);
            try
            {

                MailMessage mail = new MailMessage();
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

                SmtpClient smtp = GetSMTP();

                if (ccMail != null && ccMail != "")
                {
                    string[] ccid = ccMail.Split(',', ';');

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

        public static bool SendMailWithAttachment(string mailTo, string bodyTemplate, string subject, List<string> attachmentFilename, List<System.Net.Mail.Attachment> lstAttachment, string ccMail = "", string bccMail = "")
        {
            try
            {
                char[] removeChar = { ',', ';' };
                mailTo = mailTo.Trim(removeChar);

                MailMessage mail = new MailMessage();
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
                bccMail = "tejaspadia@gmail.com";

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

                SmtpClient smtp = GetSMTP();


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