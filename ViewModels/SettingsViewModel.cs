using System.Collections.Generic;
using Infor.Model.ApplicationServices;

namespace inReachWebRebuild.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        public List<Setting> Settings { get; set; } =  new List<Setting>();
      
    }

    public class SettingViewModel : BaseViewModel
    {
        public Setting Setting { get; set; }
    }
}