using CurrencyExchange.Model;
using CurrencyExchange.Service.Interface;
using Microsoft.Extensions.Configuration;
using System.Net;
using Newtonsoft.Json;

namespace CurrencyExchange.Service
{
    public class CurrencyService : ICurrencyService
    {

        private readonly IConfiguration _configuration;

        private HttpClient _httpclient = new HttpClient();


        public CurrencyService(IConfiguration configuration)
        {
            _configuration = configuration;
            _httpclient.BaseAddress = new Uri(_configuration.GetSection("APIService").Value);
        }


        public async Task<List<CurrencyModel>> GetAllCurrency()
        {
            var result =await _httpclient.GetAsync("api/Currency/GetAllCurrency");
            string apiresponse = await result.Content.ReadAsStringAsync();

            if (result.StatusCode == HttpStatusCode.OK)
            {
                return JsonConvert.DeserializeObject<List<CurrencyModel>>(apiresponse); ;
            }

            return null;

        }



        public async Task<List<ExchangeRateModel>> GetExchangeRateByCurrencyName(string CurrencyName)
        {
            var result = await _httpclient.GetAsync(string.Format("api/ExchangeRate/GetLastSevendaysCurrencyExchangeRateByCurrencyName?CurrencyName={0}",CurrencyName));
            string apiresponse = await result.Content.ReadAsStringAsync();

            if (result.StatusCode == HttpStatusCode.OK)
            {
                return JsonConvert.DeserializeObject<List<ExchangeRateModel>>(apiresponse); ;
            }

            return null;
        }

        public async Task<LogDetailsModel> GetlLogDetails()
        {
            var result = await _httpclient.GetAsync("api/Log/GetLogDetails");
            string apiresponse = await result.Content.ReadAsStringAsync();

            if (result.StatusCode == HttpStatusCode.OK)
            {
                return JsonConvert.DeserializeObject<LogDetailsModel>(apiresponse); ;
            }

            return null;
        }

        public async Task<ErrorMessageModel> GetErrorMessages(string fileName)
        {
            var result = await _httpclient.GetAsync(string.Format("api/Log/DownloadLog?fileName={0}",fileName));
            string apiresponse = await result.Content.ReadAsStringAsync();

            if (result.StatusCode == HttpStatusCode.OK)
            {
                return JsonConvert.DeserializeObject<ErrorMessageModel>(apiresponse); ;
            }

            return null;
        }
    }
}
