using App.Core.BusinessLogic;
using App.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace App.Web.ViewModels
{
    public class ViewTransactionReportViewModel
    {
        public ViewTransactionReportViewModel()
        {

        }
        public ViewTransactionReportViewModel(string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            ReportSearchDateModel = new ReportSearchDateModel();
            FintechSelectListItems = DropdownManager.PopulateFintechSelectListItems(callerFormName, callerFormMethod, callerIpAddress);
        }

        public string FintechFinacleTermId { get; set; }
        public List<SelectListItem> FintechSelectListItems { get; set; }
        public ReportSearchDateModel ReportSearchDateModel { get; set; }

        public bool ShowFintechDropdown { get; set; }
        public bool ShowSolSelection { get; set; }
    }
}