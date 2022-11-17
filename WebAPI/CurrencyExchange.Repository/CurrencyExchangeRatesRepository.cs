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
    public class CurrencyExchangeRatesRepository : ICurrencyExchangeRatesRepository
    {
        private readonly IDapperContext dbContext;
        public CurrencyExchangeRatesRepository(IDapperContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<List<CurrencyExchangeRatesModel>> GetCurrencyExchangeRateByCurrency(string CurrencyName)
        {
            string Query = @"select rate.* from CurrencyExchangeRate rate inner join
                            CurrencyTable cur on rate.CurrencyID=cur.ID
                            where cur.CurrencyName=@CurrencyName";
            DynamicParameters parameters= new DynamicParameters();
            parameters.Add("CurrencyName", CurrencyName);
            IEnumerable<CurrencyExchangeRatesModel> model=await dbContext.QueryAsync<CurrencyExchangeRatesModel>(Query,parameters);
            return model.ToList();

        }

        public async Task<CurrencyExchangeRatesModel> getCurrencyRateByCurrencyIdAndCurrencyDate(string CurrencyId, string CurrencyDate)
        {
            try
            {
                string Query = @"select * from CurrencyExchangeRates where CurrencyDate=@CurrencyDate and CurrencyID=@CurrencyID";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("CurrencyDate", CurrencyDate);
                parameters.Add("CurrencyID", CurrencyId);
                CurrencyExchangeRatesModel model = await dbContext.QuerySingleAsync<CurrencyExchangeRatesModel>(Query, parameters);
                return model;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<CurrencyExchangeRatesModel>> GetLastSevendaysExchangeRateByCurrencyName(string CurrencyName)
        {
            string Query = @"select top 7 Exchange.ID ,Exchange.CurrencyID, curr.CurrencyName,Exchange.CurrencyRate, Exchange.CurrencyDate from CurrencyExchangeRates Exchange inner join
                                CurrencyTable curr on Exchange.CurrencyID=curr.ID
                                where curr.CurrencyName= @CurrencyName
                                order by
                                Exchange.CurrencyDate
                                desc";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("CurrencyName", CurrencyName);
            IEnumerable<CurrencyExchangeRatesModel> model = await dbContext.QueryAsync<CurrencyExchangeRatesModel>(Query, parameters);
            return model.ToList();
        }

        public async Task<int> InsertCurrencyExchangeRate(CurrencyExchangeRatesModel Model)
        {
            string Query = @"insert into CurrencyExchangeRates(CurrencyID, CurrencyDate, CurrencyRate)
                             values(@CurrencyID, @CurrencyDate, @CurrencyRate)";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("CurrencyID", Model.CurrencyID);
            parameters.Add("CurrencyDate", Model.CurrencyDate);
            parameters.Add("CurrencyRate", Model.CurrencyRate);
            int i = await dbContext.ExecuteAsync(Query, parameters);
            return i;
        }
    }
}
