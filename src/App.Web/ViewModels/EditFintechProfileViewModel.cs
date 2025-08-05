using App.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.Web.ViewModels
{
    public class EditFintechProfileViewModel
    {
        public FintechModel FintechModel { get; set; }
        public List<FintechOnBoardingDocumentsModel> FintechOnBoardingDocumentsModels { get; set; }
    }
}