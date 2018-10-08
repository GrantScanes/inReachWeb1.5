using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.ModelBinding;
using System.Web.Mvc;
using inReachWebAPI.Classes;
using inReachWebAPI.Common;
using inReachWebAPI.Models;
using inReachWebAPI.ViewModels;

using Microsoft.Owin.Security;
using NLog;
using Westwind.Utilities;
using System.Xml.Linq;
using Infor.Model;
using Infor.Model.ApplicationServices;

namespace inReachWebAPI.Controllers
{
    public class HomeController : BaseController
    {        private readonly HomeViewModel _viewModel = new HomeViewModel();
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public InforUser IUser = null;
        //private readonly string _oosurl = "https://onenote.officeapps-df.live.com/hosting/discovery";
        private const string Oosurl = "http://infowgs-oos9868.cloudapp.net/hosting/discovery";
        private static string _tu = SettingsManager.GetSettingValueAsString("TU");  
        private static string _tup = SettingsManager.GetSettingValueAsString("TUP");  

        [AllowCrossSiteJson]
       [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
       [System.Web.Mvc.AllowAnonymous]
        public async Task<ActionResult> Index()
        {


  

            //ConvertToHtml(@"C:\Users\grant\Documents\Informotion Investment Term Sheet.docx", @"C:\Users\grant\Documents");
            return View();
        }

        public FileStreamResult GetPdf()
        {


            var wordApp = new Microsoft.Office.Interop.Word.Application();
            var wordDocument = wordApp.Documents.Open(@"C:\Users\grant\Documents\Informotion Investment Term Sheet.docx");

            wordDocument.ExportAsFixedFormat(@"C:\Users\grant\Documents\test.PDF", Microsoft.Office.Interop.Word.WdExportFormat.wdExportFormatPDF);

            wordDocument.Close(Microsoft.Office.Interop.Word.WdSaveOptions.wdDoNotSaveChanges,
                               Microsoft.Office.Interop.Word.WdOriginalFormat.wdOriginalDocumentFormat,
                               false); //Close document

            wordApp.Quit(); //

            //ConvertToHtml(@"C:\Users\grant\Documents\Informotion Investment Term Sheet.docx", @"C:\Users\grant\Documents");
            var fs = new FileStream(@"C:\Users\grant\Documents\test.PDF", FileMode.Open, FileAccess.Read);
            return File(fs, "application/pdf");
        }



        //public static void ConvertToHtml(string file, string outputDirectory)
        //{
        //    var fi = new FileInfo(file);
        //    Console.WriteLine(fi.Name);
        //    var byteArray = System.IO.File.ReadAllBytes(fi.FullName);
        //    using (var memoryStream = new MemoryStream())
        //    {
        //        memoryStream.Write(byteArray, 0, byteArray.Length);
        //        using (var wDoc = WordprocessingDocument.Open(memoryStream, true))
        //        {
        //            var destFileName = new FileInfo(fi.Name.Replace(".docx", ".html"));
        //            if (!string.IsNullOrEmpty(outputDirectory))
        //            {
        //                var di = new DirectoryInfo(outputDirectory);
        //                if (!di.Exists)
        //                {
        //                    throw new OpenXmlPowerToolsException("Output directory does not exist");
        //                }
        //                destFileName = new FileInfo(Path.Combine(di.FullName, destFileName.Name));
        //            }
        //            var imageDirectoryName = destFileName.FullName.Substring(0, destFileName.FullName.Length - 5) + "_files";
        //            var imageCounter = 0;

        //            var pageTitle = fi.FullName;
        //            var part = wDoc.CoreFilePropertiesPart;
        //            if (part != null)
        //            {
        //                pageTitle = (string)part.GetXDocument().Descendants(DC.title).FirstOrDefault() ?? fi.FullName;
        //            }

        //            // TODO: Determine max-width from size of content area.
        //            var settings = new HtmlConverterSettings()
        //            {
                        
        //                AdditionalCss = "body { margin: 1cm auto; max-width: 20cm; padding: 0; }",
        //                PageTitle = pageTitle,
        //                FabricateCssClasses = true,
        //                CssClassPrefix = "pt-",
        //                RestrictToSupportedLanguages = false,
        //                RestrictToSupportedNumberingFormats = false,
        //                ImageHandler = imageInfo =>
        //                {
        //                    var localDirInfo = new DirectoryInfo(imageDirectoryName);
        //                    if (!localDirInfo.Exists)
        //                        localDirInfo.Create();
        //                    ++imageCounter;
        //                    var extension = imageInfo.ContentType.Split('/')[1].ToLower();
        //                    ImageFormat imageFormat = null;
        //                    switch (extension)
        //                    {
        //                        case "png":
        //                            imageFormat = ImageFormat.Png;
        //                            break;
        //                        case "gif":
        //                            imageFormat = ImageFormat.Gif;
        //                            break;
        //                        case "bmp":
        //                            imageFormat = ImageFormat.Bmp;
        //                            break;
        //                        case "jpeg":
        //                            imageFormat = ImageFormat.Jpeg;
        //                            break;
        //                        case "tiff":
        //                            // Convert tiff to gif.
        //                            extension = "gif";
        //                            imageFormat = ImageFormat.Gif;
        //                            break;
        //                        case "x-wmf":
        //                            extension = "wmf";
        //                            imageFormat = ImageFormat.Wmf;
        //                            break;
        //                    }

        //                    // If the image format isn't one that we expect, ignore it,
        //                    // and don't return markup for the link.
        //                    if (imageFormat == null)
        //                        return null;

        //                    var imageFileName = imageDirectoryName + "/image" +
        //                        imageCounter.ToString() + "." + extension;
        //                    try
        //                    {
        //                        imageInfo.Bitmap.Save(imageFileName, imageFormat);
        //                    }
        //                    catch (System.Runtime.InteropServices.ExternalException)
        //                    {
        //                        return null;
        //                    }
        //                    var imageSource = localDirInfo.Name + "/image" +
        //                        imageCounter.ToString() + "." + extension;

        //                    var img = new XElement(Xhtml.img,
        //                        new XAttribute(NoNamespace.src, imageSource),
        //                        imageInfo.ImgStyleAttribute,
        //                        imageInfo.AltText != null ?
        //                            new XAttribute(NoNamespace.alt, imageInfo.AltText) : null);
        //                    return img;
        //                }
        //            };
        //            var htmlElement = HtmlConverter.ConvertToHtml(wDoc, settings);

        //            // Produce HTML document with <!DOCTYPE html > declaration to tell the browser
        //            // we are using HTML5.
        //            var html = new XDocument(
        //                new XDocumentType("html", null, null, null),
        //                htmlElement);

        //            // Note: the xhtml returned by ConvertToHtmlTransform contains objects of type
        //            // XEntity.  PtOpenXmlUtil.cs define the XEntity class.  See
        //            // http://blogs.msdn.com/ericwhite/archive/2010/01/21/writing-entity-references-using-linq-to-xml.aspx
        //            // for detailed explanation.
        //            //
        //            // If you further transform the XML tree returned by ConvertToHtmlTransform, you
        //            // must do it correctly, or entities will not be serialized properly.

        //            var htmlString = html.ToString(SaveOptions.DisableFormatting);
        //            System.IO.File.WriteAllText(destFileName.FullName, htmlString, Encoding.UTF8);
        //        }
        //    }
        //}

        public JsonResult RefreshRecord([FromUri] RecordRequest recordRequest)
        {
            var conn = new InforConnection(tu: _tu, tup: _tup);
            var id = recordRequest.Id;
            return id != null ? this.Jsonp(conn.GetRecordWeb((long) id, AppUserState.UserName, AppUserState.Wgs, AppUserState.Ds)) : null;
        }


        public FileResult DownloadView([FromUri] RecordRequest recordRequest)
        {
            var conn = new InforConnection(tu: _tu, tup: _tup);
            var id = recordRequest.Id;

            var ip = Request.UserHostName;
            var compName = ip;//Helpers.DetermineCompName(ip);

            var fileName = conn.DownloadRecordDocumentWeb((long)id, false, AppUserState.UserName,
                    AppUserState.Wgs, AppUserState.Ds, compName).ToLower();
            var rec = conn.GetRecordWeb((long) id, AppUserState.UserName,
                AppUserState.Wgs, AppUserState.Ds);
            return id != null ? File(fileName, MimeMapping.GetMimeMapping(fileName), rec.SuggestedFileName) : null;
        }

        [System.Web.Mvc.HttpPost]
        public void Upload( long uri)
        {
            for (var i = 0; i < Request.Files.Count; i++)
            {
                var file = Request.Files[i];

                var fileName = Path.GetFileName(file.FileName);
                var path = Path.Combine(@"C:\\inetpub\\wwwroot\\wopi\\files\\Checkin", fileName);
                file.SaveAs(path);

                var conn = new InforConnection(tu: _tu, tup: _tup);
                conn.CheckinDocumentWeb(uri, AppUserState.UserName,
                    AppUserState.Wgs, AppUserState.Ds, path);
            }


          

        }

        public FileResult DownloadEdit([FromUri] RecordRequest recordRequest)
        {
            var conn = new InforConnection(tu: _tu, tup: _tup);
            var id = recordRequest.Id;

            var ip = Request.UserHostName;
            //var compName = Classes.Helpers.DetermineCompName(ip);

            var fileName = conn.DownloadRecordDocumentWeb((long)id, true, AppUserState.UserName,
                    AppUserState.Wgs, AppUserState.Ds, ip).ToLower();
            var rec = conn.GetRecordWeb((long)id, AppUserState.UserName,
                AppUserState.Wgs, AppUserState.Ds);
            return id != null ? File(fileName, MimeMapping.GetMimeMapping(fileName), rec.SuggestedFileName) : null;
        }

        public string GetLinkEditorEdit([FromUri] RecordRequest recordRequest)
        {
            var conn = new InforConnection(tu: _tu, tup: _tup);
            var id = recordRequest.Id;

            var ip = Request.UserHostName;
            var compName = Classes.Helpers.DetermineCompName(ip);

            var fileName = conn.GetRecordDocumentWeb((long)id, true, AppUserState.UserName,
                    AppUserState.Wgs, AppUserState.Ds).ToLower();
            return fileName;
        }

        public async Task<string> GetLinkEdit([FromUri] FileRequest fileRequest)
        {
            if (ModelState.IsValid)
            {
                var wopiServer = WebConfigurationManager.AppSettings["appWopiServer"];
                var updateEnabled = true;
                var wopiHelper = new WopiAppHelper(await Getxml(), true);

                var result = wopiHelper.GetDocumentLink(wopiServer, fileRequest.name, AppUserState.UserName,
                    AppUserState.Wgs, AppUserState.Ds);

                var rv = new Link
                {
                    Url = result
                };
               return result;
               // return "\\asppages\\Editor.aspx";
            }

            throw new ApplicationException("Invalid ModelState");
        }

 

     

        public async Task<string> GetLinkView([FromUri] FileRequest fileRequest)
        {
            Logger.Info($"Get File Info called for file name {fileRequest.name}");
            if (ModelState.IsValid)
            {
                var wopiServer = WebConfigurationManager.AppSettings["appWopiServer"];
                Logger.Info($"WOPI server is  {wopiServer}");
                var updateEnabled = false;
                var wopiHelper = new WopiAppHelper(await Getxml(), false);

                var result = wopiHelper.GetDocumentLink(wopiServer, fileRequest.name, AppUserState.UserName,
                    AppUserState.Wgs, AppUserState.Ds);
                Logger.Info($"Result is {wopiServer}");
                var rv = new Link
                {
                    Url = result
                };
                return result;
            }

            throw new ApplicationException("Invalid ModelState");
        }

        private async Task<string> Getxml()
        {
            var client = new HttpClient();
            using (
                var response =
                    await client.GetAsync(Oosurl))
            {
                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadAsStringAsync();
            }
            return string.Empty;
        }



        //[System.Web.Mvc.HttpGet]
        //public ActionResult LogOn()
        //{
        //    Logger.Info($"Logon");
        //    //if (!string.IsNullOrEmpty(message))
        //    //    _viewModel.ErrorDisplay.ShowError(message);

        //    return View("LogOn", _viewModel);
        //}

        //[System.Web.Mvc.HttpPost]
        //public ActionResult LogOn(string userName, string password, string wgs, string ds, bool? rememberMe, string returnUrl)
        //{
        //    Logger.Info($"Logon for {userName}");
        //    var conn = new InforConnection();

        //    var challengeResult = conn.AuthenticateUser(userName, password, wgs, ds);

        //    if (challengeResult.Success)
        //    {
        //        var user = challengeResult.ReturnObject as InforUser;
        //        if (user == null)
        //        {
        //            ErrorDisplay.ShowError(challengeResult.Faults[0].Message);
        //            return View(_viewModel);
        //        }

        //        var appUserState = new AppUserState()
        //        {
        //            Email = user.Email,
        //            Name = user.Name,
        //            UserId = user.UserId,
        //            UserName = user.UserName,
        //            Wgs = user.Wgs,
        //            Ds = user.Ds,
        //            Connected = true
        //        };

        //        IdentitySignin(appUserState, user.UserId, rememberMe??false);
        //        Logger.Info($"identity set for {appUserState.UserName}");
        //        if (!string.IsNullOrEmpty(returnUrl))
        //            return Redirect(returnUrl);

        //        returnUrl = WebUtils.ResolveServerUrl("~/");
        //        return Redirect(returnUrl);
        //    }
        //    ErrorDisplay.ShowError(challengeResult.Faults[0].Message);
        //    return View(_viewModel);


        //}

        //public ActionResult LogOff()
        //{
        //    IdentitySignout();
        //    return RedirectToAction("Index");
        //}


        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            _tu = SettingsManager.GetSettingValueAsString("TU");
            _tup = SettingsManager.GetSettingValueAsString("TUP");
            IUser = new InforUser();
            _viewModel.User = IUser;
            _viewModel.ErrorDisplay = ErrorDisplay;
            _viewModel.AppUserState = AppUserState;

            ViewData["UserState"] = AppUserState;
        }

