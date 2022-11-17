using CurrencyExchange.API.AppServices;
using CurrencyExchange.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyExchange.API.Controller
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LogController : ControllerBase
    {
        private readonly ILogger<LogController> logger;
        private readonly IWebHostEnvironment webHostEnvironment;
        public LogController(ILogger<LogController> logger, IWebHostEnvironment webHostEnvironment)
        {
            this.logger = logger;
            this.webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public IActionResult GetLogDetails()
        {
            try
            {
                logger.LogDebug(LogMsgTemplate.Start, nameof(GetLogDetails));
                string[] debug = System.IO.Directory.GetFiles(Path.Combine(webHostEnvironment.WebRootPath, LogService.DebugFolder));
                string[] error = System.IO.Directory.GetFiles(Path.Combine(webHostEnvironment.WebRootPath, LogService.ErrorFolder));
                string[] critical = System.IO.Directory.GetFiles(Path.Combine(webHostEnvironment.WebRootPath, LogService.CriticalFolder));
                debug = debug.Select(x => Path.GetFileName(x)).ToArray();
                error = error.Select(x => Path.GetFileName(x)).ToArray();
                critical = critical.Select(x => Path.GetFileName(x)).ToArray();
                return Ok(new { debug, error, critical });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, LogMsgTemplate.Error, nameof(GetLogDetails));
                return BadRequest();
            }
        }

        [HttpGet]
        public IActionResult DownloadLog(string fileName)
        {
            try
            {
                logger.LogDebug(LogMsgTemplate.Start, nameof(DownloadLog));
                string fullPath;
                if (fileName.EndsWith(LogService.FileExtension) == false)
                {
                    logger.LogCritical("Invalid log file access {name}", fileName);
                    throw new FileNotFoundException("Selected file type not found");
                }
                if (fileName.StartsWith(LogService.FilePrefix_Debug))
                    fullPath = Path.Combine(webHostEnvironment.WebRootPath, LogService.DebugFolder, fileName);
                else if (fileName.StartsWith(LogService.FilePrefix_Error))
                    fullPath = Path.Combine(webHostEnvironment.WebRootPath, LogService.ErrorFolder, fileName);
                else if (fileName.StartsWith(LogService.FilePrefix_Critical))
                    fullPath = Path.Combine(webHostEnvironment.WebRootPath, LogService.CriticalFolder, fileName);
                else
                {
                    logger.LogCritical("Invalid log file access {name}", fileName);
                    throw new FileNotFoundException("Selected file type not found");
                }

                string stream=string.Empty;
                using (StreamReader sw = new StreamReader(fullPath, true))
                {
                    stream= sw.ReadToEnd();
                }

                //stream = System.IO.File.ReadAllText(fullPath);
                //var stream = System.IO.File.OpenRead(fullPath);

                return Ok(new { message = stream }); //new FileStreamResult(stream, "text/plain");
            }
            catch (IOException ex) when (ex.Message.EndsWith("because it is being used by another process."))
            {
                logger.LogError(ex, LogMsgTemplate.Error, nameof(DownloadLog));
                return BadRequest("File is in use");
            }
            catch (IOException ex)
            {
                logger.LogError(ex, LogMsgTemplate.Error, nameof(DownloadLog));
                return BadRequest("Unexpected read error");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, LogMsgTemplate.Error, nameof(DownloadLog));
                return BadRequest("Failed to find file specified");
            }
        }
    }
}
