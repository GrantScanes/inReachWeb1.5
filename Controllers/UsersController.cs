/* 
*  Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license. 
*  See LICENSE in the source repository root for complete license information. 
*/

using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using inReachWebRebuild.Classes;
using inReachWebRebuild.Common;
using inReachWebRebuild.ViewModels;
using Infor.Model;
using Infor.Model.ApplicationServices;
using Infor.Model.WebServices.LocationServices;
using Newtonsoft.Json;

namespace inReachWebRebuild.Controllers
{
    [Authorize]
    public class UsersController : BaseController
    {
        private static readonly string ReportserverUrl = SettingsManager.GetSettingValueAsString("rpurl");

        private static string _tu = SettingsManager.GetSettingValueAsString("TU");
        private static string _tup = SettingsManager.GetSettingValueAsString("TUP");
        //private readonly UsersService _usersService = new UsersService();

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
            _tu = SettingsManager.GetSettingValueAsString("TU");
            _tup = SettingsManager.GetSettingValueAsString("TUP");

            ViewData["UserState"] = AppUserState;
        }


        // Load the view.
        public ActionResult Index()
        {
            return View("Users");
        }

        // Get all users.
        //public async Task<ActionResult> GetUsers()
        //{
        //    var results = new ResultsViewModel();
        //    try
        //    {
        //        // Initialize the GraphServiceClient.
        //        var graphClient = await SDKHelper.GetAuthenticatedClient();

        //        // Get users.
        //        results.Items = await _usersService.GetUsers(graphClient);
        //    }
        //    catch (ServiceException se)
        //    {
        //        if (se.Error.Message == Resource.Error_AuthChallengeNeeded) return new EmptyResult();
        //        return RedirectToAction("Index", "Error",
        //            new
        //            {
        //                message = string.Format(Resource.Error_Message, Request.RawUrl, se.Error.Code, se.Error.Message)
        //            });
        //    }

        //    return View("Users", results);
        //}

        // Get the current user's profile.
        //public async Task<ActionResult> GetMe()
        //{
        //    var results = new ResultsViewModel();
        //    try
        //    {
        //        // Initialize the GraphServiceClient.
        //        var graphClient = await SDKHelper.GetAuthenticatedClient();

        //        // Get the current user's profile.
        //        results.Items = await _usersService.GetMe(graphClient);
        //    }
        //    catch (ServiceException se)
        //    {
        //        if (se.Error.Message == Resource.Error_AuthChallengeNeeded) return new EmptyResult();
        //        return RedirectToAction("Index", "Error",
        //            new
        //            {
        //                message = string.Format(Resource.Error_Message, Request.RawUrl, se.Error.Code, se.Error.Message)
        //            });
        //    }

        //    return View("Users", results);
        //}

        // Get the current user's manager.
        //public async Task<ActionResult> GetMyManager()
        //{
        //    var results = new ResultsViewModel();
        //    try
        //    {
        //        // Initialize the GraphServiceClient.
        //        var graphClient = await SDKHelper.GetAuthenticatedClient();

        //        // Get the current user's manager.
        //        results.Items = await _usersService.GetMyManager(graphClient);
        //    }

        //    // Throws exception if manager is null, with Request_ResourceNotFound code.
        //    catch (ServiceException se)
        //    {
        //        if (se.Error.Message == Resource.Error_AuthChallengeNeeded) return new EmptyResult();
        //        return RedirectToAction("Index", "Error",
        //            new
        //            {
        //                message = string.Format(Resource.Error_Message, Request.RawUrl, se.Error.Code, se.Error.Message)
        //            });
        //    }

        //    return View("Users", results);
        //}

        // Get the current user's photo. 
        //public async Task<ActionResult> GetMyPhoto()
        //{
        //    var results = new ResultsViewModel();
        //    results.Selectable = false;
        //    try
        //    {
        //        // Initialize the GraphServiceClient.
        //        var graphClient = await SDKHelper.GetAuthenticatedClient();

        //        // Get my photo.
        //        results.Items = await _usersService.GetMyPhoto(graphClient);
        //    }

        //    // Throws exception if photo is null, with itemNotFound code.
        //    catch (ServiceException se)
        //    {
        //        if (se.Error.Message == Resource.Error_AuthChallengeNeeded) return new EmptyResult();
        //        return RedirectToAction("Index", "Error",
        //            new
        //            {
        //                message = string.Format(Resource.Error_Message, Request.RawUrl, se.Error.Code, se.Error.Message)
        //            });
        //    }

        //    return View("Users", results);
        //}

