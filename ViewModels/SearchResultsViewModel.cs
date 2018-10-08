using Infor.Model;

namespace inReachWebRebuild.ViewModels
{
    public class SearchResultsViewModel
    {
        public InforRecordsLite Results { get; set; } = new InforRecordsLite();
       
        public bool VerticalTree { get; set; }
    }
}