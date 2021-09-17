using Replication.RooTool.Library.Models;
using System.IO;
using System.Threading.Tasks;

namespace Replication.RooTool.Core.Interfaces
{
    public interface IReportingService
    {
        Task<Stream> BuildRooTool(InputInformation input, Mappings map);
    }
}
