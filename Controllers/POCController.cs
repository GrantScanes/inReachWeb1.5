using System;
using System.Web.Mvc;
using inReachWebRebuild.Classes;
using inReachWebRebuild.Models;

namespace inReachWebRebuild.Controllers
{
    public class PocController : BaseController
    {
        // GET: POC  [AllowCrossSiteJson]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [System.Web.Mvc.AllowAnonymous]
        public ActionResult Index()
        {
            var d = "27/03/2017";
            var cStart = DateTime.Parse(d.ToString());
            return View();
        }

        public ActionResult Editor(string fileName, string mode)
        {
            mode = mode ?? string.Empty;

            var file = new FileModel
            {
                TypeDesktop = mode != "embedded",
                FileName = fileName
            };

            return View("Editor", file);
        }

        //public ActionResult Sample(string fileExt)
        //{
        //    var fileName = DocManagerHelper.CreateDemo(fileExt);
        //    Response.Redirect(Url.Action("Editor", "Poc", new { fileName = fileName }));
        //    return null;
        //}
    }
}