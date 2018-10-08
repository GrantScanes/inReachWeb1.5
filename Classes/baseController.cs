using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Mvc;

using System.Web.Routing;
using inReachWebRebuild.Controllers;
using inReachWebRebuild.Models;
using Infor.Model.ApplicationServices;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.OData;
using Microsoft.Owin.Security;
using NLog;

namespace inReachWebRebuild.Classes
{
    public class BaseController : Controller
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static string _wgs = SettingsManager.GetSettingValueAsString("WGS");
        private static bool _checkConnection = SettingsManager.GetSettingValueAsBoolean("CheckConnection");
        private static string _ds = SettingsManager.GetSettingValueAsString("DS");


        private ApplicationSignInManager _signInManager;
        public IAuthenticationManager AuthenticationManager => HttpContext.GetOwinContext().Authentication;

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set { _signInManager = value; }
        }

        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        /// <summary>
        ///     Contains User state information retrieved from the authentication system
        /// </summary>
        protected AppUserState AppUserState = new AppUserState();

        /// <summary>
        ///     ErrorDisplay control that holds page level error information
        /// </summary>
        protected ErrorDisplay ErrorDisplay = new ErrorDisplay();

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
            if(!SettingsManager.Settings.Any())
                SettingsManager.Initiase(Path.Combine(AppDomain.CurrentDomain.GetData("DataDirectory").ToString(), "inReachWeb"));


            _checkConnection = SettingsManager.GetSettingValueAsBoolean("CheckConnection");
            _wgs = SettingsManager.GetSettingValueAsString("WGS");
            _ds = SettingsManager.GetSettingValueAsString("DS");


            // Grab the user's login information from Identity
            var appUserState = new AppUserState();
            if (User is ClaimsPrincipal)
            {
               var ucl= ClaimsPrincipal.Current.Claims.ToList().FirstOrDefault(c => c.Type == "userState");
            
                var userStateString = ucl?.Value;
                Logger.Info($"ucl  {userStateString}");
                if (!string.IsNullOrEmpty(userStateString))
                {
                    appUserState.FromString(userStateString);
                }
            }
            AppUserState = appUserState;
            Logger.Info($"initialise user is  {AppUserState.Name}");
            ViewData["UserState"] = AppUserState;
            ViewData["ErrorDisplay"] = ErrorDisplay;
            if (!_checkConnection || !AppUserState.Connected || AppUserState.UserId == 123456) return;
            if ( _wgs == AppUserState.Wgs && _ds == AppUserState.Ds) return;
            IdentitySignout();
            requestContext.HttpContext.Response.Clear();
            requestContext.HttpContext.Response.Redirect(Url.Action("LogOff", "Auth"));
            requestContext.HttpContext.Response.End();
        }

        public void IdentitySignout()
        {
            AppUserState = new AppUserState();
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie,
                DefaultAuthenticationTypes.ExternalCookie);
        }

        public static string GetClaim(List<Claim> claims, string key)
        {
            var claim = claims.FirstOrDefault(c => c.Type == key);
            if (claim == null)
                return null;
            return claim.Value;
        }

        /// <summary>
        ///     Allow external initialization of this controller by explicitly
        ///     passing in a request context
        /// </summary>
        /// <param name="requestContext"></param>
        public void InitializeForced(RequestContext requestContext)
        {
            Initialize(requestContext);
        }

        /// <summary>
        ///     Displays a self contained error page without redirecting.
        ///     Depends on ErrorController.ShowError() to exist
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="redirectTo"></param>
        /// <returns></returns>
        protected internal ActionResult DisplayErrorPage(string title, string message, string redirectTo = null)
        {
            var controller = new ErrorController();
            controller.InitializeForced(ControllerContext.RequestContext);
            return controller.ShowError(title, message, redirectTo);
        }


        public static string RenderViewToString(ControllerContext context, string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = context.RouteData.GetRequiredString("action");

            var viewData = new ViewDataDictionary(model);

            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(context, viewName);
                var viewContext = new ViewContext(context, viewResult.View, viewData, new TempDataDictionary(), sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }
    }


    public class BaseODataController : ODataController
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
       
 

        

        /// <summary>
        ///     Contains User state information retrieved from the authentication system
        /// </summary>
        protected AppUserState AppUserState = new AppUserState();

        /// <inheritdoc />
        /// <summary>
        ///     ErrorDisplay control that holds page level error information
        /// </summary>
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            if (!SettingsManager.Settings.Any())
                SettingsManager.Initiase(Path.Combine(AppDomain.CurrentDomain.GetData("DataDirectory").ToString(), "inReachWeb"));


             
            

            // Grab the user's login information from Identity
            var appUserState = new AppUserState();
            if (User is ClaimsPrincipal)
            {
                var ucl = ClaimsPrincipal.Current.Claims.ToList().FirstOrDefault(c => c.Type == "userState");

                var userStateString = ucl?.Value;
                Logger.Info($"ucl  {userStateString}");
                if (!string.IsNullOrEmpty(userStateString))
                {
                    appUserState.FromString(userStateString);
                }
            }
            AppUserState = appUserState;
            Logger.Info($"initialise user is  {AppUserState.Name}");
            
             
            
        }
       
   
    }
}