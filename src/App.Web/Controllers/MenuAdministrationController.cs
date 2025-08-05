using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;
using App.Core.BusinessLogic;
using App.Core.Utilities;
using App.Core.Services;
using App.Web.Models;
using App.Web.ViewModels;
using App.DataModels.Models;

namespace App.Web.Controllers
{
    [Authorize]
    [RoutePrefix("menu-administration")]
    public class MenuAdministrationController : BaseController
    {
        private const string CallerFormName = "MenuAdministrationController";
        private readonly string callerIpAddress;
        private readonly string callerMacAddress;
        private ViewRoleAccessViewModel viewRoleAccessViewModel;
        private readonly ReviewMakerCheckerLogViewModel reviewMakerCheckerLogViewModel;
        private List<MakerCheckerLogModel> makerCheckerLogModels;
        private readonly user userData;

        private bool stopCheckFlag = false;
        private string stopCheckMessage = "";

        public MenuAdministrationController()
        {
            callerIpAddress = IpAddressManager.GetClientComputerIpAddress();
            callerMacAddress = MacAddressManager.GetClientComputerMacAddress();
            reviewMakerCheckerLogViewModel = new ReviewMakerCheckerLogViewModel();
            makerCheckerLogModels = new List<MakerCheckerLogModel>();
            userData = GetUser(CallerFormName, "Ctor|MenuAdministrationController", callerIpAddress);
        }

        #region ActionResult

