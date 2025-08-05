using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Core.DataTransferObjects
{
    public class GetAccountDetailsResponse
    {
        public string CustomerId { get; set; }
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public string AccountType { get; set; }
        public string FreezeCode { get; set; }
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
        public string MobileNo { get; set; }
        public string Email { get; set; }
        public string RelationshipManagerId { get; set; }
        public string RequestId { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
    }
}
