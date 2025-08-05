using App.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.Web.ViewModels
{
    public class CreateFintechContactPersonViewModel
    {
        public CreateFintechContactPersonViewModel()
        {
            PersonModel = new PersonModel();
        }
        public string FintechName { get; set; }
        public string FintechUrlQueryString { get; set; }
        public PersonModel PersonModel { get; set; }
    }
}