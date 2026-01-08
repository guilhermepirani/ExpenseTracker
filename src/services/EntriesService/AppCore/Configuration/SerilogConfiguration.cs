using Serilog;
using Serilog.Events;

namespace AppCore.Configuration;

public static class SerilogConfiguration
{
    public static ILogger ConfigureSerilog()
    {
        var logOutput = "[{Timestamp:HH:mm:ss} {Level:u3}] [{TraceId}] {Message:lj}{NewLine}{Exception}";
        return Log.Logger = new LoggerConfiguration()
           .Enrich.FromLogContext()
           .WriteTo.Console(
               restrictedToMinimumLevel: LogEventLevel.Debug,
               outputTemplate: logOutput)
           .WriteTo.File("logs/apiLogFile.txt", outputTemplate: logOutput)
           .CreateLogger();
    }
}