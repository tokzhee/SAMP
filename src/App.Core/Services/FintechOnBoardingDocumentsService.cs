using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Core.Utilities;
using App.Database.Access;
using App.DataModels.Models;

namespace App.Core.Services
{
    public static class FintechOnBoardingDocumentsService
    {
        public static List<fintechonboardingdocument> GetWithFintechId(string fintechId, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            List<fintechonboardingdocument> list = null;

            try
            {
                using (var db = new Data())
                {
                    list = (List<fintechonboardingdocument>)db.GetList<fintechonboardingdocument>("where fintech_id = @fintech_id", new { fintech_id  = fintechId});
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return list;
        }
        public static long Insert(fintechonboardingdocument model, Data db, IDbTransaction dbTransaction, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            long result = 0;

            try
            {
                var inputParameters = new Dictionary<string, object>
                {
                    { "@paramFintechId", model.fintech_id },
                    { "@paramDocumentId", model.document_id},
                    { "@paramDocumentSavePath", model.document_save_path },
                    { "@paramCreatedOn", model.created_on },
                    { "@paramCreatedBy", model.created_by },
                    { "@paramApprovedOn", model.approved_on },
                    { "@paramApprovedBy", model.approved_by }
                };

                var outputParameters = new Dictionary<string, object>
                {
                    { "@paramId", "" }
                };

                var insertResult = db.ExecuteProcedure<object>("sp_FintechOnboardingDocumentsInsertFintechOnboardingDocuments", inputParameters, outputParameters, db.DbConnection, dbTransaction);
                result = Convert.ToInt64(insertResult["@paramId"]);
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return result;
        }
        public static long Insert(List<fintechonboardingdocument> fintechonboardingdocuments, Data db, IDbTransaction dbTransaction, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            long result = 0;

            try
            {
                foreach (var item in fintechonboardingdocuments)
                {
                    var inputParameters = new Dictionary<string, object>
                    {
                        { "@paramFintechId", item.fintech_id },
                        { "@paramDocumentId", item.document_id},
                        { "@paramDocumentSavePath", item.document_save_path },
                        { "@paramCreatedOn", item.created_on },
                        { "@paramCreatedBy", item.created_by },
                        { "@paramApprovedOn", item.approved_on },
                        { "@paramApprovedBy", item.approved_by }
                    };

                    var outputParameters = new Dictionary<string, object>
                    {
                        { "@paramId", "" }
                    };

                    var insertResult = db.ExecuteProcedure<object>("sp_FintechOnboardingDocumentsInsertFintechOnboardingDocuments", inputParameters, outputParameters, db.DbConnection, dbTransaction);
                    var insertId = Convert.ToInt64(insertResult["@paramId"]);
                    if (insertId > 0)
                    {
                        result += 1;
                    }

                }

            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return result;
        }
        public static long Update(List<fintechonboardingdocument> fintechonboardingdocuments, Data db, IDbTransaction dbTransaction, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            long result = 0;

            try
            {
                foreach (var item in fintechonboardingdocuments)
                {
                    var inputParameters = new Dictionary<string, object>
                    {
                        { "@paramFintechId", item.fintech_id },
                        { "@paramDocumentId", item.document_id},
                        { "@paramDocumentSavePath", item.document_save_path },
                        { "@paramLastModifiedOn", item.last_modified_on },
                        { "@paramLastModifiedBy", item.last_modified_by }
                    };

                    var outputParameters = new Dictionary<string, object>
                    {
                        { "@paramId", "" }
                    };

                    var updatedResult = db.ExecuteProcedure<object>("sp_FintechOnboardingDocumentsUpdateFintechOnboardingDocuments", inputParameters, outputParameters, db.DbConnection, dbTransaction);
                    var updateId = Convert.ToInt64(updatedResult["@paramId"]);
                    if (updateId > 0)
                    {
                        result += 1;
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
