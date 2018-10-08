namespace inReachWebRebuild.ViewModels
{
    public class NotesViewModel
    {
        public string Notes { get; set; }
        public string RecordNumber { get; set; }
        public long RecordUri { get; set; }
        public string Title { get; set; }
        public bool ReadOnly { get; set; }
        public long ProcessUri { get; set; }
    }
}