using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyExchange.Model
{
    public class ExchangeRateModel
    {
        public int Id { get; set; }
        public int CurrencyID { get; set; }
        public DateTime CurrencyDate { get; set; }
        public decimal? CurrencyRate { get; set; }
        public string? CurrencyName { get; set; }

    }
}
