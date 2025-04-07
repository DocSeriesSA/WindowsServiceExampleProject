using System.Collections.Generic;

namespace Doc.ECM.APIHelper.DTO
{
    public class DynamicTableDTO
    {
        public int Id { get; set; }
        public int OrganizationID { get; set; }
        public string OrganizationName { get; set; }
        public string Name { get; set; }
        public List<DynamicTableColumnDTO> Columns { get; set; } = new List<DynamicTableColumnDTO>();
    }

    public class DynamicTableColumnDTO
    {
        public int Order { get; set; }
        public string Name { get; set; }
        public bool LinkableColumn { get; set; }
        public bool IsRequired { get; set; }
        public int? RelatedTableID { get; set; }
        public string RelatedTable { get; set; }
    }
}