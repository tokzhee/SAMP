using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.Web.Models
{
    public class SalProfilingDetailsModel
    {
        public string AverageSalary { get; set; }
        public string MaxSalaryValue { get; set; }
        public string MinSalaryValue { get; set; }
        public string MostFrequentNarration { get; set; }
        public string MostFrequentTransactionDate { get; set; }
        public string FirstMonth { get; set; }
        public string SecondMonth { get; set; }
        public string ThirdMonth { get; set; }
        public string FourthMonth { get; set; }
        public string FifthMonth { get; set; }
        public string SixthMonth { get; set; }
        public string Source { get; set; }
        public string InsertedDate { get; set; }

        //
        public string IsValidBvn { get; set; }
        public string BvnCheckDate { get; set; }

        //
        public string IsValidCRMS { get; set; }
        public string CRMSCheckDate { get; set; }

        //
        public string IsValidCRC { get; set; }
        public string CRCSCheckDate { get; set; }
    }
}