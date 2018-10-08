using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Infor.Model;
using Infor.Model.ApplicationServices;
using Infor.Model.SchedulerServices;
using Infor.Model.SchedulerServices.JobTypes;
using Infor.Model.WebServices.LocationServices;
using MohammadYounes.Owin.Security.MixedAuth;
using Newtonsoft.Json;
using NLog;
using Telerik.Reporting.Services.WebApi;
using WebApiContrib.Formatting.Jsonp;

namespace inReachWebRebuild
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public MvcApplication()
        {
            this.RegisterMixedAuth();
        }
        public static FluentRegistry ExportJobs { get; set; }
        protected void Application_Start()
        {
            Logger.Info($"App start");
            ReportsControllerConfiguration.RegisterRoutes(GlobalConfiguration.Configuration);
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            GlobalConfiguration.Configuration.AddJsonpFormatter();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AntiForgeryConfig.SuppressXFrameOptionsHeader = true;


            try
            {
                SettingsManager.Initiase(Path.Combine(AppDomain.CurrentDomain.GetData("DataDirectory").ToString(), "inReachWeb"));
                SchedulerManager.AddJobs(JobTypeEnum.ExportJob);
                SchedulerManager.ClearAllJobs();
                SchedulerManager.AddJobs(JobTypeEnum.InFuseExportJob);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Logger.Error($"Error loading settings {e.Message} at {e.StackTrace}");
            }
            var startTime = DateTime.Now;

            var cachePath = HttpContext.Current.Server.MapPath($"~/App_Data/inReachRepos/Global/Cache");
            Directory.CreateDirectory(cachePath);
            var locCache = new LocationCache();
            if (File.Exists($"{cachePath}/accloc"))
                locCache = JsonConvert.DeserializeObject<LocationCache>(
                    File.ReadAllText($"{cachePath}/accloc"));
            if (locCache.IsSyncing) return;
            try
            {
                locCache.IsSyncing = true;
                File.WriteAllText($"{cachePath}/accloc", JsonConvert.SerializeObject(locCache));
                LocationService.CacheActionLocations(locCache.Locations);
                locCache.IsSyncing = false;
                locCache.LastSync = DateTime.Now;
                locCache.LastSyncMilliseconds = (startTime - locCache.LastSync).TotalMilliseconds;
                File.WriteAllText($"{cachePath}/accloc", JsonConvert.SerializeObject(locCache));
            }
            catch (Exception e)
            {
                locCache.IsSyncing = false;
                File.WriteAllText($"{cachePath}/accloc", JsonConvert.SerializeObject(locCache));
                Logger.Info($"Error Caching Locations {e.Message} at {e.StackTrace}");
            }


            //ViewEngines.Engines.Insert(0, new inFuseViewEngine());

            //var j = SchedulerManager.Jobs[0];
            //var newjob = ((InFuseExportJob)j).ConvertExportJobToModular();
            //newjob.Execute();

            //var job = (ExportJob)SchedulerManager.Jobs[0];
            //job.ConvertToModuleManager();
            //job.RunJob();
            //SchedulerManager.InitialiseJobs();
            //ExportJobs = new FluentRegistry();
            //JobManager.Initialize(ExportJobs);
            //ExportJobs.LoadExportJobs();
            //Application["FluentRegistry"] = ExportJobs;
        }

        protected void Application_End(object sender, EventArgs e)
        {
            EndAppHelper.EndApp();

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            HttpContext.Current.Response.AddHeader("Access-Control-Allow-Origin", "*");
            if (HttpContext.Current.Request.HttpMethod == "OPTIONS")
            {
                HttpContext.Current.Response.AddHeader("Access-Control-Allow-Methods", "POST, PUT, DELETE");
                HttpContext.Current.Response.AddHeader("Access-Control-Allow-Headers", "Content-Type, Accept");
                HttpContext.Current.Response.AddHeader("Access-Control-Max-Age", "1728000");
                HttpContext.Current.Response.End();
            }
        }
    }
}
