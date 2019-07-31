using System;
using System.Collections.Generic;
using AVS.CoreLib.Data.Domain.Logging;
using AVS.CoreLib.Utils;

namespace AVS.CoreLib.Services.Logging.Loggers
{
    public class ConsoleLogger : BaseLogger
    {
        public ConsoleLogger()
        {
            Level = LogLevel.SYSTEM;
        }

        protected virtual ConsoleColor GetColor(LogLevel lvl)
        {
            switch (lvl)
            {
                case LogLevel.SYSTEM:
                    return ConsoleColor.DarkYellow;
                case LogLevel.DEBUG:
                    return ConsoleColor.Cyan;
                case LogLevel.INFO:
                    return ConsoleColor.Blue;
                case LogLevel.INFO_SUCCESS:
                    return ConsoleColor.Green;
                case LogLevel.INFO_FAIL:
                    return ConsoleColor.Yellow;
                case LogLevel.WARNING:
                    return ConsoleColor.Magenta;
                case LogLevel.ERROR:
                    return ConsoleColor.DarkRed;
                case LogLevel.FATAL:
                    return ConsoleColor.White;
                default:
                    return ConsoleColor.Gray;
            }
        }

        protected virtual void PrintMessage(LogLevel level, string message)
        {
            Console.WriteLine($"{DateTime.Now:HH-mm:ss} {level}: {message}");
        }

        protected override void WriteLog(LogLevel level, string message, string details)
        {
            var color = Console.ForegroundColor;

            using (var locker = ConsoleLocker.Create())
            {
                Console.ForegroundColor = GetColor(level);
                PrintMessage(level, message);

                if (!string.IsNullOrEmpty(details))
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine($"{details}");
                }
                Console.ForegroundColor = color;
            }
        }
        
        protected internal override void WriteMany(IEnumerable<Log> logs)
        {
            using (var locker = ConsoleLocker.Create())
            {
                var color = Console.ForegroundColor;
                foreach (var log in logs)
                {
                    if (Level > log.LogLevel)
                        continue;
                    if (Filter != null && !Filter.ShouldWrite(log.LogLevel, log.ShortMessage, log.FullMessage))
                        continue;

                    Console.ForegroundColor = GetColor(log.LogLevel);
                    Console.WriteLine($"{DateTime.Now:HH-mm:ss} {log.LogLevel}: {log.ShortMessage}");

                    if (!string.IsNullOrEmpty(log.FullMessage))
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.WriteLine($"{log.FullMessage}");
                    }
                }
                Console.ForegroundColor = color;
            }
        }

        public static void TestColors()
        {
            var logger = new ConsoleLogger(){Level = LogLevel.DEBUG};
            logger.Info("Console colors test: ");
            logger.System("system");
            logger.Debug("debug");
            logger.Info("info");
            logger.Success( "info success");
            logger.Fail( "info fail");
            logger.Warning("warning");
            logger.Error("error");
            logger.Fatal("fatal", new Exception());
        }
    }

    
}