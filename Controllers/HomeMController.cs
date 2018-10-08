#region Using

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using inReachWebRebuild.Classes;
using inReachWebRebuild.Common;
using inReachWebRebuild.Models;
using inReachWebRebuild.ViewModels;
using Infor.Model;
using Infor.Model.ApplicationServices;
using Infor.Model.Enums;
using Infor.Model.WebServices.ActionServices;
using Infor.Model.WebServices.LocationServices;
using Infor.Model.WebServices.ProcessServices;
using Newtonsoft.Json;
using NLog;
using InforSearches = Infor.Model.InforSearches;

#endregion

namespace inReachWebRebuild.Controllers
{
    public class HomeMController : BaseController
    {
        public static readonly string LocalStorageContainer =
            Path.Combine(AppDomain.CurrentDomain.GetData("DataDirectory").ToString(), $"UserConfig");

        private static string _tu = SettingsManager.GetSettingValueAsString("TU");
        private static string _tup = SettingsManager.GetSettingValueAsString("TUP");
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly CompleteProcessViewModel _completeProcessViewModel = new CompleteProcessViewModel();
        private readonly LocationPickerViewModel _locationPickerViewModel = new LocationPickerViewModel();
        private readonly ProcessActionsViewModel _processActionsViewModel = new ProcessActionsViewModel();
        private readonly ProcessViewModel _processViewModel = new ProcessViewModel();
        private readonly ReasignLocationViewModel _reasignLocationViewModelViewModel = new ReasignLocationViewModel();
        //private readonly UsersService _userService = new UsersService();
        //private readonly UsersService _usersService = new UsersService();

        private readonly HomeMViewModel _viewModel = new HomeMViewModel();
        // GET: HomeM

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
            _tu = SettingsManager.GetSettingValueAsString("TU");
            _tup = SettingsManager.GetSettingValueAsString("TUP");
            _viewModel.ErrorDisplay = ErrorDisplay;
            _viewModel.AppUserState = AppUserState;
            _processViewModel.ErrorDisplay = ErrorDisplay;
            _processViewModel.AppUserState = AppUserState;
            _reasignLocationViewModelViewModel.ErrorDisplay = ErrorDisplay;
            _reasignLocationViewModelViewModel.AppUserState = AppUserState;
            _completeProcessViewModel.ErrorDisplay = ErrorDisplay;
            _completeProcessViewModel.AppUserState = AppUserState;
            _processActionsViewModel.ErrorDisplay = ErrorDisplay;
            _processActionsViewModel.AppUserState = AppUserState;
            _locationPickerViewModel.ErrorDisplay = ErrorDisplay;
            _locationPickerViewModel.AppUserState = AppUserState;
            ViewData["UserState"] = AppUserState;
        }

