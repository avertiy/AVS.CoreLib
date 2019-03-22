namespace AVS.CoreLib.Data.Domain.Logging
{
    public enum LogLevel
    {
        DEBUG = 1,
        SYSTEM = 10,//we can use system level to write logs for scheduled tasks
        INFO = 20,
        INFO_SUCCESS =21,
        INFO_FAIL = 22,

        WARNING = 30,
        ERROR = 40,
        FATAL = 50
    }
}