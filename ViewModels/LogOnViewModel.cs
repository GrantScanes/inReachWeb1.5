using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using inReachWebRebuild.Classes;
using inReachWebRebuild.Models;
using Infor.Model;

namespace inReachWebRebuild.ViewModels
{
    public class LogOnViewModel
    {
        [Required]
        [AllowHtml]
        [DataType(DataType.Text)]
        public string Username { get; set; }

        [Required]
        [AllowHtml]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [AllowHtml]
        [DataType(DataType.Text)]
        public string WorkGroupServer { get; set; }

        [Required]
        [AllowHtml]
        [DataType(DataType.Text)]
        public string DataSetId { get; set; }

        [Required]
        [AllowHtml]
       public bool RememberMe { get; set; }

        [HiddenInput]
        public string ReturnUrl { get; set; }

        public ErrorDisplay ErrorDisplay = null;
        public InforUser User = null;
        public AppUserState AppUserState = null;

    }
}