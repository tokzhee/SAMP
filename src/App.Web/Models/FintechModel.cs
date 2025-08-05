using App.Core.BusinessLogic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace App.Web.Models
{
    public class FintechModel
    {
        public FintechModel()
        {

        }
        public FintechModel(string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            FeeScaleSelectListItems = DropdownManager.PopulateFeeScaleSelectListItems(callerFormName, callerFormMethod, callerIpAddress);
            RelationshipManagerPerson = new PersonModel();
        }

        public string FintechId { get; set; }

        [Required(ErrorMessage = "FINTECH Corporate Name field is required")]
        public string CorporateName { get; set; }

        [Required(ErrorMessage = "FINTECH Official Email Address field is required")]
        public string OfficialEmailAddress { get; set; }
        
        public string HeadOfficeAddress { get; set; }

        [Required(ErrorMessage = "RM Staff ID field is required")]
        public string RelationshipManagerStaffId { get; set; }
        public PersonModel RelationshipManagerPerson { get; set; }

        [Required(ErrorMessage = "RM Sol ID field is required")]
        public string RelationshipManagerSolId { get; set; }

        [Required(ErrorMessage = "RM Sol Name field is required")]
        public string RelationshipManagerSolName { get; set; }

        [Required(ErrorMessage = "RM Sol Address field is required")]
        public string RelationshipManagerSolAddress { get; set; }

        [Required(ErrorMessage = "Account Number field is required")]
        public string AccountNumber { get; set; }

        [Required(ErrorMessage = "Account Name field is required")]
        public string AccountName { get; set; }

        [Required(ErrorMessage = "Finacle Term ID field is required")]
        public string FinacleTermId { get; set; }

        [Required(ErrorMessage = "Fee Scale field is required")]
        public string FeeScale { get; set; }
        public List<SelectListItem> FeeScaleSelectListItems { get; set; }

        [Required(ErrorMessage = "Scale Value field is required")]
        public string ScaleValue { get; set; }
        public string CapAmount { get; set; }
        public string CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public string ApprovedOn { get; set; }
        public string ApprovedBy { get; set; }
        public string LastModifiedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string UrlQueryString { get; set; }
        public int NumberOfContactPersons { get; set; }
    }
}