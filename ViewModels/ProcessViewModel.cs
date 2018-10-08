using Infor.Model;

namespace inReachWebRebuild.ViewModels
{
    public class ProcessViewModel : BaseViewModel
    {
        public InforNodes Nodes { get; set; }
        public string RecordNumber { get; set; }
        public string Title { get; set; }
        public long Uri { get; set; }
        public InforNodes FlatNodes { get; set; }
        public long ProcessRootUri { get; set; }
    }

    public class ProcessActionsViewModel : BaseViewModel
    {
        public string Title { get; set; }
        public long Uri { get; set; }
        public InforNodes Node { get; set; }
        public string Notes { get; set; }
        public long ParentUri { get; set; }
        
    }

}