using Doc.ECM.APIHelper.DTO;

namespace WindowsServiceExample.ConfigHelper
{
    internal class YourCompanyConfig
    {
        public DocECMAPIParametersDTO DocECMParameters { get; set; } = new DocECMAPIParametersDTO();
        public ProcessParameters ProcessParameters { get; set; } = new ProcessParameters();
        public YourAPIConfig YourAPIConfig { get; set; } = new YourAPIConfig();

        // Add anything else you need
    }
}
