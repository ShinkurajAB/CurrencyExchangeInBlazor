using CurrencyExchange.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyExchange.Repository.Interface
{
    public interface ICurrencyTableRepository
    {
        Task<int> InsertCurrencyTable(string CurrencyName);
        Task<List<CurrencyTableModel>> GetAllCurrency();
        Task<CurrencyTableModel> GetCurrencyByCurrencyName(string CurrencyName);
    }
}
