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

namespace App.Web.Controllers
{
    [Authorize]
    [RoutePrefix("role-administration")]
    public class RoleAdministrationController : BaseController
    {
        private const string CallerFormName = "RoleAdministrationController";
        private readonly string callerIpAddress;
        private readonly string callerMacAddress;

        private readonly ViewRolesViewModel viewRolesViewModel;
        private readonly CreateRolesViewModel createRolesViewModel;
        private readonly EditRolesViewModel editRolesViewModel;
        private readonly ReviewMakerCheckerLogViewModel reviewMakerCheckerLogViewModel;
        private List<MakerCheckerLogModel> MakerCheckerLogModels;
        private readonly user userData;

        private bool stopCheckFlag = false;
        private string stopCheckMessage = "";

        public RoleAdministrationController()
        {
            callerIpAddress = IpAddressManager.GetClientComputerIpAddress();
            callerMacAddress = MacAddressManager.GetClientComputerMacAddress();
            viewRolesViewModel = new ViewRolesViewModel();
            createRolesViewModel = new CreateRolesViewModel();
            editRolesViewModel = new EditRolesViewModel();
            reviewMakerCheckerLogViewModel = new ReviewMakerCheckerLogViewModel();
            MakerCheckerLogModels = new List<MakerCheckerLogModel>();
            userData = GetUser(CallerFormName, "Ctor|RoleAdministrationController", callerIpAddress);
        }

        #region ActionResult