        public async Task<ActionResult> Index()
        {
            var conn = new InforConnection(reportServerAddress: $"{SettingsManager.GetSettingValueAsString("rpurl")}",
                tu: _tu, tup: _tup);
            if (AppUserState == null || AppUserState.Connected == false) return RedirectToAction("LogOff", "Auth");
            var result = await conn.GetSearchesWeb(AppUserState.UserName, AppUserState.Wgs, AppUserState.Ds,
                AppUserState.ReportingUserName);
            if (result.Faults.Any())
            {
                ErrorDisplay.ShowError(result.Faults[0].Message);
                return PartialView(_viewModel);
            }

            var allsearches = (InforSearches) result.ReturnObject;
            var distinctApps = allsearches.GroupBy(search => search.App).Select(group => group.First());
            _viewModel.User = AppUserState;
            _viewModel.AppTiles = new AppTiles();
            _viewModel.UserTiles = new AppTiles();
            _viewModel.ShowSearch = false;
            var displayOrder = 99;
            foreach (var app in distinctApps)
            {
                var imgurl = "";
                switch (app.App)
                {
                    case "Search":
                        imgurl = $"/Content/Images/Apps/inReachIcon2.ico";
                        displayOrder = 0;
                        _viewModel.ShowSearch = true;
                        break;
                    case "inScribe":
                        imgurl = $"/Content/Images/Apps/inScribe.ico";
                        break;
                    case "Process":
                        imgurl = $"/Content/Images/Apps/eTick.jpg";
                        displayOrder = 1;
                        break;
                    case "Reporting":
                        imgurl = $"/Content/Images/Apps/eTick.jpg";
                        displayOrder = 2;
                        break;
                    case "Administration":
                        imgurl = $"/Content/Images/Apps/eTick.jpg";
                        displayOrder = 3;
                        break;
                    default:
                        imgurl = $"/Content/Images/Apps/inReachIcon2.jpg";
                        break;
                }

                var apptile = new AppTile
                {
                    Title = app.App,
                    IconUrl = imgurl,
                    AppTiles = new Tiles(),
                    DisplayOrder = displayOrder
                };
                foreach (var search in allsearches.Where(search => search.App == app.App))
                    apptile.AppTiles.Add(new Tile
                    {
                        Title = search.Name,
                        IconUrl = search.GetImageUrl(),
                        Search = search,
                        AppTitle = search.App,
                        DisplayOrder = search.DisplayOrder
                    });
                apptile.UserTiles = new Tiles();
                //{
                //    new Tile {Title = $"User Tile Test 1 for app {app.App}", IconUrl = $"/Content/Images/LoggedIn.png"}
                //};
                _viewModel.AppTiles.Add(apptile);
            }

            var usertile = new AppTile {Title = "UserTiles", IconUrl = "", AppTiles = new Tiles()};
            //var usertilestCookie = Request.Cookies["usertiles"];
            //if (usertilestCookie != null)
            //{
            //    var usertilesString = usertilestCookie.Value;
            //    var usertilesList = usertilesString.Split('|');
            //    foreach (var s in usertilesList)
            //    {
            //        var searchTile = allsearches.FirstOrDefault(search => search.Name == s);
            //        if (searchTile != null) usertile.AppTiles.Add(new Tile { Title = searchTile.Name, IconUrl = searchTile.Imageurl, Search = searchTile, AppTitle = searchTile.App});
            //    }
            //}
            _viewModel.UserTiles.Add(usertile);
            if (!System.IO.File.Exists(Path.Combine(LocalStorageContainer,
                $"{AppUserState.Wgs}{AppUserState.Ds}{AppUserState.UserId}"))) return View(_viewModel);
            var serhvm = JsonConvert.DeserializeObject<HomePageViewModel>(System.IO.File.ReadAllText(
                Path.Combine(LocalStorageContainer, $"{AppUserState.Wgs}{AppUserState.Ds}{AppUserState.UserId}")));
            _viewModel.AutoCloseCharms = serhvm.AutoCloseCharms;
            _viewModel.AutoScrollCaro = serhvm.AutoScrollCaro;
            _viewModel.VerticalTrees = serhvm.VerticalTrees;
            _viewModel.SavedSearches =
                conn.GetSavedSearchesWeb(AppUserState.UserName, AppUserState.Wgs, AppUserState.Ds);
            _viewModel.Theme = serhvm.Theme;


            return View(_viewModel);
        }

        public void CacheLocations()
        {

            var cachePath = System.Web.HttpContext.Current.Server.MapPath($"~/App_Data/inReachRepos/Global/Cache");
            Directory.CreateDirectory(cachePath);
            var startTime = DateTime.Now;
            var locCache = new LocationCache();
            if (System.IO.File.Exists($"{cachePath}/accloc"))
                locCache = JsonConvert.DeserializeObject<LocationCache>(
                    System.IO.File.ReadAllText($"{cachePath}/accloc"));
            if (locCache.IsSyncing) return;
            try
            {
                locCache.IsSyncing = true;
                System.IO.File.WriteAllText($"{cachePath}/accloc", JsonConvert.SerializeObject(locCache));
                LocationService.CacheActionLocations(locCache.Locations);
                locCache.IsSyncing = false;
                locCache.LastSync = DateTime.Now;
                locCache.LastSyncMilliseconds = (startTime - locCache.LastSync).TotalMilliseconds;
                System.IO.File.WriteAllText($"{cachePath}/accloc", JsonConvert.SerializeObject(locCache));
            }
            catch (Exception e)
            {
                locCache.IsSyncing = false;
                System.IO.File.WriteAllText($"{cachePath}/accloc", JsonConvert.SerializeObject(locCache));
                Logger.Info($"Error Caching Locations {e.Message} at {e.StackTrace}");
            }
        }

