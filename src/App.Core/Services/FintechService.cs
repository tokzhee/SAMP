using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Core.BusinessLogic;
using App.Core.Utilities;
using App.Database.Access;
using App.DataModels.Models;
using Newtonsoft.Json;

namespace App.Core.Services
{
    public static class FintechService
    {
        public static fintech GetWithFintechId(string fintechId, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            fintech fintech = null;

            try
            {
                using (var db = new Data())
                {
                    fintech = db.Get<fintech>(fintechId);
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return fintech;
        }
        public static fintech GetWithRelationshipManagerPersonId(string rmPersonId, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            fintech fintech = null;

            try
            {
                using (var db = new Data())
                {
                    fintech = db.Query<fintech>("select * from fintech where relationship_manager_person_id = @relationship_manager_person_id", new { relationship_manager_person_id = rmPersonId});
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return fintech;
        }
        public static fintech GetWithFintechOfficialEmailAddress(string emailAddress, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            fintech fintech = null;

            try
            {
                using (var db = new Data())
                {
                    fintech = db.Query<fintech>("select * from fintech where official_email_address = @OfficialEmailAddress", new { OfficialEmailAddress = emailAddress });
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return fintech;
        }
        public static fintech GetWithUrlQueryString(string queryString, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            fintech fintech = null;

            try
            {
                using (var db = new Data())
                {
                    fintech = db.Query<fintech>("select * from fintech where query_string = @UrlQueryString", new { UrlQueryString = queryString });
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return fintech;
        }
        public static List<fintech> GetAll(string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            List<fintech> list = null;

            try
            {
                using (var db = new Data())
                {
                    list = (List<fintech>)db.GetList<fintech>();
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return list;
        }
        public static long Insert(fintech model, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            long result = 0;

            try
            {
                using (var db = new Data())
                {
                    db.DbConnection.Open();

                    using (var dbTransaction = db.DbConnection.BeginTransaction())
                    {
                        //Add RM
                        var relationshipManagerPersonId = PersonService.Insert(model.person, db, dbTransaction, callerFormName, callerFormMethod, callerIpAddress);
                        if (relationshipManagerPersonId <= 0)
                        {
                            dbTransaction.Rollback();
                            return result;
                        }

                        //Add Fintech
                        var inputParameters = new Dictionary<string, object>
                        {
                            { "@paramCorporateName", model.corporate_name },
                            { "@paramOfficialEmailAddress", model.official_email_address },
                            { "@paramHeadOfficeAddress", model.head_office_address },
                            { "@paramRelationshipManagerStaffId", model.relationship_manager_staff_id },
                            { "@paramRelationshipManagerPersonId",  relationshipManagerPersonId},
                            { "@paramRelationshipManagerSolId", model.relationship_manager_sol_id },
                            { "@paramRelationshipManagerSolName", model.relationship_manager_sol_name },
                            { "@paramRelationshipManagerSolAddress", model.relationship_manager_sol_address },
                            { "@paramAccountNumber", model.account_number },
                            { "@paramAccountName", model.account_name },
                            { "@paramFinacleTermId", model.finacle_term_id },
                            { "@paramFeeScale", model.fee_scale },
                            { "@paramScaleValue", model.scale_value },
                            { "@paramCapAmount", model.cap_amount },
                            { "@paramCreatedOn", model.created_on },
                            { "@paramCreatedBy", model.created_by },
                            { "@paramApprovedOn", model.approved_on },
                            { "@paramApprovedBy", model.approved_by }
                        };
                        var outputParameters = new Dictionary<string, object>
                        {
                            { "@paramId", "" }
                        };

                        var fintechInsertResult = db.ExecuteProcedure<object>("sp_FintechInsertFintech", inputParameters, outputParameters, db.DbConnection, dbTransaction);
                        var fintechId = Convert.ToInt64(fintechInsertResult["@paramId"]);
                        if (fintechId <= 0)
                        {
                            dbTransaction.Rollback();
                            return result;
                        }

                        //Add On-Boarding Documents
                        var onBoardingDocuments = model.fintechonboardingdocuments.Select(document => new fintechonboardingdocument
                        {
                            fintech_id = fintechId,
                            document_id = document.document_id,
                            document_save_path = document.document_save_path,
                            created_on = document.created_on,
                            created_by = document.created_by,
                            approved_on = document.approved_on,
                            approved_by = document.approved_by

                        }).ToList();
                        var numberOfDocumentsSaved = FintechOnBoardingDocumentsService.Insert(onBoardingDocuments, db, dbTransaction, callerFormName, callerFormMethod, callerIpAddress);
                        if (!numberOfDocumentsSaved.Equals(onBoardingDocuments.Count))
                        {
                            dbTransaction.Rollback();
                            return result;
                        }

                        //Add Operation Object
                        var fee = new Fee
                        {
                            TermId = model.finacle_term_id.ToUpper(),
                            TillAccountNo = model.account_number,
                            FeeActNo1 = "",
                            FeeActNo2 = "",
                            AmtUpperBand = model.fee_scale.ToUpper().Equals("FLAT") ? "0" : model.cap_amount.ToString(),//model.cap_amount.ToString(),
                            AmtLowerBand = "0",
                            FlatRate = model.fee_scale.ToUpper().Equals("FLAT") ? Convert.ToString(model.cap_amount) : "0",
                            CatClassif = "C",
                            Percent = model.fee_scale.ToUpper().Equals("PERCENTAGE") ? Convert.ToString(model.scale_value) : "0",
                            DelFlag = "N",
                            RcreUserId = "SYSTEM",
                            RcreTime = model.created_on.ToString("M/d/yyyy"),
                            LchgUserId = "",
                            LchgTime = "",
                            BankId = "01"
                        };

                        var fintechfeeoperationlog = new fintechfeeoperationlog
                        {
                            fintech_id = fintechId,
                            operation_id = (int)FeeOperationName.Insert,
                            operation_data = JsonConvert.SerializeObject(fee),
                            initiated_on = Convert.ToDateTime(model.approved_on),
                            initiated_by = model.approved_by,
                            operation_status_id = (int)FeeOperationStatus.Pending
                        };

                        var feeOperationLogInsertResult = FintechFeeOperationLogService.Insert(fintechfeeoperationlog, db, dbTransaction, callerFormName, callerFormMethod, callerIpAddress);
                        if (feeOperationLogInsertResult <= 0)
                        {
                            dbTransaction.Rollback();
                            return result;
                        }

                        dbTransaction.Commit();
                        result = fintechId;

                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return result;
        }
        public static long Update(fintech model, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            long result = 0;

            try
            {
                using (var db = new Data())
                {
                    db.DbConnection.Open();

                    using (var dbTransaction = db.DbConnection.BeginTransaction())
                    {
                        //Add RM
                        var relationshipManagerPersonId = PersonService.Update(model.person, db, dbTransaction, callerFormName, callerFormMethod, callerIpAddress);
                        if (relationshipManagerPersonId <= 0)
                        {
                            dbTransaction.Rollback();
                            return result;
                        }

                        //Add Fintech
                        var inputParameters = new Dictionary<string, object>
                        {
                            { "@paramFintechId", model.id },
                            { "@paramCorporateName", model.corporate_name },
                            { "@paramOfficialEmailAddress", model.official_email_address },
                            { "@paramHeadOfficeAddress", model.head_office_address },
                            { "@paramRelationshipManagerStaffId", model.relationship_manager_staff_id },
                            { "@paramRelationshipManagerPersonId",  relationshipManagerPersonId},
                            { "@paramRelationshipManagerSolId", model.relationship_manager_sol_id },
                            { "@paramRelationshipManagerSolName", model.relationship_manager_sol_name },
                            { "@paramRelationshipManagerSolAddress", model.relationship_manager_sol_address },
                            { "@paramAccountNumber", model.account_number },
                            { "@paramAccountName", model.account_name },
                            { "@paramFinacleTermId", model.finacle_term_id },
                            { "@paramFeeScale", model.fee_scale },
                            { "@paramScaleValue", model.scale_value },
                            { "@paramCapAmount", model.cap_amount },
                            { "@paramLastModifiedOn", model.last_modified_on },
                            { "@paramLastModifiedBy", model.last_modified_by }
                        };
                        var outputParameters = new Dictionary<string, object>
                        {
                            { "@paramId", "" }
                        };

                        var fintechUpdateResult = db.ExecuteProcedure<object>("sp_FintechUpdateFintech", inputParameters, outputParameters, db.DbConnection, dbTransaction);
                        var fintechId = Convert.ToInt64(fintechUpdateResult["@paramId"]);
                        if (fintechId <= 0)
                        {
                            dbTransaction.Rollback();
                            return result;
                        }

                        //Add On-Boarding Documents
                        var onBoardingDocuments = model.fintechonboardingdocuments.Select(document => new fintechonboardingdocument
                        {
                            fintech_id = model.id,
                            document_id = document.document_id,
                            document_save_path = document.document_save_path,
                            last_modified_on = document.last_modified_on,
                            last_modified_by = document.last_modified_by,

                        }).ToList();

                        var numberOfDocumentsSaved = FintechOnBoardingDocumentsService.Update(onBoardingDocuments, db, dbTransaction, callerFormName, callerFormMethod, callerIpAddress);
                        if (!numberOfDocumentsSaved.Equals(onBoardingDocuments.Count))
                        {
                            dbTransaction.Rollback();
                            return result;
                        }

                        //Add Operation Object
                        var fee = new Fee
                        {
                            TermId = model.finacle_term_id.ToUpper(),
                            TillAccountNo = model.account_number,
                            FeeActNo1 = "",
                            FeeActNo2 = "",
                            AmtUpperBand = model.fee_scale.ToUpper().Equals("FLAT") ? "0" : model.cap_amount.ToString(), //model.cap_amount.ToString(),
                            AmtLowerBand = "0",
                            FlatRate = model.fee_scale.ToUpper().Equals("FLAT") ? Convert.ToString(model.cap_amount) : "0",
                            CatClassif = "C",
                            Percent = model.fee_scale.ToUpper().Equals("PERCENTAGE") ? Convert.ToString(model.scale_value) : "0",
                            DelFlag = "N",
                            RcreUserId = "",
                            RcreTime = "",
                            LchgUserId = "SYSTEM",
                            LchgTime = Convert.ToDateTime(model.last_modified_on).Date.ToString("M/d/yyyy"),
                            BankId = "01"
                        };

                        var fintechfeeoperationlog = new fintechfeeoperationlog
                        {
                            fintech_id = fintechId,
                            operation_id = (int)FeeOperationName.Update,
                            operation_data = JsonConvert.SerializeObject(fee),
                            initiated_on = Convert.ToDateTime(model.last_modified_on),
                            initiated_by = model.last_modified_by,
                            operation_status_id = (int)FeeOperationStatus.Pending
                        };

                        var feeOperationLogInsertResult = FintechFeeOperationLogService.Insert(fintechfeeoperationlog, db, dbTransaction, callerFormName, callerFormMethod, callerIpAddress);
                        if (feeOperationLogInsertResult <= 0)
                        {
                            dbTransaction.Rollback();
                            return result;
                        }

                        dbTransaction.Commit();
                        result = fintechId;
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return result;
        }
    }
}
