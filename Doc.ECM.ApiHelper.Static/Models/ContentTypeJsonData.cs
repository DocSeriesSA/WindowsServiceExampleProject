using System.ComponentModel.DataAnnotations;

namespace Doc.ECM.APIHelper.Models
{
    /// <summary>
    /// A search request to retrieve documents
    /// </summary>
    public class ContentTypeJsonData
    {
        /// <remarks>
        ///The text to search.
        /// </remarks>
        public string ContentTypeStr { get; set; }
    }
}