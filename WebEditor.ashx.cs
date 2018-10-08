using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Services;
using inReachWebRebuild.Controllers;
using Infor.Model.DocumentServices;
using Newtonsoft.Json.Linq;
using NLog;

namespace inReachWebRebuild
{
    /// <summary>
    ///     Summary description for WebEditor
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class WebEditor : IHttpHandler
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly string _PDPath = HttpContext.Current.Server.MapPath($"~/App_Data/inReachRepos/Global/PD");
        public void ProcessRequest(HttpContext context)
        {
            Logger.Info($"incomming request {context.Request["type"]}");

            switch (context.Request["type"])
            {
                //case "upload":
                //    Upload(context);
                //    break;
                //case "convert":
                //    Convert(context);
                //    break;
                case "track":
                    Track(context);
                    break;
            }
        }

        public bool IsReusable => false;

        private static void Track(HttpContext context)
        {
            Logger.Info($"track");
            string documentContents;
            using (var receiveStream = context.Request.InputStream)
            {
                using (var readStream = new StreamReader(receiveStream, Encoding.UTF8))
                {
                    documentContents = readStream.ReadToEnd();
                }
            }

            Logger.Info($"request {documentContents}");


            var userAddress = context.Request["userAddress"];
            var fileName = context.Request["fileName"];
            Logger.Info($"{userAddress} and {fileName} ");
            var storagePath = "";
            var userdata = "";
            var recUri = 0;
            var key = "";
            var status = TrackerStatus.NotFound;
            var downloadUri = "";
            try
            {
                dynamic data = JObject.Parse(documentContents);
                key = (string) data.key;
                recUri = Convert.ToInt32(key.Split(new[] {"__"}, StringSplitOptions.None)[0]);
                var userUri = key.Split(new[] {"__"}, StringSplitOptions.None)[1];
                storagePath = DocManagerHelper.StoragePath(Convert.ToInt64(userUri)) + $"\\{fileName}";
                Logger.Info($"{storagePath} ");

                status = data.status;
                Logger.Info($"status is {status} ");

                downloadUri = (string) data.url;
                Logger.Info($"url is {downloadUri}");

                userdata = (string) data.userdata;
                Logger.Info($"userdata is {userdata}");
            }
            catch (Exception e)
            {
                context.Response.Write("{\"error\":0}");
                Logger.Info($"error {e.Message} ");
            }

            string body;
            try
            {
                using (var receiveStream = context.Request.InputStream)
                using (var readStream = new StreamReader(receiveStream))
                {
                    body = readStream.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                context.Response.Write("{\"error\":0}");
                Logger.Info($"saving error");
                throw new HttpException((int) HttpStatusCode.BadRequest, e.Message);
            }

            var docController = new DocumentController();
          
            switch (status)
            {
                case TrackerStatus.Closed:
                    //WriteDocumentFile(downloadUri, storagePath);
                    if (userdata != null)
                    {
                        if (userdata.Contains("checkin"))
                        {
                            docController.DropUser(key, userdata);
                        }
                    }
                    break;

                case TrackerStatus.MustSave:
                case TrackerStatus.Corrupted:
                case TrackerStatus.Saved:

                    //if (!File.Exists(Path.Combine(_PDPath, Path.GetFileNameWithoutExtension(storagePath))))
                    //{
                    //    var f = File.Create(Path.Combine(_PDPath, Path.GetFileNameWithoutExtension(storagePath)));
                    //    f.Close();
                    //}

                   
                    WriteDocumentFile(downloadUri, storagePath);
                    if (userdata != null)
                    {
                        if (userdata.Contains("checkin"))
                        {
                            docController.CheckinDocument(recUri, userdata.Split(new[] { "||" }, StringSplitOptions.None)[3],
                                userdata.Split(new[] { "||" }, StringSplitOptions.None)[4],
                                userdata.Split(new[] { "||" }, StringSplitOptions.None)[2]);
                        }
                        else
                        {
                            if (File.Exists(Path.Combine(_PDPath, Path.GetFileNameWithoutExtension(storagePath))))
                                File.Delete(Path.Combine(_PDPath, Path.GetFileNameWithoutExtension(storagePath)));
                        }
                    }
                    else
                    {
                        if (File.Exists(Path.Combine(_PDPath, Path.GetFileNameWithoutExtension(storagePath))))
                            File.Delete(Path.Combine(_PDPath, Path.GetFileNameWithoutExtension(storagePath)));
                    }


                    break;
            }


            context.Response.Write("{ \"error\" : \"" + 0 + "\"}");
        }

        private static void WriteDocumentFile(string downloadUri, string storagePath)
        {
            var req = (HttpWebRequest) WebRequest.Create(downloadUri);

            var saved = 1;
            try
            {
                using (var stream = req.GetResponse().GetResponseStream())
                {
                    if (stream == null) throw new Exception("stream is null");
                    const int bufferSize = 4096;

                    using (var fs = File.Open(storagePath, FileMode.Create))
                    {
                        var buffer = new byte[bufferSize];
                        int readed;
                        while ((readed = stream.Read(buffer, 0, bufferSize)) != 0) fs.Write(buffer, 0, readed);
                    }
                   

                }
            }
            catch (Exception e)
            {
                 
                saved = 0;
            }
        }

        //private static void Upload(HttpContext context)
        //{
        //    context.Response.ContentType = "text/plain";
        //    try
        //    {
        //        var httpPostedFile = context.Request.Files[0];
        //        string fileName;

        //        if (HttpContext.Current.Request.Browser.Browser.ToUpper() == "IE")
        //        {
        //            var files = httpPostedFile.FileName.Split(new char[] { '\\' });
        //            fileName = files[files.Length - 1];
        //        }
        //        else
        //        {
        //            fileName = httpPostedFile.FileName;
        //        }

        //        var curSize = httpPostedFile.ContentLength;
        //        if (DocManagerHelper.MaxFileSize < curSize || curSize <= 0)
        //        {
        //            throw new Exception("File size is incorrect");
        //        }

        //        var curExt = (Path.GetExtension(fileName) ?? "").ToLower();
        //        if (!DocManagerHelper.FileExts.Contains(curExt))
        //        {
        //            throw new Exception("File type is not supported");
        //        }

        //        fileName = DocManagerHelper.GetCorrectName(fileName);

        //        var savedFileName = DocManagerHelper.StoragePath(fileName);
        //        httpPostedFile.SaveAs(savedFileName);

        //        context.Response.Write("{ \"filename\": \"" + fileName + "\"}");
        //    }
        //    catch (Exception e)
        //    {
        //        context.Response.Write("{ \"error\": \"" + e.Message + "\"}");
        //    }
        //}

        //private static void Convert(HttpContext context)
        //{
        //    context.Response.ContentType = "text/plain";
        //    try
        //    {
        //        var fileName = context.Request["filename"];
        //        var fileUri = DocManagerHelper.GetFileUri(fileName);

        //        var extension = (Path.GetExtension(fileUri) ?? "").Trim('.');
        //        var internalExtension = DocManagerHelper.GetInternalExtension(FileUtility.GetFileType(fileName)).Trim('.');

        //        if (DocManagerHelper.ConvertExts.Contains("." + extension)
        //            && !string.IsNullOrEmpty(internalExtension))
        //        {
        //            var key = ServiceConverter.GenerateRevisionId(fileUri);

        //            string newFileUri;
        //            var result = ServiceConverter.GetConvertedUri(fileUri, extension, internalExtension, key, true, out newFileUri);
        //            if (result != 100)
        //            {
        //                context.Response.Write("{ \"step\" : \"" + result + "\", \"filename\" : \"" + fileName + "\"}");
        //                return;
        //            }

        //            var correctName = DocManagerHelper.GetCorrectName(Path.GetFileNameWithoutExtension(fileName) + "." + internalExtension);

        //            var req = (HttpWebRequest)WebRequest.Create(newFileUri);

        //            using (var stream = req.GetResponse().GetResponseStream())
        //            {
        //                if (stream == null) throw new Exception("Stream is null");
        //                const int bufferSize = 4096;

        //                using (var fs = File.Open(DocManagerHelper.StoragePath(correctName), FileMode.Create))
        //                {
        //                    var buffer = new byte[bufferSize];
        //                    int readed;
        //                    while ((readed = stream.Read(buffer, 0, bufferSize)) != 0)
        //                    {
        //                        fs.Write(buffer, 0, readed);
        //                    }
        //                }
        //            }

        //            File.Delete(DocManagerHelper.StoragePath(fileName));
        //            fileName = correctName;
        //        }

        //        context.Response.Write("{ \"filename\" : \"" + fileName + "\"}");
        //    }
        //    catch (Exception e)
        //    {
        //        context.Response.Write("{ \"error\": \"" + e.Message + "\"}");
        //    }
        //}

        private enum TrackerStatus
        {
            NotFound = 0,
            Editing = 1,
            MustSave = 2,
            Corrupted = 3,
            Closed = 4,
            Saved = 6
        }
    }
}