        public async Task<PartialViewResult> PinnedTiles()
        {
            var hvm = new HomePageViewModel
            {
                ErrorDisplay = ErrorDisplay,
                AppUserState = AppUserState
            };
            var conn = new InforConnection(reportServerAddress: $"{SettingsManager.GetSettingValueAsString("rpurl")}",
                tu: _tu, tup: _tup);
            if (AppUserState == null || AppUserState.Connected == false) return PartialView("_PinnedTiles", hvm);
            hvm.PinnedTiles = new AppTile();
            if (System.IO.File.Exists(Path.Combine(LocalStorageContainer,
                $"{AppUserState.Wgs}{AppUserState.Ds}{AppUserState.UserId}")))
                if (System.IO.File.Exists(Path.Combine(LocalStorageContainer,
                    $"{AppUserState.Wgs}{AppUserState.Ds}{AppUserState.UserId}")))
                    hvm = JsonConvert.DeserializeObject<HomePageViewModel>(System.IO.File.ReadAllText(
                        Path.Combine(LocalStorageContainer,
                            $"{AppUserState.Wgs}{AppUserState.Ds}{AppUserState.UserId}")));
            var result = await conn.GetSearchesWeb(AppUserState.UserName, AppUserState.Wgs, AppUserState.Ds,
                AppUserState.ReportingUserName);
            if (result.Faults.Any())
            {
                hvm.AppUserState.Connected = false;
                ErrorDisplay.ShowError(result.Faults[0].Message);
                return PartialView("_PinnedTiles", hvm);
            }

            var allsearches = (InforSearches) result.ReturnObject;
            var distinctApps = allsearches.GroupBy(search => search.App).Select(group => group.First());
            hvm.PinnedTiles.UserTiles.RemoveAll(n => distinctApps.All(ap => ap.App != n.Search.App));
            System.IO.File.WriteAllText(
                Path.Combine(LocalStorageContainer, $"{AppUserState.Wgs}{AppUserState.Ds}{AppUserState.UserId}"),
                JsonConvert.SerializeObject(hvm));
            foreach (var tile in hvm.PinnedTiles.UserTiles)
                //if (tile.Search.IncludeCount)
                //{
                tile.Count = conn.CountWeb(tile.Search, AppUserState.UserName, AppUserState.Wgs, AppUserState.Ds);
            // }
            return PartialView("_PinnedTiles", hvm);
        }

        public async Task<PartialViewResult> PinnedQuickTiles()
        {
            var hvm = new HomePageViewModel
            {
                ErrorDisplay = ErrorDisplay,
                AppUserState = AppUserState
            };
            var conn = new InforConnection(reportServerAddress: $"{SettingsManager.GetSettingValueAsString("rpurl")}",
                tu: _tu, tup: _tup);
            if (AppUserState == null || AppUserState.Connected == false) return PartialView("_QuickTiles", hvm);
            hvm.PinnedTiles = new AppTile();
            if (Classes.Helpers.CookieExists("HomePageViewModel", Response))
            {
                hvm = JsonConvert.DeserializeObject<HomePageViewModel>(
                    Classes.Helpers.GetCookieValue("HomePageViewModel", Response));
                System.IO.File.WriteAllText(
                    Path.Combine(LocalStorageContainer, $"{AppUserState.Wgs}{AppUserState.Ds}{AppUserState.UserId}"),
                    JsonConvert.SerializeObject(hvm.PinnedTiles));
                Classes.Helpers.RemoveCookie("HomePageViewModel", Response);
            }
            else
            {
                if (System.IO.File.Exists(Path.Combine(LocalStorageContainer,
                    $"{AppUserState.Wgs}{AppUserState.Ds}{AppUserState.UserId}")))
                    hvm = JsonConvert.DeserializeObject<HomePageViewModel>(System.IO.File.ReadAllText(
                        Path.Combine(LocalStorageContainer,
                            $"{AppUserState.Wgs}{AppUserState.Ds}{AppUserState.UserId}")));
            }

            var result = await conn.GetSearchesWeb(AppUserState.UserName, AppUserState.Wgs, AppUserState.Ds,
                AppUserState.ReportingUserName);
            if (result.Faults.Any())
            {
                ErrorDisplay.ShowError(result.Faults[0].Message);
                return PartialView("_QuickTiles", hvm);
            }

            var allsearches = (InforSearches) result.ReturnObject;
            var distinctApps = allsearches.GroupBy(search => search.App).Select(group => group.First());
            hvm.PinnedTiles.UserTiles.RemoveAll(n => distinctApps.All(ap => ap.App != n.Search.App));
            //var removeindexes = (from pin in hvm.PinnedTiles.UserTiles where distinctApps.All(a => a.App != pin.Search.App) select hvm.PinnedTiles.UserTiles.IndexOf(pin)).ToList();
            //foreach (var i in removeindexes)
            //{
            //    hvm.PinnedTiles.UserTiles.RemoveAt(i);
            //}
            return PartialView("_QuickTiles", hvm);
        }

