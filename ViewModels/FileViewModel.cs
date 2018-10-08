using inReachWebRebuild.Models;

namespace inReachWebRebuild.ViewModels
{
    public class FileViewModel : BaseViewModel
    {
        public FileModel FileModel { get; set; }
        public AppUserState User { get; set; }
        public bool Edit { get; set; }
        public string RecordNumber { get; set; }
        public string Title { get; set; }
        public long Uri { get; set; }
    }
}