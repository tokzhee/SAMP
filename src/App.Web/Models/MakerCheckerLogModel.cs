using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.Web.Models
{
    public class MakerCheckerLogModel
    {
        public string MakerCheckerCategoryId { get; set; }
        public string MakerCheckerTypeId { get; set; }
        public string ActionName { get; set; }
        public string ActionDetails { get; set; }
        public string ActionData { get; set; }
        public string MakerId { get; set; }
        public string MakerFullname { get; set; }
        public string MakerSolId { get; set; }
        public string MakerCheckerStatus { get; set; }
        public string DateMade { get; set; }
        public string CheckerId { get; set; }
        public string CheckerFullname { get; set; }
        public string CheckerSolId { get; set; }
        public string CheckerRemarks { get; set; }
        public string DateChecked { get; set; }
        public string QueryString { get; set; }
    }
}