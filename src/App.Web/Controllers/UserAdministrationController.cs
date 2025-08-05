using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;
using App.Core.BusinessLogic;
using App.Core.Utilities;
using App.Web.Models;
using App.Web.ViewModels;
using App.DataModels.Models;
using App.Core.Services;
using System.Web;
using PasswordGenerator;

namespace App.Web.Controllers
{
    [Authorize]
    [RoutePrefix("user-administration")]
    public class UserAdministrationController : BaseController
    {
        private const string CallerFormName = "UserAdministrationController";
        private readonly string callerIpAddress;
        private readonly string callerMacAddress;

        private readonly ViewUsersViewModel viewUsersViewModel;
        private CreateUsersViewModel createUsersViewModel;
        private readonly EditUsersViewModel editUsersViewModel;
        private readonly ReviewMakerCheckerLogViewModel reviewMakerCheckerLogViewModel;
        private List<MakerCheckerLogModel> MakerCheckerLogModels;
        private readonly ViewUserProfileViewModel viewUserProfileViewModel;
        private readonly user userData;

        private bool stopCheckFlag = false;
        private string stopCheckMessage = "";

        //initialize password policy
        private readonly bool includeLowerCase;
        private readonly bool includeUpperCase;
        private readonly bool includeNumeric;
        private readonly bool includeSpecial;
        private readonly int passwordLength;

        private readonly string encryptionKey;

        public UserAdministrationController()
        {
            callerIpAddress = IpAddressManager.GetClientComputerIpAddress();
            callerMacAddress = MacAddressManager.GetClientComputerMacAddress();
            viewUsersViewModel = new ViewUsersViewModel();
            editUsersViewModel = new EditUsersViewModel();
            reviewMakerCheckerLogViewModel = new ReviewMakerCheckerLogViewModel();
            MakerCheckerLogModels = new List<MakerCheckerLogModel>();
            viewUserProfileViewModel = new ViewUserProfileViewModel();
            userData = GetUser(CallerFormName, "Ctor|UserAdministrationController", callerIpAddress);

            includeLowerCase = ConfigurationUtility.GetAppSettingValue("IncludeLowerCase").Equals("N") ? false : true;
            includeUpperCase = ConfigurationUtility.GetAppSettingValue("IncludeUpperCase").Equals("N") ? false : true;
            includeNumeric = ConfigurationUtility.GetAppSettingValue("IncludeNumeric").Equals("N") ? false : true;
            includeSpecial = ConfigurationUtility.GetAppSettingValue("IncludeSpecial").Equals("N") ? false : true;
            passwordLength = string.IsNullOrEmpty(ConfigurationUtility.GetAppSettingValue("PasswordLength")) ? 8 : Convert.ToInt32(ConfigurationUtility.GetAppSettingValue("PasswordLength")) < 8 ? 8 : Convert.ToInt32(ConfigurationUtility.GetAppSettingValue("PasswordLength"));

            encryptionKey = ConfigurationUtility.GetAppSettingValue("EncryptionKey");
        }

        #region ActionResult

