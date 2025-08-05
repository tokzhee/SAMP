using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.DataModels.Models
{
    public class SalProfiling
    {
        public string CifId { get; set; }
        public string Foracid { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Middlename { get; set; }
        public string Bvn { get; set; }
        public string DateOfBirth { get; set; }
        public string Average { get; set; }
        public string StrdDev { get; set; }
        public string Cov { get; set; }
        public string ModeStat { get; set; }
        public string MaxVal { get; set; }
        public string MinVal { get; set; }
        public string MostFreqNarr { get; set; }
        public string MostFreqTranDate { get; set; }
        public string FirstMonth { get; set; }
        public string SecondMonth { get; set; }
        public string ThirdMonth { get; set; }
        public string FourthMonth { get; set; }
        public string FifthMonth { get; set; }
        public string SixthMonth { get; set; }
        public string Src { get; set; }
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

        //
        public string AccountStatus { get; set; }
    }
}
