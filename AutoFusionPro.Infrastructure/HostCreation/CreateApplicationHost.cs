using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using ILogger = Serilog.ILogger;

namespace AutoFusionPro.Infrastructure.HostCreation
{
    public static class CreateApplicationHost
    {
        public static IHost GetDesignTimeHost(ILogger logger)
        {
            var builder = Host.CreateApplicationBuilder();
            builder.Logging.ClearProviders();
            builder.Logging.AddSerilog(logger);
            return builder.Build();
        }
    }
}
