using System;
using AVS.CoreLib.Data.Domain.Logging;

namespace AVS.CoreLib.Services.Logging.Extensions
{
    public static class LoggerExtensions
    {
        public static void System<T>(this ILogger logger, string message, string details = "")
        {
            logger.System($"{typeof(T).Name} => {message}", details);
        }

        public static void Debug<T>(this ILogger logger, string message, string details = "")
        {
            logger.Debug($"{typeof(T).Name} => {message}", details);
        }

        public static void Info<T>(this ILogger logger, string message, string details = "")
        {
            logger.Info($"{typeof(T).Name} => {message}", details);
        }

        public static void Success<T>(this ILogger logger, string message, string details = "")
        {
            logger.Success($"{typeof(T).Name} => {message}", details);
        }

        public static void Fail<T>(this ILogger logger, string message, string details = "")
        {
            logger.Fail($"{typeof(T).Name} => {message}", details);
        }

        public static void Warning<T>(this ILogger logger, string message, string details = "")
        {
            logger.Warning($"{typeof(T).Name} => {message}", details);
        }

        public static void Error<T>(this ILogger logger, string message, string details = "")
        {
            logger.Error($"{typeof(T).Name} => {message}", details);
        }

        public static void Error<T>(this ILogger logger, string message, Exception exception)
        {
            logger.Error($"{typeof(T).Name} => {message}", exception);
        }

        public static bool IsDetailedLogging(this LogLevel lvl)
        {
            return lvl <= LogLevel.SYSTEM;
        }
    }
}