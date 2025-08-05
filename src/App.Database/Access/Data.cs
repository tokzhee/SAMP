using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using Oracle.ManagedDataAccess.Client;

namespace App.Database.Access
{
    public class Data : IData, IDisposable
    {
        public readonly IDbConnection DbConnection;
        public Data()
        {
            DbConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        }
        public Data(string connectionString, int? connectionType)
        {
            switch (connectionType)
            {
                case 1:
                    DbConnection = new OracleConnection(connectionString);
                    break;
                default:
                    DbConnection = new SqlConnection(connectionString);
                    break;
            }
        }
        
        public T Get<T>(object id) where T : class
        {
            try
            {
                return DbConnection.Get<T>(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IEnumerable<T> GetList<T>() where T : class
        {
            try
            {
                return DbConnection.GetList<T>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IEnumerable<T> GetList<T>(string conditions) where T : class
        {
            try
            {
                return DbConnection.GetList<T>(conditions);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IEnumerable<T> GetList<T>(string conditions, object parameters) where T : class
        {
            try
            {
                return DbConnection.GetList<T>(conditions, parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IEnumerable<T> GetListPaged<T>(int pageNumber, int itemsPerPage, string conditions, string orderBy, object parameters) where T : class
        {
            try
            {
                return DbConnection.GetListPaged<T>(pageNumber, itemsPerPage, conditions, orderBy, parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public Dictionary<string, string> ExecuteProcedure<T>(string procedureName, Dictionary<string, string> inputProcedureParameters, Dictionary<string, string> outputProcedureParameters) where T : class
        {
            var result = new Dictionary<string, string>();

            try
            {
                var dynamicParameters = new DynamicParameters();
                foreach (var inputParameter in inputProcedureParameters)
                {
                    dynamicParameters.Add(inputParameter.Key, inputParameter.Value);
                }
                foreach (var outputParameter in outputProcedureParameters)
                {
                    dynamicParameters.Add(outputParameter.Key, outputParameter.Value, direction: ParameterDirection.Output);
                }
                
                try
                {
                    DbConnection.Execute(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);
                    foreach (var returnOutputParameter in outputProcedureParameters)
                    {
                        result.Add(returnOutputParameter.Key, dynamicParameters.Get<string>(returnOutputParameter.Key));
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }
        public Dictionary<string, string> ExecuteProcedure<T>(string procedureName, Dictionary<string, object> inputProcedureParameters, Dictionary<string, object> outputProcedureParameters) where T : class
        {
            var result = new Dictionary<string, string>();

            try
            {
                var dynamicParameters = new DynamicParameters();
                foreach (var inputParameter in inputProcedureParameters)
                {
                    dynamicParameters.Add(inputParameter.Key, inputParameter.Value);
                }
                foreach (var outputParameter in outputProcedureParameters)
                {
                    dynamicParameters.Add(outputParameter.Key, outputParameter.Value, direction: ParameterDirection.Output);
                }

                try
                {
                    DbConnection.Execute(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);
                    foreach (var returnOutputParameter in outputProcedureParameters)
                    {
                        result.Add(returnOutputParameter.Key, dynamicParameters.Get<string>(returnOutputParameter.Key));
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }
        public Dictionary<string, string> ExecuteProcedure<T>(string procedureName, Dictionary<string, object> inputProcedureParameters, Dictionary<string, object> outputProcedureParameters, IDbConnection dbConnection, IDbTransaction dbTransaction) where T : class
        {
            var result = new Dictionary<string, string>();

            try
            {
                var dynamicParameters = new DynamicParameters();
                foreach (var inputParameter in inputProcedureParameters)
                {
                    dynamicParameters.Add(inputParameter.Key, inputParameter.Value);
                }

                foreach (var outputParameter in outputProcedureParameters)
                {
                    dynamicParameters.Add(outputParameter.Key, outputParameter.Value, direction: ParameterDirection.Output);
                }

                try
                {
                    dbConnection.Execute(procedureName, dynamicParameters, transaction: dbTransaction, commandType: CommandType.StoredProcedure);
                    foreach (var returnOutputParameter in outputProcedureParameters)
                    {
                        result.Add(returnOutputParameter.Key, dynamicParameters.Get<string>(returnOutputParameter.Key));
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }
        public long Insert<T>(T t) where T : class
        {
            try
            {
                return Convert.ToInt64(DbConnection.Insert(t));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public long Insert<T>(T t, IDbTransaction dbTransaction) where T : class
        {
            try
            {
                return Convert.ToInt64(DbConnection.Insert(t, dbTransaction));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public long Insert<T>(T t, IDbConnection dbConnection, IDbTransaction dbTransaction) where T : class
        {
            try
            {
                return Convert.ToInt64(dbConnection.Insert(t, dbTransaction));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public long Update<T>(T t) where T : class
        {
            try
            {
                return DbConnection.Update(t);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void Delete<T>(object id) where T : class
        {
            try
            {
                DbConnection.Delete(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void Delete<T>(T t) where T : class
        {
            try
            {
                DbConnection.Delete(t);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public long DeleteList<T>(string conditions, object parameters) where T : class
        {
            try
            {
                return DbConnection.DeleteList<T>(conditions, parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public long RecordCount<T>(string conditions, object parameters) where T : class
        {
            try
            {
                return DbConnection.RecordCount<T>(conditions, parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public T Query<T>(string query) where T : class
        {
            try
            {
                return DbConnection.QueryFirstOrDefault<T>(query);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public T Query<T>(string query, object parameters) where T : class
        {
            try
            {
                return DbConnection.QueryFirstOrDefault<T>(query, parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IEnumerable<T> QueryList<T>(string query) where T : class
        {
            try
            {
                return DbConnection.Query<T>(query);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IEnumerable<T> QueryList<T>(string query, object parameters) where T : class
        {
            try
            {
                return DbConnection.Query<T>(query, parameters, commandTimeout: 15);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<T> GetAsync<T>(object id) where T : class
        {
            try
            {
                return await DbConnection.GetAsync<T>(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<T>> GetListAsync<T>() where T : class
        {
            try
            {
                return await DbConnection.GetListAsync<T>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<T>> GetListAsync<T>(string conditions) where T : class
        {
            try
            {
                return await DbConnection.GetListAsync<T>(conditions);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<T>> GetListAsync<T>(string conditions, object parameters) where T : class
        {
            try
            {
                return await DbConnection.GetListAsync<T>(conditions, parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<T>> GetListPagedAsync<T>(int pageNumber, int itemsPerPage, string conditions, string orderBy, object parameters) where T : class
        {
            try
            {
                return await DbConnection.GetListPagedAsync<T>(pageNumber, itemsPerPage, conditions, orderBy, parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<long> InsertAsync<T>(T t) where T : class
        {
            try
            {
                return Convert.ToInt64(await DbConnection.InsertAsync(t));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<long> UpdateAsync<T>(T t) where T : class
        {
            try
            {
                return await DbConnection.UpdateAsync(t);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task DeleteAsync<T>(object id) where T : class
        {
            try
            {
                await DbConnection.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task DeleteAsync<T>(T t) where T : class
        {
            try
            {
                await DbConnection.DeleteAsync(t);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<long> DeleteListAsync<T>(string conditions, object parameters) where T : class
        {
            try
            {
                return await DbConnection.DeleteListAsync<T>(conditions, parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<long> RecordCountAsync<T>(string conditions, object parameters) where T : class
        {
            try
            {
                return await DbConnection.RecordCountAsync<T>(conditions, parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<T> QueryAsync<T>(string query) where T : class
        {
            try
            {
                return await DbConnection.QueryFirstOrDefaultAsync<T>(query);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<T> QueryAsync<T>(string query, object parameters) where T : class
        {
            try
            {
                return await DbConnection.QueryFirstOrDefaultAsync<T>(query, parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<T>> QueryListAsync<T>(string query) where T : class
        {
            try
            {
                return await DbConnection.QueryAsync<T>(query);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<T>> QueryListAsync<T>(string query, object parameters) where T : class
        {
            try
            {
                return await DbConnection.QueryAsync<T>(query, parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void Dispose()
        {
            try
            {
                DbConnection.Dispose();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