        public PartialViewResult RecordClicked(int id, bool verticalTrees)
        {
            var hvm = new SearchResultsViewModel();
            if (AppUserState == null || AppUserState.Connected == false) return null;
            var conn = new InforConnection(tu: _tu, tup: _tup);
            var recs = conn.GetRecordChildrenWeb(id, AppUserState.UserName, AppUserState.Wgs, AppUserState.Ds);
            var lites = new InforRecordsLite();
            lites.AddRange(recs.Select(rec => rec as InforRecordLite));
            hvm.Results = lites;
            hvm.VerticalTree = verticalTrees;
            return PartialView("_SearchResults", hvm);
        }

        public async Task<PartialViewResult> EmailFolderClicked(string folderid)
        {
            var hvm = new EmailFolderContentResultsViewModel();
            //var client = await SDKHelper.GetAuthenticatedClient();
            //var conn = new InforConnection();
            //hvm.Results = await conn.GetEmailFolderMessages(folderid, client);
            return PartialView("_EmailFolderContents", hvm);
        }

        public PartialViewResult TileClicked(string incommingsearch, bool verticalTrees)
        {
            var search = JsonConvert.DeserializeObject<InforSearch>(incommingsearch);
            if (AppUserState == null || AppUserState.Connected == false) return null;
            var conn = new InforConnection(tu: _tu, tup: _tup);
            var recs = conn.SearchWeb(search, AppUserState.UserName, AppUserState.Wgs, AppUserState.Ds);
            var hvm = new SearchResultsViewModel();
            switch (search.App)
            {
                case "Search":
                    var lites = new InforRecordsLite();
                    lites.AddRange(recs.Select(rec => rec as InforRecordLite));
                    hvm.Results = lites;
                    hvm.VerticalTree = verticalTrees;
                    return PartialView("_SearchResults", hvm);
                case "Process":
                    var hvmN = new SearchResultsNodesViewModel();
                    var nodes = new InforNodesLite();
                    nodes.AddRange(recs.Select(rec => rec as InforNodeLite));
                    hvmN.Results = nodes;
                    hvmN.VerticalTree = verticalTrees;
                    if (nodes.Any())
                        hvmN.ProcessRootUri = nodes[0].ProcessUri;
                    return PartialView("~/Views/Process/_SearchResultsNodes.cshtml", hvmN);
            }

            return PartialView("_SearchResults", hvm);
        }

        public void PinUserTile(string incommingTile)
        {
            var tile = JsonConvert.DeserializeObject<Tile>(incommingTile);
            var savedtiles = new HomePageViewModel();
            if (System.IO.File.Exists(Path.Combine(LocalStorageContainer,
                $"{AppUserState.Wgs}{AppUserState.Ds}{AppUserState.UserId}")))
                savedtiles = JsonConvert.DeserializeObject<HomePageViewModel>(
                    System.IO.File.ReadAllText(Path.Combine(LocalStorageContainer,
                        $"{AppUserState.Wgs}{AppUserState.Ds}{AppUserState.UserId}")));
            savedtiles.PinnedTiles.UserTiles.Add(tile);
            System.IO.File.WriteAllText(
                Path.Combine(LocalStorageContainer, $"{AppUserState.Wgs}{AppUserState.Ds}{AppUserState.UserId}"),
                JsonConvert.SerializeObject(savedtiles));
        }

        public void RemovePinUserTile(string incommingTile)
        {
            var tile = JsonConvert.DeserializeObject<Tile>(incommingTile);
            var savedtiles = new HomePageViewModel();
            if (System.IO.File.Exists(Path.Combine(LocalStorageContainer,
                $"{AppUserState.Wgs}{AppUserState.Ds}{AppUserState.UserId}")))
                savedtiles = JsonConvert.DeserializeObject<HomePageViewModel>(
                    System.IO.File.ReadAllText(Path.Combine(LocalStorageContainer,
                        $"{AppUserState.Wgs}{AppUserState.Ds}{AppUserState.UserId}")));
            var tileIndex =
                savedtiles.PinnedTiles.UserTiles.FindIndex(n => n.Title == tile.Title && n.AppTitle == tile.AppTitle);
            savedtiles.PinnedTiles.UserTiles.RemoveAt(tileIndex);
            System.IO.File.WriteAllText(
                Path.Combine(LocalStorageContainer, $"{AppUserState.Wgs}{AppUserState.Ds}{AppUserState.UserId}"),
                JsonConvert.SerializeObject(savedtiles));
        }

