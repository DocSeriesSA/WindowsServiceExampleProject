using Newtonsoft.Json;
using System;
using System.IO;

namespace WindowsServiceExample.ServiceLogger
{
    internal class LogHelper
    {
        private string LogBaseFolder { get { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs"); } }
        private string LogPath { get { return Path.Combine(LogBaseFolder, $"{DateTime.Now:yyyyMMdd}.log.txt"); } }
        private string LogType { get; set; }

        public LogHelper(string logType)
        {
            LogType = logType;
            Directory.CreateDirectory(LogBaseFolder);
        }

        public void Log(LogLevel type, string text)
        {
            LogDTO logInfo = new LogDTO()
            {
                Date = DateTime.Now,
                Type = LogType,
                Level = type,
                Log = text
            };

            string jsonLog = JsonConvert.SerializeObject(logInfo, Formatting.None);

            string logText = $"{jsonLog}{Environment.NewLine}";
            File.AppendAllText(LogPath, logText);
        }
    }
}