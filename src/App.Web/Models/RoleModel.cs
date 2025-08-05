using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace App.Web.Models
{
    public class RoleModel
    {
        [Required(ErrorMessage = "Role name field is required")]
        [RegularExpression("(^[ A-Za-z0-9./-]+$)", ErrorMessage = ("special characters are not allowed"))]
        public string RoleName { get; set; }
        public bool ActiveFlag { get; set; }
        public bool ReservedFlag { get; set; }
        public string CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public string ApprovedOn { get; set; }
        public string ApprovedBy { get; set; }
        public string LastModifiedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string UrlQueryString { get; set; }
    }
}