#region Using

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Web.Mvc;
using System.Web.Routing;
using inReachWebRebuild.Classes;
using inReachWebRebuild.Common;
using inReachWebRebuild.Models;
using inReachWebRebuild.ViewModels;
using Infor.Model;
using Infor.Model.ApplicationServices;
using Infor.Model.DocumentServices;
using Infor.Model.Enums;
using Newtonsoft.Json;
using NLog;
using RestSharp;

#endregion

namespace inReachWebRebuild.Controllers
{
    public class DocumentController : BaseController
    {
        private static string _tu = SettingsManager.GetSettingValueAsString("TU");
        private static string _tup = SettingsManager.GetSettingValueAsString("TUP");
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly FileViewModel _viewModel = new FileViewModel();
        private static readonly string PdPath = System.Web.HttpContext.Current.Server.MapPath($"~/App_Data/inReachRepos/Global/PD");

        public ActionResult DocumentView(long uri)
        {
            Logger.Info($"Document View");
            var file = DocManagerHelper.GetDocumentForView(uri, AppUserState.UserName, AppUserState.Wgs, AppUserState.Ds, AppUserState.UserId);
            var conn = new InforConnection(tu: _tu, tup: _tup);
            var rec = (InforRecord)conn.GetRecordWeb(uri, AppUserState.UserName, AppUserState.Wgs, AppUserState.Ds);
            var hvm = new FileViewModel
            {
                Edit = false,
                ErrorDisplay = ErrorDisplay,
                AppUserState = AppUserState,
                RecordNumber = rec.RecordNumber,
                Title = rec.Title,
                FileModel = new FileModel
                {
                    TypeDesktop = true,
                    FileName = file,
                    AppUserState = AppUserState,
                    RecordUri = rec.Uri
                },
                Uri = uri
            };
            Logger.Info($"{hvm.FileModel.FileUri}");
            Logger.Info($"{hvm.FileModel.Key}");
            Logger.Info($"{hvm.FileModel.DocumentType}");
            return PartialView("_DocumentView", hvm);
        }

        public ActionResult DocumentEdit(long uri)
        {
            var file = DocManagerHelper.GetDocumentForEdit(uri, AppUserState.UserName, AppUserState.Wgs, AppUserState.Ds, AppUserState.UserId);
            var conn = new InforConnection(tu: _tu, tup: _tup);
            var rec = (InforRecord)conn.GetRecordWeb(uri, AppUserState.UserName, AppUserState.Wgs, AppUserState.Ds);
            var hvm = new FileViewModel
            {
                Edit = true,
                ErrorDisplay = ErrorDisplay,
                AppUserState = AppUserState,
                RecordNumber = rec.RecordNumber,
                Title = rec.Title,
            FileModel = new FileModel
                {
                    TypeDesktop = true,
                    FileName = file,
                    AppUserState = AppUserState,
                    RecordUri = rec.Uri
            },
                Uri = uri
            };
            return PartialView("_DocumentView", hvm);
        }

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
            _tu = SettingsManager.GetSettingValueAsString("TU");
            _tup = SettingsManager.GetSettingValueAsString("TUP");
            _viewModel.ErrorDisplay = ErrorDisplay;
            _viewModel.AppUserState = AppUserState;
            ViewData["UserState"] = AppUserState;
        }

        public void CheckinDocument(long uri)
        {
            Logger.Info($"File Checked in  {uri}");
            DocManagerHelper.CheckinDocument(uri, AppUserState.UserName, AppUserState.Wgs, AppUserState.Ds);
            var filename = InforConnection.GetFileNameSansExtension(Convert.ToInt64(uri), AppUserState.UserName, AppUserState.Wgs, AppUserState.Ds);

            if (System.IO.File.Exists(Path.Combine(PdPath, filename)))
                System.IO.File.Delete(Path.Combine(PdPath, filename));
        }

        public void CancelDocument(long uri)
        {
            Logger.Info($"File Edit Canceled in  {uri}");
            DocManagerHelper.CancelDocument(uri, AppUserState.UserName, AppUserState.Wgs, AppUserState.Ds);
        }

        public void ViewDocumentWebSessionEnd(long uri)
        {
            DocManagerHelper.ViewDocumentWebSessionEnd(uri, AppUserState.UserName, AppUserState.Wgs, AppUserState.Ds);
        }

        public PartialViewResult InforPropsForRecordForPartial(long uri, InforProp parent)
        {
            var conn = new InforConnection(tu: _tu, tup: _tup);
            var rec = conn.GetRecordWeb(uri, AppUserState.UserName, AppUserState.Wgs, AppUserState.Ds);
            if (AppUserState == null || AppUserState.Connected == false) return PartialView(@"~/views/Reports/Properties.cshtml", new InforPropsViewModel());
              
            var hvm = new InforPropsViewModel { Properties = (parent == null ?  conn.GetPropertiesWeb(AppUserState.UserName, AppUserState.Wgs, AppUserState.Ds,InforObjectype.Record, null,rec, true) : 
                conn.GetPropertiesWeb(AppUserState.UserName, AppUserState.Wgs, AppUserState.Ds,parent.Type, parent, rec, true)) as InforProps,IncludeAddButton = true};
            return PartialView(@"~/views/Reports/Properties.cshtml", hvm);
        }

