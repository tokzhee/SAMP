using App.DataModels.Models;
using App.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.Web.ViewModels
{
    public class RelationshipManagerDashboardViewModel
    {
        public ChartDataModel UserStatusChart { get; set; }
        public ChartDataModel MyLoginChart { get; set; }
        public ChartDataModel OverallLoginChart { get; set; }
        public List<SettlementReport> SettlementReports { get; set; }
    }
}