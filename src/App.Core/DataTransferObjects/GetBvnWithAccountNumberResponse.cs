using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Core.DataTransferObjects
{
    public class GetBvnWithAccountNumberResponse
    {
        public string AccountNumber { get; set; }
        public string CifId { get; set; }
        public object CustomerId { get; set; }
        public string Bvn { get; set; }
        public string RequestId { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
    }
}