        [HttpGet]
        [Route("view-role-access")]
        public ActionResult ViewRoleAccess()
        {
            const string callerFormMethod = "HttpGet|ViewRoleAccess";

            try
            {
                viewRoleAccessViewModel = new ViewRoleAccessViewModel(CallerFormName, callerFormMethod, callerIpAddress);
            }
            catch (Exception ex)
            {
                LogUtility.LogError(CallerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return View(viewRoleAccessViewModel);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("view-role-access")]
        public ActionResult ViewRoleAccess(ViewRoleAccessViewModel model)
        {
            const string callerFormMethod = "HttpPost|ViewRoleAccess";

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
                    var errorMessages = string.Join(", ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
                    stopCheckFlag = true;
                    stopCheckMessage = errorMessages;
                }

                if (!stopCheckFlag)
                {
                    viewRoleAccessViewModel = new ViewRoleAccessViewModel(CallerFormName, callerFormMethod, callerIpAddress)
                    {
                        AssignRoleAccessViewModel = new AssignRoleAccessViewModel(model.RoleId, CallerFormName, callerFormMethod, callerIpAddress)
                    };
                }
                else
                {
                    AlertUser(stopCheckMessage, AlertType.Error);
                    return RedirectToAction("ViewRoleAccess");
                }

            }
            catch (Exception ex)
            {
                LogUtility.LogError(CallerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return View(viewRoleAccessViewModel);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("assign-role-access")]
        public ActionResult AssignRoleAccess(AssignRoleAccessViewModel model)
        {
            const string callerFormMethod = "HttpPost|AssignRoleAccess";

            try
            {
                var checkerEmails = "";
                if (!AuthorizeRequest(Request.Url.AbsolutePath, ref checkerEmails, CallerFormName, callerFormMethod, callerIpAddress))
                {
                    AlertUser("Unauthorised Access! You may not have been permitted to access the resource. Kindly login to re-authenticate. If issue persist, contact administrator.", AlertType.Error);
                    return RedirectToAction("Logout", "Account");
                }

                var makerCheckerAction = "Assign Role Access";
                var makerCheckerActionDetails = "";
                foreach (var item in model.SelectedSubMenus)
                {
                    if (item.IsSelected)
                    {
                        makerCheckerActionDetails += $"Assign Access '{item.SelectedAccessName}' to Role '{RoleService.GetWithRoleId(model.RoleId, CallerFormName, callerFormMethod, callerIpAddress)?.role_name}'|";
                    }
                    else
                    {
                        if (MenuService.IsMenuAssigned(model.RoleId, Convert.ToString(item.SelectedSubMenuId), CallerFormName, callerFormMethod, callerIpAddress))
                        {
                            makerCheckerActionDetails += $"Unassign Access '{item.SelectedAccessName}' from Role '{RoleService.GetWithRoleId(model.RoleId, CallerFormName, callerFormMethod, callerIpAddress)?.role_name}'|";
                        }
                    }
                }

                makerCheckerActionDetails = makerCheckerActionDetails.Substring(0, makerCheckerActionDetails.Length - 1);


                if (string.IsNullOrEmpty(makerCheckerActionDetails))
                {
                    stopCheckFlag = true;
                    stopCheckMessage = "Unable to intiate request; no access has been selected!";
                }

                if (!stopCheckFlag)
                {
                    var permissionList = new List<SelectedSubMenu>();
                    foreach (var item in model.SelectedSubMenus)
                    {
                        var permission = new SelectedSubMenu
                        {
                            IsSelected = item.IsSelected,
                            SelectedSubMenuId = item.SelectedSubMenuId,
                            SelectedAccessName = item.SelectedAccessName,
                            SelectedRoleId = model.RoleId
                        };

                        permissionList.Add(permission);
                    }

                    var makerCheckerActionData = JsonConvert.SerializeObject(permissionList);
                    var sol = userData.branch_user_flag ? FinacleServices.GetFinacleSol2(userData.username, CallerFormName, callerFormMethod, callerIpAddress) : null;
                    var makercheckerlog = new makercheckerlog
                    {
                        maker_checker_category_id = (int)MakerCheckerCategory.RoleAccess,
                        maker_checker_type_id = (int)MakerCheckerType.AssignRoleAccess,
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

                    var makerCheckerLogInsertResult = MakerCheckerLogService.Insert(makercheckerlog, CallerFormName, callerFormMethod, callerIpAddress);
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
                            var mailBody = System.IO.File.ReadAllText(templatePath).Replace("{ItemType}", "Menu Assignment");
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

            return RedirectToAction("ViewRoleAccess");
        }

        [HttpGet]
        [Route("view-pending-assigned-role-access")]
        public ActionResult ViewPendingAssignedRoleAccess()
        {
            const string callerFormMethod = "HttpGet|ViewPendingAssignedRoleAccess";

            try
            {
                var checkerEmails = "";
                if (!AuthorizeRequest(Request.Url.AbsolutePath, ref checkerEmails, CallerFormName, callerFormMethod, callerIpAddress))
                {
                    AlertUser("Unauthorised Access! You may not have been permitted to access the resource. Kindly login to re-authenticate. If issue persist, contact administrator.", AlertType.Error);
                    return RedirectToAction("Logout", "Account");
                }

                var sol = userData.branch_user_flag ? FinacleServices.GetFinacleSol2(userData.username, CallerFormName, callerFormMethod, callerIpAddress) : null;
                var logs = userData.branch_user_flag ? MakerCheckerLogService.GetWithMakerCheckerCategoryAndSolIdAndStatus((int)MakerCheckerCategory.RoleAccess, sol?.SolId, (int)MakerCheckerStatus.Initiated, CallerFormName, callerFormMethod, callerIpAddress) : MakerCheckerLogService.GetWithMakerCheckerCategoryAndStatus((int)MakerCheckerCategory.RoleAccess, (int)MakerCheckerStatus.Initiated, CallerFormName, callerFormMethod, callerIpAddress);
                if (logs.Count > 0)
                {
                    makerCheckerLogModels = logs.Select(log => new MakerCheckerLogModel
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

            return View(makerCheckerLogModels);
        }

        [HttpGet]
        [Route("review-pending-assigned-role-access")]
        public ActionResult ReviewPendingAssignedRoleAccess(string q)
        {
            const string callerFormMethod = "HttpGet|ReviewPendingAssignedRoleAccess";

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
        [Route("approve-pending-assigned-role-access")]
        public ActionResult ApprovePendingAssignedRoleAccess(MakerCheckerLogModel model)
        {
            const string callerFormMethod = "HttpPost|ApprovePendingAssignedRoleAccess";

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
                    stopCheckMessage = "Unable to retrieve pending assigned role access at the moment, kindly try again later";
                }


                if (!stopCheckFlag)
                {
                    var applyObject = JsonConvert.DeserializeObject<List<SelectedSubMenu>>(log.action_data);
                    foreach (var item in applyObject)
                    {
                        MenuService.AssignMenu(userData.username, item.SelectedRoleId, Convert.ToString(item.SelectedSubMenuId), item.IsSelected, CallerFormName, callerFormMethod, callerIpAddress, callerMacAddress);
                    }

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
                        stopCheckMessage = "Unable to approve pending assigned role access at the moment; maker-checker log could not be updated. Try again later!";
                    }
                }


                if (!stopCheckFlag)
                {
                    CacheData.Rolemenus = MenuService.GetRoleMenus(CallerFormName, callerFormMethod, callerIpAddress);
                    Session["RoleMenu"] = CacheData.Rolemenus.Where(c => c.role_id.Equals(userData.role_id)).ToList();

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
                    return RedirectToAction("ReviewPendingAssignedRoleAccess", new { q = model.QueryString });
                }

            }
            catch (Exception ex)
            {
                LogUtility.LogError(CallerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return RedirectToAction("ViewPendingAssignedRoleAccess");
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("reject-pending-assigned-role-access")]
        public ActionResult RejectPendingAssignedRoleAccess(MakerCheckerLogModel model)
        {
            const string callerFormMethod = "HttpPost|RejectPendingAssignedRoleAccess";

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
                    stopCheckMessage = "Unable to retrieve pending assigned role access at the moment, kindly try again later";
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
                        stopCheckMessage = "Unable to reject pending assigned role access at the moment; maker-checker log could not be updated. Try again later!";
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
                    return RedirectToAction("ReviewPendingAssignedRoleAccess", new { q = model.QueryString });
                }

            }
            catch (Exception ex)
            {
                LogUtility.LogError(CallerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return RedirectToAction("ViewPendingAssignedRoleAccess");
        }

        #endregion
    }
}