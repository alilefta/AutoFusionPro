using Serilog;

namespace AutoFusionPro.Infrastructure.Logging
{
   public class LoggingService
    {
        
        public LoggerConfiguration GetLoggerConfiguration()
        {
            return new LoggerConfiguration()
               .Enrich.FromLogContext()
               .MinimumLevel.Information()
               .WriteTo.Debug(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext} - {Message:lj}{NewLine}{Exception}")
               .WriteTo.File("Logs/log-.txt", outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext} - {Message:lj}{NewLine}{Exception}",  rollingInterval: RollingInterval.Day);
        }
    }
}
