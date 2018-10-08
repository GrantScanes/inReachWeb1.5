using System;
using System.Collections.Generic;
using Infor.Model;

namespace inReachWebRebuild.Models
{
    public class AppUserState
    {
        public bool Connected;
        public string Ds = string.Empty;
        public string Email = string.Empty;
        public string Message = string.Empty;
        public string Name = string.Empty;
        public string ReportingUserName = string.Empty;
        public string SignInFrom = string.Empty;
        public string Token = string.Empty;
        public long UserId;
        public string UserImgPath = string.Empty;
        public string UserName = string.Empty;
        public string Wgs = string.Empty;
        public List<RLocation> ActionLocations = new List<RLocation>();
        /// <summary>
        ///     Exports a short string list of Id, Email, Name separated by |
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Join("|", UserId, Name, Wgs, Ds, UserName, Connected.ToString(), UserImgPath, ReportingUserName, Token, SignInFrom, Message);
        }

        //public JwtSecurityToken ToToken()
        //{
        //    return Classes.Helpers.GenerateToken(UserId.ToString(), Wgs, Ds, "inReachWeb");
        //}

        /// <summary>
        ///     Imports Id, Email and Name from a | separated string
        /// </summary>
        /// <param name="itemString"></param>
        public bool FromString(string itemString)
        {
            if (string.IsNullOrEmpty(itemString))
                return false;
            var strings = itemString.Split('|');
            if (strings.Length < 5)
                return false;
            UserId = Convert.ToInt64(strings[0]);
            Name = strings[1];
            Wgs = strings[2];
            Ds = strings[3];
            UserName = strings[4];
            Connected = strings[5] == "True";
            if (strings.Length > 6)
                UserImgPath = strings[6];
            if (strings.Length > 7)
                ReportingUserName = strings[7];
            if (strings.Length > 8)
                Token = strings[8];
            if (strings.Length > 9)
                SignInFrom = strings[9];
            if (strings.Length > 10)
                Message = strings[10];
            return true;
        }

        /// <summary>
        ///     Populates the AppUserState properties from a
        ///     User instance
        /// </summary>
        /// <param name="user"></param>
        public void FromUser(InforUser user)
        {
            UserId = user.UserId;
            Name = user.Name;
            Email = user.Email;
            Wgs = user.Wgs;
            Ds = user.Ds;
            UserName = user.UserName;
            UserImgPath = user.UserImagePath;
            ReportingUserName = user.ReportingUserName;
            SignInFrom = user.SigninFrom;
        }

        /// <summary>
        ///     Determines if the user is logged in
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty()
        {
            if (string.IsNullOrEmpty(UserId.ToString()) || string.IsNullOrEmpty(Name))
                return true;
            return false;
        }
    }
}