        public void SaveTile(string incommingTile)
        {
            var tile = JsonConvert.DeserializeObject<Tile>(incommingTile);
            var savedtiles = new HomePageViewModel();
            if (System.IO.File.Exists(Path.Combine(LocalStorageContainer,
                $"{AppUserState.Wgs}{AppUserState.Ds}{AppUserState.UserId}")))
                savedtiles = JsonConvert.DeserializeObject<HomePageViewModel>(
                    System.IO.File.ReadAllText(Path.Combine(LocalStorageContainer,
                        $"{AppUserState.Wgs}{AppUserState.Ds}{AppUserState.UserId}")));
            var tileIndex =
                savedtiles.PinnedTiles.UserTiles.FindIndex(n => n.Title == tile.Title && n.AppTitle == tile.AppTitle);
            savedtiles.PinnedTiles.UserTiles[tileIndex] = tile;
            System.IO.File.WriteAllText(
                Path.Combine(LocalStorageContainer, $"{AppUserState.Wgs}{AppUserState.Ds}{AppUserState.UserId}"),
                JsonConvert.SerializeObject(savedtiles));
        }

        public void SaveTiles(string incommingTiles)
        {
            var tiles = JsonConvert.DeserializeObject<Tiles>(incommingTiles);
            var savedtiles = new HomePageViewModel();
            if (System.IO.File.Exists(Path.Combine(LocalStorageContainer,
                $"{AppUserState.Wgs}{AppUserState.Ds}{AppUserState.UserId}")))
                savedtiles = JsonConvert.DeserializeObject<HomePageViewModel>(
                    System.IO.File.ReadAllText(Path.Combine(LocalStorageContainer,
                        $"{AppUserState.Wgs}{AppUserState.Ds}{AppUserState.UserId}")));
            foreach (var tile in tiles)
            {
                var tileIndex =
                    savedtiles.PinnedTiles.UserTiles.FindIndex(
                        n => n.Title == tile.Title && n.AppTitle == tile.AppTitle);
                savedtiles.PinnedTiles.UserTiles[tileIndex] = tile;
            }

            System.IO.File.WriteAllText(
                Path.Combine(LocalStorageContainer, $"{AppUserState.Wgs}{AppUserState.Ds}{AppUserState.UserId}"),
                JsonConvert.SerializeObject(savedtiles));
        }

        public void SaveUserTheme(string theme)
        {
            var hvm = new HomePageViewModel();
            if (System.IO.File.Exists(Path.Combine(LocalStorageContainer,
                $"{AppUserState.Wgs}{AppUserState.Ds}{AppUserState.UserId}")))
                hvm = JsonConvert.DeserializeObject<HomePageViewModel>(System.IO.File.ReadAllText(
                    Path.Combine(LocalStorageContainer, $"{AppUserState.Wgs}{AppUserState.Ds}{AppUserState.UserId}")));
            hvm.Theme = theme;
            System.IO.File.WriteAllText(
                Path.Combine(LocalStorageContainer, $"{AppUserState.Wgs}{AppUserState.Ds}{AppUserState.UserId}"),
                JsonConvert.SerializeObject(hvm));
        }

        public void SaveUserToggle(bool autoScroll)
        {
            var hvm = new HomePageViewModel();
            if (System.IO.File.Exists(Path.Combine(LocalStorageContainer,
                $"{AppUserState.Wgs}{AppUserState.Ds}{AppUserState.UserId}")))
                hvm = JsonConvert.DeserializeObject<HomePageViewModel>(System.IO.File.ReadAllText(
                    Path.Combine(LocalStorageContainer, $"{AppUserState.Wgs}{AppUserState.Ds}{AppUserState.UserId}")));
            hvm.AutoCloseCharms = false;
            hvm.AutoScrollCaro = autoScroll;
            hvm.VerticalTrees = false;
            System.IO.File.WriteAllText(
                Path.Combine(LocalStorageContainer, $"{AppUserState.Wgs}{AppUserState.Ds}{AppUserState.UserId}"),
                JsonConvert.SerializeObject(hvm));
        }

