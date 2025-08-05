using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.DataModels.Models
{
	public class SalaryAccountsRacProfiling
	{
		public long id { get; set; }
		public long salary_account_id { get; set; }
		public string account_number { get; set; }
		public string account_name { get; set; }
		public DateTime created_on { get; set; }
		public string created_by { get; set; }
		public DateTime? approved_on { get; set; }
		public string approved_by { get; set; }
		public int? rac_profiled_status_id { get; set; }
		public DateTime? rac_profiled_on { get; set; }
		public string rac_profiled_by { get; set; }
		public Guid query_string { get; set; }
	}
}
