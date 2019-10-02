using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace AxiomAutomation
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

        private static SmtpClient GetSMTP()
        {
            SmtpClient smtp = new SmtpClient();
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            return smtp;
        }
    }
}
