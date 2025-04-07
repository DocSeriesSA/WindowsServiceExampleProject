using Doc.ECM.APIHelper.DTO;

namespace WindowsServiceExample.ConfigHelper
{
    internal class YourCompanyConfig
    {
        public DocECMAPIParametersDTO DocECMParameters { get; set; } = new DocECMAPIParametersDTO();
        public ProcessParameters ProcessParameters { get; set; } = new ProcessParameters();

        // Add here your configuration (your apis, your parameters, etc.)
    }
}