        public JsonResult GetInforPropsForParentForRecord(long uri, InforProp parent)
        {
            if (AppUserState == null || AppUserState.Connected == false) return null;
            var conn = new InforConnection(tu: _tu, tup: _tup);
            var rec = conn.GetRecordWeb(uri, AppUserState.UserName, AppUserState.Wgs, AppUserState.Ds);
            var props = parent == null ? conn.GetPropertiesWeb(AppUserState.UserName, AppUserState.Wgs, AppUserState.Ds, InforObjectype.Record, null, rec, true) :
                conn.GetPropertiesWeb(AppUserState.UserName, AppUserState.Wgs, AppUserState.Ds, parent.Type, parent, rec, true);
            return this.Jsonp(props);
        }


        public void DocServerCommand(string cc, string keyc, string userdatac)
        {
            try
            {
               

                if (cc == "forcesave")
                {
                    var recUri = keyc.Split(new[] { "__" }, StringSplitOptions.None)[0];
                    var filename = InforConnection.GetFileNameSansExtension(Convert.ToInt64(recUri), AppUserState.UserName, AppUserState.Wgs, AppUserState.Ds);
                    if (!System.IO.File.Exists(Path.Combine(PdPath, filename)))
                    {
                        var f = System.IO.File.Create(Path.Combine(PdPath, filename));
                        f.Close();
                    }

                    Thread.Sleep(5000);
                    
                    var client = new RestClient(SettingsManager.GetSettingValueAsString("files.docservice.url.commandService"));
                    var request = new RestRequest(Method.POST) { RequestFormat = DataFormat.Json };
                    request.AddBody(new { c = cc, key = keyc, userdata = userdatac });
                    request.AddHeader("Content-Type", "application/json");
                    request.AddHeader("Cache-Control", "no-cache");
                    var responsestring = client.Execute(request).Content;
                    //check errors
                   
                    var response = JsonConvert.DeserializeObject<CommandServiceResponse>(responsestring);
                  
                    var validResponses = new List<long> {  4 };
                    if (validResponses.Contains(response.error))
                    {
                        if (!userdatac.Contains("checkin")){
                            if (System.IO.File.Exists(Path.Combine(PdPath, filename)))
                                System.IO.File.Delete(Path.Combine(PdPath, filename));
                        }
                       
                    }

                    if (!validResponses.Contains(response.error)) return;

                   

                    if (!userdatac.Contains("checkin")) return;
                    
                    var docUri = userdatac.Split(new[] { "||" }, StringSplitOptions.None)[1];
                   
                    CheckinDocument(Convert.ToInt64(docUri));
                    DropUser(keyc,"");
                    
                }

                if (cc != "drop") return;
                {
                    DropUser(keyc, userdatac);
                    if (!userdatac.Contains("cancel")) return;
                    CancelDocument(Convert.ToInt64(userdatac.Split(new[] { "||" }, StringSplitOptions.None)[1]));
                }
            }
            catch (Exception e)
            {
               Logger.Info($"Error {e.Message}");
            }
            
        }

        public  void DropUser( string keyc, string userdatac)
        {
            Logger.Info($"Calling Drop User");
          //  var client = new RestClient(SettingsManager.GetSettingValueAsString("files.docservice.url.commandService"));
          //  var dropRequest = new RestRequest(Method.POST) { RequestFormat = DataFormat.Json };
          //  var body = new {c = "drop", key = keyc, userdata = userdatac, user = new[] {AppUserState.UserId.ToString()}};
          //  dropRequest.AddBody(body);
          //  dropRequest.AddHeader("Content-Type", "application/json");
          //  dropRequest.AddHeader("Cache-Control", "no-cache");
          //var responsestring = client.Execute(dropRequest).Content;
          //  //check errors
          //  Logger.Info($"Response {responsestring}");
          //  var response = JsonConvert.DeserializeObject<CommandServiceResponse>(responsestring);
        }

        public void DocumentStatus(string keyc, string userdatac)
        {
            Logger.Info($"Calling Document Status for {keyc}");
            var client = new RestClient(SettingsManager.GetSettingValueAsString("files.docservice.url.commandService"));
            var dropRequest = new RestRequest(Method.POST) { RequestFormat = DataFormat.Json };
            dropRequest.AddBody(new { c = "info", key = keyc, userdata = $"{userdatac}" });
            dropRequest.AddHeader("Content-Type", "application/json");
            dropRequest.AddHeader("Cache-Control", "no-cache");
            client.Execute(dropRequest); 
        }

        public void CheckinDocument(long uri, string wgs, string ds, string userName)
        {
            Logger.Info($"File Checked in  {uri}");
            DocManagerHelper.CheckinDocument(uri, userName, wgs, ds);
        }

    }

    public class CommandServiceResponse
    {
        public string key { get; set; }
        public long error { get; set; }
    }
}