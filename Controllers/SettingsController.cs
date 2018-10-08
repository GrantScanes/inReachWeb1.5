using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using inReachWebRebuild.Classes;
using inReachWebRebuild.ViewModels;
using Infor.Model.ApplicationServices;
using Newtonsoft.Json;

namespace inReachWebRebuild.Controllers
{
    public class SettingsController : BaseController
    {
        private readonly SettingsViewModel _viewModel = new SettingsViewModel();
        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);

           
            _viewModel.ErrorDisplay = ErrorDisplay;
            _viewModel.AppUserState = AppUserState;

            ViewData["UserState"] = AppUserState;
        }

        // GET: Settings
        public ActionResult Index()
        {

            if (AppUserState.Name == "Admin" && AppUserState.UserId == 123456)
            {
                SettingsManager.Initiase(Path.Combine(AppDomain.CurrentDomain.GetData("DataDirectory").ToString(), "inReachWeb"));
                _viewModel.Settings = SettingsManager.Settings ;
                return View(_viewModel);
            }
          

            ErrorDisplay.ShowError($"Sign In");
            return View(_viewModel);
        }

        public ActionResult Print()
        {
            _viewModel.Settings = SettingsManager.Settings;
            return View(_viewModel);
        }

        public bool SaveSettings(string settingsString)
        {
            var settings = JsonConvert.DeserializeObject<List<dynamic>>(settingsString);
            foreach (var setting in settings)
            {
                SettingsManager.GetSettingByKey((string)setting["Id"]).Value = setting.Value;
                SettingsManager.GetSettingByKey((string)setting["Id"]).Collapsed = setting.Collapsed;
            }
            SettingsManager.Save();
            return true;
        }
        public bool SaveCollapsed(string settingsString)
        {
            var settings = JsonConvert.DeserializeObject<List<dynamic>>(settingsString);
            foreach (var setting in settings)
            {
               SettingsManager.GetSettingByKey((string)setting["Id"]).Collapsed = setting.Collapsed;
            }
            SettingsManager.Save();
            return true;
        }

        public bool RefreshSettings()
        {
            SettingsManager.Initiase(Path.Combine(AppDomain.CurrentDomain.GetData("DataDirectory").ToString(), "inReachWeb"));
            return true;
        }
    }
}