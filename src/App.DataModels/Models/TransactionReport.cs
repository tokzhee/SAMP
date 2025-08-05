using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.DataModels.Models
{
    public class TransactionReport
    {
        public string datetime_req { get; set; }
        public string tran_nr { get; set; }
        public string message_type { get; set; }
        public string pan { get; set; }
        public string from_account_id { get; set; }
        public string to_account_id { get; set; }
        public string tran_amount_req { get; set; }
        public string tran_amount_rsp { get; set; }
        public string terminal_id { get; set; }
        public string retrieval_reference_nr { get; set; }
        public string system_trace_audit_nr { get; set; }
        public string rsp_code_rsp { get; set; }
        public string source_node_name { get; set; }
        public string sink_node_name { get; set; }
    }
}
