using Infor.Model;

namespace inReachWebRebuild.ViewModels
{
    public class LocationsViewModel
    {
        public InforLocations Locations { get; set; } = new InforLocations();
    }

    public class LocationViewModel
    {
        public InforLocation Location { get; set; }
        public bool IsSelected { get; set; }
        public bool AllowMultiSelect { get; set; }
    }
}