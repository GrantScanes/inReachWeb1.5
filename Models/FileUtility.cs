﻿using System.Collections.Generic;
using System.IO;

namespace inReachWebRebuild.Models
{
    public class FileUtility1
    {
        public enum FileType
        {
            Text,
            Spreadsheet,
            Presentation
        }

        public static FileType GetFileType(string fileName)
        {
            var ext = Path.GetExtension(fileName).ToLower();

            if (ExtsDocument.Contains(ext)) return FileType.Text;
            if (ExtsSpreadsheet.Contains(ext)) return FileType.Spreadsheet;
            if (ExtsPresentation.Contains(ext)) return FileType.Presentation;

            return FileType.Text;
        }

        public static readonly List<string> ExtsDocument = new List<string>
            {
                ".docx", ".doc", ".odt", ".rtf", ".txt",
                ".html", ".htm", ".mht", ".pdf", ".djvu",
                ".fb2", ".epub", ".xps", ".idtx"
            };

        public static readonly List<string> ExtsSpreadsheet = new List<string>
            {
                ".xls", ".xlsx",
                ".ods", ".csv"
            };

        public static readonly List<string> ExtsPresentation = new List<string>
            {
                ".pps", ".ppsx",
                ".ppt", ".pptx",
                ".odp"
            };
    }
}
