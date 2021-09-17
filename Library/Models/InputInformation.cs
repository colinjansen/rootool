using System.Collections.Generic;
using System.IO;

namespace Replication.RooTool.Library.Models
{
    public class InputInformation
    {
        public string[][] Data;
        public Dictionary<string, FileInfo> Photos { get; set; } = new Dictionary<string, FileInfo>();
    }
}
