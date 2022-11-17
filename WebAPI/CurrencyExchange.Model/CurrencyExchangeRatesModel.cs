using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyExchange.Model
{
    public class CurrencyExchangeRatesModel
    {
        public int ID { get; set; }
        public int CurrencyID { get; set; }
        public DateTime? CurrencyDate { get; set; }
        public decimal CurrencyRate { get; set; }
        public string? CurrencyName { get; set; }
        public decimal CurrencyPercentage { get; set; }

    }
}
