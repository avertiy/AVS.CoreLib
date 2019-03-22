using AVS.CoreLib.Data.Domain.Logging;

namespace AVS.CoreLib.Services.Logging.Filters
{
    public interface IFilter
    {
        bool ShouldWrite(LogLevel level, string message, string details);
    }
}