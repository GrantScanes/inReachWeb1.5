#region Using

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using inReachWebRebuild.Classes;
using inReachWebRebuild.Common;
using inReachWebRebuild.ViewModels;
using Infor.Model;
using Infor.Model.ApplicationServices;
using Infor.Model.Enums;
using Infor.Model.SchedulerServices;
using Infor.Model.SchedulerServices.JobTypes;
using Infor.Model.SchedulerServices.ScheduleTypes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using Telerik.Reporting;

#endregion

namespace inReachWebRebuild.Controllers
{
    public class ReportsController : BaseController
    {
        // GET: Reports
        private static string _reportserverUrl = SettingsManager.GetSettingValueAsString("rpurl");

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static string _tu = SettingsManager.GetSettingValueAsString("TU");
        private static string _tup = SettingsManager.GetSettingValueAsString("TUP");
        private static readonly ScheduleTypeFactory ScheduleTypeFactory = new ScheduleTypeFactory();

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
            _tu = SettingsManager.GetSettingValueAsString("TU");
            _tup = SettingsManager.GetSettingValueAsString("TUP");
            _reportserverUrl = SettingsManager.GetSettingValueAsString("rpurl");
            ViewData["UserState"] = AppUserState;
        }

        public PartialViewResult ReportViewer(string id)
        {
            var client = new HttpClient {BaseAddress = new Uri(_reportserverUrl)};
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var reportusername = AppUserState.ReportingUserName;
            var reportpassword = "Summer16";
            var data = $"grant_type=password&username={reportusername}&password={reportpassword}";
            var token = client.PostAsync($"{_reportserverUrl}/Token", new StringContent(data)).Result;
            dynamic t = JObject.Parse(token.Content.ReadAsStringAsync().Result);
            var rvm = new TemplatedReportViewerViewModel {ReportServer = _reportserverUrl, ReportSource = new UriReportSource {Uri = id.Replace("|", "/")}, Token = t.access_token.Value};
            return PartialView("_ReportViewer", rvm);
        }

        [AllowAnonymous]
        public ActionResult ReportViewerPage(string id)
        {
            Logger.Info($"report viewer page");
            if (AppUserState == null || AppUserState.Connected == false)
            {
                Logger.Info($"user not logged in redirect to login with return path of {System.Web.HttpContext.Current.Request.Url.PathAndQuery}");
                return RedirectToAction("WindowsLogOnRedirect", "Auth", new {returnPath = System.Web.HttpContext.Current.Request.Url.PathAndQuery});
            }
            Logger.Info($"user logged in");
            var client = new HttpClient {BaseAddress = new Uri(_reportserverUrl)};
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var reportusername = AppUserState.ReportingUserName;
            Logger.Info($"username {reportusername} ");
            var reportpassword = "Summer16";
            var data = $"grant_type=password&username={reportusername}&password={reportpassword}";
            Logger.Info($"data {data} ");
            var token = client.PostAsync($"{_reportserverUrl}/Token", new StringContent(data)).Result;
            Logger.Info($"_reportserverUrl {_reportserverUrl} ");
            dynamic t = JObject.Parse(token.Content.ReadAsStringAsync().Result);
            var rvm = new TemplatedReportViewerViewModel {ReportServer = _reportserverUrl, ReportSource = new UriReportSource {Uri = id.Replace("|", "/")}, Token = t.access_token.Value};
            return View(rvm);
        }

        public PartialViewResult Index()
        {
            var conn = new InforConnection(reportServerAddress: _reportserverUrl, tu: _tu, tup: _tup);
            if (AppUserState == null || AppUserState.Connected == false) RedirectToAction("LogOff", "Auth");
            var hvm = new ReportConfigViewModel {SavedSearches = conn.GetSavedSearchesWeb(AppUserState.UserName, AppUserState.Wgs, AppUserState.Ds)};
            return PartialView("Index", hvm);
        }

