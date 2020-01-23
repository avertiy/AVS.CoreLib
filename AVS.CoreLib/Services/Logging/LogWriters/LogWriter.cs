using System;
using System.Globalization;
using AVS.CoreLib.Infrastructure.Config;
using AVS.CoreLib._System.Net;
using Newtonsoft.Json;
using LogLevel= AVS.CoreLib.Data.Domain.Logging.LogLevel;
namespace AVS.CoreLib.Services.Logging.LogWriters
{
    public interface ILogWriter
    {
        //void Write(LogLevel level, string message, string details = "");
        /// <summary>
        /// Writes log message, serializing context object to json string as a log details
        /// </summary>
        void Write(string message, object context);
        void Write(string message, IResponse response);

        void Write(LogLevel level, string message, object context);
        void WriteF(FormattableString message);

        void WriteIf(bool condition, string message, string details = "");

        #region Write methods by LogLevel
        /// <summary>
        /// LogLevel = Info
        /// </summary>
        void Write(string message, string details = "");
        /// <summary>
        /// LogLevel = Debug
        /// </summary>
        void WriteDetails(string message, string details = "");
        /// <summary>
        /// LogLevel = System
        /// </summary>
        void WriteSystemDetails(string message, string details = "");
        /// <summary>
        /// LogLevel = Info_Fail
        /// </summary>
        void WriteFail(string message, string details = "");
        /// <summary>
        /// LogLevel = Warning
        /// </summary>
        void WriteWarning(string message, string details = "");
        /// <summary>
        /// LogLevel = Error
        /// </summary>
        void WriteError(string message, Exception ex);
        #endregion
    }

    public class LogWriter : ILogWriter
    {
        protected readonly ILogger Logger;
        public IFormatProvider FormatProvider { get; set; }
        public bool DetailedLogging { get; set; }
        public bool SystemLogging { get; set; }
        
        public LogWriter(ILogger logger, IAppConfig config)
        {
            Logger = logger;
            if (config.Tasks != null)
            {
                DetailedLogging = config.Tasks.DetailedLogging;
                SystemLogging = config.Tasks.SystemLogging;
            }
            FormatProvider = CultureInfo.CurrentCulture;
        }

        #region Public non virtual Write methods

        public void WriteF(FormattableString message)
        {
            Write(LogLevel.INFO, message.ToString(FormatProvider), "");
        }

        public void Write(string message, string details = "")
        {
            Write(LogLevel.INFO, message, details);
        }

        public void WriteDetails(string message, string details = "")
        {
            if (DetailedLogging)
                Write(LogLevel.DEBUG, message, details);
        }

        public void WriteSystemDetails(string message, string details = "")
        {
            if (SystemLogging)
                Write(LogLevel.SYSTEM, message, details);
        }

        public void WriteFail(string message, string details = "")
        {
            Write(LogLevel.INFO_FAIL, message, details);
        }

        public void WriteSuccess(string message, string details = "")
        {
            Write(LogLevel.INFO_SUCCESS, message, details);
        }

        public void WriteWarning(string message, string details = "")
        {
            Write(LogLevel.WARNING, message, details);
        }

        public void WriteError(string message, Exception ex)
        {
            Write(LogLevel.ERROR, message, ex.ToString());
        }

        public void WriteError(string message, string details = "")
        {
            Write(LogLevel.ERROR, message, details);
        }

        public void Write(string message, object context)
        {
            if (context == null)
                Write(LogLevel.INFO, message);
            Write(LogLevel.INFO, message, JsonConvert.SerializeObject(context));
        }

        public void Write(string message, IResponse response)
        {
            if (response == null)
            {
                WriteFail(message, "response is null");
                return;
            }

            if (response.Success == false)
            {
                WriteFail(message, response.Error);
                return;
            }

            Write(message, response.ToString());
        }

        public void Write(LogLevel level, string message, object context)
        {
            if (context == null)
                Write(level, message);
            else
            {
                var details = context == null ? "" : JsonConvert.SerializeObject(context);
                Write(level, message, details);
            }
        }

        public void WriteIf(bool condition, string message, string details = "")
        {
            if (condition)
                Write(message, details);
        }

        public void WriteDetailsIf(bool condition, string message, string details = "")
        {
            if (condition)
                WriteDetails(message, details);
        }
        #endregion

        protected virtual void Write(LogLevel level, string message, string details = "")
        {
            Logger.Write(level, message, details);
        }
    }

    

    

    
}