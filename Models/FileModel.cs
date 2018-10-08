using System;
using inReachWebRebuild.ViewModels;
using Infor.Model.DocumentServices;

namespace inReachWebRebuild.Models
{
    public class FileModel : BaseViewModel
    {
        public bool TypeDesktop { get; set; }
        public string Docservice { get;set; }
        public string FileUri => DocManagerHelper.GetFileUri(FileName, AppUserState.UserId);
        public string FileName { get; set; }
        public long RecordUri { get; set; }
        public string DocumentType => FileUtility.GetFileType(FileName).ToString().ToLower();
        public string Key => $"{RecordUri}__{AppUserState.UserId}__{DateTime.Now.Ticks}" ;
        public string CallbackUrl => DocManagerHelper.GetCallback(FileName);
        public string UserName => AppUserState.UserName;
        public string Wgs => AppUserState.Wgs;
        public string Ds => AppUserState.Ds;
    }

    //public class FileModel
    //{
    //    [JsonProperty(PropertyName = "id")]
    //    public Guid id { get; set; }

    //    [JsonProperty(PropertyName = "Key")]
    //    public string Key { get; set; }

    //    [JsonProperty(PropertyName = "Rapi")]
    //    public string Rapi { get; set; }

    //    [JsonProperty(PropertyName = "LockValue")]
    //    public string LockValue { get; set; }

    //    [JsonProperty(PropertyName = "LockExpires")]
    //    public DateTime? LockExpires { get; set; }

    //    [JsonProperty(PropertyName = "OwnerId")]
    //    public string OwnerId { get; set; }

    //    [JsonProperty(PropertyName = "BaseFileName")]
    //    public string BaseFileName { get; set; }

    //    [JsonProperty(PropertyName = "Container")]
    //    public string Container { get; set; }

    //    [JsonProperty(PropertyName = "Size")]
    //    public long Size { get; set; }

    //    [JsonProperty(PropertyName = "Version")]
    //    public int Version { get; set; }

    //    [JsonProperty(PropertyName = "UserInfo")]
    //    public string UserInfo { get; set; }
    //}
}