using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using App.Core.BusinessLogic;
using App.Core.DataTransferObjects;
using App.Core.Services;
using App.Core.Utilities;
using App.DataModels.Models;
using App.Web.Models;
using Excel;
using Newtonsoft.Json;

namespace App.Web.Controllers
{
    public class BaseController : Controller
    {
        protected NIBSSBvnValidatorManager nIBSSBvnValidatorManager;
        protected CRMSManager cRMSManager;

        protected void AlertUser(string alertMessage, AlertType alertType)
        {
            var alert = new AlertModel(alertMessage, (int)alertType);
            TempData["Alert"] = alert;
        }
        protected void AlertUser(string alertMessageSalutation, string alertMessage, AlertType alertType)
        {
            var alert = new AlertModel(alertMessageSalutation, alertMessage, (int)alertType);
            TempData["Alert"] = alert;
        }
        protected string ValidateUser(user user, string callerFormName, string callerFormMethod, string callerIpAddress, string callerMacAddress)
        {
            var result = "";

            try
            {
                var person = PersonService.GetWithPersonId(Convert.ToString(user.person_id), callerFormName, callerFormMethod, callerIpAddress);
                if (person == null)
                {
                    return result;
                }

                if (user.authentication_type_id.Equals((int)UserAccountAuthenticationType.ActiveDirectoryAuthentication))
                {
                    var userDetails = ActiveDirectoryManager.GetUserDetails(user.username, callerFormName, callerFormMethod, callerIpAddress);
                    if (!string.IsNullOrEmpty(userDetails))
                    {
                        var fullname = userDetails.Split('|').GetValue(0).ToString();
                        var surname = "";
                        var firstname = "";
                        var middlename = "";

                        string[] names = fullname.Split(' ');
                        if (names.Length >= 3)
                        {
                            surname = names[2];
                            firstname = names[0];
                            middlename = names[1];
                        }
                        else if (names.Length.Equals(2))
                        {
                            surname = names[1];
                            firstname = names[0];
                        }
                        else if (names.Length.Equals(1))
                        {
                            surname = names[0];
                            firstname = names[0];
                        }

                        var emailAddress = userDetails.Split('|').GetValue(1).ToString();

                        if (person != null)
                        {
                            var toUpdate = false;

                            if (string.IsNullOrEmpty(person.surname)) person.surname = "";
                            if (!person.surname.Equals(surname))
                            {
                                person.surname = surname;
                                toUpdate = true;
                            }

                            if (string.IsNullOrEmpty(person.first_name)) person.first_name = "";
                            if (!person.first_name.Equals(firstname))
                            {
                                person.first_name = firstname;
                                toUpdate = true;
                            }


                            if (string.IsNullOrEmpty(person.middle_name)) person.middle_name = "";
                            if (!person.middle_name.Equals(middlename))
                            {
                                person.middle_name = middlename;
                                toUpdate = true;
                            }

                            if (string.IsNullOrEmpty(person.email_address)) person.email_address = "";
                            if (!person.email_address.Equals(emailAddress))
                            {
                                person.email_address = emailAddress;
                                toUpdate = true;
                            }

                            if (toUpdate)
                            {
                                PersonService.Update(person, callerFormName, callerFormMethod, callerIpAddress);
                            }
                        }

                    }
                }

                switch (user.status_id)
                {
                    case (int)UserAccountStatus.LoggedIn:
                        var timeSpan = DateTime.UtcNow.AddHours(1) - Convert.ToDateTime(user.last_login_date);
                        if (timeSpan.TotalMinutes >= Convert.ToDouble(ConfigurationUtility.GetAppSettingValue("SessionTimeOutInMinutes")))
                        {
                            user.status_id = (int)UserAccountStatus.Active;
                            var userUpdateResult = UsersService.Update(user, callerFormName, callerFormMethod, callerIpAddress);
                            if (userUpdateResult > 0)
                            {
                                result = "false|You failed to logout properly. You have an active session however your active session has been forcefully terminated. kindly login again.";
                            }
                        }
                        else
                        {
                            result = $"false|You have an active logged in session. You would not be able to login until your active session expires in the next {ConfigurationUtility.GetAppSettingValue("SessionTimeOutInMinutes")} minutes";
                        }
                        break;
                    case (int)UserAccountStatus.LockedOut:
                        result = "false|user is locked";
                        break;
                    case (int)UserAccountStatus.Deleted:
                        result = "false|user has been deactivated";
                        break;
                    case (int)UserAccountStatus.Dormant:
                        result = "false|user account is dormant";
                        break;
                    case (int)UserAccountStatus.Inactive:
                        result = "false|User account is inactive";
                        break;
                    case (int)UserAccountStatus.Active:
                        var lastActivityDate = (user.last_logout_date ?? user.last_login_date) ?? user.created_on;
                        var timeDifference = DateTime.UtcNow.AddHours(1) - Convert.ToDateTime(lastActivityDate);
                        if (timeDifference.TotalDays >= Convert.ToDouble(ConfigurationUtility.GetAppSettingValue("UserInactivityPeriodInDays")))
                        {
                            user.status_id = (int)UserAccountStatus.Dormant;
                            var userUpdateResult = UsersService.Update(user, callerFormName, callerFormMethod, callerIpAddress);
                            if (userUpdateResult > 0)
                            {
                                result = "false|User account is dormant";
                            }
                        }
                        else
                        {
                            var failedLogonAttempts = UserAccessActivityService.GetAllFailedLogonAttempts(user.username, callerFormName, callerFormMethod, callerIpAddress);
                            if (failedLogonAttempts != null && failedLogonAttempts.Count >= Convert.ToInt32(ConfigurationUtility.GetAppSettingValue("MaximumAllowedFailedLogonAttempt")))
                            {
                                user.status_id = (int)UserAccountStatus.LockedOut;
                                var userUpdateResult = UsersService.Update(user, callerFormName, callerFormMethod, callerIpAddress);
                                if (userUpdateResult > 0)
                                {
                                    result = "false|User account is locked out";
                                }
                            }
                            else
                            {
                                var myRoleMenu = CacheData.Rolemenus.Where(c => c.role_id.Equals(user.role_id)).ToList();
                                if (myRoleMenu.Count > 0)
                                {
                                    result = "true|" + JsonConvert.SerializeObject(user);
                                    Session["RoleMenu"] = myRoleMenu;
                                }
                                else
                                {
                                    result = "false|No authorized resource";
                                }
                            }
                        }
                        break;
                    default:
                        result = "false|Invalid User Account Status";
                        break;
                }

            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return result;
        }
        protected HttpCookie LoginUser(string loginResult, string callerFormName, string callerFormMethod, string callerIpAddress, string callerMacAddress)
        {
            HttpCookie authenticationCookie = null;

            try
            {
                var authenticationTicket = new FormsAuthenticationTicket(1, FormsAuthentication.FormsCookieName, DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(1).AddMinutes(Convert.ToDouble(ConfigurationUtility.GetAppSettingValue("SessionTimeOutInMinutes"))), true, loginResult.Split('|').GetValue(1).ToString());
                var accessKey = FormsAuthentication.Encrypt(authenticationTicket);

                authenticationCookie = new HttpCookie(FormsAuthentication.FormsCookieName, accessKey)
                {
                    Expires = DateTime.UtcNow.AddHours(1).AddMinutes(Convert.ToDouble(ConfigurationUtility.GetAppSettingValue("SessionTimeOutInMinutes")))
                };

                AuditSuccessfulLoginRemarks(accessKey, JsonConvert.DeserializeObject<user>(loginResult.Split('|').GetValue(1).ToString()), "Successful", callerFormName, callerFormMethod, callerIpAddress, callerMacAddress);
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return authenticationCookie;
        }
        public user GetUser(string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            user userData = null;

            try
            {
                if (!(System.Web.HttpContext.Current.User.Identity is FormsIdentity formsIdentity)) return userData;
                if (formsIdentity.Ticket == null) return userData;

                var formsAuthenticationTicket = formsIdentity.Ticket;
                if (formsAuthenticationTicket == null) return userData;
                if (formsAuthenticationTicket.UserData == null) return userData;

                userData = JsonConvert.DeserializeObject<user>(formsAuthenticationTicket.UserData);
                if (userData != null)
                {
                    userData.person = PersonService.GetWithPersonId(Convert.ToString(userData.person_id), callerFormName, callerFormMethod, callerIpAddress);
                    userData.role = RoleService.GetWithRoleId(Convert.ToString(userData.role_id), callerFormName, callerFormMethod, callerIpAddress);
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return userData;
        }
        protected void AuditLoginAttemptRemarks(string username, string remarks, string callerFormName, string callerFormMethod, string callerIpAddress, string callerMacAddress)
        {
            try
            {
                var fullname = "";
                var user = UsersService.GetWithUsername(username, callerFormName, callerFormMethod, callerIpAddress);
                if (user != null)
                {
                    var person = PersonService.GetWithPersonId(Convert.ToString(user.person_id), callerFormName, callerFormMethod, callerIpAddress);
                    if (person != null)
                    {
                        fullname = $"{person.surname}, {person.first_name} {person.middle_name}";
                    }
                }
                
                var useraccessactivity = new useraccessactivity
                {
                    username = username,
                    fullname = fullname,
                    ipaddress = callerIpAddress,
                    macaddress = callerMacAddress,
                    remarks = remarks,
                    created_on = DateTime.UtcNow.AddHours(1)
                };

                UserAccessActivityService.Insert(useraccessactivity, callerFormName, callerFormMethod, callerIpAddress);
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }
        }
        private void AuditSuccessfulLoginRemarks(string sessionKey, user user, string remarks, string callerFormName, string callerFormMethod, string callerIpAddress, string callerMacAddress)
        {
            try
            {
                var fullname = "";
                var person = PersonService.GetWithPersonId(Convert.ToString(user.person_id), callerFormName, callerFormMethod, callerIpAddress);
                if (person != null)
                {
                    fullname = $"{person.surname}, {person.first_name} {person.middle_name}";
                }

                user.status_id = (int)UserAccountStatus.LoggedIn;
                user.last_login_date = DateTime.UtcNow.AddHours(1);

                var userUpdateResult = UsersService.Update(user, callerFormName, callerFormMethod, callerIpAddress);
                if (userUpdateResult > 0)
                {
                    var useraccessactivity  = new useraccessactivity
                    {
                        username = user.username,
                        fullname = fullname,
                        ipaddress = callerIpAddress,
                        macaddress = callerMacAddress,
                        access_key = sessionKey,
                        key_expiration_date = DateTime.UtcNow.AddHours(1).AddMinutes(Convert.ToDouble(ConfigurationUtility.GetAppSettingValue("SessionTimeOutInMinutes"))),
                        remarks = remarks,
                        created_on = DateTime.UtcNow.AddHours(1)
                    };

                    UserAccessActivityService.Insert(useraccessactivity, callerFormName, callerFormMethod, callerIpAddress);
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }
        }
        protected void AuditLogoutRemarks(string sessionKey, string callerFormName, string callerFormMethod, string callerIpAddress, string callerMacAddress)
        {
            try
            {
                var usageLog = UserAccessActivityService.GetWithKey(sessionKey, callerFormName, callerFormMethod, callerIpAddress);
                if (usageLog != null)
                {
                    usageLog.logout_date = DateTime.UtcNow.AddHours(1);
                    UserAccessActivityService.Update(usageLog, callerFormName, callerFormMethod, callerIpAddress);
                }

                var user = UsersService.GetWithUsername(usageLog?.username, callerFormName, callerFormMethod, callerIpAddress);
                if (user != null)
                {
                    user.status_id = (int)UserAccountStatus.Active;
                    user.last_logout_date = DateTime.UtcNow.AddHours(1);
                    UsersService.Update(user, callerFormName, callerFormMethod, callerIpAddress);
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }
        }
        protected bool AuthorizeRequest(string requestUrlAbsolutePath, ref string checkerUserEmailsString, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            var result = false;

            try
            {
                if (string.IsNullOrEmpty(requestUrlAbsolutePath))
                {
                    return result;
                }

                requestUrlAbsolutePath = requestUrlAbsolutePath.Substring(ConfigurationUtility.GetAppSettingValue("ApplicationDeployedFolderPath").Length);
                var subMenus = (from subMenu in CacheData.Submenus
                                join roleMenu in (List<rolemenu>)Session["RoleMenu"] on subMenu.id equals roleMenu.menu_sub_id
                                where subMenu.active_flag.Equals(true)
                                select new
                                {
                                    subMenu.id,
                                    subMenu.display_name,
                                    subMenu.access_name,
                                    subMenu.main_menu_id,
                                    subMenu.url,
                                    subMenu.active_flag,
                                    subMenu.arrangement_order,
                                    subMenu.display_flag,
                                    subMenu.maker_page_flag,
                                    subMenu.checker_page_id

                                }).OrderBy(c => c.display_name).ToList();

                var assignedSubMenu = subMenus.FirstOrDefault(c => c.url.ToLower().Equals(requestUrlAbsolutePath));
                if (assignedSubMenu != null)
                {
                    result = true;
                }

                if (assignedSubMenu.maker_page_flag)
                {
                    var checkerPageId = assignedSubMenu.checker_page_id ?? 0;
                    var checkerUserEmails = (from subMenu in CacheData.Submenus
                                             join roleMenu in (List<rolemenu>)Session["RoleMenu"] on subMenu.id equals roleMenu.menu_sub_id
                                             join user in UsersService.GetAllActiveUsers(callerFormName, callerFormMethod, callerIpAddress) on roleMenu.role_id equals user.role_id
                                             join person in PersonService.GetAll(callerFormName, callerFormMethod, callerIpAddress) on user.person_id equals person.id
                                             where subMenu.id.Equals(checkerPageId)
                                             select new
                                             {
                                                 person.email_address

                                             }).ToList();

                    checkerUserEmailsString = string.Join(",", checkerUserEmails.Select(c => c.email_address));
                }

            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return result;
        }
        public JsonResult GetStaffDetails(string staffId)
        {
            var result = new Dictionary<string, object>
            {
                {"ResponseCode", null },
                {"ResponseMessage", null },
                {"ResponseDescription", null },
                {"StaffSurname", null },
                {"StaffFirstname", null },
                {"StaffMiddlename", null },
                {"StaffEmailAddress", null },
                {"StaffSolId", null },
                {"StaffSolName", null },
                {"StaffSolAddress", null }
            };

            try
            {
                bool stopCheckFlag = false;
                var stopCheckMessage = "";

                var rmDetails = ActiveDirectoryManager.GetUserDetails(staffId, "BaseController", "GetStaffDetails", "");
                if (string.IsNullOrEmpty(rmDetails))
                {
                    stopCheckFlag = true;
                    stopCheckMessage = "Unable to fetch staff details from Active Directory server at the moment";
                }

                if (!stopCheckFlag)
                {
                    if (!rmDetails.Contains("|"))
                    {
                        stopCheckFlag = true;
                        stopCheckMessage = "Incorrect format of staff details fetched from Active Directory server";
                    }
                }

                var surname = "";
                var firstname = "";
                var middlename = "";
                var emailAddress = "";
                var solID = "";
                var solName = "";
                var solAddress = "";

                if (!stopCheckFlag)
                {
                    var fullname = rmDetails.Split('|').GetValue(0).ToString();
                    string[] names = fullname.Split(' ');
                    if (names.Length >= 3)
                    {
                        surname = names[2];
                        firstname = names[0];
                        middlename = names[1];
                    }
                    else if (names.Length.Equals(2))
                    {
                        surname = names[1];
                        firstname = names[0];
                        middlename = null;
                    }
                    else if (names.Length.Equals(1))
                    {
                        surname = names[0];
                        firstname = names[0];
                        middlename = null;
                    }

                    emailAddress = rmDetails.Split('|').GetValue(1).ToString();
                    var sol = FinacleServices.GetFinacleSol1(staffId, "BaseController", "GetStaffDetails", "");
                    solID = !string.IsNullOrEmpty(sol?.SolId) ? sol?.SolId : "n/p";
                    solName = !string.IsNullOrEmpty(sol?.SolName) ? sol?.SolName : "n/p";
                    solAddress = !string.IsNullOrEmpty(sol?.SolAddress) ? sol?.SolAddress : "n/p";
                }

                if (!stopCheckFlag)
                {
                    result["ResponseCode"] = "00";
                    result["ResponseMessage"] = "Successful";
                    result["StaffSurname"] = surname;
                    result["StaffFirstname"] = firstname;
                    result["StaffMiddlename"] = middlename;
                    result["StaffEmailAddress"] = emailAddress;
                    result["StaffSolId"] = solID;
                    result["StaffSolName"] = solName;
                    result["StaffSolAddress"] = solAddress;
                }
                else
                {
                    result["ResponseCode"] = "01";
                    result["ResponseMessage"] = "Failed";
                    result["ResponseDescription"] = stopCheckMessage;
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError("BaseController", "GetStaffDetails", "", ex);
            }

            return Json(JsonConvert.SerializeObject(result), JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> GetAccountName(string accountNumber)
        {
            //var result = new Dictionary<string, object>
            //{
            //    {"ResponseCode", null },
            //    {"ResponseMessage", null },
            //    {"ResponseDescription", null },
            //    {"AccountName", null }

            //};

            //try
            //{
            //    bool stopCheckFlag = false;
            //    var stopCheckMessage = "";

            //    var accountName = FinacleServices.GetAccountName(accountNumber, "BaseController", "GetAccountName", "");
            //    if (string.IsNullOrEmpty(accountName))
            //    {
            //        stopCheckFlag = true;
            //        stopCheckMessage = "Unable to fetch account name at the moment";
            //    }

            //    if (!stopCheckFlag)
            //    {
            //        result["ResponseCode"] = "00";
            //        result["ResponseMessage"] = "Successful";
            //        result["AccountName"] = accountName;
            //    }
            //    else
            //    {
            //        result["ResponseCode"] = "01";
            //        result["ResponseMessage"] = "Failed";
            //        result["ResponseDescription"] = stopCheckMessage;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    LogUtility.LogError("BaseController", "GetAccountName", "", ex);
            //}

            //return Json(JsonConvert.SerializeObject(result), JsonRequestBehavior.AllowGet);


            var result = new Dictionary<string, object>
            {
                {"ResponseCode", null },
                {"ResponseMessage", null },
                {"ResponseDescription", null },
                {"AccountName", null },
                {"AccountNumber", null }
            };

            try
            {
                var getAccountNameRequest = new GetAccountNameRequest
                {
                    RequestId = Guid.NewGuid().ToString("N").ToUpper(),
                    CountryId = $"{ConfigurationUtility.GetAppSettingValue("CountryId")}",
                    AccountNumber = accountNumber
                };

                var getAccountNameResponse = await FIBridgeManager.GetAccountNameAsync(getAccountNameRequest, "BaseController", "GetAccountName", "");

                result["ResponseCode"] = getAccountNameResponse?.ResponseCode;
                result["ResponseMessage"] = getAccountNameResponse?.ResponseMessage;
                result["AccountName"] = getAccountNameResponse?.AccountName;
                result["AccountNumber"] = accountNumber;
                result["ResponseDescription"] = getAccountNameResponse?.ResponseCode != "00" ? "Unable to fetch account name at the moment" : "";
            }
            catch (Exception ex)
            {
                LogUtility.LogError("BaseController", "GetAccountName", "", ex);
            }

            return Json(JsonConvert.SerializeObject(result), JsonRequestBehavior.AllowGet);
        }
        protected string SaveFile(HttpPostedFileBase fileUpload, string fileUploadPurpose, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            var savedFileUrl = "";

            try
            {
                //get details of file to upload
                var fileInfo = new FileInfo(fileUpload.FileName);
                var fileName = fileInfo.Name;
                var fileExtension = fileInfo.Extension;

                //what is the name of the folder where i want to save the file?
                const string saveFolder = "FileUploads";

                //update file name and file path
                var saveFileName = $"{fileUploadPurpose.Replace("/", "-").Replace(@"\", "-")}-{DateTime.Now.ToString(CultureInfo.InvariantCulture).Replace("/", "").Replace(" ", "").Replace(":", "")}{fileExtension}";

                //if folder does not already exist then create folder
                var saveDirectory = Server.MapPath($"~/{saveFolder}");
                if (!Directory.Exists(saveDirectory))
                {
                    Directory.CreateDirectory(saveDirectory);
                }
                
                //get the file save path
                var savePath = Path.Combine(saveDirectory, saveFileName);

                //save file and return
                fileUpload.SaveAs(savePath);

                //return with the save url
                savedFileUrl = $"~/{ saveFolder}/{saveFileName}";
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return savedFileUrl;
        }
        protected DataSet ReadExcelFile(string filePath, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            DataSet result = null;

            try
            {
                var fileStream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read);
                var myFile = filePath.Split('.');
                var myData = myFile[myFile.GetUpperBound(0)];

                IExcelDataReader excelReader;
                if (myData.ToUpper() == "XLS")
                {
                    excelReader = ExcelReaderFactory.CreateBinaryReader(fileStream);
                    result = excelReader.AsDataSet();
                    excelReader.IsFirstRowAsColumnNames = true;
                    excelReader.Close();
                }
                else if (myData.ToUpper() == "XLSX")
                {
                    //2. Reading from a OpenXml Excel file (2007 format; *.xlsx)
                    excelReader = ExcelReaderFactory.CreateOpenXmlReader(fileStream);
                    result = excelReader.AsDataSet();
                    excelReader.IsFirstRowAsColumnNames = true;
                    excelReader.Close();
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return result;
        }
    }
}