using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Doc.ECM.APIHelper.DTO
{
    public class ContentTypeDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public short MajVersion { get; set; }
        public short MinVersion { get; set; }
        public bool IsLastVersion { get; set; }
        public int OrganizationID { get; set; }
        public string OrganizationName { get; set; }
        public bool EncryptDocuments { get; set; }
        public List<DefFieldDTO> DefFields { get; set; } = new List<DefFieldDTO>();
        public bool HasDocuments { get; set; }
        public bool KeepActualVersion { get; set; }
        public string Version => string.Format("v{0}.{1}", MajVersion, MinVersion);
    }
    public class ContentTypeExportConfigDTO
    {
        public bool IncludeOriginalFile { get; set; }
        public bool ExportMultipleFiles { get; set; }
        public string ExportFileNamePrefix { get; set; }
        public string ExportFileNameSuffix { get; set; }
        public string ExportFileNameExtension { get; set; }
        public string ExportFilePath { get; set; }
        public int ExportEncoding { get; set; }
        public string ConditionStr { get; set; }
        public string ExportMask { get; set; }
    }

    public partial class ContentTypesRightsDTO
    {
        public int Id { get; set; }
        public int RoleTypeID { get; set; }
        public string RoleTypeName { get; set; }
        public int AccessRightId { get; set; }
        public string AccessRightName { get; set; }
        public int DigitalSignatureConfigId { get; set; }
        public string DigitalSignatureConfigName { get; set; }
        public string ConditionStr { get; set; }
        public string DisplayConditionStr { get; set; }
    }

    public class ContentTypeAlertDTO
    {
        public string ConditionStr { get; set; }
        public string DisplayConditionStr { get; set; }
        public bool NotifyByEmail { get; set; }
        public List<ContentTypeAlertActionDTO> Actions { get; set; }
        public List<int> RoleTypeIDs { get; set; }
        public int ContentTypeAlertTriggerID { get; set; }
    }
    public class ContentTypeAlertActionDTO
    {
        public string DefFieldCode { get; set; }
        public string Value { get; set; }
    }
    public class DefFieldDTO
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Default { get; set; }
        public string Comments { get; set; }
        public string Type { get; set; }
        public int Order { get; set; }
        public bool IsRequired { get; set; }
        public bool IsVisible { get; set; }
        public bool IsReadOnly { get; set; }
        public bool IsUnique { get; set; }
        public string ContentTypeName { get; set; }
        public FieldType FieldType { get; set; }
    }

    public class DefFieldDateTimeDTO : DefFieldDTO
    {
        public string Mask { get; set; }
    }

    public class DefFieldNumericDTO : DefFieldDTO
    {
        public string Mask { get; set; }
        public decimal MinVal { get; set; }
        public decimal MaxVal { get; set; }
        public int Scale { get; set; }
        public string DecSep { get; set; }
        public string ThouSep { get; set; }
    }

    public class DefFieldStringDTO : DefFieldDTO
    {
        public int MaxLength { get; set; }
    }

    public class DefFieldListDTO : DefFieldDTO
    {
        public string ListType { get; set; }
        public string SelMode { get; set; }
        public string CnnStr { get; set; }
        public string Command { get; set; }
        public string NoResultQuery { get; set; }
        public int? DynamicTableID { get; set; }
        public bool UseODBC { get; set; }
        public List<string> RelatedFieldList { get; set; } = new List<string>();
        public List<DefFieldListElementDTO> ListElements { get; set; } = new List<DefFieldListElementDTO>();
        public DefFieldListDynamicTableDTO DynamicTable { get; set; }
    }

    public class DefFieldListElementDTO
    {
        public int Id { get; set; }
        public string Value { get; set; }
        public int Order { get; set; }
    }
    public class DefFieldListDynamicTableDTO
    {
        public string[] DisplayColumns { get; set; }
        public List<DefFieldListDynamicTableFilterDTO> Filters { get; set; }
        public string SortColumn { get; set; }
        public DynamicTableDTO TableData { get; set; }
    }
    public class DefFieldListDynamicTableFilterDTO
    {
        public string FilterColumn { get; set; }
        public string FilterValue { get; set; }
        public string FilterOperator { get; set; } = "";
    }
    public class DefFieldConditionDTO
    {
        public int Id { get; set; }
        public int DefFieldID { get; set; }
        public string DefFieldName { get; set; }
        public string ConditionStr { get; set; }
        public string DisplayConditionStr { get; set; }
        public int RoleTypeID { get; set; }
        public string RoleTypeName { get; set; }
        public bool Visible { get; set; }
        public bool Readonly { get; set; }
    }

    public class DynamicListParamDTO
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public bool Multivalue { get; set; } = false;
    }

    public class ContentTypeImportDTO
    {
        public string ContentTypeStr { get; set; }
        public ContentTypeDTO ContentType { get; set; }
        public List<string> RolesNames { get; set; } = new List<string>();
    }

    public class DynamicListTestDto
    {
        public string ConnectionString { get; set; }
        public string Query { get; set; }
        public string NoResultQuery { get; set; }
        public bool UseODBC { get; set; }
        public List<DynamicListParamDTO> Parameters { get; set; } = new List<DynamicListParamDTO>();
        public int? InternaTableId { get; set; }
    }
}