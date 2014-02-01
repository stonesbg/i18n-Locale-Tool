using System.Collections.Generic;
using System.IO;

namespace i18n.LocaleTool.Models
{
    public class i18nDirectoryFile
    {
        public FileInfo FileInfo { get; set; }
        public Dictionary<string, object> Dictionary { get; set; }
    }
}
