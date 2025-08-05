using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.DataModels.Models
{
    public class LoanExposure
    {
        public string SchemeCode { get; set; }
        public string LoanAccount { get; set; }
        public string OperationalAccount { get; set; }
        public string AccountName { get; set; }
        public string ReferenceDescription { get; set; }
        public string Principal { get; set; }
        public string PrincipalRepayment { get; set; }
        public string InterestRepayment { get; set; }
        public string Interest { get; set; }
        public string DateApproved { get; set; }
        public string DateDisbursed { get; set; }
        public string OutstandingBalance { get; set; }
        public string Frequency { get; set; }
        public string Tenor { get; set; }
        public string OutstandingTenor { get; set; }
        public string LoanSource { get; set; }
    }
}
