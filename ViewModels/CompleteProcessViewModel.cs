using Infor.Model;

namespace inReachWebRebuild.ViewModels
{
    public class CompleteProcessViewModel : BaseViewModel
    { 
        public InforProcessStamps Stamps { get; set; }
        public string Title { get; set; }
        public string RecordNumber { get; set; }
        public long Uri { get; set; }
        public long RootUri { get; set; }
    }
}