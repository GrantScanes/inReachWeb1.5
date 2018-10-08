using System;
using Infor.Model;
using Infor.Model.SchedulerServices;
using Infor.Model.SchedulerServices.ScheduleTypes;

namespace inReachWebRebuild.ViewModels
{
    public class SecondsViewModel
    {
        public InforDataItems Props { get; set; } = new InforDataItems();
        public ScheduleType Schedule { get; set; }
        public TimeUoM Uom { get; set; }
        public string JobName { get; set; }
        public Guid Id { get; }
    }
}