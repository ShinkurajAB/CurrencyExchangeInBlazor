using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyExchange.DataBaseContext.Dapper
{
    public class DapperContext: IDapperContext
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        private readonly object objLock = new object();

        private IDbConnection? _connection = null;
        private IDbTransaction? _transaction = null;

        public int DefaultTimeout { get; set; } = 120;
        public bool AllowEmptyUpdate { get; set; } = false;
        public bool IsInTransaction { get { return (_transaction != null); } }

        public DapperContext(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("TestDB");

            //_connectionString = _configuration.GetConnectionString("mysqlDB"); //MySqlAdmin
        }
        ~DapperContext()
        {
            if (_transaction != null)
                RollbackTransaction();
            if (_connection != null)
            {
                //_connection.Close();
                //_connection.Dispose();

                _connection = null;

            }
        }

        // public IDbConnection CreateConnection() => new SqlConnection(_connectionString);
        private IDbConnection GetConnection()
        {
            if (_connection == null)
            {
                lock (objLock)
                {
                    if (_connection == null)
                        _connection = new SqlConnection(_connectionString);
                    //_connection = new MySql.Data.MySqlClient.MySqlConnection(_connectionString);
                }
            }

            return _connection;
        }

        public void BeginTransaction()
        {
            if (_transaction != null)
                throw new NotSupportedException("Active transaction exist");
            var connection = GetConnection();
            if (_connection.State == ConnectionState.Open)
                _connection.Close();
            _connection.Open();
            _transaction = connection.BeginTransaction();
        }

        public void CommitTransaction()
        {
            if (_transaction == null)
                throw new NullReferenceException("No active transaction");
            _transaction.Commit();
            _transaction.Dispose();
            _transaction = null;
        }

        public void RollbackTransaction()
        {
            if (_transaction == null)
                throw new NullReferenceException("No active transaction");
            if(IsInTransaction)
                _transaction.Rollback();
            _transaction.Dispose();
            _transaction = null;
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string query) where T : class, new()
        {
            var connection = GetConnection();
            return await connection.QueryAsync<T>(query, transaction: _transaction, commandTimeout: DefaultTimeout, commandType: CommandType.Text);
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string query, DynamicParameters param) where T : class, new()
        {
            var connection = GetConnection();
            return await connection.QueryAsync<T>(query, param, transaction: _transaction, commandTimeout: DefaultTimeout, commandType: CommandType.Text);
        }

        public async Task<T> QueryFirstAsync<T>(string query, DynamicParameters param) where T : class, new()
        {
            var connection = GetConnection();
            return await connection.QueryFirstAsync<T>(query, param, transaction: _transaction, commandTimeout: DefaultTimeout, commandType: CommandType.Text);
        }

        public async Task<T> QuerySingleAsync<T>(string query, DynamicParameters param)
        {
            var connection = GetConnection();
            return await connection.QuerySingleAsync<T>(query, param: param, transaction: _transaction, commandTimeout: DefaultTimeout, commandType: CommandType.Text);
        }

        public async Task<int> ExecuteAsync(string query, DynamicParameters param)
        {
            var connection = GetConnection();
            int rowCount = await connection.ExecuteAsync(query, param, transaction: _transaction, commandTimeout: DefaultTimeout, commandType: CommandType.Text);
            if (rowCount == 0 && AllowEmptyUpdate == false)
                throw new RowNotInTableException("No records affected");
            return rowCount;
        }

    }
}
