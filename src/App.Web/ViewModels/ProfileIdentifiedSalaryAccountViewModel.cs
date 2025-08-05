using App.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace App.Web.ViewModels
{
    public class ProfileIdentifiedSalaryAccountViewModel
    {
        public string AccountNumber { get; set; }
        public List<SalaryPaymentModel> SalaryPaymentModels { get; set; }
    }
}