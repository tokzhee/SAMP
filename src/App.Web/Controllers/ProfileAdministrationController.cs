using App.Core.BusinessLogic;
using App.Core.Services;
using App.Core.Utilities;
using App.DataModels.Models;
using App.Web.Models;
using App.Web.ViewModels;
using Newtonsoft.Json;
using PasswordGenerator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace App.Web.Controllers
{
    [Authorize]
    [RoutePrefix("profile-administration")]
    public class ProfileAdministrationController : BaseController
    {
        private const string CallerFormName = "ProfileAdministrationController";
        private readonly string callerIpAddress;
        private readonly string callerMacAddress;

        private readonly ViewFintechsViewModel viewFintechsViewModel;
        private readonly CreateFintechProfileViewModel createFintechProfileViewModel;
        private readonly EditFintechProfileViewModel editFintechProfileViewModel;
        private readonly ReviewMakerCheckerLogViewModel reviewMakerCheckerLogViewModel;
        private List<MakerCheckerLogModel> MakerCheckerLogModels;
        private readonly ViewFintechProfileViewModel viewFintechProfileViewModel;
        private readonly ViewFintechContactPersonsViewModel viewFintechContactPersonsViewModel;
        private readonly CreateFintechContactPersonViewModel createFintechContactPersonViewModel;
        private readonly EditFintechContactPersonViewModel editFintechContactPersonViewModel;
        private readonly user userData;

        private bool stopCheckFlag = false;
        private string stopCheckMessage = "";

        //initialize password policy
        private readonly bool includeLowerCase;
        private readonly bool includeUpperCase;
        private readonly bool includeNumeric;
        private readonly bool includeSpecial;
        private readonly int passwordLength;

        private readonly string encryptionKey;

        public ProfileAdministrationController()
        {
            callerIpAddress = IpAddressManager.GetClientComputerIpAddress();
            callerMacAddress = MacAddressManager.GetClientComputerMacAddress();
            viewFintechsViewModel = new ViewFintechsViewModel();
            createFintechProfileViewModel = new CreateFintechProfileViewModel(CallerFormName, "Ctor|ProfileAdministrationController", callerIpAddress);
            editFintechProfileViewModel = new EditFintechProfileViewModel();
            reviewMakerCheckerLogViewModel = new ReviewMakerCheckerLogViewModel();
            MakerCheckerLogModels = new List<MakerCheckerLogModel>();
            viewFintechProfileViewModel = new ViewFintechProfileViewModel();
            viewFintechContactPersonsViewModel = new ViewFintechContactPersonsViewModel();
            createFintechContactPersonViewModel = new CreateFintechContactPersonViewModel();
            editFintechContactPersonViewModel = new EditFintechContactPersonViewModel();
            userData = GetUser(CallerFormName, "Ctor|ProfileAdministrationController", callerIpAddress);

            includeLowerCase = ConfigurationUtility.GetAppSettingValue("IncludeLowerCase").Equals("N") ? false : true;
            includeUpperCase = ConfigurationUtility.GetAppSettingValue("IncludeUpperCase").Equals("N") ? false : true;
            includeNumeric = ConfigurationUtility.GetAppSettingValue("IncludeNumeric").Equals("N") ? false : true;
            includeSpecial = ConfigurationUtility.GetAppSettingValue("IncludeSpecial").Equals("N") ? false : true;
            passwordLength = string.IsNullOrEmpty(ConfigurationUtility.GetAppSettingValue("PasswordLength")) ? 8 : Convert.ToInt32(ConfigurationUtility.GetAppSettingValue("PasswordLength")) < 8 ? 8 : Convert.ToInt32(ConfigurationUtility.GetAppSettingValue("PasswordLength"));

            encryptionKey = ConfigurationUtility.GetAppSettingValue("EncryptionKey");
        }

        [HttpGet]
        [Route("view-fintechs")]
        public ActionResult ViewFintechs()
        {
            const string callerFormMethod = "HttpGet|ViewFintechs";

            try
            {
                var fintechs = FintechService.GetAll(CallerFormName, callerFormMethod, callerIpAddress);
                if (fintechs.Count > 0)
                {
                    var list = fintechs.Select(fintech => new FintechModel
                    {
                        FintechId = Convert.ToString(fintech.id),
                        CorporateName = fintech.corporate_name,
                        OfficialEmailAddress = fintech.official_email_address,
                        HeadOfficeAddress = fintech.head_office_address,
                        RelationshipManagerStaffId = fintech.relationship_manager_staff_id,
                        RelationshipManagerSolId = fintech.relationship_manager_sol_id,
                        RelationshipManagerSolName = fintech.relationship_manager_sol_name,
                        RelationshipManagerSolAddress = fintech.relationship_manager_sol_address,
                        AccountNumber = fintech.account_number,
                        AccountName = fintech.account_name,
                        FinacleTermId = fintech.finacle_term_id,
                        FeeScale = fintech.fee_scale,
                        ScaleValue = fintech.scale_value.ToString("N"),
                        CapAmount = fintech.cap_amount.ToString("N"),
                        CreatedOn = Convert.ToDateTime(fintech.created_on).ToString("dd-MM-yyyy hh:mm tt"),
                        CreatedBy = fintech.created_by,
                        ApprovedOn = !string.IsNullOrEmpty(Convert.ToString(fintech.approved_on)) ? Convert.ToDateTime(fintech.approved_on).ToString("dd-MM-yyyy hh:mm tt") : "n/p",
                        ApprovedBy = fintech.approved_by,
                        LastModifiedOn = !string.IsNullOrEmpty(Convert.ToString(fintech.last_modified_on)) ? Convert.ToDateTime(fintech.last_modified_on).ToString("dd-MM-yyyy hh:mm tt") : "n/p",
                        LastModifiedBy = fintech.last_modified_by,
                        UrlQueryString = Convert.ToString(fintech.query_string),
                        NumberOfContactPersons = FintechContactPersonService.GetWithFintechId(Convert.ToString(fintech.id), CallerFormName, callerFormMethod, callerIpAddress).Count

                    }).ToList();

                    //handle the RM
                    if (userData.person.person_type_id.Equals((int)PersonType.BankRelationshipManager))
                    {
                        var fintechId = FintechService.GetWithRelationshipManagerPersonId(Convert.ToString(userData.person_id), CallerFormName, callerFormMethod, callerIpAddress)?.id;
                        list = list.Where(fintech => fintech.FintechId.Equals(Convert.ToString(fintechId))).ToList();
                    }

                    //handle the FCP
                    if (userData.person.person_type_id.Equals((int)PersonType.FintechContactPerson))
                    {
                        var fintechId = FintechContactPersonService.GetWithContactPersonId(Convert.ToString(userData.person_id), CallerFormName, callerFormMethod, callerIpAddress)?.fintech_id;
                        list = list.Where(fintech => fintech.FintechId.Equals(Convert.ToString(fintechId))).ToList();
                    }

                    viewFintechsViewModel.FintechModels = list;
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(CallerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return View(viewFintechsViewModel);
        }

        [HttpGet]
        [Route("create-new-fintech-profile")]
        public ActionResult CreateNewFintechProfile()
        {
            return View(createFintechProfileViewModel);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("create-new-fintech-profile")]
        public ActionResult CreateNewFintechProfile(FintechModel fintechModel, List<FintechOnBoardingDocumentsModel> fintechOnBoardingDocuments)
        {
            const string callerFormMethod = "HttpPost|CreateNewFintechProfile";

            try
            {
                var checkerEmails = "";
                if (!AuthorizeRequest(Request.Url.AbsolutePath, ref checkerEmails, CallerFormName, callerFormMethod, callerIpAddress))
                {
                    AlertUser("Unauthorised Access! You may not have been permitted to access the resource. Kindly login to re-authenticate. If issue persist, contact administrator.", AlertType.Error);
                    return RedirectToAction("Logout", "Account");
                }

                if (!ValidationUtility.IsValidEmail(fintechModel.OfficialEmailAddress))
                {
                    ModelState.AddModelError("OfficialEmailAddress", "FINTECH Official Email Address field does not meet the required data format");
                }

                if (!ValidationUtility.IsValidTextInput(fintechModel.RelationshipManagerPerson.Surname))
                {
                    ModelState.AddModelError("RMSurname", "Relationship Manager Surname field is required");
                }

                if (!ValidationUtility.IsValidTextInput(fintechModel.RelationshipManagerPerson.Firstname))
                {
                    ModelState.AddModelError("RMFirstname", "Relationship Manager First name field is required");
                }

                if (!ValidationUtility.IsValidEmail(fintechModel.RelationshipManagerPerson.EmailAddress))
                {
                    ModelState.AddModelError("RMEmailAddress", "Relationship Manager Email address field does not meet the required data format");
                }

                if (!ValidationUtility.IsValidDecimalInput(Convert.ToString(fintechModel.ScaleValue)))
                {
                    ModelState.AddModelError("ScaleValue", "Fee Scale Value field does not meet the required data format");
                }
                else
                {
                    if (Convert.ToDecimal(fintechModel.ScaleValue) <= 0)
                    {
                        ModelState.AddModelError("ScaleValue", "Fee scale value must be greater than zero");
                    }
                }

                if (!string.IsNullOrEmpty(fintechModel.FeeScale) && fintechModel.FeeScale.Equals("PERCENTAGE"))
                {
                    if (!ValidationUtility.IsValidDecimalInput(Convert.ToString(fintechModel.CapAmount)))
                    {
                        ModelState.AddModelError("CapAmount", "Cap Ammount field does not meet the required data format");
                    }
                    else
                    {
                        if (Convert.ToDecimal(fintechModel.CapAmount) <= 0)
                        {
                            ModelState.AddModelError("CapAmount", "Cap Ammount value must be greater than zero");
                        }
                    }
                }

                for (int i = 0; i < fintechOnBoardingDocuments.Count; i++)
                {
                    if (fintechOnBoardingDocuments[i].DocumentFile == null || fintechOnBoardingDocuments[i].DocumentFile.ContentLength == 0)
                    {
                        if (fintechOnBoardingDocuments[i].MandatoryFlag)
                        {
                            ModelState.AddModelError($"{fintechOnBoardingDocuments[i].DocumentName.Replace(" ", "")}", $"{fintechOnBoardingDocuments[i].DocumentName} is required");
                        }
                    }
                    else
                    {
                        var fileExtension = Path.GetExtension(fintechOnBoardingDocuments[i].DocumentFile.FileName);
                        var supportedOnBoardingDocumentsUploadFormats = ConfigurationUtility.GetAppSettingValue("SupportedOnBoardingDocumentsUploadFormats").Split(',');
                        if (!supportedOnBoardingDocumentsUploadFormats.Contains(fileExtension))
                        {
                            ModelState.AddModelError($"{fintechOnBoardingDocuments[i].DocumentName.Replace(" ", "")}", $"{fintechOnBoardingDocuments[i].DocumentName} uploaded does not meet the required file format");
                        }

                        var fileSize = fintechOnBoardingDocuments[i].DocumentFile.ContentLength;
                        var maxUploadSize = ConfigurationUtility.GetAppSettingValue("MaxOnBoardingDocumentsUploadSize");
                        if (fileSize > Convert.ToInt32(maxUploadSize))
                        {
                            ModelState.AddModelError($"{fintechOnBoardingDocuments[i].DocumentName.Replace(" ", "")}", $"{fintechOnBoardingDocuments[i].DocumentName} uploaded is more than the required file upload size");
                        }
                    }
                }

                if (!ModelState.IsValid)
                {
                    var errorMessages = string.Join("<br/>", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
                    stopCheckFlag = true;
                    stopCheckMessage = errorMessages;
                }
                
                var makerCheckerAction = "FINTECH Profile Creation";
                var makerCheckerActionDetails = $"Create new FINTECH Profile '{fintechModel.CorporateName}'";

                fintech fintech = null;
                if (!stopCheckFlag)
                {
                    fintech = FintechService.GetWithFintechOfficialEmailAddress(fintechModel.OfficialEmailAddress, CallerFormName, callerFormMethod, callerIpAddress);
                    if (fintech != null)
                    {
                        stopCheckFlag = true;
                        stopCheckMessage = "Unable to intiate request; FINTECH already exists.";
                    }
                }

                if (!stopCheckFlag)
                {
                    fintech = new fintech
                    {
                        corporate_name = fintechModel.CorporateName,
                        official_email_address = fintechModel.OfficialEmailAddress,
                        head_office_address = fintechModel.HeadOfficeAddress,
                        relationship_manager_staff_id = fintechModel.RelationshipManagerStaffId,
                        person = new person
                        {
                            surname = fintechModel.RelationshipManagerPerson.Surname,
                            first_name = fintechModel.RelationshipManagerPerson.Firstname,
                            middle_name = fintechModel.RelationshipManagerPerson.Middlename,
                            mobile_number = fintechModel.RelationshipManagerPerson.MobileNumber,
                            email_address = fintechModel.RelationshipManagerPerson.EmailAddress,
                            person_type_id = (int)PersonType.BankRelationshipManager,
                            created_on = DateTime.UtcNow.AddHours(1),
                            created_by = $"{userData.person?.surname}, {userData.person?.first_name} {userData.person?.middle_name}",
                            query_string = Guid.NewGuid()
                        },
                        relationship_manager_sol_id = fintechModel.RelationshipManagerSolId,
                        relationship_manager_sol_name = fintechModel.RelationshipManagerSolName,
                        relationship_manager_sol_address = fintechModel.RelationshipManagerSolAddress,
                        account_number = fintechModel.AccountNumber,
                        account_name = fintechModel.AccountName,
                        finacle_term_id = fintechModel.FinacleTermId,
                        fee_scale = fintechModel.FeeScale,
                        scale_value = Convert.ToDecimal(fintechModel.ScaleValue),
                        cap_amount = fintechModel.FeeScale.Equals("FLAT") ? Convert.ToDecimal(fintechModel.ScaleValue) : Convert.ToDecimal(fintechModel.CapAmount),

                        fintechonboardingdocuments = fintechOnBoardingDocuments.Select(onBoardingDocument => new fintechonboardingdocument
                        {
                            document_id = Convert.ToInt64(onBoardingDocument.DocumentId),
                            document_save_path = (onBoardingDocument.DocumentFile != null && onBoardingDocument.DocumentFile.ContentLength > 0) ? SaveFile(onBoardingDocument.DocumentFile, $"{Guid.NewGuid().ToString()}-{onBoardingDocument.DocumentName}", CallerFormName, callerFormMethod, callerIpAddress) : null,
                            created_on = DateTime.UtcNow.AddHours(1),
                            created_by = $"{userData.person?.surname}, {userData.person?.first_name} {userData.person?.middle_name}",
                            query_string = Guid.NewGuid()

                        }).ToList(),

                        created_on = DateTime.UtcNow.AddHours(1),
                        created_by = $"{userData.person?.surname}, {userData.person?.first_name} {userData.person?.middle_name}",
                        query_string = Guid.NewGuid(),
                    };

                    var makerCheckerActionData = JsonConvert.SerializeObject(fintech);
                    var makerCheckerLog = new makercheckerlog
                    {
                        maker_checker_category_id = (int)MakerCheckerCategory.Fintech,
                        maker_checker_type_id = (int)MakerCheckerType.CreateNewFintechProfile,
                        action_name = makerCheckerAction,
                        action_details = makerCheckerActionDetails,
                        action_data = makerCheckerActionData,
                        maker_person_id = userData.person_id,
                        maker_username = userData.username,
                        maker_fullname = $"{userData.person?.surname}, {userData.person?.first_name} {userData.person?.middle_name}",
                        maker_checker_status = (int)MakerCheckerStatus.Initiated,
                        date_made = DateTime.UtcNow.AddHours(1),
                        query_string = Guid.NewGuid()
                    };

                    var makerCheckerLogInsertResult = MakerCheckerLogService.Insert(makerCheckerLog, CallerFormName, callerFormMethod, callerIpAddress);
                    if (makerCheckerLogInsertResult <= 0)
                    {
                        stopCheckFlag = true;
                        stopCheckMessage = "Unable to submit request for approval at the moment. Please try again later!";
                    }
                }

                if (!stopCheckFlag)
                {
                    //Task 1
                    new Task(() =>
                    {
                        var appActivity = new appactivity
                        {
                            audit_username = userData.username,
                            audit_fullname = $"{userData.person?.surname}, {userData.person?.first_name} {userData.person?.middle_name}",
                            audit_ipaddress = callerIpAddress,
                            audit_macaddress = callerMacAddress,
                            operation = $"{makerCheckerAction}".ToUpper(),
                            comments = $"{makerCheckerActionDetails}",
                            created_on = DateTime.Now,
                            created_by = "System"
                        };
                        AppActivityService.Insert(appActivity, CallerFormName, callerFormMethod, callerIpAddress);

                    }).Start();

                    //Task 2
                    new Task(() =>
                    {
                        if (!string.IsNullOrEmpty(checkerEmails.Trim().Replace(",", "")))
                        {
                            var templatePath = Server.MapPath(ConfigurationUtility.GetAppSettingValue("PendingItemsNotificationBodyTemplate"));
                            var mailBody = System.IO.File.ReadAllText(templatePath).Replace("{ItemType}", "Created FINTECH Profile");
                            var mailRecipient = checkerEmails;
                            EmailLogManager.Log(EmailType.PendingItemsNotification, mailBody, mailRecipient, "", "", CallerFormName, callerFormMethod, callerIpAddress);
                        }

                    }).Start();

                    AlertUser("Successfully submitted request for approval. Your changes will be effected once approved.", AlertType.Success);

                    return RedirectToAction("CreateNewFintechProfile");
                }
                else
                {
                    AlertUser(stopCheckMessage, AlertType.Error);
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(CallerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return View(createFintechProfileViewModel);
        }

        [HttpGet]
        [Route("edit-existing-fintech-profile")]
        public ActionResult EditExistingFintechProfile(string q)
        {
            const string callerFormMethod = "HttpGet|EditExistingFintechProfile";

            try
            {
                var fintech = FintechService.GetWithUrlQueryString(q, CallerFormName, callerFormMethod, callerIpAddress);
                if (fintech == null)
                {
                    stopCheckFlag = true;
                    stopCheckMessage = "No Fintech Profile Found with the Search Parameter";
                }

                if (!stopCheckFlag)
                {
                    editFintechProfileViewModel.FintechModel = new FintechModel(CallerFormName, callerFormMethod, callerIpAddress)
                    {
                        CorporateName = fintech.corporate_name,
                        OfficialEmailAddress = fintech.official_email_address,
                        HeadOfficeAddress = fintech.head_office_address,
                        RelationshipManagerStaffId = fintech.relationship_manager_staff_id,
                        RelationshipManagerPerson = (from p in PersonService.GetAll(CallerFormName, callerFormMethod, callerIpAddress)
                                                     where p.id.Equals(fintech.relationship_manager_person_id)
                                                     select new PersonModel
                                                     {
                                                         Surname = p.surname,
                                                         Firstname = p.first_name,
                                                         Middlename = p.middle_name,
                                                         MobileNumber = p.mobile_number,
                                                         EmailAddress = p.email_address

                                                     }).FirstOrDefault(),

                        RelationshipManagerSolId = fintech.relationship_manager_sol_id,
                        RelationshipManagerSolName = fintech.relationship_manager_sol_name,
                        RelationshipManagerSolAddress = fintech.relationship_manager_sol_address,
                        AccountNumber = fintech.account_number,
                        AccountName = fintech.account_name,
                        FinacleTermId = fintech.finacle_term_id,
                        FeeScale = fintech.fee_scale,
                        ScaleValue = Convert.ToString(fintech.scale_value),
                        CapAmount = Convert.ToString(fintech.cap_amount),
                        UrlQueryString = q
                    };

                    editFintechProfileViewModel.FintechOnBoardingDocumentsModels = (from fod in FintechOnBoardingDocumentsService.GetWithFintechId(Convert.ToString(fintech.id), CallerFormName, callerFormMethod, callerIpAddress)
                                                                                    select new FintechOnBoardingDocumentsModel
                                                                                    {
                                                                                        DocumentId = Convert.ToString(fod.document_id),
                                                                                        DocumentName = CacheData.Onboardingdocuments.FirstOrDefault(c => c.id.Equals(fod.document_id))?.document_name,
                                                                                        DocumentSavePath = fod.document_save_path,
                                                                                        MandatoryFlag = CacheData.Onboardingdocuments.FirstOrDefault(c => c.id.Equals(fod.document_id)).mandatory_flag

                                                                                    }).ToList();
                }
                else
                {
                    AlertUser(stopCheckMessage, AlertType.Error);
                    return RedirectToAction("ViewFintechs");
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(CallerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return View(editFintechProfileViewModel);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("edit-existing-fintech-profile")]
        public ActionResult EditExistingFintechProfile(FintechModel fintechModel, List<FintechOnBoardingDocumentsModel> fintechOnBoardingDocuments)
        {
            const string callerFormMethod = "HttpPost|EditExistingFintechProfile";

            try
            {
                var checkerEmails = "";
                if (!AuthorizeRequest(Request.Url.AbsolutePath, ref checkerEmails, CallerFormName, callerFormMethod, callerIpAddress))
                {
                    AlertUser("Unauthorised Access! You may not have been permitted to access the resource. Kindly login to re-authenticate. If issue persist, contact administrator.", AlertType.Error);
                    return RedirectToAction("Logout", "Account");
                }

                if (!ValidationUtility.IsValidEmail(fintechModel.OfficialEmailAddress))
                {
                    ModelState.AddModelError("OfficialEmailAddress", "FINTECH Official Email Address field does not meet the required data format");
                }

                if (!ValidationUtility.IsValidTextInput(fintechModel.RelationshipManagerPerson.Surname))
                {
                    ModelState.AddModelError("RMSurname", "Relationship Manager Surname field is required");
                }

                if (!ValidationUtility.IsValidTextInput(fintechModel.RelationshipManagerPerson.Firstname))
                {
                    ModelState.AddModelError("RMFirstname", "Relationship Manager First name field is required");
                }

                if (!ValidationUtility.IsValidEmail(fintechModel.RelationshipManagerPerson.EmailAddress))
                {
                    ModelState.AddModelError("RMEmailAddress", "Relationship Manager Email address field does not meet the required data format");
                }

                if (!ValidationUtility.IsValidDecimalInput(Convert.ToString(fintechModel.ScaleValue)))
                {
                    ModelState.AddModelError("ScaleValue", "Fee Scale Value field does not meet the required data format");
                }
                else
                {
                    if (Convert.ToDecimal(fintechModel.ScaleValue) <= 0)
                    {
                        ModelState.AddModelError("ScaleValue", "Fee scale value must be greater than zero");
                    }
                }

                if (!string.IsNullOrEmpty(fintechModel.FeeScale) && fintechModel.FeeScale.Equals("PERCENTAGE"))
                {
                    if (!ValidationUtility.IsValidDecimalInput(Convert.ToString(fintechModel.CapAmount)))
                    {
                        ModelState.AddModelError("CapAmount", "Cap Ammount field does not meet the required data format");
                    }
                    else
                    {
                        if (Convert.ToDecimal(fintechModel.CapAmount) <= 0)
                        {
                            ModelState.AddModelError("CapAmount", "Cap Ammount value must be greater than zero");
                        }
                    }
                }

                for (int i = 0; i < fintechOnBoardingDocuments.Count; i++)
                {
                    if (string.IsNullOrEmpty(fintechOnBoardingDocuments[i].DocumentSavePath))
                    {
                        if (fintechOnBoardingDocuments[i].DocumentFile == null || fintechOnBoardingDocuments[i].DocumentFile.ContentLength == 0)
                        {
                            if (fintechOnBoardingDocuments[i].MandatoryFlag)
                            {
                                ModelState.AddModelError($"{fintechOnBoardingDocuments[i].DocumentName.Replace(" ", "")}", $"{fintechOnBoardingDocuments[i].DocumentName} is required");
                            }
                        }
                        else
                        {
                            var fileExtension = Path.GetExtension(fintechOnBoardingDocuments[i].DocumentFile.FileName);
                            var supportedOnBoardingDocumentsUploadFormats = ConfigurationUtility.GetAppSettingValue("SupportedOnBoardingDocumentsUploadFormats").Split(',');
                            if (!supportedOnBoardingDocumentsUploadFormats.Contains(fileExtension))
                            {
                                ModelState.AddModelError($"{fintechOnBoardingDocuments[i].DocumentName.Replace(" ", "")}", $"{fintechOnBoardingDocuments[i].DocumentName} uploaded does not meet the required file format");
                            }

                            var fileSize = fintechOnBoardingDocuments[i].DocumentFile.ContentLength;
                            var maxUploadSize = ConfigurationUtility.GetAppSettingValue("MaxOnBoardingDocumentsUploadSize");
                            if (fileSize > Convert.ToInt32(maxUploadSize))
                            {
                                ModelState.AddModelError($"{fintechOnBoardingDocuments[i].DocumentName.Replace(" ", "")}", $"{fintechOnBoardingDocuments[i].DocumentName} uploaded is more than the required file upload size");
                            }
                        }
                    }
                }

                if (!ModelState.IsValid)
                {
                    var errorMessages = string.Join("<br/>", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
                    stopCheckFlag = true;
                    stopCheckMessage = errorMessages;
                }

                var makerCheckerAction = "FINTECH Profile Editing";
                var makerCheckerActionDetails = "";

                fintech fintech = null;
                if (!stopCheckFlag)
                {
                    fintech = FintechService.GetWithUrlQueryString(fintechModel.UrlQueryString, CallerFormName, callerFormMethod, callerIpAddress);
                    if (fintech == null)
                    {
                        stopCheckFlag = true;
                        stopCheckMessage = "Unable to retrieve existing fintech profile at the moment, kindly try again later";
                    }
                }

                if (!stopCheckFlag)
                {
                    if (fintechModel.CorporateName != fintech.corporate_name)
                    {
                        makerCheckerActionDetails += $"Edit Corporate Name:{fintech.corporate_name} From '{fintech.corporate_name}' to '{fintechModel.CorporateName}'|";
                    }

                    if (fintechModel.OfficialEmailAddress != fintech.official_email_address)
                    {
                        makerCheckerActionDetails += $"Edit Official Email Address:{fintech.official_email_address} From '{fintech.official_email_address}' to '{fintechModel.OfficialEmailAddress}'|";
                    }

                    //!fintechModel.HeadOfficeAddress.Equals(fintech.head_office_address)
                    if (fintechModel.HeadOfficeAddress != fintech.head_office_address)
                    {
                        makerCheckerActionDetails += $"Edit Head Office Address:{fintech.head_office_address} From '{fintech.head_office_address}' to '{fintechModel.HeadOfficeAddress}'|";
                    }

                    if (fintechModel.RelationshipManagerStaffId != fintech.relationship_manager_staff_id)
                    {
                        makerCheckerActionDetails += $"Edit Relationship Staff ID:{fintech.relationship_manager_staff_id} From '{fintech.relationship_manager_staff_id}' to '{fintechModel.RelationshipManagerStaffId}'|";
                    }

                    if (fintechModel.AccountNumber != fintech.account_number)
                    {
                        makerCheckerActionDetails += $"Edit Account Number:{fintech.account_number} From '{fintech.account_number}' to '{fintechModel.AccountNumber}'|";
                    }

                    if (fintechModel.FinacleTermId != fintech.finacle_term_id)
                    {
                        makerCheckerActionDetails += $"Edit Finacle Term ID:{fintech.finacle_term_id} From '{fintech.finacle_term_id}' to '{fintechModel.FinacleTermId}'|";
                    }

                    if (fintechModel.FeeScale != fintech.fee_scale)
                    {
                        makerCheckerActionDetails += $"Edit Fee Scale:{fintech.fee_scale} From '{fintech.fee_scale}' to '{fintechModel.FeeScale}'|";
                    }

                    if (Convert.ToString(fintechModel.ScaleValue) != Convert.ToString(fintech.scale_value))
                    {
                        if (!Convert.ToDecimal(fintechModel.ScaleValue).Equals(Convert.ToDecimal(fintech.scale_value)))
                        {
                            makerCheckerActionDetails += $"Edit Scale Value:{fintech.scale_value} From '{fintech.scale_value}' to '{fintechModel.ScaleValue}'|";
                        }

                    }

                    if (Convert.ToString(fintechModel.CapAmount) != Convert.ToString(fintech.cap_amount))
                    {
                        if (!Convert.ToDecimal(fintechModel.CapAmount).Equals(Convert.ToDecimal(fintech.cap_amount)))
                        {
                            makerCheckerActionDetails += $"Edit Cap Amount:{fintech.cap_amount} From '{fintech.cap_amount}' to '{fintechModel.CapAmount}'|";
                        }

                    }

                    for (int i = 0; i < fintechOnBoardingDocuments.Count; i++)
                    {
                        if (fintechOnBoardingDocuments[i].DocumentFile != null && fintechOnBoardingDocuments[i].DocumentFile.ContentLength > 0)
                        {
                            makerCheckerActionDetails += $"Uploaded New Document for '{fintechOnBoardingDocuments[i].DocumentName}'|";
                        }
                    }


                    makerCheckerActionDetails = makerCheckerActionDetails.Length > 0 ? makerCheckerActionDetails.Substring(0, makerCheckerActionDetails.Length - 1) : "";

                    if (string.IsNullOrEmpty(makerCheckerActionDetails))
                    {
                        stopCheckFlag = true;
                        stopCheckMessage = "Unable to initiate changes because no modification observed";
                    }
                }

                if (!stopCheckFlag)
                {
                    fintech = new fintech
                    {
                        id = fintech.id,
                        corporate_name = fintechModel.CorporateName,
                        official_email_address = fintechModel.OfficialEmailAddress,
                        head_office_address = fintechModel.HeadOfficeAddress,
                        relationship_manager_staff_id = fintechModel.RelationshipManagerStaffId,
                        person = new person
                        {
                            surname = fintechModel.RelationshipManagerPerson.Surname,
                            first_name = fintechModel.RelationshipManagerPerson.Firstname,
                            middle_name = fintechModel.RelationshipManagerPerson.Middlename,
                            mobile_number = fintechModel.RelationshipManagerPerson.MobileNumber,
                            email_address = fintechModel.RelationshipManagerPerson.EmailAddress,
                            person_type_id = (int)PersonType.BankRelationshipManager,
                            last_modified_on = DateTime.UtcNow.AddHours(1),
                            last_modified_by = $"{userData.person?.surname}, {userData.person?.first_name} {userData.person?.middle_name}"
                        },
                        relationship_manager_sol_id = fintechModel.RelationshipManagerSolId,
                        relationship_manager_sol_name = fintechModel.RelationshipManagerSolName,
                        relationship_manager_sol_address = fintechModel.RelationshipManagerSolAddress,
                        account_number = fintechModel.AccountNumber,
                        account_name = fintechModel.AccountName,
                        finacle_term_id = fintechModel.FinacleTermId,
                        fee_scale = fintechModel.FeeScale,
                        scale_value = Convert.ToDecimal(fintechModel.ScaleValue),
                        cap_amount = fintechModel.FeeScale.Equals("FLAT") ? Convert.ToDecimal(fintechModel.ScaleValue) : Convert.ToDecimal(fintechModel.CapAmount),

                        fintechonboardingdocuments = fintechOnBoardingDocuments.Select(onBoardingDocument => new fintechonboardingdocument
                        {
                            document_id = Convert.ToInt64(onBoardingDocument.DocumentId),
                            document_save_path = (onBoardingDocument.DocumentFile != null && onBoardingDocument.DocumentFile.ContentLength > 0) ? SaveFile(onBoardingDocument.DocumentFile, $"{Guid.NewGuid().ToString()}-{onBoardingDocument.DocumentName}", CallerFormName, callerFormMethod, callerIpAddress) : onBoardingDocument.DocumentSavePath,
                            last_modified_on = DateTime.UtcNow.AddHours(1),
                            last_modified_by = $"{userData.person?.surname}, {userData.person?.first_name} {userData.person?.middle_name}"

                        }).ToList(),

                        last_modified_on = DateTime.UtcNow.AddHours(1),
                        last_modified_by = $"{userData.person?.surname}, {userData.person?.first_name} {userData.person?.middle_name}"
                    };

                    var makerCheckerActionData = JsonConvert.SerializeObject(fintech);
                    var makerCheckerLog = new makercheckerlog
                    {
                        maker_checker_category_id = (int)MakerCheckerCategory.Fintech,
                        maker_checker_type_id = (int)MakerCheckerType.EditExistingFintechProfile,
                        action_name = makerCheckerAction,
                        action_details = makerCheckerActionDetails,
                        action_data = makerCheckerActionData,
                        maker_person_id = userData.person_id,
                        maker_username = userData.username,
                        maker_fullname = $"{userData.person?.surname}, {userData.person?.first_name} {userData.person?.middle_name}",
                        maker_checker_status = (int)MakerCheckerStatus.Initiated,
                        date_made = DateTime.UtcNow.AddHours(1),
                        query_string = Guid.NewGuid()
                    };

                    var makerCheckerLogInsertResult = MakerCheckerLogService.Insert(makerCheckerLog, CallerFormName, callerFormMethod, callerIpAddress);
                    if (makerCheckerLogInsertResult <= 0)
                    {
                        stopCheckFlag = true;
                        stopCheckMessage = "Unable to submit request for approval at the moment. Please try again later!";
                    }
                }

                if (!stopCheckFlag)
                {
                    //Task 1
                    new Task(() =>
                    {
                        var appActivity = new appactivity
                        {
                            audit_username = userData.username,
                            audit_fullname = $"{userData.person?.surname}, {userData.person?.first_name} {userData.person?.middle_name}",
                            audit_ipaddress = callerIpAddress,
                            audit_macaddress = callerMacAddress,
                            operation = $"{makerCheckerAction}".ToUpper(),
                            comments = $"{makerCheckerActionDetails}",
                            created_on = DateTime.Now,
                            created_by = "System"
                        };
                        AppActivityService.Insert(appActivity, CallerFormName, callerFormMethod, callerIpAddress);

                    }).Start();

                    //Task 2
                    new Task(() =>
                    {
                        if (!string.IsNullOrEmpty(checkerEmails.Trim().Replace(",", "")))
                        {
                            var templatePath = Server.MapPath(ConfigurationUtility.GetAppSettingValue("PendingItemsNotificationBodyTemplate"));
                            var mailBody = System.IO.File.ReadAllText(templatePath).Replace("{ItemType}", "Edited FINTECH Profile");
                            var mailRecipient = checkerEmails;
                            EmailLogManager.Log(EmailType.PendingItemsNotification, mailBody, mailRecipient, "", "", CallerFormName, callerFormMethod, callerIpAddress);
                        }

                    }).Start();

                    AlertUser("Successfully submitted request for approval. Your changes will be effected once approved.", AlertType.Success);
                }
                else
                {
                    AlertUser(stopCheckMessage, AlertType.Error);
                }

            }
            catch (Exception ex)
            {
                LogUtility.LogError(CallerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return RedirectToAction("ViewFintechs");
        }

        [HttpGet]
        [Route("view-pending-created-and-edited-fintech-profile")]
        public ActionResult ViewPendingCreatedAndEditedFintechProfile()
        {
            const string callerFormMethod = "HttpGet|ViewPendingCreatedAndEditedFintechProfile";

            try
            {
                var checkerEmails = "";
                if (!AuthorizeRequest(Request.Url.AbsolutePath, ref checkerEmails, CallerFormName, callerFormMethod, callerIpAddress))
                {
                    AlertUser("Unauthorised Access! You may not have been permitted to access the resource. Kindly login to re-authenticate. If issue persist, contact administrator.", AlertType.Error);
                    return RedirectToAction("Logout", "Account");
                }

                var logs = MakerCheckerLogService.GetWithMakerCheckerCategoryAndStatus((int)MakerCheckerCategory.Fintech, (int)MakerCheckerStatus.Initiated, CallerFormName, callerFormMethod, callerIpAddress);
                if (logs.Count > 0)
                {
                    MakerCheckerLogModels = logs.Select(log => new MakerCheckerLogModel
                    {
                        MakerCheckerCategoryId = Convert.ToString(log.maker_checker_category_id),
                        MakerCheckerTypeId = Convert.ToString(log.maker_checker_type_id),
                        ActionName = log.action_name,
                        ActionDetails = log.action_details,
                        MakerId = log.maker_username,
                        MakerFullname = log.maker_fullname,
                        MakerCheckerStatus = Convert.ToString(log.maker_checker_status),
                        DateMade = Convert.ToDateTime(log.date_made).ToString("dd-MM-yyyy hh:mm tt"),
                        CheckerId = !string.IsNullOrEmpty(log.checker_username) ? log.checker_username : "n/p",
                        CheckerFullname = !string.IsNullOrEmpty(log.checker_fullname) ? log.checker_fullname : "n/p",
                        CheckerRemarks = !string.IsNullOrEmpty(log.checker_remarks) ? log.checker_remarks : "n/p",
                        DateChecked = !string.IsNullOrEmpty(Convert.ToString(log.date_checked)) ? Convert.ToDateTime(log.date_checked).ToString("dd-MM-yyyy hh:mm tt") : "n/p",
                        QueryString = Convert.ToString(log.query_string),

                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(CallerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return View(MakerCheckerLogModels);
        }

        [HttpGet]
        [Route("review-pending-created-and-edited-fintech-profile")]
        public ActionResult ReviewPendingCreatedAndEditedFintechProfile(string q)
        {
            const string callerFormMethod = "HttpGet|ReviewPendingCreatedAndEditedFintechProfile";

            try
            {
                var log = MakerCheckerLogService.GetWithQueryString(q, CallerFormName, callerFormMethod, callerIpAddress);
                if (log != null)
                {
                    reviewMakerCheckerLogViewModel.MakerCheckerLogModel = new MakerCheckerLogModel
                    {
                        MakerCheckerCategoryId = Convert.ToString(log.maker_checker_category_id),
                        MakerCheckerTypeId = Convert.ToString(log.maker_checker_type_id),
                        ActionName = log.action_name,
                        ActionDetails = log.action_details,
                        ActionData = log.action_data,
                        MakerId = log.maker_username,
                        MakerFullname = log.maker_fullname,
                        MakerCheckerStatus = Convert.ToString(log.maker_checker_status),
                        DateMade = Convert.ToDateTime(log.date_made).ToString("dd-MM-yyyy hh:mm tt"),
                        CheckerId = log.checker_username,
                        CheckerFullname = log.checker_fullname,
                        CheckerRemarks = log.checker_remarks,
                        DateChecked = !string.IsNullOrEmpty(Convert.ToString(log.date_checked)) ? Convert.ToDateTime(log.date_checked).ToString("dd-MM-yyyy hh:mm tt") : "n/p",
                        QueryString = Convert.ToString(log.query_string)
                    };
                }

            }
            catch (Exception ex)
            {
                LogUtility.LogError(CallerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return View(reviewMakerCheckerLogViewModel);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("approve-pending-created-and-edited-fintech-profile")]
        public ActionResult ApprovePendingCreatedAndEditedFintechProfile(MakerCheckerLogModel model)
        {
            const string callerFormMethod = "HttpPost|ApprovePendingCreatedAndEditedFintechProfile";

            try
            {
                var checkerEmails = "";
                if (!AuthorizeRequest(Request.Url.AbsolutePath, ref checkerEmails, CallerFormName, callerFormMethod, callerIpAddress))
                {
                    AlertUser("Unauthorised Access! You may not have been permitted to access the resource. Kindly login to re-authenticate. If issue persist, contact administrator.", AlertType.Error);
                    return RedirectToAction("Logout", "Account");
                }

                var log = MakerCheckerLogService.GetWithQueryString(model.QueryString, CallerFormName, callerFormMethod, callerIpAddress);
                if (log == null)
                {
                    stopCheckFlag = true;
                    stopCheckMessage = "Unable to retrieve pending created/edited fintech profile at the moment, kindly try again later";
                }

                long applyResult = 0;
                if (!stopCheckFlag)
                {
                    switch (log.maker_checker_type_id)
                    {
                        case (int)MakerCheckerType.CreateNewFintechProfile:

                            var fintechInsertObject = JsonConvert.DeserializeObject<fintech>(log.action_data);
                            fintechInsertObject.approved_on = DateTime.UtcNow.AddHours(1);
                            fintechInsertObject.approved_by = $"{userData.person?.surname}, {userData.person?.first_name} {userData.person?.middle_name}";
                            fintechInsertObject.fintechonboardingdocuments.ToList().ForEach(document =>
                            {
                                document.approved_on = DateTime.UtcNow.AddHours(1);
                                document.approved_by = $"{userData.person?.surname}, {userData.person?.first_name} {userData.person?.middle_name}";
                            });

                            var fintechInsertResult = FintechService.Insert(fintechInsertObject, CallerFormName, callerFormMethod, callerIpAddress);
                            if (fintechInsertResult <= 0)
                            {
                                stopCheckFlag = true;
                                stopCheckMessage = "Unable to approve pending created fintech profile at the moment; Kindly try again later!";

                            }

                            fintech insertedFintech = null;
                            if (!stopCheckFlag)
                            {
                                insertedFintech = FintechService.GetWithFintechId(Convert.ToString(fintechInsertResult), CallerFormName, callerFormMethod, callerIpAddress);
                                if (insertedFintech == null)
                                {
                                    stopCheckFlag = true;
                                    stopCheckMessage = "Unable to fetch approved created fintech profile at the moment; Kindly try again later!";
                                }
                            }

                            if (!stopCheckFlag)
                            {
                                //RM
                                new Task(() =>
                                {
                                    var rmUser = new user
                                    {
                                        username = insertedFintech.relationship_manager_staff_id,
                                        authentication_type_id = (int)UserAccountAuthenticationType.ActiveDirectoryAuthentication,
                                        person_id = insertedFintech.relationship_manager_person_id,
                                        role_id = Convert.ToInt64(ConfigurationUtility.GetAppSettingValue("RelationshipManagerRoleId")),
                                        status_id = (int)UserAccountStatus.Active,
                                        created_on = DateTime.UtcNow.AddHours(1),
                                        created_by = $"{userData.person?.surname}, {userData.person?.first_name} {userData.person?.middle_name}",
                                        approved_on = DateTime.UtcNow.AddHours(1),
                                        approved_by = $"{userData.person?.surname}, {userData.person?.first_name} {userData.person?.middle_name}",
                                        query_string = Guid.NewGuid()
                                    };

                                    var rmUserInsertResult = UsersService.Insert(rmUser, CallerFormName, callerFormMethod, callerIpAddress);
                                    if (rmUserInsertResult > 0)
                                    {
                                        var relationshipManager = PersonService.GetWithPersonId(Convert.ToString(insertedFintech.relationship_manager_person_id), CallerFormName, callerFormMethod, callerIpAddress);
                                        var templatePath = Server.MapPath(ConfigurationUtility.GetAppSettingValue("AccountCreationNotificationBodyTemplate"));
                                        var mailBody = System.IO.File.ReadAllText(templatePath).Replace("{Name}", relationshipManager?.first_name).Replace("{Username}", rmUser.username).Replace("{Password}", "[Active Directory Password]");
                                        var mailRecipient = relationshipManager?.email_address;
                                        EmailLogManager.Log(EmailType.AccountCreationNotification, mailBody, mailRecipient, "", "", CallerFormName, callerFormMethod, callerIpAddress);
                                    }

                                }).Start();

                                applyResult = fintechInsertResult;
                            }
                            break;

                        case (int)MakerCheckerType.EditExistingFintechProfile:

                            var fintechUpdateObject = JsonConvert.DeserializeObject<fintech>(log.action_data);
                            var fintechUpdateResult = FintechService.Update(fintechUpdateObject, CallerFormName, callerFormMethod, callerIpAddress);
                            if (fintechUpdateResult <= 0)
                            {
                                stopCheckFlag = true;
                                stopCheckMessage = "Unable to approve pending edited fintech profile at the moment; Kindly try again later!";

                            }

                            fintech updatedFintech = null;
                            if (!stopCheckFlag)
                            {
                                updatedFintech = FintechService.GetWithFintechId(Convert.ToString(fintechUpdateResult), CallerFormName, callerFormMethod, callerIpAddress);
                                if (updatedFintech == null)
                                {
                                    stopCheckFlag = true;
                                    stopCheckMessage = "Unable to fetch approved edited fintech profile at the moment; Kindly try again later!";
                                }
                            }

                            if (!stopCheckFlag)
                            {
                                //RM
                                new Task(() =>
                                {
                                    var rmUser = new user
                                    {
                                        username = updatedFintech.relationship_manager_staff_id,
                                        authentication_type_id = (int)UserAccountAuthenticationType.ActiveDirectoryAuthentication,
                                        person_id = updatedFintech.relationship_manager_person_id,
                                        role_id = Convert.ToInt64(ConfigurationUtility.GetAppSettingValue("RelationshipManagerRoleId")),
                                        status_id = (int)UserAccountStatus.Active,
                                        created_on = DateTime.UtcNow.AddHours(1),
                                        created_by = $"{userData.person?.surname}, {userData.person?.first_name} {userData.person?.middle_name}",
                                        approved_on = DateTime.UtcNow.AddHours(1),
                                        approved_by = $"{userData.person?.surname}, {userData.person?.first_name} {userData.person?.middle_name}",
                                        query_string = Guid.NewGuid()
                                    };

                                    var rmUserInsertResult = UsersService.Insert(rmUser, CallerFormName, callerFormMethod, callerIpAddress);
                                    if (rmUserInsertResult > 0)
                                    {
                                        var relationshipManager = PersonService.GetWithPersonId(Convert.ToString(updatedFintech.relationship_manager_person_id), CallerFormName, callerFormMethod, callerIpAddress);
                                        var templatePath = Server.MapPath(ConfigurationUtility.GetAppSettingValue("AccountCreationNotificationBodyTemplate"));
                                        var mailBody = System.IO.File.ReadAllText(templatePath).Replace("{Name}", relationshipManager?.first_name).Replace("{Username}", rmUser.username).Replace("{Password}", "[Active Directory Password]");
                                        var mailRecipient = relationshipManager?.email_address;
                                        EmailLogManager.Log(EmailType.AccountCreationNotification, mailBody, mailRecipient, "", "", CallerFormName, callerFormMethod, callerIpAddress);
                                    }

                                }).Start();

                                applyResult = fintechUpdateResult;
                            }
                            break;

                        default:
                            stopCheckFlag = true;
                            stopCheckMessage = "Invalid Maker-Checker Type";
                            break;
                    }

                }

                if (!stopCheckFlag)
                {
                    if (applyResult <= 0)
                    {
                        stopCheckFlag = true;
                        stopCheckMessage = "Unable to approve pending created/edited fintech profile at the moment. Kindly try again later!";
                    }
                }

                long logUpdateResult = 0;
                if (!stopCheckFlag)
                {
                    log.maker_checker_status = (int)MakerCheckerStatus.Approved;
                    log.checker_person_id = userData.person_id;
                    log.checker_username = userData.username;
                    log.checker_fullname = $"{userData.person?.surname}, {userData.person?.first_name} {userData.person?.middle_name}";
                    log.checker_remarks = model.CheckerRemarks;
                    log.date_checked = DateTime.UtcNow.AddHours(1);

                    logUpdateResult = MakerCheckerLogService.Update(log, CallerFormName, callerFormMethod, callerIpAddress);
                    if (logUpdateResult <= 0)
                    {
                        stopCheckFlag = true;
                        stopCheckMessage = "Unable to approve pending created/edited fintech profile at the moment; maker-checker log could not be updated. Try again later!";
                    }
                }

                if (!stopCheckFlag)
                {
                    //Task 1
                    new Task(() =>
                    {
                        var appActivity = new appactivity
                        {
                            audit_username = userData.username,
                            audit_fullname = $"{userData.person?.surname}, {userData.person?.first_name} {userData.person?.middle_name}",
                            audit_ipaddress = callerIpAddress,
                            audit_macaddress = callerMacAddress,
                            operation = $"Approved - {log.action_name}".ToUpper(),
                            comments = $"Approved - {log.action_details}",
                            created_on = DateTime.Now,
                            created_by = "System"
                        };
                        AppActivityService.Insert(appActivity, CallerFormName, callerFormMethod, callerIpAddress);

                    }).Start();

                    //Task 2
                    new Task(() =>
                    {
                        var makerFirstname = "";
                        var makerEmailAddress = "";
                        var makerPerson = PersonService.GetWithPersonId(Convert.ToString(log.maker_person_id), CallerFormName, callerFormMethod, callerIpAddress);
                        if (makerPerson != null)
                        {
                            makerFirstname = makerPerson.first_name;
                            makerEmailAddress = makerPerson.email_address;
                        }

                        if (!string.IsNullOrEmpty(makerEmailAddress))
                        {
                            var templatePath = Server.MapPath(ConfigurationUtility.GetAppSettingValue("ApprovalNotificationBodyTemplate"));
                            var mailBody = System.IO.File.ReadAllText(templatePath).Replace("{Name}", makerFirstname).Replace("{ActionName}", log.action_name).Replace("{ActionDetails}", log.action_details).Replace("{ApprovedBy}", log.checker_fullname).Replace("{ApprovedOn}", Convert.ToDateTime(log.date_checked).ToString("dd-MM-yyyy hh:mm tt"));
                            var mailRecipient = makerEmailAddress;
                            EmailLogManager.Log(EmailType.ApprovalNotification, mailBody, mailRecipient, "", "", CallerFormName, callerFormMethod, callerIpAddress);
                        }

                    }).Start();

                    AlertUser("Successfully approved request. Your changes have been effected.", AlertType.Success);
                }
                else
                {
                    AlertUser(stopCheckMessage, AlertType.Error);
                    return RedirectToAction("ReviewPendingCreatedAndEditedFintechProfile", new { q = model.QueryString });
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(CallerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return RedirectToAction("ViewPendingCreatedAndEditedFintechProfile");
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("reject-pending-created-and-edited-fintech-profile")]
        public ActionResult RejectPendingCreatedAndEditedFintechProfile(MakerCheckerLogModel model)
        {
            const string callerFormMethod = "HttpPost|RejectPendingCreatedAndEditedFintechProfile";

            try
            {
                var checkerEmails = "";
                if (!AuthorizeRequest(Request.Url.AbsolutePath, ref checkerEmails, CallerFormName, callerFormMethod, callerIpAddress))
                {
                    AlertUser("Unauthorised Access! You may not have been permitted to access the resource. Kindly login to re-authenticate. If issue persist, contact administrator.", AlertType.Error);
                    return RedirectToAction("Logout", "Account");
                }

                var log = MakerCheckerLogService.GetWithQueryString(model.QueryString, CallerFormName, callerFormMethod, callerIpAddress);
                if (log == null)
                {
                    stopCheckFlag = true;
                    stopCheckMessage = "Unable to retrieve pending created/edited fintech profile at the moment, kindly try again later";
                }

                if (!stopCheckFlag)
                {
                    log.maker_checker_status = (int)MakerCheckerStatus.Rejected;
                    log.checker_person_id = userData.person_id;
                    log.checker_username = userData.username;
                    log.checker_fullname = $"{userData.person?.surname}, {userData.person?.first_name} {userData.person?.middle_name}";
                    log.checker_remarks = model.CheckerRemarks;
                    log.date_checked = DateTime.UtcNow.AddHours(1);

                    var logUpdateResult = MakerCheckerLogService.Update(log, CallerFormName, callerFormMethod, callerIpAddress);
                    if (logUpdateResult <= 0)
                    {
                        stopCheckFlag = true;
                        stopCheckMessage = "Unable to reject pending created/edited fintech profile at the moment; maker-checker log could not be updated. Try again later!";
                    }
                }

                if (!stopCheckFlag)
                {
                    //Task 1
                    new Task(() =>
                    {
                        var appActivity = new appactivity
                        {
                            audit_username = userData.username,
                            audit_fullname = $"{userData.person?.surname}, {userData.person?.first_name} {userData.person?.middle_name}",
                            audit_ipaddress = callerIpAddress,
                            audit_macaddress = callerMacAddress,
                            operation = $"Rejected - {log.action_name}".ToUpper(),
                            comments = $"Rejected - {log.action_details} with remarks- {model.CheckerRemarks}",
                            created_on = DateTime.Now,
                            created_by = "System"
                        };
                        AppActivityService.Insert(appActivity, CallerFormName, callerFormMethod, callerIpAddress);

                    }).Start();

                    //Task 2
                    new Task(() =>
                    {
                        var makerFirstname = "";
                        var makerEmailAddress = "";
                        var makerPerson = PersonService.GetWithPersonId(Convert.ToString(log.maker_person_id), CallerFormName, callerFormMethod, callerIpAddress);
                        if (makerPerson != null)
                        {
                            makerFirstname = makerPerson.first_name;
                            makerEmailAddress = makerPerson.email_address;
                        }

                        if (!string.IsNullOrEmpty(makerEmailAddress))
                        {
                            var templatePath = Server.MapPath(ConfigurationUtility.GetAppSettingValue("RejectionNotificationBodyTemplate"));
                            var mailBody = System.IO.File.ReadAllText(templatePath).Replace("{Name}", makerFirstname).Replace("{ActionName}", log.action_name).Replace("{ActionDetails}", log.action_details).Replace("{RejectedBy}", log.checker_fullname).Replace("{RejectedOn}", Convert.ToDateTime(log.date_checked).ToString("dd-MM-yyyy hh:mm tt")).Replace("{RejectionReason}", log.checker_remarks);
                            var mailRecipient = makerEmailAddress;
                            EmailLogManager.Log(EmailType.RejectionNotification, mailBody, mailRecipient, "", "", CallerFormName, callerFormMethod, callerIpAddress);
                        }

                    }).Start();

                    AlertUser($"Successfully rejected request with remarks '{model.CheckerRemarks}'", AlertType.Success);
                }
                else
                {
                    AlertUser(stopCheckMessage, AlertType.Error);
                    return RedirectToAction("ReviewPendingCreatedAndEditedFintechProfile", new { q = model.QueryString });
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(CallerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return RedirectToAction("ViewPendingCreatedAndEditedFintechProfile");
        }

        [Route("view-fintech-profile")]
        public ActionResult ViewFintechProfile(string q)
        {
            const string callerFormMethod = "HttpGet|ViewFintechProfile";

            try
            {
                var checkerEmails = "";
                if (!AuthorizeRequest(Request.Url.AbsolutePath, ref checkerEmails, CallerFormName, callerFormMethod, callerIpAddress))
                {
                    AlertUser("Unauthorised Access! You may not have been permitted to access the resource. Kindly login to re-authenticate. If issue persist, contact administrator.", AlertType.Error);
                    return RedirectToAction("Logout", "Account");
                }


                var fintech = FintechService.GetWithUrlQueryString(q, CallerFormName, callerFormMethod, callerIpAddress);
                if (fintech != null)
                {
                    if (userData.person.person_type_id.Equals((int)PersonType.FintechContactPerson))
                    {
                        var fintechId = FintechContactPersonService.GetWithContactPersonId(Convert.ToString(userData.person_id), CallerFormName, callerFormMethod, callerIpAddress)?.fintech_id;
                        if (!fintech.id.Equals(fintechId))
                        {
                            return RedirectToAction("ViewFintechs");
                        }
                    }
                    if (userData.person.person_type_id.Equals((int)PersonType.BankRelationshipManager))
                    {
                        var fintechId = FintechService.GetWithRelationshipManagerPersonId(Convert.ToString(userData.person_id), CallerFormName, callerFormMethod, callerIpAddress)?.id;
                        if (!fintech.id.Equals(fintechId))
                        {
                            return RedirectToAction("ViewFintechs");
                        }
                    }

                    viewFintechProfileViewModel.FintechModel = new FintechModel
                    {
                        CorporateName = fintech.corporate_name,
                        OfficialEmailAddress = fintech.official_email_address,
                        HeadOfficeAddress = fintech.head_office_address,
                        RelationshipManagerStaffId = fintech.relationship_manager_staff_id,

                        RelationshipManagerPerson = PersonService.GetAll(CallerFormName, callerFormMethod, callerIpAddress).Where(person => person.id.Equals(fintech.relationship_manager_person_id)).Select(person => new PersonModel
                        {
                            Surname = person.surname,
                            Firstname = person.first_name,
                            Middlename = person.middle_name,
                            MobileNumber = person.mobile_number,
                            EmailAddress = person.email_address

                        }).FirstOrDefault(),

                        RelationshipManagerSolId = fintech.relationship_manager_sol_id,
                        RelationshipManagerSolName = fintech.relationship_manager_sol_name,
                        RelationshipManagerSolAddress = fintech.relationship_manager_sol_address,
                        AccountNumber = fintech.account_number,
                        AccountName = fintech.account_name,
                        FinacleTermId = fintech.finacle_term_id,
                        FeeScale = fintech.fee_scale,
                        ScaleValue = fintech.scale_value.ToString("N"),
                        CapAmount = fintech.cap_amount.ToString("N"),
                        CreatedOn = Convert.ToDateTime(fintech.created_on).ToString("dd-MM-yyyy hh:mm tt"),
                        CreatedBy = fintech.created_by,
                        ApprovedOn = !string.IsNullOrEmpty(Convert.ToString(fintech.approved_on)) ? Convert.ToDateTime(fintech.approved_on).ToString("dd-MM-yyyy hh:mm tt") : "n/p",
                        ApprovedBy = fintech.approved_by,
                        LastModifiedOn = !string.IsNullOrEmpty(Convert.ToString(fintech.last_modified_on)) ? Convert.ToDateTime(fintech.last_modified_on).ToString("dd-MM-yyyy hh:mm tt") : "n/p",
                        LastModifiedBy = fintech.last_modified_by,
                        UrlQueryString = Convert.ToString(fintech.query_string)
                    };

                    viewFintechProfileViewModel.FintechOnBoardingDocumentsModels = FintechOnBoardingDocumentsService.GetWithFintechId(Convert.ToString(fintech.id), CallerFormName, callerFormMethod, callerIpAddress).Select(document => new FintechOnBoardingDocumentsModel
                    {
                        DocumentName = CacheData.Onboardingdocuments.FirstOrDefault(c => c.id.Equals(document.document_id)).document_name,
                        DocumentSavePath = document.document_save_path

                    }).ToList();

                    viewFintechProfileViewModel.FintechContactPersons = (from fcp in FintechContactPersonService.GetWithFintechId(Convert.ToString(fintech.id), CallerFormName, callerFormMethod, callerIpAddress)
                                                                         join p in PersonService.GetAll(CallerFormName, callerFormMethod, callerIpAddress) on fcp.person_id equals p.id
                                                                         select new PersonModel
                                                                         {
                                                                             Surname = p.surname,
                                                                             Firstname = p.first_name,
                                                                             Middlename = p.middle_name,
                                                                             MobileNumber = p.mobile_number,
                                                                             EmailAddress = p.email_address

                                                                         }).ToList();
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(CallerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return View(viewFintechProfileViewModel);
        }

        [HttpGet]
        [Route("view-fintech-contact-persons")]
        public ActionResult ViewFintechContactPersons(string q)
        {
            const string callerFormMethod = "HttpGet|ViewFintechContactPersons";

            try
            {
                var fintech = FintechService.GetWithUrlQueryString(q, CallerFormName, callerFormMethod, callerIpAddress);
                if (fintech != null)
                {
                    viewFintechContactPersonsViewModel.FintechName = fintech.corporate_name.ToUpper();
                    viewFintechContactPersonsViewModel.FintechContactPersons = (from fcp in FintechContactPersonService.GetAll(CallerFormName, callerFormMethod, callerIpAddress)
                                                                                join p in PersonService.GetAll(CallerFormName, callerFormMethod, callerIpAddress) on fcp.person_id equals p.id
                                                                                where fcp.fintech_id.Equals(fintech.id)
                                                                                select new PersonModel
                                                                                {
                                                                                    Surname = p.surname,
                                                                                    Firstname = p.first_name,
                                                                                    Middlename = p.middle_name,
                                                                                    MobileNumber = !string.IsNullOrEmpty(p.mobile_number) ? p.mobile_number : "n/p",
                                                                                    EmailAddress = p.email_address,
                                                                                    Passport = p.passport,
                                                                                    CreatedOn = Convert.ToDateTime(p.created_on).ToString("dd-MM-yyyy hh:mm tt"),
                                                                                    CreatedBy = p.created_by,
                                                                                    LastModifiedOn = !string.IsNullOrEmpty(Convert.ToString(p.last_modified_on)) ? Convert.ToDateTime(p.last_modified_on).ToString("dd-MM-yyyy hh:mm tt") : "n/p",
                                                                                    LastModifiedBy = p.last_modified_by,
                                                                                    UrlQueryString = Convert.ToString(p.query_string)

                                                                                }).ToList();
                }
                
            }
            catch (Exception ex)
            {
                LogUtility.LogError(CallerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return View(viewFintechContactPersonsViewModel);
        }

        [HttpGet]
        [Route("add-new-fintech-contact-person")]
        public ActionResult AddNewFintechContactPerson(string q)
        {
            const string callerFormMethod = "HttpGet|AddNewFintechContactPerson";

            try
            {
                var fintech = FintechService.GetWithUrlQueryString(q, CallerFormName, callerFormMethod, callerIpAddress);
                if (fintech != null)
                {
                    createFintechContactPersonViewModel.FintechName = fintech.corporate_name;
                    createFintechContactPersonViewModel.FintechUrlQueryString = q;
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(CallerFormName, callerFormMethod, callerIpAddress, ex);
            }
            
            return View(createFintechContactPersonViewModel);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("add-new-fintech-contact-person")]
        public ActionResult AddNewFintechContactPerson(string fintechUrlQueryString, PersonModel model)
        {
            const string callerFormMethod = "HttpPost|AddNewFintechContactPerson";

            try
            {
                var checkerEmails = "";
                if (!AuthorizeRequest(Request.Url.AbsolutePath, ref checkerEmails, CallerFormName, callerFormMethod, callerIpAddress))
                {
                    AlertUser("Unauthorised Access! You may not have been permitted to access the resource. Kindly login to re-authenticate. If issue persist, contact administrator.", AlertType.Error);
                    return RedirectToAction("Logout", "Account");
                }

                if (!ValidationUtility.IsValidTextInput(model.Surname))
                {
                    ModelState.AddModelError("Surname", "Surname field is required");
                }

                if (!ValidationUtility.IsValidTextInput(model.Firstname))
                {
                    ModelState.AddModelError("Firstname", "Firstname field is required");
                }

                if (!ValidationUtility.IsValidEmail(model.EmailAddress))
                {
                    ModelState.AddModelError("EmailAddress", "Email Address field does not meet the required data format");
                }

                if (!ModelState.IsValid)
                {
                    var errorMessages = string.Join("<br/>", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
                    stopCheckFlag = true;
                    stopCheckMessage = errorMessages;
                }

                fintech fintech = null;
                if (!stopCheckFlag)
                {
                    fintech = FintechService.GetWithUrlQueryString(fintechUrlQueryString, CallerFormName, callerFormMethod, callerIpAddress);
                    if (fintech == null)
                    {
                        stopCheckFlag = true;
                        stopCheckMessage = "Unable to initiate request; no fintech profile found with the search parameter";
                    }
                }

                fintechcontactperson fintechcontactperson = null;
                if (!stopCheckFlag)
                {
                    fintechcontactperson = FintechContactPersonService.GetWithContactPersonEmailAddress(model.EmailAddress, CallerFormName, callerFormMethod, callerIpAddress);
                    if (fintechcontactperson != null)
                    {
                        stopCheckFlag = true;

                        if (fintechcontactperson.fintech_id.Equals(fintech.id))
                        {
                            stopCheckMessage = "Unable to initiate request; contact person already exist for this fintech";
                        }
                        else
                        {
                            stopCheckMessage = "Unable to initiate request; contact person already exists for another fintech";
                        }
                    }
                }

                var makerCheckerAction = "FINTECH Contact Person Creation";
                var makerCheckerActionDetails = $"Add New Fintech Contact Person '{model.Fullname}' with Email Address '{model.EmailAddress}' to {fintech.corporate_name}";

                if (!stopCheckFlag)
                {
                    fintechcontactperson = new fintechcontactperson
                    {
                        fintech_id = fintech.id,
                        person = new person
                        {
                            surname = model.Surname,
                            first_name = model.Firstname,
                            middle_name = model.Middlename,
                            email_address = model.EmailAddress,
                            person_type_id = (int)PersonType.FintechContactPerson,
                            created_on = DateTime.UtcNow.AddHours(1),
                            created_by = $"{userData.person?.surname}, {userData.person?.first_name} {userData.person?.middle_name}",
                            query_string = Guid.NewGuid()
                        },
                        created_on = DateTime.UtcNow.AddHours(1),
                        created_by = $"{userData.person?.surname}, {userData.person?.first_name} {userData.person?.middle_name}",
                        query_string = Guid.NewGuid()
                    };

                    var makerCheckerActionData = JsonConvert.SerializeObject(fintechcontactperson);
                    var makerCheckerLog = new makercheckerlog
                    {
                        maker_checker_category_id = (int)MakerCheckerCategory.FintechContactPerson,
                        maker_checker_type_id = (int)MakerCheckerType.CreateNewFintechContactPerson,
                        action_name = makerCheckerAction,
                        action_details = makerCheckerActionDetails,
                        action_data = makerCheckerActionData,
                        maker_person_id = userData.person_id,
                        maker_username = userData.username,
                        maker_fullname = $"{userData.person?.surname}, {userData.person?.first_name} {userData.person?.middle_name}",
                        maker_checker_status = (int)MakerCheckerStatus.Initiated,
                        date_made = DateTime.UtcNow.AddHours(1),
                        query_string = Guid.NewGuid()
                    };

                    var makerCheckerLogInsertResult = MakerCheckerLogService.Insert(makerCheckerLog, CallerFormName, callerFormMethod, callerIpAddress);
                    if (makerCheckerLogInsertResult <= 0)
                    {
                        stopCheckFlag = true;
                        stopCheckMessage = "Unable to submit request for approval at the moment. Please try again later!";
                    }
                }

                if (!stopCheckFlag)
                {
                    //Task 1
                    new Task(() =>
                    {
                        var appActivity = new appactivity
                        {
                            audit_username = userData.username,
                            audit_fullname = $"{userData.person?.surname}, {userData.person?.first_name} {userData.person?.middle_name}",
                            audit_ipaddress = callerIpAddress,
                            audit_macaddress = callerMacAddress,
                            operation = $"Initiated - {makerCheckerAction}".ToUpper(),
                            comments = $"Initiated - {makerCheckerActionDetails}",
                            created_on = DateTime.Now,
                            created_by = "System"
                        };

                        AppActivityService.Insert(appActivity, CallerFormName, callerFormMethod, callerIpAddress);

                    }).Start();

                    //Task 2
                    new Task(() =>
                    {
                        if (!string.IsNullOrEmpty(checkerEmails.Trim().Replace(",", "")))
                        {
                            var templatePath = Server.MapPath(ConfigurationUtility.GetAppSettingValue("PendingItemsNotificationBodyTemplate"));
                            var mailBody = System.IO.File.ReadAllText(templatePath).Replace("{ItemType}", "Fintech Contact Person Creation");
                            var mailRecipient = checkerEmails;
                            EmailLogManager.Log(EmailType.PendingItemsNotification, mailBody, mailRecipient, "", "", CallerFormName, callerFormMethod, callerIpAddress);
                        }

                    }).Start();

                    AlertUser("Successfully submitted request for approval. Your changes will be effected once approved.", AlertType.Success);
                }
                else
                {
                    AlertUser(stopCheckMessage, AlertType.Error);
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(CallerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return RedirectToAction("AddNewFintechContactPerson", new { q = fintechUrlQueryString });
        }

        [HttpGet]
        [Route("edit-existing-fintech-contact-person")]
        public ActionResult EditExistingFintechContactPerson(string q)
        {
            const string callerFormMethod = "HttpGet|EditExistingFintechContactPerson";

            try
            {
                var person = PersonService.GetWithUrlQueryString(q, CallerFormName, callerFormMethod, callerIpAddress);
                if (person == null)
                {
                    stopCheckFlag = true;
                    stopCheckMessage = "No Person Profile Found with the Search Parameter";
                }

                if (!stopCheckFlag)
                {
                    editFintechContactPersonViewModel.PersonModel = new PersonModel
                    {
                        Surname = person.surname,
                        Firstname = person.first_name,
                        Middlename = person.middle_name,
                        UrlQueryString = q
                    };
                }
                else
                {
                    AlertUser(stopCheckMessage, AlertType.Error);
                    return RedirectToAction("ViewFintechs");
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(CallerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return View(editFintechContactPersonViewModel);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("edit-existing-fintech-contact-person")]
        public ActionResult EditExistingFintechContactPerson(PersonModel model)
        {
            const string callerFormMethod = "HttpPost|EditExistingFintechContactPerson";

            try
            {
                var checkerEmails = "";
                if (!AuthorizeRequest(Request.Url.AbsolutePath, ref checkerEmails, CallerFormName, callerFormMethod, callerIpAddress))
                {
                    AlertUser("Unauthorised Access! You may not have been permitted to access the resource. Kindly login to re-authenticate. If issue persist, contact administrator.", AlertType.Error);
                    return RedirectToAction("Logout", "Account");
                }

                if (!ValidationUtility.IsValidTextInput(model.Surname))
                {
                    ModelState.AddModelError("Surname", "Surname field is required");
                }

                if (!ValidationUtility.IsValidTextInput(model.Firstname))
                {
                    ModelState.AddModelError("Firstname", "Firstname field is required");
                }

                if (!ModelState.IsValid)
                {
                    var errorMessages = string.Join("<br/>", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
                    stopCheckFlag = true;
                    stopCheckMessage = errorMessages;
                }

                var makerCheckerAction = "FINTECH Contact Person Editing";
                var makerCheckerActionDetails = "";

                person person = null;
                if (!stopCheckFlag)
                {
                    person = PersonService.GetWithUrlQueryString(model.UrlQueryString, CallerFormName, callerFormMethod, callerIpAddress);
                    if (person == null)
                    {
                        stopCheckFlag = true;
                        stopCheckMessage = "Unable to retrieve existing person profile at the moment, kindly try again later";
                    }
                }
                
                if (!stopCheckFlag)
                {
                    if (!person.surname.Equals(model.Surname))
                    {
                        makerCheckerActionDetails += $"Edit Surname:{person.surname} From '{person.surname}' to '{model.Surname}'|";
                    }

                    if (!person.first_name.Equals(model.Firstname))
                    {
                        makerCheckerActionDetails += $"Edit Firstname:{person.first_name} From '{person.first_name}' to '{model.Firstname}'|";
                    }

                    if (!string.IsNullOrEmpty(person.middle_name))
                    {
                        if (!person.middle_name.Equals(model.Middlename))
                        {
                            makerCheckerActionDetails += $"Edit Middlename:{person.middle_name} From '{person.middle_name}' to '{model.Middlename}'|";
                        }
                    }
                    else
                    {
                        makerCheckerActionDetails += $"Edit Middlename to '{model.Middlename}'|";
                    }
                    makerCheckerActionDetails = makerCheckerActionDetails.Substring(0, makerCheckerActionDetails.Length - 1);

                    //
                    if (string.IsNullOrEmpty(makerCheckerActionDetails))
                    {
                        stopCheckFlag = true;
                        stopCheckMessage = "Unable to initiate changes because no modification observed";
                    }
                }

                if (!stopCheckFlag)
                {
                    person.surname = model.Surname;
                    person.first_name = model.Firstname;
                    person.middle_name = model.Middlename;
                    
                    var makerCheckerActionData = JsonConvert.SerializeObject(person);
                    var makerCheckerLog = new makercheckerlog
                    {
                        maker_checker_category_id = (int)MakerCheckerCategory.FintechContactPerson,
                        maker_checker_type_id = (int)MakerCheckerType.EditExistingFintechContactPerson,
                        action_name = makerCheckerAction,
                        action_details = makerCheckerActionDetails,
                        action_data = makerCheckerActionData,
                        maker_person_id = userData.person_id,
                        maker_username = userData.username,
                        maker_fullname = $"{userData.person?.surname}, {userData.person?.first_name} {userData.person?.middle_name}",
                        maker_checker_status = (int)MakerCheckerStatus.Initiated,
                        date_made = DateTime.UtcNow.AddHours(1),
                        query_string = Guid.NewGuid()
                    };

                    var makerCheckerLogInsertResult = MakerCheckerLogService.Insert(makerCheckerLog, CallerFormName, callerFormMethod, callerIpAddress);
                    if (makerCheckerLogInsertResult <= 0)
                    {
                        stopCheckFlag = true;
                        stopCheckMessage = "Unable to submit request for approval at the moment. Please try again later!";
                    }
                }
                
                if (!stopCheckFlag)
                {
                    //Task 1
                    new Task(() =>
                    {
                        var appActivity = new appactivity
                        {
                            audit_username = userData.username,
                            audit_fullname = $"{userData.person?.surname}, {userData.person?.first_name} {userData.person?.middle_name}",
                            audit_ipaddress = callerIpAddress,
                            audit_macaddress = callerMacAddress,
                            operation = $"Initiated - {makerCheckerAction}".ToUpper(),
                            comments = $"Initiated - {makerCheckerActionDetails}",
                            created_on = DateTime.Now,
                            created_by = "System"
                        };
                        AppActivityService.Insert(appActivity, CallerFormName, callerFormMethod, callerIpAddress);

                    }).Start();

                    //Task 2
                    new Task(() =>
                    {
                        if (!string.IsNullOrEmpty(checkerEmails.Trim().Replace(",", "")))
                        {
                            var templatePath = Server.MapPath(ConfigurationUtility.GetAppSettingValue("PendingItemsNotificationBodyTemplate"));
                            var mailBody = System.IO.File.ReadAllText(templatePath).Replace("{ItemType}", "Fintech Contact Person Editing");
                            var mailRecipient = checkerEmails;
                            EmailLogManager.Log(EmailType.PendingItemsNotification, mailBody, mailRecipient, "", "", CallerFormName, callerFormMethod, callerIpAddress);
                        }

                    }).Start();

                    AlertUser("Successfully submitted request for approval. Your changes will be effected once approved.", AlertType.Success);
                }
                else
                {
                    AlertUser(stopCheckMessage, AlertType.Error);
                    return RedirectToAction("EditExistingFintechContactPerson", new { q = model.UrlQueryString });
                }

            }
            catch (Exception ex)
            {
                LogUtility.LogError(CallerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return RedirectToAction("ViewFintechs");
        }

        [HttpGet]
        [Route("view-pending-created-fintech-contact-person")]
        public ActionResult ViewPendingCreatedAndEditedFintechContactPerson()
        {
            const string callerFormMethod = "HttpGet|ViewPendingCreatedAndEditedFintechContactPerson";

            try
            {
                var checkerEmails = "";
                if (!AuthorizeRequest(Request.Url.AbsolutePath, ref checkerEmails, CallerFormName, callerFormMethod, callerIpAddress))
                {
                    AlertUser("Unauthorised Access! You may not have been permitted to access the resource. Kindly login to re-authenticate. If issue persist, contact administrator.", AlertType.Error);
                    return RedirectToAction("Logout", "Account");
                }

                var logs = MakerCheckerLogService.GetWithMakerCheckerCategoryAndStatus((int)MakerCheckerCategory.FintechContactPerson, (int)MakerCheckerStatus.Initiated, CallerFormName, callerFormMethod, callerIpAddress);
                if (logs.Count > 0)
                {
                    MakerCheckerLogModels = logs.Select(log => new MakerCheckerLogModel
                    {
                        MakerCheckerCategoryId = Convert.ToString(log.maker_checker_category_id),
                        MakerCheckerTypeId = Convert.ToString(log.maker_checker_type_id),
                        ActionName = log.action_name,
                        ActionDetails = log.action_details,
                        MakerId = log.maker_username,
                        MakerFullname = log.maker_fullname,
                        MakerCheckerStatus = Convert.ToString(log.maker_checker_status),
                        DateMade = Convert.ToDateTime(log.date_made).ToString("dd-MM-yyyy hh:mm tt"),
                        CheckerId = !string.IsNullOrEmpty(log.checker_username) ? log.checker_username : "n/p",
                        CheckerFullname = !string.IsNullOrEmpty(log.checker_fullname) ? log.checker_fullname : "n/p",
                        CheckerRemarks = !string.IsNullOrEmpty(log.checker_remarks) ? log.checker_remarks : "n/p",
                        DateChecked = !string.IsNullOrEmpty(Convert.ToString(log.date_checked)) ? Convert.ToDateTime(log.date_checked).ToString("dd-MM-yyyy hh:mm tt") : "n/p",
                        QueryString = Convert.ToString(log.query_string),

                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(CallerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return View(MakerCheckerLogModels);
        }

        [HttpGet]
        [Route("review-pending-created-fintech-contact-person")]
        public ActionResult ReviewPendingCreatedAndEditedFintechContactPerson(string q)
        {
            const string callerFormMethod = "HttpGet|ReviewPendingCreatedAndEditedFintechContactPerson";

            try
            {
                var log = MakerCheckerLogService.GetWithQueryString(q, CallerFormName, callerFormMethod, callerIpAddress);
                if (log != null)
                {
                    reviewMakerCheckerLogViewModel.MakerCheckerLogModel = new MakerCheckerLogModel
                    {
                        MakerCheckerCategoryId = Convert.ToString(log.maker_checker_category_id),
                        MakerCheckerTypeId = Convert.ToString(log.maker_checker_type_id),
                        ActionName = log.action_name,
                        ActionDetails = log.action_details,
                        ActionData = log.action_data,
                        MakerId = log.maker_username,
                        MakerFullname = log.maker_fullname,
                        MakerCheckerStatus = Convert.ToString(log.maker_checker_status),
                        DateMade = Convert.ToDateTime(log.date_made).ToString("dd-MM-yyyy hh:mm tt"),
                        CheckerId = log.checker_username,
                        CheckerFullname = log.checker_fullname,
                        CheckerRemarks = log.checker_remarks,
                        DateChecked = !string.IsNullOrEmpty(Convert.ToString(log.date_checked)) ? Convert.ToDateTime(log.date_checked).ToString("dd-MM-yyyy hh:mm tt") : "n/p",
                        QueryString = Convert.ToString(log.query_string)
                    };
                }

            }
            catch (Exception ex)
            {
                LogUtility.LogError(CallerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return View(reviewMakerCheckerLogViewModel);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("approve-pending-created-and-edited-fintech-contact-person")]
        public ActionResult ApprovePendingCreatedAndEditedFintechContactPerson(MakerCheckerLogModel model)
        {
            const string callerFormMethod = "HttpPost|ApprovePendingCreatedAndEditedFintechContactPerson";

            try
            {
                var checkerEmails = "";
                if (!AuthorizeRequest(Request.Url.AbsolutePath, ref checkerEmails, CallerFormName, callerFormMethod, callerIpAddress))
                {
                    AlertUser("Unauthorised Access! You may not have been permitted to access the resource. Kindly login to re-authenticate. If issue persist, contact administrator.", AlertType.Error);
                    return RedirectToAction("Logout", "Account");
                }

                var log = MakerCheckerLogService.GetWithQueryString(model.QueryString, CallerFormName, callerFormMethod, callerIpAddress);
                if (log == null)
                {
                    stopCheckFlag = true;
                    stopCheckMessage = "Unable to retrieve pending created/edited fintech contact person at the moment, kindly try again later";
                }

                long applyResult = 0;
                if (!stopCheckFlag)
                {
                    switch (log.maker_checker_type_id)
                    {
                        case (int)MakerCheckerType.CreateNewFintechContactPerson:

                            var fintechContactPersonInsertObject = JsonConvert.DeserializeObject<fintechcontactperson>(log.action_data);
                            var fintechContactPersonInsertResult = FintechContactPersonService.Insert(fintechContactPersonInsertObject, CallerFormName, callerFormMethod, callerIpAddress);
                            if (fintechContactPersonInsertResult <= 0)
                            {
                                stopCheckFlag = true;
                                stopCheckMessage = "Unable to approve pending created fintech contact person at the moment; Kindly try again later!";

                            }

                            person person = null;
                            if (!stopCheckFlag)
                            {
                                person = PersonService.GetWithFintechContactPersonId(Convert.ToString(fintechContactPersonInsertResult), CallerFormName, callerFormMethod, callerIpAddress);
                                if (person == null)
                                {
                                    stopCheckFlag = true;
                                    stopCheckMessage = "Unable to fetch approved created fintech contact person at the moment; Kindly try again later!";
                                }
                            }

                            var generatedPassword = "";
                            if (!stopCheckFlag)
                            {
                                generatedPassword = new Password(includeLowerCase, includeUpperCase, includeNumeric, includeSpecial, passwordLength).Next();
                                new Task(() =>
                                {
                                    var fintechUser = new user
                                    {
                                        username = fintechContactPersonInsertObject.person.email_address,
                                        authentication_type_id = (int)UserAccountAuthenticationType.LocalAccountAuthentication,
                                        local_password = EncryptionUtility.Encrypt(generatedPassword, encryptionKey),
                                        //password_expiry_date = DateTime.UtcNow.AddDays(Convert.ToDouble(ConfigurationUtility.GetAppSettingValue("PasswordValidityPeriod"))),
                                        person_id = person.id,
                                        role_id = Convert.ToInt64(ConfigurationUtility.GetAppSettingValue("FINTECHContactPersonRoleId")),
                                        status_id = (int)UserAccountStatus.Active,
                                        created_on = DateTime.UtcNow.AddHours(1),
                                        created_by = $"{userData.person?.surname}, {userData.person?.first_name} {userData.person?.middle_name}",
                                        approved_on = DateTime.UtcNow.AddHours(1),
                                        approved_by = $"{userData.person?.surname}, {userData.person?.first_name} {userData.person?.middle_name}",
                                        query_string = Guid.NewGuid()
                                    };

                                    var fintechUserInsertResult = UsersService.Insert(fintechUser, CallerFormName, callerFormMethod, callerIpAddress);
                                    if (fintechUserInsertResult > 0)
                                    {
                                        var templatePath = Server.MapPath(ConfigurationUtility.GetAppSettingValue("AccountCreationNotificationBodyTemplate"));
                                        var mailBody = System.IO.File.ReadAllText(templatePath).Replace("{Name}", person.first_name).Replace("{Username}", fintechUser.username).Replace("{Password}", generatedPassword);
                                        var mailRecipient = fintechUser.username;
                                        EmailLogManager.Log(EmailType.AccountCreationNotification, mailBody, mailRecipient, "", "", CallerFormName, callerFormMethod, callerIpAddress);
                                    }

                                }).Start();

                                applyResult = fintechContactPersonInsertResult;
                            }
                            break;

                        case (int)MakerCheckerType.EditExistingFintechContactPerson:

                            var personUpdateObject = JsonConvert.DeserializeObject<person>(log.action_data);
                            personUpdateObject.last_modified_on = DateTime.UtcNow.AddHours(1);
                            personUpdateObject.last_modified_by = $"{userData.person?.surname}, {userData.person?.first_name} {userData.person?.middle_name}";
                            var personUpdateResult = PersonService.Update(personUpdateObject, CallerFormName, callerFormMethod, callerIpAddress);
                            if (personUpdateResult <= 0)
                            {
                                stopCheckFlag = true;
                                stopCheckMessage = "Unable to approve pending edited fintech contact person at the moment; Kindly try again later!";

                            }
                            
                            if (!stopCheckFlag)
                            {
                                applyResult = personUpdateResult;
                            }
                            break;

                        default:
                            stopCheckFlag = true;
                            stopCheckMessage = "Invalid Maker-Checker Type";
                            break;
                    }
                }

                if (!stopCheckFlag)
                {
                    if (applyResult <= 0)
                    {
                        stopCheckFlag = true;
                        stopCheckMessage = "Unable to approve pending created/edited fintech contact person at the moment. Kindly try again later!";
                    }
                }

                long logUpdateResult = 0;
                if (!stopCheckFlag)
                {
                    log.maker_checker_status = (int)MakerCheckerStatus.Approved;
                    log.checker_person_id = userData.person_id;
                    log.checker_username = userData.username;
                    log.checker_fullname = $"{userData.person?.surname}, {userData.person?.first_name} {userData.person?.middle_name}";
                    log.checker_remarks = model.CheckerRemarks;
                    log.date_checked = DateTime.UtcNow.AddHours(1);

                    logUpdateResult = MakerCheckerLogService.Update(log, CallerFormName, callerFormMethod, callerIpAddress);
                    if (logUpdateResult <= 0)
                    {
                        stopCheckFlag = true;
                        stopCheckMessage = "Unable to approve pending created/edited fintech contact person at the moment; maker-checker log could not be updated. Try again later!";
                    }
                }

                if (!stopCheckFlag)
                {
                    //Task 1
                    new Task(() =>
                    {
                        var appActivity = new appactivity
                        {
                            audit_username = userData.username,
                            audit_fullname = $"{userData.person?.surname}, {userData.person?.first_name} {userData.person?.middle_name}",
                            audit_ipaddress = callerIpAddress,
                            audit_macaddress = callerMacAddress,
                            operation = $"Approved - {log.action_name}".ToUpper(),
                            comments = $"Approved - {log.action_details}",
                            created_on = DateTime.Now,
                            created_by = "System"
                        };
                        AppActivityService.Insert(appActivity, CallerFormName, callerFormMethod, callerIpAddress);

                    }).Start();

                    //Task 2
                    new Task(() =>
                    {
                        var makerFirstname = "";
                        var makerEmailAddress = "";
                        var makerPerson = PersonService.GetWithPersonId(Convert.ToString(log.maker_person_id), CallerFormName, callerFormMethod, callerIpAddress);
                        if (makerPerson != null)
                        {
                            makerFirstname = makerPerson.first_name;
                            makerEmailAddress = makerPerson.email_address;
                        }

                        if (!string.IsNullOrEmpty(makerEmailAddress))
                        {
                            var templatePath = Server.MapPath(ConfigurationUtility.GetAppSettingValue("ApprovalNotificationBodyTemplate"));
                            var mailBody = System.IO.File.ReadAllText(templatePath).Replace("{Name}", makerFirstname).Replace("{ActionName}", log.action_name).Replace("{ActionDetails}", log.action_details).Replace("{ApprovedBy}", log.checker_fullname).Replace("{ApprovedOn}", Convert.ToDateTime(log.date_checked).ToString("dd-MM-yyyy hh:mm tt"));
                            var mailRecipient = makerEmailAddress;
                            EmailLogManager.Log(EmailType.ApprovalNotification, mailBody, mailRecipient, "", "", CallerFormName, callerFormMethod, callerIpAddress);
                        }

                    }).Start();

                    AlertUser("Successfully approved request. Your changes have been effected.", AlertType.Success);
                }
                else
                {
                    AlertUser(stopCheckMessage, AlertType.Error);
                    return RedirectToAction("ReviewPendingCreatedAndEditedFintechContactPerson", new { q = model.QueryString });
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(CallerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return RedirectToAction("ViewPendingCreatedAndEditedFintechContactPerson");
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("reject-pending-created-and-edited-fintech-contact-person")]
        public ActionResult RejectPendingCreatedAndEditedFintechContactPerson(MakerCheckerLogModel model)
        {
            const string callerFormMethod = "HttpPost|RejectPendingCreatedAndEditedFintechContactPerson";

            try
            {
                var checkerEmails = "";
                if (!AuthorizeRequest(Request.Url.AbsolutePath, ref checkerEmails, CallerFormName, callerFormMethod, callerIpAddress))
                {
                    AlertUser("Unauthorised Access! You may not have been permitted to access the resource. Kindly login to re-authenticate. If issue persist, contact administrator.", AlertType.Error);
                    return RedirectToAction("Logout", "Account");
                }

                var log = MakerCheckerLogService.GetWithQueryString(model.QueryString, CallerFormName, callerFormMethod, callerIpAddress);
                if (log == null)
                {
                    stopCheckFlag = true;
                    stopCheckMessage = "Unable to retrieve pending created/edited fintech contact person at the moment, kindly try again later";
                }

                if (!stopCheckFlag)
                {
                    log.maker_checker_status = (int)MakerCheckerStatus.Rejected;
                    log.checker_person_id = userData.person_id;
                    log.checker_username = userData.username;
                    log.checker_fullname = $"{userData.person?.surname}, {userData.person?.first_name} {userData.person?.middle_name}";
                    log.checker_remarks = model.CheckerRemarks;
                    log.date_checked = DateTime.UtcNow.AddHours(1);

                    var logUpdateResult = MakerCheckerLogService.Update(log, CallerFormName, callerFormMethod, callerIpAddress);
                    if (logUpdateResult <= 0)
                    {
                        stopCheckFlag = true;
                        stopCheckMessage = "Unable to reject pending created/edited fintech contact person at the moment; maker-checker log could not be updated. Try again later!";
                    }
                }

                if (!stopCheckFlag)
                {
                    //Task 1
                    new Task(() =>
                    {
                        var appActivity = new appactivity
                        {
                            audit_username = userData.username,
                            audit_fullname = $"{userData.person?.surname}, {userData.person?.first_name} {userData.person?.middle_name}",
                            audit_ipaddress = callerIpAddress,
                            audit_macaddress = callerMacAddress,
                            operation = $"Rejected - {log.action_name}".ToUpper(),
                            comments = $"Rejected - {log.action_details} with remarks- {model.CheckerRemarks}",
                            created_on = DateTime.Now,
                            created_by = "System"
                        };
                        AppActivityService.Insert(appActivity, CallerFormName, callerFormMethod, callerIpAddress);

                    }).Start();

                    //Task 2
                    new Task(() =>
                    {
                        var makerFirstname = "";
                        var makerEmailAddress = "";
                        var makerPerson = PersonService.GetWithPersonId(Convert.ToString(log.maker_person_id), CallerFormName, callerFormMethod, callerIpAddress);
                        if (makerPerson != null)
                        {
                            makerFirstname = makerPerson.first_name;
                            makerEmailAddress = makerPerson.email_address;
                        }

                        if (!string.IsNullOrEmpty(makerEmailAddress))
                        {
                            var templatePath = Server.MapPath(ConfigurationUtility.GetAppSettingValue("RejectionNotificationBodyTemplate"));
                            var mailBody = System.IO.File.ReadAllText(templatePath).Replace("{Name}", makerFirstname).Replace("{ActionName}", log.action_name).Replace("{ActionDetails}", log.action_details).Replace("{RejectedBy}", log.checker_fullname).Replace("{RejectedOn}", Convert.ToDateTime(log.date_checked).ToString("dd-MM-yyyy hh:mm tt")).Replace("{RejectionReason}", log.checker_remarks);
                            var mailRecipient = makerEmailAddress;
                            EmailLogManager.Log(EmailType.RejectionNotification, mailBody, mailRecipient, "", "", CallerFormName, callerFormMethod, callerIpAddress);
                        }

                    }).Start();

                    AlertUser($"Successfully rejected request with remarks '{model.CheckerRemarks}'", AlertType.Success);
                }
                else
                {
                    AlertUser(stopCheckMessage, AlertType.Error);
                    return RedirectToAction("ReviewPendingCreatedAndEditedFintechContactPerson", new { q = model.QueryString });
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(CallerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return RedirectToAction("ViewPendingCreatedAndEditedFintechContactPerson");
        }
    }
}