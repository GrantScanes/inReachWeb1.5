using inReachWebRebuild.Models;

namespace inReachWebRebuild.ViewModels
{
    public class HomePageViewModel : BaseViewModel
    {
        public AppTile PinnedTiles { get; set; } = new AppTile();
        public bool AutoScrollCaro { get; set; }
        public bool AutoCloseCharms { get; set; }
        public bool VerticalTrees { get; set; }
        public string Theme { get; set; }

    }
}