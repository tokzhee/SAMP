using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Core.DataTransferObjects
{
    public class GetCustomerAccountsRequest
    {
        public string RequestId { get; set; }
        public string CountryId { get; set; }
        public string CustomerId { get; set; }
    }
}
