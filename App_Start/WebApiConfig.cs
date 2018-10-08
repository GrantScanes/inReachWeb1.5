using System.Web.Http;
using Infor.Model;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.OData.Edm;
using Microsoft.Owin.Security.OAuth;

namespace inReachWebRebuild
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
           // var cors = new EnableCorsAttribute(
           //origins: "*",
           //headers: "*",
           //methods: "*");
           // config.EnableCors(cors);
            // Web API configuration and services
            // Configure Web API to use only bearer token authentication.
            //config.SuppressDefaultHostAuthentication();
           

            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

            // Web API routes
            config.MapHttpAttributeRoutes();

            ODataModelBuilder builder = new ODataConventionModelBuilder();
            builder.EntitySet<RLocation>("Locations");
            config.MapODataServiceRoute(
                routeName: "Locations",
                routePrefix: "odata",
                model: builder.GetEdmModel());
            config.Select().Expand().Filter().OrderBy().MaxTop(null).Count();
            //config.Routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);
            // config.Routes.MapHttpRoute(
            //    "Records", // Route name
            //    "{controller}/{action}/{uri}", // URL with parameters
            //    new { controller = "Records", action = "Get" } // Parameter defaults
            //);

            // config.Routes.MapHttpRoute(
            //    "DefaultAPI", // Route name
            //    "api/{controller}/{uri}", // URL with parameters
            //    new { controller = "Home", action = "Index", id = RouteParameter.Optional } // Parameter defaults
            //);

            // config.Routes.MapHttpRoute(
            //name: "Records",
            //routeTemplate: "records",
            //defaults: new { controller = "Records", action = "Get" }
            //);
            //  config.Routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);
        }
        private static IEdmModel GetEdmModel()
        {
            var builder = new ODataConventionModelBuilder
            {
                Namespace = "inReach",
                ContainerName = "inReach"
            };
            builder.EntitySet<RLocation>("Location"); 
            var edmModel = builder.GetEdmModel();
            return edmModel;
        }
    }
}
