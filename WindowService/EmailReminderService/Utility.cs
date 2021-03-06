﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace EmailReminderService
{
    public static class Utility
    {

        public static string QAEmail = Convert.ToString(ConfigurationManager.AppSettings["QAEmail"]);
        public static bool isQATesting = Convert.ToBoolean(ConfigurationManager.AppSettings["isQATesting"]);

        public static void SendMailBilling(int CompanyNo, string subject, string body, string SendTo, bool isHtml, string SendFrom = "AxiomSupport@axiomcopy.com", List<System.Net.Mail.Attachment> attachments = null, string cc = "", string bcc = "")
        { 
            string fromEmail = string.Empty;
            SmtpClient smtp = GetSMTPByCompany(CompanyNo, out fromEmail);
            System.Net.NetworkCredential nc = (NetworkCredential)smtp.Credentials;

            // SmtpClient smtp = GetSMTP();

            MailMessage mail = new MailMessage();

            mail.From = new MailAddress(fromEmail, fromEmail);

            // MailMessage mail = new MailMessage(SendFrom, SendTo, subject, body);
            //MailMessage mail = new MailMessage();
            if (isQATesting)
                mail.Subject = subject + " [Actul Email to be Send : " + SendTo + " ]";
            else
                mail.Subject = subject;


            mail.Body = body;
            mail.IsBodyHtml = true;

            if (!string.IsNullOrEmpty(SendTo))
            {
                if (!string.IsNullOrEmpty(QAEmail) && isQATesting)
                    mail.To.Add(QAEmail);
                else
                {
                    string[] toEmail = SendTo.Split(',').Select(x => x.Trim()).Distinct().ToArray();
                    foreach (var email in toEmail)
                        mail.To.Add(email);
                }
            }
            if (isQATesting)
            {
                cc = "";
            }
            bcc = "tejaspadia@gmail.com";
            if (!string.IsNullOrEmpty(cc))
            {
                string[] ccEmail = cc.Split(',').Select(x => x.Trim()).Distinct().ToArray();
                foreach (var email in ccEmail)
                {
                    mail.CC.Add(email);
                }
            }

            if (!string.IsNullOrEmpty(bcc))
            {
                string[] bccEmail = bcc.Split(',').Select(x => x.Trim()).Distinct().ToArray();
                foreach (var email in bccEmail)
                {
                    mail.Bcc.Add(email);
                }
            }

            if (attachments != null)
            {
                foreach (var m in attachments)
                {
                    mail.Attachments.Add(m);
                }
            }

            smtp.Send(mail);
            mail.Dispose();
            smtp.Dispose();
            mail = null;
            smtp = null;
        }

        private static SmtpClient GetSMTP()
        {
            SmtpClient smtp = new SmtpClient();
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            return smtp;
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
        public static bool isValidEmail(string inputEmail)
        {
            string strRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                  @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                  @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
            System.Text.RegularExpressions.Regex re = new System.Text.RegularExpressions.Regex(strRegex);
            if (re.IsMatch(inputEmail))
                return (true);
            else
                return (false);
        }

    }
}
