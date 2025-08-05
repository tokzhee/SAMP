using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.DataModels.Models
{
    public class EmployerProfileSalaryAccountsReport
    {
		public virtual string AccountNumber { get; set; }
		public virtual string AccountName { get; set; }
		public virtual string EmployerName { get; set; }
		public virtual DateTime CreatedOn { get; set; }
		public virtual string CreatedBy { get; set; }
		public virtual DateTime? ApprovedOn { get; set; }
		public virtual string ApprovedBy { get; set; }
	}
}
