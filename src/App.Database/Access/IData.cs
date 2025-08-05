using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace App.Database.Access
{
    public interface IData
    {
        T Get<T>(object id) where T : class;
        IEnumerable<T> GetList<T>() where T : class;
        IEnumerable<T> GetList<T>(string conditions) where T : class;
        IEnumerable<T> GetList<T>(string conditions, object parameters) where T : class;
        IEnumerable<T> GetListPaged<T>(int pageNumber, int itemsPerPage, string conditions, string orderBy, object parameters) where T : class;
        Dictionary<string, string> ExecuteProcedure<T>(string procedureName, Dictionary<string, string> inputProcedureParameters, Dictionary<string, string> outputProcedureParameters) where T : class;
        Dictionary<string, string> ExecuteProcedure<T>(string procedureName, Dictionary<string, object> inputProcedureParameters, Dictionary<string, object> outputProcedureParameters) where T : class;
        Dictionary<string, string> ExecuteProcedure<T>(string procedureName, Dictionary<string, object> inputProcedureParameters, Dictionary<string, object> outputProcedureParameters, IDbConnection dbConnection, IDbTransaction dbTransaction) where T : class;
        long Insert<T>(T t) where T : class;
        long Insert<T>(T t, IDbTransaction dbTransaction) where T : class;
        long Insert<T>(T t, IDbConnection dbConnection, IDbTransaction dbTransaction) where T : class;
        long Update<T>(T t) where T : class;
        void Delete<T>(object id) where T : class;
        void Delete<T>(T t) where T : class;
        long DeleteList<T>(string conditions, object parameters) where T : class;
        long RecordCount<T>(string conditions, object parameters) where T : class;
        T Query<T>(string query) where T : class;
        T Query<T>(string query, object parameters) where T : class;
        IEnumerable<T> QueryList<T>(string query) where T : class;
        IEnumerable<T> QueryList<T>(string query, object parameters) where T : class;
        Task<T> GetAsync<T>(object id) where T : class;
        Task<IEnumerable<T>> GetListAsync<T>() where T : class;
        Task<IEnumerable<T>> GetListAsync<T>(string conditions) where T : class;
        Task<IEnumerable<T>> GetListAsync<T>(string conditions, object parameters) where T : class;
        Task<IEnumerable<T>> GetListPagedAsync<T>(int pageNumber, int itemsPerPage, string conditions, string orderBy, object parameters) where T : class;
        Task<long> InsertAsync<T>(T t) where T : class;
        Task<long> UpdateAsync<T>(T t) where T : class;
        Task DeleteAsync<T>(object id) where T : class;
        Task DeleteAsync<T>(T t) where T : class;
        Task<long> DeleteListAsync<T>(string conditions, object parameters) where T : class;
        Task<long> RecordCountAsync<T>(string conditions, object parameters) where T : class;
        Task<T> QueryAsync<T>(string query) where T : class;
        Task<T> QueryAsync<T>(string query, object parameters) where T : class;
        Task<IEnumerable<T>> QueryListAsync<T>(string query) where T : class;
        Task<IEnumerable<T>> QueryListAsync<T>(string query, object parameters) where T : class;
    }
}
