using App.Web.Models;
using System.Collections.Generic;

namespace App.Web.ViewModels
{
    public class ViewFintechProfileViewModel
    {
        public FintechModel FintechModel { get; set; }
        public List<FintechOnBoardingDocumentsModel> FintechOnBoardingDocumentsModels { get; set; }
        public List<PersonModel> FintechContactPersons { get; set; }
    }
}