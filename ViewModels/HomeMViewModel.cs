using System.Collections.Generic;
using inReachWebRebuild.Models;
using Infor.Model;

namespace inReachWebRebuild.ViewModels
{
    public class HomeMViewModel : BaseViewModel
    {
        public InforActions Actions { get; set; } = new InforActions();
        public AppTiles AppTiles { get; set; }
        public AppTiles UserTiles { get; set; }
        public Tile SelectedTile { get; set; }
        public AppUserState User { get; set; }
        public bool AutoScrollCaro { get; set; }
        public bool AutoCloseCharms { get; set; }
        public bool VerticalTrees { get; set; }
        public string Theme { get; set; }

        public List<string> SavedSearches { get; set; } = new List<string>();
        public bool ShowSearch { get; set; }

    }
}