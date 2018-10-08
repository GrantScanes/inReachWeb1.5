using Infor.Model;

namespace inReachWebRebuild.ViewModels
{
    public class ActionsViewModel : BaseViewModel
    {
        public InforActions Actions { get; set; }
        public string RecordNumber { get; set; }
        public long RecordUri { get; set; }
        public string Title { get; set; }
        public bool VerticalView { get; set; }
        public long CurrentActionUri { get; set; }

        public bool CanAddActions { get; set; }
        //   public bool HasCurrentAction { get; set; }

    }

    public class ActionViewModel : BaseViewModel
    {
        public InforAction Action { get; set; }
        public string RecordNumber { get; set; }
        public long RecordUri { get; set; }
        public string Title { get; set; }

        public bool CanChangeDuration { get; set; }
        public bool CanReassign { get; set; }
        public bool CanComplete { get; set; }

    }

}