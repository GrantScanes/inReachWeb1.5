#region Using

using System.Web.Mvc;
using System.Web.Routing;
using inReachWebRebuild.Classes;
using inReachWebRebuild.Common;
using inReachWebRebuild.ViewModels;
using Infor.Model;
using Infor.Model.ApplicationServices;
using Infor.Model.WebServices.ActionServices;

#endregion

namespace inReachWebRebuild.Controllers
{
    public class ActionsController : BaseController
    {
        private static string _tu = SettingsManager.GetSettingValueAsString("TU");
        private static string _tup = SettingsManager.GetSettingValueAsString("TUP");
        private readonly ActionViewModel _actionViewModel = new ActionViewModel();

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
            _tu = SettingsManager.GetSettingValueAsString("TU");
            _tup = SettingsManager.GetSettingValueAsString("TUP");
            _actionViewModel.ErrorDisplay = ErrorDisplay;
            _actionViewModel.AppUserState = AppUserState;
            ViewData["UserState"] = AppUserState;
        }

        public PartialViewResult GetAction(int uri)
        {
            if (AppUserState == null || AppUserState.Connected == false) return null;
            var action = ActionService.GetAction(AppUserState.Wgs, AppUserState.Ds, AppUserState.UserName, uri);
            _actionViewModel.Action = action;
            _actionViewModel.RecordNumber = action.RecorNumber;
            _actionViewModel.RecordUri = action.RecordUri;
            _actionViewModel.Title = action.Name;
            return PartialView("_ActionsActions", _actionViewModel);
        }

        public PartialViewResult GetActionDetails(int uri, int recUri)
        {
            var conn = new InforConnection(tu: _tu, tup: _tup);
            if (AppUserState == null || AppUserState.Connected == false) return null;
            if (uri > 0)
            {
                var action = ActionService.GetAction(AppUserState.Wgs, AppUserState.Ds, AppUserState.UserName, uri);
                _actionViewModel.Action = action;
                _actionViewModel.RecordNumber = action.RecorNumber;
                _actionViewModel.RecordUri = action.RecordUri;
                _actionViewModel.Title = action.Name;
            }
            else
            {
                var rec = conn.GetRecordWeb(recUri, AppUserState.UserName, AppUserState.Wgs, AppUserState.Ds);
                _actionViewModel.RecordNumber = rec.RecordNumber;
                _actionViewModel.RecordUri = rec.Uri;
                _actionViewModel.Title = rec.Title;
            }
            return PartialView("_ActionDetails", _actionViewModel);
        }

        public JsonResult GetActionTypes()
        {
            var actionTypes = (InforActionDefs) ActionService.GetAttachableActions(AppUserState.Wgs, AppUserState.Ds, AppUserState.UserName).ReturnObject;
            return this.Jsonp(actionTypes);
        }

        public JsonResult AddAction(long recordUri, long actionDefUri, long responsibleLocation)
        {
            var result = ActionService.AttachAction(AppUserState.Wgs, AppUserState.Ds, AppUserState.UserName, recordUri, actionDefUri, responsibleLocation);
            return this.Jsonp(result);
        }

        public JsonResult UpdateAction(long actionUri, long responsibleLocation, int days, int hours, int minutes)
        {
            var result = ActionService.UpdateAction(AppUserState.Wgs, AppUserState.Ds, AppUserState.UserName, actionUri, responsibleLocation, days, hours, minutes);
            return this.Jsonp(result);
        }
    }
}