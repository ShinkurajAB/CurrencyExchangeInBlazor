using CurrencyExchange.APIService.Interface;
using CurrencyExchange.Model;
using CurrencyExchange.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyExchange.API.Controller
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CurrencyController : ControllerBase
    {
        private readonly ILogger<CurrencyController> logger;
        private readonly ICurrencyExchangeService CurrencyServices;
        public CurrencyController(ILogger<CurrencyController> logger, ICurrencyExchangeService currencyServices)
        {
            this.logger = logger;
            CurrencyServices = currencyServices;
        }

             


        [HttpGet]
        public async Task<IActionResult> GetAllCurrency()
        {
            try
            {
                logger.LogDebug(LogMsgTemplate.Start, nameof(GetAllCurrency));
                List<CurrencyTableModel> list = await CurrencyServices.GetAllCurrency();
                logger.LogDebug(LogMsgTemplate.End, nameof(GetAllCurrency));
                return Ok(list);

            }
            catch (Exception ex)
            {
                logger.LogError(ex, LogMsgTemplate.Error, nameof(GetAllCurrency));
                return BadRequest(ex.Message);
            }

        }
    }
}
