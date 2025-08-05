using App.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace App.Web.ViewModels
{
    public class UpdateEmployerDetailsInSingleViewModel
    {
        [Required(ErrorMessage = "Kindly enter account number")]
        public string AccountNumber { get; set; }
        public bool AccountNumberDisabled { get; set; }

        [Required(ErrorMessage = "Kindly enter employer name")]
        public string EmployerName { get; set; }
    }
}