        // Create a new user in the signed-in user's tenant.
        // This snippet requires an admin work account. 
        //public async Task<ActionResult> CreateUser()
        //{
        //    var results = new ResultsViewModel();
        //    try
        //    {
        //        // Initialize the GraphServiceClient.
        //        var graphClient = await SDKHelper.GetAuthenticatedClient();

        //        // Add the user.
        //        results.Items = await _usersService.CreateUser(graphClient);
        //    }
        //    catch (ServiceException se)
        //    {
        //        if (se.Error.Message == Resource.Error_AuthChallengeNeeded) return new EmptyResult();
        //        return RedirectToAction("Index", "Error",
        //            new
        //            {
        //                message = string.Format(Resource.Error_Message, Request.RawUrl, se.Error.Code, se.Error.Message)
        //            });
        //    }

        //    return View("Users", results);
        //}

        // Get a specified user.
        //public async Task<ActionResult> GetUser(string id)
        //{
        //    var results = new ResultsViewModel();
        //    try
        //    {
        //        // Initialize the GraphServiceClient.
        //        var graphClient = await SDKHelper.GetAuthenticatedClient();

        //        // Get the user.
        //        results.Items = await _usersService.GetUser(graphClient, id);
        //    }
        //    catch (ServiceException se)
        //    {
        //        if (se.Error.Message == Resource.Error_AuthChallengeNeeded) return new EmptyResult();
        //        return RedirectToAction("Index", "Error",
        //            new
        //            {
        //                message = string.Format(Resource.Error_Message, Request.RawUrl, se.Error.Code, se.Error.Message)
        //            });
        //    }

        //    return View("Users", results);
        //}

        // Get a specified user's photo.
        //public async Task<ActionResult> GetUserPhoto(string id)
        //{
        //    var results = new ResultsViewModel();
        //    results.Selectable = false;
        //    try
        //    {
        //        // Initialize the GraphServiceClient.
        //        var graphClient = await SDKHelper.GetAuthenticatedClient();

        //        // Get the user's photo.
        //        results.Items = await _usersService.GetUserPhoto(graphClient, id);
        //    }

        //    // Throws an exception when requesting the photo for unlicensed users (such as those created by this sample), with message "The requested user '<user-name>' is invalid."
        //    catch (ServiceException se)
        //    {
        //        if (se.Error.Message == Resource.Error_AuthChallengeNeeded) return new EmptyResult();
        //        return RedirectToAction("Index", "Error",
        //            new
        //            {
        //                message = string.Format(Resource.Error_Message, Request.RawUrl, se.Error.Code, se.Error.Message)
        //            });
        //    }

        //    return View("Users", results);
        //}

        // Get the direct reports of a specified user.
        //public async Task<ActionResult> GetDirectReports(string id)
        //{
        //    var results = new ResultsViewModel();
        //    try
        //    {
        //        // Initialize the GraphServiceClient.
        //        var graphClient = await SDKHelper.GetAuthenticatedClient();

        //        // Get user's direct reports.
        //        results.Items = await _usersService.GetDirectReports(graphClient, id);
        //    }
        //    catch (ServiceException se)
        //    {
        //        if (se.Error.Message == Resource.Error_AuthChallengeNeeded) return new EmptyResult();
        //        return RedirectToAction("Index", "Error",
        //            new
        //            {
        //                message = string.Format(Resource.Error_Message, Request.RawUrl, se.Error.Code, se.Error.Message)
        //            });
        //    }

        //    return View("Users", results);
        //}

        // Update a user.
        // This snippet changes the user's display name. 
        // This snippet requires an admin work account. 
        //public async Task<ActionResult> UpdateUser(string id, string name)
        //{
        //    var results = new ResultsViewModel();
        //    results.Selectable = false;
        //    try
        //    {
        //        // Initialize the GraphServiceClient.
        //        var graphClient = await SDKHelper.GetAuthenticatedClient();

        //        // Change user display name.
        //        results.Items = await _usersService.UpdateUser(graphClient, id, name);
        //    }
        //    catch (ServiceException se)
        //    {
        //        if (se.Error.Message == Resource.Error_AuthChallengeNeeded) return new EmptyResult();
        //        return RedirectToAction("Index", "Error",
        //            new
        //            {
        //                message = string.Format(Resource.Error_Message, Request.RawUrl, se.Error.Code, se.Error.Message)
        //            });
        //    }

        //    return View("Users", results);
        //}

