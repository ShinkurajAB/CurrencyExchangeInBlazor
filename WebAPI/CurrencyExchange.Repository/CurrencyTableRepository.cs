using CurrencyExchange.DataBaseContext.Dapper;
using CurrencyExchange.Model;
using CurrencyExchange.Repository.Interface;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyExchange.Repository
{
    public class CurrencyTableRepository : ICurrencyTableRepository
    {
        private readonly IDapperContext dbContext;

        public CurrencyTableRepository(IDapperContext _dbContext)
        {
            this.dbContext = _dbContext;
        }

        public async Task<List<CurrencyTableModel>> GetAllCurrency()
        {
            string Query = "select * from CurrencyTable";
            IEnumerable<CurrencyTableModel> result =await dbContext.QueryAsync<CurrencyTableModel>(Query);
            return result.ToList();
        }

        public async Task<CurrencyTableModel> GetCurrencyByCurrencyName(string CurrencyName)
        {
            try
            {
                string Query = "select * from CurrencyTable where CurrencyName=@CurrencyName";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("CurrencyName", CurrencyName);
                CurrencyTableModel currencyTableModel = await dbContext.QueryFirstAsync<CurrencyTableModel>(Query, parameters);
                return currencyTableModel;
            }
            catch
            {
                return null;
            }
        }

        public async Task<int> InsertCurrencyTable(string CurrencyName)
        {
            string Query = @"insert into CurrencyTable(CurrencyName)
                             values(@CurrencyName)";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("CurrencyName", CurrencyName);             
            int i = await dbContext.ExecuteAsync(Query, parameters);
            return i;
        }

        
    }
}
