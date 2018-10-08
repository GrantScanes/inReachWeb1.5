using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Serialization;
using inReachWebRebuild.Models;
using Infor.Model;
using Infor.Model.ApplicationServices;
using NLog;

namespace inReachWebRebuild.Common
{
    public class WopiAppHelper
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private string _discoveryFile;
        private readonly bool _updateEnabled = false;
        private readonly wopidiscovery _wopiDiscovery;

        private static readonly string Tu = SettingsManager.GetSettingValueAsString("TU");  
        private static readonly string Tup = SettingsManager.GetSettingValueAsString("TUP");  

        public WopiAppHelper(string discoveryXml)
        {
            var xr = XmlReader.Create(new StringReader(discoveryXml));

            var reader = new XmlSerializer(typeof(wopidiscovery));
            var wd = reader.Deserialize(xr) as wopidiscovery;
            _wopiDiscovery = wd;

            //_discoveryFile = discoveryXml;

            //using (StreamReader file = new StreamReader(discoveryXml))
            //{
            //    XmlSerializer reader = new XmlSerializer(typeof(WopiHost.wopidiscovery));
            //    var wopiDiscovery = reader.Deserialize(file) as WopiHost.wopidiscovery;
            //    _wopiDiscovery = wopiDiscovery;
            //}
        }

        public WopiAppHelper(string discoveryXml, bool updateEnabled)
            : this(discoveryXml)
        {
            _updateEnabled = updateEnabled;
        }

        public wopidiscoveryNetzoneApp GetZone(string appName)
        {
            var netzone = _wopiDiscovery.netzone.FirstOrDefault(f => f.name.Contains("external"));

            var rv = netzone.app.Where(c => c.name == appName).FirstOrDefault();
            return rv;
        }

        public string GetDocumentLink(string wopiHost, string file, string username, string wgs, string ds)
        {
            try
            {
              

                var uri = file;

            var conn = new InforConnection(tu: Tu, tup: Tup);
                var rec = conn.GetRecordWeb(Convert.ToInt64(uri),  username, wgs, ds);
            var fileName = conn.GetRecordDocumentWeb(Convert.ToInt64(uri), _updateEnabled, username, wgs, ds).ToLower();

                Logger.Info($"Get docuemnt link {fileName}");

                var accessToken = GetToken(fileName);
            var fileExt = fileName.Substring(fileName.LastIndexOf('.') + 1).ToLower();
                Logger.Info($"Extension {fileExt}");
                var netzone = _wopiDiscovery.netzone.FirstOrDefault(f => f.name.Contains("external"));

            var netzoneApp = netzone.app.AsEnumerable()
                .Where(c => c.action.Where(d => d.ext == fileExt).Count() > 0);

            var appName = netzoneApp.FirstOrDefault();

                if (null == appName) return null;
                var fn = fileName.Substring(fileName.LastIndexOf('\\') + 1);
            var rv = GetDocumentLink(appName.name, fileExt, $"{wopiHost}{fn}", accessToken);
                    
            return rv;
            }
            catch (Exception ex)
            {

            return null;
            }
        }

        private string GetToken(string fileName)
        {
            KeyGen keyGen = new KeyGen();
            var rv = keyGen.GetHash(fileName);

            return HttpUtility.UrlEncode(rv);
        }

        private const string SWopiHostFormat = "{0}?WOPISrc={1}?access_token={2}";
        //HACK:
        private const string SWopiHostFormatPdf = "{0}?PdfMode=1&WOPISrc={1}?access_token={2}";

        public string GetDocumentLink(string appName, string fileExtension, string wopiHostAndFile, string accessToken)
        {
            var wopiHostUrlsafe = HttpUtility.UrlEncode(wopiHostAndFile.Replace(" ", "%20"));
            var netzone = _wopiDiscovery.netzone.FirstOrDefault(f => f.name.Contains("external"));

            var appStuff = netzone.app.Where(c => c.name == appName).FirstOrDefault();

            if (null == appStuff)
                throw new ApplicationException("Can't locate App: " + appName);

            var action = _updateEnabled ? "edit" : "view";

            var appAction = appStuff.action.Where(c => c.ext == fileExtension && c.name == action).FirstOrDefault();

            if (null == appAction)
                throw new ApplicationException("Can't locate UrlSrc for : " + appName);

            var endPoint = appAction.urlsrc.IndexOf('?');
            var endAction = appAction.urlsrc.Substring(0, endPoint);

            string fullPath = null;
            ////HACK: for PDF now just append WordPdf option...
            if (fileExtension.Contains("pdf"))
            {
                fullPath = string.Format(SWopiHostFormatPdf, endAction, wopiHostUrlsafe, accessToken);
            }
            else
            {
                fullPath = string.Format(SWopiHostFormat, endAction, wopiHostUrlsafe, accessToken);
            }
            Logger.Info($"path {fullPath}");
            return fullPath;
        }
    }
}