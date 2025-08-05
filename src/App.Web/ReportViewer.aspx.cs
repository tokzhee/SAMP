using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using App.Core.Utilities;
using App.Core.Services;
using Microsoft.Reporting.WebForms;
using App.DataModels.Models;
using App.Web.Controllers;
using App.Core.BusinessLogic;

namespace LMS_Portal.UI.HRLoans
{
    public partial class ReportViewer : Page
    {
        private const string CallerFormName = "ReportViewer.aspx";
        private readonly string callerIpAddress;
        private readonly user userData;
        public ReportViewer()
        {
            callerIpAddress = IpAddressManager.GetClientComputerIpAddress();
            userData = new BaseController().GetUser(CallerFormName, "Ctor|ReportViewer", callerIpAddress);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (Request.QueryString["Action"] != null)
                {
                    var action = Request.QueryString["Action"];
                    var dateFrom = "";
                    var dateTo = "";
                    var fintech = "";

                    switch (action)
                    {
                        case "1":
                            dateFrom = HttpUtility.UrlDecode(Request.QueryString["from"]);
                            dateTo = HttpUtility.UrlDecode(Request.QueryString["to"]);
                            ShowUserAccessLogReport(dateFrom, dateTo);
                            break;
                        case "2":
                            dateFrom = HttpUtility.UrlDecode(Request.QueryString["from"]);
                            dateTo = HttpUtility.UrlDecode(Request.QueryString["to"]);
                            ShowApplicationAuditLogReport(dateFrom, dateTo);
                            break;
                        case "3":
                            fintech = HttpUtility.UrlDecode(Request.QueryString["fintech"]);
                            dateFrom = HttpUtility.UrlDecode(Request.QueryString["from"]);
                            dateTo = HttpUtility.UrlDecode(Request.QueryString["to"]);
                            ShowSettlementReport(fintech, dateFrom, dateTo);
                            break;
                        case "4":
                            fintech = HttpUtility.UrlDecode(Request.QueryString["fintech"]);
                            dateFrom = HttpUtility.UrlDecode(Request.QueryString["from"]);
                            dateTo = HttpUtility.UrlDecode(Request.QueryString["to"]);
                            ShowTransactionReport(fintech, dateFrom, dateTo);
                            break;
                        case "5":
                            dateFrom = HttpUtility.UrlDecode(Request.QueryString["from"]);
                            dateTo = HttpUtility.UrlDecode(Request.QueryString["to"]);
                            ShowAppUsersReport(dateFrom, dateTo);
                            break;
                        case "6":
                            var employerProfiledSalaryAccountSearchCriteriaId = HttpUtility.UrlDecode(Request.QueryString["criteria"]);
                            var employer = HttpUtility.UrlDecode(Request.QueryString["employer"]);
                            var accountNumber = HttpUtility.UrlDecode(Request.QueryString["accountnumber"]);
                            dateFrom = HttpUtility.UrlDecode(Request.QueryString["from"]);
                            dateTo = HttpUtility.UrlDecode(Request.QueryString["to"]);
                            ShowEmployerProfiledSalaryAccountsReport(employerProfiledSalaryAccountSearchCriteriaId, employer, accountNumber, dateFrom, dateTo);
                            break;
                        case "7":
                            var viewType = HttpUtility.UrlDecode(Request.QueryString["viewtype"]);
                            ShowIdentifiedSalaryAccountsReport(viewType);
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        private void ShowUserAccessLogReport(string dateFrom, string dateTo)
        {
            try
            {
                rv.Reset();
                rv.LocalReport.DisplayName = "User Access Log Report";
                rv.LocalReport.ReportPath = Server.MapPath(ConfigurationUtility.GetAppSettingValue("UserAccessLogReportPath"));

                var dataTable = new DataTable();
                var userAccessLogList = UserAccessActivityService.GetAllWithStartDateAndEndDate(dateFrom, dateTo, CallerFormName, "ShowUserAccessLogReport", callerIpAddress);
                if (userAccessLogList.Count > 0)
                {
                    dataTable.Columns.Add("Username");
                    dataTable.Columns.Add("Fullname");
                    dataTable.Columns.Add("IpAddress");
                    dataTable.Columns.Add("MacAddress");
                    dataTable.Columns.Add("Comments");
                    dataTable.Columns.Add("LoginDate");
                    dataTable.Columns.Add("LogoutDate");
                    dataTable.Columns.Add("DateFrom");
                    dataTable.Columns.Add("DateTo");
                    dataTable.Rows.Clear();

                    foreach (var item in userAccessLogList)
                    {
                        if (!string.IsNullOrEmpty(Convert.ToString(item.logout_date)))
                        {
                            dataTable.Rows.Add(item.username, item.fullname, item.ipaddress, item.macaddress, item.remarks, Convert.ToDateTime(item.created_on).ToString("dd-MM-yyyy hh:mm tt"), Convert.ToDateTime(item.logout_date).ToString("dd-MM-yyyy hh:mm tt"), dateFrom, dateTo);
                        }
                        else
                        {
                            dataTable.Rows.Add(item.username, item.fullname, item.ipaddress, item.macaddress, item.remarks, Convert.ToDateTime(item.created_on).ToString("dd-MM-yyyy hh:mm tt"), "", dateFrom, dateTo);
                        }
                    }
                }

                rv.ProcessingMode = ProcessingMode.Local;
                rv.LocalReport.DataSources.Add(new ReportDataSource("dsUserAccessLog", dataTable));
                rv.LocalReport.Refresh();

            }
            catch (Exception ex)
            {
                LogUtility.LogError(CallerFormName, "ShowUserAccessLogReport", callerIpAddress, ex);
            }
        }
        private void ShowApplicationAuditLogReport(string dateFrom, string dateTo)
        {
            try
            {
                rv.Reset();
                rv.LocalReport.DisplayName = "Application Audit Log Report";
                rv.LocalReport.ReportPath = Server.MapPath(ConfigurationUtility.GetAppSettingValue("ApplicationAuditLogReportPath"));

                var dataTable = new DataTable();
                var applicationAuditLogList = AppActivityService.GetAllWithStartDateAndEndDate(dateFrom, dateTo, CallerFormName, "ShowApplicationAuditLogReport", callerIpAddress);
                if (applicationAuditLogList.Count > 0)
                {
                    dataTable.Columns.Add("Operation");
                    dataTable.Columns.Add("OperationDetails");
                    dataTable.Columns.Add("PerformedBy");
                    dataTable.Columns.Add("IpAddress");
                    dataTable.Columns.Add("MacAddress");
                    dataTable.Columns.Add("DatePerformed");
                    dataTable.Columns.Add("DateFrom");
                    dataTable.Columns.Add("DateTo");
                    dataTable.Columns.Add("Fullname");
                    dataTable.Rows.Clear();

                    foreach (var item in applicationAuditLogList)
                    {
                        dataTable.Rows.Add(item.operation, item.comments, item.audit_username, item.audit_ipaddress, item.audit_macaddress, Convert.ToDateTime(item.created_on).ToString("dd-MM-yyyy hh:mm tt"), dateFrom, dateTo, item.audit_fullname);
                    }
                }

                rv.ProcessingMode = ProcessingMode.Local;
                rv.LocalReport.DataSources.Add(new ReportDataSource("dsApplicationAuditLog", dataTable));
                rv.LocalReport.Refresh();

            }
            catch (Exception ex)
            {
                LogUtility.LogError(CallerFormName, "ShowApplicationAuditLogReport", callerIpAddress, ex);
            }
        }
        private void ShowAppUsersReport(string dateFrom, string dateTo)
        {
            try
            {
                rv.Reset();
                rv.LocalReport.DisplayName = "Application Users Report";
                rv.LocalReport.ReportPath = Server.MapPath(ConfigurationUtility.GetAppSettingValue("AppUsersReportPath"));

                var dataTable = new DataTable();
                var userReports = UsersService.GetAllCreatedBetweenStartDateAndEndDate(dateFrom, dateTo, CallerFormName, "ShowAppUsersReport", callerIpAddress);
                if (userReports.Count > 0)
                {
                    dataTable.Columns.Add("Username");
                    dataTable.Columns.Add("Fullname");
                    dataTable.Columns.Add("RoleName");
                    dataTable.Columns.Add("LastLoginDate");
                    dataTable.Columns.Add("CreatedOn");
                    dataTable.Columns.Add("CreatedBy");
                    dataTable.Columns.Add("ApprovedOn");
                    dataTable.Columns.Add("ApprovedBy");
                    dataTable.Columns.Add("AccountStatus");
                    dataTable.Columns.Add("DateFrom");
                    dataTable.Columns.Add("DateTo");
                    dataTable.Rows.Clear();

                    foreach (var item in userReports)
                    {
                        dataTable.Rows.Add(
                            item.Username,
                            item.Fullname,
                            item.RoleName,
                            !string.IsNullOrEmpty(Convert.ToString(item.LastLoginDate)) ? Convert.ToDateTime(item.LastLoginDate).ToString("dd-MM-yyyy hh:mm tt") : "",
                            Convert.ToDateTime(item.CreatedOn).ToString("dd-MM-yyyy hh:mm tt"),
                            item.CreatedBy,
                            !string.IsNullOrEmpty(Convert.ToString(item.ApprovedOn)) ? Convert.ToDateTime(item.ApprovedOn).ToString("dd-MM-yyyy hh:mm tt") : "",
                            item.ApprovedBy,
                            item.AccountStatus,
                            dateFrom,
                            dateTo);
                    }
                }

                rv.ProcessingMode = ProcessingMode.Local;
                rv.LocalReport.DataSources.Add(new ReportDataSource("dsAppUsersReport", dataTable));
                rv.LocalReport.Refresh();

            }
            catch (Exception ex)
            {
                LogUtility.LogError(CallerFormName, "ShowAppUsersReport", callerIpAddress, ex);
            }
        }
        private void ShowSettlementReport(string fintechFinacleTermId, string dateFrom, string dateTo)
        {
            try
            {
                var showReport = true;

                if (userData.person.person_type_id.Equals((int)PersonType.FintechContactPerson))
                {
                    var fintech = (from fcp in FintechContactPersonService.GetAll(CallerFormName, "ShowSettlementReport", callerIpAddress)
                                   join f in FintechService.GetAll(CallerFormName, "ShowSettlementReport", callerIpAddress) on fcp.fintech_id equals f.id
                                   where fcp.person_id.Equals(userData.person_id)
                                   select f).FirstOrDefault();
                    if (fintech.finacle_term_id != fintechFinacleTermId)
                    {
                        showReport = false;
                    }
                }
                if (userData.person.person_type_id.Equals((int)PersonType.BankRelationshipManager))
                {
                    var fintech = FintechService.GetWithRelationshipManagerPersonId(Convert.ToString(userData.person_id), CallerFormName, "ShowSettlementReport", callerIpAddress);
                    if (fintech.finacle_term_id != fintechFinacleTermId)
                    {
                        showReport = false;
                    }
                }

                if (showReport)
                {
                    rv.Reset();
                    rv.LocalReport.DisplayName = "Settlement Report";
                    rv.LocalReport.ReportPath = Server.MapPath(ConfigurationUtility.GetAppSettingValue("SettlementReportPath"));
                    if (userData.person.person_type_id.Equals((int)PersonType.FintechContactPerson))
                    {
                        rv.LocalReport.ReportPath = Server.MapPath(ConfigurationUtility.GetAppSettingValue("SettlementReportPathFintech"));
                    }

                    var dataTable = new DataTable();
                    var settlementReportList = new List<SettlementReport>();
                    var htdReports = FinacleServices.GetHistoricSettlementReport(dateFrom, dateTo, fintechFinacleTermId, CallerFormName, "ShowSettlementReport", callerIpAddress);
                    if (htdReports != null && htdReports.Count > 0)
                    {
                        settlementReportList.AddRange(htdReports);
                    }

                    var dtdReports = FinacleServices.GetTodaysSettlementReport(dateFrom, dateTo, fintechFinacleTermId, CallerFormName, "ShowSettlementReport", callerIpAddress);
                    if (dtdReports != null && dtdReports.Count > 0)
                    {
                        settlementReportList.AddRange(dtdReports);
                    }

                    if (settlementReportList.Count > 0)
                    {
                        dataTable.Columns.Add("InitSolId");
                        dataTable.Columns.Add("AccountNumber");
                        dataTable.Columns.Add("AccountName");
                        dataTable.Columns.Add("TranAmt");
                        dataTable.Columns.Add("TranDate");
                        dataTable.Columns.Add("TranId");
                        dataTable.Columns.Add("TransactionType");
                        dataTable.Columns.Add("Narration");
                        dataTable.Columns.Add("TranAmtSum");
                        dataTable.Columns.Add("FinacleTermId");
                        dataTable.Columns.Add("RefNum");
                        dataTable.Columns.Add("TranRmks");
                        dataTable.Columns.Add("TranParticular2");
                        dataTable.Columns.Add("ValueDate");

                        dataTable.Rows.Clear();

                        foreach (var item in settlementReportList)
                        {
                            dataTable.Rows.Add(item.InitSolId, item.AccountNumber, item.AccountName, Convert.ToDecimal(item.TranAmt).ToString("N"), Convert.ToDateTime(item.TranDate).ToString("dd-MM-yyyy"), item.TranId, item.TransactionType, item.Narration, settlementReportList.Sum(report => Convert.ToDecimal(report.TranAmt)).ToString("N"), fintechFinacleTermId.ToUpper(), item.RefNum, item.TranRmks, item.TranParticular2, Convert.ToDateTime(item.ValueDate).ToString("dd-MM-yyyy"));
                        }
                    }

                    rv.ProcessingMode = ProcessingMode.Local;
                    rv.LocalReport.DataSources.Add(new ReportDataSource("dsSettlementReport", dataTable));
                    rv.LocalReport.Refresh();
                }

            }
            catch (Exception ex)
            {
                LogUtility.LogError(CallerFormName, "ShowSettlementReport", callerIpAddress, ex);
            }
        }
        private void ShowTransactionReport(string fintechFinacleTermId, string dateFrom, string dateTo)
        {
            try
            {
                var showReport = true;

                if (userData.person.person_type_id.Equals((int)PersonType.FintechContactPerson))
                {
                    var fintech = (from fcp in FintechContactPersonService.GetAll(CallerFormName, "ShowTransactionReport", callerIpAddress)
                                   join f in FintechService.GetAll(CallerFormName, "ShowTransactionReport", callerIpAddress) on fcp.fintech_id equals f.id
                                   where fcp.person_id.Equals(userData.person_id)
                                   select f).FirstOrDefault();
                    if (fintech.finacle_term_id != fintechFinacleTermId)
                    {
                        showReport = false;
                    }
                }
                if (userData.person.person_type_id.Equals((int)PersonType.BankRelationshipManager))
                {
                    var fintech = FintechService.GetWithRelationshipManagerPersonId(Convert.ToString(userData.person_id), CallerFormName, "ShowTransactionReport", callerIpAddress);
                    if (fintech.finacle_term_id != fintechFinacleTermId)
                    {
                        showReport = false;
                    }
                }

                if (showReport)
                {
                    rv.Reset();
                    rv.LocalReport.DisplayName = "Transaction Report";
                    rv.LocalReport.ReportPath = Server.MapPath(ConfigurationUtility.GetAppSettingValue("TransactionReportPath"));

                    var dataTable = new DataTable();
                    var transactionReportList = new List<TransactionReport>();
                    var txnReports = TransactionReportService.GetTransactionReport(dateFrom, dateTo, fintechFinacleTermId, CallerFormName, "ShowTransactionReport", callerIpAddress);
                    if (txnReports != null && txnReports.Count > 0)
                    {
                        transactionReportList.AddRange(txnReports);
                    }

                    if (transactionReportList.Count > 0)
                    {
                        dataTable.Columns.Add("DateTimeReq");
                        dataTable.Columns.Add("TranNr");
                        dataTable.Columns.Add("MessageType");
                        dataTable.Columns.Add("Pan");
                        dataTable.Columns.Add("FromAccountId");
                        dataTable.Columns.Add("ToAccountId");
                        dataTable.Columns.Add("TranAmountReq");
                        dataTable.Columns.Add("TranAmountResp");
                        dataTable.Columns.Add("TerminalId");
                        dataTable.Columns.Add("RetrievalReferenceNr");
                        dataTable.Columns.Add("SystemTraceAuditNr");
                        dataTable.Columns.Add("RspCodeRsp");
                        dataTable.Columns.Add("SourceNodeName"); 
                        dataTable.Columns.Add("SinkNodeName");
                        dataTable.Columns.Add("FinacleTermId");

                        dataTable.Rows.Clear();

                        foreach (var item in transactionReportList)
                        {
                            dataTable.Rows.Add(item.datetime_req, item.tran_nr, item.message_type, item.pan, item.from_account_id, item.to_account_id, item.tran_amount_req, item.tran_amount_rsp, item.terminal_id, item.retrieval_reference_nr, item.system_trace_audit_nr, item.rsp_code_rsp, item.source_node_name, item.sink_node_name, fintechFinacleTermId.ToUpper());
                        }
                    }

                    rv.ProcessingMode = ProcessingMode.Local;
                    rv.LocalReport.DataSources.Add(new ReportDataSource("dsTransactionReport", dataTable));
                    rv.LocalReport.Refresh();
                }

            }
            catch (Exception ex)
            {
                LogUtility.LogError(CallerFormName, "ShowSettlementReport", callerIpAddress, ex);
            }
        }
        private void ShowEmployerProfiledSalaryAccountsReport(string criteriaId, string employerId, string accountNumber, string dateFrom, string dateTo)
        {
            try
            {
                rv.Reset();
                rv.LocalReport.DisplayName = "Employer-Profiled Salary Accounts Report";
                rv.LocalReport.ReportPath = Server.MapPath(ConfigurationUtility.GetAppSettingValue("EmployerProfiledSalaryAccountsReportPath"));

                List<EmployerProfileSalaryAccountsReport> employerProfileSalaryAccountsReports = null;
                switch (criteriaId)
                {
                    case "0":
                        employerProfileSalaryAccountsReports = SalaryAccountsService.GetAllEmployerProfiled(CallerFormName, "ShowEmployerProfiledSalaryAccountsReport", callerIpAddress);
                        break;
                    case "1":
                        employerProfileSalaryAccountsReports = SalaryAccountsService.GetAllEmployerProfiledWithEmployerId(employerId,CallerFormName, "ShowEmployerProfiledSalaryAccountsReport", callerIpAddress);
                        break;
                    case "2":
                        employerProfileSalaryAccountsReports = SalaryAccountsService.GetAllEmployerProfiledWithAccountNumber(accountNumber, CallerFormName, "ShowEmployerProfiledSalaryAccountsReport", callerIpAddress);
                        break;
                    case "3":
                        employerProfileSalaryAccountsReports = SalaryAccountsService.GetAllEmployerProfiledBetweenStartDateAndEndDate(dateFrom, dateTo, CallerFormName, "ShowEmployerProfiledSalaryAccountsReport", callerIpAddress);
                        break;
                    default:
                        break;
                }

                var dataTable = new DataTable();

                if (employerProfileSalaryAccountsReports != null && employerProfileSalaryAccountsReports.Count > 0)
                {
                    dataTable.Columns.Add("AccountNumber");
                    dataTable.Columns.Add("AccountName");
                    dataTable.Columns.Add("EmployerName");
                    dataTable.Columns.Add("CreatedOn");
                    dataTable.Columns.Add("CreatedBy");
                    dataTable.Columns.Add("ApprovedOn");
                    dataTable.Columns.Add("ApprovedBy");
                    dataTable.Rows.Clear();

                    foreach (var item in employerProfileSalaryAccountsReports)
                    {
                        dataTable.Rows.Add(
                            item.AccountNumber,
                            item.AccountName,
                            item.EmployerName,
                            Convert.ToDateTime(item.CreatedOn).ToString("dd-MM-yyyy hh:mm tt"),
                            item.CreatedBy,
                            !string.IsNullOrEmpty(Convert.ToString(item.ApprovedOn)) ? Convert.ToDateTime(item.ApprovedOn).ToString("dd-MM-yyyy hh:mm tt") : "",
                            item.ApprovedBy);
                    }
                }

                rv.ProcessingMode = ProcessingMode.Local;
                rv.LocalReport.DataSources.Add(new ReportDataSource("dsEmployerProfiledSalaryAccountsReport", dataTable));
                rv.LocalReport.Refresh();

            }
            catch (Exception ex)
            {
                LogUtility.LogError(CallerFormName, "ShowEmployerProfiledSalaryAccountsReport", callerIpAddress, ex);
            }
        }
        private void ShowIdentifiedSalaryAccountsReport(string viewType)
        {
            try
            {
                rv.Reset();
                rv.LocalReport.DisplayName = "Identified Salary Accounts Report";
                rv.LocalReport.ReportPath = Server.MapPath(ConfigurationUtility.GetAppSettingValue("IdentifiedSalaryAccountsReportPath"));

                List<SalProfiling> salProfilings = null;

                switch (viewType)
                {
                    case "BVN":
                        salProfilings = FinacleServices.GetAllFromSalProfiling($"SELECT [COLUMNS] from {ConfigurationUtility.GetAppSettingValue("MISSchemaName")}.mis_sal_profiling where IS_VALID_BVN IS NOT NULL order by INSERTED_DATE desc", CallerFormName, "ShowIdentifiedSalaryAccountsReport", callerIpAddress);
                        break;
                    case "CRMS":
                        salProfilings = FinacleServices.GetAllFromSalProfiling($"SELECT [COLUMNS] from {ConfigurationUtility.GetAppSettingValue("MISSchemaName")}.mis_sal_profiling where IS_VALID_CRMS IS NOT NULL order by INSERTED_DATE desc", CallerFormName, "ShowIdentifiedSalaryAccountsReport", callerIpAddress);
                        break;
                    case "CRC":
                        salProfilings = FinacleServices.GetAllFromSalProfiling($"SELECT [COLUMNS] from {ConfigurationUtility.GetAppSettingValue("MISSchemaName")}.mis_sal_profiling where IS_VALID_CRC IS NOT NULL order by INSERTED_DATE desc", CallerFormName, "ShowIdentifiedSalaryAccountsReport", callerIpAddress);
                        break;
                    case "STM":
                        salProfilings = FinacleServices.GetAllFromSalProfiling($"SELECT [COLUMNS] from {ConfigurationUtility.GetAppSettingValue("MISSchemaName")}.mis_sal_profiling where SRC='STM' order by INSERTED_DATE desc", CallerFormName, "ShowIdentifiedSalaryAccountsReport", callerIpAddress);
                        break;
                    case "NRM":
                        salProfilings = FinacleServices.GetAllFromSalProfiling($"SELECT [COLUMNS] from {ConfigurationUtility.GetAppSettingValue("MISSchemaName")}.mis_sal_profiling where SRC='NRM' order by INSERTED_DATE desc", CallerFormName, "ShowIdentifiedSalaryAccountsReport", callerIpAddress);
                        break;
                    case "SAMP":
                        salProfilings = FinacleServices.GetAllFromSalProfiling($"SELECT [COLUMNS] from {ConfigurationUtility.GetAppSettingValue("MISSchemaName")}.mis_sal_profiling where SRC='SAMP' order by INSERTED_DATE desc", CallerFormName, "ShowIdentifiedSalaryAccountsReport", callerIpAddress);
                        break;
                    default:
                        salProfilings = FinacleServices.GetAllFromSalProfiling(CallerFormName, "ShowIdentifiedSalaryAccountsReport", callerIpAddress);
                        break;
                }

                var dataTable = new DataTable();

                if (salProfilings != null && salProfilings.Count > 0)
                {
                    dataTable.Columns.Add("AccountNumber");
                    dataTable.Columns.Add("LastName");
                    dataTable.Columns.Add("FirstName");
                    dataTable.Columns.Add("MiddleName");
                    dataTable.Columns.Add("MostFrequentNarration");
                    dataTable.Columns.Add("MostFrequentTransactionDate");
                    dataTable.Columns.Add("Src");
                    dataTable.Columns.Add("InsertedDate");
                    dataTable.Columns.Add("IsValidBvn");
                    dataTable.Columns.Add("BvnCheckDate");
                    dataTable.Columns.Add("IsValidCRMS");
                    dataTable.Columns.Add("CRMSCheckDate");
                    dataTable.Columns.Add("IsValidCRC");
                    dataTable.Columns.Add("CRCCheckDate");
                    dataTable.Rows.Clear();

                    foreach (var item in salProfilings)
                    {
                        dataTable.Rows.Add(
                            item.Foracid,
                            item.LastName,
                            item.FirstName,
                            item.Middlename,
                            item.MostFreqNarr,
                            item.MostFreqTranDate,
                            item.Src,
                            item.InsertedDate,
                            item.IsValidBvn,
                            item.BvnCheckDate,
                            item.IsValidCRMS,
                            item.CRMSCheckDate,
                            item.IsValidCRC,
                            item.CRCSCheckDate);
                    }
                }

                rv.ProcessingMode = ProcessingMode.Local;
                rv.LocalReport.DataSources.Add(new ReportDataSource("dsIdentifiedSalaryAccountsReport", dataTable));
                rv.LocalReport.Refresh();

            }
            catch (Exception ex)
            {
                LogUtility.LogError(CallerFormName, "ShowIdentifiedSalaryAccountsReport", callerIpAddress, ex);
            }
        }
    }
}