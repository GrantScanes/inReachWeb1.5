#region Using

using System;
using System.ComponentModel;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using inReachWebRebuild.Classes;
using inReachWebRebuild.Common;
using inReachWebRebuild.ViewModels;
using Infor.Model;
using Infor.Model.ApplicationServices;
using Infor.Model.Helpers;
using Infor.Model.WebServices.RecordServices;
using NLog;
using InforSearches = Infor.Model.Enums.InforSearches;

#endregion

namespace inReachWebRebuild.Controllers
{
    [AllowAnonymous]
    public class RecordsController : BaseController
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static string _tu = SettingsManager.GetSettingValueAsString("TU");
        private static string _tup = SettingsManager.GetSettingValueAsString("TUP");

        [AllowCrossSiteJson]
        public JsonResult All()
        {
            //    Logger.Info($"all records");
            var conn = new InforConnection(tu: _tu, tup: _tup);

            //    if (AppUserState == null || AppUserState.Connected == false) return null;
            //    Logger.Info($"get records for user  {AppUserState.UserName}");
            var s = new InforSearch
            {
                SearchType = InforSearches.Favourites,
                Name = InforSearches.Favourites.GetAttribute<DescriptionAttribute>().Description,
                Function = InforSearches.Favourites.GetAttribute<SearchStringAttribute>().Term,
                App = InforSearches.Favourites.GetAttribute<ApplicationAttribute>().Application,
                IsAdminable = false,
                //Icon = new BitmapImage(new Uri(@"/Images/SearchIcons/favourite.jpg", UriKind.RelativeOrAbsolute)),
                RunAsAdmin = false
            };
            //Logger.Info($"going to do search");
            try
            {
                var recs = conn.SearchWeb(s, AppUserState.UserName, AppUserState.Wgs, AppUserState.Ds);
                Logger.Info($"search resturned {recs.Count} records");
                return this.Jsonp(recs);
            }
            catch (Exception ex)
            {
                Logger.Info($"search resulted in an error {ex.Message} with stack {ex.StackTrace}");
            }
            return null;
        }

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
            _tu = SettingsManager.GetSettingValueAsString("TU");
            _tup = SettingsManager.GetSettingValueAsString("TUP");
            ViewData["UserState"] = AppUserState;
        }

        public JsonResult Index(int? id, string search)
        {
            InforSearch se = null;
            Logger.Info($"Search for user {AppUserState.UserName}");
            if (AppUserState == null || AppUserState.Connected == false) return null;
            if (!string.IsNullOrEmpty(search))
                se = JsonHelpers.Deserialize<InforSearch>(search);
            var conn = new InforConnection(tu: _tu, tup: _tup);
            var s = new InforSearch
            {
                SearchType = InforSearches.Favourites,
                Name = InforSearches.Favourites.GetAttribute<DescriptionAttribute>().Description,
                Function = InforSearches.Favourites.GetAttribute<SearchStringAttribute>().Term,
                App = InforSearches.Favourites.GetAttribute<ApplicationAttribute>().Application,
                IsAdminable = false,
                //Icon = new BitmapImage(new Uri(@"/Images/SearchIcons/favourite.jpg", UriKind.RelativeOrAbsolute)),
                RunAsAdmin = false
            };
            if (se != null)
                s = se;
            return
                this.Jsonp(id == null
                    ? conn.SearchWeb(s, AppUserState.UserName, AppUserState.Wgs, AppUserState.Ds)
                    : conn.GetRecordChildrenWeb((int) id, AppUserState.UserName, AppUserState.Wgs, AppUserState.Ds));

            //return this.Jsonp(recs);
        }

        public JsonResult Update()
        {
            //var employees = this.DeserializeObject<IEnumerable<EmployeeDirectoryModel>>("models");

            //if (employees != null)
            //{
            //    foreach (var employee in employees)
            //    {
            //        EmployeeDirectoryRepository.Update(employee);
            //    }
            //}

            //return this.Jsonp(employees);
            return null;
        }

        public JsonResult Destroy()
        {
            //var employees = this.DeserializeObject<IEnumerable<EmployeeDirectoryModel>>("models");

            //if (employees != null)
            //{
            //    foreach (var employee in employees)
            //    {
            //        EmployeeDirectoryRepository.Delete(employee);
            //    }
            //}

            //return this.Jsonp(employees);
            return null;
        }

        public JsonResult Create()
        {
            //var employees = this.DeserializeObject<IEnumerable<EmployeeDirectoryModel>>("models");

            //if (employees != null)
            //{
            //    foreach (var employee in employees)
            //    {
            //        EmployeeDirectoryRepository.Insert(employee);
            //    }
            //}

            //return this.Jsonp(employees);
            return null;
        }

        [HttpPost]
        public JsonResult UpdateNotes(long uri, string notes)
        {
            if (AppUserState == null || AppUserState.Connected == false) return null;
            var conn = new InforConnection(tu: _tu, tup: _tup);
            return
                this.Jsonp(conn.SaveRecordNotesWeb(uri, notes, AppUserState.UserName, AppUserState.Wgs, AppUserState.Ds));
        }

        [HttpPost]
        public JsonResult CompleteAction(long recordUri, long actionUri, string notes)
        {
            if (AppUserState == null || AppUserState.Connected == false) return null;
            var conn = new InforConnection(tu: _tu, tup: _tup);
            return
                this.Jsonp(conn.CompleteRecordActionsWeb(recordUri, actionUri, notes, AppUserState.UserName,
                    AppUserState.Wgs, AppUserState.Ds));
        }

        public PartialViewResult QuickSearch(string searchTerm)
        {
            var hvm = new SearchResultsViewModel();
            if (AppUserState == null || AppUserState.Connected == false) return null;
            InforActionResult recs;
            InforRecordsLite lites;
            if (searchTerm.Substring(0, 2).ToLower() == "#:")
            {
                recs = SearchService.SearchRecordNumber(AppUserState.Wgs, AppUserState.Ds, AppUserState.UserName, searchTerm.Substring(2), -1);
                lites = new InforRecordsLite();
                lites.AddRange(((InforRecords) recs.ReturnObject).Select(rec => rec.ToLite()));
                hvm.Results = lites;
                return PartialView("/Views/HomeM/_SearchResults.cshtml", hvm);
            }
            if (searchTerm.Length > 6 &&  searchTerm.Substring(0, 6).ToLower() == "title:")
            {
                recs = SearchService.SearchTitle(AppUserState.Wgs, AppUserState.Ds, AppUserState.UserName, searchTerm.Substring(6), -1);
                lites = new InforRecordsLite();
                lites.AddRange(((InforRecords) recs.ReturnObject).Select(rec => rec.ToLite()));
                hvm.Results = lites;
                return PartialView("/Views/HomeM/_SearchResults.cshtml", hvm);
            }
            if (searchTerm.Length > 6 &&  searchTerm.Substring(0, 6).ToLower() == "notes:")
            {
                recs = SearchService.SearchNotes(AppUserState.Wgs, AppUserState.Ds, AppUserState.UserName, searchTerm.Substring(6), -1);
                lites = new InforRecordsLite();
                lites.AddRange(((InforRecords) recs.ReturnObject).Select(rec => rec.ToLite()));
                hvm.Results = lites;
                return PartialView("/Views/HomeM/_SearchResults.cshtml", hvm);
            }
            recs = SearchService.SearchTitle(AppUserState.Wgs, AppUserState.Ds, AppUserState.UserName, searchTerm, -1);
            lites = new InforRecordsLite();
            if (recs.ReturnObject != null)
            {
                lites.AddRange(((InforRecords) recs.ReturnObject).Select(rec => rec.ToLite()));
            }
            hvm.Results = lites;
            return PartialView("/Views/HomeM/_SearchResults.cshtml", hvm);
        }
    }
}