        [HttpGet]
        [Route("view-users")]
        public ActionResult ViewUsers()
        {
            const string callerFormMethod = "HttpGet|ViewUsers";

            try
            {
                var checkerEmails = "";
                if (!AuthorizeRequest(Request.Url.AbsolutePath, ref checkerEmails, CallerFormName, callerFormMethod, callerIpAddress))
                {
                    AlertUser("Unauthorised Access! You may not have been permitted to access the resource. Kindly login to re-authenticate. If issue persist, contact administrator.", AlertType.Error);
                    return RedirectToAction("Logout", "Account");
                }

                var users = UsersService.GetAllUsersExceptLoggedInUser(userData.username, CallerFormName, callerFormMethod, callerIpAddress);
                if (users.Count > 0)
                {
                    viewUsersViewModel.UserModels = (from u in users
                                                     join p in PersonService.GetAll(CallerFormName, callerFormMethod, callerIpAddress) on u.person_id equals p.id
                                                     select new UserModel
                                                     {
                                                         Username = u.username,
                                                         AuthenticationTypeId = Convert.ToString(u.authentication_type_id),
                                                         AuthenticationTypeName = CacheData.Useraccountauthenticationtypes.FirstOrDefault(c => c.id.Equals(u.authentication_type_id))?.authentication_type_name,
                                                         IsBranchUser = u.branch_user_flag,

                                                         LocalPassword = u.local_password,
                                                         Surname = p.surname,
                                                         Firstname = p.first_name,
                                                         Middlename = p.middle_name,
                                                         MobileNumber = p.mobile_number,
                                                         EmailAddress = p.email_address,
                                                         Passport = p.passport,
                                                         PersonTypeName = CacheData.Persontypes.FirstOrDefault(c => c.id.Equals(p.person_type_id))?.person_type_name,

                                                         RoleId = Convert.ToString(u.role_id),
                                                         RoleName = RoleService.GetWithRoleId(Convert.ToString(u.role_id), CallerFormName, callerFormMethod, callerIpAddress)?.role_name,
                                                         AccountStatusId = Convert.ToString(u.status_id),
                                                         AccountStatusName = CacheData.Useraccountstatus.FirstOrDefault(status => status.id.Equals(u.status_id))?.account_status,
                                                         LastLoginDate = !string.IsNullOrEmpty(Convert.ToString(u.last_login_date)) ? Convert.ToDateTime(u.last_login_date).ToString("dd-MM-yyyy hh:mm tt") : "n/p",
                                                         LastLogoutDate = !string.IsNullOrEmpty(Convert.ToString(u.last_logout_date)) ? Convert.ToDateTime(u.last_logout_date).ToString("dd-MM-yyyy hh:mm tt") : "n/p",
                                                         CreatedOn = Convert.ToDateTime(u.created_on).ToString("dd-MM-yyyy hh:mm tt"),
                                                         CreatedBy = u.created_by,
                                                         ApprovedOn = !string.IsNullOrEmpty(Convert.ToString(u.approved_on)) ? Convert.ToDateTime(u.approved_on).ToString("dd-MM-yyyy hh:mm tt") : "n/p",
                                                         ApprovedBy = u.approved_by,
                                                         LastModifiedOn = !string.IsNullOrEmpty(Convert.ToString(u.last_modified_on)) ? Convert.ToDateTime(u.last_modified_on).ToString("dd-MM-yyyy hh:mm tt") : "n/p",
                                                         LastModifiedBy = u.last_modified_by,
                                                         UrlQueryString = Convert.ToString(u.query_string)

                                                     }).ToList();
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(CallerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return View(viewUsersViewModel);

        }

        [HttpGet]
        [Route("create-new-user")]
        public ActionResult CreateNewUser()
        {
            const string callerFormMethod = "HttpGet|CreateNewUser";

            try
            {
                createUsersViewModel = new CreateUsersViewModel(CallerFormName, "HttpGet|CreateNewUser", callerIpAddress);
            }
            catch (Exception ex)
            {
                LogUtility.LogError(CallerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return View(createUsersViewModel);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("create-new-user")]
        public ActionResult CreateNewUser(UserModel model)
        {
            const string callerFormMethod = "HttpPost|CreateNewUser";

            try
            {
                var checkerEmails = "";
                if (!AuthorizeRequest(Request.Url.AbsolutePath, ref checkerEmails, CallerFormName, callerFormMethod, callerIpAddress))
                {
                    AlertUser("Unauthorised Access! You may not have been permitted to access the resource. Kindly login to re-authenticate. If issue persist, contact administrator.", AlertType.Error);
                    return RedirectToAction("Logout", "Account");
                }
                
                var userDetails = ActiveDirectoryManager.GetUserDetails(model.Username, CallerFormName, callerFormMethod, callerIpAddress);
                if (string.IsNullOrEmpty(userDetails))
                {
                    stopCheckFlag = true;
                    stopCheckMessage = "Unable to fetch users details from active directory server. Please try again later!";
                }

                if (!stopCheckFlag)
                {
                    if (!userDetails.Contains("|"))
                    {
                        stopCheckFlag = true;
                        stopCheckMessage = "Incorrect format of user details fetched from active directory server. Please try again later!";
                    }
                }

                if (!stopCheckFlag)
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
                        middlename = null;
                    }
                    else if (names.Length.Equals(1))
                    {
                        surname = names[0];
                        firstname = names[0];
                        middlename = null;
                    }

                    var emailAddress = userDetails.Split('|').GetValue(1).ToString();

                    model.Surname = surname;
                    model.Firstname = firstname;
                    model.Middlename = middlename;
                    model.EmailAddress = emailAddress;
                }

                if (!stopCheckFlag)
                {
                    if (!ValidationUtility.IsValidTextInput(model.Surname))
                    {
                        ModelState.AddModelError("Surname", "Surname field is required");
                    }

                    if (!ValidationUtility.IsValidTextInput(model.Firstname))
                    {
                        ModelState.AddModelError("Firstname", "First name field is required");
                    }

                    if (!ValidationUtility.IsValidLongInput(model.RoleId))
                    {
                        ModelState.AddModelError("RoleId", "Role field is required");
                    }
                }

                if (!ModelState.IsValid)
                {
                    var errorMessages = string.Join(", ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
                    stopCheckFlag = true;
                    stopCheckMessage = errorMessages;
                }

                if (!stopCheckFlag)
                {
                    var role = RoleService.GetWithRoleId(model.RoleId, CallerFormName, callerFormMethod, callerIpAddress);
                    if (role != null && role.system_reserved_flag)
                    {
                        stopCheckFlag = true;
                        stopCheckMessage = "Unable to initiate request; selected role is reserved";
                    }
                }

                var makerCheckerAction = "User Creation";
                var makerCheckerActionDetails = $"Create New User '{model.Fullname}({model.Username})' as '{RoleService.GetWithRoleId(model.RoleId, CallerFormName, callerFormMethod, callerIpAddress)?.role_name}'";
                
                user user = null;
                if (!stopCheckFlag)
                {
                    user = UsersService.GetWithUsername(model.Username, CallerFormName, callerFormMethod, callerIpAddress);
                    if (user != null)
                    {
                        stopCheckFlag = true;
                        stopCheckMessage = "Unable to initiate request; username already exists!";
                    }
                }

                if (!stopCheckFlag)
                {
                    user = new user
                    {
                        username = model.Username,
                        authentication_type_id = (int)UserAccountAuthenticationType.ActiveDirectoryAuthentication,
                        branch_user_flag = model.IsBranchUser,
                        person = new person
                        {
                            surname = model.Surname,
                            first_name = model.Firstname,
                            middle_name = model.Middlename,
                            mobile_number = model.MobileNumber,
                            email_address = model.EmailAddress,
                            created_on = DateTime.UtcNow.AddHours(1),
                            created_by = userData.username,
                            query_string = Guid.NewGuid()
                        },
                        role_id = Convert.ToInt32(model.RoleId),
                        status_id = (int)UserAccountStatus.Active,
                        created_on = DateTime.UtcNow.AddHours(1),
                        created_by = userData.username,
                        query_string = Guid.NewGuid()
                    };

                    var makerCheckerActionData = JsonConvert.SerializeObject(user);
                    var sol = userData.branch_user_flag ? FinacleServices.GetFinacleSol2(userData.username, CallerFormName, callerFormMethod, callerIpAddress) : null;
                    var makerCheckerLog = new makercheckerlog
                    {
                        maker_checker_category_id = (int)MakerCheckerCategory.User,
                        maker_checker_type_id = (int)MakerCheckerType.CreateNewUser,
                        action_name = makerCheckerAction,
                        action_details = makerCheckerActionDetails,
                        action_data = makerCheckerActionData,
                        maker_person_id = userData.person_id,
                        maker_username = userData.username,
                        maker_fullname = $"{userData.person?.surname}, {userData.person?.first_name} {userData.person?.middle_name}",

                        maker_sol_id = sol?.SolId,
                        maker_sol_name = sol?.SolName,
                        maker_sol_address = sol?.SolAddress,

                        maker_checker_status = (int)MakerCheckerStatus.Initiated,
                        date_made = DateTime.UtcNow.AddHours(1),
                        query_string = Guid.NewGuid()
                    };

                    var makerCheckerLogInsertResult = MakerCheckerLogService.Insert(makerCheckerLog, CallerFormName, callerFormMethod, callerIpAddress);
                    if (makerCheckerLogInsertResult <= 0)
                    {
                        stopCheckFlag = true;
                        stopCheckMessage = "Unable to submit request for approval at the moment. Please try again later!";
                    }

                }

                if (!stopCheckFlag)
                {
                    //Task 1
                    new Task(() =>
                    {
                        var appActivity = new appactivity
                        {
                            audit_username = userData.username,
                            audit_fullname = $"{userData.person?.surname}, {userData.person?.first_name} {userData.person?.middle_name}",
                            audit_ipaddress = callerIpAddress,
                            audit_macaddress = callerMacAddress,
                            operation = $"Initiated - {makerCheckerAction}".ToUpper(),
                            comments = $"Initiated - {makerCheckerActionDetails}",
                            created_on = DateTime.Now,
                            created_by = "System"
                        };

                        AppActivityService.Insert(appActivity, CallerFormName, callerFormMethod, callerIpAddress);

                    }).Start();
                    
                    //Task 2
                    new Task(() =>
                    {
                        if (!string.IsNullOrEmpty(checkerEmails.Trim().Replace(",", "")))
                        {
                            var templatePath = Server.MapPath(ConfigurationUtility.GetAppSettingValue("PendingItemsNotificationBodyTemplate"));
                            var mailBody = System.IO.File.ReadAllText(templatePath).Replace("{ItemType}", "User Creation");
                            var mailRecipient = checkerEmails;
                            EmailLogManager.Log(EmailType.PendingItemsNotification, mailBody, mailRecipient, "", "", CallerFormName, callerFormMethod, callerIpAddress);
                        }

                    }).Start();

                    AlertUser("Successfully submitted request for approval. Your changes will be effected once approved.", AlertType.Success);
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

            return RedirectToAction("CreateNewUser");
        }

        [HttpGet]
        [Route("edit-existing-user")]
        public ActionResult EditExistingUser(string q)
        {
            const string callerFormMethod = "HttpGet|EditExistingUser";

            var redirectAction = "EditExistingBankUser";

            try
            {
                var user = UsersService.GetWithUrlQueryString(q, CallerFormName, callerFormMethod, callerIpAddress);
                if (user == null)
                {
                    stopCheckFlag = true;
                    stopCheckMessage = "No User Found with the Search Parameter";
                }

                person person = null;
                if (!stopCheckFlag)
                {
                    person = PersonService.GetWithPersonId(Convert.ToString(user.person_id), CallerFormName, callerFormMethod, callerIpAddress);
                    if (person == null)
                    {
                        stopCheckFlag = true;
                        stopCheckMessage = "No Person Record Found with the Search Parameter";
                    }
                }

                if (!stopCheckFlag)
                {
                    if (person.person_type_id.Equals((int)PersonType.FintechContactPerson) || person.person_type_id.Equals((int)PersonType.BankRelationshipManager))
                    {
                        redirectAction = "EditExistingFintechContactPersonAndRelationshipManager";
                    }
                }
                else
                {
                    AlertUser(stopCheckMessage, AlertType.Error);
                    return RedirectToAction("ViewUsers");
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(CallerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return RedirectToAction(redirectAction, new { q });
        }

        [HttpGet]
        [Route("edit-existing-bank-user")]
        public ActionResult EditExistingBankUser(string q)
        {
            const string callerFormMethod = "HttpGet|EditExistingBankUser";

            try
            {
                var user = UsersService.GetWithUrlQueryString(q, CallerFormName, callerFormMethod, callerIpAddress);
                if (user == null)
                {
                    stopCheckFlag = true;
                    stopCheckMessage = "No User Found with the Search Parameter";
                }

                person person = null;
                if (!stopCheckFlag)
                {
                    person = PersonService.GetWithPersonId(Convert.ToString(user.person_id), CallerFormName, callerFormMethod, callerIpAddress);
                    if (person == null)
                    {
                        stopCheckFlag = true;
                        stopCheckMessage = "No person record found with the search parameter";
                    }
                }

                if (!stopCheckFlag)
                {
                    editUsersViewModel.UserModel = new UserModel(CallerFormName, callerFormMethod, callerIpAddress)
                    {
                        IsBranchUser = user.branch_user_flag,
                        Surname = person.surname,
                        Firstname = person.first_name,
                        Middlename = person.middle_name,
                        RoleId = Convert.ToString(user.role_id),
                        AccountStatusId = Convert.ToString(user.status_id),
                        UrlQueryString = Convert.ToString(user.query_string)
                    };
                }
                else
                {
                    AlertUser(stopCheckMessage, AlertType.Error);
                    return RedirectToAction("ViewUsers");
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(CallerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return View(editUsersViewModel);
        }
        
        [HttpGet]
        [Route("edit-existing-fintech-contact-person-and-relationship-manager")]
        public ActionResult EditExistingFintechContactPersonAndRelationshipManager(string q)
        {
            const string callerFormMethod = "HttpGet|EditExistingFintechContactPersonAndRelationshipManager";

            try
            {
                var user = UsersService.GetWithUrlQueryString(q, CallerFormName, callerFormMethod, callerIpAddress);
                if (user == null)
                {
                    stopCheckFlag = true;
                    stopCheckMessage = "No User Found with the Search Parameter";
                }

                person person = null;
                if (!stopCheckFlag)
                {
                    person = PersonService.GetWithPersonId(Convert.ToString(user.person_id), CallerFormName, callerFormMethod, callerIpAddress);
                    if (person == null)
                    {
                        stopCheckFlag = true;
                        stopCheckMessage = "No person record found with the search parameter";
                    }
                }

                if (!stopCheckFlag)
                {
                    editUsersViewModel.UserModel = new UserModel(CallerFormName, callerFormMethod, callerIpAddress)
                    {
                        IsBranchUser = user.branch_user_flag,
                        Surname = person.surname,
                        Firstname = person.first_name,
                        Middlename = person.middle_name,
                        AccountStatusId = Convert.ToString(user.status_id),
                        UrlQueryString = Convert.ToString(user.query_string)
                    };
                }
                else
                {
                    AlertUser(stopCheckMessage, AlertType.Error);
                    return RedirectToAction("ViewUsers");
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(CallerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return View(editUsersViewModel);
        }
        
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("edit-existing-user")]
        public ActionResult EditExistingUser(EditUsersViewModel model)
        {
            const string callerFormMethod = "HttpPost|EditExistingUser";

            try
            {
                var checkerEmails = "";
                if (!AuthorizeRequest(Request.Url.AbsolutePath, ref checkerEmails, CallerFormName, callerFormMethod, callerIpAddress))
                {
                    AlertUser("Unauthorised Access! You may not have been permitted to access the resource. Kindly login to re-authenticate. If issue persist, contact administrator.", AlertType.Error);
                    return RedirectToAction("Logout", "Account");
                }

                user user = null;
                if (!stopCheckFlag)
                {
                    user = UsersService.GetWithUrlQueryString(model.UserModel.UrlQueryString, CallerFormName, callerFormMethod, callerIpAddress);
                    if (user == null)
                    {
                        stopCheckFlag = true;
                        stopCheckMessage = "Unable to retrieve existing user at the moment, kindly try again later";
                    }
                }

                person person = null;
                if (!stopCheckFlag)
                {
                    person = PersonService.GetWithPersonId(Convert.ToString(user.person_id), CallerFormName, callerFormMethod, callerIpAddress);
                    if (person == null)
                    {
                        stopCheckFlag = true;
                        stopCheckMessage = "Unable to retrieve existing person record found with the search parameter";
                    }
                }

                if (!stopCheckFlag)
                {
                    if (person.person_type_id.Equals((int)PersonType.BankUser))
                    {
                        if (!ValidationUtility.IsValidLongInput(model.UserModel.RoleId))
                        {
                            ModelState.AddModelError("RoleId", "Role field does not meet the required data format");
                        }
                    }

                    if (!ValidationUtility.IsValidLongInput(model.UserModel.AccountStatusId))
                    {
                        ModelState.AddModelError("AccountStatusId", "Account status field does not meet the required data format");
                    }

                    if (!ModelState.IsValid)
                    {
                        var errorMessages = string.Join("<br/>", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
                        stopCheckFlag = true;
                        stopCheckMessage = errorMessages;
                    }
                }

                var makerCheckerAction = "User Editing";
                var makerCheckerActionDetails = "";
                
                if (!stopCheckFlag)
                {
                    if (person.person_type_id.Equals((int)PersonType.BankUser))
                    {
                        if (!user.branch_user_flag.Equals(model.UserModel.IsBranchUser))
                        {
                            makerCheckerActionDetails += $"Edit User:{user.username} Branch User Flag From '{user.branch_user_flag}' to '{model.UserModel.IsBranchUser}'|";
                        }

                        if (!user.role_id.Equals(Convert.ToInt64(model.UserModel.RoleId)))
                        {
                            makerCheckerActionDetails += $"Edit User:{user.username} Role From '{RoleService.GetWithRoleId(Convert.ToString(user.role_id), CallerFormName, callerFormMethod, callerIpAddress)?.role_name}' to '{RoleService.GetWithRoleId(model.UserModel.RoleId, CallerFormName, callerFormMethod, callerIpAddress)?.role_name}'|";
                        }
                    }
                    if (!user.status_id.Equals(Convert.ToInt32(model.UserModel.AccountStatusId)))
                    {
                        makerCheckerActionDetails += $"Edit User:{user.username} Status From '{CacheData.Useraccountstatus.FirstOrDefault(c => c.id.Equals(user.status_id))?.account_status}' to '{CacheData.Useraccountstatus.FirstOrDefault(c => c.id.ToString().Equals(model.UserModel.AccountStatusId))?.account_status}'|";
                    }

                    makerCheckerActionDetails = makerCheckerActionDetails.Substring(0, makerCheckerActionDetails.Length - 1);

                    //
                    if (string.IsNullOrEmpty(makerCheckerActionDetails))
                    {
                        stopCheckFlag = true;
                        stopCheckMessage = "Unable to initiate changes because no modification observed";
                    }
                }
                
                if (!stopCheckFlag)
                {
                    if (person.person_type_id.Equals((int)PersonType.BankUser))
                    {
                        user.branch_user_flag = model.UserModel.IsBranchUser;
                        user.role_id = Convert.ToInt32(model.UserModel.RoleId);
                    }

                    if (user.status_id.Equals((int)UserAccountStatus.Dormant) && Convert.ToInt32(model.UserModel.AccountStatusId).Equals((int)UserAccountStatus.Active))
                    {
                        user.last_login_date = DateTime.UtcNow.AddHours(1);
                        user.last_logout_date = DateTime.UtcNow.AddHours(1).AddMinutes(Convert.ToDouble(ConfigurationUtility.GetAppSettingValue("UserInactivityPeriodInDays")));
                    }

                    user.status_id = Convert.ToInt32(model.UserModel.AccountStatusId);


                    var makerCheckerActionData = JsonConvert.SerializeObject(user);
                    var sol = userData.branch_user_flag ? FinacleServices.GetFinacleSol2(userData.username, CallerFormName, callerFormMethod, callerIpAddress) : null;
                    var makerCheckerLog = new makercheckerlog
                    {
                        maker_checker_category_id = (int)MakerCheckerCategory.User,
                        maker_checker_type_id = (int)MakerCheckerType.EditExistingUser,
                        action_name = makerCheckerAction,
                        action_details = makerCheckerActionDetails,
                        action_data = makerCheckerActionData,
                        maker_person_id = userData.person_id,
                        maker_username = userData.username,
                        maker_fullname = $"{userData.person?.surname}, {userData.person?.first_name} {userData.person?.middle_name}",

                        maker_sol_id = sol?.SolId,
                        maker_sol_name = sol?.SolName,
                        maker_sol_address = sol?.SolAddress,

                        maker_checker_status = (int)MakerCheckerStatus.Initiated,
                        date_made = DateTime.UtcNow.AddHours(1),
                        query_string = Guid.NewGuid()
                    };

                    var makerCheckerLogInsertResult = MakerCheckerLogService.Insert(makerCheckerLog, CallerFormName, callerFormMethod, callerIpAddress);
                    if (makerCheckerLogInsertResult <= 0)
                    {
                        stopCheckFlag = true;
                        stopCheckMessage = "Unable to submit request for approval at the moment. Please try again later!";
                    }
                }

                if (!stopCheckFlag)
                {
                    //Task 1
                    new Task(() =>
                    {
                        var appActivity = new appactivity
                        {
                            audit_username = userData.username,
                            audit_fullname = $"{userData.person?.surname}, {userData.person?.first_name} {userData.person?.middle_name}",
                            audit_ipaddress = callerIpAddress,
                            audit_macaddress = callerMacAddress,
                            operation = $"Initiated - {makerCheckerAction}".ToUpper(),
                            comments = $"Initiated - {makerCheckerActionDetails}",
                            created_on = DateTime.Now,
                            created_by = "System"
                        };

                        AppActivityService.Insert(appActivity, CallerFormName, callerFormMethod, callerIpAddress);

                    }).Start();

                    //Task 2
                    new Task(() =>
                    {
                        if (!string.IsNullOrEmpty(checkerEmails.Trim().Replace(",", "")))
                        {
                            var templatePath = Server.MapPath(ConfigurationUtility.GetAppSettingValue("PendingItemsNotificationBodyTemplate"));
                            var mailBody = System.IO.File.ReadAllText(templatePath).Replace("{ItemType}", "User Editing");
                            var mailRecipient = checkerEmails;
                            EmailLogManager.Log(EmailType.PendingItemsNotification, mailBody, mailRecipient, "", "", CallerFormName, callerFormMethod, callerIpAddress);
                        }

                    }).Start();

                    AlertUser("Successfully submitted request for approval. Your changes will be effected once approved.", AlertType.Success);
                }
                else
                {
                    AlertUser(stopCheckMessage, AlertType.Error);
                    return RedirectToAction("EditExistingUser", new { q = model.UserModel.UrlQueryString });
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(CallerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return RedirectToAction("ViewUsers");
        }
        
        [HttpGet]
        [Route("view-pending-created-users")]
        public ActionResult ViewPendingCreatedUsers()
        {
            const string callerFormMethod = "HttpGet|ViewPendingCreatedUsers";

            try
            {
                var checkerEmails = "";
                if (!AuthorizeRequest(Request.Url.AbsolutePath, ref checkerEmails, CallerFormName, callerFormMethod, callerIpAddress))
                {
                    AlertUser("Unauthorised Access! You may not have been permitted to access the resource. Kindly login to re-authenticate. If issue persist, contact administrator.", AlertType.Error);
                    return RedirectToAction("Logout", "Account");
                }

                var sol = userData.branch_user_flag ? FinacleServices.GetFinacleSol2(userData.username, CallerFormName, callerFormMethod, callerIpAddress) : null;
                var logs = userData.branch_user_flag ? MakerCheckerLogService.GetWithMakerCheckerCategoryAndSolIdAndStatus((int)MakerCheckerCategory.User, sol?.SolId, (int)MakerCheckerStatus.Initiated, CallerFormName, callerFormMethod, callerIpAddress) : MakerCheckerLogService.GetWithMakerCheckerCategoryAndStatus((int)MakerCheckerCategory.User, (int)MakerCheckerStatus.Initiated, CallerFormName, callerFormMethod, callerIpAddress);
                if (logs.Count > 0)
                {
                    MakerCheckerLogModels = logs.Select(log => new MakerCheckerLogModel
                    {
                        MakerCheckerCategoryId = Convert.ToString(log.maker_checker_category_id),
                        MakerCheckerTypeId = Convert.ToString(log.maker_checker_type_id),
                        ActionName = log.action_name,
                        ActionDetails = log.action_details,
                        MakerId = log.maker_username,
                        MakerFullname = log.maker_fullname,
                        MakerSolId = !string.IsNullOrEmpty(log.maker_sol_id) ? log.maker_sol_id : "n/p",
                        MakerCheckerStatus = Convert.ToString(log.maker_checker_status),
                        DateMade = Convert.ToDateTime(log.date_made).ToString("dd-MM-yyyy hh:mm tt"),
                        CheckerId = !string.IsNullOrEmpty(log.checker_username) ? log.checker_username : "n/p",
                        CheckerFullname = !string.IsNullOrEmpty(log.checker_fullname) ? log.checker_fullname : "n/p",
                        CheckerSolId = !string.IsNullOrEmpty(log.checker_sol_id) ? log.checker_sol_id : "n/p",
                        CheckerRemarks = !string.IsNullOrEmpty(log.checker_remarks) ? log.checker_remarks : "n/p",
                        DateChecked = !string.IsNullOrEmpty(Convert.ToString(log.date_checked)) ? Convert.ToDateTime(log.date_checked).ToString("dd-MM-yyyy hh:mm tt") : "n/p",
                        QueryString = Convert.ToString(log.query_string),

                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(CallerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return View(MakerCheckerLogModels);
        }

        [HttpGet]
        [Route("review-pending-created-user")]
        public ActionResult ReviewPendingCreatedUser(string q)
        {
            const string callerFormMethod = "HttpGet|ReviewPendingCreatedUser";

            try
            {
                var log = MakerCheckerLogService.GetWithQueryString(q, CallerFormName, callerFormMethod, callerIpAddress);
                if (log != null)
                {
                    reviewMakerCheckerLogViewModel.MakerCheckerLogModel = new MakerCheckerLogModel
                    {
                        MakerCheckerCategoryId = Convert.ToString(log.maker_checker_category_id),
                        MakerCheckerTypeId = Convert.ToString(log.maker_checker_type_id),
                        ActionName = log.action_name,
                        ActionDetails = log.action_details,
                        ActionData = log.action_data,
                        MakerId = log.maker_username,
                        MakerFullname = log.maker_fullname,
                        MakerSolId = log.maker_sol_id,
                        MakerCheckerStatus = Convert.ToString(log.maker_checker_status),
                        DateMade = Convert.ToDateTime(log.date_made).ToString("dd-MM-yyyy hh:mm tt"),
                        CheckerId = log.checker_username,
                        CheckerFullname = log.checker_fullname,
                        CheckerSolId = log.checker_sol_id,
                        CheckerRemarks = log.checker_remarks,
                        DateChecked = !string.IsNullOrEmpty(Convert.ToString(log.date_checked)) ? Convert.ToDateTime(log.date_checked).ToString("dd-MM-yyyy hh:mm tt") : "n/p",
                        QueryString = Convert.ToString(log.query_string)
                    };
                }

            }
            catch (Exception ex)
            {
                LogUtility.LogError(CallerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return View(reviewMakerCheckerLogViewModel);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("approve-pending-created-user")]
        public ActionResult ApprovePendingCreatedUser(MakerCheckerLogModel model)
        {
            const string callerFormMethod = "HttpPost|ApprovePendingCreatedUser";

            try
            {
                var checkerEmails = "";
                if (!AuthorizeRequest(Request.Url.AbsolutePath, ref checkerEmails, CallerFormName, callerFormMethod, callerIpAddress))
                {
                    AlertUser("Unauthorised Access! You may not have been permitted to access the resource. Kindly login to re-authenticate. If issue persist, contact administrator.", AlertType.Error);
                    return RedirectToAction("Logout", "Account");
                }

                var log = MakerCheckerLogService.GetWithQueryString(model.QueryString, CallerFormName, callerFormMethod, callerIpAddress);
                if (log == null)
                {
                    stopCheckFlag = true;
                    stopCheckMessage = "Unable to retrieve pending created user at the moment, kindly try again later";
                }
                
                long applyResult = 0;
                if (!stopCheckFlag)
                {
                    var applyObject = JsonConvert.DeserializeObject<user>(log.action_data);
                    if (log.maker_checker_type_id.Equals((int)MakerCheckerType.CreateNewUser))
                    {
                        var user = UsersService.GetWithUsername(applyObject.username, CallerFormName, callerFormMethod, callerIpAddress);
                        if (user != null)
                        {
                            stopCheckFlag = true;
                            stopCheckMessage = "Unable to approve pending created user at the moment; user with same username already exists";
                        }

                        long personInsertResult = 0;
                        if (!stopCheckFlag)
                        {
                            personInsertResult = PersonService.Insert(applyObject.person, CallerFormName, callerFormMethod, callerIpAddress);
                            if (personInsertResult <= 0)
                            {
                                stopCheckFlag = true;
                                stopCheckMessage = "Unable to approve pending created user at the moment; person could not be profiled. Kindly try again later!";
                            }
                        }

                        if (!stopCheckFlag)
                        {
                            applyObject.person_id = personInsertResult;
                            applyObject.approved_on = DateTime.UtcNow.AddHours(1);
                            applyObject.approved_by = userData.username;
                            applyResult = UsersService.Insert(applyObject, CallerFormName, callerFormMethod, callerIpAddress);

                            if (applyResult > 0)
                            {
                                var templatePath = Server.MapPath(ConfigurationUtility.GetAppSettingValue("AccountCreationNotificationBodyTemplate"));
                                var mailBody = System.IO.File.ReadAllText(templatePath).Replace("{Name}", applyObject.person.first_name).Replace("{Username}", applyObject.username).Replace("{Password}", "[Active Directory Password]");
                                var mailRecipient = applyObject.person.email_address;
                                EmailLogManager.Log(EmailType.AccountCreationNotification, mailBody, mailRecipient, "", "", CallerFormName, callerFormMethod, callerIpAddress);
                            }
                        }
                        
                    }
                    else if (log.maker_checker_type_id.Equals((int)MakerCheckerType.EditExistingUser))
                    {
                        var user = UsersService.GetWithUrlQueryString(Convert.ToString(applyObject.query_string), CallerFormName, callerFormMethod, callerIpAddress);
                        if (user == null)
                        {
                            stopCheckFlag = true;
                            stopCheckMessage = "Unable to retrieve existing user at the moment, kindly try again later";
                        }
                        
                        if (!stopCheckFlag)
                        {
                            applyObject.last_modified_on = DateTime.UtcNow.AddHours(1);
                            applyObject.last_modified_by = userData.username;
                            applyResult = UsersService.Update(applyObject, CallerFormName, callerFormMethod, callerIpAddress);

                            if (user.status_id.Equals((int)UserAccountStatus.LockedOut) && applyObject.status_id.Equals((int)UserAccountStatus.Active))
                            {
                                var failedLogonAttempts = UserAccessActivityService.GetAllFailedLogonAttempts(user.username, CallerFormName, callerFormMethod, callerIpAddress);
                                if (failedLogonAttempts != null)
                                {
                                    foreach (var useraccessactivity in failedLogonAttempts)
                                    {
                                        if (useraccessactivity.ignore_for_account_lock == null)
                                        {
                                            useraccessactivity.ignore_for_account_lock = true;
                                            UserAccessActivityService.Update(useraccessactivity, CallerFormName, callerFormMethod, callerIpAddress);
                                        }
                                        else
                                        {
                                            if (!Convert.ToBoolean(useraccessactivity.ignore_for_account_lock))
                                            {
                                                useraccessactivity.ignore_for_account_lock = true;
                                                UserAccessActivityService.Update(useraccessactivity, CallerFormName, callerFormMethod, callerIpAddress);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        stopCheckFlag = true;
                        stopCheckMessage = "Invalid Maker-Checker Type";
                    }
                }

                if (!stopCheckFlag)
                {
                    if (applyResult <= 0)
                    {
                        stopCheckFlag = true;
                        stopCheckMessage = "Unable to approve pending created/edited user at the moment. Kindly try again later!";
                    }
                }

                if (!stopCheckFlag)
                {
                    var sol = userData.branch_user_flag ? FinacleServices.GetFinacleSol2(userData.username, CallerFormName, callerFormMethod, callerIpAddress) : null;
                    log.maker_checker_status = (int)MakerCheckerStatus.Approved;
                    log.checker_person_id = userData.person_id;
                    log.checker_username = userData.username;
                    log.checker_fullname = $"{userData.person?.surname}, {userData.person?.first_name} {userData.person?.middle_name}";
                    log.checker_sol_id = sol?.SolId;
                    log.checker_sol_name = sol?.SolName;
                    log.checker_sol_address = sol?.SolAddress;
                    log.checker_remarks = model.CheckerRemarks;
                    log.date_checked = DateTime.UtcNow.AddHours(1);

                    var logUpdateResult = MakerCheckerLogService.Update(log, CallerFormName, callerFormMethod, callerIpAddress);
                    if (logUpdateResult <= 0)
                    {
                        stopCheckFlag = true;
                        stopCheckMessage = "Unable to approve pending created user at the moment; maker-checker log could not be updated. Try again later!";
                    }
                }

                if (!stopCheckFlag)
                {
                    //Task 1
                    new Task(() =>
                    {
                        var appActivity = new appactivity
                        {
                            audit_username = userData.username,
                            audit_fullname = $"{userData.person?.surname}, {userData.person?.first_name} {userData.person?.middle_name}",
                            audit_ipaddress = callerIpAddress,
                            audit_macaddress = callerMacAddress,
                            operation = $"Approved - {log.action_name}".ToUpper(),
                            comments = $"Approved - {log.action_details}",
                            created_on = DateTime.Now,
                            created_by = "System"
                        };

                        AppActivityService.Insert(appActivity, CallerFormName, callerFormMethod, callerIpAddress);

                    }).Start();

                    //Task 2
                    new Task(() =>
                    {
                        var makerFirstname = "";
                        var makerEmailAddress = "";
                        var makerPerson = PersonService.GetWithPersonId(Convert.ToString(log.maker_person_id), CallerFormName, callerFormMethod, callerIpAddress);
                        if (makerPerson != null)
                        {
                            makerFirstname = makerPerson.first_name;
                            makerEmailAddress = makerPerson.email_address;
                        }

                        if (!string.IsNullOrEmpty(makerEmailAddress))
                        {
                            var templatePath = Server.MapPath(ConfigurationUtility.GetAppSettingValue("ApprovalNotificationBodyTemplate"));
                            var mailBody = System.IO.File.ReadAllText(templatePath).Replace("{Name}", makerFirstname).Replace("{ActionName}", log.action_name).Replace("{ActionDetails}", log.action_details).Replace("{ApprovedBy}", log.checker_fullname).Replace("{ApprovedOn}", Convert.ToDateTime(log.date_checked).ToString("dd-MM-yyyy hh:mm tt"));
                            var mailRecipient = makerEmailAddress;
                            EmailLogManager.Log(EmailType.ApprovalNotification, mailBody, mailRecipient, "", "", CallerFormName, callerFormMethod, callerIpAddress);
                        }

                    }).Start();

                    //Task 3


                    AlertUser("Successfully approved request. Your changes have been effected.", AlertType.Success);
                }
                else
                {
                    AlertUser(stopCheckMessage, AlertType.Error);
                    return RedirectToAction("ReviewPendingCreatedUser", new { q = model.QueryString });
                }

            }
            catch (Exception ex)
            {
                LogUtility.LogError(CallerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return RedirectToAction("ViewPendingCreatedUsers");
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("reject-pending-created-user")]
        public ActionResult RejectPendingCreatedUser(MakerCheckerLogModel model)
        {
            const string callerFormMethod = "HttpPost|RejectPendingCreatedUser";

            try
            {
                var checkerEmails = "";
                if (!AuthorizeRequest(Request.Url.AbsolutePath, ref checkerEmails, CallerFormName, callerFormMethod, callerIpAddress))
                {
                    AlertUser("Unauthorised Access! You may not have been permitted to access the resource. Kindly login to re-authenticate. If issue persist, contact administrator.", AlertType.Error);
                    return RedirectToAction("Logout", "Account");
                }

                var log = MakerCheckerLogService.GetWithQueryString(model.QueryString, CallerFormName, callerFormMethod, callerIpAddress);
                if (log == null)
                {
                    stopCheckFlag = true;
                    stopCheckMessage = "Unable to retrieve pending created user at the moment, kindly try again later";
                }

                if (!stopCheckFlag)
                {
                    var sol = userData.branch_user_flag ? FinacleServices.GetFinacleSol2(userData.username, CallerFormName, callerFormMethod, callerIpAddress) : null;
                    log.maker_checker_status = (int)MakerCheckerStatus.Rejected;
                    log.checker_person_id = userData.person_id;
                    log.checker_username = userData.username;
                    log.checker_fullname = $"{userData.person?.surname}, {userData.person?.first_name} {userData.person?.middle_name}";
                    log.checker_sol_id = sol?.SolId;
                    log.checker_sol_name = sol?.SolName;
                    log.checker_sol_address = sol?.SolAddress;
                    log.checker_remarks = model.CheckerRemarks;
                    log.date_checked = DateTime.UtcNow.AddHours(1);

                    var logUpdateResult = MakerCheckerLogService.Update(log, CallerFormName, callerFormMethod, callerIpAddress);
                    if (logUpdateResult <= 0)
                    {
                        stopCheckFlag = true;
                        stopCheckMessage = "Unable to reject pending created user at the moment; maker-checker log could not be updated. Try again later!";
                    }
                }

                if (!stopCheckFlag)
                {
                    //Task 1
                    new Task(() =>
                    {
                        var appActivity = new appactivity
                        {
                            audit_username = userData.username,
                            audit_fullname = $"{userData.person?.surname}, {userData.person?.first_name} {userData.person?.middle_name}",
                            audit_ipaddress = callerIpAddress,
                            audit_macaddress = callerMacAddress,
                            operation = $"Rejected - {log.action_name}".ToUpper(),
                            comments = $"Rejected - {log.action_details} with remarks- {model.CheckerRemarks}",
                            created_on = DateTime.Now,
                            created_by = "System"
                        };

                        AppActivityService.Insert(appActivity, CallerFormName, callerFormMethod, callerIpAddress);

                    }).Start();

                    //Task 2
                    new Task(() =>
                    {
                        var makerFirstname = "";
                        var makerEmailAddress = "";
                        var makerPerson = PersonService.GetWithPersonId(Convert.ToString(log.maker_person_id), CallerFormName, callerFormMethod, callerIpAddress);
                        if (makerPerson != null)
                        {
                            makerFirstname = makerPerson.first_name;
                            makerEmailAddress = makerPerson.email_address;
                        }

                        if (!string.IsNullOrEmpty(makerEmailAddress))
                        {
                            var templatePath = Server.MapPath(ConfigurationUtility.GetAppSettingValue("RejectionNotificationBodyTemplate"));
                            var mailBody = System.IO.File.ReadAllText(templatePath).Replace("{Name}", makerFirstname).Replace("{ActionName}", log.action_name).Replace("{ActionDetails}", log.action_details).Replace("{RejectedBy}", log.checker_fullname).Replace("{RejectedOn}", Convert.ToDateTime(log.date_checked).ToString("dd-MM-yyyy hh:mm tt")).Replace("{RejectionReason}", log.checker_remarks);
                            var mailRecipient = makerEmailAddress;
                            EmailLogManager.Log(EmailType.RejectionNotification, mailBody, mailRecipient, "", "", CallerFormName, callerFormMethod, callerIpAddress);
                        }

                    }).Start();

                    AlertUser($"Successfully rejected request with remarks '{model.CheckerRemarks}'", AlertType.Success);
                }
                else
                {
                    AlertUser(stopCheckMessage, AlertType.Error);
                    return RedirectToAction("ReviewPendingCreatedUser", new { q = model.QueryString });
                }

            }
            catch (Exception ex)
            {
                LogUtility.LogError(CallerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return RedirectToAction("ViewPendingCreatedUsers");
        }

        [Route("view-user-profile")]
        public ActionResult ViewUserProfile(string q)
        {
            const string callerFormMethod = "HttpGet|ViewUserProfile";

            try
            {
                var checkerEmails = "";
                if (!AuthorizeRequest(Request.Url.AbsolutePath, ref checkerEmails, CallerFormName, callerFormMethod, callerIpAddress))
                {
                    AlertUser("Unauthorised Access! You may not have been permitted to access the resource. Kindly login to re-authenticate. If issue persist, contact administrator.", AlertType.Error);
                    return RedirectToAction("Logout", "Account");
                }

                var user = UsersService.GetWithUrlQueryString(q, CallerFormName, callerFormMethod, callerIpAddress);
                if (user != null)
                {
                    var surname = "";
                    var firstname = "";
                    var middlename = "";
                    var mobileNumber = "";
                    var emailAddress = "";
                    var passport = "";

                    var person = PersonService.GetWithPersonId(Convert.ToString(user.person_id), CallerFormName, callerFormMethod, callerIpAddress);
                    if (person != null)
                    {
                        surname = person?.surname;
                        firstname = person?.first_name;
                        middlename = person?.middle_name;
                        mobileNumber = person?.mobile_number;
                        emailAddress = person?.email_address;
                        passport = person?.passport;
                    }
                    
                    viewUserProfileViewModel.UserModel = new UserModel
                    {
                        Username = user.username,
                        AuthenticationTypeId = Convert.ToString(user.authentication_type_id),
                        AuthenticationTypeName = CacheData.Useraccountauthenticationtypes.FirstOrDefault(c => c.id.Equals(user.authentication_type_id))?.authentication_type_name,
                        IsBranchUser = user.branch_user_flag,

                        LocalPassword = user.local_password,
                        Surname = surname,
                        Firstname = firstname,
                        Middlename = middlename,
                        MobileNumber = mobileNumber,
                        EmailAddress = emailAddress,
                        Passport = passport,
                        IsVisiblePassportUploadLink = userData.person_id.Equals(user.person_id),
                        IsVisibleChangePasswordLink = userData.person_id.Equals(user.person_id) && user.authentication_type_id.Equals((int)UserAccountAuthenticationType.LocalAccountAuthentication),

                        RoleId = Convert.ToString(user.role_id),
                        RoleName = RoleService.GetWithRoleId(Convert.ToString(user.role_id), CallerFormName, callerFormMethod, callerIpAddress)?.role_name,
                        AccountStatusId = Convert.ToString(user.status_id),
                        AccountStatusName = Enum.GetName(typeof(UserAccountStatus), user.status_id),
                        LastLoginDate = !string.IsNullOrEmpty(Convert.ToString(user.last_login_date)) ? Convert.ToDateTime(user.last_login_date).ToString("dd-MM-yyyy hh:mm tt") : "n/p",
                        LastLogoutDate = !string.IsNullOrEmpty(Convert.ToString(user.last_logout_date)) ? Convert.ToDateTime(user.last_logout_date).ToString("dd-MM-yyyy hh:mm tt") : "n/p",
                        CreatedOn = Convert.ToDateTime(user.created_on).ToString("dd-MM-yyyy hh:mm tt"),
                        CreatedBy = user.created_by,
                        ApprovedOn = !string.IsNullOrEmpty(Convert.ToString(user.approved_on)) ? Convert.ToDateTime(user.approved_on).ToString("dd-MM-yyyy hh:mm tt") : "n/p",
                        ApprovedBy = user.approved_by,
                        LastModifiedOn = !string.IsNullOrEmpty(Convert.ToString(user.last_modified_on)) ? Convert.ToDateTime(user.last_modified_on).ToString("dd-MM-yyyy hh:mm tt") : "n/p",
                        LastModifiedBy = user.last_modified_by,
                        UrlQueryString = Convert.ToString(user.query_string)
                    };
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(CallerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return View(viewUserProfileViewModel);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("upload-user-profile-picture")]
        public ActionResult UploadUserProfilePicture(UserModel model)
        {
            const string callerFormMethod = "HttpPost|UploadUserProfilePicture";

            try
            {
                var checkerEmails = "";
                if (!AuthorizeRequest(Request.Url.AbsolutePath, ref checkerEmails, CallerFormName, callerFormMethod, callerIpAddress))
                {
                    AlertUser("Unauthorised Access! You may not have been permitted to access the resource. Kindly login to re-authenticate. If issue persist, contact administrator.", AlertType.Error);
                    return RedirectToAction("Logout", "Account");
                }

                HttpPostedFileBase httpPostedFileBase = null;
                if (!stopCheckFlag)
                {
                    httpPostedFileBase = Request.Files[0];
                    if (httpPostedFileBase == null || httpPostedFileBase.ContentLength == 0)
                    {
                        stopCheckFlag = true;
                        stopCheckMessage = "Invalid upload file!";
                    }
                }

                var supportedFileTypes = new[] { ".png", ".jpg", ".jpeg", ".PNG", ".JPG", ".JPEG" };
                if (!stopCheckFlag)
                {
                    var fileExtension = System.IO.Path.GetExtension(httpPostedFileBase.FileName);
                    if (!supportedFileTypes.Contains(fileExtension))
                    {
                        stopCheckFlag = true;
                        stopCheckMessage = "Invalid file format - file must be in any of .png, .jpg, .jpeg formats!";
                    }
                }

                if (!stopCheckFlag)
                {
                    var fileSize = httpPostedFileBase.ContentLength;
                    var maxUploadSize = ConfigurationUtility.GetAppSettingValue("MaxProfilePixUploadSize");
                    if (fileSize > Convert.ToInt32(maxUploadSize))
                    {
                        stopCheckFlag = true;
                        stopCheckMessage = "Invalid file upload size - profile picture upload size is restricted to 15kb";
                    }
                }

                user user = null;
                if (!stopCheckFlag)
                {
                    user = UsersService.GetWithUrlQueryString(model.UrlQueryString, CallerFormName, callerFormMethod, callerIpAddress);
                    if (user == null)
                    {
                        stopCheckFlag = true;
                        stopCheckMessage = "Unable to retrieve user with query parameter at the moment, kindly try again later";
                    }
                }

                person person = null;
                if (!stopCheckFlag)
                {
                    person = PersonService.GetWithPersonId(Convert.ToString(user.person_id), CallerFormName, callerFormMethod, callerIpAddress);
                    if (person == null)
                    {
                        stopCheckFlag = true;
                        stopCheckMessage = "Unable to retrieve user personal details at the moment, kindly try again later";
                    }
                }

                var passportSaveUrl = "";
                if (!stopCheckFlag)
                {
                    passportSaveUrl = SaveFile(httpPostedFileBase, $"{model.UrlQueryString}-profile-pix", CallerFormName, callerFormMethod, callerIpAddress);
                    if (string.IsNullOrEmpty(passportSaveUrl))
                    {
                        stopCheckFlag = true;
                        stopCheckMessage = "Unable to save profile picture file at the moment, kindly try again later";
                    }
                }

                long personUpdateResult = 0;
                if (!stopCheckFlag)
                {
                    person.passport = passportSaveUrl;
                    person.last_modified_on = DateTime.UtcNow.AddHours(1);
                    person.last_modified_by = userData.username;
                    personUpdateResult = PersonService.Update(person, CallerFormName, callerFormMethod, callerIpAddress);
                    if (personUpdateResult <= 0)
                    {
                        stopCheckFlag = true;
                        stopCheckMessage = "Unable to complete request at the moment; personal details could not be saved. Kindly try again later!";
                    }
                }

                if (!stopCheckFlag)
                {
                    //Task 1
                    new Task(() =>
                    {
                        var appActivity = new appactivity
                        {
                            audit_username = userData.username,
                            audit_fullname = $"{userData.person?.surname}, {userData.person?.first_name} {userData.person?.middle_name}",
                            audit_ipaddress = callerIpAddress,
                            audit_macaddress = callerMacAddress,
                            operation = $"Uploaded Profile Picture".ToUpper(),
                            comments = $"Uploaded Profile Picture - {passportSaveUrl}",
                            created_on = DateTime.Now,
                            created_by = "System"
                        };

                        AppActivityService.Insert(appActivity, CallerFormName, callerFormMethod, callerIpAddress);

                    }).Start();

                    AlertUser("Profile Picture Has Been Successfully Uploaded and Effected.", AlertType.Success);
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

        [Route("reset-user-login-credentials")]
        public ActionResult ResetUserLoginCredentials(string q)
        {
            const string callerFormMethod = "HttpGet|ResetUserLoginCredentials";

            try
            {
                var checkerEmails = "";
                if (!AuthorizeRequest(Request.Url.AbsolutePath, ref checkerEmails, CallerFormName, callerFormMethod, callerIpAddress))
                {
                    AlertUser("Unauthorised Access! You may not have been permitted to access the resource. Kindly login to re-authenticate. If issue persist, contact administrator.", AlertType.Error);
                    return RedirectToAction("Logout", "Account");
                }
                
                var user = UsersService.GetWithUrlQueryString(q, CallerFormName, callerFormMethod, callerIpAddress);
                if (user == null)
                {
                    stopCheckFlag = true;
                    stopCheckMessage = "Unable to retrieve existing user at the moment, kindly try again later";
                }
                
                if (!stopCheckFlag)
                {
                    if (!user.authentication_type_id.Equals((int)UserAccountAuthenticationType.LocalAccountAuthentication))
                    {
                        stopCheckFlag = true;
                        stopCheckMessage = "Unable to complete request; user account is not on local authentication";
                    }
                }

                var generatedPassword = "";
                long userUpdateResult = 0;
                if (!stopCheckFlag)
                {
                    generatedPassword = new Password(includeLowerCase, includeUpperCase, includeNumeric, includeSpecial, passwordLength).Next();
                    user.local_password = EncryptionUtility.Encrypt(generatedPassword, encryptionKey);
                    user.password_expiry_date = null;
                    userUpdateResult = UsersService.Update(user, CallerFormName, callerFormMethod, callerIpAddress);
                    if (userUpdateResult <= 0)
                    {
                        stopCheckFlag = true;
                        stopCheckMessage = "Unable to complete request at the moment; password could not be updated. Kindly try again later!";
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
                            audit_fullname = $"{userData.person?.surname}, {userData.person?.first_name} {userData.person?.middle_name}",
                            audit_ipaddress = callerIpAddress,
                            audit_macaddress = callerMacAddress,
                            operation = $"Password Reset".ToUpper(),
                            comments = $"'{user.username}' password reset by '{userData.username}'",
                            created_on = DateTime.Now,
                            created_by = "System"
                        };

                        AppActivityService.Insert(appActivity, CallerFormName, callerFormMethod, callerIpAddress);

                    }).Start();

                    //Task 2
                    new Task(() =>
                    {
                        var firstname = "";
                        var emailAddress = "";
                        var person = PersonService.GetWithPersonId(Convert.ToString(user.person_id), CallerFormName, callerFormMethod, callerIpAddress);
                        if (person != null)
                        {
                            firstname = person.first_name;
                            emailAddress = person.email_address;
                        }

                        if (!string.IsNullOrEmpty(emailAddress))
                        {
                            var templatePath = Server.MapPath(ConfigurationUtility.GetAppSettingValue("AccountResetNotificationBodyTemplate"));
                            var mailBody = System.IO.File.ReadAllText(templatePath).Replace("{Name}", firstname).Replace("{Username}", user.username).Replace("{Password}", generatedPassword);
                            var mailRecipient = emailAddress;
                            EmailLogManager.Log(EmailType.AccountResetNotification, mailBody, mailRecipient, "", "", CallerFormName, callerFormMethod, callerIpAddress);
                        }

                    }).Start();

                    AlertUser("Password Reset Successful!", AlertType.Success);
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

            return RedirectToAction("ViewUsers");
        }

        #endregion
    }
}