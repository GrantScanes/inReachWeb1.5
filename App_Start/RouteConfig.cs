using System.Web.Mvc;
using System.Web.Routing;

namespace inReachWebRebuild
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapMvcAttributeRoutes();
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "HomeM", action = "Index", id = UrlParameter.Optional }
            );
          

            // routes.MapRoute(
            //    name: "DefaultApi",
            //    url: "{controller}/{action}/{id}",
            //    defaults: new { controller = "Link", id = UrlParameter.Optional }
            //);


        }
    }
}
