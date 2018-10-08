using Infor.Model;

namespace inReachWebRebuild.ViewModels
{
    public class SearchResultsNodesViewModel
    {
        public InforNodesLite Results { get; set; } = new InforNodesLite();
        public bool VerticalTree { get; set; }
        public long ProcessRootUri { get; set; }
    }
}