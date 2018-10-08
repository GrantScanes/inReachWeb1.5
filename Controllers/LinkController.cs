using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Http;
using inReachWebRebuild.Classes;
using inReachWebRebuild.Common;
using inReachWebRebuild.Models;

namespace inReachWebRebuild.Controllers
{
    
    public class LinkController : BaseController
    {
        //private readonly string _oosurl = "https://onenote.officeapps-df.live.com/hosting/discovery";
        private readonly string _oosurl = "http://infowgs-oos9868.cloudapp.net/hosting/discovery";

        /// <summary>
        ///     Provides a link that can be used to Open a document in the relative viewer
        ///     from the Office Web Apps server
        /// </summary>
        /// <param name="fileRequest">indicates the request type</param>
        /// <returns>A link usable for HREF</returns>
        [Route("Link/GetLink/{id}")]
        public async Task<Link> GetLink([FromUri] FileRequest fileRequest)
        {
            if (ModelState.IsValid)
            {
                var wopiServer = WebConfigurationManager.AppSettings["appWopiServer"];
                var updateEnabled = false;
                bool.TryParse(WebConfigurationManager.AppSettings["updateEnabled"], out updateEnabled);
                var wopiHelper = new WopiAppHelper(await Getxml(), updateEnabled);

                var result = wopiHelper.GetDocumentLink(wopiServer, fileRequest.name, AppUserState.UserName,
                    AppUserState.Wgs, AppUserState.Ds);

                var rv = new Link
                {
                    Url = result
                };
                return rv;
            }

            throw new ApplicationException("Invalid ModelState");
        }

        private async Task<string> Getxml()
        {
            var client = new HttpClient();
            using (
                var response =
                    await client.GetAsync(_oosurl))
            {
                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadAsStringAsync();
            }
            return string.Empty;
        }
    }
}