        public void ExecuteExportJob(string name)
        {
            //var eJobs = HttpContext.Application["FluentRegistry"] as FluentRegistry;
            // eJobs?.ExportJobs?.FirstOrDefault(n => n.Name == name)?.Execute();
            //((FluentRegistry)HttpContext.Application["FluentRegistry"]).ExportJobs.FirstOrDefault(n => n.Name == name)?.Execute(AppUserState.UserName);
            SchedulerManager.ExecuteJobByName(name, AppUserState.UserName);
            //SchedulerManager.Jobs.FirstOrDefault(n => n.Name == name)?.Execute(AppUserState.UserName);
        }

        public void DeleteExportJob(Guid id)
        {
            SchedulerManager.DeleteJobById(id);
            //var ind = ((FluentRegistry)HttpContext.Application["FluentRegistry"]).ExportJobs.FindIndex(n => n.Id == id);
            //if (ind >= 0)
            //{
            //    ((FluentRegistry)HttpContext.Application["FluentRegistry"]).ExportJobs[ind].DeleteJob();
            //    //((FluentRegistry)HttpContext.Application["FluentRegistry"]).ExportJobs[ind].Stop(true);
            //    //((FluentRegistry)HttpContext.Application["FluentRegistry"]).ExportJobs.RemoveAt(ind);
            //}
            //var fluentRegistry = (FluentRegistry)HttpContext.Application["FluentRegistry"];
            //fluentRegistry?.CommitExportJobs(id);
        }

        public void CloneExportJob(Guid id)
        {
            SchedulerManager.CloneJobById(id);

            ////ExportJob clone = null;
            //var ind = ((FluentRegistry)HttpContext.Application["FluentRegistry"]).ExportJobs.FindIndex(n => n.Id == id);
            //if (ind >= 0)
            //{
            //    ((FluentRegistry)HttpContext.Application["FluentRegistry"]).ExportJobs[ind].CloneJob();

            //    //clone = ((FluentRegistry)HttpContext.Application["FluentRegistry"]).ExportJobs.Copy(id);
            //    //((FluentRegistry)HttpContext.Application["FluentRegistry"]).ExportJobs.Add(clone);
            //}
            ////var fluentRegistry = (FluentRegistry)HttpContext.Application["FluentRegistry"];
            ////fluentRegistry?.CommitExportJobs(clone.Id);
        }

        

        public void CancelExecution(Guid id)
        {
            SchedulerManager.StopExecutionById(id, true, AppUserState.Name);

            //var ind = ((FluentRegistry)HttpContext.Application["FluentRegistry"]).ExportJobs.FindIndex(n => n.Id == id);
            //if (ind >= 0)
            //{
            //    ((FluentRegistry)HttpContext.Application["FluentRegistry"]).ExportJobs[ind].Stop(true, AppUserState.Name);

            //}
            ////var fluentRegistry = (FluentRegistry)HttpContext.Application["FluentRegistry"];
            ////fluentRegistry?.CommitExportJobs(id);
        }

        public void CommitExportJob(ExportJobLite job, string scheduleValues, string scheduleFequency, bool repeating, bool disabled)
        {
            try
            {
                var j = SchedulerManager.GetJobById(job.Id);
                if (j == null)
                {
                    var ej = job.ToInFuseExportJob(null);
                    SchedulerManager.AddJob(JobTypeEnum.InFuseExportJob, JsonConvert.SerializeObject(ej), scheduleValues, scheduleFequency, repeating, disabled);
                    //SchedulerManager.AddJob(JobTypeEnum.ExportJob, JsonConvert.SerializeObject(job.ToExportJob(null)), scheduleValues, scheduleFequency, repeating, disabled);
                }
                else
                {
                    var ej = job.ToInFuseExportJob(job.Id);
                    SchedulerManager.AddJob(JobTypeEnum.InFuseExportJob, JsonConvert.SerializeObject(ej), scheduleValues, scheduleFequency, repeating, disabled);
                }
            }
            catch (Exception e)
            {
                Logger.Info($"Errors saving Export Job {e.Message} - {e.StackTrace}");
                if (e.InnerException != null)
                {
                    Logger.Info($"Errors saving Export Job INNER exception {e.InnerException.Message} - {e.InnerException.StackTrace}");
                }

                throw;
            }
           
        }

