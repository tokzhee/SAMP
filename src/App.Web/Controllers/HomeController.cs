using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using App.Core.BusinessLogic;
using App.Core.Utilities;
using App.Web.Models;
using App.Web.ViewModels;
using App.DataModels.Models;
using App.Core.Services;

namespace App.Web.Controllers
{
    [Authorize]
    [RoutePrefix("home")]
    public class HomeController : BaseController
    {
        private const string CallerFormName = "HomeController";
        private readonly string callerIpAddress;
        private readonly string callerMacAddress;

        private readonly BankUserDashboardViewModel bankUserDashboardViewModel;
        private readonly FintechContactPersonDashboardViewModel fintechContactPersonDashboardViewModel;
        private readonly RelationshipManagerDashboardViewModel relationshipManagerDashboardViewModel;
        private readonly ProfileViewModel profileViewModel;
        private readonly user userData;
        

        public HomeController()
        {
            callerIpAddress = IpAddressManager.GetClientComputerIpAddress();
            callerMacAddress = MacAddressManager.GetClientComputerMacAddress();

            bankUserDashboardViewModel = new BankUserDashboardViewModel();
            fintechContactPersonDashboardViewModel = new FintechContactPersonDashboardViewModel();
            relationshipManagerDashboardViewModel = new RelationshipManagerDashboardViewModel();
            profileViewModel = new ProfileViewModel();
            userData = GetUser(CallerFormName, "Ctor|HomeController", callerIpAddress);
        }

        #region ActionResult

