using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace i18n.LocaleTool.Models
{
    public class i18nDirectoryFile
    {
        public FileInfo FileInfo { get; set; }
        public Dictionary<string, object> Dictionary { get; set; }
    }
}
