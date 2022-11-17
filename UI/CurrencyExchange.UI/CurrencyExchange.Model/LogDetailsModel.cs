using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyExchange.Model
{
    public class LogDetailsModel
    {
        public string[] Debug { get; set; }

        public string[] Error { get; set; }

        public string[] Critical { get; set; }
    }
}
