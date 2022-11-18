using CurrencyExchange.APIService.Interface;
using CurrencyExchange.DataBaseContext.Dapper;
using CurrencyExchange.Model;
using CurrencyExchange.Repository.Interface;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;

namespace CurrencyExchange.APIService
{
    public class CurrencyExchangeService : ICurrencyExchangeService
    {
        private readonly IDapperContext dbContext;
        private readonly ICurrencyTableRepository currencyTableRepository;
        private readonly IConfiguration configuration;
        private readonly ICurrencyExchangeRatesRepository rateRepository;
         

        private HttpClient client=new HttpClient();
        private string BaseCurrency=string.Empty;
        public CurrencyExchangeService(IDapperContext _dbContext, ICurrencyTableRepository _currencyTableRepository, 
            IConfiguration _configuration, ICurrencyExchangeRatesRepository _rateRepository)
        {
            this.dbContext = _dbContext;
            this.currencyTableRepository = _currencyTableRepository;
            this.configuration = _configuration;
            this.rateRepository = _rateRepository;
            client.BaseAddress = new Uri(configuration.GetSection("ExchangeUrl").Value);
            BaseCurrency = configuration.GetSection("BaseCurrency").Value;
           
        }

        public async Task<List<CurrencyTableModel>> GetAllCurrency()
        {
            List<CurrencyTableModel> result = new List<CurrencyTableModel>();   
            result=await currencyTableRepository.GetAllCurrency();
            return result;
        }

        public async Task<List<CurrencyExchangeRatesModel>> GetLastSevenDaysCurrencyRateByCurrencyName(string currencyName)
        {
            List<CurrencyExchangeRatesModel> result = new List<CurrencyExchangeRatesModel>();
            result = await rateRepository.GetLastSevendaysExchangeRateByCurrencyName(currencyName);
            // set First date value Percentage assign 0
            var leastDate = result.Min(x => x.CurrencyDate);
            result.Where(x => x.CurrencyDate == leastDate).ToList().ForEach(x => x.CurrencyPercentage = 0);

            //date getting desc order(max date to min date) so find next index and calculate percentage added into crrrent indexlist
            foreach(var ExchangeRate in result)
            {
                int NextIndex= result.IndexOf(ExchangeRate)+1;
                if(NextIndex!=result.Count)
                {
                    decimal diffrence = ExchangeRate.CurrencyRate - result[NextIndex].CurrencyRate;
                    decimal percentage=diffrence/ExchangeRate.CurrencyRate*100;
                    result.Where(x=> x== ExchangeRate).ToList().ForEach(x=> x.CurrencyPercentage = percentage);

                }

            }

            return result;
        }

        public async Task<int> ImportCurrency(string passingDate="")
        {
            try
            {
                Dictionary<string, string> resultContent = new Dictionary<string, string>();
                string url = Urlbuilder(passingDate);
                var result = await client.GetAsync(url);
                string apiResponse = await result.Content.ReadAsStringAsync();

                //Parsing the Json Object and get the currency Name sections
                JObject obj = JObject.Parse(apiResponse);
                var CurrencyDatas = obj.GetValue("rates");


                if (result.StatusCode == HttpStatusCode.OK)
                {
                    resultContent = JsonConvert.DeserializeObject<Dictionary<string, string>>(CurrencyDatas.ToString());
                }

                CurrencyTableModel model = new CurrencyTableModel();
                int sucess=0;

                //check the Currency Name and if it is not exist then added into the Database
                if (resultContent.Count > 0)
                {
                    dbContext.BeginTransaction();
                    foreach (var item in resultContent)
                    {
                       
                        model = await currencyTableRepository.GetCurrencyByCurrencyName(item.Key);
                        if (model == null)
                        {                            
                            sucess =await currencyTableRepository.InsertCurrencyTable(item.Key);
                            if(sucess==0)
                            {
                                if(dbContext.IsInTransaction)
                                {
                                    dbContext.RollbackTransaction();
                                }
                            }
                        }

                    }                     
                    dbContext.CommitTransaction();                   
                   
                }
                return 1;
            }
            catch (Exception ex)
            {
                if(dbContext.IsInTransaction)
                    dbContext.RollbackTransaction();
                throw ex;

            }
                    
        }

        public async Task<int> ImportCurrencyExchangeRate()
        {
            try
            {
                // get all currency
                List<CurrencyTableModel> CurrencyList = await currencyTableRepository.GetAllCurrency();

                // Get the current date and looping the last 7 days
                string passingDate = DateTime.Now.ToString("yyyy-MM-dd");               
                for (int j = 0; j <= 7; j++)
                {
                    int PreviousDayCount = 0;
                    if (j != 0)
                    {
                        PreviousDayCount = j;
                        PreviousDayCount = PreviousDayCount * -1;
                        passingDate = DateTime.Now.AddDays(PreviousDayCount).ToString("yyyy-MM-dd");
                    }
                    Dictionary<string, string> resultContent = new Dictionary<string, string>();
                    string url = Urlbuilder(passingDate);
                    var result = await client.GetAsync(url);
                    string apiResponse = await result.Content.ReadAsStringAsync();

                    JObject obj = JObject.Parse(apiResponse);
                    var CurrencyDatas = obj.GetValue("rates");

                    if (result.StatusCode == HttpStatusCode.OK)
                    {
                        resultContent = JsonConvert.DeserializeObject<Dictionary<string, string>>(CurrencyDatas.ToString());
                    }

                    if(resultContent.Count>0)
                    {
                        foreach(var currency in CurrencyList)
                        {
                            // Check the Currency and rate already Exist in database
                            // if Rate is not exist then added into database
                            var data = await rateRepository.getCurrencyRateByCurrencyIdAndCurrencyDate(currency.ID.ToString(), passingDate);

                            if(data==null)
                            {
                                if (!string.IsNullOrEmpty(resultContent[currency.CurrencyName]))
                                {
                                    decimal rate = decimal.Parse(resultContent[currency.CurrencyName], System.Globalization.NumberStyles.Float); //Convert.ToDecimal(resultContent[currency.CurrencyName]);
                                    CurrencyExchangeRatesModel model = new CurrencyExchangeRatesModel();
                                    model.CurrencyID = currency.ID;
                                    model.CurrencyRate = rate;
                                    model.CurrencyDate = Convert.ToDateTime(passingDate);
                                    await rateRepository.InsertCurrencyExchangeRate(model);
                                }
                            }
                        }
                    }


                }




                return 1;

                
               
            }
            catch(Exception ex)
            {
                if (dbContext.IsInTransaction)
                    dbContext.RollbackTransaction();
                throw ex;
            }
        }

        private string Urlbuilder(string passingDate,string CurrencyName="")
        {
            string url=string.Empty;

            if(string.IsNullOrEmpty(passingDate))
            {
                url = $"{DateTime.Now.ToString("yyy-MM-dd")}?base={BaseCurrency}";
               
            }
            else
            {
                url= $"{passingDate}?base={BaseCurrency}";
               
            }

            if(!string.IsNullOrEmpty(CurrencyName))
            {
                url += $"&symbols={CurrencyName}";
            }

            return url;

        }
    }
}
