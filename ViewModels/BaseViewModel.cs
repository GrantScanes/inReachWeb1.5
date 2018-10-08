using System.Web;
using inReachWebRebuild.Classes;
using inReachWebRebuild.Models;

namespace inReachWebRebuild.ViewModels
{
    public class BaseViewModel
    {
        public ErrorDisplay ErrorDisplay = null;
        public AppUserState AppUserState = null;
        public string BaseUrl = HttpContext.Current.Request.ApplicationPath;
        public string PageTitle = null;

        public PagingDetails Paging = null;
    }
}