        public PartialViewResult RefreshRecordTile(long uri)
        {
            var hvm = new RecordTileViewModel();
            if (AppUserState == null || AppUserState.Connected == false) return null;
            var conn = new InforConnection(tu: _tu, tup: _tup);
            hvm.Record =
                ((InforRecord) conn.GetRecordWeb(uri, AppUserState.UserName, AppUserState.Wgs, AppUserState.Ds))
                .ToLite();
            return PartialView("_RecordTile", hvm);
        }

        public PartialViewResult RefreshProcessTile(long uri)
        {
            var hvm = new ProcessTileViewModel();
            if (AppUserState == null || AppUserState.Connected == false) return null;
            var conn = new InforConnection(tu: _tu, tup: _tup);
            hvm.Process = conn.GetProcessWeb(uri, AppUserState.UserName, AppUserState.Wgs, AppUserState.Ds);
            return PartialView("/Views/Process/_ProcessTile.cshtml", hvm);
        }

        public PartialViewResult GetActionsView(long uri, bool vertical)
        {
            var hvm = new ActionsViewModel();
            if (AppUserState == null || AppUserState.Connected == false) return null;
            var conn = new InforConnection(tu: _tu, tup: _tup);
            var rec = (InforRecord) conn.GetRecordWeb(uri, AppUserState.UserName, AppUserState.Wgs, AppUserState.Ds);
            hvm.Actions = ActionService.GetActions(AppUserState.Wgs, AppUserState.Ds, AppUserState.UserName, uri);
            hvm.RecordNumber = rec.RecordNumber;
            var firstOrDefault = hvm.Actions.FirstOrDefault(ac => ac.Status == InforActionStatus.Current);
            if (firstOrDefault != null) hvm.CurrentActionUri = firstOrDefault.Uri;
            hvm.RecordUri = rec.Uri;
            hvm.Title = rec.Title;
            hvm.VerticalView = vertical;
            hvm.CanAddActions = rec.CanAddActions;
            return PartialView("_Actions", hvm);
        }

        public PartialViewResult GetProcessView(long uri, bool vertical)
        {
            var path = System.Web.HttpContext.Current.Server.MapPath("~/Content/Images/UserImages/");
            if (AppUserState == null || AppUserState.Connected == false) return null;
            var res = ProcessService.GetProcessTree(AppUserState.Wgs, AppUserState.Ds, AppUserState.UserName, uri,
                path);
            if (res.Faults.Any())
            {
                ErrorDisplay.ShowError(res.Faults[0].Message);
                return PartialView("/Views/Process/_Process.cshtml", _processViewModel);
            }

            _processViewModel.FlatNodes = (InforNodes) res.ReturnObject;
            var mainrec = _processViewModel.FlatNodes[0].Record;
            _processViewModel.RecordNumber = mainrec.RecordNumber;
            _processViewModel.ProcessRootUri = uri;
            _processViewModel.Uri = uri;
            _processViewModel.Title = mainrec.Title;
            return PartialView("/Views/Process/_Process.cshtml", _processViewModel);
        }

        public JsonResult GetProcessCompletionStamps(long uri, long rootUri)
        {
            var stampresult =
                ProcessService.GetProcessStamps(AppUserState.Wgs, AppUserState.Ds, AppUserState.UserName, uri);
            return this.Jsonp((InforProcessStamps) stampresult.ReturnObject);
        }

        public PartialViewResult GetCompleteStepPartialView(long uri, long rootUri)
        {
            var stampresult =
                ProcessService.GetProcessStamps(AppUserState.Wgs, AppUserState.Ds, AppUserState.UserName, uri);
            if (stampresult.Faults.Any())
            {
                ErrorDisplay.ShowError(stampresult.Faults[0].Message);
                return PartialView("/Views/Process/_CompleteProcess.cshtml", _completeProcessViewModel);
            }

            var conn = new InforConnection(tu: _tu, tup: _tup);
            var rec = (InforRecord) conn.GetRecordWeb(uri, AppUserState.UserName, AppUserState.Wgs, AppUserState.Ds);
            _completeProcessViewModel.Title = rec.Title;
            _completeProcessViewModel.RecordNumber = rec.RecordNumber;
            _completeProcessViewModel.Uri = uri;
            _completeProcessViewModel.RootUri = rootUri;
            _completeProcessViewModel.Stamps = (InforProcessStamps) stampresult.ReturnObject;
            return PartialView("/Views/Process/_CompleteProcess.cshtml", _completeProcessViewModel);
        }