        protected override void Dispose(bool disposing)
        {

            IUser = null;

            base.Dispose(disposing);
        }



        #region SignIn and Signout

        /// <summary>
        /// Helper method that adds the Identity cookie to the request output
        /// headers. Assigns the userState to the claims for holding user
        /// data without having to reload the data from disk on each request.
        /// 
        /// AppUserState is read in as part of the baseController class.
        /// </summary>
        /// <param name="appUserState"></param>
        /// <param name="providerKey"></param>
        /// <param name="isPersistent"></param>
        //public void IdentitySignin(AppUserState appUserState, string providerKey = null, bool isPersistent = false)
        //{
        //    var claims = new List<Claim>();

        //    // create *required* claims
        //    claims.Add(new Claim(ClaimTypes.NameIdentifier, appUserState.UserId.ToString()));
        //    claims.Add(new Claim(ClaimTypes.Name, appUserState.Name));

        //    // serialized AppUserState object
        //    claims.Add(new Claim("userState", appUserState.ToString()));

        //    var identity = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);

        //    // add to user here!
        //    AuthenticationManager.SignIn(new AuthenticationProperties()
        //    {
        //        AllowRefresh = true,
        //        IsPersistent = isPersistent,
        //        ExpiresUtc = DateTime.UtcNow.AddDays(7)
        //    }, identity);

