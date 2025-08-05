using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.Web.Models
{
    public class EmployerHistoryModel
    {
        public string EmployerName { get; set; }
        public string CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public string ApprovedOn { get; set; }
        public string ApprovedBy { get; set; }
    }
}