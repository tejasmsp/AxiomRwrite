using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Configuration;
using System.Net.Mail;
using System.Configuration;
using System.Net;
using System.IO;
using UploadSweepService.Entity;

namespace UploadSweepService
{
    public static class EmailHelper
    {
        public static string QAEmail = Convert.ToString(ConfigurationManager.AppSettings["QAEmail"]);
        public static bool isQATesting = Convert.ToBoolean(ConfigurationManager.AppSettings["isQATesting"]);

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
        public static SmtpClient GetSMTPByCompany(int CompanyNo, out string fromEmail)
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
        public static void SendMail(CompanyDetailForEmailEntity objCompany,  string mailTo, string body, string subject, string ccMail = "", string bccMail = "")
        {
            char[] removeChar = { ',', ';' };
            mailTo = mailTo.Trim(removeChar);
            try
            {
                // SmtpClient smtp = GetSMTP();
                string fromEmail = string.Empty;
                SmtpClient smtp = GetSMTPByCompany(objCompany.CompNo, out fromEmail);

                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(fromEmail, fromEmail);

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
                
                Log.ServicLog("========== Exception at SendEmail ==================");
                Log.ServicLog(ex.ToString());
            }
        }
    }
}
