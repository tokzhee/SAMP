using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.DataModels.Models
{
    public class UserReport
    {
        public string Username { get; set; }
        public string Fullname { get; set; }
        public string RoleName { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ApprovedOn { get; set; }
        public string ApprovedBy { get; set; }
        public string AccountStatus { get; set; }
    }
}
