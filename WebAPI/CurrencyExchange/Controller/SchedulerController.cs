using CurrencyExchange.APIService.Interface;
using CurrencyExchange.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyExchange.API.Controller
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SchedulerController : ControllerBase
    {
        private readonly ILogger<SchedulerController> logger;
        private readonly ICurrencyExchangeService CurrencyServices;
       

        public SchedulerController(ILogger<SchedulerController> logger, ICurrencyExchangeService currencyServices)
        {
            this.logger = logger;
            CurrencyServices = currencyServices;
        }

        [HttpGet]
        public async Task<IActionResult> ImportCurrency()
        {
            try
            {
                logger.LogDebug(LogMsgTemplate.Start, nameof(ImportCurrency));
                string Message = string.Empty;
                string currentDate = DateTime.Now.ToString("yyyy-MM-dd");
                int Sucess = await CurrencyServices.ImportCurrency(currentDate);
                if (Sucess > 0)
                {
                    Message = "Sucessfully Imported";
                }
                else
                {
                    Message = "Importing Faild";
                }

                logger.LogDebug(LogMsgTemplate.End, nameof(ImportCurrency));
                return Ok(Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, LogMsgTemplate.Error, nameof(ImportCurrency));
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> ImportCurrencyExchangeRate()
        {
            try
            {
                logger.LogDebug(LogMsgTemplate.Start, nameof(ImportCurrency));
                string Message = string.Empty;
               
                int Sucess = await CurrencyServices.ImportCurrencyExchangeRate();
                if (Sucess > 0)
                {
                    Message = "Sucessfully Imported";
                }
                else
                {
                    Message = "Importing Faild";
                }

                logger.LogDebug(LogMsgTemplate.End, nameof(ImportCurrency));
                return Ok(Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, LogMsgTemplate.Error, nameof(ImportCurrency));
                return BadRequest(ex.Message);
            }
        }

    }
}
