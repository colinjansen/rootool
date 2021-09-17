using Replication.RooTool.Core.Interfaces;
using Replication.RooTool.Library;
using Replication.RooTool.Library.Models;
using System.IO;
using System.Threading.Tasks;

namespace Replication.RooTool.Infrastructure
{
    public class ReportingService: IReportingService
    {
        public async Task<Stream> BuildRooTool(InputInformation input, Mappings map)
        {
            return await Task.Run(() =>
            {
                var tool = new RecordOfObservationTool(input, map);
                tool.Process();

                return tool.GetStream();
            });
        }
    }
}
