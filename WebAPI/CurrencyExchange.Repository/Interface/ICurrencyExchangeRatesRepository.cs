using CurrencyExchange.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyExchange.Repository.Interface
{
    public interface ICurrencyExchangeRatesRepository
    {
        Task<int> InsertCurrencyExchangeRate(CurrencyExchangeRatesModel Model);
        Task<List<CurrencyExchangeRatesModel>> GetCurrencyExchangeRateByCurrency(string CurrencyName);
        Task<CurrencyExchangeRatesModel> getCurrencyRateByCurrencyIdAndCurrencyDate(string CurrencyId,string CurrencyDate);
        Task<List<CurrencyExchangeRatesModel>> GetLastSevendaysExchangeRateByCurrencyName(string CurrencyName);
    }
}