        public string PreviewExportJob(string savedSearch, InforProps props, string delimeter, string qualifier, bool includeHeaders)
        {
            var conn = new InforConnection(reportServerAddress: _reportserverUrl, tu: _tu, tup: _tup);
            if (AppUserState == null || AppUserState.Connected == false) return "";
            var returnstring = conn.PreviewExport(AppUserState.UserName, AppUserState.Wgs, AppUserState.Ds, savedSearch, props, delimeter, qualifier, includeHeaders);
            return returnstring;
        }

        public JsonResult ExportJob(Guid? id)
        {
            var ej = (InFuseExportJob) SchedulerManager.GetJobById(id) ?? new InFuseExportJob(); //((FluentRegistry) HttpContext.Application["FluentRegistry"]).ExportJobs?.FirstOrDefault(n => n.Id == id) ?? new ExportJob();
            return this.Jsonp(ej.ToExportJobLite());
        }

        public PartialViewResult ExportJobs()
        {
            //var refr = HttpContext.Application["FluentRegistry"] as FluentRegistry;
            //refr.LoadExportJobs();
            if (AppUserState == null || AppUserState.Connected == false) return PartialView("ExportJobs", new ExportJobViewModel());
            var hvm = new ExportJobViewModel {EJobs = SchedulerManager.GetJobsByType(JobTypeEnum.InFuseExportJob) };
            return PartialView("ExportJobs", hvm);
        }

        public PartialViewResult GetSchedulePartialView(string name)
        {
            if (AppUserState == null || AppUserState.Connected == false) return PartialView("ExportJobs", new ExportJobViewModel());

            //var jobs = ((FluentRegistry)HttpContext.Application["FluentRegistry"]).ExportJobs;
            //var returnjob = jobs.FirstOrDefault(n => n.Name == name);
            var hvm = new ScheduleViewModel {Schedule = SchedulerManager.GetJobByName(name).ScheduleType};
            return PartialView("SchedulePartialView", hvm);
        }

        public PartialViewResult InforPropsForPartial(InforProp parent)
        {
            var conn = new InforConnection(reportServerAddress: _reportserverUrl, tu: _tu, tup: _tup);
            if (AppUserState == null || AppUserState.Connected == false) return PartialView("Properties", new InforPropsViewModel());
            var hvm = new InforPropsViewModel {Properties = parent == null ? conn.GetPropertiesWeb(AppUserState.UserName, AppUserState.Wgs, AppUserState.Ds, InforObjectype.Record, null) : conn.GetPropertiesWeb(AppUserState.UserName, AppUserState.Wgs, AppUserState.Ds, parent.Type, parent)};
            return PartialView("Properties", hvm);
        }

        public JsonResult GetInforProps(InforProp parent)
        {
            var conn = new InforConnection(reportServerAddress: _reportserverUrl, tu: _tu, tup: _tup);
            if (AppUserState == null || AppUserState.Connected == false) return null;
            var props = parent == null ? conn.GetPropertiesWeb(AppUserState.UserName, AppUserState.Wgs, AppUserState.Ds, InforObjectype.Record, null) : conn.GetPropertiesWeb(AppUserState.UserName, AppUserState.Wgs, AppUserState.Ds, parent.Type, parent);
            return this.Jsonp(props);
        }

