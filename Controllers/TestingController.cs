using System.Web.Mvc;
using Infor.Model.EmailServices;

namespace inReachWebRebuild.Controllers
{
    public class TestingController : Controller
    {
        // GET: Testing
        public ActionResult Index()
        {
            return View();
        }

        public void SendTestMail()
        {
            EmailServices.SendMail("grant.scanes@informotion.com.au", "test", "test");

        }
    }
}