        private ChartDataModel BuildUserStatusLoginChart(string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            ChartDataModel chartDataModel = null;

            try
            {
                var statusList = UserAccountStatusService.GetAll(CallerFormName, callerFormMethod, callerIpAddress);
                var userStatusChart = UsersService.GetUserStatusChartData(CallerFormName, callerFormMethod, callerIpAddress);
                if (userStatusChart.Count > 0)
                {
                    if (statusList != null)
                    {
                        foreach (var status in statusList)
                        {
                            if (!userStatusChart.Any(c => c.ChartKey.Equals($"\"{status.account_status}\"")))
                            {
                                userStatusChart.Add(new Chart { ChartKey = $"\"{status.account_status}\"", ChartValue = 0 });
                            }
                        }
                    }
                }
                else
                {
                    if (statusList != null)
                    {
                        userStatusChart = new List<Chart>();
                        foreach (var status in statusList)
                        {
                            userStatusChart.Add(new Chart { ChartKey = $"\"{status.account_status}\"", ChartValue = 0 });
                        }
                    }
                }
                
                if (userStatusChart.Count > 0)
                {
                    chartDataModel = new ChartDataModel
                    {
                        ChartKeyString = string.Join(",", userStatusChart.Where(c => !string.IsNullOrEmpty(c.ChartKey)).Select(c => c.ChartKey).ToList()),
                        ChartValueString = string.Join(",", userStatusChart.Where(c => !string.IsNullOrEmpty(Convert.ToString(c.ChartValue))).Select(c => c.ChartValue).ToList())
                    };
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return chartDataModel;
        }
        private ChartDataModel BuildMyLoginChart(string username, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            ChartDataModel chartDataModel = null;

            try
            {
                var myLoginChart = UserAccessActivityService.GetMyLoginChartDataWithUsername(username, CallerFormName, callerFormMethod, callerIpAddress);
                if (myLoginChart.Count > 0)
                {
                    if (!myLoginChart.Any(c => c.ChartKey.Equals("\"Failed Login\"")))
                    {
                        myLoginChart.Add(new Chart { ChartKey = "\"Failed Login\"", ChartValue = 0 });
                    }

                    if (!myLoginChart.Any(c => c.ChartKey.Equals("\"Successful Login\"")))
                    {
                        myLoginChart.Add(new Chart { ChartKey = "\"Successful Login\"", ChartValue = 0 });
                    }
                }
                else
                {
                    myLoginChart = new List<Chart>
                    {
                        new Chart { ChartKey = "\"Failed Login\"", ChartValue = 0 },
                        new Chart { ChartKey = "\"Successful Login\"", ChartValue = 0 }
                    };
                }

                if (myLoginChart.Count > 0)
                {
                    chartDataModel = new ChartDataModel
                    {
                        ChartKeyString = string.Join(",", myLoginChart.Where(c => !string.IsNullOrEmpty(c.ChartKey)).Select(c => c.ChartKey).ToList()),
                        ChartValueString = string.Join(",", myLoginChart.Where(c => !string.IsNullOrEmpty(Convert.ToString(c.ChartValue))).Select(c => c.ChartValue).ToList())
                    };
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return chartDataModel;
        }
        private ChartDataModel BuildOverallLoginChart(string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            ChartDataModel chartDataModel = null;

            try
            {
                var overallLoginChart = UserAccessActivityService.GetOverallLoginChartData(CallerFormName, callerFormMethod, callerIpAddress);
                if (overallLoginChart.Count > 0)
                {
                    if (!overallLoginChart.Any(c => c.ChartKey.Equals("\"Failed Login\"")))
                    {
                        overallLoginChart.Add(new Chart { ChartKey = "\"Failed Login\"", ChartValue = 0 });
                    }

                    if (!overallLoginChart.Any(c => c.ChartKey.Equals("\"Successful Login\"")))
                    {
                        overallLoginChart.Add(new Chart { ChartKey = "\"Successful Login\"", ChartValue = 0 });
                    }
                }
                else
                {
                    overallLoginChart = new List<Chart>
                    {
                        new Chart { ChartKey = "\"Failed Login\"", ChartValue = 0 },
                        new Chart { ChartKey = "\"Successful Login\"", ChartValue = 0 }
                    };
                }

                if (overallLoginChart.Count > 0)
                {
                    chartDataModel = new ChartDataModel
                    {
                        ChartKeyString = string.Join(",", overallLoginChart.Where(c => !string.IsNullOrEmpty(c.ChartKey)).Select(c => c.ChartKey).ToList()),
                        ChartValueString = string.Join(",", overallLoginChart.Where(c => !string.IsNullOrEmpty(Convert.ToString(c.ChartValue))).Select(c => c.ChartValue).ToList())
                    };
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return chartDataModel;
        }

        [Route("dashboard")]
        public ActionResult Dashboard()
        {
            const string callerFormMethod = "HttpGet|Dashboard";

            var redirectAction = "BankUserDashboard";

            try
            {
                var checkerEmails = "";
                if (!AuthorizeRequest(Request.Url.AbsolutePath, ref checkerEmails, CallerFormName, callerFormMethod, callerIpAddress))
                {
                    AlertUser("Unauthorised Access! You may not have been permitted to access the resource. Kindly login to re-authenticate. If issue persist, contact administrator.", AlertType.Error);
                    return RedirectToAction("Logout", "Account");
                }

                if (userData.authentication_type_id.Equals((int)UserAccountAuthenticationType.LocalAccountAuthentication))
                {
                    if (string.IsNullOrEmpty(Convert.ToString(userData.password_expiry_date)))
                    {
                        return RedirectToAction("ChangePassword", "Account", new { q = userData.query_string });
                    }
                    else
                    {
                        if (DateTime.UtcNow.AddHours(1) > userData.password_expiry_date)
                        {
                            return RedirectToAction("ChangePassword", "Account", new { q = userData.query_string });
                        }
                    }
                }
                
                if (userData.person.person_type_id.Equals((int)PersonType.FintechContactPerson))
                {
                    redirectAction = "FintechContactPersonDashboard";
                }
                else if (userData.person.person_type_id.Equals((int)PersonType.BankRelationshipManager))
                {
                    redirectAction = "RelationshipManagerDashboard";
                }
                else
                {
                    //
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(CallerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return RedirectToAction(redirectAction);
        }

        [Route("bank-user-dashboard")]
        public ActionResult BankUserDashboard()
        {
            const string callerFormMethod = "HttpGet|BankUserDashboard";

            try
            {
                if (!userData.person.person_type_id.Equals((int)PersonType.BankUser))
                {
                    return RedirectToAction("Dashboard");
                }

                long identifiedSalaryAccountCount = FinacleServices.GetAllCountFromSalProfiling(CallerFormName, callerFormMethod, callerIpAddress);
                long identifiedSalaryAccountWithSTMCount = 0;
                long identifiedSalaryAccountWithNRMCount = 0;
                long identifiedSalaryAccountWithSAMPCount = 0;
                long identifiedSalaryAccountWithOTHCount = 0;

                long bvnCheckedCount = 0;
                decimal bvnCheckedPercentage = 0;
                long cRMSCheckedCount = 0;
                decimal cRMSCheckedPercentage = 0;
                long cRCCheckedCount = 0;
                decimal cRCCheckedPercentage = 0;

                if (identifiedSalaryAccountCount > 0)
                {
                    var baseQuery = $"SELECT count(*) as TOTAL_COUNT from {ConfigurationUtility.GetAppSettingValue("MISSchemaName")}.mis_sal_profiling";
                    identifiedSalaryAccountWithSTMCount = FinacleServices.GetAllCountFromSalProfiling($"{baseQuery} where SRC = 'STM'", CallerFormName, callerFormMethod, callerIpAddress);
                    identifiedSalaryAccountWithNRMCount = FinacleServices.GetAllCountFromSalProfiling($"{baseQuery} where SRC = 'NRM'", CallerFormName, callerFormMethod, callerIpAddress);
                    identifiedSalaryAccountWithSAMPCount = FinacleServices.GetAllCountFromSalProfiling($"{baseQuery} where SRC = 'SAMP'", CallerFormName, callerFormMethod, callerIpAddress);
                    identifiedSalaryAccountWithOTHCount = FinacleServices.GetAllCountFromSalProfiling($"{baseQuery} where SRC = 'OTH'", CallerFormName, callerFormMethod, callerIpAddress);

                    bvnCheckedCount = FinacleServices.GetAllCountFromSalProfiling($"SELECT count(*) as TOTAL_COUNT from {ConfigurationUtility.GetAppSettingValue("MISSchemaName")}.mis_sal_profiling where IS_VALID_BVN IS NOT NULL", CallerFormName, callerFormMethod, callerIpAddress);
                    bvnCheckedPercentage = bvnCheckedCount / identifiedSalaryAccountCount * 100;

                    cRMSCheckedCount = FinacleServices.GetAllCountFromSalProfiling($"SELECT count(*) as TOTAL_COUNT from {ConfigurationUtility.GetAppSettingValue("MISSchemaName")}.mis_sal_profiling where IS_VALID_CRMS IS NOT NULL", CallerFormName, callerFormMethod, callerIpAddress);
                    cRMSCheckedPercentage = cRMSCheckedCount / identifiedSalaryAccountCount * 100;

                    cRCCheckedCount = FinacleServices.GetAllCountFromSalProfiling($"SELECT count(*) as TOTAL_COUNT from {ConfigurationUtility.GetAppSettingValue("MISSchemaName")}.mis_sal_profiling where IS_VALID_CRC IS NOT NULL", CallerFormName, callerFormMethod, callerIpAddress);
                    cRCCheckedPercentage = cRCCheckedCount / identifiedSalaryAccountCount * 100;
                }

                bankUserDashboardViewModel.IdentifiedSalaryAccountCount = identifiedSalaryAccountCount.ToString("N0");
                bankUserDashboardViewModel.IdentifiedSalaryAccountWithSTMCount = identifiedSalaryAccountWithSTMCount.ToString("N0");
                bankUserDashboardViewModel.IdentifiedSalaryAccountWithNRMCount = identifiedSalaryAccountWithNRMCount.ToString("N0");
                bankUserDashboardViewModel.IdentifiedSalaryAccountWithSAMPCount = identifiedSalaryAccountWithSAMPCount.ToString("N0");
                bankUserDashboardViewModel.IdentifiedSalaryAccountWithOTHCount = identifiedSalaryAccountWithOTHCount.ToString("N0");

                bankUserDashboardViewModel.BvnCheckedCount = bvnCheckedCount.ToString("N0");
                bankUserDashboardViewModel.BvnCheckedPercentage = bvnCheckedPercentage.ToString("N");

                bankUserDashboardViewModel.CRMSCheckedCount = cRMSCheckedCount.ToString("N0");
                bankUserDashboardViewModel.CRMSCheckedPercentage = cRMSCheckedPercentage.ToString("N");

                bankUserDashboardViewModel.CRCCheckedCount = cRCCheckedCount.ToString("N0");
                bankUserDashboardViewModel.CRCCheckedPercentage = cRCCheckedPercentage.ToString("N");

                var salaryaccounts = SalaryAccountsService.GetAllEmployerProfiled(CallerFormName, callerFormMethod, callerIpAddress);
                bankUserDashboardViewModel.EmployerProfiledSalaryAccountCount = salaryaccounts != null ? salaryaccounts.Count.ToString("N0") : "0";
                

                bankUserDashboardViewModel.UserStatusChart = BuildUserStatusLoginChart(CallerFormName, callerFormMethod, callerIpAddress);
                bankUserDashboardViewModel.MyLoginChart = BuildMyLoginChart(userData.username, CallerFormName, callerFormMethod, callerIpAddress);
                bankUserDashboardViewModel.OverallLoginChart = BuildOverallLoginChart(CallerFormName, callerFormMethod, callerIpAddress);
                
                //
                //bankUserDashboardViewModel.SettlementReports = FinacleServices.GetTodaysSettlementReport("", CallerFormName, callerFormMethod, callerIpAddress);
            }
            catch (Exception ex)
            {
                LogUtility.LogError(CallerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return View(bankUserDashboardViewModel);
        }

        [Route("fintech-contact-person-dashboard")]
        public ActionResult FintechContactPersonDashboard()
        {
            const string callerFormMethod = "HttpGet|FintechContactPersonDashboard";

            try
            {
                if (!userData.person.person_type_id.Equals((int)PersonType.FintechContactPerson))
                {
                    return RedirectToAction("Dashboard");
                }

                fintechContactPersonDashboardViewModel.UserStatusChart = BuildUserStatusLoginChart(CallerFormName, callerFormMethod, callerIpAddress);
                fintechContactPersonDashboardViewModel.MyLoginChart = BuildMyLoginChart(userData.username, CallerFormName, callerFormMethod, callerIpAddress);
                fintechContactPersonDashboardViewModel.OverallLoginChart = BuildOverallLoginChart(CallerFormName, callerFormMethod, callerIpAddress);

                //
                var fintech = (from fcp in FintechContactPersonService.GetAll(CallerFormName, callerFormMethod, callerIpAddress)
                               join f in FintechService.GetAll(CallerFormName, callerFormMethod, callerIpAddress) on fcp.fintech_id equals f.id
                               where fcp.person_id.Equals(userData.person_id)
                               select f).FirstOrDefault();
                if (fintech != null && !string.IsNullOrEmpty(fintech.finacle_term_id))
                {
                    fintechContactPersonDashboardViewModel.SettlementReports = FinacleServices.GetTodaysSettlementReport(fintech.finacle_term_id.ToUpper(), CallerFormName, callerFormMethod, callerIpAddress);
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(CallerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return View(fintechContactPersonDashboardViewModel);
        }

        [Route("relationship-manager-dashboard")]
        public ActionResult RelationshipManagerDashboard()
        {
            const string callerFormMethod = "HttpGet|RelationshipManagerDashboard";

            try
            {
                if (!userData.person.person_type_id.Equals((int)PersonType.BankRelationshipManager))
                {
                    return RedirectToAction("Dashboard");
                }

                relationshipManagerDashboardViewModel.UserStatusChart = BuildUserStatusLoginChart(CallerFormName, callerFormMethod, callerIpAddress);
                relationshipManagerDashboardViewModel.MyLoginChart = BuildMyLoginChart(userData.username, CallerFormName, callerFormMethod, callerIpAddress);
                relationshipManagerDashboardViewModel.OverallLoginChart = BuildOverallLoginChart(CallerFormName, callerFormMethod, callerIpAddress);

                //
                var fintech = FintechService.GetWithRelationshipManagerPersonId(Convert.ToString(userData.person_id), CallerFormName, callerFormMethod, callerIpAddress);
                if (fintech != null && !string.IsNullOrEmpty(fintech.finacle_term_id))
                {
                    relationshipManagerDashboardViewModel.SettlementReports = FinacleServices.GetTodaysSettlementReport(fintech.finacle_term_id.ToUpper(), CallerFormName, callerFormMethod, callerIpAddress);
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(CallerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return View(relationshipManagerDashboardViewModel);
        }

        [Route("my-profile")]
        public ActionResult MyProfile()
        {
            const string callerFormMethod = "HttpGet|MyProfile";

            try
            {
                var checkerEmails = "";
                if (!AuthorizeRequest(Request.Url.AbsolutePath, ref checkerEmails, CallerFormName, callerFormMethod, callerIpAddress))
                {
                    AlertUser("Unauthorised Access! You may not have been permitted to access the resource. Kindly login to re-authenticate. If issue persist, contact administrator.", AlertType.Error);
                    return RedirectToAction("Logout", "Account");
                }

                profileViewModel.UserModel = new UserModel
                {
                    Username = userData.username,
                    AuthenticationTypeId = Convert.ToString(userData.authentication_type_id),
                    AuthenticationTypeName = CacheData.Useraccountauthenticationtypes.FirstOrDefault(c => c.id.Equals(userData.authentication_type_id))?.authentication_type_name,

                    LocalPassword = userData.local_password,
                    Surname = userData.person?.surname,
                    Firstname = userData.person?.first_name,
                    Middlename = userData.person?.middle_name,
                    MobileNumber = userData.person?.mobile_number,
                    EmailAddress = userData.person?.email_address,
                    Passport = userData.person?.passport,
                    IsVisiblePassportUploadLink = true,
                    IsVisibleChangePasswordLink = userData.authentication_type_id.Equals((int)UserAccountAuthenticationType.LocalAccountAuthentication),

                    RoleId = Convert.ToString(userData.role_id),
                    RoleName = RoleService.GetWithRoleId(Convert.ToString(userData.role_id), CallerFormName, callerFormMethod, callerIpAddress)?.role_name,
                    AccountStatusId = Convert.ToString(userData.status_id),
                    AccountStatusName = CacheData.Useraccountstatus.FirstOrDefault(status => status.id.Equals(userData.status_id))?.account_status,
                    LastLoginDate = !string.IsNullOrEmpty(Convert.ToString(userData.last_login_date)) ? Convert.ToDateTime(userData.last_login_date).ToString("dd-MM-yyyy hh:mm tt") : "n/p",
                    LastLogoutDate = !string.IsNullOrEmpty(Convert.ToString(userData.last_logout_date)) ? Convert.ToDateTime(userData.last_logout_date).ToString("dd-MM-yyyy hh:mm tt") : "n/p",
                    CreatedOn = Convert.ToDateTime(userData.created_on).ToString("dd-MM-yyyy hh:mm tt"),
                    CreatedBy = userData.created_by,
                    ApprovedOn = !string.IsNullOrEmpty(Convert.ToString(userData.approved_on)) ? Convert.ToDateTime(userData.approved_on).ToString("dd-MM-yyyy hh:mm tt") : "n/p",
                    ApprovedBy = userData.approved_by,
                    LastModifiedOn = !string.IsNullOrEmpty(Convert.ToString(userData.last_modified_on)) ? Convert.ToDateTime(userData.last_modified_on).ToString("dd-MM-yyyy hh:mm tt") : "n/p",
                    LastModifiedBy = userData.last_modified_by,
                    UrlQueryString = Convert.ToString(userData.query_string)
                };
            }
            catch (Exception ex)
            {
                LogUtility.LogError(CallerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return View(profileViewModel);
        }
        

        #endregion
    }
}