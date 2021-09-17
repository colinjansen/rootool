using Ionic.Zip;
using Newtonsoft.Json;
using Replication.RooTool.Library.Models;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Replication.RooTool.Console
{
    internal class BigSmellyFileProcesser
    {
        private string TEMP_FOLDER = "__temp";

        private string _inputFile = string.Empty;
        private string _outputFile = string.Empty;
        private string _dataFile = string.Empty;
        private string _mappingFile = string.Empty;

        public BigSmellyFileProcesser(string inputFile, string outputFile, string dataFile, string mappingFile)
        {
            _inputFile = inputFile;
            _outputFile = outputFile;
            _dataFile = dataFile;
            _mappingFile = mappingFile;
        }

        public BigSmellyFileProcesser Extract()
        {
            using (var readStream = new FileStream(_inputFile, FileMode.Open))
            using (var zip = ZipFile.Read(readStream))
            {
                zip.ExtractAll(TEMP_FOLDER, ExtractExistingFileAction.OverwriteSilently);
            }
            return this;
        }

        public BigSmellyFileProcesser Output(Stream stream)
        {
            using var output = File.OpenWrite(_outputFile);
            stream.Seek(0, SeekOrigin.Begin);
            stream.CopyTo(output);
            output.Close();
            return this;
        }

        public void Cleanup()
        {
            Directory.Delete(TEMP_FOLDER, true);
        }

        private string GetCsvFileInTempFolder(string tempFolder)
        {
            foreach (var file in Directory.GetFiles(tempFolder))
            {
                if (file.ToLower().EndsWith("csv"))
                {
                    return file;
                }
            }
            return null;
        }

        private string[] ProcessLine(string line)
        {
            var lines = Regex.Split(line, ",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
            for (var i = 0; i < lines.Length; i++)
            {
                lines[i] = lines[i].Trim('"');
            }
            return lines;
        }

        public InputInformation GetRooToolInputData()
        {
            var info = new InputInformation();

            var file = GetCsvFileInTempFolder(TEMP_FOLDER);
            if (file == null)
            {
                return info;
            }

            info.Data = File.ReadAllLines(file)
                .Select(line => ProcessLine(line))
                .ToArray();

            foreach (var photo in Directory.GetFiles($"{TEMP_FOLDER}/photos"))
            {
                var photoInfo = new FileInfo(photo);
                info.Photos.Add(Path.GetFileNameWithoutExtension(photoInfo.FullName), photoInfo);
            }

            return info;
        }

        public Mappings GetMappings()
        {
            var text = File.ReadAllText(_mappingFile);
            var map = JsonConvert.DeserializeObject<Mappings>(text);
            return map;
        }
    }
}
