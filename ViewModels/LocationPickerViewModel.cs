#region Using

using System;
using Infor.Model;

#endregion

namespace inReachWebRebuild.ViewModels
{
    public class LocationPickerViewModel : BaseViewModel
    {
        public string TxtBoxTitle { get; set; }
        public bool ShowInternal { get; set; }
        public bool ShowExternal { get; set; }
        public InforLocations Locations { get; set; }
        public long SelectedLocationUri { get; set; }
        public long CurrentLocationUri { get; set; }
        public bool ShowToggles { get; set; } = true;
    }

    public class ReasignLocationViewModel : BaseViewModel
    {
        public string TxtBoxTitle { get; set; }
        public bool ShowInternal { get; set; }
        public bool ShowExternal { get; set; }
        public InforLocations Locations { get; set; }
        public DateTime DueDate { get; set; }
        public string Reason { get; set; }
        public DateTime? CurrentNodeDueDate { get; set; }
        public long NodeUri { get; set; }
        public long AddBelow { get; set; }
        public long AddAbove { get; set; }
        public long ProcessUri { get; set; }
        public long CurrentLocationUri { get; set; }
        public string CurrentLocationName { get; set; }
        public string Title { get; set; }
        public string RecordNumber { get; set; }
    }
}