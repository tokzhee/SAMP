using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.DataModels.Models
{
    public class SettlementReport
    {
        public string TranDate { get; set; }
        public string TranId { get; set; }
        public string InitSolId { get; set; }
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public string TranAmt { get; set; }
        public string TransactionType { get; set; }
        public string Narration { get; set; }
        public string RefNum { get; set; }
        public string TranRmks { get; set; }
        public string TranParticular2 { get; set; }
        public string ValueDate { get; set; }
    }
}
