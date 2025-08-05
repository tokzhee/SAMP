using App.Core.BusinessLogic;
using App.Core.Services;
using App.Core.Utilities;
using App.DataModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace App.EmailFlush.Console
{
    public class EmailFlush
    {
        private const string CallerFormName = "Direct on-us Portal-EmailFlush";
        private readonly Timer timer;
        public EmailFlush()
        {
            timer = new Timer()
            {
                Interval = GetRunInterval(),
                AutoReset = true
            };

            timer.Elapsed += TimerElapsed;
        }
        private double GetRunInterval()
        {
            const string callerFormMethod = "GetRunInterval";

            double runInterval = 0;

            try
            {
                runInterval = Convert.ToDouble(ConfigurationUtility.GetAppSettingValue("RunInterval"));
            }
            catch (Exception ex)
            {
                LogUtility.LogError(CallerFormName, callerFormMethod, "", ex);
                LogUtility.LogError(CallerFormName, callerFormMethod, "", "Failed on converting run interval to integer value. kindly ensure run interval value in the config is an integer");
            }

            return runInterval;
        }
        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            FlushEmail();
        }
        public void Start()
        {
            const string callerFormMethod = "Start";

            try
            {
                timer.Start();
            }
            catch (Exception ex)
            {
                LogUtility.LogError(CallerFormName, callerFormMethod, "", ex);
                LogUtility.LogError(CallerFormName, callerFormMethod, "", "Failed to start timer");
            }

            LogUtility.LogInfo(CallerFormName, callerFormMethod, "", $"Direct on-us Portal email flush service started at {DateTime.UtcNow.AddHours(1).ToString("dd-MM-yyyy hh:mm tt")}");
        }
        public void Stop()
        {
            const string callerFormMethod = "Stop";

            try
            {
                timer.Stop();
            }
            catch (Exception ex)
            {
                LogUtility.LogError(CallerFormName, callerFormMethod, "", ex);
                LogUtility.LogError(CallerFormName, callerFormMethod, "", "Failed to stop timer");
            }

            LogUtility.LogInfo(CallerFormName, callerFormMethod, "", $"Direct on-us Portal email flush service stopped at {DateTime.UtcNow.AddHours(1).ToString("dd-MM-yyyy hh:mm tt")}");
        }
        private void FlushEmail()
        {
            const string callerFormMethod = "FlushEmail";
            
            try
            {
                var emailLogs = (from emailLog in EmailLogService.GetAll(CallerFormName, callerFormMethod, "")
                                 where emailLog.sent_flag.Equals(false)
                                 select new emaillog
                                 {
                                     
                                     id = emailLog.id,
                                     subject = emailLog.subject,
                                     message = emailLog.message,
                                     is_message_html_body = emailLog.is_message_html_body,
                                     recipients = emailLog.recipients,
                                     copy = emailLog.copy,
                                     blind_copy = emailLog.blind_copy,
                                     logged_on = emailLog.logged_on,
                                     logged_by = emailLog.logged_by,
                                     send_attempts = emailLog.send_attempts,
                                     last_modified_on = emailLog.last_modified_on,
                                     last_modified_by = emailLog.last_modified_by,
                                     sent_flag = emailLog.sent_flag,
                                     sent_on = emailLog.sent_on,
                                     sent_by = emailLog.sent_by

                                 }).ToList();

                if (emailLogs == null)
                {
                    LogUtility.LogInfo(CallerFormName, callerFormMethod, "", $"The unsent email log records retrieved at {DateTime.UtcNow.AddHours(1).ToString("dd-MM-yyyy hh:mm tt")} is null");
                    return;
                }

                if (emailLogs.Count <= 0)
                {
                    LogUtility.LogInfo(CallerFormName, callerFormMethod, "", $"The unsent email log records retrieved at {DateTime.UtcNow.AddHours(1).ToString("dd-MM-yyyy hh:mm tt")} is 0");
                    return;
                }

                foreach (var emailLog in emailLogs)
                {
                    if (!string.IsNullOrEmpty(Convert.ToString(emailLog.send_attempts)))
                    {
                        if (Convert.ToInt64(emailLog.send_attempts) >= Convert.ToInt64(ConfigurationUtility.GetAppSettingValue("MaximumTryCount")))
                        {
                            LogUtility.LogInfo(CallerFormName, callerFormMethod, "", $"The maximum send attempts has been reached or exceeded for email log record of ID: {emailLog.id} at {DateTime.UtcNow.AddHours(1).ToString("dd-MM-yyyy hh:mm tt")}");
                            continue;
                        }
                    }

                    var emailSendResult = new EmailManager().SendEmail(emailLog.subject, emailLog.message, emailLog.recipients, emailLog.copy, emailLog.blind_copy, emailLog.is_message_html_body, CallerFormName, callerFormMethod, "");
                    if (emailSendResult)
                    {
                        emailLog.send_attempts = (string.IsNullOrEmpty(Convert.ToString(emailLog.send_attempts)) ? 0 : emailLog.send_attempts) + 1;
                        emailLog.last_modified_on = DateTime.UtcNow.AddHours(1);
                        emailLog.last_modified_by = "SYSTEM";
                        emailLog.sent_flag = true;
                        emailLog.sent_on = DateTime.UtcNow.AddHours(1);
                        emailLog.sent_by = "SYSTEM";
                    }
                    else
                    {
                        emailLog.send_attempts = (string.IsNullOrEmpty(Convert.ToString(emailLog.send_attempts)) ? 0 : emailLog.send_attempts) + 1;
                        emailLog.last_modified_on = DateTime.UtcNow.AddHours(1);
                        emailLog.last_modified_by = "SYSTEM";
                        emailLog.sent_flag = false;
                    }

                    var updateResult = EmailLogService.Update(emailLog, CallerFormName, callerFormMethod, "");
                    if (updateResult > 0)
                    {
                        LogUtility.LogInfo(CallerFormName, callerFormMethod, "", $"Email log has been updated for record of ID: {emailLog.id} at {DateTime.UtcNow.AddHours(1).ToString("dd-MM-yyyy hh:mm tt")}");
                    }
                    else
                    {
                        LogUtility.LogInfo(CallerFormName, callerFormMethod, "", $"Email log could not be updated for record of ID: {emailLog.id} at {DateTime.UtcNow.AddHours(1).ToString("dd-MM-yyyy hh:mm tt")}");
                    }
                }

            }
            catch (Exception ex)
            {
                LogUtility.LogError(CallerFormName, callerFormMethod, "", ex);
            }
        }
    }
}
