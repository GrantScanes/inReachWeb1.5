using System.Collections.Generic;
using inReachWebRebuild.Classes;
using inReachWebRebuild.Models;
using Infor.Model;

namespace inReachWebRebuild.ViewModels
{
    public class HomeViewModel
    {
        public string Title { get; set; }
        public InforSearches InReachSearches { get; set; }
        public InforSearches InScribeSearches { get; set; }
        public InforSearches InProcessSearches { get; set; }

        public AppUserState appUserState { get; set; }

        public ErrorDisplay ErrorDisplay = null;
        public InforUser User = null;
        public AppUserState AppUserState = null;

        public bool IsMobile { get; set; }

        public List<string> ValidWopiExtensions { get; set; }
    }
}