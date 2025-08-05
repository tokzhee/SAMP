using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using App.Core.BusinessLogic;
using App.Core.Services;
using App.Core.Utilities;
using App.DataModels.Models;
using App.Web.Models;
using App.Web.ViewModels;
using Newtonsoft.Json;
using PasswordGenerator;

namespace App.Web.Controllers
{
    [RoutePrefix("account")]
    public class AccountController : BaseController
    {
        private const string CallerFormName = "AccountController";
        private readonly string callerIpAddress;
        private readonly string callerMacAddress;

        //view models
        private readonly LoginViewModel loginViewModel;
        private readonly ChangePasswordViewModel changePasswordViewModel;
        private bool stopCheckFlag = false;
        private string stopCheckMessage = "";

        private readonly string encryptionKey;

        public AccountController()
        {
            callerIpAddress = IpAddressManager.GetClientComputerIpAddress();
            callerMacAddress = MacAddressManager.GetClientComputerMacAddress();
            loginViewModel = new LoginViewModel(CallerFormName, "ctor", callerIpAddress);
            changePasswordViewModel = new ChangePasswordViewModel();

            encryptionKey = ConfigurationUtility.GetAppSettingValue("EncryptionKey");
        }

        #region ActionResult

        [HttpGet]
        [AllowAnonymous]
        [Route("~/")]
        [Route("~/account")]
        [Route("login")]
        public ActionResult Login()
        {
            //var response = NIBSSBvnValidatorManager.ValidateBvn("22222222943", "","","");
            //var response1 = new  CRMSManager().CreditCheck("22256605204", "","","");
            //var response2 = new CRMSManager().CreditCheckSummary("22256605204", "", "", "");
            return View(loginViewModel);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [AllowAnonymous]
        [Route("~/")]
        [Route("~/account")]
        [Route("login")]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            const string callerFormMethod = "HttpPost|Login";

            try
            {
                if (!ModelState.IsValid)
                {
                    var errorMessages = string.Join("<br/>", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
                    stopCheckFlag = true;
                    stopCheckMessage = errorMessages;
                }

                user user = null;
                if (!stopCheckFlag)
                {
                    user = UsersService.GetWithUsername(model.Username, CallerFormName, callerFormMethod, callerIpAddress);
                    if (user == null)
                    {
                        AuditLoginAttemptRemarks(model.Username, "Login Failure|User is not profiled on the system", CallerFormName, callerFormMethod, callerIpAddress, callerMacAddress);
                        stopCheckFlag = true;
                        stopCheckMessage = "Login Failure";
                    }
                }
                
                if (!stopCheckFlag)
                {
                    if (user.authentication_type_id.Equals((int)UserAccountAuthenticationType.LocalAccountAuthentication))
                    {
                        if (!model.Password.Equals(EncryptionUtility.Decrypt(user.local_password, encryptionKey)))
                        {
                            AuditLoginAttemptRemarks(model.Username, "Login Failure|Local Account Authentication Failed", CallerFormName, callerFormMethod, callerIpAddress, callerMacAddress);
                            stopCheckFlag = true;
                            stopCheckMessage = "Login Failure";
                        }
                    }
                    else if (user.authentication_type_id.Equals((int)UserAccountAuthenticationType.ActiveDirectoryAuthentication))
                    {
                        var adAuthenticationResult = ActiveDirectoryManager.AuthenticateUser(model.Username, model.Password, CallerFormName, callerFormMethod, callerIpAddress);
                        if (!adAuthenticationResult)
                        {
                            AuditLoginAttemptRemarks(model.Username, "Login Failure|Active Directory Authentication Failed", CallerFormName, callerFormMethod, callerIpAddress, callerMacAddress);
                            stopCheckFlag = true;
                            stopCheckMessage = "Login Failure";
                        }
                    }
                    else
                    {
                        AuditLoginAttemptRemarks(model.Username, "Login Failure|Invalid Authentication Type", CallerFormName, callerFormMethod, callerIpAddress, callerMacAddress);
                        stopCheckFlag = true;
                        stopCheckMessage = "Login Failure";
                    }
                }
                
                if (!stopCheckFlag)
                {
                    var tokenAuthenticationResult = TokenManager.AuthenticateUserToken(model.Username, model.AccessToken, CallerFormName, callerFormMethod, callerIpAddress);
                    if (!tokenAuthenticationResult)
                    {
                        AuditLoginAttemptRemarks(model.Username, "Login Failure|Token Authentication Failed", CallerFormName, callerFormMethod, callerIpAddress, callerMacAddress);
                        stopCheckFlag = true;
                        stopCheckMessage = "Login Failure";
                    }
                }
                
                var loginResult = "";
                if (!stopCheckFlag)
                {
                    loginResult = ValidateUser(user, CallerFormName, callerFormMethod, callerIpAddress, callerMacAddress);
                    if (!string.IsNullOrEmpty(loginResult))
                    {
                        if (!Convert.ToBoolean(loginResult.Split('|').GetValue(0).ToString()))
                        {
                            AuditLoginAttemptRemarks(model.Username, loginResult, CallerFormName, callerFormMethod, callerIpAddress, callerMacAddress);
                            stopCheckFlag = true;
                            stopCheckMessage = loginResult.Split('|').GetValue(1).ToString();
                        }
                    }
                    else
                    {
                        stopCheckFlag = true;
                        stopCheckMessage = "Login Failure";
                    }
                }
                
                if (!stopCheckFlag)
                {
                    var authenticationCookie = LoginUser(loginResult, CallerFormName, callerFormMethod, callerIpAddress, callerMacAddress);
                    if (authenticationCookie != null)
                    {
                        HttpContext.Response.Cookies.Add(authenticationCookie);

                        //Task 1
                        new Task(() =>
                        {
                            var person = PersonService.GetWithPersonId(Convert.ToString(user.person_id), CallerFormName, callerFormMethod, callerIpAddress);
                            var templatePath = Server.MapPath(ConfigurationUtility.GetAppSettingValue("LoginNotificationBodyTemplate"));
                            var mailBody = System.IO.File.ReadAllText(templatePath).Replace("{Name}", person?.first_name).Replace("{LogonDate}", DateTime.UtcNow.AddHours(1).ToString("dd-MM-yyyy hh:mm tt"));
                            var mailRecipient = person?.email_address;
                            EmailLogManager.Log(EmailType.LoginNotification, mailBody, mailRecipient, "", "", CallerFormName, callerFormMethod, callerIpAddress);

                        }).Start();

                        if (!string.IsNullOrEmpty(returnUrl) && HttpUtility.UrlDecode(returnUrl) != $"{ConfigurationUtility.GetAppSettingValue("ApplicationDeployedFolderPath")}/account/logout")
                        {
                            return Redirect(returnUrl);
                        }

                        return RedirectToAction("dashboard", "home");
                    }
                }
                else
                {
                    AlertUser(stopCheckMessage, AlertType.Error);
                }

            }
            catch (Exception ex)
            {
                LogUtility.LogError(CallerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return View(loginViewModel);
        }

        [HttpGet]
        [Authorize]
        [Route("logout")]
        public ActionResult Logout()
        {
            const string callerFormMethod = "HttpGet|Logout";

            try
            {
                var existingAuthenticationCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                if (existingAuthenticationCookie != null)
                {
                    AuditLogoutRemarks(existingAuthenticationCookie.Value, CallerFormName, callerFormMethod, callerIpAddress, callerMacAddress);
                }

                HttpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddHours(1).AddMinutes(-1000));
                HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                HttpContext.Response.Cache.SetNoStore();

                var authenticationCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                if (authenticationCookie != null)
                {
                    authenticationCookie.Expires = DateTime.UtcNow.AddHours(1).AddMinutes(-1000);
                    Response.Cookies.Set(authenticationCookie);
                }

                authenticationCookie = new HttpCookie(FormsAuthentication.FormsCookieName, "")
                {
                    Expires = DateTime.UtcNow.AddHours(1).AddMinutes(-1000)
                };
                Response.Cookies.Add(authenticationCookie);

                FormsAuthentication.SignOut();

                Session.Clear();
                Session.Abandon();
                Session.RemoveAll();
            }
            catch (Exception ex)
            {
                LogUtility.LogError(CallerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        [Authorize]
        [Route("change-password")]
        public ActionResult ChangePassword(string q)
        {
            const string callerFormMethod = "HttpGet|ChangePassword";

            try
            {
                changePasswordViewModel.UrlQueryString = q;
            }
            catch (Exception ex)
            {
                LogUtility.LogError(CallerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return View(changePasswordViewModel);
        }

        [HttpPost]
        [Authorize]
        [Route("change-password")]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(string urlQueryString, string currentPassword, string newPassword, string confirmNewPassword)
        {
            const string callerFormMethod = "HttpPost|ChangePassword";

            try
            {
                if (!ValidationUtility.IsValidTextInput(currentPassword))
                {
                    stopCheckFlag = true;
                    stopCheckMessage = "Unable to complete request; Please provide your current password to proceed.";
                }

                if (!stopCheckFlag)
                {
                    if (!ValidationUtility.IsValidPasswordStrength(newPassword))
                    {
                        stopCheckFlag = true;
                        stopCheckMessage = "Unable to complete request; Passwords must be at least 8 characters and contain at 3 of 4 of the following: upper case (A-Z), lower case (a-z), number (0-9) and special character (e.g. !@#$%^&*)";
                    }
                }
                
                if (!stopCheckFlag)
                {
                    if (!ValidationUtility.IsComparableStrings(newPassword, confirmNewPassword))
                    {
                        stopCheckFlag = true;
                        stopCheckMessage = "Unable to complete request; The passwords do not match.";
                    }
                }
                
                user user = null;
                if (!stopCheckFlag)
                {
                    user = GetUser(CallerFormName, callerFormMethod, callerIpAddress);
                    if (!ValidationUtility.IsComparableStrings(urlQueryString, Convert.ToString(user.query_string)))
                    {
                        stopCheckFlag = true;
                        stopCheckMessage = "Unable to complete request; field tampering observed!";
                    }
                }
                
                if (!stopCheckFlag)
                {
                    user = UsersService.GetWithUrlQueryString(urlQueryString, CallerFormName, callerFormMethod, callerIpAddress);
                    if (user == null)
                    {
                        stopCheckFlag = true;
                        stopCheckMessage = "Unable to retrieve existing user at the moment, kindly try again later";
                    }
                }

                if (!stopCheckFlag)
                {
                    if (!currentPassword.Equals(EncryptionUtility.Decrypt(user.local_password, encryptionKey)))
                    {
                        stopCheckFlag = true;
                        stopCheckMessage = "Unable to complete request; Current password provided is incorrect.";
                    }
                }
                
                if (!stopCheckFlag)
                {
                    if (!user.authentication_type_id.Equals((int)UserAccountAuthenticationType.LocalAccountAuthentication))
                    {
                        stopCheckFlag = true;
                        stopCheckMessage = "Unable to complete request; user account is not on local authentication";
                    }
                }

                if (!stopCheckFlag)
                {
                    var lastHistory = PasswordChangeLogService.GetLatestTwelveRecordsWithPersonId(Convert.ToString(user.person_id), CallerFormName, callerFormMethod, callerIpAddress);
                    if (lastHistory.Count > 0)
                    {
                        if (lastHistory.Any(c => c.password.Equals(EncryptionUtility.Encrypt(newPassword, encryptionKey))))
                        {
                            stopCheckFlag = true;
                            stopCheckMessage = "Unable to complete request; new password has been used before";
                        }
                    }
                }

                long userUpdateResult = 0;
                if (!stopCheckFlag)
                {
                    user.local_password = EncryptionUtility.Encrypt(newPassword, encryptionKey);
                    if (string.IsNullOrEmpty(Convert.ToString(user.password_expiry_date)))
                    {
                        user.password_expiry_date = DateTime.UtcNow.AddDays(Convert.ToDouble(ConfigurationUtility.GetAppSettingValue("PasswordValidityPeriod")));
                    }
                    else
                    {
                        if (DateTime.UtcNow.AddHours(1) > user.password_expiry_date)
                        {
                            user.password_expiry_date = DateTime.UtcNow.AddDays(Convert.ToDouble(ConfigurationUtility.GetAppSettingValue("PasswordValidityPeriod")));
                        }
                    }

                    userUpdateResult = UsersService.Update(user, CallerFormName, callerFormMethod, callerIpAddress);
                    if (userUpdateResult <= 0)
                    {
                        stopCheckFlag = true;
                        stopCheckMessage = "Unable to complete request at the moment; password could not be updated. Kindly try again later!";
                    }
                }

                long historyLogResult = 0;
                if (!stopCheckFlag)
                {
                    var passwordChangeLog = new passwordchangelog
                    {
                        person_id = user.person_id,
                        username = user.username,
                        password = user.local_password,
                        logged_on  = DateTime.UtcNow.AddHours(1)
                    };

                    historyLogResult = PasswordChangeLogService.Insert(passwordChangeLog, CallerFormName, callerFormMethod, callerIpAddress);
                    if (historyLogResult <= 0)
                    {
                        //
                    }
                }


                if (!stopCheckFlag)
                {
                    //Task 1
                    new Task(() =>
                    {
                        var appActivity = new appactivity
                        {
                            audit_username = user.username,
                            audit_fullname = $"{user.person?.surname}, {user.person?.first_name} {user.person?.middle_name}",
                            audit_ipaddress = callerIpAddress,
                            audit_macaddress = callerMacAddress,
                            operation = $"Password Change".ToUpper(),
                            comments = $"'{user.username}' has changed password",
                            created_on = DateTime.Now,
                            created_by = "System"
                        };
                        AppActivityService.Insert(appActivity, CallerFormName, callerFormMethod, callerIpAddress);

                    }).Start();

                    AlertUser("You have successfully changed your password. Kindly ensure you use your new password to logon on next login attempt.", AlertType.Success);
                }
                else
                {
                    AlertUser(stopCheckMessage, AlertType.Error);
                }

            }
            catch (Exception ex)
            {
                LogUtility.LogError(CallerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return RedirectToAction("MyProfile", "Home");
        }
        
        [HttpPost]
        [Route("forgot-password")]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(string username)
        {
            const string callerFormMethod = "HttpPost|ForgotPassword";

            var result = new Dictionary<string, object>
            {
                {"ResponseCode", null },
                {"ResponseMessage", null }
            };

            try
            {
                if (!ValidationUtility.IsValidTextInput(username))
                {
                    stopCheckFlag = true;
                    stopCheckMessage = "Unable to complete request; Please provide your username to proceed.";
                }

                user user = null;
                if (!stopCheckFlag)
                {
                    user = UsersService.GetWithUsername(username, CallerFormName, callerFormMethod, callerIpAddress);
                    if (user == null)
                    {
                        stopCheckFlag = true;
                        stopCheckMessage = "Unable to complete request; Invalid username.";
                    }
                }

                if (!stopCheckFlag)
                {
                    if (user.authentication_type_id.Equals((int)UserAccountAuthenticationType.ActiveDirectoryAuthentication))
                    {
                        stopCheckFlag = true;
                        stopCheckMessage = $"Invalid request; User with username: {username} is on active directory authentication.";
                    }
                }

                person person = null;
                if (!stopCheckFlag)
                {
                    person = PersonService.GetWithPersonId(Convert.ToString(user.person_id), CallerFormName, callerFormMethod, callerIpAddress);
                    if (person == null)
                    {
                        stopCheckFlag = true;
                        stopCheckMessage = "Unable to retrieve existing user records at the moment, kindly try again later";
                    }
                }

                if (!stopCheckFlag)
                {
                    if (string.IsNullOrEmpty(person.email_address))
                    {
                        stopCheckFlag = true;
                        stopCheckMessage = "Unable to complete request; Invalid email address profiled on user record.";
                    }
                }

                forgotpasswordlog forgotpasswordlog = null;
                if (!stopCheckFlag)
                {
                    forgotpasswordlog = new forgotpasswordlog
                    {
                        person_id = person.id,
                        username = username,
                        forgot_password_code = new Password(true, true, true, false, 150).Next(),
                        forgot_password_generated_date = DateTime.UtcNow.AddHours(1)
                    };
                    
                    var insertResult = ForgotPasswordLogService.Insert(forgotpasswordlog, CallerFormName, callerFormMethod, callerIpAddress);
                    if (insertResult <= 0)
                    {
                        stopCheckFlag = true;
                        stopCheckMessage = "Unable to initiate request at the moment, kindly try again later";
                    }
                }

                if (!stopCheckFlag)
                {
                    var resetLink = $"{ConfigurationUtility.GetAppSettingValue("ApplicationBaseUrl")}/account/reset-password?q={forgotpasswordlog.forgot_password_code}";

                    new Task(() =>
                    {
                        var templatePath = Server.MapPath(ConfigurationUtility.GetAppSettingValue("ForgotPasswordNotificationBodyTemplate"));
                        var mailBody = System.IO.File.ReadAllText(templatePath).Replace("{Name}", person.first_name).Replace("{ResetLink}", resetLink).Replace("{ValidityPeriod}", ConfigurationUtility.GetAppSettingValue("PasswordResetLinkValidityPeriod"));
                        var mailRecipient = person.email_address;
                        EmailLogManager.Log(EmailType.ForgotPasswordNotification, mailBody, mailRecipient, "", "", CallerFormName, callerFormMethod, callerIpAddress);

                    }).Start();

                    AlertUser("A forgot-password link has been sent to your profiled email address. Kindly check your mail to proceed.", AlertType.Success);
                    result["ResponseCode"] = "00";
                    result["ResponseMessage"] = "Successful";
                }
                else
                {
                    AlertUser(stopCheckMessage, AlertType.Error);
                    result["ResponseCode"] = "01";
                    result["ResponseMessage"] = "Failed";
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(CallerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return Json(JsonConvert.SerializeObject(result), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("reset-password")]
        public ActionResult ResetPassword(string q)
        {
            TempData["q"] = q;
            return View();
        }
        
        [HttpPost]
        [Route("reset-password")]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(string q, string newPassword, string confirmNewPassword)
        {
            const string callerFormMethod = "HttpPost|ResetPassword";

            try
            {
                if (!ValidationUtility.IsValidPasswordStrength(newPassword))
                {
                    stopCheckFlag = true;
                    stopCheckMessage = "Unable to complete request; Passwords must be at least 8 characters and contain at 3 of 4 of the following: upper case (A-Z), lower case (a-z), number (0-9) and special character (e.g. !@#$%^&*)";
                }

                if (!stopCheckFlag)
                {
                    if (!ValidationUtility.IsComparableStrings(newPassword, confirmNewPassword))
                    {
                        stopCheckFlag = true;
                        stopCheckMessage = "Unable to complete request; The passwords do not match.";
                    }
                }

                forgotpasswordlog forgotpasswordlog = null;
                if (!stopCheckFlag)
                {
                    forgotpasswordlog = ForgotPasswordLogService.GetWithForgotPasswordCode(q, CallerFormName, callerFormMethod, callerIpAddress);
                    if (forgotpasswordlog == null)
                    {
                        stopCheckFlag = true;
                        stopCheckMessage = "Unable to complete request; Invalid password reset link.";
                    }
                }
                
                if (!stopCheckFlag)
                {
                    if ((DateTime.UtcNow.AddHours(1) - Convert.ToDateTime(forgotpasswordlog.forgot_password_generated_date)).TotalMinutes > Convert.ToDouble(ConfigurationUtility.GetAppSettingValue("PasswordResetLinkValidityPeriod")))
                    {
                        stopCheckFlag = true;
                        stopCheckMessage = "Unable to complete request; Password reset link has expired, kindly re-initiate.";
                    }
                }

                user user = null;
                if (!stopCheckFlag)
                {
                    user = UsersService.GetWithUsername(forgotpasswordlog.username, CallerFormName, callerFormMethod, callerIpAddress);
                    if (user == null)
                    {
                        stopCheckFlag = true;
                        stopCheckMessage = "Unable to complete request; Invalid username.";
                    }
                }

                if (!stopCheckFlag)
                {
                    if (!user.authentication_type_id.Equals((int)UserAccountAuthenticationType.LocalAccountAuthentication))
                    {
                        stopCheckFlag = true;
                        stopCheckMessage = "Unable to complete request; your user account is not on local authentication";
                    }
                }

                if (!stopCheckFlag)
                {
                    var lastHistory = PasswordChangeLogService.GetLatestTwelveRecordsWithPersonId(Convert.ToString(user.person_id), CallerFormName, callerFormMethod, callerIpAddress);
                    if (lastHistory.Count > 0)
                    {
                        if (lastHistory.Any(c => c.password.Equals(EncryptionUtility.Encrypt(newPassword, encryptionKey))))
                        {
                            stopCheckFlag = true;
                            stopCheckMessage = "Unable to complete request; new password has been used before";
                        }
                    }
                }

                long userUpdateResult = 0;
                if (!stopCheckFlag)
                {
                    user.local_password = EncryptionUtility.Encrypt(newPassword, encryptionKey);
                    user.password_expiry_date = DateTime.UtcNow.AddDays(Convert.ToDouble(ConfigurationUtility.GetAppSettingValue("PasswordValidityPeriod")));

                    userUpdateResult = UsersService.Update(user, CallerFormName, callerFormMethod, callerIpAddress);
                    if (userUpdateResult <= 0)
                    {
                        stopCheckFlag = true;
                        stopCheckMessage = "Unable to complete request at the moment; password could not be updated. Kindly try again later!";
                    }
                }

                long historyLogResult = 0;
                if (!stopCheckFlag)
                {
                    var passwordChangeLog = new passwordchangelog
                    {
                        person_id = user.person_id,
                        username = user.username,
                        password = user.local_password,
                        logged_on = DateTime.UtcNow.AddHours(1)
                    };

                    historyLogResult = PasswordChangeLogService.Insert(passwordChangeLog, CallerFormName, callerFormMethod, callerIpAddress);
                    if (historyLogResult <= 0)
                    {
                        //
                    }
                }
                
                if (!stopCheckFlag)
                {
                    //Task 1
                    new Task(() =>
                    {
                        var appActivity = new appactivity
                        {
                            audit_username = user.username,
                            audit_fullname = $"{user.person?.surname}, {user.person?.first_name} {user.person?.middle_name}",
                            audit_ipaddress = callerIpAddress,
                            audit_macaddress = callerMacAddress,
                            operation = $"Password Reset".ToUpper(),
                            comments = $"'{user.username}' has reset password",
                            created_on = DateTime.Now,
                            created_by = "System"
                        };
                        AppActivityService.Insert(appActivity, CallerFormName, callerFormMethod, callerIpAddress);

                    }).Start();

                    AlertUser("You have successfully reset your password. Kindly logon with your new password.", AlertType.Success);
                }
                else
                {
                    AlertUser(stopCheckMessage, AlertType.Error);
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(CallerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return RedirectToAction("Login");
        }


        #endregion

    }
}