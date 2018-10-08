using System.Collections.Generic;
using Infor.Model;

namespace inReachWebRebuild.DataSource
{
    

    public class DataSources
    {
  

        private static DataSources _instance = null;
        public static DataSources Instance => _instance ?? (_instance = new DataSources());
        public List<RLocation> Locations { get; set; }

        

        private DataSources()
        {
            this.Reset();
           
        }
        public void Reset()
        {
            this.Locations = new List<RLocation>();
            
        }
       
    }
}