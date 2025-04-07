using System.Collections;

namespace Doc.ECM.APIHelper
{
    internal class DataSourceLoadOptionsBase
    {
        public bool? StringToLowerDefault { get; set; }
        public bool? PaginateViaPrimaryKey { get; set; }
        public bool? StringToLower { get; set; }
        public string DefaultSort { get; set; }
        public string[] PrimaryKey { get; set; }
        public bool? ExpandLinqSumType { get; set; }
        public bool? RemoteGrouping { get; set; }
        public bool? RemoteSelect { get; set; }
        public string[] PreSelect { get; set; }
        public string[] Select { get; set; }
        public SummaryInfo[] GroupSummary { get; set; }
        public SummaryInfo[] TotalSummary { get; set; }
        public IList Filter { get; set; }
        public GroupingInfo[] Group { get; set; }
        public SortingInfo[] Sort { get; set; }
        public int Take { get; set; }
        public int Skip { get; set; }
        public bool IsSummaryQuery { get; set; }
        public bool IsCountQuery { get; set; }
        public bool RequireGroupCount { get; set; }
        public bool RequireTotalCount { get; set; }
        public bool? SortByPrimaryKey { get; set; }
        public bool AllowAsyncOverSync { get; set; }
    }
}