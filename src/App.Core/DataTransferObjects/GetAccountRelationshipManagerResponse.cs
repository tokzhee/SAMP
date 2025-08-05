using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Core.DataTransferObjects
{
    public class GetAccountRelationshipManagerResponse
    {
        public string RelationshipManagerId { get; set; }
        public string RelationshipManagerEmail { get; set; }
        public string CustomerAccountName { get; set; }
        public string CustomerAccountNumber { get; set; }
        public string RequestId { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
    }
}
