using App.Web.Models;
using App.Core.BusinessLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace App.Web.ViewModels
{
    public class ViewEmployerProfiledSalaryAccountsReportViewModel
    {
        public ViewEmployerProfiledSalaryAccountsReportViewModel()
        {

        }
        public ViewEmployerProfiledSalaryAccountsReportViewModel(string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            EmployerProfiledSalaryAccountSearchCriteriaSelectListItems = DropdownManager.PopulateEmployerProfiledSalaryAccountSearchCriteriaSelectListItems(callerFormName, callerFormMethod, callerIpAddress);
            EmployersSelectListItems = DropdownManager.PopulateEmployerSelectListItems(callerFormName, callerFormMethod, callerIpAddress);
        }

        public string EmployerProfiledSalaryAccountSearchCriteriaId { get; set; }
        public List<SelectListItem> EmployerProfiledSalaryAccountSearchCriteriaSelectListItems { get; set; }

        public string EmployerId { get; set; }
        public List<SelectListItem> EmployersSelectListItems { get; set; }
        public string AccountNumber { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public string From { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public string To { get; set; }
    }
}