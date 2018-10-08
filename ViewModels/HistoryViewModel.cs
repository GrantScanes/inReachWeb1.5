using System.Collections.Generic;
using Infor.Model.SchedulerServices;

namespace inReachWebRebuild.ViewModels
{
    public class HistoryViewModel
    {
        public List<FluentJobExecutionHistory> History { get; set; } = new List<FluentJobExecutionHistory>();
    }
}