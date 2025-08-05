using App.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.Web.ViewModels
{
    public class ViewFintechContactPersonsViewModel
    {
        public string FintechName { get; set; }
        public List<PersonModel> FintechContactPersons { get; set; }
    }
}