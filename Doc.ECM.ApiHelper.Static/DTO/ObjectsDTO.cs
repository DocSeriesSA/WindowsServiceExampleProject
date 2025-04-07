using System;
using System.Collections.Generic;
using System.Linq;

namespace Doc.ECM.APIHelper.DTO
{
    public class ObjectsDTO
    {
        public int ObjectID { get; set; }
        public int OriginalID { get; set; }
        public int ContentTypeID { get; set; }
        public string ContentType { get; set; }
        public int? Source { get; set; }
        public string IpAddress { get; set; }
        public DateTimeOffset? CreationDate { get; set; }
        public bool IsLastVersion { get; set; }
        public bool IsDigitallySigned { get; set; }
        public string Version { get; set; }
        public string Author { get; set; }
        public List<FieldsDTO> Fields { get; set; }
        public AttachmentsDTO Attachment { get; set; }

        public ObjectsDTO()
        {
            Fields = new List<FieldsDTO>();
        }
        public string FieldsString { get { return string.Join("|", Fields.Select(f => $"{f.Code}: {f.Value}")); } }
    }
    public class FieldsDTO
    {
        public int DefFieldID { get; set; }
        public string Title { get; set; }
        public FieldType Type { get; set; }
        public string Code { get; set; }
        public string Value { get; set; }
        public string Observations { get; set; }
        public bool IsRequired { get; set; }
        public bool IsVisible { get; set; }
        public bool IsReadOnly { get; set; }
        public List<ListElementsDTO> ListElements { get; set; }
        public List<string> RelatedFieldsCodes { get; set; }
        public FieldValidationsDTO Validations { get; set; }
        public FieldsDTO()
        {
            Validations = new FieldValidationsDTO();
        }
    }
    public class AttachmentsDTO
    {
        public int Id { get; set; }
        public string FileName { get; set; }

        public byte[] File { get; set; }
        public string Base64File { get; set; }
    }

    public class DocumentAttachmentDTO
    {
        public int Id { get; set; }
        public int ObjectID { get; set; }
        public string Description { get; set; }
        public string FileID { get; set; }
        public string FileName { get; set; }
        public string CreationDate { get; set; }
        public int UserID { get; set; }
        public string UserDisplayName { get; set; }
        public bool CanDisplay { get; set; }
    }

    public class ListElementsDTO
    {
        public string Id { get; set; }
        public string DisplayValue { get; set; }
        public bool Selected { get; set; }
    }
    public class FieldValidationsDTO
    {
        public string Mask { get; set; }
        public decimal? MinVal { get; set; }
        public decimal? MaxVal { get; set; }
        public int? Scale { get; set; }
        public int? MaxLength { get; set; }
    }
    public enum FieldType
    {
        String = 0,
        DateTime = 1,
        Numeric = 2,
        StaticList = 3,
        DynamicList = 4,
        MultiValueList = 5,
        DynamicTableList = 6,
    }
}
