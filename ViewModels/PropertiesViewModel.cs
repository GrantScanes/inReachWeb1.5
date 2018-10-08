using Infor.Model;

namespace inReachWebRebuild.ViewModels
{
    public class PropertiesViewModel
    {
        public InforProps Properties { get; set; }
        public string RecordNumber { get; set; }
        public long RecordUri { get; set; }
        public string Title { get; set; }
    }
}