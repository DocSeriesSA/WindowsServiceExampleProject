using System;

namespace WindowsServiceExample.ServiceLogger
{
    internal class LogDTO
    {
        public DateTime Date { get; set; }
        public string Type { get; set; }
        public LogLevel Level { get; set; }
        public string Log { get; set; }
    }
}
