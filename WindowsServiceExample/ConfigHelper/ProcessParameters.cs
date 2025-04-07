using System.Globalization;
using System;

namespace WindowsServiceExample.ConfigHelper
{
    internal class ProcessParameters
    {
        public int JobScheduleInMinutes { get; set; } = 30;
        public string LastUpdatedAt { get; set; } = new DateTime(1940, 1, 1).ToString("O", CultureInfo.GetCultureInfo("fr-FR"));
    }
}