        public PartialViewResult GetNotesView(long uri, long processUri)
        {
            var hvm = new NotesViewModel();
            if (AppUserState == null || AppUserState.Connected == false) return null;
            var conn = new InforConnection(tu: _tu, tup: _tup);
            var rec = (InforRecord) conn.GetRecordWeb(uri, AppUserState.UserName, AppUserState.Wgs, AppUserState.Ds);
            hvm.RecordNumber = rec.RecordNumber;
            hvm.RecordUri = rec.Uri;
            hvm.Title = rec.Title;
            hvm.Notes = rec.DisplayNotes;
            hvm.ReadOnly = !rec.CanAddToNotes;
            hvm.ProcessUri = processUri;
            return PartialView("_Notes", hvm);
        }

        public PartialViewResult GetNodeNotesView(long uri)
        {
            var hvm = new NotesViewModel();
            if (AppUserState == null || AppUserState.Connected == false) return null;
            var conn = new InforConnection(tu: _tu, tup: _tup);
            var rec = (InforRecord) conn.GetRecordWeb(uri, AppUserState.UserName, AppUserState.Wgs, AppUserState.Ds);
            hvm.RecordNumber = rec.RecordNumber;
            hvm.RecordUri = rec.Uri;
            hvm.Title = rec.Title;
            hvm.Notes = rec.DisplayNotes;
            hvm.ProcessUri = uri;
            hvm.ReadOnly = true;
            return PartialView("_Notes", hvm);
        }

        public PartialViewResult GetProcessActions(long uri, long processRootUri)
        {
            if (AppUserState == null || AppUserState.Connected == false) return null;
            var conn = new InforConnection(tu: _tu, tup: _tup);
            var result = ProcessService.GetProcessNode(AppUserState.Wgs, AppUserState.Ds, AppUserState.UserName, uri);
            if (result.Faults.Any())
            {
                ErrorDisplay.ShowError(result.Faults[0].Message);
                return PartialView("/Views/Process/_ProcessActions.cshtml", _processActionsViewModel);
            }

            var process = (InforNode) result.ReturnObject;
            var rec = (InforRecord) conn.GetRecordWeb(uri, AppUserState.UserName, AppUserState.Wgs, AppUserState.Ds);
            _processActionsViewModel.Uri = uri;
            _processActionsViewModel.ParentUri = processRootUri;
            _processActionsViewModel.Node = new InforNodes {process};
            _processActionsViewModel.Title = $"{process.AllocationString} {process.DateDueString}";
            _processActionsViewModel.Notes = rec.DisplayNotes;
            return PartialView("/Views/Process/_ProcessActions.cshtml", _processActionsViewModel);
        }

        public PartialViewResult GetPropertiesView(long uri)
        {
            var hvm = new PropertiesViewModel();
            if (AppUserState == null || AppUserState.Connected == false) return null;
            var conn = new InforConnection(tu: _tu, tup: _tup);
            var rec = (InforRecord) conn.GetRecordWeb(uri, AppUserState.UserName, AppUserState.Wgs, AppUserState.Ds);
            hvm.RecordNumber = rec.RecordNumber;
            hvm.RecordUri = rec.Uri;
            hvm.Title = rec.Title;
            hvm.Properties = rec.Properties;
            return PartialView("_Properties", hvm);
        }