        //    //var token = Helpers.GenerateToken(appUserState.Name.ToLower(), appUserState.Wgs, appUserState.Ds, "inreachweb");
        //    //ViewData["access_token"] = Helpers.WriteToken(token);
        //    //ViewData["access_token_ttl"] = token.ValidTo.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;

        //}

        //public void IdentitySignout()
        //{
        //    AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie,
        //        DefaultAuthenticationTypes.ExternalCookie);
        //}

        private IAuthenticationManager AuthenticationManager => HttpContext.GetOwinContext().Authentication;

        #endregion



        [System.Web.Mvc.HttpPost]
        public ActionResult Submit(IEnumerable<HttpPostedFileBase> files)
        {
            if (files != null)
            {
                TempData["UploadedFiles"] = GetFileInfo(files);
            }

            return RedirectToRoute("Demo", new { section = "upload", example = "result" });
        }

        public ActionResult Save(IEnumerable<HttpPostedFileBase> files)
        {
            // The Name of the Upload component is "files"
            if (files != null)
            {
                foreach (var file in files)
                {
                    // Some browsers send file names with full path. This needs to be stripped.
                    //var fileName = Path.GetFileName(file.FileName);
                    //var physicalPath = Path.Combine(Server.MapPath("~/App_Data"), fileName);

                    // The files are not actually saved in this demo
                    // file.SaveAs(physicalPath);
                }
            }

            // Return an empty string to signify success
            return Content("");
        }

        public ActionResult Remove(string[] fileNames)
        {
            // The parameter of the Remove action must be called "fileNames"

            if (fileNames != null)
            {
                foreach (var fullName in fileNames)
                {
                    var fileName = Path.GetFileName(fullName);
                    var physicalPath = Path.Combine(Server.MapPath("~/App_Data"), fileName);

                    // TODO: Verify user permissions

                    if (System.IO.File.Exists(physicalPath))
                    {
                        // The files are not actually removed in this demo
                        // System.IO.File.Delete(physicalPath);
                    }
                }
            }

            // Return an empty string to signify success
            return Content("");
        }

        private IEnumerable<string> GetFileInfo(IEnumerable<HttpPostedFileBase> files)
        {
            return
                from a in files
                where a != null
                select string.Format("{0} ({1} bytes)", Path.GetFileName(a.FileName), a.ContentLength);
        }

    }


}
