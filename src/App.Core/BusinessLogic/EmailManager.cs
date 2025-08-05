using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using App.Core.Utilities;

namespace App.Core.BusinessLogic
{
    public class EmailManager
    {
        private readonly string _emailSender;
        private readonly string _emailSenderAlias;
        private readonly string _emailDomain;
        private readonly string _emailServer;
        private readonly string _emailPort;
        private readonly string _emailUsername;
        private readonly string _emailPassword;

        public EmailManager()
        {
            _emailSender = ConfigurationUtility.GetAppSettingValue("EmailSender");
            _emailSenderAlias = ConfigurationUtility.GetAppSettingValue("EmailSenderAlias");
            _emailDomain = ConfigurationUtility.GetAppSettingValue("EmailDomain");
            _emailServer = ConfigurationUtility.GetAppSettingValue("EmailServer");
            _emailPort = ConfigurationUtility.GetAppSettingValue("EmailPort");
            _emailUsername = ConfigurationUtility.GetAppSettingValue("EmailUsername");
            _emailPassword = ConfigurationUtility.GetAppSettingValue("EmailPassword");
        }
        public bool SendEmail(string emailSubject, string emailMessage, string emailRecipients, string emailCopy, string emailBlindCopy, bool isEmailBodyHtml, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            var result = false;

            char[] characterSeparator = { ',' };

            try
            {
                var mailMessage = new MailMessage();

                var mailFrom = new MailAddress(_emailSender, _emailSenderAlias);
                mailMessage.From = mailFrom;

                if (!string.IsNullOrEmpty(emailRecipients))
                {
                    emailRecipients = emailRecipients.Replace(";", ",");
                    var emailRecipientArray = emailRecipients.Split(characterSeparator, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var emailEntry in emailRecipientArray)
                    {
                        var mailTo = new MailAddress(emailEntry);
                        mailMessage.To.Add(mailTo);
                    }
                }

                if (!string.IsNullOrEmpty(emailCopy))
                {
                    emailCopy = emailCopy.Replace(";", ",");
                    var emailCopyArray = emailCopy.Split(characterSeparator, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var emailEntry in emailCopyArray)
                    {
                        var mailCopy = new MailAddress(emailEntry);
                        mailMessage.CC.Add(mailCopy);
                    }
                }

                if (!string.IsNullOrEmpty(emailBlindCopy))
                {
                    emailBlindCopy = emailBlindCopy.Replace(";", ",");
                    var emailBlindCopyArray = emailBlindCopy.Split(characterSeparator, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var emailEntry in emailBlindCopyArray)
                    {
                        var mailCopy = new MailAddress(emailEntry);
                        mailMessage.Bcc.Add(mailCopy);
                    }
                }

                mailMessage.Subject = emailSubject;

                var alternateView = AlternateView.CreateAlternateViewFromString(emailMessage, null, MediaTypeNames.Text.Html);
                mailMessage.AlternateViews.Clear();
                mailMessage.AlternateViews.Add(alternateView);
                mailMessage.IsBodyHtml = isEmailBodyHtml;

                var sendmail = new SmtpClient
                {
                    Host = _emailServer,
                    Port = Convert.ToInt16(_emailPort),
                    Credentials = new NetworkCredential(_emailUsername, _emailPassword)
                };
                sendmail.Send(mailMessage);
                mailMessage.Dispose();

                result = true;
            }
            catch (SmtpException smtpException)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, smtpException);
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return result;
        }
        public bool SendEmail(string emailSubject, string emailMessage, string emailRecipients, string emailCopy, string emailBlindCopy, bool isEmailBodyHtml, byte[] emailAttachment, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            var result = false;

            try
            {
                var mailMessage = new MailMessage();

                var mailFrom = new MailAddress(_emailSender, _emailSenderAlias);
                mailMessage.From = mailFrom;

                char[] characterSeparator = { ',' };
                if (!string.IsNullOrEmpty(emailRecipients))
                {
                    emailRecipients = emailRecipients.Replace(";", ",");
                    var emailRecipientArray = emailRecipients.Split(characterSeparator, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var emailEntry in emailRecipientArray)
                    {
                        var mailTo = new MailAddress(emailEntry);
                        mailMessage.To.Add(mailTo);
                    }
                }

                if (!string.IsNullOrEmpty(emailCopy))
                {
                    emailCopy = emailCopy.Replace(";", ",");
                    var emailCopyArray = emailCopy.Split(characterSeparator, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var emailEntry in emailCopyArray)
                    {
                        var mailCopy = new MailAddress(emailEntry);
                        mailMessage.CC.Add(mailCopy);
                    }
                }

                if (!string.IsNullOrEmpty(emailBlindCopy))
                {
                    emailBlindCopy = emailBlindCopy.Replace(";", ",");
                    var emailBlindCopyArray = emailBlindCopy.Split(characterSeparator, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var emailEntry in emailBlindCopyArray)
                    {
                        var mailCopy = new MailAddress(emailEntry);
                        mailMessage.Bcc.Add(mailCopy);
                    }
                }

                //add attachment
                var memoryStream = new MemoryStream(emailAttachment);
                var attachment = new Attachment(memoryStream, "Loan Offer Letter");
                mailMessage.Attachments.Add(attachment);

                mailMessage.Subject = emailSubject;

                var alternateView = AlternateView.CreateAlternateViewFromString(emailMessage, null, MediaTypeNames.Text.Html);
                mailMessage.AlternateViews.Clear();
                mailMessage.AlternateViews.Add(alternateView);
                mailMessage.IsBodyHtml = isEmailBodyHtml;

                var sendmail = new SmtpClient
                {
                    Host = _emailServer,
                    Port = Convert.ToInt16(_emailPort),
                    Credentials = new NetworkCredential(_emailUsername, _emailPassword)
                };
                sendmail.Send(mailMessage);
                mailMessage.Dispose();

                result = true;
            }
            catch (SmtpException smtpException)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, smtpException);
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return result;
        }
        public bool SendEmail(string emailSubject, string emailMessage, string emailRecipients, string emailCopy, string emailBlindCopy, bool isEmailBodyHtml, string attachmentFilePath, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            var result = false;

            try
            {
                var mailMessage = new MailMessage();

                var mailFrom = new MailAddress(_emailSender, _emailSenderAlias);
                mailMessage.From = mailFrom;

                char[] characterSeparator = { ',' };
                if (!string.IsNullOrEmpty(emailRecipients))
                {
                    emailRecipients = emailRecipients.Replace(";", ",");
                    var emailRecipientArray = emailRecipients.Split(characterSeparator, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var emailEntry in emailRecipientArray)
                    {
                        var mailTo = new MailAddress(emailEntry);
                        mailMessage.To.Add(mailTo);
                    }
                }

                if (!string.IsNullOrEmpty(emailCopy))
                {
                    emailCopy = emailCopy.Replace(";", ",");
                    var emailCopyArray = emailCopy.Split(characterSeparator, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var emailEntry in emailCopyArray)
                    {
                        var mailCopy = new MailAddress(emailEntry);
                        mailMessage.CC.Add(mailCopy);
                    }
                }

                if (!string.IsNullOrEmpty(emailBlindCopy))
                {
                    emailBlindCopy = emailBlindCopy.Replace(";", ",");
                    var emailBlindCopyArray = emailBlindCopy.Split(characterSeparator, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var emailEntry in emailBlindCopyArray)
                    {
                        var mailCopy = new MailAddress(emailEntry);
                        mailMessage.Bcc.Add(mailCopy);
                    }
                }

                //add attachment
                var attachment = new Attachment(attachmentFilePath);
                mailMessage.Attachments.Add(attachment);

                mailMessage.Subject = emailSubject;

                var alternateView = AlternateView.CreateAlternateViewFromString(emailMessage, null, MediaTypeNames.Text.Html);
                mailMessage.AlternateViews.Clear();
                mailMessage.AlternateViews.Add(alternateView);
                mailMessage.IsBodyHtml = isEmailBodyHtml;

                var sendmail = new SmtpClient
                {
                    Host = _emailServer,
                    Port = Convert.ToInt16(_emailPort),
                    Credentials = new NetworkCredential(_emailUsername, _emailPassword)
                };
                sendmail.Send(mailMessage);
                mailMessage.Dispose();

                result = true;
            }
            catch (SmtpException smtpException)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, smtpException);
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return result;
        }
    }
}
