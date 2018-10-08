using Telerik.Reporting;

namespace inReachWebRebuild.ViewModels
{
    public class TemplatedReportViewerViewModel
    {
        public string TemplateUri { get; set; }
        public UriReportSource ReportSource { get; set; }
        public string ReportServer { get; set; }
        public string Token { get; set; }
    }
}