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
    public static class FintechContactPersonService
    {
        public static fintechcontactperson GetWithFintechContactPersonId(string fintechContactPersonId, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            fintechcontactperson fintechcontactperson = null;

            try
            {
                using (var db = new Data())
                {
                    fintechcontactperson = db.Get<fintechcontactperson>(fintechContactPersonId);
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return fintechcontactperson;
        }
        public static fintechcontactperson GetWithContactPersonId(string contactPersonId, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            fintechcontactperson fintechcontactperson = null;

            try
            {
                using (var db = new Data())
                {
                    fintechcontactperson = db.Query<fintechcontactperson>("select * from fintech_contact_persons where person_id = @contactPersonId", new { contactPersonId });
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return fintechcontactperson;
        }
        public static fintechcontactperson GetWithContactPersonEmailAddress(string emailAddress, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            fintechcontactperson fintechcontactperson = null;

            try
            {
                using (var db = new Data())
                {
                    fintechcontactperson = db.Query<fintechcontactperson>("select A.* from fintech_contact_persons A inner join person B on A.person_id = B.id where B.email_address = @contactPersonEmailAddress", new { contactPersonEmailAddress = emailAddress });
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return fintechcontactperson;
        }
        public static List<fintechcontactperson> GetWithFintechId(string fintechId, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            List<fintechcontactperson> list = null;

            try
            {
                using (var db = new Data())
                {
                    list = (List<fintechcontactperson>)db.GetList<fintechcontactperson>("where fintech_id = @fintech_id", new { fintech_id = fintechId });
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return list;
        }
        public static List<fintechcontactperson> GetAll(string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            List<fintechcontactperson> list = null;

            try
            {
                using (var db = new Data())
                {
                    list = (List<fintechcontactperson>)db.GetList<fintechcontactperson>();
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return list;
        }
        public static long Insert(fintechcontactperson model, Data db, IDbTransaction dbTransaction, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            long result = 0;

            try
            {
                var inputParameters = new Dictionary<string, object>
                {
                    { "@paramFintechId", model.fintech_id },
                    { "@paramPersonId", model.person_id},
                    { "@paramCreatedOn", model.created_on },
                    { "@paramCreatedBy", model.created_by }
                };

                var outputParameters = new Dictionary<string, object>
                {
                    { "@paramId", "" }
                };

                var insertResult = db.ExecuteProcedure<object>("sp_FintechContactPersonsInsertFintechContactPersons", inputParameters, outputParameters, db.DbConnection, dbTransaction);
                result = Convert.ToInt64(insertResult["@paramId"]);
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return result;
        }
        public static long Insert(fintechcontactperson model, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            long result = 0;

            try
            {
                using (var db = new Data())
                {
                    db.DbConnection.Open();

                    using (var dbTransaction = db.DbConnection.BeginTransaction())
                    {
                        //Add Person
                        var personId = PersonService.Insert(model.person, db, dbTransaction, callerFormName, callerFormMethod, callerIpAddress);
                        if (personId <= 0)
                        {
                            dbTransaction.Rollback();
                            return result;
                        }

                        //Add contact Person
                        var inputParameters = new Dictionary<string, object>
                        {
                            { "@paramFintechId", model.fintech_id },
                            { "@paramPersonId", personId},
                            { "@paramCreatedOn", model.created_on },
                            { "@paramCreatedBy", model.created_by }
                        };

                        var outputParameters = new Dictionary<string, object>
                        {
                            { "@paramId", "" }
                        };

                        var fintechContactPersonInsertResult = db.ExecuteProcedure<object>("sp_FintechContactPersonsInsertFintechContactPersons", inputParameters, outputParameters, db.DbConnection, dbTransaction);
                        var fintechContactPersonId = Convert.ToInt64(fintechContactPersonInsertResult["@paramId"]);
                        if (fintechContactPersonId <= 0)
                        {
                            dbTransaction.Rollback();
                            return result;
                        }
                        
                        dbTransaction.Commit();
                        result = fintechContactPersonId;
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return result;
        }
        public static long Delete(string fintechId, Data db, IDbTransaction dbTransaction, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            long result = 0;

            try
            {
                var inputParameters = new Dictionary<string, object>
                {
                    { "@paramFintechId", fintechId }
                };

                var outputParameters = new Dictionary<string, object>
                {
                    { "@paramId", "" }
                };

                var deleteResult = db.ExecuteProcedure<object>("sp_FintechContactPersonsDeleteFintechContactPersons", inputParameters, outputParameters, db.DbConnection, dbTransaction);
                result = Convert.ToInt64(deleteResult["@paramId"]);
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return result;
        }
    }
}
