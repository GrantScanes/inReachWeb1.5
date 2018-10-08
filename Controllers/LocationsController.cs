using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;

using inReachWebRebuild.Classes;
using Infor.Model;
using Infor.Model.WebServices.LocationServices;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Newtonsoft.Json;

namespace inReachWebRebuild.Controllers
{
    /*
    The WebApiConfig class may require additional changes to add a route for this controller. Merge these statements into the Register method of the WebApiConfig class as applicable. Note that OData URLs are case sensitive.

    using System.Web.Http.OData.Builder;
    using System.Web.Http.OData.Extensions;
    using Infor.Model;
    ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
    builder.EntitySet<InforLocations>("Locations");
    config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class LocationsController : BaseODataController
    {
        //[EnableQuery(PageSize = 10, AllowedQueryOptions = AllowedQueryOptions.All)]
        //public IHttpActionResult Get()
        //{
        //    var path = System.Web.HttpContext.Current.Server.MapPath("~/Content/Images/UserImages/");
        //    if (DataSources.Instance.Locations.Any()) return Ok(DataSources.Instance.Locations.AsQueryable());
        //    var locs = (InforLocations)LocationService.GetLocations(AppUserState.Wgs, AppUserState.Ds, AppUserState.UserName,
        //        LocationSearchType.ActiveValid, 1, "", path).ReturnObject;
        //    foreach (var loc in locs)
        //    {
        //        DataSources.Instance.Locations.Add(new Location { UserId = loc.UserId, Name = loc.Name });
        //    }


        //    var res = _peopleService.GetAllAsQueryable();
        //    return Ok(res);
        //}

        [EnableQuery(AllowedQueryOptions = AllowedQueryOptions.All, EnsureStableOrdering = false)]
        // GET: odata/Locations
        public IHttpActionResult GetLocations(ODataQueryOptions<RLocation> queryOptions, long currentUri)
        {
            var cachePath = HttpContext.Current.Server.MapPath($"~/App_Data/inReachRepos/Global/Cache");
            if (!File.Exists($"{cachePath}/accloc")) return StatusCode(HttpStatusCode.NotImplemented);
            var locCache = JsonConvert.DeserializeObject<LocationCache>(File.ReadAllText($"{cachePath}/accloc"));
            if (currentUri > 0)
                if (locCache.Locations.All(id => id.UserId != currentUri))
                {
                    var addloc = (InforLocation) LocationService
                        .GetSpecificLocation(AppUserState.Wgs, AppUserState.Ds, AppUserState.UserName, currentUri)
                        .ReturnObject;
                    locCache.Locations.Add(new RLocation
                    {
                        Name = addloc.Name,

                        UserId = addloc.UserId
                    });
                }


            var locs = locCache.Locations;
            return Ok(locs);
        }


        // GET: odata/Locations(5)
        public IHttpActionResult GetInforLocations([FromODataUri] Guid key, ODataQueryOptions<RLocation> queryOptions)
        {
            // validate the query.

            // return Ok<InforLocations>(inforLocations);
            return StatusCode(HttpStatusCode.NotImplemented);
        }
    }
}