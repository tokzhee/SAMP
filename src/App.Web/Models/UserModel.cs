using App.Core.BusinessLogic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace App.Web.Models
{
    public class UserModel : PersonModel
    {
        public UserModel()
        {

        }
        public UserModel(string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            RoleSelectListItems = DropdownManager.PopulateUnreservedRoleSelectListItems(callerFormName, callerFormMethod, callerIpAddress);
            AccountStatusSelectListItems = DropdownManager.PopulateUserAccountStatusSelectListItems(callerFormName, callerFormMethod, callerIpAddress);
        }
        public string Username { get; set; }
        public string AuthenticationTypeId { get; set; }
        public string AuthenticationTypeName { get; set; }
        public bool IsBranchUser { get; set; }
        public string LocalPassword { get; set; }
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public List<SelectListItem> RoleSelectListItems { get; set; }
        public string AccountStatusId { get; set; }
        public string AccountStatusName { get; set; }
        public List<SelectListItem> AccountStatusSelectListItems { get; set; }
        public string LastLoginDate { get; set; }
        public string LastLogoutDate { get; set; }
        public new string CreatedOn { get; set; }
        public new string CreatedBy { get; set; }
        public string ApprovedOn { get; set; }
        public string ApprovedBy { get; set; }
        public new string LastModifiedOn { get; set; }
        public new string LastModifiedBy { get; set; }
        public new string UrlQueryString { get; set; }
        public bool IsVisiblePassportUploadLink { get; set; }
        public bool IsVisibleChangePasswordLink { get; set; }
    }
}