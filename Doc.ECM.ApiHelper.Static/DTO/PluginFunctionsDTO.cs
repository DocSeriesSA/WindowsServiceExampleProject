using System.Collections.Generic;

namespace Doc.ECM.APIHelper.DTO
{
    public class PluginActionDTO
    {
        public int Action { get; set; }
        public string Data { get; set; }
    }
    public class CreateOrUpdateInternalTableDTO
    {
        public string TableName { get; set; }
        public List<string> ColumnNames { get; set; }
    }
    public class GetDataInternalTableDTO
    {
        public string TableName { get; set; }
        public List<InternalTableFilterDTO> Filters { get; set; }
    }

    public class SaveDataInternalTableDTO
    {
        public string TableName { get; set; }
        public List<InternalTableRowDTO> Rows { get; set; } = new List<InternalTableRowDTO>();
    }
    public class InternalTableRowDTO
    {
        public string Id { get; set; }
        public List<InternalTableCellDTO> Cells { get; set; } = new List<InternalTableCellDTO>();
    }
    public class InternalTableCellDTO
    {
        public string ColumnName { get; set; }
        public string Value { get; set; }
    }
    public class InternalTableFilterDTO
    {
        public string ColumnName { get; set; }
        public string Value { get; set; }
        public string Operator { get; set; }
    }
}
