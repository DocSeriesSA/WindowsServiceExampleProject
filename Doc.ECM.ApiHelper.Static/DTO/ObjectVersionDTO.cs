using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doc.ECM.APIHelper.DTO
{
    public class ObjectVersionDTO
    {
        public int ObjectID { get; set; }
        public string Autor { get; set; }
        public string Date { get; set; }
        public string Text { get; set; }
        public string VersionNumber { get; set; }
        public bool HasChanges { get; set; }
    }
}
