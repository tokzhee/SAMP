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
using System.Threading.Tasks;

namespace App.Web.Controllers
{
    [Authorize]
    [RoutePrefix("reports")]
    public class ReportsController : BaseController
    {
        private const string CallerFormName = "ReportsController";
        private readonly string callerIpAddress;
        private readonly string callerMacAddress;

        private readonly ViewUserAccessActivityViewModel viewUserAccessActivityViewModel;
        private readonly ViewAppActivityViewModel viewAppActivityViewModel;
        private readonly ViewSettlementReportViewModel viewSettlementReportViewModel;
        private readonly ViewTransactionReportViewModel viewTransactionReportViewModel;
        private readonly ViewAppUsersViewModel viewAppUsersViewModel;
        private readonly ViewEmployerProfiledSalaryAccountsReportViewModel viewEmployerProfiledSalaryAccountsReportViewModel;
        private readonly user userData;

        private bool stopCheckFlag = false;
        private string stopCheckMessage = "";

        public ReportsController()
        {
            callerIpAddress = IpAddressManager.GetClientComputerIpAddress();
            callerMacAddress = MacAddressManager.GetClientComputerMacAddress();
            viewUserAccessActivityViewModel = new ViewUserAccessActivityViewModel();
            viewAppActivityViewModel = new ViewAppActivityViewModel();
            viewSettlementReportViewModel = new ViewSettlementReportViewModel(CallerFormName, "Ctor|ReportsController", callerIpAddress);
            viewTransactionReportViewModel = new ViewTransactionReportViewModel(CallerFormName, "Ctor|ReportsController", callerIpAddress);
            viewAppUsersViewModel = new ViewAppUsersViewModel();
            viewEmployerProfiledSalaryAccountsReportViewModel = new ViewEmployerProfiledSalaryAccountsReportViewModel(CallerFormName, "Ctor|ReportsController", callerIpAddress);
            userData = GetUser(CallerFormName, "Ctor|ReportsController", callerIpAddress);
        }

        #region ActionResult

