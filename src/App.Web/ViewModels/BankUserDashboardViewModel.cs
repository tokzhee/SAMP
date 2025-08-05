using App.DataModels.Models;
using App.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.Web.ViewModels
{
    public class BankUserDashboardViewModel
    {
        public string IdentifiedSalaryAccountCount { get; set; }

        public string IdentifiedSalaryAccountWithSTMCount { get; set; }
        public string IdentifiedSalaryAccountWithNRMCount { get; set; }
        public string IdentifiedSalaryAccountWithSAMPCount { get; set; }
        public string IdentifiedSalaryAccountWithOTHCount { get; set; }

        public string BvnCheckedCount { get; set; }
        public string BvnCheckedPercentage { get; set; }
        public string CRMSCheckedCount { get; set; }
        public string CRMSCheckedPercentage { get; set; }
        public string CRCCheckedCount { get; set; }
        public string CRCCheckedPercentage { get; set; }
        public string EmployerProfiledSalaryAccountCount { get; set; }
        public ChartDataModel UserStatusChart { get; set; }
        public ChartDataModel MyLoginChart { get; set; }
        public ChartDataModel OverallLoginChart { get; set; }
        public List<SettlementReport> SettlementReports { get; set; }
    }
}