        public PartialViewResult ReassPartialViewResult(long id, long addBelow, long addAbove, long processUri)
        {
            if (AppUserState == null || AppUserState.Connected == false) return null;
            var conn = new InforConnection(tu: _tu, tup: _tup);
            DateTime? duedate = null;
            var uri = id;
            long currentLocationUri = -1;
            var currentLocation = "";
            if (id > -1)
            {
                var process = conn.GetProcessWeb(id, AppUserState.UserName, AppUserState.Wgs, AppUserState.Ds);
                duedate = process.DueDate;
                uri = process.Uri;
                currentLocationUri = process.Location.UserId;
                currentLocation = process.Location.Name;
            }

            var rec = (InforRecord) conn.GetRecordWeb(uri, AppUserState.UserName, AppUserState.Wgs, AppUserState.Ds);
            //var lcoationresult = LocationService.GetLocations(AppUserState.Wgs, AppUserState.Ds, AppUserState.UserName, SearchType.ProcessAll, 0);
            //if (lcoationresult.Faults.Any())
            //{
            //    ErrorDisplay.ShowError(lcoationresult.Faults[0].Message);
            //    return PartialView("/Views/Process/_Reassign.cshtml", _reasignLocationViewModelViewModel);
            //}
            _reasignLocationViewModelViewModel.ProcessUri = processUri;
            _reasignLocationViewModelViewModel.AddAbove = addAbove;
            _reasignLocationViewModelViewModel.AddBelow = addBelow;
            _reasignLocationViewModelViewModel.NodeUri = uri;
            _reasignLocationViewModelViewModel.CurrentNodeDueDate = duedate;
            _reasignLocationViewModelViewModel.TxtBoxTitle = "Select Location";
            _reasignLocationViewModelViewModel.ShowExternal = true;
            _reasignLocationViewModelViewModel.ShowInternal = true;
            _reasignLocationViewModelViewModel.CurrentLocationUri = currentLocationUri;
            _reasignLocationViewModelViewModel.CurrentLocationName = currentLocation;
            _reasignLocationViewModelViewModel.Title = rec.Title;
            _reasignLocationViewModelViewModel.RecordNumber = rec.RecordNumber;
            //_reasignLocationViewModelViewModel.Locations = (InforLocations)lcoationresult.ReturnObject;
            return PartialView("/Views/Process/_Reassign.cshtml", _reasignLocationViewModelViewModel);
        }

        public PartialViewResult LocationPickerPartialViewResult()
        {
            if (AppUserState == null || AppUserState.Connected == false) return null;
            var lcoationresult = LocationService.GetLocations(AppUserState.Wgs, AppUserState.Ds, AppUserState.UserName,
                LocationSearchType.All, -1);
            if (lcoationresult.Faults.Any())
            {
                ErrorDisplay.ShowError(lcoationresult.Faults[0].Message);
                return PartialView("/Views/Users/_LocationPickerPartial.cshtml", _locationPickerViewModel);
            }

            _locationPickerViewModel.TxtBoxTitle = "Select Location";
            _locationPickerViewModel.ShowExternal = true;
            _locationPickerViewModel.ShowInternal = true;
            //_locationPickerViewModel.CurrentLocationUri = currentLocationUri;
            _locationPickerViewModel.Locations = (InforLocations) lcoationresult.ReturnObject;
            return PartialView("/Views/Users/_LocationPickerPartial.cshtml", _locationPickerViewModel);
        }

        public JsonResult FlagProcessTask(long uri, string stamp, string comments)
        {
            InforActionResult result = null;
            var flag = JsonConvert.DeserializeObject<InforProcessStamp>(stamp);
            if (AppUserState == null || AppUserState.Connected == false) return null;
            result = ProcessService.FlagProcess(AppUserState.Wgs, AppUserState.Ds, AppUserState.UserName, uri, flag,
                comments);
            return this.Jsonp(result);
        }

        public JsonResult ReasignNode(long processNode, long responsible, DateTime? dueDate, string notes,
            long addAboveUri = 0, long addBelowUri = 0)
        {
            var result = new InforActionResult();
            try
            {
                if (AppUserState == null || AppUserState.Connected == false) return null;
                if (addAboveUri > 0 || addBelowUri > 0)
                {
                    if (addAboveUri > 0)
                        result = ProcessService.AddStepAbove(AppUserState.Wgs, AppUserState.Ds, AppUserState.UserName,
                            addAboveUri, responsible, dueDate, notes);
                    if (addBelowUri > 0)
                        result = ProcessService.AddStepBelow(AppUserState.Wgs, AppUserState.Ds, AppUserState.UserName,
                            addBelowUri, responsible, dueDate, notes);
                }
                else
                {
                    if (processNode > 0)
                        result = ProcessService.ReassignProcess(AppUserState.Wgs, AppUserState.Ds, AppUserState.UserName,
                            processNode, notes, responsible, dueDate);
                }

                
            }
            catch (Exception e)
            {
                result.Success = false;
                result.Faults.Add(new InforFault{Message = $"Error {e.Message}", IsWarningOnly = false});
            }
            return this.Jsonp(result);
        }
    }
}