        [HttpGet]
        [Route("view-user-access-logs")]
        public ActionResult ViewUserAccessLogs()
        {
            const string callerFormMethod = "HttpPost|ViewUserAccessLogs";

            string from = DateTime.UtcNow.AddHours(1).Date.ToString();
            string to = DateTime.UtcNow.AddHours(1).Date.ToString();

            try
            {
                var checkerEmails = "";
                if (!AuthorizeRequest(Request.Url.AbsolutePath, ref checkerEmails, CallerFormName, callerFormMethod, callerIpAddress))
                {
                    AlertUser("Unauthorised Access! You may not have been permitted to access the resource. Kindly login to re-authenticate. If issue persist, contact administrator.", AlertType.Error);
                    return RedirectToAction("Logout", "Account");
                }

                var userAccessLogList = UserAccessActivityService.GetAllWithStartDateAndEndDate(from, to, CallerFormName, callerFormMethod, callerIpAddress);
                if (userAccessLogList.Count > 0)
                {
                    ViewBag.ReportAction = (int)ReportAction.SpoolUserAccessList;
                    ViewBag.DateFrom = DateTime.UtcNow.AddHours(1).Date;
                    ViewBag.DateTo = DateTime.UtcNow.AddHours(1).Date;

                    AlertUser($"By default you are viewing data for {Convert.ToDateTime(from):dd-MMM-yyyy}. You may wish to export data", AlertType.Success);
                }
                else
                {
                    AlertUser("No records found for today!", AlertType.Information);
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(CallerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return View(viewUserAccessActivityViewModel);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("view-user-access-logs")]
        public ActionResult ViewUserAccessLogs(ReportSearchDateModel model)
        {
            const string callerFormMethod = "HttpPost|ViewUserAccessLogs";

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
                
                if (!stopCheckFlag)
                {
                    var userAccessLogList = UserAccessActivityService.GetAllWithStartDateAndEndDate(model.From, model.To, CallerFormName, callerFormMethod, callerIpAddress);
                    if (userAccessLogList.Count > 0)
                    {
                        ViewBag.ReportAction = (int)ReportAction.SpoolUserAccessList;
                        ViewBag.DateFrom = model.From;
                        ViewBag.DateTo = model.To;

                        AlertUser($"You have successfully selected to view data between {Convert.ToDateTime(model.From):dd-MMM-yyyy} and {Convert.ToDateTime(model.To):dd-MMM-yyyy}. You may wish to export data", AlertType.Success);
                    }
                    else
                    {
                        AlertUser("No records found!", AlertType.Information);
                    }
                }
                else
                {
                    AlertUser(stopCheckMessage, AlertType.Error);
                    return RedirectToAction("ViewUserAccessLogs");
                }

            }
            catch (Exception ex)
            {
                LogUtility.LogError(CallerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return View(viewUserAccessActivityViewModel);
        }

        [HttpGet]
        [Route("view-application-audit-logs")]
        public ActionResult ViewApplicationAuditLogs()
        {
            const string callerFormMethod = "HttpPost|ViewApplicationAuditLogs";

            string from = DateTime.UtcNow.AddHours(1).Date.ToString();
            string to = DateTime.UtcNow.AddHours(1).Date.ToString();

            try
            {
                var checkerEmails = "";
                if (!AuthorizeRequest(Request.Url.AbsolutePath, ref checkerEmails, CallerFormName, callerFormMethod, callerIpAddress))
                {
                    AlertUser("Unauthorised Access! You may not have been permitted to access the resource. Kindly login to re-authenticate. If issue persist, contact administrator.", AlertType.Error);
                    return RedirectToAction("Logout", "Account");
                }

                var applicationAuditLogList = AppActivityService.GetAllWithStartDateAndEndDate(from, to, CallerFormName, callerFormMethod, callerIpAddress);
                if (applicationAuditLogList.Count > 0)
                {
                    ViewBag.ReportAction = (int)ReportAction.SpoolApplicationAuditList;
                    ViewBag.DateFrom = DateTime.UtcNow.AddHours(1).Date;
                    ViewBag.DateTo = DateTime.UtcNow.AddHours(1).Date;

                    AlertUser($"By default you are viewing data for {Convert.ToDateTime(from):dd-MMM-yyyy}. You may wish to export data", AlertType.Success);
                }
                else
                {
                    AlertUser("No records found for today!", AlertType.Information);
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(CallerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return View(viewAppActivityViewModel);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("view-application-audit-logs")]
        public ActionResult ViewApplicationAuditLogs(ReportSearchDateModel model)
        {
            const string callerFormMethod = "HttpPost|ViewApplicationAuditLogs";

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

                if (!stopCheckFlag)
                {
                    var applicationAuditLogList = AppActivityService.GetAllWithStartDateAndEndDate(model.From, model.To, CallerFormName, callerFormMethod, callerIpAddress);
                    if (applicationAuditLogList.Count > 0)
                    {

                        ViewBag.ReportAction = (int)ReportAction.SpoolApplicationAuditList;
                        ViewBag.DateFrom = model.From;
                        ViewBag.DateTo = model.To;

                        AlertUser($"You have successfully selected to view data between {Convert.ToDateTime(model.From):dd-MMM-yyyy} and {Convert.ToDateTime(model.To):dd-MMM-yyyy}. You may wish to export data", AlertType.Success);
                    }
                    else
                    {
                        AlertUser("No records found!", AlertType.Information);
                    }
                }
                else
                {
                    AlertUser(stopCheckMessage, AlertType.Error);
                    return RedirectToAction("ViewApplicationAuditLogs");
                }
                
            }
            catch (Exception ex)
            {
                LogUtility.LogError(CallerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return View(viewAppActivityViewModel);
        }

        [HttpGet]
        [Route("view-app-users-report")]
        public ActionResult ViewAppUsers()
        {
            const string callerFormMethod = "HttpPost|ViewAppUsers";

            string from = DateTime.UtcNow.AddHours(1).Date.ToString();
            string to = DateTime.UtcNow.AddHours(1).Date.ToString();

            try
            {
                var checkerEmails = "";
                if (!AuthorizeRequest(Request.Url.AbsolutePath, ref checkerEmails, CallerFormName, callerFormMethod, callerIpAddress))
                {
                    AlertUser("Unauthorised Access! You may not have been permitted to access the resource. Kindly login to re-authenticate. If issue persist, contact administrator.", AlertType.Error);
                    return RedirectToAction("Logout", "Account");
                }

                var userReports = UsersService.GetAllCreatedBetweenStartDateAndEndDate(from, to, CallerFormName, callerFormMethod, callerIpAddress);
                if (userReports.Count > 0)
                {
                    ViewBag.ReportAction = (int)ReportAction.SpoolAppUsersReport;
                    ViewBag.DateFrom = DateTime.UtcNow.AddHours(1).Date;
                    ViewBag.DateTo = DateTime.UtcNow.AddHours(1).Date;

                    AlertUser($"By default you are viewing data for {Convert.ToDateTime(from):dd-MMM-yyyy}. You may wish to export data", AlertType.Success);
                }
                else
                {
                    AlertUser("No records found for today!", AlertType.Information);
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(CallerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return View(viewAppUsersViewModel);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("view-app-users-report")]
        public ActionResult ViewAppUsers(ReportSearchDateModel model)
        {
            const string callerFormMethod = "HttpPost|ViewAppUsers";

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

                if (!stopCheckFlag)
                {
                    var userReports = UsersService.GetAllCreatedBetweenStartDateAndEndDate(model.From, model.To, CallerFormName, callerFormMethod, callerIpAddress);
                    if (userReports.Count > 0)
                    {
                        ViewBag.ReportAction = (int)ReportAction.SpoolAppUsersReport;
                        ViewBag.DateFrom = model.From;
                        ViewBag.DateTo = model.To;

                        AlertUser($"You have successfully selected to view data between {Convert.ToDateTime(model.From):dd-MMM-yyyy} and {Convert.ToDateTime(model.To):dd-MMM-yyyy}. You may wish to export data", AlertType.Success);
                    }
                    else
                    {
                        AlertUser("No records found!", AlertType.Information);
                    }
                }
                else
                {
                    AlertUser(stopCheckMessage, AlertType.Error);
                    return RedirectToAction("ViewAppUsers");
                }

            }
            catch (Exception ex)
            {
                LogUtility.LogError(CallerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return View(viewAppUsersViewModel);
        }

        [HttpGet]
        [Route("view-settlement-report")]
        public ActionResult ViewSettlementReport()
        {
            if (userData.person.person_type_id.Equals((int)PersonType.FintechContactPerson))
            {
                viewSettlementReportViewModel.ShowFintechDropdown = false;
                viewSettlementReportViewModel.ShowSolSelection = false;
            }
            else if (userData.person.person_type_id.Equals((int)PersonType.BankRelationshipManager))
            {
                viewSettlementReportViewModel.ShowFintechDropdown = false;
                viewSettlementReportViewModel.ShowSolSelection = true;
            }
            else
            {
                viewSettlementReportViewModel.ShowFintechDropdown = true;
                viewSettlementReportViewModel.ShowSolSelection = true;
            }

            return View(viewSettlementReportViewModel);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("view-settlement-report")]
        public ActionResult ViewSettlementReport(string fintechFinacleTermId, ReportSearchDateModel model)
        {
            const string callerFormMethod = "HttpPost|ViewSettlementReport";

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

                if (!stopCheckFlag)
                {
                    if (userData.person.person_type_id.Equals((int)PersonType.BankUser) && string.IsNullOrEmpty(fintechFinacleTermId))
                    {
                        stopCheckFlag = true;
                        stopCheckMessage = "Fintech Term ID field is required";
                    }
                }

                if (!stopCheckFlag)
                {
                    var settlementReports = new List<SettlementReport>();
                    if (userData.person.person_type_id.Equals((int)PersonType.FintechContactPerson))
                    {
                        var fintech = (from fcp in FintechContactPersonService.GetAll(CallerFormName, callerFormMethod, callerIpAddress)
                                       join f in FintechService.GetAll(CallerFormName, callerFormMethod, callerIpAddress) on fcp.fintech_id equals f.id
                                       where fcp.person_id.Equals(userData.person_id)
                                       select f).FirstOrDefault();
                        if (fintech != null && !string.IsNullOrEmpty(fintech.finacle_term_id))
                        {
                            fintechFinacleTermId = fintech.finacle_term_id;

                            var htdReports = FinacleServices.GetHistoricSettlementReport(model.From, model.To, fintech.finacle_term_id, CallerFormName, callerFormMethod, callerIpAddress);
                            if (htdReports != null && htdReports.Count > 0)
                            {
                                settlementReports.AddRange(htdReports);
                            }

                            var dtdReports = FinacleServices.GetTodaysSettlementReport(model.From, model.To, fintech.finacle_term_id, CallerFormName, callerFormMethod, callerIpAddress);
                            if (dtdReports != null && dtdReports.Count > 0)
                            {
                                settlementReports.AddRange(dtdReports);
                            }
                        }
                    }
                    else if (userData.person.person_type_id.Equals((int)PersonType.BankRelationshipManager))
                    {
                        var fintech = FintechService.GetWithRelationshipManagerPersonId(Convert.ToString(userData.person_id), CallerFormName, callerFormMethod, callerIpAddress);
                        if (fintech != null && !string.IsNullOrEmpty(fintech.finacle_term_id))
                        {
                            fintechFinacleTermId = fintech.finacle_term_id;

                            var htdReports = FinacleServices.GetHistoricSettlementReport(model.From, model.To, fintech.finacle_term_id, CallerFormName, callerFormMethod, callerIpAddress);
                            if (htdReports != null && htdReports.Count > 0)
                            {
                                settlementReports.AddRange(htdReports);
                            }

                            var dtdReports = FinacleServices.GetTodaysSettlementReport(model.From, model.To, fintech.finacle_term_id, CallerFormName, callerFormMethod, callerIpAddress);
                            if (dtdReports != null && dtdReports.Count > 0)
                            {
                                settlementReports.AddRange(dtdReports);
                            }

                        }
                    }
                    else
                    {
                        var htdReports = FinacleServices.GetHistoricSettlementReport(model.From, model.To, fintechFinacleTermId, CallerFormName, callerFormMethod, callerIpAddress);
                        if (htdReports != null && htdReports.Count > 0)
                        {
                            settlementReports.AddRange(htdReports);
                        }

                        var dtdReports = FinacleServices.GetTodaysSettlementReport(model.From, model.To, fintechFinacleTermId, CallerFormName, callerFormMethod, callerIpAddress);
                        if (dtdReports != null && dtdReports.Count > 0)
                        {
                            settlementReports.AddRange(dtdReports);
                        }
                    }

                    if (settlementReports.Count > 0)
                    {
                        TempData["ReportAction"] = (int)ReportAction.SpoolSettlementReport;
                        TempData["FintechFinacleTermId"] = fintechFinacleTermId;
                        TempData["DateFrom"] = model.From;
                        TempData["DateTo"] = model.To;

                        AlertUser($"You have successfully selected to view data between {Convert.ToDateTime(model.From):dd-MMM-yyyy} and {Convert.ToDateTime(model.To):dd-MMM-yyyy}. You may wish to export data", AlertType.Success);
                    }
                    else
                    {
                        AlertUser("No records found!", AlertType.Information);
                    }
                }
                else
                {
                    AlertUser(stopCheckMessage, AlertType.Error);
                    return RedirectToAction("ViewSettlementReport");
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(CallerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return RedirectToAction("ViewSettlementReport");
        }

        [HttpGet]
        [Route("view-transaction-report")]
        public ActionResult ViewTransactionReport()
        {
            if (userData.person.person_type_id.Equals((int)PersonType.FintechContactPerson))
            {
                viewTransactionReportViewModel.ShowFintechDropdown = false;
                viewTransactionReportViewModel.ShowSolSelection = false;
            }
            else if (userData.person.person_type_id.Equals((int)PersonType.BankRelationshipManager))
            {
                viewTransactionReportViewModel.ShowFintechDropdown = false;
                viewTransactionReportViewModel.ShowSolSelection = true;
            }
            else
            {
                viewTransactionReportViewModel.ShowFintechDropdown = true;
                viewTransactionReportViewModel.ShowSolSelection = true;
            }

            return View(viewTransactionReportViewModel);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("view-transaction-report")]
        public ActionResult ViewTransactionReport(string fintechFinacleTermId, ReportSearchDateModel model)
        {
            const string callerFormMethod = "HttpPost|ViewTransactionReport";

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

                if (!stopCheckFlag)
                {
                    if (userData.person.person_type_id.Equals((int)PersonType.BankUser) && string.IsNullOrEmpty(fintechFinacleTermId))
                    {
                        stopCheckFlag = true;
                        stopCheckMessage = "Fintech Term ID field is required";
                    }
                }

                if (!stopCheckFlag)
                {
                    var transactionReports = new List<TransactionReport>();
                    if (userData.person.person_type_id.Equals((int)PersonType.FintechContactPerson))
                    {
                        var fintech = (from fcp in FintechContactPersonService.GetAll(CallerFormName, callerFormMethod, callerIpAddress)
                                       join f in FintechService.GetAll(CallerFormName, callerFormMethod, callerIpAddress) on fcp.fintech_id equals f.id
                                       where fcp.person_id.Equals(userData.person_id)
                                       select f).FirstOrDefault();
                        if (fintech != null && !string.IsNullOrEmpty(fintech.finacle_term_id))
                        {
                            var txnReports = TransactionReportService.GetTransactionReport(model.From, model.To, fintech.finacle_term_id, CallerFormName, callerFormMethod, callerIpAddress);
                            if (txnReports != null && txnReports.Count > 0)
                            {
                                transactionReports.AddRange(txnReports);
                            }
                        }
                    }
                    else if (userData.person.person_type_id.Equals((int)PersonType.BankRelationshipManager))
                    {
                        var fintech = FintechService.GetWithRelationshipManagerPersonId(Convert.ToString(userData.person_id), CallerFormName, callerFormMethod, callerIpAddress);
                        if (fintech != null && !string.IsNullOrEmpty(fintech.finacle_term_id))
                        {
                            var txnReports = TransactionReportService.GetTransactionReport(model.From, model.To, fintech.finacle_term_id, CallerFormName, callerFormMethod, callerIpAddress);
                            if (txnReports != null && txnReports.Count > 0)
                            {
                                transactionReports.AddRange(txnReports);
                            }
                        }
                    }
                    else
                    {
                        var txnReports = TransactionReportService.GetTransactionReport(model.From, model.To, fintechFinacleTermId, CallerFormName, callerFormMethod, callerIpAddress);
                        if (txnReports != null && txnReports.Count > 0)
                        {
                            transactionReports.AddRange(txnReports);
                        }
                    }

                    if (transactionReports.Count > 0)
                    {
                        TempData["ReportAction"] = (int)ReportAction.SpoolTransactionReport;
                        TempData["FintechFinacleTermId"] = fintechFinacleTermId;
                        TempData["DateFrom"] = model.From;
                        TempData["DateTo"] = model.To;

                        AlertUser($"You have successfully selected to view data between {Convert.ToDateTime(model.From):dd-MMM-yyyy} and {Convert.ToDateTime(model.To):dd-MMM-yyyy}. You may wish to export data", AlertType.Success);
                    }
                    else
                    {
                        AlertUser("No records found!", AlertType.Information);
                    }
                }
                else
                {
                    AlertUser(stopCheckMessage, AlertType.Error);
                    return RedirectToAction("ViewTransactionReport");
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(CallerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return RedirectToAction("ViewTransactionReport");
        }

        [HttpGet]
        [Route("view-employer-profiled-salary-accounts-report")]
        public ActionResult ViewEmployerProfileSalaryAccountsReport()
        {
            const string callerFormMethod = "HttpPost|ViewEmployerProfileSalaryAccountsReport";

            try
            {
                var checkerEmails = "";
                if (!AuthorizeRequest(Request.Url.AbsolutePath, ref checkerEmails, CallerFormName, callerFormMethod, callerIpAddress))
                {
                    AlertUser("Unauthorised Access! You may not have been permitted to access the resource. Kindly login to re-authenticate. If issue persist, contact administrator.", AlertType.Error);
                    return RedirectToAction("Logout", "Account");
                }

                var employerProfileSalaryAccountsReports = SalaryAccountsService.GetAllEmployerProfiled(CallerFormName, callerFormMethod, callerIpAddress);
                if (employerProfileSalaryAccountsReports.Count > 0)
                {
                    ViewBag.ReportAction = (int)ReportAction.SpoolEmployerProfiledSalaryAccountsReport;
                    ViewBag.EmployerProfiledSalaryAccountSearchCriteriaId = "0";
                    ViewBag.EmployerId = "";
                    ViewBag.AccountNumber = "";
                    ViewBag.DateFrom = "";
                    ViewBag.DateTo = "";

                    AlertUser($"By default you are viewing all employer-profiled salary accounts. You may wish to export data", AlertType.Success);
                }
                else
                {
                    AlertUser("No records found for today!", AlertType.Information);
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(CallerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return View(viewEmployerProfiledSalaryAccountsReportViewModel);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("view-employer-profiled-salary-accounts-report")]
        public ActionResult ViewEmployerProfileSalaryAccountsReport(string employerProfiledSalaryAccountSearchCriteriaId, string employerId, string accountNumber, string from, string to)
        {
            const string callerFormMethod = "HttpPost|ViewEmployerProfileSalaryAccountsReport";

            try
            {
                var checkerEmails = "";
                if (!AuthorizeRequest(Request.Url.AbsolutePath, ref checkerEmails, CallerFormName, callerFormMethod, callerIpAddress))
                {
                    AlertUser("Unauthorised Access! You may not have been permitted to access the resource. Kindly login to re-authenticate. If issue persist, contact administrator.", AlertType.Error);
                    return RedirectToAction("Logout", "Account");
                }

                if (employerProfiledSalaryAccountSearchCriteriaId == "1")
                {
                    if (!ValidationUtility.IsValidIntegerInput(employerId))
                    {
                        stopCheckFlag = true;
                        stopCheckMessage = "Employer Selection is required for the selected search criteria";
                    }
                }

                if (!stopCheckFlag)
                {
                    if (employerProfiledSalaryAccountSearchCriteriaId == "2")
                    {
                        if (!ValidationUtility.IsValidTextInput(accountNumber))
                        {
                            stopCheckFlag = true;
                            stopCheckMessage = "Account Number is required for the selected search criteria";
                        }
                    }
                }

                if (!stopCheckFlag)
                {
                    if (employerProfiledSalaryAccountSearchCriteriaId == "3")
                    {
                        if (!ValidationUtility.IsValidDateFormat(from, "dd-MM-yyyy"))
                        {
                            if (!ValidationUtility.IsValidDateFormat(from, "yyyy-MM-dd"))
                            {
                                stopCheckFlag = true;
                                stopCheckMessage = "Start Date is required for the selected search criteria";
                            }
                        }

                        if (!stopCheckFlag)
                        {
                            if (!ValidationUtility.IsValidDateFormat(to, "dd-MM-yyyy"))
                            {
                                if (!ValidationUtility.IsValidDateFormat(from, "yyyy-MM-dd"))
                                {
                                    stopCheckFlag = true;
                                    stopCheckMessage = "End Date is required for the selected search criteria";
                                }
                            }
                        }
                    }
                }

                if (!stopCheckFlag)
                {
                    List<EmployerProfileSalaryAccountsReport> employerProfileSalaryAccountsReports = null;

                    switch (employerProfiledSalaryAccountSearchCriteriaId)
                    {
                        case "0":
                            employerProfileSalaryAccountsReports = SalaryAccountsService.GetAllEmployerProfiled(CallerFormName, callerFormMethod, callerIpAddress);
                            break;
                        case "1":
                            employerProfileSalaryAccountsReports = SalaryAccountsService.GetAllEmployerProfiledWithEmployerId(employerId, CallerFormName, callerFormMethod, callerIpAddress);
                            break;
                        case "2":
                            employerProfileSalaryAccountsReports = SalaryAccountsService.GetAllEmployerProfiledWithAccountNumber(accountNumber, CallerFormName, callerFormMethod, callerIpAddress);
                            break;
                        case "3":
                            employerProfileSalaryAccountsReports = SalaryAccountsService.GetAllEmployerProfiledBetweenStartDateAndEndDate(from, to, CallerFormName, callerFormMethod, callerIpAddress);
                            break;
                        default:
                            break;
                    }

                    if (employerProfileSalaryAccountsReports != null && employerProfileSalaryAccountsReports.Count > 0)
                    {
                        ViewBag.ReportAction = (int)ReportAction.SpoolEmployerProfiledSalaryAccountsReport;

                        switch (employerProfiledSalaryAccountSearchCriteriaId)
                        {
                            case "0":
                                ViewBag.EmployerProfiledSalaryAccountSearchCriteriaId = "0";
                                ViewBag.EmployerId = "";
                                ViewBag.AccountNumber = "";
                                ViewBag.DateFrom = "";
                                ViewBag.DateTo = "";
                                AlertUser($"By default you are viewing all employer-profiled salary accounts. You may wish to export data", AlertType.Success);
                                break;
                            case "1":
                                ViewBag.EmployerProfiledSalaryAccountSearchCriteriaId = "1";
                                ViewBag.EmployerId = employerId;
                                ViewBag.AccountNumber = "";
                                ViewBag.DateFrom = "";
                                ViewBag.DateTo = "";
                                AlertUser($"You have successfully selected to view data for employer: {EmployerService.GetWithEmployerId(employerId, CallerFormName, callerFormMethod, callerIpAddress)?.employer_name}. You may wish to export data", AlertType.Success);
                                break;
                            case "2":
                                ViewBag.EmployerProfiledSalaryAccountSearchCriteriaId = "2";
                                ViewBag.EmployerId = "";
                                ViewBag.AccountNumber = accountNumber;
                                ViewBag.DateFrom = "";
                                ViewBag.DateTo = "";
                                AlertUser($"You have successfully selected to view data for account number: {accountNumber}. You may wish to export data", AlertType.Success);
                                break;
                            case "3":
                                ViewBag.EmployerProfiledSalaryAccountSearchCriteriaId = "3";
                                ViewBag.EmployerId = "";
                                ViewBag.AccountNumber = "";
                                ViewBag.DateFrom = from;
                                ViewBag.DateTo = to;
                                AlertUser($"You have successfully selected to view data between {Convert.ToDateTime(from):dd-MMM-yyyy} and {Convert.ToDateTime(to):dd-MMM-yyyy}. You may wish to export data", AlertType.Success);
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        AlertUser("No records found!", AlertType.Information);
                    }
                }
                else
                {
                    AlertUser(stopCheckMessage, AlertType.Error);
                    return RedirectToAction("ViewEmployerProfileSalaryAccountsReport");
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(CallerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return View(viewEmployerProfiledSalaryAccountsReportViewModel);
        }

        #region Old Codes
        //[HttpGet]
        //[Route("view-identified-salary-accounts-report")]
        //public ActionResult ViewIdentifiedSalaryAccountsReport(string q)
        //{
        //    const string callerFormMethod = "HttpGet|ViewIdentifiedSalaryAccountsReport";

        //    try
        //    {
        //        var checkerEmails = "";
        //        if (!AuthorizeRequest(Request.Url.AbsolutePath, ref checkerEmails, CallerFormName, callerFormMethod, callerIpAddress))
        //        {
        //            AlertUser("Unauthorised Access! You may not have been permitted to access the resource. Kindly login to re-authenticate. If issue persist, contact administrator.", AlertType.Error);
        //            return RedirectToAction("Logout", "Account");
        //        }

        //        long identifiedSalaryAccountCount = 0;

        //        switch (q)
        //        {
        //            case "BVN":
        //                identifiedSalaryAccountCount = FinacleServices.GetAllCountFromSalProfiling($"SELECT count(*) as TOTAL_COUNT from {ConfigurationUtility.GetAppSettingValue("MISSchemaName")}.mis_sal_profiling where IS_VALID_BVN IS NOT NULL", CallerFormName, callerFormMethod, callerIpAddress);
        //                break;
        //            case "CRMS":
        //                identifiedSalaryAccountCount = FinacleServices.GetAllCountFromSalProfiling($"SELECT count(*) as TOTAL_COUNT from {ConfigurationUtility.GetAppSettingValue("MISSchemaName")}.mis_sal_profiling where IS_VALID_CRMS IS NOT NULL", CallerFormName, callerFormMethod, callerIpAddress);
        //                break;
        //            case "CRC":
        //                identifiedSalaryAccountCount = FinacleServices.GetAllCountFromSalProfiling($"SELECT count(*) as TOTAL_COUNT from {ConfigurationUtility.GetAppSettingValue("MISSchemaName")}.mis_sal_profiling where IS_VALID_CRC IS NOT NULL", CallerFormName, callerFormMethod, callerIpAddress);
        //                break;
        //            case "STM":
        //                identifiedSalaryAccountCount = FinacleServices.GetAllCountFromSalProfiling($"SELECT count(*) as TOTAL_COUNT from {ConfigurationUtility.GetAppSettingValue("MISSchemaName")}.mis_sal_profiling where SRC = 'STM'", CallerFormName, callerFormMethod, callerIpAddress);
        //                break;
        //            case "NRM":
        //                identifiedSalaryAccountCount = FinacleServices.GetAllCountFromSalProfiling($"SELECT count(*) as TOTAL_COUNT from {ConfigurationUtility.GetAppSettingValue("MISSchemaName")}.mis_sal_profiling where SRC = 'NRM'", CallerFormName, callerFormMethod, callerIpAddress);
        //                break;
        //            case "SAMP":
        //                identifiedSalaryAccountCount = FinacleServices.GetAllCountFromSalProfiling($"SELECT count(*) as TOTAL_COUNT from {ConfigurationUtility.GetAppSettingValue("MISSchemaName")}.mis_sal_profiling where SRC = 'SAMP'", CallerFormName, callerFormMethod, callerIpAddress);
        //                break;
        //            default:
        //                identifiedSalaryAccountCount = FinacleServices.GetAllCountFromSalProfiling(CallerFormName, callerFormMethod, callerIpAddress);
        //                break;
        //        }

        //        if (identifiedSalaryAccountCount > 0)
        //        {
        //            ViewBag.ReportAction = (int)ReportAction.SpoolIdentifiedSalaryAccountsReport;

        //            switch (q)
        //            {
        //                case "BVN":
        //                    ViewBag.ViewType = "BVN";
        //                    AlertUser($"By default you are viewing all identified salary accounts whose BVN have been checked. You may wish to export data", AlertType.Success);
        //                    break;
        //                case "CRMS":
        //                    ViewBag.ViewType = "CRMS";
        //                    AlertUser($"By default you are viewing all identified salary accounts whose CRMS have been checked. You may wish to export data", AlertType.Success);
        //                    break;
        //                case "CRC":
        //                    ViewBag.ViewType = "CRC";
        //                    AlertUser($"By default you are viewing all identified salary accounts whose CRC have been checked. You may wish to export data", AlertType.Success);
        //                    break;
        //                case "STM":
        //                    ViewBag.ViewType = "STM";
        //                    AlertUser($"By default you are viewing all identified salary accounts with algorithm. You may wish to export data", AlertType.Success);
        //                    break;
        //                case "NRM":
        //                    ViewBag.ViewType = "NRM";
        //                    AlertUser($"By default you are viewing all identified salary accounts with narration. You may wish to export data", AlertType.Success);
        //                    break;
        //                case "SAMP":
        //                    ViewBag.ViewType = "SAMP";
        //                    AlertUser($"By default you are viewing all identified salary accounts from the branches via RMs. You may wish to export data", AlertType.Success);
        //                    break;
        //                default:
        //                    ViewBag.ViewType = "ALL";
        //                    AlertUser($"By default you are viewing all identified salary accounts. You may wish to export data", AlertType.Success);
        //                    break;
        //            }
        //        }
        //        else
        //        {
        //            AlertUser("No records found!", AlertType.Information);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LogUtility.LogError(CallerFormName, callerFormMethod, callerIpAddress, ex);
        //    }

        //    return View();
        //}
        #endregion

        [HttpGet]
        [Route("view-identified-salary-accounts-report")]
        public ActionResult ViewIdentifiedSalaryAccountsReport(string q)
        {
            const string callerFormMethod = "HttpGet|ViewIdentifiedSalaryAccountsReport";

            try
            {
                var checkerEmails = "";
                if (!AuthorizeRequest(Request.Url.AbsolutePath, ref checkerEmails, CallerFormName, callerFormMethod, callerIpAddress))
                {
                    AlertUser("Unauthorised Access! You may not have been permitted to access the resource. Kindly login to re-authenticate. If issue persist, contact administrator.", AlertType.Error);
                    return RedirectToAction("Logout", "Account");
                }

                switch (q)
                {
                    case "BVN":
                        AlertUser($"By default you are viewing all identified salary accounts whose BVN have been checked. You may wish to export data", AlertType.Success);
                        break;
                    case "CRMS":
                        AlertUser($"By default you are viewing all identified salary accounts whose CRMS have been checked. You may wish to export data", AlertType.Success);
                        break;
                    case "CRC":
                        AlertUser($"By default you are viewing all identified salary accounts whose CRC have been checked. You may wish to export data", AlertType.Success);
                        break;
                    case "STM":
                        AlertUser($"By default you are viewing all identified salary accounts with algorithm. You may wish to export data", AlertType.Success);
                        break;
                    case "NRM":
                        AlertUser($"By default you are viewing all identified salary accounts with narration. You may wish to export data", AlertType.Success);
                        break;
                    case "SAMP":
                        AlertUser($"By default you are viewing all identified salary accounts from the branches via RMs. You may wish to export data", AlertType.Success);
                        break;
                    case "OTH":
                        AlertUser($"By default you are viewing all other identified salary accounts. You may wish to export data", AlertType.Success);
                        break;
                    default:
                        AlertUser($"By default you are viewing all identified salary accounts. You may wish to export data", AlertType.Success);
                        break;
                }

                ViewBag.q = string.IsNullOrEmpty(q) ? "ALL" : q;
            }
            catch (Exception ex)
            {
                LogUtility.LogError(CallerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return View();

            #region Old Codes
            //const string callerFormMethod = "HttpGet|ViewIdentifiedSalaryAccountsReport";

            //List<SalProfiling> salProfilings = null;

            //try
            //{
            //    var checkerEmails = "";
            //    if (!AuthorizeRequest(Request.Url.AbsolutePath, ref checkerEmails, CallerFormName, callerFormMethod, callerIpAddress))
            //    {
            //        AlertUser("Unauthorised Access! You may not have been permitted to access the resource. Kindly login to re-authenticate. If issue persist, contact administrator.", AlertType.Error);
            //        return RedirectToAction("Logout", "Account");
            //    }

            //    switch (q)
            //    {
            //        case "BVN":
            //            salProfilings = FinacleServices.GetAllFromSalProfiling($"SELECT [COLUMNS] from {ConfigurationUtility.GetAppSettingValue("MISSchemaName")}.mis_sal_profiling where IS_VALID_BVN IS NOT NULL order by INSERTED_DATE desc", CallerFormName, "ShowIdentifiedSalaryAccountsReport", callerIpAddress);
            //            break;
            //        case "CRMS":
            //            salProfilings = FinacleServices.GetAllFromSalProfiling($"SELECT [COLUMNS] from {ConfigurationUtility.GetAppSettingValue("MISSchemaName")}.mis_sal_profiling where IS_VALID_CRMS IS NOT NULL order by INSERTED_DATE desc", CallerFormName, "ShowIdentifiedSalaryAccountsReport", callerIpAddress);
            //            break;
            //        case "CRC":
            //            salProfilings = FinacleServices.GetAllFromSalProfiling($"SELECT [COLUMNS] from {ConfigurationUtility.GetAppSettingValue("MISSchemaName")}.mis_sal_profiling where IS_VALID_CRC IS NOT NULL order by INSERTED_DATE desc", CallerFormName, "ShowIdentifiedSalaryAccountsReport", callerIpAddress);
            //            break;
            //        case "STM":
            //            salProfilings = FinacleServices.GetAllFromSalProfiling($"SELECT [COLUMNS] from {ConfigurationUtility.GetAppSettingValue("MISSchemaName")}.mis_sal_profiling where SRC='STM' order by INSERTED_DATE desc", CallerFormName, "ShowIdentifiedSalaryAccountsReport", callerIpAddress);
            //            break;
            //        case "NRM":
            //            salProfilings = FinacleServices.GetAllFromSalProfiling($"SELECT [COLUMNS] from {ConfigurationUtility.GetAppSettingValue("MISSchemaName")}.mis_sal_profiling where SRC='NRM' order by INSERTED_DATE desc", CallerFormName, "ShowIdentifiedSalaryAccountsReport", callerIpAddress);
            //            break;
            //        case "SAMP":
            //            salProfilings = FinacleServices.GetAllFromSalProfiling($"SELECT [COLUMNS] from {ConfigurationUtility.GetAppSettingValue("MISSchemaName")}.mis_sal_profiling where SRC='SAMP' order by INSERTED_DATE desc", CallerFormName, "ShowIdentifiedSalaryAccountsReport", callerIpAddress);
            //            break;
            //        case "OTH":
            //            salProfilings = FinacleServices.GetAllFromSalProfiling($"SELECT [COLUMNS] from {ConfigurationUtility.GetAppSettingValue("MISSchemaName")}.mis_sal_profiling where SRC='OTH' order by INSERTED_DATE desc", CallerFormName, "ShowIdentifiedSalaryAccountsReport", callerIpAddress);
            //            break;
            //        default:
            //            salProfilings = FinacleServices.GetAllFromSalProfiling(CallerFormName, "ShowIdentifiedSalaryAccountsReport", callerIpAddress);
            //            break;
            //    }

            //    if (salProfilings != null && salProfilings.Count > 0)
            //    {
            //        switch (q)
            //        {
            //            case "BVN":
            //                AlertUser($"By default you are viewing all identified salary accounts whose BVN have been checked. You may wish to export data", AlertType.Success);
            //                break;
            //            case "CRMS":
            //                AlertUser($"By default you are viewing all identified salary accounts whose CRMS have been checked. You may wish to export data", AlertType.Success);
            //                break;
            //            case "CRC":
            //                AlertUser($"By default you are viewing all identified salary accounts whose CRC have been checked. You may wish to export data", AlertType.Success);
            //                break;
            //            case "STM":
            //                AlertUser($"By default you are viewing all identified salary accounts with algorithm. You may wish to export data", AlertType.Success);
            //                break;
            //            case "NRM":
            //                AlertUser($"By default you are viewing all identified salary accounts with narration. You may wish to export data", AlertType.Success);
            //                break;
            //            case "SAMP":
            //                AlertUser($"By default you are viewing all identified salary accounts from the branches via RMs. You may wish to export data", AlertType.Success);
            //                break;
            //            case "OTH":
            //                AlertUser($"By default you are viewing all other identified salary accounts. You may wish to export data", AlertType.Success);
            //                break;
            //            default:
            //                AlertUser($"By default you are viewing all identified salary accounts. You may wish to export data", AlertType.Success);
            //                break;
            //        }
            //    }
            //    else
            //    {
            //        AlertUser("No records found!", AlertType.Information);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    LogUtility.LogError(CallerFormName, callerFormMethod, callerIpAddress, ex);
            //}

            //return View(salProfilings);
            #endregion
        }

        [HttpPost]
        [Route("load-identified-salary-accounts")]
        public ActionResult LoadIdentifiedSalaryAccounts(string q)
        {
            const string callerFormMethod = "HttpGet|LoadIdentifiedSalaryAccounts";

            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            //var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            //var sortColumnDirection = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();


            //Paging Size (10,20,50,100)    
            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;

            List<SalProfiling> salProfilings = null;

            switch (q)
            {
                case "BVN":
                    salProfilings = FinacleServices.GetAllFromSalProfiling($"SELECT [COLUMNS] from {ConfigurationUtility.GetAppSettingValue("MISSchemaName")}.mis_sal_profiling where IS_VALID_BVN IS NOT NULL order by INSERTED_DATE desc", CallerFormName, callerFormMethod, callerIpAddress);
                    break;
                case "CRMS":
                    salProfilings = FinacleServices.GetAllFromSalProfiling($"SELECT [COLUMNS] from {ConfigurationUtility.GetAppSettingValue("MISSchemaName")}.mis_sal_profiling where IS_VALID_CRMS IS NOT NULL order by INSERTED_DATE desc", CallerFormName, callerFormMethod, callerIpAddress);
                    break;
                case "CRC":
                    salProfilings = FinacleServices.GetAllFromSalProfiling($"SELECT [COLUMNS] from {ConfigurationUtility.GetAppSettingValue("MISSchemaName")}.mis_sal_profiling where IS_VALID_CRC IS NOT NULL order by INSERTED_DATE desc", CallerFormName, callerFormMethod, callerIpAddress);
                    break;
                case "STM":
                    salProfilings = FinacleServices.GetAllFromSalProfiling($"SELECT [COLUMNS] from {ConfigurationUtility.GetAppSettingValue("MISSchemaName")}.mis_sal_profiling where SRC='STM' order by INSERTED_DATE desc", CallerFormName, callerFormMethod, callerIpAddress);
                    break;
                case "NRM":
                    salProfilings = FinacleServices.GetAllFromSalProfiling($"SELECT [COLUMNS] from {ConfigurationUtility.GetAppSettingValue("MISSchemaName")}.mis_sal_profiling where SRC='NRM' order by INSERTED_DATE desc", CallerFormName, callerFormMethod, callerIpAddress);
                    break;
                case "SAMP":
                    salProfilings = FinacleServices.GetAllFromSalProfiling($"SELECT [COLUMNS] from {ConfigurationUtility.GetAppSettingValue("MISSchemaName")}.mis_sal_profiling where SRC='SAMP' order by INSERTED_DATE desc", CallerFormName, callerFormMethod, callerIpAddress);
                    break;
                case "OTH":
                    salProfilings = FinacleServices.GetAllFromSalProfiling($"SELECT [COLUMNS] from {ConfigurationUtility.GetAppSettingValue("MISSchemaName")}.mis_sal_profiling where SRC='OTH' order by INSERTED_DATE desc", CallerFormName, callerFormMethod, callerIpAddress);
                    break;
                default:
                    salProfilings = FinacleServices.GetAllFromSalProfiling(CallerFormName, callerFormMethod, callerIpAddress);
                    break;
            }

            //Sorting    
            //if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
            //{
            //    salProfilings = salProfilings.OrderBy(sortColumn + " " + sortColumnDirection);
            //}

            //Search    
            if (!string.IsNullOrEmpty(searchValue))
            {
                salProfilings = salProfilings?.Where(salProfiling => salProfiling.Foracid == searchValue || salProfiling.Src == searchValue).ToList();
            }

            //total number of rows count     
            recordsTotal = salProfilings != null ? salProfilings.Count() : 0;

            //Paging     
            var data = salProfilings?.Skip(skip).Take(pageSize).ToList();

            //Returning Json Data    
            return Json(new
            {
                draw = draw,
                recordsFiltered = recordsTotal,
                recordsTotal = recordsTotal,
                data = data
            });
        }

        #endregion
    }
}