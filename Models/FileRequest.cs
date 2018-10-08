using System.Collections.Generic;
using System.Web.Mvc;

namespace inReachWebRebuild.Models
{
    public class FileRequest
    {
        public string name { get; set; }

        public string SelectedItemId { get; set; }
        public IEnumerable<SelectListItem> Items { get; set; }
    }

    public class RecordRequest
    {
        public long? Id { get; set; }

    }

    public class FilePathRequest
    {
        public string Path { get; set; }
    }
}