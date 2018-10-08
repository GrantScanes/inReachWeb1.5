using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using ElasticIdentity;
using Microsoft.AspNet.Identity;

namespace inReachWebRebuild.Models
{

    public class inforClaimUser : ElasticUser
    {
        public string Userstate { get; set; }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(ApplicationUserManager manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            userIdentity.AddClaims(new List<Claim>()
                {
                    new Claim("userState", Userstate.ToString())
                });
            return userIdentity;
        }
    }

    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    //public class ApplicationUser : IdentityUser
    //{
    //    //public string Wgs { get; set; }
    //    //public string Ds { get; set; }
    //    //public string ImagePath { get; set; }
    //    //public string Userstate { get; set; }

    //    private static readonly string wgs = ConfigurationManager.AppSettings["WGS"];
    //    private static readonly string ds = ConfigurationManager.AppSettings["DS"];
    //    private static readonly string path = ConfigurationManager.AppSettings["UserImagePath"];
    //    private string _userState = string.Empty;

    //    public void SetState(string value)
    //    {
    //        _userState = value;
    //    }

    //    //public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, AppUserState userstate)
    //    //{
    //    //    // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
    //    //    var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);

    //    //   //var claims = userIdentity.Claims;
    //    //    //var inds = new List<int>();
    //    //    //foreach (var claim in claims)
    //    //    //{

    //    //    //    var res = await manager.RemoveClaimAsync(Id, claim);
    //    //    //    if (res != IdentityResult.Success)
    //    //    //    {
    //    //    //        var wtf = res.Errors;
    //    //    //    }
    //    //    //}
    //    //    //manager.RemoveClaim(Id, new Claim(ClaimTypes.NameIdentifier, userstate.UserId.ToString()));
    //    //    //manager.RemoveClaim(Id, new Claim(ClaimTypes.Name, userstate.Name));
    //    //    //manager.RemoveClaim(Id, new Claim("userState", userstate.ToString()));

    //    //    //manager.AddClaim(Id, new Claim(ClaimTypes.NameIdentifier, userstate.UserId.ToString()));
    //    //    //manager.AddClaim(Id, new Claim(ClaimTypes.Name, userstate.Name));

    //    //    //manager.AddClaim(Id, new Claim("userState", userstate.ToString()));
    //    //    //userIdentity.AddClaim(new Claim("userState", userstate.ToString()));
    //    //    userIdentity.AddClaims(new List<Claim>()
    //    //    {
    //    //        new Claim("userState", _userState.ToString())
    //    //    });
    //    //    return userIdentity;
    //    //}

    //    public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
    //    {
    //        // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
    //        var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
    //        userIdentity.AddClaims(new List<Claim>()
    //        {
    //            new Claim("userState", _userState.ToString())
    //        });
    //        return userIdentity;
    //    }
    //}

    //public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    //{
    //    public ApplicationDbContext()
    //        : base("DefaultConnection", throwIfV1Schema: false)
    //    {
    //    }

    //    public static ApplicationDbContext Create()
    //    {
    //        return new ApplicationDbContext();
    //    }
    //}
}