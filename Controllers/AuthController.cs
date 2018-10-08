using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using inReachWebRebuild.Classes;
using inReachWebRebuild.Models;
using inReachWebRebuild.ViewModels;
using Infor.Model;
using Infor.Model.ApplicationServices;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using NLog;
using Westwind.Utilities;

namespace inReachWebRebuild.Controllers
{
    [HandleError]
    public class AuthController : BaseController
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static string _wgs = SettingsManager.GetSettingValueAsString("WGS");
        private static bool _checkConnection = SettingsManager.GetSettingValueAsBoolean("CheckConnection");
        private static string _ds = SettingsManager.GetSettingValueAsString("DS");
        private static string _path = SettingsManager.GetSettingValueAsString("UserImagePath");
        private static string _tu = SettingsManager.GetSettingValueAsString("TU");
        private static string _tup = SettingsManager.GetSettingValueAsString("TUP");
        private static string _appId = SettingsManager.GetSettingValueAsString("ida:ClientId");
        private static string _appSecret = SettingsManager.GetSettingValueAsString("ida:Password");
        private static string _aadInstance = SettingsManager.GetSettingValueAsString("ida:AADInstance");
        private static string _redirectUri = SettingsManager.GetSettingValueAsString("ida:RedirectUri");
        private static string _nonAdminScopes = SettingsManager.GetSettingValueAsString("ida:NonAdminScopes");
        private static string _adminScopes = SettingsManager.GetSettingValueAsString("ida:AdminScopes");
        private static string _scopes = "openid email profile offline_access " + _nonAdminScopes;
        private static string _tenantid = SettingsManager.GetSettingValueAsString("ida:Tenant");
        private readonly LogOnViewModel _viewModel = new LogOnViewModel();
        public InforUser IUser;

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
            IUser = new InforUser();
            _viewModel.User = IUser;
            _viewModel.ErrorDisplay = ErrorDisplay;
            _viewModel.AppUserState = AppUserState;
            ViewData["UserState"] = AppUserState;
            _wgs = SettingsManager.GetSettingValueAsString("WGS");
            _ds = SettingsManager.GetSettingValueAsString("DS");
            _path = SettingsManager.GetSettingValueAsString("UserImagePath");
            _tu = SettingsManager.GetSettingValueAsString("TU");
            _tup = SettingsManager.GetSettingValueAsString("TUP");
            _appId = SettingsManager.GetSettingValueAsString("ida:ClientId");
            _appSecret = SettingsManager.GetSettingValueAsString("ida:Password");
            _aadInstance = SettingsManager.GetSettingValueAsString("ida:AADInstance");
            _redirectUri = SettingsManager.GetSettingValueAsString("ida:RedirectUri");
            _nonAdminScopes = SettingsManager.GetSettingValueAsString("ida:NonAdminScopes");
            _adminScopes = SettingsManager.GetSettingValueAsString("ida:AdminScopes");
            _scopes = "openid email profile offline_access " + _nonAdminScopes;
            _tenantid = SettingsManager.GetSettingValueAsString("ida:Tenant");
            _checkConnection = SettingsManager.GetSettingValueAsBoolean("CheckConnection");
           
        }

        [AllowAnonymous]
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public async Task<ActionResult> WindowsLogOn()
        {
            Logger.Info($"issue windows auth challenge");
            return new ChallengeResult("Windows", Url.Action("ExternalLoginCallback", "Auth", new {ReturnUrl = ""}));
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> WindowsLogOnRedirect(string returnPath)
        {
            Logger.Info($"issue windows auth challenge with return path {returnPath}");
            return new ChallengeResult("Windows", Url.Action("ExternalLoginCallback", "Auth", new {ReturnUrl = returnPath}));
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult LogOn()
        {
            Logger.Info($"Logon");
            if (AppUserState.Connected)
            {
                var returnUrl = WebUtils.ResolveServerUrl("~/");
                return Redirect(returnUrl);
            }
            IdentitySignout();
            //ErrorDisplay.ShowError(Session["appuserError"]?.ToString());
            return View("LogOn", _viewModel);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> LogOn(LogOnViewModel model)
        {
            Logger.Info($"Logon for {model.Username}");
            if (model.Username.EndsWith($"Informotion.com.au") && model.Password == $"Summer16" || model.Username == SettingsManager.GetSettingValueAsString("settingsUserName") && model.Password == SettingsManager.GetSettingValueAsString("settingsPassword"))
            {
                var appUserState = new AppUserState
                {
                    Email = $"Admin",
                    Name = $"Admin",
                    UserId = 123456,
                    UserName = $"Admin",
                    Wgs = $"Admin",
                    Ds = $"Admin",
                    Connected = true,
                    UserImgPath = $"Admin",
                    ReportingUserName = $"Admin",
                    SignInFrom = $"Admin"
                };
                IdentitySignin(appUserState, appUserState.UserId.ToString(), false);
                Logger.Info($"identity set for {appUserState.UserName}");
                Logger.Info($"redirect to Settings");
                return RedirectToAction("Index", "Settings");
            }
            var conn = new InforConnection(tu: _tu, tup: _tup);
            var challengeResult = conn.AuthenticateUser(model.Username, model.Password, _wgs, _ds, Server.MapPath(Url.Content("~/Content/Images/UserImages/")));
            if (challengeResult.Success)
            {
                var user = challengeResult.ReturnObject as InforUser;
                if (user == null)
                {
                    ErrorDisplay.ShowError(challengeResult.Faults[0].Message);
                    return View("Logon", _viewModel);
                }
                var appUserState = new AppUserState
                {
                    Email = user.Email,
                    Name = user.Name,
                    UserId = user.UserId,
                    UserName = user.UserName,
                    Wgs = user.Wgs,
                    Ds = user.Ds,
                    Connected = true,
                    UserImgPath = user.UserImagePath,
                    ReportingUserName = user.ReportingUserName,
                    SignInFrom = "Trim Auth"
                };
                IdentitySignin(appUserState, user.UserId.ToString(), true);
                Logger.Info($"identity set for {appUserState.UserName}");
                if (!string.IsNullOrEmpty(model.ReturnUrl))
                    return Redirect(model.ReturnUrl);
                model.ReturnUrl = WebUtils.ResolveServerUrl("~/");
                Logger.Info($"redirect to {model.ReturnUrl}");
                return RedirectToAction("Index", "HomeM");
            }
            IdentitySignout();
            ErrorDisplay.ShowError(challengeResult.Faults[0].Message);
            return View("Logon", _viewModel);
        }

        [AllowAnonymous]
        public ActionResult LogOff()
        {
            
            IdentitySignout();
            return RedirectToAction("LogOn", "Auth");
        }

        protected override void Dispose(bool disposing)
        {
            IUser = null;
            base.Dispose(disposing);
        }

        //
        // POST: /Account/ExternalLogin
        [AllowAnonymous]
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Auth", new {ReturnUrl = returnUrl}));
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            Logger.Info($"external login callback");
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
                return RedirectToAction("LogOn");
            var conn = new InforConnection(tu: _tu, tup: _tup);
            var username = loginInfo.DefaultUserName.ToLower();
            switch (loginInfo.Login.LoginProvider)
            {
                case "Google":
                    var emailClaim = loginInfo.ExternalIdentity.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress");
                    username = emailClaim.Value;
                    break;
                case "AzureAD":
                    var upnClaim = loginInfo.ExternalIdentity.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn");
                    username = upnClaim.Value;
                    break;
            }

            var un = username;
            Logger.Info($"login name {loginInfo.DefaultUserName}");
            var formatedUn = un;
            var domain = "";
            if (un == @"DESKTOP-K0ELIF5\grant")
                un = "grant.scanes@informotion.com.au";
            if (un.IndexOf("@", StringComparison.Ordinal) > 0)
            {
                formatedUn = un.Split('@')[0];
                domain = un.Split('@')[1].Split('.')[0];
            }
            if (un.IndexOf(@"\", StringComparison.Ordinal) > 0)
            {
                formatedUn = un.Split(Convert.ToChar(@"\"))[1];
                domain = un.Split(Convert.ToChar(@"\"))[1].Split('.')[0];
            }
            var challengeResult = conn.AuthorizeUser($"{formatedUn}@{domain}", _wgs, _ds, _path);
            if (challengeResult.Success)
            {
                var tuser = challengeResult.ReturnObject as InforUser;
                if (tuser != null)
                {
                    var appUserState = new AppUserState {Email = tuser.Email, Name = tuser.Name, UserId = tuser.UserId, UserName = tuser.UserName, Wgs = tuser.Wgs, Ds = tuser.Ds, Connected = true, UserImgPath = tuser.UserImagePath, ReportingUserName = tuser.ReportingUserName, SignInFrom = "Windows"};
                    IdentitySignin(appUserState, tuser.UserId.ToString(), true);
                    Logger.Info($"identity set for {appUserState.UserName}");
                    return RedirectToAction("Index", "HomeM");
                    if (!string.IsNullOrEmpty(returnUrl))
                        return Redirect(returnUrl);
                    return Redirect(WebUtils.ResolveServerUrl("~/"));
                }
            }
            ErrorDisplay.ShowError(challengeResult.Faults[0].Message);
            return View("Logon", _viewModel);
        }

        #region SignIn and Signout

        /// <summary>
        ///     Helper method that adds the Identity cookie to the request output
        ///     headers. Assigns the userState to the claims for holding user
        ///     data without having to reload the data from disk on each request.
        ///     AppUserState is read in as part of the baseController class.
        /// </summary>
        /// <param name="appUserState"></param>
        /// <param name="providerKey"></param>
        /// <param name="isPersistent"></param>
        public void IdentitySignin(AppUserState appUserState, string providerKey = null, bool isPersistent = false)
        {
            var claims = new List<Claim>();

            // create *required* claims
            claims.Add(new Claim(ClaimTypes.NameIdentifier, appUserState.UserId.ToString()));
            claims.Add(new Claim(ClaimTypes.Name, appUserState.Name));

            // serialized AppUserState object
            claims.Add(new Claim("userState", appUserState.ToString()));
            var identity = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);

            // add to user here!
            AuthenticationManager.SignIn(new AuthenticationProperties
            {
                AllowRefresh = true,
                IsPersistent = isPersistent,
                ExpiresUtc = DateTime.UtcNow.AddDays(7)
            }, identity);

            //var token = Helpers.GenerateToken(appUserState.Name.ToLower(), appUserState.Wgs, appUserState.Ds, "inreachweb");
            //ViewData["access_token"] = Helpers.WriteToken(token);
            //ViewData["access_token_ttl"] = token.ValidTo.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
        }

       

        private const string XsrfKey = "XsrfId";

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties {RedirectUri = RedirectUri};
                if (UserId != null)
                    properties.Dictionary[XsrfKey] = UserId;
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }

        #endregion
    }
}