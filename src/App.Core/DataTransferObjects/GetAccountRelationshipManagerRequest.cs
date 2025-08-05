using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Core.DataTransferObjects
{
    public class GetAccountRelationshipManagerRequest
    {
        public string AccountNumber { get; set; }
        public string RequestId { get; set; }
        public string CountryId { get; set; }
    }
}
