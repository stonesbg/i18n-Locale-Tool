using System.Collections.Generic;
using System.IO;

namespace i18n.LocaleTool.Models
{
    public class I18NDirectoryFile
    {
        public FileInfo FileInfo { get; set; }
        public Dictionary<string, object> Dictionary { get; set; }
    }
}
