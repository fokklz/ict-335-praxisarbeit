using Serilog.Configuration;
using Serilog;
using StackExchange.Redis;

namespace SaveUpBackend.Common.Serilog
{
    public static class SerilogLoggerConfigurationExtension
    {
        public static LoggerConfiguration Redis(
            this LoggerSinkConfiguration loggerConfiguration,
            IConnectionMultiplexer redis,
            string listName)
        {
            return loggerConfiguration.Sink(new RedisSink(redis, listName));
        }
    }
}
