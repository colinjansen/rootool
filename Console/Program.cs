using Replication.RooTool.Library;
using System.Diagnostics;

namespace Replication.RooTool.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var proc = new BigSmellyFileProcesser("input.zip", "output.xlsx", "data.json", "mappings.json").Extract();
            var input = proc.GetRooToolInputData();
            var map = proc.GetMappings();


            var tool = new RecordOfObservationTool(input, map);
            tool.Process();
            var stream = tool.GetStream();


            proc.Output(stream);
            proc.Cleanup();

            Process.Start(new ProcessStartInfo 
            {
                FileName = "output.xlsx",
                UseShellExecute = true
            });

            System.Console.WriteLine("should be there. you should check now... go check now. now. NOW!!!!");
        }
    }
}