        [HttpGet]
        [Route("view-roles")]
        public ActionResult ViewRoles()
        {
            const string callerFormMethod = "HttpGet|ViewRoles";

            try
            {
                var checkerEmails = "";
                if (!AuthorizeRequest(Request.Url.AbsolutePath, ref checkerEmails, CallerFormName, callerFormMethod, callerIpAddress))
                {
                    AlertUser("Unauthorised Access! You may not have been permitted to access the resource. Kindly login to re-authenticate. If issue persist, contact administrator.", AlertType.Error);
                    return RedirectToAction("Logout", "Account");
                }

                var roles = RoleService.GetAll(CallerFormName, callerFormMethod, callerIpAddress);
                if (roles.Count > 0)
                {
                    viewRolesViewModel.RoleModels = roles.Select(role => new RoleModel
                    {
                        RoleName = role.role_name,
                        ActiveFlag = Convert.ToBoolean(role.active_flag),
                        ReservedFlag = Convert.ToBoolean(role.system_reserved_flag),
                        CreatedOn = Convert.ToDateTime(role.created_on).ToString("dd-MM-yyyy hh:mm tt"),
                        CreatedBy = role.created_by,
                        ApprovedOn = !string.IsNullOrEmpty(Convert.ToString(role.approved_on)) ? Convert.ToDateTime(role.approved_on).ToString("dd-MM-yyyy hh:mm tt") : "n/p",
                        ApprovedBy = role.approved_by,
                        LastModifiedOn = !string.IsNullOrEmpty(Convert.ToString(role.last_modified_on)) ? Convert.ToDateTime(role.last_modified_on).ToString("dd-MM-yyyy hh:mm tt") : "n/p",
                        LastModifiedBy = role.last_modified_by,
                        UrlQueryString = Convert.ToString(role.query_string)

                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(CallerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return View(viewRolesViewModel);

        }

        [HttpGet]
        [Route("create-new-role")]
        public ActionResult CreateNewRole()
        {
            return View(createRolesViewModel);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("create-new-role")]
        public ActionResult CreateNewRole(RoleModel model)
        {
            const string callerFormMethod = "HttpPost|CreateNewRole";

            try
            {
                var checkerEmails = "";
                if (!AuthorizeRequest(Request.Url.AbsolutePath, ref checkerEmails, CallerFormName, callerFormMethod, callerIpAddress))
                {
                    AlertUser("Unauthorised Access! You may not have been permitted to access the resource. Kindly login to re-authenticate. If issue persist, contact administrator.", AlertType.Error);
                    return RedirectToAction("Logout", "Account");
                }

                if (!ModelState.IsValid)
                {
                    var errorMessages = string.Join("<br/>", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
                    stopCheckFlag = true;
                    stopCheckMessage = errorMessages;

                }

                var makerCheckerAction = "Role Creation";
                var makerCheckerActionDetails = $"Create New Role '{model.RoleName}'";

                role role = null;
                if (!stopCheckFlag)
                {
                    role = RoleService.GetWithRoleName(model.RoleName, CallerFormName, callerFormMethod, callerIpAddress);
                    if (role != null)
                    {
                        stopCheckFlag = true;
                        stopCheckMessage = "Unable to intiate request; rolename already exists.";
                    }
                }

                if (!stopCheckFlag)
                {
                    role = new role
                    {
                        role_name = model.RoleName,
                        active_flag = true,
                        created_on = DateTime.UtcNow.AddHours(1),
                        created_by = $"{userData.person?.surname}, {userData.person?.first_name} {userData.person?.middle_name}",
                        system_reserved_flag = false,
                        query_string = Guid.NewGuid()
                    };

                    var makerCheckerActionData = JsonConvert.SerializeObject(role);
                    var sol = userData.branch_user_flag ? FinacleServices.GetFinacleSol2(userData.username, CallerFormName, callerFormMethod, callerIpAddress) : null;
                    var makerCheckerLog = new makercheckerlog
                    {
                        maker_checker_category_id = (int)MakerCheckerCategory.Role,
                        maker_checker_type_id = (int)MakerCheckerType.CreateNewRole,
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
                            var mailBody = System.IO.File.ReadAllText(templatePath).Replace("{ItemType}", "Role Creation");
                            var mailRecipient = checkerEmails;
                            EmailLogManager.Log(EmailType.PendingItemsNotification, mailBody, mailRecipient, "", "", CallerFormName, callerFormMethod, callerIpAddress);
                        }

                    }).Start();

                    AlertUser("Successfully submitted request for approval. Your changes will be effected once approved.", AlertType.Success);
                }
                else
                {
                    AlertUser(stopCheckMessage, AlertType.Error);
                    return View(createRolesViewModel);
                }

            }
            catch (Exception ex)
            {
                LogUtility.LogError(CallerFormName, callerFormMethod, callerIpAddress, ex);
            }


            return RedirectToAction("CreateNewRole");
        }

        [HttpGet]
        [Route("edit-existing-role")]
        public ActionResult EditExistingRole(string q)
        {
            const string callerFormMethod = "HttpGet|EditExistingRole";

            try
            {
                var role = RoleService.GetWithUrlQueryString(q, CallerFormName, callerFormMethod, callerIpAddress);
                if (role == null)
                {
                    stopCheckFlag = true;
                    stopCheckMessage = "No Role Found with the Search Parameter";
                }

                if (!stopCheckFlag)
                {
                    editRolesViewModel.RoleModel = new RoleModel
                    {
                        RoleName = role.role_name,
                        ActiveFlag = Convert.ToBoolean(role.active_flag),
                        UrlQueryString = Convert.ToString(role.query_string)
                    };
                }
                else
                {
                    AlertUser(stopCheckMessage, AlertType.Error);
                    return RedirectToAction("ViewRoles");
                }

            }
            catch (Exception ex)
            {
                LogUtility.LogError(CallerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return View(editRolesViewModel);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("edit-existing-role")]
        public ActionResult EditExistingRole(RoleModel model)
        {
            const string callerFormMethod = "HttpPost|EditExistingRole";

            try
            {
                var checkerEmails = "";
                if (!AuthorizeRequest(Request.Url.AbsolutePath, ref checkerEmails, CallerFormName, callerFormMethod, callerIpAddress))
                {
                    AlertUser("Unauthorised Access! You may not have been permitted to access the resource. Kindly login to re-authenticate. If issue persist, contact administrator.", AlertType.Error);
                    return RedirectToAction("Logout", "Account");
                }

                if (!ModelState.IsValid)
                {
                    var errorMessages = string.Join("<br/>", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
                    stopCheckFlag = true;
                    stopCheckMessage = errorMessages;
                }

                var makerCheckerAction = "Role Editing";
                var makerCheckerActionDetails = "";

                role role = null;
                if (!stopCheckFlag)
                {
                    role = RoleService.GetWithUrlQueryString(model.UrlQueryString, CallerFormName, callerFormMethod, callerIpAddress);
                    if (role == null)
                    {
                        stopCheckFlag = true;
                        stopCheckMessage = "Unable to retrieve existing role at the moment, kindly try again later";
                    }
                }

                if (!stopCheckFlag)
                {
                    if (!role.role_name.ToLower().Equals(model.RoleName.ToLower()))
                    {
                        var roleWithSameRoleName = RoleService.GetWithRoleName(model.RoleName, CallerFormName, callerFormMethod, callerIpAddress);
                        if (roleWithSameRoleName != null)
                        {
                            stopCheckFlag = true;
                            stopCheckMessage = "Unable to intiate request; rolename already exists.";
                        }
                    }
                }

                if (!stopCheckFlag)
                {
                    if (!role.role_name.Equals(model.RoleName))
                    {
                        makerCheckerActionDetails += $"Edit Role:{role.role_name} Name From '{role.role_name}' to '{model.RoleName}'|";
                    }

                    if (!role.active_flag.Equals(model.ActiveFlag))
                    {
                        makerCheckerActionDetails += $"Edit Role:{role.role_name} Active Flag From '{role.active_flag}' to '{model.ActiveFlag}'|";
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
                    role.role_name = model.RoleName;
                    role.active_flag = model.ActiveFlag;

                    var makerCheckerActionData = JsonConvert.SerializeObject(role);
                    var sol = userData.branch_user_flag ? FinacleServices.GetFinacleSol2(userData.username, CallerFormName, callerFormMethod, callerIpAddress) : null;
                    var makerCheckerLog = new makercheckerlog
                    {
                        maker_checker_category_id = (int)MakerCheckerCategory.Role,
                        maker_checker_type_id = (int)MakerCheckerType.EditExistingRole,
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
                            var mailBody = System.IO.File.ReadAllText(templatePath).Replace("{ItemType}", "Role Editing");
                            var mailRecipient = checkerEmails;
                            EmailLogManager.Log(EmailType.PendingItemsNotification, mailBody, mailRecipient, "", "", CallerFormName, callerFormMethod, callerIpAddress);
                        }

                    }).Start();

                    AlertUser("Successfully submitted request for approval. Your changes will be effected once approved.", AlertType.Success);
                }
                else
                {
                    AlertUser(stopCheckMessage, AlertType.Error);
                    return RedirectToAction("EditExistingRole", new { q = model.UrlQueryString });
                }

            }
            catch (Exception ex)
            {
                LogUtility.LogError(CallerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return RedirectToAction("ViewRoles");
        }

        [HttpGet]
        [Route("view-pending-created-roles")]
        public ActionResult ViewPendingCreatedRoles()
        {
            const string callerFormMethod = "HttpGet|ViewPendingCreatedRoles";

            try
            {
                var checkerEmails = "";
                if (!AuthorizeRequest(Request.Url.AbsolutePath, ref checkerEmails, CallerFormName, callerFormMethod, callerIpAddress))
                {
                    AlertUser("Unauthorised Access! You may not have been permitted to access the resource. Kindly login to re-authenticate. If issue persist, contact administrator.", AlertType.Error);
                    return RedirectToAction("Logout", "Account");
                }

                var sol = userData.branch_user_flag ? FinacleServices.GetFinacleSol2(userData.username, CallerFormName, callerFormMethod, callerIpAddress) : null;
                var logs = userData.branch_user_flag ? MakerCheckerLogService.GetWithMakerCheckerCategoryAndSolIdAndStatus((int)MakerCheckerCategory.Role, sol?.SolId, (int)MakerCheckerStatus.Initiated, CallerFormName, callerFormMethod, callerIpAddress) : MakerCheckerLogService.GetWithMakerCheckerCategoryAndStatus((int)MakerCheckerCategory.Role, (int)MakerCheckerStatus.Initiated, CallerFormName, callerFormMethod, callerIpAddress);
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
        [Route("review-pending-created-role")]
        public ActionResult ReviewPendingCreatedRole(string q)
        {
            const string callerFormMethod = "HttpGet|ReviewPendingCreatedRole";

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
        [Route("approve-pending-created-role")]
        public ActionResult ApprovePendingCreatedRole(MakerCheckerLogModel model)
        {
            const string callerFormMethod = "HttpPost|ApprovePendingCreatedRole";

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
                    stopCheckMessage = "Unable to retrieve pending created role at the moment, kindly try again later";
                }
                
                long applyResult = 0;
                if (!stopCheckFlag)
                {
                    var applyObject = JsonConvert.DeserializeObject<role>(log.action_data);
                    if (log.maker_checker_type_id.Equals((int)MakerCheckerType.CreateNewRole))
                    {
                        var role = RoleService.GetWithRoleName(applyObject.role_name, CallerFormName, callerFormMethod, callerIpAddress);
                        if (role != null)
                        {
                            stopCheckFlag = true;
                            stopCheckMessage = "Unable to approve pending created role at the moment; role with same name already exists";

                        }

                        if (!stopCheckFlag)
                        {
                            applyObject.approved_on = DateTime.UtcNow.AddHours(1);
                            applyObject.approved_by = $"{userData.person?.surname}, {userData.person?.first_name} {userData.person?.middle_name}";
                            applyResult = RoleService.Insert(applyObject, CallerFormName, callerFormMethod, callerIpAddress);
                        }


                    }
                    else if (log.maker_checker_type_id.Equals((int)MakerCheckerType.EditExistingRole))
                    {
                        var role = RoleService.GetWithUrlQueryString(Convert.ToString(applyObject.query_string), CallerFormName, callerFormMethod, callerIpAddress);
                        if (role == null)
                        {
                            stopCheckFlag = true;
                            stopCheckMessage = "Unable to retrieve existing role at the moment, kindly try again later";
                        }

                        if (!stopCheckFlag)
                        {
                            if (!applyObject.role_name.ToLower().Equals(role.role_name.ToLower()))
                            {
                                role = RoleService.GetWithRoleName(applyObject.role_name, CallerFormName, callerFormMethod, callerIpAddress);
                                if (role != null)
                                {
                                    stopCheckFlag = true;
                                    stopCheckMessage = "Unable to intiate request; rolename already exists. Kindly try again later!";
                                }
                            }
                        }

                        if (!stopCheckFlag)
                        {
                            applyObject.last_modified_on = DateTime.UtcNow.AddHours(1);
                            applyObject.last_modified_by = $"{userData.person?.surname}, {userData.person?.first_name} {userData.person?.middle_name}";
                            applyResult = RoleService.Update(applyObject, CallerFormName, callerFormMethod, callerIpAddress);
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
                        stopCheckMessage = "Unable to approve pending created/edited role at the moment. Kindly try again later!";
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
                        stopCheckMessage = "Unable to approve pending created role at the moment; maker-checker log could not be updated. Try again later!";
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

                    AlertUser("Successfully approved request. Your changes have been effected.", AlertType.Success);
                }
                else
                {
                    AlertUser(stopCheckMessage, AlertType.Error);
                    return RedirectToAction("ReviewPendingCreatedRole", new { q = model.QueryString });
                }

            }
            catch (Exception ex)
            {
                LogUtility.LogError(CallerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return RedirectToAction("ViewPendingCreatedRoles");
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("reject-pending-created-role")]
        public ActionResult RejectPendingCreatedRole(MakerCheckerLogModel model)
        {
            const string callerFormMethod = "HttpPost|RejectPendingCreatedRole";

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
                    stopCheckMessage = "Unable to retrieve pending created role at the moment, kindly try again later";
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
                        stopCheckMessage = "Unable to reject pending created role at the moment; maker-checker log could not be updated. Try again later!";
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
                    return RedirectToAction("ReviewPendingCreatedRole", new { q = model.QueryString });
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(CallerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return RedirectToAction("ViewPendingCreatedRoles");
        }

        #endregion

    }
}