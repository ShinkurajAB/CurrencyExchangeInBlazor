using CurrencyExchange.APIService.Interface;
using CurrencyExchange.Model;
using CurrencyExchange.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace CurrencyExchange.API.Controller
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ExchangeRateController : ControllerBase
    {
        private readonly ILogger<ExchangeRateController> logger;
        private readonly ICurrencyExchangeService CurrencyServices;
        public ExchangeRateController(ILogger<ExchangeRateController> logger, ICurrencyExchangeService currencyServices)
        {
            this.logger = logger;
            CurrencyServices = currencyServices;
        }


        [HttpGet]
        public async Task<IActionResult> GetLastSevendaysCurrencyExchangeRateByCurrencyName(string CurrencyName)
        {
            try
            {
                logger.LogDebug(LogMsgTemplate.Start, nameof(GetLastSevendaysCurrencyExchangeRateByCurrencyName));
                List<CurrencyExchangeRatesModel> ExchangeRateList = await CurrencyServices.GetLastSevenDaysCurrencyRateByCurrencyName(CurrencyName);
                logger.LogDebug(LogMsgTemplate.End, nameof(GetLastSevendaysCurrencyExchangeRateByCurrencyName));
                return Ok(ExchangeRateList);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, LogMsgTemplate.Error, nameof(GetLastSevendaysCurrencyExchangeRateByCurrencyName));
                return BadRequest(ex.Message);
            }
        }
    }
}
