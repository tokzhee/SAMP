using App.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace App.Web.ViewModels
{
    public class ViewAccount360DegreesPortfolioViewModel
    {
        [Required]
        public string AccountNumber { get; set; }
        public bool ShowAccordions { get; set; }
        public AccountDetailsModel AccountDetailsModel { get; set; }
        public CustomerDetailsModel CustomerDetailsModel { get; set; }
        public AccountRelationshipManagerModel AccountRelationshipManagerModel { get; set; }
        public List<CustomerAccountModel> CustomerAccountModels { get; set; }
        public bool IsValidBvn { get; set; }
        public BvnDetailsModel BvnDetailsModel { get; set; }
        public bool IsValidCRMS { get; set; }
        public List<CRMSDetailsModel> CRMSDetailsModels { get; set; }
        public string CRMSSummary { get; set; }
        public bool IsValidCRC { get; set; }
        public string CRCSummary { get; set; }
        public List<EmployerHistoryModel> EmployerHistoryModels { get; set; }
        public SalProfilingDetailsModel SalProfilingDetailsModel { get; set; }
        public List<LoanExposureModel> LoanExposureModels { get; set; }
        public SodaRacDetailsModel SodaRacDetailsModel { get; set; }
    }
}