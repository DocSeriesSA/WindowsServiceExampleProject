using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doc.ECM.APIHelper.DTO
{
    public class DocECMAPIParametersDTO
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string ApiUrl { get; set; }
        public string WebSiteUrl { get; set; }
        public string Company { get; set; }
    }
}