        // Delete a user. Warning: This operation cannot be undone.
        // This snippet requires an admin work account. 
        //public async Task<ActionResult> DeleteUser(string id)
        //{
        //    var results = new ResultsViewModel();
        //    results.Selectable = false;
        //    try
        //    {
        //        // Initialize the GraphServiceClient.
        //        var graphClient = await SDKHelper.GetAuthenticatedClient();

        //        // Make sure that the current user is not selected.
        //        results.Items = await _usersService.DeleteUser(graphClient, id);
        //    }
        //    catch (ServiceException se)
        //    {
        //        if (se.Error.Message == Resource.Error_AuthChallengeNeeded) return new EmptyResult();
        //        return RedirectToAction("Index", "Error",
        //            new
        //            {
        //                message = string.Format(Resource.Error_Message, Request.RawUrl, se.Error.Code, se.Error.Message)
        //            });
        //    }

        //    return View("Users", results);
        //}

        public PartialViewResult _LocationPickerPartial(string groupName, long? userId)
        {
            var vm = new LocationPickerViewModel();
            var conn = new InforConnection(reportServerAddress: $"{ReportserverUrl}", tu: _tu, tup: _tup);
            //var locs = conn.GetLocationsForGroup(AppUserState.UserName, AppUserState.Wgs, AppUserState.Ds, groupName, Server.MapPath("~/Content/Images/UserImages/") );
            //vm.Locations = locs;
            vm.ShowToggles = false;
            vm.SelectedLocationUri = userId ?? (long) 0;
            return PartialView(vm);
        }


        public JsonResult GetLocations(string groupName, long? userId)
        {
            var path = System.Web.HttpContext.Current.Server.MapPath("~/Content/Images/UserImages/");

            var conn = new InforConnection(reportServerAddress: $"{ReportserverUrl}", tu: _tu, tup: _tup);
            InforLocations locs;
            if (string.IsNullOrEmpty(groupName))
                locs = (InforLocations) LocationService.GetLocations(AppUserState.Wgs, AppUserState.Ds,
                    AppUserState.UserName, LocationSearchType.ProcessAll, 0, "", path).ReturnObject;
            else
                locs = conn.GetLocationsForGroup(AppUserState.UserName, AppUserState.Wgs, AppUserState.Ds, groupName,
                    path);
            if (userId == null) return this.Jsonp(locs);
            if (!(userId > 0) || locs.Any(id => id.UserId == userId)) return this.Jsonp(locs);
            var notFoundLoc = (InforLocation) LocationService
                .GetSpecificLocation(AppUserState.Wgs, AppUserState.Ds, AppUserState.UserName, (long) userId)
                .ReturnObject;
            if (notFoundLoc != null)
                locs.Add(notFoundLoc);

            return this.Jsonp(locs);
        }

        public JsonResult GetLocationsActions(string groupName, long? userId)
        {
            var path = System.Web.HttpContext.Current.Server.MapPath("~/Content/Images/UserImages/");

            var conn = new InforConnection(reportServerAddress: $"{ReportserverUrl}", tu: _tu, tup: _tup);
            InforLocations locs;
            if (string.IsNullOrEmpty(groupName))
                locs = (InforLocations) LocationService.GetLocations(AppUserState.Wgs, AppUserState.Ds,
                    AppUserState.UserName, LocationSearchType.ActiveValid, 0, "", path).ReturnObject;
            else
                locs = conn.GetLocationsForGroup(AppUserState.UserName, AppUserState.Wgs, AppUserState.Ds, groupName,
                    path);

            if (userId == null) return this.Jsonp(locs);
            if (locs.All(id => id.UserId != userId))
                locs.Add((InforLocation) LocationService
                    .GetSpecificLocation(AppUserState.Wgs, AppUserState.Ds, AppUserState.UserName, (long) userId)
                    .ReturnObject);
            return this.Jsonp(locs);
        }

        public ActionResult ValueMapper(long[] values)
        {
            var indices = new List<int>();

            if (values == null || !values.Any()) return this.Jsonp(indices);
            var index = 0;
            var cachePath = System.Web.HttpContext.Current.Server.MapPath($"~/App_Data/inReachRepos/Global/Cache");
            if (!System.IO.File.Exists($"{cachePath}/accloc")) return null;
            var locCache =
                JsonConvert.DeserializeObject<LocationCache>(System.IO.File.ReadAllText($"{cachePath}/accloc"));

            var locs = locCache.Locations;
            foreach (var loc in locs)
            {
                if (values.Contains(loc.UserId)) indices.Add(index);

                index += 1;
            }

            return this.Jsonp(indices);
        }
    }
}