        public JsonResult GetInforPropsTreeView(string parentString)
        {
            
            var conn = new InforConnection(reportServerAddress: _reportserverUrl, tu: _tu, tup: _tup);
            if (AppUserState == null || AppUserState.Connected == false) return null;
            InforProps props;
            if (string.IsNullOrEmpty(parentString) )
            {
                props = conn.GetPropertiesWeb(AppUserState.UserName, AppUserState.Wgs, AppUserState.Ds, InforObjectype.Record, null);
                return Json(props, JsonRequestBehavior.AllowGet);
            }

            var parent = JsonConvert.DeserializeObject<InforProp>(parentString);
            props = parent == null ? conn.GetPropertiesWeb(AppUserState.UserName, AppUserState.Wgs, AppUserState.Ds, InforObjectype.Record, null) : conn.GetPropertiesWeb(AppUserState.UserName, AppUserState.Wgs, AppUserState.Ds, parent.Type, parent);
            return Json(props, JsonRequestBehavior.AllowGet);
        }

        public JsonResult FilterInforProps(string filter)
        {
            var conn = new InforConnection(reportServerAddress: _reportserverUrl, tu: _tu, tup: _tup);
            var retprops = new InforProps();
            if (AppUserState == null || AppUserState.Connected == false) return null;
            var props = conn.GetPropertiesWeb(AppUserState.UserName, AppUserState.Wgs, AppUserState.Ds, InforObjectype.Record, null);
            retprops.AddRange(props.Where(p => p.Caption.Contains(filter)).ToList());
            return this.Jsonp(retprops);
        }

        public PartialViewResult GetHistoryPartialViewResult(Guid id)
        {
            //var jobs = ((FluentRegistry)HttpContext.Application["FluentRegistry"]).ExportJobs;
            var returnjob = SchedulerManager.GetJobById(id); // jobs.FirstOrDefault(n => n.Id == id) ?? new ExportJob();
            var hvm = new HistoryViewModel {History = new List<FluentJobExecutionHistory>(returnjob.History.OrderBy(d => d.StartDateTime))};
            return PartialView("_History", hvm);
        }

        public FileResult DownloadView(string path)
        {
            //var jobs = ((FluentRegistry)HttpContext.Application["FluentRegistry"]).ExportJobs;
            //var job = jobs.FirstOrDefault(n => n.Id == new Guid(parentId)) ?? new ExportJob();
            //var step = job.History.FirstOrDefault(g => g.Id == new Guid(id));
            return path != null && System.IO.File.Exists(path) ? File(path, MimeMapping.GetMimeMapping(path), Path.GetFileName(path)) : null;
        }

        public PartialViewResult GetScheduleConfig(string uom, Guid? id)
        {
            //var jobs = ((FluentRegistry)HttpContext.Application["FluentRegistry"]).ExportJobs;
            var returnjob = SchedulerManager.GetJobById(id) ?? new InFuseExportJob {ScheduleType = new HourScheduleType {Type = TimeUoM.Hours, IntervalUnit = 1, RunMinute = 0, Disabled = true}}; //jobs.FirstOrDefault(n => n.Id == id) ?? new ExportJob();
            var model = new SecondsViewModel
            {
                Schedule = returnjob?.ScheduleType,
                JobName = returnjob?.Name,
                Uom = (TimeUoM) Enum.Parse(typeof(TimeUoM), string.IsNullOrEmpty(uom) ? returnjob?.ScheduleType.Type.ToString() : uom, true)
            };
            if (SchedulerManager.GetJobById(id) == null)
            {
                var scheduleType = ScheduleTypeFactory.CreateScheduleType(model.Uom, 1, new TimeSpan(0, 0, 0), new TimeSpan(24, 0, 0), new TimeSpan(0, 0, 0), 0);
                //model.Schedule = returnjob?.ScheduleType;
                foreach (var item in scheduleType.GetProperties())
                {
                    model.Props.Add(item);
                }
            }
            else
            {
                foreach (var item in returnjob?.ScheduleType.GetProperties())
                {
                    model.Props.Add(item);
                }
            }
            if (AppUserState == null || AppUserState.Connected == false) return PartialView("_ScheduleConfig", model);
            return PartialView("_ScheduleConfig", model);
        }

        public JsonResult CommitScheduleConfig(SecondsViewModel model)
        {
            return null;
        }
    }
}