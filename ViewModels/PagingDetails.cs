namespace inReachWebRebuild.ViewModels
{
    /// <summary>
    /// Contains information 
    /// </summary>
    public class PagingDetails
    {
        public bool RenderPager = true;

        public int Page = 1;
        public int PageCount = 1;
        public int PageSize = 0;
        public int TotalPages = 1;
        public int TotalItems = 0;

        public int MaxPageButtons = 10;

        /// <summary>
        /// Client handler function called on POST operation with page number as parameter
        /// </summary>
        public string ClientPageClickHandler = "pageClick";
    }
}