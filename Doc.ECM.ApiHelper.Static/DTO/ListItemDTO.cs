using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doc.ECM.APIHelper.DTO
{
    public class ListItemDTO : IEquatable<ListItemDTO>
    {
        public string id { get; set; }
        public string text { get; set; }
        public bool selected { get; set; }
        public override int GetHashCode()
        {
            return (id ?? string.Empty).GetHashCode();
        }

        public bool Equals(ListItemDTO other)
        {
            return id == other.id;
        }
    }
}
