using CurrencyExchange.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyExchange.Service.Interface
{
    public interface ICurrencyService
    {
        Task<List<CurrencyModel>> GetAllCurrency();
        Task<List<ExchangeRateModel>> GetExchangeRateByCurrencyName(string CurrencyName);
        Task<LogDetailsModel> GetlLogDetails();
        Task<ErrorMessageModel> GetErrorMessages(string fileName);
    }
}
