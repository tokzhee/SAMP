using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.Web.Models
{
    public class CRMSDetailsModel
    {
		public string CRMSRefNumber { get; set; }
		public string CreditType { get; set; }
		public string CreditLimit { get; set; }
		public string OutstandingAmount { get; set; }
		public string EffectiveDate { get; set; }
		public string Tenor { get; set; }
		public string ExpiryDate { get; set; }
		public string GrantingInstitution { get; set; }
		public string PerformanceStatus { get; set; }
	}
}