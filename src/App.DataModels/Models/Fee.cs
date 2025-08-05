using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.DataModels.Models
{
    public class Fee
    {
        public string TermId { get; set; }
        public string TillAccountNo { get; set; }
        public string FeeActNo1 { get; set; }
        public string FeeActNo2 { get; set; }
        public string AmtUpperBand { get; set; }
        public string AmtLowerBand { get; set; }
        public string FlatRate { get; set; }
        public string CatClassif { get; set; }
        public string Percent { get; set; }
        public string DelFlag { get; set; }
        public string RcreUserId { get; set; }
        public string RcreTime { get; set; }
        public string LchgUserId { get; set; }
        public string LchgTime { get; set; }
        public string BankId { get; set; }
    }
}
