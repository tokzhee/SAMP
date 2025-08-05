using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace App.Web.Models
{
    public class SalaryPaymentModel
    {
        public string Month { get; set; }
        public string TranctionAmount { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public string TransactionDate { get; set; }
        public HttpPostedFileBase FileEvidence { get; set; }
        public string FileEvidenceSavePath { get; set; }
    }
}