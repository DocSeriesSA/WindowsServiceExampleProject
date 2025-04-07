using System.Collections.Generic;

namespace Doc.ECM.APIHelper.DTO
{
    public class SearchResultsDTO
    {
        public int ObjectID { get; set; }
        public int ContentTypeID { get; set; }
        public bool IsInWorkflow { get; set; }
        public SearchResultsObjectLockDTO ObjectLock { get; set; }
        public bool IsDigitallySigned { get; set; }
        public bool IsDistributed { get; set; }
        public bool IsPendingConfirmation { get; set; }
        public bool IsLastVersion { get; set; }
        public string Distributor { get; set; }
        public int TotalComments { get; set; }
        public int TotalAttachments { get; set; }
        public int TotalAlerts { get; set; }
        public SearchResultsPermissionsDTO Permissions { get; set; }
        public bool HasPlugins { get; set; }
        public List<FieldsDTO> Fields { get; set; }
        public string ContentType { get; set; }
        public string Version { get; set; }
        public string Author { get; set; }
        public string CreationDate { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsLastItem { get; set; }
        public string Extension { get; set; }
    }
    public class SearchResultsPermissionsDTO
    {
        public bool DocumentRead { get; set; }
        public bool DocumentEdit { get; set; }
        public bool DocumentDelete { get; set; }
        public bool DocumentDownload { get; set; }
        public bool DocumentPrint { get; set; }
        public bool DocumentSign { get; set; }
        public bool DocumentExternalDistribution { get; set; }
        public bool CommentCreate { get; set; }
        public bool CommentRead { get; set; }
        public bool CommentUpdate { get; set; }
        public bool CommentDelete { get; set; }

        public bool DocumentAttachmentCreate { get; set; }
        public bool DocumentAttachmentRead { get; set; }
        public bool DocumentAttachmentUpdate { get; set; }
        public bool DocumentAttachmentDelete { get; set; }
        public bool DocumentAttachmentDownload { get; set; }
        public bool AuditRead { get; set; }
    }
    public class SearchResultsObjectLockDTO
    {
        public int UserID { get; set; }
        public string LockInfo { get; set; }
    }
}
