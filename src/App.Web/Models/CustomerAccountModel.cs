using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.Web.Models
{
    public class CustomerAccountModel
    {
        public string CustomerId { get; set; }
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public string AccountType { get; set; }
        public bool IsCreditFrozen { get; set; }
        public string ProductCode { get; set; }
        public string Product { get; set; }
        public string AccountStatus { get; set; }
        public string CurrencyCode { get; set; }
        public string BranchCode { get; set; }
        public string Branch { get; set; }
        public string BookBalance { get; set; }
        public string AvailableBalance { get; set; }
        public string LienAmount { get; set; }
        public string UnclearedBalance { get; set; }
        public bool IsAccountActive { get; set; }
        public bool IsCardAccount { get; set; }
        public string MobileNo { get; set; }
        public string Email { get; set; }
        public string RelationshipManagerId { get; set; }
    }
}