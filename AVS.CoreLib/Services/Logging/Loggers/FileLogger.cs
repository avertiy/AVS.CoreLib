using System;
using System.Configuration;
using System.IO;
using AVS.CoreLib.Data.Domain.Logging;

namespace AVS.CoreLib.Services.Logging.Loggers
{
    public class FileLogger : BaseLogger
    {
        private readonly string _path;

        public FileLogger()
        {
            string path = ConfigurationManager.AppSettings["logs_directory"];
            if(string.IsNullOrEmpty(path))
                throw new ApplicationException("logs_directory app setting is missing.");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            _path = path;
            if (!_path.EndsWith("\\"))
                _path += "\\";
        }

        protected override void WriteLog(LogLevel level, string message, string details)
        {
            var logFilePath = $"{_path}{DateTime.Now:yy-MM-dd HH}-log.txt";

            //open or create file
            using (StreamWriter sw = File.AppendText(logFilePath))
            {
                sw.WriteLine($"{DateTime.Now:HH-mm:ss} {level}: {message}\r\n{details}\r\n");
            }
        }
    }
}