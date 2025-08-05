using App.DataModels.Models;
using App.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.Web.ViewModels
{
    public class CreateFintechProfileViewModel
    {
        public CreateFintechProfileViewModel()
        {

        }
        public CreateFintechProfileViewModel(string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            FintechModel = new FintechModel(callerFormName, callerFormMethod, callerIpAddress);
            FintechOnBoardingDocumentsModels = CacheData.Onboardingdocuments.Where(c => c.active_flag.Equals(true)).Select(document => new FintechOnBoardingDocumentsModel
            {
                DocumentId = Convert.ToString(document.id),
                DocumentName = document.document_name,
                MandatoryFlag = document.mandatory_flag

            }).ToList();
        }

        public FintechModel FintechModel { get; set; }
        public List<FintechOnBoardingDocumentsModel> FintechOnBoardingDocumentsModels { get; set; }
    }
}