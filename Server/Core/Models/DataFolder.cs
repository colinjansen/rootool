using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Replication.RooTool.Core.Models
{
    public class DataFolder
    {
        public string Name { get; set; }
        public long Size { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
    }
}
