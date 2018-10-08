using System;
using System.Data.Entity.Utilities;
using System.Security.Claims;
using System.Threading.Tasks;
using ElasticIdentity;
using inReachWebRebuild.Models;
using Infor.Model;
using Infor.Model.ApplicationServices;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;

namespace inReachWebRebuild
{
    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your email service here to send an email.
            return Task.FromResult(0);
        }
    }

    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }
    }

    //public class CustomUserSore : IUserStore<ApplicationUser>, IUserLockoutStore<ApplicationUser, string>, IUserPasswordStore<ApplicationUser>, IUserClaimStore<ApplicationUser>, IUserEmailStore<ApplicationUser>
    //{
    //    void IDisposable.Dispose()
    //    {
    //        // throw new NotImplementedException(); 
    //    }

    //    public Task CreateAsync(ApplicationUser user)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public Task UpdateAsync(ApplicationUser user)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public Task DeleteAsync(ApplicationUser user)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public Task<ApplicationUser> FindByIdAsync(string userId)
    //    {
    //        return Task.FromResult(new ApplicationUser {Id = userId});
    //    }

    //    public Task<ApplicationUser> FindByNameAsync(string userName)
    //    {
    //        return Task.FromResult(new ApplicationUser {UserName = userName});
    //    }

    //    #region LockoutStore

    //    public Task<int> GetAccessFailedCountAsync(ApplicationUser user)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public Task<bool> GetLockoutEnabledAsync(ApplicationUser user)
    //    {
    //        return Task.Factory.StartNew(() => false);
    //    }

    //    public Task<DateTimeOffset> GetLockoutEndDateAsync(ApplicationUser user)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public Task<int> IncrementAccessFailedCountAsync(ApplicationUser user)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public Task ResetAccessFailedCountAsync(ApplicationUser user)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public Task SetLockoutEnabledAsync(ApplicationUser user, bool enabled)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public Task SetLockoutEndDateAsync(ApplicationUser user, DateTimeOffset lockoutEnd)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public Task SetPasswordHashAsync(ApplicationUser user, string passwordHash)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public Task<string> GetPasswordHashAsync(ApplicationUser user)
    //    {
    //        if (user == null)
    //            throw new ArgumentNullException("user");
    //        return Task.FromResult(user.PasswordHash);
    //    }

    //    public Task<bool> HasPasswordAsync(ApplicationUser user)
    //    {
    //        return Task.FromResult(user.PasswordHash != null);
    //    }

    //    public Task<IList<Claim>> GetClaimsAsync(ApplicationUser user)
    //    {
    //        if (user == null)
    //            throw new ArgumentNullException("user");
    //        var retli = new List<Claim>();
    //        retli.AddRange(user.Claims.Select(clm => new Claim(clm.ClaimType, clm.ClaimValue)).ToList());
    //        return Task.FromResult<IList<Claim>>(retli);
    //    }

    //    public Task AddClaimAsync(ApplicationUser user, Claim claim)
    //    {
    //        //if (user == null) throw new ArgumentNullException("user");
    //        //if (claim == null) throw new ArgumentNullException("claim");

    //        //user.AddClaim(claim);
    //        //return Task.FromResult(0);
    //        throw new NotImplementedException();
    //    }

    //    public Task RemoveClaimAsync(ApplicationUser user, Claim claim)
    //    {
    //        throw new NotImplementedException();
    //        //if (user == null) throw new ArgumentNullException("user");
    //        //if (claim == null) throw new ArgumentNullException("claim");

    //        //var userClaim = user.Claims
    //        //    .FirstOrDefault(clm => clm.ClaimType == claim.Type && clm.ClaimValue == claim.Value);

    //        //if (userClaim != null)
    //        //{
    //        //    user.RemoveClaim(userClaim);
    //        //}

    //        //return Task.FromResult(0);
    //    }

    //    public Task SetEmailAsync(ApplicationUser user, string email)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public Task<string> GetEmailAsync(ApplicationUser user)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public Task<bool> GetEmailConfirmedAsync(ApplicationUser user)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public Task SetEmailConfirmedAsync(ApplicationUser user, bool confirmed)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public Task<ApplicationUser> FindByEmailAsync(string email)
    //    {
    //        throw new NotImplementedException();
    //        //if (email == null)
    //        //{
    //        //    throw new ArgumentNullException("email");
    //        //}

    //        //string keyToLookFor = InforUserEmail.GenerateKey(email);
    //        //InforUserEmail ravenUserEmail = await _documentSession
    //        //    .Include<InforUserEmail, TUser>(usrEmail => usrEmail.UserId)
    //        //    .LoadAsync(keyToLookFor)
    //        //    .ConfigureAwait(false);

    //        //return (ravenUserEmail != null)
    //        //    ? await _documentSession.LoadAsync<TUser>(ravenUserEmail.UserId).ConfigureAwait(false)
    //        //    : default(TUser);
    //    }

    //    #endregion
    //}

    public class MyPasswordHasher : IPasswordHasher
    {
        public string HashPassword(string password)
        {
            return password;
        }

        public PasswordVerificationResult VerifyHashedPassword(string hashedPassword, string providedPassword)
        {
            if (hashedPassword == HashPassword(providedPassword))
                return PasswordVerificationResult.Success;
            return PasswordVerificationResult.Failed;
        }
    }

    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
    public class ApplicationUserManager : UserManager<inforClaimUser>
    {
        public ApplicationUserManager(IUserStore<inforClaimUser> store) : base(store)
        {
        }

        public override Task<bool> CheckPasswordAsync(inforClaimUser user, string password)
        {
            //var conn = new InforConnection();

            //var challengeResult = conn.AuthenticateUser(user.UserName, password, wgs, ds, path);

            //if (challengeResult.Success)
            //{
            //    var tuser = challengeResult.ReturnObject as InforUser;
            //    if (tuser == null)
            //    {
            //        return Task.FromResult(false);
            //    }

            //    var appUserState = new AppUserState() {Email = tuser.Email, Name = tuser.Name, UserId = tuser.UserId, UserName = tuser.UserName, Wgs = tuser.Wgs, Ds = tuser.Ds, Connected = true, UserImgPath = tuser.UserImagePath, ReportingUserName = tuser.ReportingUserName, SignInFrom = "Trim Auth"};
            //    user.Userstate = appUserState;

            //    return Task.FromResult(true);
            //}
            //return Task.FromResult(false);
            return Task.FromResult(true);
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var manager = new ApplicationUserManager(new ElasticUserStore<inforClaimUser>(new Uri("http://localhost:9200/")));//new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<inforClaimUser>(manager) {AllowOnlyAlphanumericUserNames = false, RequireUniqueEmail = true};

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator {RequiredLength = 6, RequireNonLetterOrDigit = true, RequireDigit = true, RequireLowercase = true, RequireUppercase = true};

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
            manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<inforClaimUser> {MessageFormat = "Your security code is {0}"});
            manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<inforClaimUser> {Subject = "Security Code", BodyFormat = "Your security code is {0}"});
            manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
                manager.UserTokenProvider = new DataProtectorTokenProvider<inforClaimUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            return manager;
        }
    }

    // Configure the application sign-in manager which is used in this application.
    public class ApplicationSignInManager : SignInManager<inforClaimUser, string>
    {
        private static readonly string Wgs = SettingsManager.GetSettingValueAsString("WGS");  
        private static readonly string Ds = SettingsManager.GetSettingValueAsString("DS");  
        private static readonly string Path = SettingsManager.GetSettingValueAsString("UserImagePath");  
        private static readonly string Tu = SettingsManager.GetSettingValueAsString("TU"); 
        private static readonly string Tup = SettingsManager.GetSettingValueAsString("TUP");  

        private AppUserState _appUserState;

        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager) : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(inforClaimUser user)
        {
          return user.GenerateUserIdentityAsync((ApplicationUserManager) UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }

        public  async Task SignInAsync(inforClaimUser user, bool isPersistent, bool rememberBrowser)
        {
            var userIdentity = await CreateUserIdentityAsync(user).WithCurrentCulture();

            // Clear any partial cookies from external or two factor partial sign ins
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie, DefaultAuthenticationTypes.TwoFactorCookie);
            if (rememberBrowser)
            {
                var rememberBrowserIdentity = AuthenticationManager.CreateTwoFactorRememberBrowserIdentity(ConvertIdToString(user.Id));
                AuthenticationManager.SignIn(new AuthenticationProperties {IsPersistent = isPersistent}, userIdentity, rememberBrowserIdentity);
            }
            else
            {
                AuthenticationManager.SignIn(new AuthenticationProperties {IsPersistent = isPersistent}, userIdentity);
            }
        }

        public async Task<SignInStatus> ExternalSignIn(ExternalLoginInfo loginInfo, bool isPersistent)
        {
            var conn = new InforConnection(tu: Tu, tup: Tup);
            var un = loginInfo.DefaultUserName;
            var formatedUn = un;
            var domain = "";
            if (un == @"DESKTOP-K0ELIF5\grant")
                un = "grant.scanes@informotion.com.au";
            if (un.IndexOf("@", StringComparison.Ordinal) > 0)
            {
                formatedUn = un.Split('@')[0];
                domain = un.Split('@')[1].Split('.')[0];
            }
            var challengeResult = conn.AuthorizeUser($"{formatedUn}@{domain}", Wgs, Ds, Path);
            if (challengeResult.Success)
            {
                var tuser = challengeResult.ReturnObject as InforUser;
                if (tuser != null)
                {
                    _appUserState = new AppUserState { Email = tuser.Email, Name = tuser.Name, UserId = tuser.UserId, UserName = tuser.UserName, Wgs = tuser.Wgs, Ds = tuser.Ds, Connected = true, UserImgPath = tuser.UserImagePath, ReportingUserName = tuser.ReportingUserName, SignInFrom = "Windows" };
                    var user = await UserManager.FindByNameAsync(loginInfo.DefaultUserName);
                    if (user == null)
                    {
                        var auser = new inforClaimUser() { UserName = loginInfo.DefaultUserName, Email = new ElasticUserEmail{Address = _appUserState.Email , IsConfirmed = false} };
                        var result = await UserManager.CreateAsync(auser);
                        if (result.Succeeded)
                        {
                            result = await UserManager.AddLoginAsync(auser.Id, loginInfo.Login);
                             
                        }
                        user = auser;
                         
                    }
                    //user.SetState(_appUserState.ToString());
                    await SignInAsync(user, false, false);
                     
                }
            }

            var res = await ExternalSignInAsync(loginInfo, isPersistent);
            
            return res;
        }

        private async Task<SignInStatus> SignInOrTwoFactor(inforClaimUser user, bool isPersistent)
        {
            var id = Convert.ToString(user.Id);
            if (UserManager.SupportsUserTwoFactor && await UserManager.GetTwoFactorEnabledAsync(user.Id).WithCurrentCulture() && (await UserManager.GetValidTwoFactorProvidersAsync(user.Id).WithCurrentCulture()).Count > 0 && !await AuthenticationManager.TwoFactorBrowserRememberedAsync(id).WithCurrentCulture())
            {
                var identity = new ClaimsIdentity(DefaultAuthenticationTypes.TwoFactorCookie);
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, id));
                AuthenticationManager.SignIn(identity);
                return SignInStatus.RequiresVerification;
            }
            await SignInAsync(user, isPersistent, false).WithCurrentCulture();
            return SignInStatus.Success;
        }

        public override async Task<SignInStatus> PasswordSignInAsync(string userName, string password, bool isPersistent, bool shouldLockout)
        {
            var conn = new InforConnection(tu: Tu, tup: Tup);
            InforActionResult challengeResult;
            if (UserManager == null)
                return SignInStatus.Failure;
            var user = await UserManager.FindByNameAsync(userName).WithCurrentCulture();
            if (user == null)
            {
                challengeResult = conn.AuthenticateUser(userName, password, Wgs, Ds, Path);
                if (challengeResult.Success)
                {
                    var tuser = challengeResult.ReturnObject as InforUser;
                    if (tuser != null)
                    {
                        _appUserState = new AppUserState {Email = tuser.Email, Name = tuser.Name, UserId = tuser.UserId, UserName = tuser.UserName, Wgs = tuser.Wgs, Ds = tuser.Ds, Connected = true, UserImgPath = tuser.UserImagePath, ReportingUserName = tuser.ReportingUserName, SignInFrom = "Trim Auth"};
                        user = await UserManager.FindByEmailAsync(tuser.Email);
                        if (user == null)
                        {
                            var auser = new inforClaimUser() { UserName = userName, Email = new ElasticUserEmail { Address = _appUserState.Email, IsConfirmed = false } };
                            var result = await UserManager.CreateAsync(auser);
                           user = auser;
                        }
                        //user.SetState(_appUserState.ToString());
                        var res = await SignInOrTwoFactor(user, isPersistent).WithCurrentCulture();
                         //await user.GenerateUserIdentityAsync((ApplicationUserManager) UserManager, _appUserState);
                        return res;
                    }
                }
                return SignInStatus.Failure;
            }
            if (UserManager.SupportsUserLockout && await UserManager.IsLockedOutAsync(user.Id).WithCurrentCulture())
                return SignInStatus.LockedOut;
            if (UserManager.SupportsUserPassword && await UserManager.CheckPasswordAsync(user, password).WithCurrentCulture())
            {
                challengeResult = conn.AuthenticateUser(userName, password, Wgs, Ds, Path);
                if (challengeResult.Success)
                {
                    var tuser = challengeResult.ReturnObject as InforUser;
                    if (tuser != null)
                    {
                        _appUserState = new AppUserState {Email = tuser.Email, Name = tuser.Name, UserId = tuser.UserId, UserName = tuser.UserName, Wgs = tuser.Wgs, Ds = tuser.Ds, Connected = true, UserImgPath = tuser.UserImagePath, ReportingUserName = tuser.ReportingUserName, SignInFrom = "Trim Auth"};
                        //user.SetState(_appUserState.ToString());
                        return await SignInOrTwoFactor(user, isPersistent).WithCurrentCulture();
                    }
                }
                return SignInStatus.Failure;
            }
            if (shouldLockout && UserManager.SupportsUserLockout)
            {
                // If lockout is requested, increment access failed count
                // which might lock out the user
                await UserManager.AccessFailedAsync(user.Id).WithCurrentCulture();
                if (await UserManager.IsLockedOutAsync(user.Id).WithCurrentCulture())
                    return SignInStatus.LockedOut;
            }
            return SignInStatus.Failure;
        }
    }
}