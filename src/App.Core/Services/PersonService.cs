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
    public static class PersonService
    {
        public static person GetWithPersonId(string personId, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            person person = null;

            try
            {
                using (var db = new Data())
                {
                    person = db.Get<person>(personId);
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return person;
        }
        public static person GetWithFintechContactPersonId(string fintechContactPersonId, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            person person = null;

            try
            {
                using (var db = new Data())
                {
                    person = db.Query<person>("select B.* from fintech_contact_persons A inner join person B on A.person_id = B.id where A.id = @FintechContactPersonId", new { FintechContactPersonId = fintechContactPersonId });
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return person;
        }
        public static person GetWithUrlQueryString(string queryString, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            person person = null;

            try
            {
                using (var db = new Data())
                {
                    person = db.Query<person>("select * from person where query_string = @UrlQueryString", new { UrlQueryString = queryString });
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return person;
        }
        public static List<person> GetAll(string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            List<person> list = null;

            try
            {
                using (var db = new Data())
                {
                    list = (List<person>)db.GetList<person>();
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return list;
        }
        public static long Insert(person model, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            long result = 0;

            try
            {
                var inputParameters = new Dictionary<string, object>
                {
                    { "@paramSurname", model.surname },
                    { "@paramFirstname", model.first_name},
                    { "@paramMiddlename", model.middle_name },
                    { "@paramMobileNumber", model.mobile_number },
                    { "@paramEmailAddress", model.email_address },
                    { "@paramPassport", model.passport },
                    { "@paramSignature", model.signature },
                    { "@paramPersonTypeId", model.person_type_id },
                    { "@paramCreatedOn", model.created_on },
                    { "@paramCreatedBy", model.created_by }
                };

                var outputParameters = new Dictionary<string, object>
                {
                    { "@paramId", "" }
                };

                using (var db = new Data())
                {
                    var insertResult = db.ExecuteProcedure<object>("sp_PersonInsertPerson", inputParameters, outputParameters);
                    result = Convert.ToInt64(insertResult["@paramId"]);
                }
                
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return result;
        }
        public static long Insert(person model, Data db, IDbTransaction dbTransaction, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            long result = 0;

            try
            {
                var inputParameters = new Dictionary<string, object>
                {
                    { "@paramSurname", model.surname },
                    { "@paramFirstname", model.first_name},
                    { "@paramMiddlename", model.middle_name },
                    { "@paramMobileNumber", model.mobile_number },
                    { "@paramEmailAddress", model.email_address },
                    { "@paramPassport", model.passport },
                    { "@paramSignature", model.signature },
                    { "@paramPersonTypeId", model.person_type_id },
                    { "@paramCreatedOn", model.created_on },
                    { "@paramCreatedBy", model.created_by }
                };

                var outputParameters = new Dictionary<string, object>
                {
                    { "@paramId", "" }
                };

                var insertResult = db.ExecuteProcedure<object>("sp_PersonInsertPerson", inputParameters, outputParameters, db.DbConnection, dbTransaction);
                result = Convert.ToInt64(insertResult["@paramId"]);
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return result;
        }
        public static long Insert(List<person> people, Data db, IDbTransaction dbTransaction, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            long result = 0;

            try
            {
                foreach (var item in people)
                {
                    var inputParameters = new Dictionary<string, object>
                    {
                        { "@paramSurname", item.surname },
                        { "@paramFirstname", item.first_name},
                        { "@paramMiddlename", item.middle_name },
                        { "@paramMobileNumber", item.mobile_number },
                        { "@paramEmailAddress", item.email_address },
                        { "@paramPassport", item.passport },
                        { "@paramSignature", item.signature },
                        { "@paramPersonTypeId", item.person_type_id },
                        { "@paramCreatedOn", item.created_on },
                        { "@paramCreatedBy", item.created_by }
                    };

                    var outputParameters = new Dictionary<string, object>
                    {
                        { "@paramId", "" }
                    };

                    var insertResult = db.ExecuteProcedure<object>("sp_PersonInsertPerson", inputParameters, outputParameters, db.DbConnection, dbTransaction);
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
        public static long Update(person model, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            long result = 0;

            try
            {
                using (var db = new Data())
                {
                    result = db.Update(model);
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return result;
        }
        public static long Update(person model, Data db, IDbTransaction dbTransaction, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            long result = 0;

            try
            {
                var inputParameters = new Dictionary<string, object>
                {
                    { "@paramSurname", model.surname },
                    { "@paramFirstname", model.first_name},
                    { "@paramMiddlename", model.middle_name },
                    { "@paramMobileNumber", model.mobile_number },
                    { "@paramEmailAddress", model.email_address },
                    { "@paramPassport", model.passport },
                    { "@paramSignature", model.signature },
                    { "@paramPersonTypeId", model.person_type_id },
                    { "@paramLastModifiedOn", model.last_modified_on },
                    { "@paramLastModifiedBy", model.last_modified_by }
                };

                var outputParameters = new Dictionary<string, object>
                {
                    { "@paramId", "" }
                };

                var updateResult = db.ExecuteProcedure<object>("sp_PersonUpdatePerson", inputParameters, outputParameters, db.DbConnection, dbTransaction);
                result = Convert.ToInt64(updateResult["@paramId"]);
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return result;
        }
    }
}
