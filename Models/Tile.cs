using System.Collections.Generic;
using System.Linq;
using Infor.Model;

namespace inReachWebRebuild.Models
{
    public class AppTiles : List<AppTile>
    {
    }

    public class AppTile
    {
        public int DisplayOrder { get; set; }
        public string Title { get; set; }
        public string IconUrl { get; set; }
        public Tiles AppTiles { get; set; } = new Tiles();
        public Tiles UserTiles { get; set; } = new Tiles();
    }

    public class Tiles : List<Tile>
    {
        public bool ContainsTitle(string title)
        {
            return this.Any(itm => itm.Title == title);
        }
    }

    public class Tile
    {
        public int DisplayOrder { get; set; }
        public string Title { get; set; }
        public string IconUrl { get; set; }
        public InforRecord Record { get; set; }
        public InforRecords Records { get; set; }
        public InforSearch Search { get; set; }
        public string AppTitle { get; set; }
        public double Left { get; set; }
        public double Top { get; set; }
        public long Count { get; set; }
    }
}