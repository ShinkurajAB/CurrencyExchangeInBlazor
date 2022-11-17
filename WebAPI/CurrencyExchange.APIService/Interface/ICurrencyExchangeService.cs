using CurrencyExchange.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyExchange.APIService.Interface
{
    public interface ICurrencyExchangeService
    {
        Task<int> ImportCurrency(string passingDate); //passing date should be yyy-MM-dd format

        Task<List<CurrencyTableModel>> GetAllCurrency(); 

        Task<int> ImportCurrencyExchangeRate();

        Task<List<CurrencyExchangeRatesModel>> GetLastSevenDaysCurrencyRateByCurrencyName(string currencyName);
    }
}
