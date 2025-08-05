using App.Core.BusinessLogic;
using App.Core.DataTransferObjects;
using App.Core.Services;
using App.Core.Utilities;
using App.DataModels.Models;
using App.Web.Models;
using App.Web.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace App.Web.Controllers
{
    [Authorize]
    [RoutePrefix("salary-account-management")]
    public class SalaryAccountManagementController : BaseController
    {
        private const string CallerFormName = "SalaryAccountManagementController";
        private readonly string callerIpAddress;
        private readonly string callerMacAddress;
        private readonly ViewAccount360DegreesPortfolioViewModel viewAccount360DegreesPortfolioViewModel;
        private readonly UpdateEmployerDetailsInSingleViewModel updateEmployerDetailsInSingleViewModel;
        private readonly UpdateEmployerDetailsInBulkViewModel updateEmployerDetailsInBulkViewModel;
        private ProfileIdentifiedSalaryAccountViewModel profileIdentifiedSalaryAccountViewModel;

        private readonly ReviewMakerCheckerLogViewModel reviewMakerCheckerLogViewModel;
        private List<MakerCheckerLogModel> MakerCheckerLogModels;
        private readonly user userData;

        private bool stopCheckFlag = false;
        private string stopCheckMessage = "";

        public SalaryAccountManagementController()
        {
            callerIpAddress = IpAddressManager.GetClientComputerIpAddress();
            callerMacAddress = MacAddressManager.GetClientComputerMacAddress();
            viewAccount360DegreesPortfolioViewModel = new ViewAccount360DegreesPortfolioViewModel();
            updateEmployerDetailsInSingleViewModel = new UpdateEmployerDetailsInSingleViewModel();
            updateEmployerDetailsInBulkViewModel = new UpdateEmployerDetailsInBulkViewModel();

            reviewMakerCheckerLogViewModel = new ReviewMakerCheckerLogViewModel();
            MakerCheckerLogModels = new List<MakerCheckerLogModel>();
            userData = GetUser(CallerFormName, "Ctor|SalaryAccountManagementController", callerIpAddress);

            nIBSSBvnValidatorManager = new NIBSSBvnValidatorManager();
            cRMSManager = new CRMSManager();
        }

        #region ActionResult

        [HttpGet]
        [Route("view-account-360-degrees-portfolio")]
        public ActionResult ViewAccount360DegreesPortfolio()
        {
            return View(viewAccount360DegreesPortfolioViewModel);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("view-account-360-degrees-portfolio")]
        public async Task<ActionResult> ViewAccount360DegreesPortfolio(string accountNumber)
        {
            const string callerFormMethod = "HttpPost|ViewAccount360DegreesPortfolio";

            try
            {
                var checkerEmails = "";
                if (!AuthorizeRequest(Request.Url.AbsolutePath, ref checkerEmails, CallerFormName, callerFormMethod, callerIpAddress))
                {
                    AlertUser("Unauthorised Access! You may not have been permitted to access the resource. Kindly login to re-authenticate. If issue persist, contact administrator.", AlertType.Error);
                    return RedirectToAction("Logout", "Account");
                }

                if (!ModelState.IsValid)
                {
                    var errorMessages = string.Join("<br/>", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
                    stopCheckFlag = true;
                    stopCheckMessage = errorMessages;
                }

                if (!stopCheckFlag)
                {
                    var apiResponseFeedback = "";

                    //get account details
                    var getAccountDetailsRequest = new GetAccountDetailsRequest
                    {
                        RequestId = Guid.NewGuid().ToString("N").ToUpper(),
                        CountryId = $"{ConfigurationUtility.GetAppSettingValue("CountryId")}",
                        AccountNumber = accountNumber
                    };

                    var getAccountDetailsResponse = await FIBridgeManager.GetAccountDetailsAsync(getAccountDetailsRequest, CallerFormName, callerFormMethod, callerIpAddress);
                    if (getAccountDetailsResponse?.ResponseCode == "00")
                    {
                        viewAccount360DegreesPortfolioViewModel.AccountDetailsModel = new AccountDetailsModel
                        {
                            CustomerId = getAccountDetailsResponse.CustomerId,
                            AccountNumber = getAccountDetailsResponse.AccountNumber,
                            AccountName = getAccountDetailsResponse.AccountName,
                            AccountType = getAccountDetailsResponse.AccountType,
                            FreezeCode = getAccountDetailsResponse.FreezeCode,
                            ProductCode = getAccountDetailsResponse.ProductCode,
                            Product = getAccountDetailsResponse.Product,
                            AccountStatus = getAccountDetailsResponse.AccountStatus,
                            CurrencyCode = getAccountDetailsResponse.CurrencyCode,
                            BranchCode = getAccountDetailsResponse.BranchCode,
                            Branch = getAccountDetailsResponse.Branch,
                            BookBalance = getAccountDetailsResponse.BookBalance,
                            AvailableBalance = getAccountDetailsResponse.AvailableBalance,
                            LienAmount = getAccountDetailsResponse.LienAmount,
                            UnclearedBalance = getAccountDetailsResponse.UnclearedBalance,
                            MobileNo = getAccountDetailsResponse.MobileNo,
                            Email = getAccountDetailsResponse.Email,
                            RelationshipManagerId = getAccountDetailsResponse.RelationshipManagerId
                        };
                    }
                    else
                    {
                        apiResponseFeedback += "Unable to fetch account details <br/>";
                    }

                    //get customer details
                    var getCustomerDetailsRequest = new GetCustomerDetailsRequest
                    {
                        RequestId = Guid.NewGuid().ToString("N").ToUpper(),
                        CountryId = $"{ConfigurationUtility.GetAppSettingValue("CountryId")}",
                        AccountNumber = accountNumber
                    };

                    var customerDetailsDictionary = new Dictionary<string, string>();
                    var getCustomerDetailsResponse = await FIBridgeManager.GetCustomerDetailsAsync(getCustomerDetailsRequest, CallerFormName, callerFormMethod, callerIpAddress);
                    if (getCustomerDetailsResponse?.ResponseCode == "00")
                    {
                        customerDetailsDictionary.Add("SN", getCustomerDetailsResponse?.LastName);
                        customerDetailsDictionary.Add("FN", getCustomerDetailsResponse?.FirstName);
                        customerDetailsDictionary.Add("DOB", getCustomerDetailsResponse?.DateOfBirth);
                        customerDetailsDictionary.Add("G", getCustomerDetailsResponse?.Gender);

                        viewAccount360DegreesPortfolioViewModel.CustomerDetailsModel = new CustomerDetailsModel
                        {
                            CustomerId = getCustomerDetailsResponse.CustomerId,
                            Title = getCustomerDetailsResponse.Title,
                            Gender = getCustomerDetailsResponse.Gender,
                            FirstName = getCustomerDetailsResponse.FirstName,
                            MiddleName = getCustomerDetailsResponse.MiddleName,
                            LastName = getCustomerDetailsResponse.LastName,
                            CustomerCategory = getCustomerDetailsResponse.CustomerCategory,
                            Address = getCustomerDetailsResponse.Address,
                            DateOfBirth = getCustomerDetailsResponse.DateOfBirth,
                            MobileNo = getCustomerDetailsResponse.MobileNo,
                            Email = getCustomerDetailsResponse.Email,
                            State = getCustomerDetailsResponse.State,
                            Country = getCustomerDetailsResponse.Country
                        };
                    }
                    else
                    {
                        apiResponseFeedback += "Unable to fetch customer details <br/>";
                    }

                    //get relationship manager
                    var getAccountRelationshipManagerRequest = new GetAccountRelationshipManagerRequest
                    {
                        RequestId = Guid.NewGuid().ToString("N").ToUpper(),
                        CountryId = $"{ConfigurationUtility.GetAppSettingValue("CountryId")}",
                        AccountNumber = accountNumber
                    };

                    var getAccountRelationshipManagerResponse = await FIBridgeManager.GetAccountRelationshipManagerAsync(getAccountRelationshipManagerRequest, CallerFormName, callerFormMethod, callerIpAddress);
                    if (getAccountRelationshipManagerResponse?.ResponseCode == "00")
                    {
                        viewAccount360DegreesPortfolioViewModel.AccountRelationshipManagerModel = new AccountRelationshipManagerModel
                        {
                            RelationshipManagerId = getAccountRelationshipManagerResponse.RelationshipManagerId,
                            RelationshipManagerEmail = getAccountRelationshipManagerResponse.RelationshipManagerEmail,
                            CustomerAccountName = getAccountRelationshipManagerResponse.CustomerAccountName,
                            CustomerAccountNumber = getAccountRelationshipManagerResponse.CustomerAccountNumber
                        };
                    }
                    else
                    {
                        apiResponseFeedback += "Unable to fetch relationship manager details <br/>";
                    }


                    //get customer accounts
                    var getCustomerAccountsRequest = new GetCustomerAccountsRequest
                    {
                        RequestId = Guid.NewGuid().ToString("N").ToUpper(),
                        CountryId = $"{ConfigurationUtility.GetAppSettingValue("CountryId")}",
                        CustomerId = getAccountDetailsResponse?.CustomerId
                    };

                    var customerAccountNumberList = new List<string>();

                    var getCustomerAccountsResponse = await FIBridgeManager.GetCustomerAccountAsync(getCustomerAccountsRequest, CallerFormName, callerFormMethod, callerIpAddress);
                    if (getCustomerAccountsResponse?.ResponseCode == "00")
                    {
                        if (getCustomerAccountsResponse.Accounts.Count > 0)
                        {
                            customerAccountNumberList = getCustomerAccountsResponse.Accounts.Select(customerAccount => customerAccount.AccountNumber).ToList();

                            viewAccount360DegreesPortfolioViewModel.CustomerAccountModels = getCustomerAccountsResponse.Accounts.Select(customerAccount => new CustomerAccountModel
                            {
                                CustomerId = customerAccount.CustomerId,
                                AccountNumber = customerAccount.AccountNumber,
                                AccountName = customerAccount.AccountName,
                                AccountType = customerAccount.AccountType,
                                IsCreditFrozen = customerAccount.IsCreditFrozen,
                                ProductCode = customerAccount.ProductCode,
                                Product = customerAccount.Product,
                                AccountStatus = customerAccount.AccountStatus,
                                CurrencyCode = customerAccount.CurrencyCode,
                                BranchCode = customerAccount.BranchCode,
                                Branch = customerAccount.Branch,
                                BookBalance = customerAccount.BookBalance,
                                AvailableBalance = customerAccount.AvailableBalance,
                                LienAmount = customerAccount.LienAmount,
                                UnclearedBalance = customerAccount.UnclearedBalance,
                                IsAccountActive = customerAccount.IsAccountActive,
                                IsCardAccount = customerAccount.IsCardAccount,
                                MobileNo = customerAccount.MobileNo,
                                Email = customerAccount.Email,
                                RelationshipManagerId = customerAccount.RelationshipManagerId

                            }).ToList();
                        }
                    }
                    else
                    {
                        apiResponseFeedback += "Unable to fetch customer accounts <br/>";
                    }


                    //get loan exposure
                    if (customerAccountNumberList.Count > 0)
                    {
                        List<LoanExposure> loanExposures = null;
                        await Task.Run(() => { loanExposures = FinacleServices.GetAllFromLoanExposure(customerAccountNumberList, CallerFormName, callerFormMethod, callerIpAddress); });

                        if (loanExposures != null && loanExposures.Count > 0)
                        {
                            viewAccount360DegreesPortfolioViewModel.LoanExposureModels = loanExposures.Select(data => new LoanExposureModel
                            {
                                SchemeCode = data.SchemeCode,
                                LoanAccount = data.LoanAccount,
                                OperationalAccount = data.OperationalAccount,
                                AccountName = data.AccountName,
                                ReferenceDescription = data.ReferenceDescription,
                                Principal = data.Principal,
                                PrincipalRepayment = data.PrincipalRepayment,
                                InterestRepayment =data.InterestRepayment,
                                Interest = data.Interest,
                                DateApproved = data.DateApproved,
                                DateDisbursed = data.DateDisbursed,
                                OutstandingBalance = data.OutstandingBalance,
                                Frequency = data.Frequency,
                                Tenor = data.Tenor,
                                OutstandingTenor = data.OutstandingTenor,
                                LoanSource = data.LoanSource

                            }).ToList();
                        }
                    }
                    

                    //get bvn with account number
                    var getBvnWithAccountNumberRequest = new GetBvnWithAccountNumberRequest
                    {
                        RequestId = Guid.NewGuid().ToString("N").ToUpper(),
                        CountryId = $"{ConfigurationUtility.GetAppSettingValue("CountryId")}",
                        AccountNumber = accountNumber
                    };

                    var getBvnWithAccountNumberResponse = await FIBridgeManager.GetBvnWithAccountNumberAsync(getBvnWithAccountNumberRequest, CallerFormName, callerFormMethod, callerIpAddress);
                    var bvn = getBvnWithAccountNumberResponse?.ResponseCode == "00" ? getBvnWithAccountNumberResponse.Bvn : "";

                    //get bvn validation
                    Core.BvnValidatorServiceClient.ValidatorResponse validatorResponse = null;
                    if (!string.IsNullOrEmpty(bvn)) validatorResponse = nIBSSBvnValidatorManager.ValidateBvn(bvn, CallerFormName, callerFormMethod, callerIpAddress);
                    if (validatorResponse != null && validatorResponse.Status?.ResponseCode == "00")
                    {
                        var bvnDictionary = new Dictionary<string, string>
                        {
                            { "SN", validatorResponse.Result?.LastName},
                            { "FN", validatorResponse.Result?.FirstName},
                            { "DOB", validatorResponse.Result?.DateOfBirth},
                            { "G", validatorResponse.Result?.Gender},
                        };

                        viewAccount360DegreesPortfolioViewModel.IsValidBvn = ValidationUtility.IsValidBvnDetails(customerDetailsDictionary, bvnDictionary);
                        viewAccount360DegreesPortfolioViewModel.BvnDetailsModel = new BvnDetailsModel
                        {
                            BVN = validatorResponse.Result?.BVN,
                            DateOfBirth = validatorResponse.Result?.DateOfBirth,
                            Email = validatorResponse.Result?.Email,
                            FirstName = validatorResponse.Result?.FirstName,
                            Gender = validatorResponse.Result?.Gender,
                            LastName = validatorResponse.Result?.LastName,
                            LevelOfAccount = validatorResponse.Result?.LevelOfAccount,
                            LgaOfOrigin = validatorResponse.Result?.LgaOfOrigin,
                            LgaOfResidence = validatorResponse.Result?.LgaOfResidence,
                            MaritalStatus = validatorResponse.Result?.MaritalStatus,
                            MiddleName = validatorResponse.Result?.MiddleName,
                            NIN = validatorResponse.Result?.NIN,
                            NameOnCard = validatorResponse.Result?.NameOnCard,
                            Nationality = validatorResponse.Result?.Nationality,
                            PhoneNumber1 = validatorResponse.Result?.PhoneNumber1,
                            PhoneNumber2 = validatorResponse.Result?.PhoneNumber2,
                            ResidentialAddress = validatorResponse.Result?.ResidentialAddress,
                            StateOfOrigin = validatorResponse.Result?.StateOfOrigin,
                            StateOfResidence = validatorResponse.Result?.StateOfResidence,
                            Title = validatorResponse.Result?.Title
                        };
                    }
                    else
                    {
                        apiResponseFeedback += "Unable to fetch BVN details <br/>";
                    }


                    //get crms validation
                    CRMSCreditCheckResponse creditCheckResponse = null;
                    if (!string.IsNullOrEmpty(bvn)) await Task.Run(() => { creditCheckResponse = cRMSManager.CreditCheck(bvn, CallerFormName, callerFormMethod, callerIpAddress); });
                    if (creditCheckResponse != null)
                    {
                        viewAccount360DegreesPortfolioViewModel.IsValidCRMS = ValidationUtility.IsValidCRMSDetails(creditCheckResponse.Summary);

                        if (creditCheckResponse.CRMSCredits != null && creditCheckResponse.CRMSCredits.Count > 0)
                        {
                            viewAccount360DegreesPortfolioViewModel.CRMSDetailsModels = creditCheckResponse.CRMSCredits.Select(detail => new CRMSDetailsModel
                            {
                                CRMSRefNumber = detail.CRMSRefNumber,
                                CreditType = detail.CreditType,
                                CreditLimit = detail.CreditLimit,
                                OutstandingAmount = detail.OutstandingAmount,
                                EffectiveDate = detail.EffectiveDate,
                                Tenor = detail.Tenor,
                                ExpiryDate = detail.ExpiryDate,
                                GrantingInstitution = detail.GrantingInstitution,
                                PerformanceStatus = detail.PerformanceStatus

                            }).ToList();
                        }

                        viewAccount360DegreesPortfolioViewModel.CRMSSummary = creditCheckResponse.Summary;
                    }
                    else
                    {
                        apiResponseFeedback += "Unable to fetch CRMS details <br/>";
                    }


                    //get crc validation
                    CRCCreditCheckResponse cRCCreditCheckResponse = null;
                    if (!string.IsNullOrEmpty(bvn)) cRCCreditCheckResponse = await CRCManager.CreditCheckAsync(bvn, CallerFormName, callerFormMethod, callerIpAddress);
                    if (cRCCreditCheckResponse != null)
                    {
                        var crcSummary = "";
                        viewAccount360DegreesPortfolioViewModel.IsValidCRC = ValidationUtility.IsValidCRCDetails(cRCCreditCheckResponse, ref crcSummary);
                        viewAccount360DegreesPortfolioViewModel.CRCSummary = crcSummary;
                    }
                    else
                    {
                        apiResponseFeedback += "Unable to fetch CRC details <br/>";
                    }

                    //get employer information
                    List<employer> employers = null;
                    await Task.Run(() => { employers = EmployerService.GetWithAccountNumber(accountNumber, CallerFormName, callerFormMethod, callerIpAddress); }); 
                    if (employers != null && employers.Count > 0)
                    {
                        viewAccount360DegreesPortfolioViewModel.EmployerHistoryModels = employers.Select(employer => new EmployerHistoryModel
                        {
                            EmployerName = employer.employer_name,
                            CreatedOn = Convert.ToDateTime(employer.created_on).ToString("dd-MM-yyyy hh:mm tt"),
                            CreatedBy = employer.created_by,
                            ApprovedOn = !string.IsNullOrEmpty(Convert.ToString(employer.approved_on)) ? Convert.ToDateTime(employer.approved_on).ToString("dd-MM-yyyy hh:mm tt") : "n/p",
                            ApprovedBy = employer.approved_by,

                        }).ToList();
                    }


                    //get sal profiling details
                    SalProfiling salProfiling = null;
                    await Task.Run(() => { salProfiling = FinacleServices.GetFromSalProfilingWithAccountNumber(ConfigurationUtility.GetConnectionStringValue("ObieeDbConnection"),accountNumber, CallerFormName, callerFormMethod, callerIpAddress);}); 
                    if (salProfiling != null)
                    {
                        viewAccount360DegreesPortfolioViewModel.SalProfilingDetailsModel = new SalProfilingDetailsModel
                        {
                            AverageSalary = salProfiling.Average,
                            MaxSalaryValue = salProfiling.MaxVal,
                            MinSalaryValue = salProfiling.MinVal,
                            MostFrequentNarration = salProfiling.MostFreqNarr,
                            MostFrequentTransactionDate = salProfiling.MostFreqTranDate,
                            FirstMonth = salProfiling.FirstMonth,
                            SecondMonth = salProfiling.SecondMonth, 
                            ThirdMonth = salProfiling.ThirdMonth,
                            FourthMonth = salProfiling.FourthMonth,
                            FifthMonth = salProfiling.FifthMonth,
                            SixthMonth = salProfiling.SixthMonth,
                            Source = salProfiling.Src,
                            InsertedDate = salProfiling.InsertedDate,
                            IsValidBvn = salProfiling.IsValidBvn,
                            BvnCheckDate = salProfiling.BvnCheckDate,
                            IsValidCRMS = salProfiling.IsValidCRMS,
                            CRMSCheckDate = salProfiling.CRMSCheckDate,
                            IsValidCRC = salProfiling.IsValidCRC,
                            CRCSCheckDate = salProfiling.CRCSCheckDate
                        };
                    }


                    //get soda details
                    SodaRac sodaRac = null;
                    await Task.Run(() => { sodaRac = FinacleServices.GetFromSodaRacWithAccountNumber(accountNumber, CallerFormName, callerFormMethod, callerIpAddress); });
                    if (sodaRac != null)
                    {
                        viewAccount360DegreesPortfolioViewModel.SodaRacDetailsModel = new SodaRacDetailsModel
                        {
                            AverageSalary = Convert.ToDecimal(sodaRac.AverageSalary).ToString("N")
                        };
                    }


                    //
                    if (!string.IsNullOrEmpty(apiResponseFeedback)) AlertUser(apiResponseFeedback, AlertType.Information);

                    //log activity
                    new Task(() =>
                    {
                        var appActivity = new appactivity
                        {
                            audit_username = userData.username,
                            audit_fullname = $"{userData.person?.surname}, {userData.person?.first_name} {userData.person?.middle_name}",
                            audit_ipaddress = callerIpAddress,
                            audit_macaddress = callerMacAddress,
                            operation = $"Viewed Account 360-Degrees Portfolio".ToUpper(),
                            comments = $"Viewed Account 360-Degrees Portfolio for Account Number: '{accountNumber}'",
                            created_on = DateTime.Now,
                            created_by = "System"
                        };

                        AppActivityService.Insert(appActivity, CallerFormName, callerFormMethod, callerIpAddress);

                    }).Start();

                    viewAccount360DegreesPortfolioViewModel.AccountNumber = accountNumber;
                    viewAccount360DegreesPortfolioViewModel.ShowAccordions = true;
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

            return View(viewAccount360DegreesPortfolioViewModel);
        }

        [HttpGet]
        [Route("update-employer-details-single")]
        public ActionResult UpdateEmployerDetailsInSingle(string q)
        {
            if (!string.IsNullOrEmpty(q))
            {
                updateEmployerDetailsInSingleViewModel.AccountNumber = EncryptionUtility.Decrypt(HttpUtility.UrlDecode(q), ConfigurationUtility.GetAppSettingValue("EncryptionKey"));
                updateEmployerDetailsInSingleViewModel.AccountNumberDisabled = true;
            }
            else
            {
                updateEmployerDetailsInSingleViewModel.AccountNumber = "";
            }

            return View(updateEmployerDetailsInSingleViewModel);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("update-employer-details-single")]
        public async Task<ActionResult> UpdateEmployerDetailsInSingle(UpdateEmployerDetailsInSingleViewModel model)
        {
            const string callerFormMethod = "HttpPost|UpdateEmployerDetailsInSingle";

            try
            {
                var checkerEmails = "";
                if (!AuthorizeRequest(Request.Url.AbsolutePath, ref checkerEmails, CallerFormName, callerFormMethod, callerIpAddress))
                {
                    AlertUser("Unauthorised Access! You may not have been permitted to access the resource. Kindly login to re-authenticate. If issue persist, contact administrator.", AlertType.Error);
                    return RedirectToAction("Logout", "Account");
                }

                if (!ModelState.IsValid)
                {
                    var errorMessages = string.Join("<br/>", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
                    stopCheckFlag = true;
                    stopCheckMessage = errorMessages;
                }

                var makerCheckerAction = "Update Employer Detail";
                var makerCheckerActionDetails = $"Update Employer: '{model.EmployerName}' for Account Number: '{model.AccountNumber}'";

                var accountName = "";
                if (!stopCheckFlag)
                {
                    var getAccountDetailsRequest = new GetAccountDetailsRequest
                    {
                        RequestId = Guid.NewGuid().ToString("N").ToUpper(),
                        CountryId = $"{ConfigurationUtility.GetAppSettingValue("CountryId")}",
                        AccountNumber = model.AccountNumber
                    };

                    var getAccountDetailsResponse = await FIBridgeManager.GetAccountDetailsAsync(getAccountDetailsRequest, CallerFormName, callerFormMethod, callerIpAddress);
                    if (getAccountDetailsResponse.ResponseCode == "00") accountName = getAccountDetailsResponse?.AccountName;
                    if (string.IsNullOrEmpty(accountName))
                    {
                        stopCheckFlag = true;
                        stopCheckMessage = "Unable to fetch account name for account number : " + model.AccountNumber + ". Kindly check account number and try again.";
                    }
                }

                var salaryaccountsemployers = new List<salaryaccountsemployer>();

                if (!stopCheckFlag)
                {
                    var salaryaccountsemployer = new salaryaccountsemployer
                    {
                        salaryaccount = new salaryaccount
                        {
                            account_number = model.AccountNumber,
                            account_name = accountName,
                            created_on = DateTime.UtcNow.AddHours(1),
                            created_by = $"{userData.person?.surname}, {userData.person?.first_name} {userData.person?.middle_name}",
                            query_string = Guid.NewGuid()
                        },

                        employer = new employer
                        {
                            employer_name = model.EmployerName,
                            created_on = DateTime.UtcNow.AddHours(1),
                            created_by = $"{userData.person?.surname}, {userData.person?.first_name} {userData.person?.middle_name}",
                            query_string = Guid.NewGuid()
                        },

                        created_on = DateTime.UtcNow.AddHours(1),
                        created_by = $"{userData.person?.surname}, {userData.person?.first_name} {userData.person?.middle_name}",
                        query_string = Guid.NewGuid(),
                    };

                    salaryaccountsemployers.Add(salaryaccountsemployer);

                    var makerCheckerActionData = JsonConvert.SerializeObject(salaryaccountsemployers);
                    var sol = userData.branch_user_flag ? FinacleServices.GetFinacleSol2(userData.username, CallerFormName, callerFormMethod, callerIpAddress) : null;
                    var makerCheckerLog = new makercheckerlog
                    {
                        maker_checker_category_id = (int)MakerCheckerCategory.AccountEmployerUpdate,
                        maker_checker_type_id = (int)MakerCheckerType.UpdateEmployerDetails,
                        action_name = makerCheckerAction,
                        action_details = makerCheckerActionDetails,
                        action_data = makerCheckerActionData,
                        maker_person_id = userData.person_id,
                        maker_username = userData.username,
                        maker_fullname = $"{userData.person?.surname}, {userData.person?.first_name} {userData.person?.middle_name}",

                        maker_sol_id = sol?.SolId,
                        maker_sol_name = sol?.SolName,
                        maker_sol_address = sol?.SolAddress,

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
                            var mailBody = System.IO.File.ReadAllText(templatePath).Replace("{ItemType}", "Updated Employer Details");
                            var mailRecipient = checkerEmails;
                            EmailLogManager.Log(EmailType.PendingItemsNotification, mailBody, mailRecipient, "", "", CallerFormName, callerFormMethod, callerIpAddress);
                        }

                    }).Start();

                    AlertUser("Successfully submitted request for approval. Your changes will be effected once approved.", AlertType.Success);

                    return RedirectToAction("UpdateEmployerDetailsInSingle");
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

            return View(model);
        }

        [HttpGet]
        [Route("update-employer-details-bulk")]
        public ActionResult UpdateEmployerDetailsInBulk()
        {
            return View(updateEmployerDetailsInBulkViewModel);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("update-employer-details-bulk")]
        public async Task<ActionResult> UpdateEmployerDetailsInBulk(UpdateEmployerDetailsInBulkViewModel model)
        {
            const string callerFormMethod = "HttpPost|UpdateEmployerDetailsInBulk";

            try
            {
                var httpPostedFileBase = Request.Files[0];
                if (httpPostedFileBase == null || httpPostedFileBase.ContentLength == 0)
                {
                    stopCheckFlag = true;
                    stopCheckMessage = "Invalid upload file!";
                }

                var supportedFileTypes = new[] { ".xls", ".XLS", ".xlsx", ".XLSX" };
                if (!stopCheckFlag)
                {
                    var fileExtension = System.IO.Path.GetExtension(httpPostedFileBase.FileName);
                    if (!supportedFileTypes.Contains(fileExtension))
                    {
                        stopCheckFlag = true;
                        stopCheckMessage = "Invalid file format!";
                    }
                }

                var excelSaveUrl = "";
                if (!stopCheckFlag)
                {
                    excelSaveUrl = SaveFile(httpPostedFileBase, "update-employer-details-bulk-excel", CallerFormName, callerFormMethod, callerIpAddress);
                    if (string.IsNullOrEmpty(excelSaveUrl))
                    {
                        stopCheckFlag = true;
                        stopCheckMessage = "Unable to save excel file at the moment, kindly try again later";
                    }
                }

                var dataSet = new DataSet();
                if (!stopCheckFlag)
                {
                    dataSet = ReadExcelFile(Server.MapPath(excelSaveUrl), CallerFormName, callerFormMethod, callerIpAddress);
                    if (dataSet == null || dataSet.Tables[0].Rows.Count <= 1)
                    {
                        stopCheckFlag = true;
                        stopCheckMessage = "No records on excel file uploaded";
                    }
                }


                if (!stopCheckFlag)
                {
                    var bulkEmployerUpdateValidExcelDatas = new List<BulkEmployerUpdateExcelData>();
                    var bulkEmployerUpdateInvalidExcelDatas = new List<BulkEmployerUpdateExcelData>();

                    for (var i = 1; i < dataSet.Tables[0].Rows.Count; i++)
                    {
                        var accountNumber = Convert.ToString(dataSet.Tables[0].Rows[i][1]);
                        var employerName = Convert.ToString(dataSet.Tables[0].Rows[i][2]);

                        var accountName = "";
                        var getAccountDetailsRequest = new GetAccountDetailsRequest
                        {
                            RequestId = Guid.NewGuid().ToString("N").ToUpper(),
                            CountryId = $"{ConfigurationUtility.GetAppSettingValue("CountryId")}",
                            AccountNumber = accountNumber
                        };

                        var getAccountDetailsResponse = await FIBridgeManager.GetAccountDetailsAsync(getAccountDetailsRequest, CallerFormName, callerFormMethod, callerIpAddress);
                        if (getAccountDetailsResponse.ResponseCode == "00") accountName = getAccountDetailsResponse?.AccountName;

                        if (!string.IsNullOrEmpty(accountName) && !string.IsNullOrEmpty(employerName))
                        {
                            var bulkEmployerUpdateExcelData = new BulkEmployerUpdateExcelData()
                            {
                                Sn = i.ToString(),
                                AccountNumber = accountNumber,
                                EmployerName = employerName,
                                AccountName = accountName
                            };

                            bulkEmployerUpdateValidExcelDatas.Add(bulkEmployerUpdateExcelData);
                        }
                        else
                        {
                            var bulkEmployerUpdateExcelData = new BulkEmployerUpdateExcelData()
                            {
                                Sn = i.ToString(),
                                AccountNumber = accountNumber,
                                EmployerName = employerName
                            };

                            bulkEmployerUpdateInvalidExcelDatas.Add(bulkEmployerUpdateExcelData);
                        }
                    }

                    TempData["BulkEmployerUpdateValidExcelData"] = bulkEmployerUpdateValidExcelDatas;
                    TempData["BulkEmployerUpdateInvalidExcelData"] = bulkEmployerUpdateInvalidExcelDatas;
                    
                    TempData.Keep("BulkEmployerUpdateValidExcelData");
                    TempData.Keep("BulkEmployerUpdateInvalidExcelData");
                }


                if (stopCheckFlag) AlertUser(stopCheckMessage, AlertType.Error);
            }
            catch (Exception ex)
            {
                LogUtility.LogError(CallerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return View(updateEmployerDetailsInBulkViewModel);
        }

        [HttpGet]
        [Route("save-employer-details-bulk")]
        public ActionResult SaveEmployerDetailsBulk()
        {
            const string callerFormMethod = "HttpGet|SaveEmployerDetailsBulk";

            try
            {
                if (userData == null)
                {
                    stopCheckFlag = true;
                    stopCheckMessage = "--";
                }

                if (!stopCheckFlag)
                {
                    if (TempData["BulkEmployerUpdateValidExcelData"] == null)
                    {
                        stopCheckFlag = true;
                        stopCheckMessage = "Kindly upload employer details record excel data before saving";
                    }
                }

                var makerCheckerAction = "Update Employer Detail";
                var makerCheckerActionDetails = "";

                var bulkEmployerUpdateExcelDatas = new List<BulkEmployerUpdateExcelData>();
                if (!stopCheckFlag)
                {
                    bulkEmployerUpdateExcelDatas = (List<BulkEmployerUpdateExcelData>)TempData["BulkEmployerUpdateValidExcelData"];

                    foreach (var item in bulkEmployerUpdateExcelDatas)
                    {
                        makerCheckerActionDetails += $"Update Employer: '{item.EmployerName}' for Account Number: '{item.AccountNumber}'|";
                    }

                    makerCheckerActionDetails = makerCheckerActionDetails.Substring(0, makerCheckerActionDetails.Length - 1);

                    if (string.IsNullOrEmpty(makerCheckerActionDetails))
                    {
                        stopCheckFlag = true;
                        stopCheckMessage = "Unable to intiate request";
                    }
                }

                if (!stopCheckFlag)
                {
                    var salaryaccountsemployers = new List<salaryaccountsemployer>();

                    foreach (var item in bulkEmployerUpdateExcelDatas)
                    {
                        var salaryaccountsemployer = new salaryaccountsemployer
                        {
                            salaryaccount = new salaryaccount
                            {
                                account_number = item.AccountNumber,
                                account_name = item.AccountName,
                                created_on = DateTime.UtcNow.AddHours(1),
                                created_by = $"{userData.person?.surname}, {userData.person?.first_name} {userData.person?.middle_name}",
                                query_string = Guid.NewGuid()
                            },

                            employer = new employer
                            {
                                employer_name = item.EmployerName,
                                created_on = DateTime.UtcNow.AddHours(1),
                                created_by = $"{userData.person?.surname}, {userData.person?.first_name} {userData.person?.middle_name}",
                                query_string = Guid.NewGuid()
                            },

                            created_on = DateTime.UtcNow.AddHours(1),
                            created_by = $"{userData.person?.surname}, {userData.person?.first_name} {userData.person?.middle_name}",
                            query_string = Guid.NewGuid(),
                        };

                        salaryaccountsemployers.Add(salaryaccountsemployer);
                    }

                    var makerCheckerActionData = JsonConvert.SerializeObject(salaryaccountsemployers);
                    var sol = userData.branch_user_flag ? FinacleServices.GetFinacleSol2(userData.username, CallerFormName, callerFormMethod, callerIpAddress) : null;
                    var makerCheckerLog = new makercheckerlog
                    {
                        maker_checker_category_id = (int)MakerCheckerCategory.AccountEmployerUpdate,
                        maker_checker_type_id = (int)MakerCheckerType.UpdateEmployerDetails,
                        action_name = makerCheckerAction,
                        action_details = makerCheckerActionDetails,
                        action_data = makerCheckerActionData,
                        maker_person_id = userData.person_id,
                        maker_username = userData.username,
                        maker_fullname = $"{userData.person?.surname}, {userData.person?.first_name} {userData.person?.middle_name}",

                        maker_sol_id = sol?.SolId,
                        maker_sol_name = sol?.SolName,
                        maker_sol_address = sol?.SolAddress,

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

            return RedirectToAction("UpdateEmployerDetailsInBulk");
        }

        [HttpGet]
        [Route("view-pending-updated-employer-details")]
        public ActionResult ViewPendingUpdatedEmployerDetails()
        {
            const string callerFormMethod = "HttpGet|ViewPendingUpdatedEmployerDetails";

            try
            {
                var checkerEmails = "";
                if (!AuthorizeRequest(Request.Url.AbsolutePath, ref checkerEmails, CallerFormName, callerFormMethod, callerIpAddress))
                {
                    AlertUser("Unauthorised Access! You may not have been permitted to access the resource. Kindly login to re-authenticate. If issue persist, contact administrator.", AlertType.Error);
                    return RedirectToAction("Logout", "Account");
                }

                var sol = userData.branch_user_flag ? FinacleServices.GetFinacleSol2(userData.username, CallerFormName, callerFormMethod, callerIpAddress) : null;
                var logs = userData.branch_user_flag ? MakerCheckerLogService.GetWithMakerCheckerCategoryAndSolIdAndStatus((int)MakerCheckerCategory.AccountEmployerUpdate, sol?.SolId, (int)MakerCheckerStatus.Initiated, CallerFormName, callerFormMethod, callerIpAddress) : MakerCheckerLogService.GetWithMakerCheckerCategoryAndStatus((int)MakerCheckerCategory.AccountEmployerUpdate, (int)MakerCheckerStatus.Initiated, CallerFormName, callerFormMethod, callerIpAddress);
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
                        MakerSolId = !string.IsNullOrEmpty(log.maker_sol_id) ? log.maker_sol_id : "n/p",
                        MakerCheckerStatus = Convert.ToString(log.maker_checker_status),
                        DateMade = Convert.ToDateTime(log.date_made).ToString("dd-MM-yyyy hh:mm tt"),
                        CheckerId = !string.IsNullOrEmpty(log.checker_username) ? log.checker_username : "n/p",
                        CheckerFullname = !string.IsNullOrEmpty(log.checker_fullname) ? log.checker_fullname : "n/p",
                        CheckerSolId = !string.IsNullOrEmpty(log.checker_sol_id) ? log.checker_sol_id : "n/p",
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
        [Route("review-pending-updated-employer-detail")]
        public ActionResult ReviewPendingUpdatedEmployerDetail(string q)
        {
            const string callerFormMethod = "HttpGet|ReviewPendingUpdatedEmployerDetail";

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
                        MakerSolId = log.maker_sol_id,
                        MakerCheckerStatus = Convert.ToString(log.maker_checker_status),
                        DateMade = Convert.ToDateTime(log.date_made).ToString("dd-MM-yyyy hh:mm tt"),
                        CheckerId = log.checker_username,
                        CheckerFullname = log.checker_fullname,
                        CheckerSolId = log.checker_sol_id,
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
        [Route("approve-pending-updated-employer-detail")]
        public ActionResult ApprovePendingUpdatedEmployerDetail(MakerCheckerLogModel model)
        {
            const string callerFormMethod = "HttpPost|ApprovePendingUpdatedEmployerDetail";

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
                    stopCheckMessage = "Unable to retrieve pending updated employer details at the moment, kindly try again later";
                }

                long totalRecords = 0;
                long insertedRecords = 0;

                if (!stopCheckFlag)
                {
                    switch (log.maker_checker_type_id)
                    {
                        case (int)MakerCheckerType.UpdateEmployerDetails:

                            var salaryaccountsemployers = JsonConvert.DeserializeObject<List<salaryaccountsemployer>>(log.action_data);
                            totalRecords = salaryaccountsemployers.Count;

                            salaryaccountsemployers.ForEach(taco =>
                            {
                                taco.salaryaccount.approved_on = DateTime.UtcNow.AddHours(1);
                                taco.salaryaccount.approved_by = $"{userData.person?.surname}, {userData.person?.first_name} {userData.person?.middle_name}";

                                taco.employer.approved_on = DateTime.UtcNow.AddHours(1);
                                taco.employer.approved_by = $"{userData.person?.surname}, {userData.person?.first_name} {userData.person?.middle_name}";

                                taco.approved_on = DateTime.UtcNow.AddHours(1);
                                taco.approved_by = $"{userData.person?.surname}, {userData.person?.first_name} {userData.person?.middle_name}";
                            });

                            foreach (var item in salaryaccountsemployers)
                            {
                                long employerInsertResult = 0;
                                var employer = EmployerService.GetWithEmployerName(item.employer.employer_name, CallerFormName, callerFormMethod, callerIpAddress);
                                employerInsertResult = employer != null ? employer.id : EmployerService.Insert(item.employer, CallerFormName, callerFormMethod, callerIpAddress);

                                long salaryAccountInsertResult = 0;
                                var salaryaccount = SalaryAccountsService.GetWithAccountNumber(item.salaryaccount.account_number, CallerFormName, callerFormMethod, callerIpAddress);
                                salaryAccountInsertResult = salaryaccount != null ? salaryaccount.id : SalaryAccountsService.Insert(item.salaryaccount, CallerFormName, callerFormMethod, callerIpAddress);


                                long salaryAccountEmployerInsertResult = 0;
                                var salaryaccountsemployer = SalaryAccountsEmployersService.GetWithSalaryAccountIdAndEmployerId(Convert.ToString(salaryAccountInsertResult), Convert.ToString(employerInsertResult), CallerFormName, callerFormMethod, callerIpAddress);
                                if (salaryaccountsemployer == null)
                                {
                                    salaryaccountsemployer = new salaryaccountsemployer
                                    {
                                        salary_account_id = salaryAccountInsertResult,
                                        employer_id = employerInsertResult,
                                        created_on = item.created_on,
                                        created_by = item.created_by,
                                        approved_on = item.approved_on,
                                        approved_by = item.approved_by,
                                        query_string = item.query_string
                                    };

                                    salaryAccountEmployerInsertResult = SalaryAccountsEmployersService.Insert(salaryaccountsemployer, CallerFormName, callerFormMethod, callerIpAddress);
                                }

                                if (salaryAccountEmployerInsertResult > 0) insertedRecords++;
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
                    if (insertedRecords <= 0)
                    {
                        stopCheckFlag = true;
                        stopCheckMessage = "Unable to approve pending updated employer details at the moment. Kindly try again later!";
                    }
                }

                long logUpdateResult = 0;
                if (!stopCheckFlag)
                {
                    var sol = userData.branch_user_flag ? FinacleServices.GetFinacleSol2(userData.username, CallerFormName, callerFormMethod, callerIpAddress) : null;
                    log.maker_checker_status = (int)MakerCheckerStatus.Approved;
                    log.checker_person_id = userData.person_id;
                    log.checker_username = userData.username;
                    log.checker_fullname = $"{userData.person?.surname}, {userData.person?.first_name} {userData.person?.middle_name}";

                    log.checker_sol_id = sol?.SolId;
                    log.checker_sol_name = sol?.SolName;
                    log.checker_sol_address = sol?.SolAddress;

                    log.checker_remarks = model.CheckerRemarks;
                    log.date_checked = DateTime.UtcNow.AddHours(1);

                    logUpdateResult = MakerCheckerLogService.Update(log, CallerFormName, callerFormMethod, callerIpAddress);
                    if (logUpdateResult <= 0)
                    {
                        stopCheckFlag = true;
                        stopCheckMessage = "Unable to approve pending updated employer details at the moment; maker-checker log could not be updated. Try again later!";
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

                    AlertUser($"Successfully approved request. {insertedRecords} out of {totalRecords} have been effected.", AlertType.Success);
                }
                else
                {
                    AlertUser(stopCheckMessage, AlertType.Error);
                    return RedirectToAction("ReviewPendingUpdatedEmployerDetail", new { q = model.QueryString });
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(CallerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return RedirectToAction("ViewPendingUpdatedEmployerDetails");
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("reject-pending-updated-employer-detail")]
        public ActionResult RejectPendingUpdatedEmployerDetail(MakerCheckerLogModel model)
        {
            const string callerFormMethod = "HttpPost|RejectPendingUpdatedEmployerDetail";

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
                    stopCheckMessage = "Unable to retrieve pending updated employer detail at the moment, kindly try again later";
                }

                if (!stopCheckFlag)
                {
                    var sol = userData.branch_user_flag ? FinacleServices.GetFinacleSol2(userData.username, CallerFormName, callerFormMethod, callerIpAddress) : null;
                    log.maker_checker_status = (int)MakerCheckerStatus.Rejected;
                    log.checker_person_id = userData.person_id;
                    log.checker_username = userData.username;
                    log.checker_fullname = $"{userData.person?.surname}, {userData.person?.first_name} {userData.person?.middle_name}";

                    log.checker_sol_id = sol?.SolId;
                    log.checker_sol_name = sol?.SolName;
                    log.checker_sol_address = sol?.SolAddress;

                    log.checker_remarks = model.CheckerRemarks;
                    log.date_checked = DateTime.UtcNow.AddHours(1);

                    var logUpdateResult = MakerCheckerLogService.Update(log, CallerFormName, callerFormMethod, callerIpAddress);
                    if (logUpdateResult <= 0)
                    {
                        stopCheckFlag = true;
                        stopCheckMessage = "Unable to reject pending updated employer detail at the moment; maker-checker log could not be updated. Try again later!";
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
                    return RedirectToAction("ReviewPendingUpdatedEmployerDetail", new { q = model.QueryString });
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(CallerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return RedirectToAction("ViewPendingUpdatedEmployerDetails");
        }

        [HttpGet]
        [Route("profile-identified-salary-account")]
        public ActionResult ProfileIdentifiedSalaryAccount(string q)
        {
            if (string.IsNullOrEmpty(q))
            {
                return RedirectToAction("ViewAccount360DegreesPortfolio");
            }

            profileIdentifiedSalaryAccountViewModel = new ProfileIdentifiedSalaryAccountViewModel
            {
                AccountNumber = EncryptionUtility.Decrypt(HttpUtility.UrlDecode(q), ConfigurationUtility.GetAppSettingValue("EncryptionKey")),
                SalaryPaymentModels = new List<SalaryPaymentModel>
                {
                    new SalaryPaymentModel
                    {
                        Month = "First Month"
                    },
                    new SalaryPaymentModel
                    {
                        Month = "Second Month"
                    },
                    new SalaryPaymentModel
                    {
                        Month = "Third Month"
                    },
                    new SalaryPaymentModel
                    {
                        Month = "Fourth Month"
                    },
                    new SalaryPaymentModel
                    {
                        Month = "Fifth Month"
                    },
                    new SalaryPaymentModel
                    {
                        Month = "Sixth Month"
                    }
                }
            };

            return View(profileIdentifiedSalaryAccountViewModel);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("profile-identified-salary-account")]
        public ActionResult ProfileIdentifiedSalaryAccount(string accountNumber, List<SalaryPaymentModel> models)
        {
            const string callerFormMethod = "HttpPost|ProfileIdentifiedSalaryAccount";

            try
            {
                var checkerEmails = "";
                if (!AuthorizeRequest(Request.Url.AbsolutePath, ref checkerEmails, CallerFormName, callerFormMethod, callerIpAddress))
                {
                    AlertUser("Unauthorised Access! You may not have been permitted to access the resource. Kindly login to re-authenticate. If issue persist, contact administrator.", AlertType.Error);
                    return RedirectToAction("Logout", "Account");
                }

                if (!ValidationUtility.IsValidTextInput(accountNumber))
                {
                    ModelState.AddModelError("AccountNumber", "Account Number field is required");
                }

                var salaryPaymentModels = new List<SalaryPaymentModel>();
                for (int i = 0; i < models.Count; i++)
                {
                    if (ValidationUtility.IsValidDecimalInput(models[i].TranctionAmount) && (ValidationUtility.IsValidDateFormat(models[i].TransactionDate, "dd-MM-yyyy") || ValidationUtility.IsValidDateFormat(models[i].TransactionDate, "yyyy-MM-dd")) && models[i].FileEvidence != null && models[i].FileEvidence.ContentLength > 0)
                    {
                        //get file extension
                        var fileExtension = Path.GetExtension(models[i].FileEvidence.FileName);
                        var supportedOnBoardingDocumentsUploadFormats = ".pdf,.png,.jpg,.jpeg".Split(',');

                        //get file size
                        var fileSize = models[i].FileEvidence.ContentLength;
                        var maxUploadSize = ConfigurationUtility.GetAppSettingValue("MaxOnBoardingDocumentsUploadSize");

                        if (supportedOnBoardingDocumentsUploadFormats.Contains(fileExtension) && fileSize <= Convert.ToInt32(maxUploadSize))
                        {
                            var salaryPaymentModel = new SalaryPaymentModel
                            {
                                Month = models[i].Month,
                                TranctionAmount = models[i].TranctionAmount,
                                TransactionDate = models[i].TransactionDate,
                                FileEvidence = models[i].FileEvidence,
                                FileEvidenceSavePath = models[i].FileEvidenceSavePath
                            };

                            salaryPaymentModels.Add(salaryPaymentModel);
                        }
                    }
                }

                if (salaryPaymentModels.Count < 2)
                {
                    ModelState.AddModelError("SalaryPaymentModels", "Kindly ensure you enter the right field values in all fields of atleast two rows of the salary history table which represents atleast two salary histories. " +
                        "All fields should have the right data format. Payment Evidence file format should be either of pdf, png, jpg, jpeg formats and " +
                        "file size not more than 1MB");
                }

                #region OldCodes
                //for (int i = 0; i < salaryPaymentModels.Count; i++)
                //{
                //    if (!string.IsNullOrEmpty(salaryPaymentModels[i].TranctionAmount))
                //    {
                //        if (!ValidationUtility.IsValidDecimalInput(salaryPaymentModels[i].Amount))
                //        {
                //            ModelState.AddModelError($"{salaryPaymentModels[i].Month.Replace(" ", "")}", $"Invalid data on {salaryPaymentModels[i].Month} salary amount field");
                //        }
                //    }
                //}

                //for (int i = 0; i < salaryPaymentModels.Count; i++)
                //{
                //    if (salaryPaymentModels[i].FileEvidence == null || salaryPaymentModels[i].FileEvidence.ContentLength == 0)
                //    {
                //        ModelState.AddModelError($"{salaryPaymentModels[i].Month.Replace(" ", "")}", $"{salaryPaymentModels[i].Month} salary payment evidence is required");
                //    }
                //    else
                //    {
                //        var fileExtension = Path.GetExtension(salaryPaymentModels[i].FileEvidence.FileName);
                //        var supportedOnBoardingDocumentsUploadFormats = ".pdf,.png,.jpg,.jpeg".Split(',');
                //        if (!supportedOnBoardingDocumentsUploadFormats.Contains(fileExtension))
                //        {
                //            ModelState.AddModelError($"{salaryPaymentModels[i].Month.Replace(" ", "")}", $"{salaryPaymentModels[i].Month} salary payment evidence uploaded does not meet the required file format");
                //        }

                //        var fileSize = salaryPaymentModels[i].FileEvidence.ContentLength;
                //        var maxUploadSize = ConfigurationUtility.GetAppSettingValue("MaxOnBoardingDocumentsUploadSize");
                //        if (fileSize > Convert.ToInt32(maxUploadSize))
                //        {
                //            ModelState.AddModelError($"{salaryPaymentModels[i].Month.Replace(" ", "")}", $"{salaryPaymentModels[i].Month} salary payment evidence uploaded is more than the required file upload size");
                //        }
                //    }
                //}
                #endregion

                if (!ModelState.IsValid)
                {
                    var errorMessages = string.Join("<br/>", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
                    stopCheckFlag = true;
                    stopCheckMessage = errorMessages;
                }

                if (!stopCheckFlag)
                {
                    var sodaRac = FinacleServices.GetFromSodaRacWithAccountNumber(accountNumber, CallerFormName, callerFormMethod, callerIpAddress);
                    if (sodaRac != null)
                    {
                        stopCheckFlag = true;
                        stopCheckMessage = $"The account number: {accountNumber} is already existing on First Advance RAC. There is no need to intiate RAC profiling request.";
                    }
                }

                if (!stopCheckFlag)
                {
                    var salProfiling = FinacleServices.GetFromSalProfilingWithAccountNumber(ConfigurationUtility.GetConnectionStringValue("ObieeDbConnection"),accountNumber, CallerFormName, callerFormMethod, callerIpAddress);
                    if (salProfiling != null)
                    {
                        stopCheckFlag = true;
                        stopCheckMessage = $"The account number: {accountNumber} has already been identified for profiling. Kindly check account 360 degrees view later.";
                    }
                }

                salaryaccountsracprofiling salaryaccountsracprofiling = null;
                if (!stopCheckFlag)
                {
                    salaryaccountsracprofiling = SalaryAccountsRacProfilingService.GetWithAccountNumber(accountNumber, CallerFormName, callerFormMethod, callerIpAddress);
                    if (salaryaccountsracprofiling != null)
                    {
                        stopCheckFlag = true;
                        stopCheckMessage = $"Unable to intiate request; profiling has already been initiated on this account: {accountNumber} by {salaryaccountsracprofiling.created_by} on {salaryaccountsracprofiling.created_on}";
                    }
                }

                var makerCheckerAction = "Profile Salary Account on First Advance RAC";
                var makerCheckerActionDetails = $"Profile Salary Account: {accountNumber} on First Advance RAC";

                if (!stopCheckFlag)
                {
                    salaryaccountsracprofiling = new salaryaccountsracprofiling
                    {
                        salaryaccount = new salaryaccount
                        {
                            account_number = accountNumber,
                            created_on = DateTime.UtcNow.AddHours(1),
                            created_by = $"{userData.person?.surname}, {userData.person?.first_name} {userData.person?.middle_name}",
                            query_string = Guid.NewGuid()
                        },
                        salarypaymenthistory = salaryPaymentModels.Select(payment => new salarypaymenthistory
                        {
                            month = payment.Month,
                            amount = Convert.ToDecimal(payment.TranctionAmount),
                            transaction_date = Convert.ToDateTime(payment.TransactionDate),
                            evidence_save_path = SaveFile(payment.FileEvidence, $"{Guid.NewGuid()}-{accountNumber}-{payment.Month}", CallerFormName, callerFormMethod, callerIpAddress),
                            created_on = DateTime.UtcNow.AddHours(1),
                            created_by = $"{userData.person?.surname}, {userData.person?.first_name} {userData.person?.middle_name}",
                            query_string = Guid.NewGuid()

                        }).ToList(),
                        created_on = DateTime.UtcNow.AddHours(1),
                        created_by = $"{userData.person?.surname}, {userData.person?.first_name} {userData.person?.middle_name}",
                        query_string = Guid.NewGuid(),
                    };

                    var makerCheckerActionData = JsonConvert.SerializeObject(salaryaccountsracprofiling);
                    var sol = userData.branch_user_flag ? FinacleServices.GetFinacleSol2(userData.username, CallerFormName, callerFormMethod, callerIpAddress) : null;
                    var makerCheckerLog = new makercheckerlog
                    {
                        maker_checker_category_id = (int)MakerCheckerCategory.RacProfiling,
                        maker_checker_type_id = (int)MakerCheckerType.ProfileIdentifiedSalaryAccount,
                        action_name = makerCheckerAction,
                        action_details = makerCheckerActionDetails,
                        action_data = makerCheckerActionData,
                        maker_person_id = userData.person_id,
                        maker_username = userData.username,
                        maker_fullname = $"{userData.person?.surname}, {userData.person?.first_name} {userData.person?.middle_name}",

                        maker_sol_id = sol?.SolId,
                        maker_sol_name = sol?.SolName,
                        maker_sol_address = sol?.SolAddress,

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
                            var mailBody = System.IO.File.ReadAllText(templatePath).Replace("{ItemType}", "Profiled Salary Account on First Advance RAC");
                            var mailRecipient = checkerEmails;
                            EmailLogManager.Log(EmailType.PendingItemsNotification, mailBody, mailRecipient, "", "", CallerFormName, callerFormMethod, callerIpAddress);
                        }

                    }).Start();

                    AlertUser("Successfully submitted request for approval. Your changes will be effected once approved.", AlertType.Success);

                    return RedirectToAction("ViewAccount360DegreesPortfolio");
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

            return RedirectToAction("ProfileIdentifiedSalaryAccount", new { q = HttpUtility.UrlEncode(EncryptionUtility.Encrypt(accountNumber, ConfigurationUtility.GetAppSettingValue("EncryptionKey"))) });
        }

        [HttpGet]
        [Route("view-pending-profiled-salary-accounts")]
        public ActionResult ViewPendingProfiledSalaryAccounts()
        {
            const string callerFormMethod = "HttpGet|ViewPendingProfiledSalaryAccounts";

            try
            {
                var checkerEmails = "";
                if (!AuthorizeRequest(Request.Url.AbsolutePath, ref checkerEmails, CallerFormName, callerFormMethod, callerIpAddress))
                {
                    AlertUser("Unauthorised Access! You may not have been permitted to access the resource. Kindly login to re-authenticate. If issue persist, contact administrator.", AlertType.Error);
                    return RedirectToAction("Logout", "Account");
                }

                var sol = userData.branch_user_flag ? FinacleServices.GetFinacleSol2(userData.username, CallerFormName, callerFormMethod, callerIpAddress) : null;
                var logs = userData.branch_user_flag ? MakerCheckerLogService.GetWithMakerCheckerCategoryAndSolIdAndStatus((int)MakerCheckerCategory.RacProfiling, sol?.SolId, (int)MakerCheckerStatus.Initiated, CallerFormName, callerFormMethod, callerIpAddress) : MakerCheckerLogService.GetWithMakerCheckerCategoryAndStatus((int)MakerCheckerCategory.RacProfiling, (int)MakerCheckerStatus.Initiated, CallerFormName, callerFormMethod, callerIpAddress);
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
                        MakerSolId = !string.IsNullOrEmpty(log.maker_sol_id) ? log.maker_sol_id : "n/p",
                        MakerCheckerStatus = Convert.ToString(log.maker_checker_status),
                        DateMade = Convert.ToDateTime(log.date_made).ToString("dd-MM-yyyy hh:mm tt"),
                        CheckerId = !string.IsNullOrEmpty(log.checker_username) ? log.checker_username : "n/p",
                        CheckerFullname = !string.IsNullOrEmpty(log.checker_fullname) ? log.checker_fullname : "n/p",
                        CheckerSolId = !string.IsNullOrEmpty(log.checker_sol_id) ? log.checker_sol_id : "n/p",
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
        [Route("review-pending-profiled-salary-account")]
        public ActionResult ReviewPendingProfiledSalaryAccount(string q)
        {
            const string callerFormMethod = "HttpGet|ReviewPendingProfiledSalaryAccount";

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
                        MakerSolId = log.maker_sol_id,
                        MakerCheckerStatus = Convert.ToString(log.maker_checker_status),
                        DateMade = Convert.ToDateTime(log.date_made).ToString("dd-MM-yyyy hh:mm tt"),
                        CheckerId = log.checker_username,
                        CheckerFullname = log.checker_fullname,
                        CheckerSolId = log.checker_sol_id,
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
        [Route("approve-pending-profiled-salary-account")]
        public async Task<ActionResult> ApprovePendingProfiledSalaryAccount(MakerCheckerLogModel model)
        {
            const string callerFormMethod = "HttpPost|ApprovePendingProfiledSalaryAccount";

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
                    stopCheckMessage = "Unable to retrieve pending profiled salary account at the moment, kindly try again later";
                }

                salaryaccountsracprofiling salaryaccountsracprofiling = null;
                if (!stopCheckFlag)
                {
                    salaryaccountsracprofiling = JsonConvert.DeserializeObject<salaryaccountsracprofiling>(log.action_data);
                    var getAccountDetailsRequest = new GetAccountDetailsRequest
                    {
                        RequestId = Guid.NewGuid().ToString("N").ToUpper(),
                        CountryId = $"{ConfigurationUtility.GetAppSettingValue("CountryId")}",
                        AccountNumber = salaryaccountsracprofiling.salaryaccount.account_number
                    };

                    var getAccountDetailsResponse = await FIBridgeManager.GetAccountDetailsAsync(getAccountDetailsRequest, CallerFormName, callerFormMethod, callerIpAddress);
                    if (getAccountDetailsResponse?.ResponseCode == "00")
                    {
                        salaryaccountsracprofiling.salaryaccount.account_name = getAccountDetailsResponse?.AccountName;
                    }
                    else
                    {
                        stopCheckFlag = true;
                        stopCheckMessage = "Unable to retrieve account details at the moment, kindly try again later";
                    }
                }

                if (!stopCheckFlag)
                {
                    var sodaRac = FinacleServices.GetFromSodaRacWithAccountNumber(salaryaccountsracprofiling.salaryaccount.account_number, CallerFormName, callerFormMethod, callerIpAddress);
                    if (sodaRac != null)
                    {
                        stopCheckFlag = true;
                        stopCheckMessage = $"The account number is already existing on First Advance RAC. Kindly reject this request.";
                    }
                }

                if (!stopCheckFlag)
                {
                    var salProfiling = FinacleServices.GetFromSalProfilingWithAccountNumber(ConfigurationUtility.GetConnectionStringValue("ObieeDbConnection"),salaryaccountsracprofiling.salaryaccount.account_number, CallerFormName, callerFormMethod, callerIpAddress);
                    if (salProfiling != null)
                    {
                        stopCheckFlag = true;
                        stopCheckMessage = $"The account number has already been identified for profiling. Kindly reject this request.";
                    }
                }

                if (!stopCheckFlag)
                {
                    var checkExistingSalaryAccountRacProfile = SalaryAccountsRacProfilingService.GetWithAccountNumber(salaryaccountsracprofiling.salaryaccount.account_number, CallerFormName, callerFormMethod, callerIpAddress);
                    if (checkExistingSalaryAccountRacProfile != null)
                    {
                        stopCheckFlag = true;
                        stopCheckMessage = $"Unable to intiate request; profiling has already been created on this account by {checkExistingSalaryAccountRacProfile.created_by} on {checkExistingSalaryAccountRacProfile.created_on} and approved by {checkExistingSalaryAccountRacProfile.approved_by} on {checkExistingSalaryAccountRacProfile.approved_on}";
                    }
                }

                long applyResult = 0;
                if (!stopCheckFlag)
                {
                    salaryaccountsracprofiling.salaryaccount.approved_on = DateTime.UtcNow.AddHours(1);
                    salaryaccountsracprofiling.salaryaccount.approved_by = $"{userData.person?.surname}, {userData.person?.first_name} {userData.person?.middle_name}";
                    salaryaccountsracprofiling.salarypaymenthistory.ToList().ForEach(payment =>
                    {
                        payment.approved_on = DateTime.UtcNow.AddHours(1);
                        payment.approved_by = $"{userData.person?.surname}, {userData.person?.first_name} {userData.person?.middle_name}";
                    });
                    salaryaccountsracprofiling.approved_on = DateTime.UtcNow.AddHours(1);
                    salaryaccountsracprofiling.approved_by = $"{userData.person?.surname}, {userData.person?.first_name} {userData.person?.middle_name}";
                    
                    var racProfilingInsertResult = SalaryAccountsRacProfilingService.Insert(salaryaccountsracprofiling, CallerFormName, callerFormMethod, callerIpAddress);
                    if (racProfilingInsertResult <= 0)
                    {
                        stopCheckFlag = true;
                        stopCheckMessage = "Unable to approve pending profiled salary account at the moment; Kindly try again later!";
                    }

                    if (!stopCheckFlag) applyResult = racProfilingInsertResult;
                }

                if (!stopCheckFlag)
                {
                    if (applyResult <= 0)
                    {
                        stopCheckFlag = true;
                        stopCheckMessage = "Unable to approve pending profiled salary account at the moment. Kindly try again later!";
                    }
                }

                long logUpdateResult = 0;
                if (!stopCheckFlag)
                {
                    var sol = userData.branch_user_flag ? FinacleServices.GetFinacleSol2(userData.username, CallerFormName, callerFormMethod, callerIpAddress) : null;
                    log.maker_checker_status = (int)MakerCheckerStatus.Approved;
                    log.checker_person_id = userData.person_id;
                    log.checker_username = userData.username;
                    log.checker_fullname = $"{userData.person?.surname}, {userData.person?.first_name} {userData.person?.middle_name}";
                    log.checker_sol_id = sol?.SolId;
                    log.checker_sol_name = sol?.SolName;
                    log.checker_sol_address = sol?.SolAddress;
                    log.checker_remarks = model.CheckerRemarks;
                    log.date_checked = DateTime.UtcNow.AddHours(1);

                    logUpdateResult = MakerCheckerLogService.Update(log, CallerFormName, callerFormMethod, callerIpAddress);
                    if (logUpdateResult <= 0)
                    {
                        stopCheckFlag = true;
                        stopCheckMessage = "Unable to approve pending profiled salary account at the moment; maker-checker log could not be updated. Try again later!";
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
                    return RedirectToAction("ReviewPendingProfiledSalaryAccount", new { q = model.QueryString });
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(CallerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return RedirectToAction("ViewPendingProfiledSalaryAccounts");
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("reject-pending-profiled-salary-account")]
        public ActionResult RejectPendingProfiledSalaryAccount(MakerCheckerLogModel model)
        {
            const string callerFormMethod = "HttpPost|RejectPendingProfiledSalaryAccount";

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
                    stopCheckMessage = "Unable to retrieve pending profiled salary account at the moment, kindly try again later";
                }

                if (!stopCheckFlag)
                {
                    var sol = userData.branch_user_flag ? FinacleServices.GetFinacleSol2(userData.username, CallerFormName, callerFormMethod, callerIpAddress) : null;
                    log.maker_checker_status = (int)MakerCheckerStatus.Rejected;
                    log.checker_person_id = userData.person_id;
                    log.checker_username = userData.username;
                    log.checker_fullname = $"{userData.person?.surname}, {userData.person?.first_name} {userData.person?.middle_name}";
                    log.checker_sol_id = sol?.SolId;
                    log.checker_sol_name = sol?.SolName;
                    log.checker_sol_address = sol?.SolAddress;
                    log.checker_remarks = model.CheckerRemarks;
                    log.date_checked = DateTime.UtcNow.AddHours(1);

                    var logUpdateResult = MakerCheckerLogService.Update(log, CallerFormName, callerFormMethod, callerIpAddress);
                    if (logUpdateResult <= 0)
                    {
                        stopCheckFlag = true;
                        stopCheckMessage = "Unable to reject pending profiled salary account at the moment; maker-checker log could not be updated. Try again later!";
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
                    return RedirectToAction("ReviewPendingProfiledSalaryAccount", new { q = model.QueryString });
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(CallerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return RedirectToAction("ViewPendingProfiledSalaryAccounts");
        }

        #endregion
    }
}