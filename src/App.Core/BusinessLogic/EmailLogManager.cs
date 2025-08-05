using App.Core.Services;
using App.Core.Utilities;
using App.DataModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Core.BusinessLogic
{
    public static class EmailLogManager
    {
        public static void Log(EmailType emailType, string body, string recipient, string copy, string blindCopy, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            try
            {
                var subject = "";

                switch (emailType)
                {
                    case EmailType.LoginNotification:
                        subject = ConfigurationUtility.GetAppSettingValue("LoginNotificationSubject");
                        break;
                    case EmailType.PendingItemsNotification:
                        subject = ConfigurationUtility.GetAppSettingValue("PendingItemsNotificationSubject");
                        break;
                    case EmailType.ApprovalNotification:
                        subject = ConfigurationUtility.GetAppSettingValue("ApprovalNotificationSubject");
                        break;
                    case EmailType.RejectionNotification:
                        subject = ConfigurationUtility.GetAppSettingValue("RejectionNotificationSubject");
                        break;
                    case EmailType.AccountCreationNotification:
                        subject = ConfigurationUtility.GetAppSettingValue("AccountCreationNotificationSubject");
                        break;
                    case EmailType.AccountResetNotification:
                        subject = ConfigurationUtility.GetAppSettingValue("AccountResetNotificationSubject");
                        break;
                    case EmailType.ForgotPasswordNotification:
                        subject = ConfigurationUtility.GetAppSettingValue("ForgotPasswordNotificationSubject");
                        break;
                    default:
                        break;
                }

                var emailLog = new emaillog
                {
                    subject = subject,
                    message = body,
                    is_message_html_body = true,
                    recipients = recipient,
                    copy = copy,
                    blind_copy = blindCopy,
                    logged_on = DateTime.UtcNow.AddHours(1),
                    logged_by = "System",
                    sent_flag = false
                };

                EmailLogService.Insert(emailLog, callerFormName, callerFormMethod, callerIpAddress);
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

        }
    }
}
