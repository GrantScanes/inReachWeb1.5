#region Using

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using Infor.Model;
using Infor.Model.ApplicationServices;
using Infor.Model.DocumentServices;
using NLog;

#endregion

namespace inReachWebRebuild.Helpers
{
    public class DocManagerHelper1
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly string Tu = SettingsManager.GetSettingValueAsString("TU");
        private static readonly string Tup = SettingsManager.GetSettingValueAsString("TUP");
        private static readonly string PdPath = HttpContext.Current.Server.MapPath($"~/App_Data/inReachRepos/Global/PD");

        public static bool IsFileBeingProcessed(string filename)
        {
            return File.Exists(Path.Combine(PdPath, filename));
        }

        public static long MaxFileSize
        {
            get
            {
                long size;
                long.TryParse(SettingsManager.GetSettingValueAsString("filesize-max") , out size);
                return size > 0 ? size : 5 * 1024 * 1024;
            }
        }

        public static List<string> FileExts => ViewedExts.Concat(EditedExts).Concat(ConvertExts).ToList();

        public static List<string> ViewedExts => (SettingsManager.GetSettingValueAsString("files.docservice.viewed-docs")  ?? "").Split(new[]
        {
            '|',
            ','
        }, StringSplitOptions.RemoveEmptyEntries).ToList();

        public static List<string> EditedExts => (SettingsManager.GetSettingValueAsString("files.docservice.edited-docs")  ?? "").Split(new[]
        {
            '|',
            ','
        }, StringSplitOptions.RemoveEmptyEntries).ToList();

        public static List<string> ConvertExts => (SettingsManager.GetSettingValueAsString("files.docservice.convert-docs")  ?? "").Split(new[]
        {
            '|',
            ','
        }, StringSplitOptions.RemoveEmptyEntries).ToList();

        public static string CurUserHostAddress(string userAddress = null)
        {
            return Regex.Replace(userAddress ?? HttpContext.Current.Request.UserHostAddress, "[^0-9a-zA-Z.=]", "_");
        }

        public static string StoragePath(string fileName, string userAddress = null)
        {
            var directory = HttpRuntime.AppDomainAppPath + CurUserHostAddress(userAddress) + "\\";
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            return directory + fileName;
        }

        public static string StoragePath(  string userAddress = null)
        {
            string directory;
            if (string.IsNullOrEmpty(SettingsManager.GetSettingValueAsString("files.docservice.url.localstorage")))
            {
                directory = HttpRuntime.AppDomainAppPath + CurUserHostAddress(userAddress) + "\\";
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                return directory;
            }
            directory = SettingsManager.GetSettingValueAsString("files.docservice.url.localstorage") + "\\" + CurUserHostAddress(userAddress) + "\\";
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            return directory;
        }

        public static string GetCorrectName(string fileName)
        {
            var baseName = Path.GetFileNameWithoutExtension(fileName);
            var ext = Path.GetExtension(fileName);
            var name = baseName + ext;
            for (var i = 1; File.Exists(StoragePath(name)); i++)
            {
                name = baseName + " (" + i + ")" + ext;
            }
            return name;
        }

        public static string CreateDemo(string fileExt)
        {
            var demoName = "sample." + fileExt;
            var fileName = GetCorrectName(demoName);
            File.Copy(HttpRuntime.AppDomainAppPath + "app_data\\" + demoName, StoragePath(fileName));
            return fileName;
        }

        public static string GetFileUri(string fileName)
        {
            var uri = new UriBuilder(HttpContext.Current.Request.Url)
            {
                Path = HttpRuntime.AppDomainAppVirtualPath + "/"
                       + CurUserHostAddress() + "/"
                       + fileName,
                Query = ""
            };
            return uri.ToString();
        }

        public static string GetCallback(string fileName)
        {
            var callbackUrl = new UriBuilder(HttpContext.Current.Request.Url)
            {
                Path =
                    HttpRuntime.AppDomainAppVirtualPath
                    + (HttpRuntime.AppDomainAppVirtualPath != null && HttpRuntime.AppDomainAppVirtualPath.EndsWith("/") ? "" : "/")
                    + "webeditor.ashx",
                Query = "type=track"
                        + "&fileName=" + HttpUtility.UrlEncode(fileName)
                        + "&userAddress=" + HttpUtility.UrlEncode(HttpContext.Current.Request.UserHostAddress)
            };
            return callbackUrl.ToString();
        }

        public static string GetInternalExtension(FileUtility.FileType fileType)
        {
            switch (fileType)
            {
                case FileUtility.FileType.Text:
                    return ".docx";
                case FileUtility.FileType.Spreadsheet:
                    return ".xlsx";
                case FileUtility.FileType.Presentation:
                    return ".pptx";
                default:
                    return ".docx";
            }
        }

        public static string GetDocumentForView(long uri, string username, string wgs, string ds)
        {
            try
            {
               
                var conn = new InforConnection(tu: Tu, tup: Tup);
                var fileName = conn.GetRecordDocumentWeb(Convert.ToInt64(uri), false, username, wgs, ds, StoragePath()).ToLower();
                var curExt = (Path.GetExtension(fileName) ?? "").ToLower();
                if (!FileExts.Contains(curExt))
                {
                    throw new Exception("File type is not supported");
                }
                fileName = GetCorrectName(fileName);
                return fileName;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static string GetDocumentForEdit(long uri, string username, string wgs, string ds)
        {
            try
            {

                var conn = new InforConnection(tu: Tu, tup: Tup);
                var fileName = conn.GetRecordDocumentWeb(Convert.ToInt64(uri), true, username, wgs, ds, StoragePath()).ToLower();
                var curExt = (Path.GetExtension(fileName) ?? "").ToLower();
                if (!FileExts.Contains(curExt))
                {
                    throw new Exception("File type is not supported");
                }
                fileName = GetCorrectName(fileName);
                return fileName;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}