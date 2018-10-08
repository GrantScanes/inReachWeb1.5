using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace inReachWebRebuild.Classes
{
    public static class Helpers
    {
        public static string DetermineCompName(string ip)
        {
            var myIp = IPAddress.Parse(ip);
            var getIpHost = Dns.GetHostEntry(myIp);
            var compName = getIpHost.HostName.Split('.').ToList();
            return compName.First();
        }

        public static byte[] GetBytes(string str)
        {
            var bytes = new byte[str.Length*sizeof(char)];
            Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        public static string GetString(byte[] bytes)
        {
            var chars = new char[bytes.Length/sizeof(char)];
            Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

        //public static JwtSecurityToken GenerateToken(string user, string wgs, string ds, string origin)
        //{
        //    //var now = DateTime.UtcNow;
        //    //var tokenHandler = new JwtSecurityTokenHandler();
        //    ////var signingCert = getCert();
        //    //var tokenDescriptor = new SecurityTokenDescriptor
        //    //{
        //    //    Subject = new ClaimsIdentity(new[]
        //    //    {
        //    //        new Claim(ClaimTypes.Name, user),
        //    //        new Claim("wgs", wgs),
        //    //        new Claim("ds", ds),
        //    //        new Claim("origin", origin)
        //    //    }),
        //    //    Audience = "inreachweb",
        //    //    IssuedAt = now,
        //    //    Issuer = "inReachWeb",
        //    //    Expires = now.AddDays(1)
        //    //};
        //    //var token = (JwtSecurityToken) tokenHandler.CreateToken(tokenDescriptor);
        //    //return token;
        //    return null;
        //}

        //public static string WriteToken(JwtSecurityToken token)
        //{
        //    var tokenHandler = new JwtSecurityTokenHandler();

        //    return tokenHandler.WriteToken(token);
        //}

       
        public static string GetCookieValue(string cookieName, HttpResponseBase response)
        {
            var cookie = HttpCookieEncryption.Decrypt(response.Cookies[cookieName]);

           return cookie.Value;
        }

        public static bool CookieExists(string cookieName, HttpResponseBase response)
        {
            return response.Cookies.AllKeys.Contains(cookieName);
        }

        public static void RemoveCookie(string cookieName, HttpResponseBase response)
        {
            if (!CookieExists(cookieName, response)) return;
            var cookie = HttpCookieEncryption.Decrypt(response.Cookies[cookieName]);
            cookie.Expires = DateTime.Now.AddDays(-1);
            response.Cookies.Set(HttpCookieEncryption.Encrypt(cookie));
        }

        public static MvcHtmlString Nl2Br(this HtmlHelper htmlHelper, string text)
        {
            if (string.IsNullOrEmpty(text))
                return MvcHtmlString.Create(text);
            var builder = new StringBuilder();
            var lines = text.Split('\n');
            for (var i = 0; i < lines.Length; i++)
            {
                if (i > 0)
                    builder.Append("<br/>\n");
                builder.Append(HttpUtility.HtmlEncode(lines[i]));
            }
            return MvcHtmlString.Create(builder.ToString());
        }

       
    }
}