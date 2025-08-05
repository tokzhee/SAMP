using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.Web.Models
{
    public class AccountRelationshipManagerModel
    {
        public string RelationshipManagerId { get; set; }
        public string RelationshipManagerEmail { get; set; }
        public string CustomerAccountName { get; set; }
        public string CustomerAccountNumber { get; set; }
    }
}