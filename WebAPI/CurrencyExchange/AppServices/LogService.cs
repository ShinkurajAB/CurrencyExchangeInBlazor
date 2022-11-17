using Serilog;
using Serilog.Events;

namespace CurrencyExchange.API.AppServices
{
    public class LogService
    {
        public static string FilePrefix_Debug = "Debug";
        public static string FilePrefix_Error = "Error";
        public static string FilePrefix_Critical = "Critical";

        public static string FileExtension = ".log";
        private static string FileSuffix = "-" + FileExtension;

        private static readonly string BaseFolder = Path.Combine("Files", "Logs");
        public static readonly string DebugFolder = Path.Combine(BaseFolder, "Debug");
        public static readonly string ErrorFolder = Path.Combine(BaseFolder, "Error");
        public static readonly string CriticalFolder = Path.Combine(BaseFolder, "Critical");

        private static readonly long Size_10_MB = 10 * 1024 * 1024;
        private static readonly string DefaultOutputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}\t{Level:u3}\t{SourceContext}\t{Message:lj}{NewLine}{Exception}";
        private static LoggerConfiguration CreateConfig(bool isDevelopment)
        {
            var loggerConfig = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.FromLogContext();

            if (isDevelopment)
            {
                loggerConfig.WriteTo.File(
                        Path.Combine(DebugFolder, FilePrefix_Debug + FileSuffix),
                        LogEventLevel.Debug,
                        rollingInterval: RollingInterval.Day,
                        fileSizeLimitBytes: Size_10_MB,
                        rollOnFileSizeLimit: true,
                        retainedFileCountLimit: 10,
                        outputTemplate: DefaultOutputTemplate);
            }
            loggerConfig.WriteTo.File(
                    Path.Combine(ErrorFolder, FilePrefix_Error + FileSuffix),
                    LogEventLevel.Error,
                    rollingInterval: RollingInterval.Day,
                    fileSizeLimitBytes: Size_10_MB,
                    rollOnFileSizeLimit: true,
                    outputTemplate: DefaultOutputTemplate);
            loggerConfig.WriteTo.File(
                    Path.Combine(CriticalFolder, FilePrefix_Critical + FileSuffix),
                    LogEventLevel.Fatal,
                    rollingInterval: RollingInterval.Day,
                    fileSizeLimitBytes: Size_10_MB,
                    rollOnFileSizeLimit: true,
                    retainedFileCountLimit: 120,
                    outputTemplate: DefaultOutputTemplate);

            return loggerConfig;
        }
        public static void AddLogger(WebApplicationBuilder builder)
        {
            bool isDevelopment = builder.Environment.IsDevelopment();
            var loggerConfig = CreateConfig(isDevelopment);
            var logger = loggerConfig.CreateLogger();
            //builder.Host.UseSerilog(logger, true);

            builder.Logging.ClearProviders();
            builder.Logging.AddSerilog(logger, true);

            logger.Information("Logger started");
        }
    }
}
