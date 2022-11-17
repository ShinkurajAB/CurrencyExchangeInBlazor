using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyExchange.DataBaseContext.Dapper
{
    public interface IDapperContext
    {
        bool AllowEmptyUpdate { get; set; }
        int DefaultTimeout { get; set; }
        bool IsInTransaction { get; }

        void BeginTransaction();
        void CommitTransaction();
        void RollbackTransaction();

        Task<IEnumerable<T>> QueryAsync<T>(string query) where T : class, new();
        Task<IEnumerable<T>> QueryAsync<T>(string query, DynamicParameters param) where T : class, new();
        Task<T> QueryFirstAsync<T>(string query, DynamicParameters param) where T : class, new();
        Task<T> QuerySingleAsync<T>(string query, DynamicParameters param);
        Task<int> ExecuteAsync(string query, DynamicParameters param);

    }
}
