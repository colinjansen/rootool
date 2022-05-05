using Ionic.Zip;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Replication.RooTool.Core.Entities;
using Replication.RooTool.Core.Interfaces;
using Replication.RooTool.Data;
using Replication.RooTool.Library.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Replication.RooTool.Controllers
{
    public class StructureInformationDto
    {
        public string[][] Data { get; set; }
    }

    public class MapDto
    { 
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Data { get; set; }
    }

    [ApiController]
    [Route("api/v1")]
    public class RooToolController : ControllerBase
    {
        const string AccountName = "cjansen@replicationconsulting.ca";

        private readonly IReportingService _reporting;
        private readonly IStorageService _storage;
        private readonly RooToolDbContext _context;

        public RooToolController(IReportingService reporting, IStorageService storage, RooToolDbContext context)
        {
            _storage = storage;
            _reporting = reporting;
            _context = context;
        }

        public class FileUploadDto
        { 
            public IFormFile File { get; set; }
        }


        [HttpPost]
        [DisableRequestSizeLimit]
        [Route("test")]
        public async Task<IActionResult> ProcessFile(IFormFile inputZippedFile)
        {
            var ZipMimeTypes = new List<string> { "application/zip", "application/x-zip-compressed" };
            var mimeType = inputZippedFile.ContentType.ToLower();

            if (!ZipMimeTypes.Contains(mimeType))
            {
                return BadRequest($"unrecognized file type [{mimeType}]");
            }

            var tempFolder = $"{Path.GetTempPath()}{Guid.NewGuid()}";

            try
            {
                using (var readStream = inputZippedFile.OpenReadStream())
                using (var zip = ZipFile.Read(readStream))
                {
                    zip.ExtractAll(tempFolder);
                }

                var inputInformation = GetRooToolInputData(tempFolder);
                var map = GetMappings(inputInformation);
                var stream = await _reporting.BuildRooTool(inputInformation, map);
                stream.Seek(0, SeekOrigin.Begin);
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "workbook.xlsx");
            }
            catch (Exception ex)
            {
                return Problem(ex.StackTrace, ex.Message);
            }
            finally
            {
                Directory.Delete(tempFolder, true);
            }
        }


        [HttpPost]
        [Route("data/upload")]
        public async Task<IActionResult> UploadDataFile([FromForm] FileUploadDto dto)
        {
            if (!dto.File.ContentType.ToLower().Equals("application/x-zip-compressed"))
            {
                return BadRequest("unrecognized file type");
            }
            
            var tempFolder = $"{Path.GetTempPath()}{Guid.NewGuid()}";

            try
            {
                using (var readStream = dto.File.OpenReadStream())
                using (var zip = ZipFile.Read(readStream))
                {
                    zip.ExtractAll(tempFolder);
                }
                await UploadFilesFromTempFolder(dto.File.FileName , tempFolder, AccountName);
                return Ok();
            }
            catch (Exception ex)
            {
                return Problem(ex.StackTrace, ex.Message);
            }
            finally
            {
                Directory.Delete(tempFolder, true);
            }
        }

        [HttpGet]
        [Route("data/list")]
        public async Task<IActionResult> GetDataList()
        {
            try
            {
                return Ok(await _storage.GetListingOfDataFolders(AccountName));
            }
            catch (Exception ex)
            {
                return Problem(ex.StackTrace, ex.Message);
            }
        }

        [HttpDelete]
        [Route("data/delete/{encodedName}")]
        public async Task<IActionResult> DeleteData(string encodedName)
        {
            try
            {
                var name = Base64Decode(encodedName);
                await _storage.DeleteDataFolder($"{AccountName}/{name}");
                return Ok();
            }
            catch (Exception ex)
            {
                return Problem(ex.StackTrace, ex.Message);
            }
        }

        [HttpPost]
        [Route("map/upsert")]
        public async Task<IActionResult> SaveMappingData([FromBody] MapDto dto)
        {
            var map = new DataMapping
            {
                Id = dto.Id,
                Name = dto.Name,
                Data = dto.Data
            };
            if (await _context.DataMappings.AnyAsync(d => d.Id == dto.Id))
            {
                _context.DataMappings.Update(map);
            }
            else
            {
                _context.DataMappings.Add(map);
            }
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet]
        [Route("map/list")]
        public async Task<IActionResult> ListMappingData()
        {
            return Ok(await _context.DataMappings.ToListAsync());
        }

        [HttpDelete]
        [Route("map/delete/{id:guid}")]
        public async Task<IActionResult> DeleteMappingData(Guid id)
        {
            try
            {
                var map = _context.DataMappings.Where(d => d.Id == id).FirstOrDefault();
                if (map != null) {
                    _context.DataMappings.Remove(map);
                    _context.SaveChanges();
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return Problem(ex.StackTrace, ex.Message);
            }
        }

        [HttpPost]
        [Route("map/buildFromData/{encodedName}")]
        public async Task<IActionResult> BuildMapStructureFromData(string encodedName)
        {
            try
            {
                var name = Base64Decode(encodedName);
                var stream = await _storage.GetDataFile($"{AccountName}/{name}", "data.csv");
                if (stream == null)
                {
                    return NotFound();
                }
                var dto = GetStructureData(stream);
                return Ok(dto);
            }
            catch (Exception ex)
            {
                return Problem(ex.StackTrace, ex.Message);
            }
        }

        private string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        private async Task UploadFilesFromTempFolder(string name, string tempFolder, string account)
        {
            var imageExtensions = new List<string> { ".JPG", ".JPEG", ".BMP", ".GIF", ".PNG" };

            var path = await _storage.AssertUniqueDirectoryName($"{account}/{name}");

            foreach (var file in Directory.GetFiles(tempFolder))
            {
                // find the CSV in the root
                if (file.ToLower().EndsWith("csv"))
                {
                    await _storage.UploadFile(file, path, "data.csv");
                }
            }
            foreach (var file in Directory.GetFiles($"{tempFolder}/photos"))
            {
                var info = new FileInfo(file);
                if (imageExtensions.Contains(info.Extension.ToUpperInvariant()))
                {
                    await _storage.UploadFile(file, $"{path}/photos", info.Name);
                }
            }
        }

        private Mappings GetMappings(InputInformation inputInformation)
        {
            var text = System.IO.File.ReadAllText("mappings.json");
            var map = JsonConvert.DeserializeObject<Mappings>(text);

            if (inputInformation.Data.Length == 0)
            {
                throw new Exception("invalid input data: no data");
            }

            var columnNames = inputInformation.Data[0];
            SetOffsetOfColumnName(map.ASEAerialPhotos, columnNames);
            SetOffsetOfColumnName(map.Criteria, columnNames);
            SetOffsetOfColumnName(map.Disposition, columnNames);
            SetOffsetOfColumnName(map.EcoSite, columnNames);
            SetOffsetOfColumnName(map.LandscapeComments, columnNames);
            SetOffsetOfColumnName(map.Latitude, columnNames);
            SetOffsetOfColumnName(map.LegalLocation, columnNames);
            SetOffsetOfColumnName(map.LevelOfDistruption, columnNames);
            SetOffsetOfColumnName(map.Longitude, columnNames);
            foreach (var note in map.Notes)
            {
                SetOffsetOfColumnName(note, columnNames);
            }
            SetOffsetOfColumnName(map.Operator, columnNames);
            SetOffsetOfColumnName(map.Regeneration, columnNames);
            SetOffsetOfColumnName(map.SoilZone, columnNames);
            SetOffsetOfColumnName(map.SubRegion, columnNames);
            foreach (var comment in map.VegetationComments)
            {
                SetOffsetOfColumnName(comment, columnNames);
            }

            return map;
        }

        private void SetOffsetOfColumnName(Mapping mapping, string[] columnNames)
        {
            mapping.Offset = columnNames.ToList().IndexOf(mapping.ColumnName);
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

        private List<string> ReadLinesFromFile(Stream stream, int numberOfLinesToRead = 3)
        {
            var lines = new List<string>();
            string line;
            var counter = 0;
            StreamReader csv = new StreamReader(stream);
            while ((line = csv.ReadLine()) != null && counter++ < numberOfLinesToRead)
            {
                lines.Add(line);
            }
            csv.Close();
            return lines;
        }

        private StructureInformationDto GetStructureData(Stream stream)
        {
            var dto = new StructureInformationDto();

            dto.Data = ReadLinesFromFile(stream, 5)
                .Select(l => ProcessLine(l))
                .ToArray();

            return dto;
        }

        private InputInformation GetRooToolInputData(string tempFolder)
        {
            var info = new InputInformation();

            var file = GetCsvFileInTempFolder(tempFolder);
            if (file == null)
            {
                return info;
            }

            info.Data = System.IO.File.ReadAllLines(file)
                .Select(line => ProcessLine(line))
                .ToArray();

            if (Directory.Exists($"{tempFolder}/photos"))
            {
                foreach (var photo in Directory.GetFiles($"{tempFolder}/photos"))
                {
                    var photoInfo = new FileInfo(photo);
                    info.Photos.Add(Path.GetFileNameWithoutExtension(photoInfo.FullName), photoInfo);
                }
            }
            
            return info;
        }
    }
}
