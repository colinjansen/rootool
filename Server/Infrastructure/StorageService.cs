using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Files.Shares;
using Microsoft.Extensions.Configuration;
using Replication.RooTool.Core.Interfaces;
using Replication.RooTool.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Replication.RooTool.Infrastructure
{
    public class StorageService: IStorageService
    {
        private readonly IConfiguration _configuration;

        private ShareClient _shareClient;
        private BlobServiceClient _blobServiceClient;

        private ShareClient ShareClient
        {
            get
            {
                if (_shareClient == null)
                {
                    _shareClient = new ShareClient(_configuration.GetConnectionString("Storage"), "files");
                    _shareClient.CreateIfNotExists();
                }
                return _shareClient;
            }
        }

        private BlobServiceClient BlobServiceClient
        {
            get
            {
                if (_blobServiceClient == null)
                {
                    _blobServiceClient = new BlobServiceClient(_configuration.GetConnectionString("Storage"));
                }
                return _blobServiceClient;
            }
        }

        public StorageService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> AssertUniqueDirectoryName(string path)
        {
            string checkPath = path;
            int counter = 0;
            while (! await FileShareDirectoryPathIsUnique(checkPath))
            {
                checkPath = $"{path} ({++counter})";
            }
            return checkPath;
        }

        public async Task<List<DataFolder>> GetListingOfDataFolders(string folder)
        {
            var list = new List<DataFolder>();
            var directory = await GetFileShareDirectoryAsync(folder);
            foreach (var item in directory.GetFilesAndDirectories())
            {
                // they should all be directories
                if (item.IsDirectory)
                {
                    var subDirectory = directory.GetSubdirectoryClient(item.Name);
                    list.Add(new DataFolder
                    { 
                        Name = item.Name,
                        CreatedOn = await GetCreationDateOfFileShareDiretory(subDirectory),
                        Size = GetTotalFileSizeInFileShareDiretory(subDirectory)
                    });
                }
            }
            return list;
        }

        public async Task<Stream> GetDataFile(string folder, string fileName)
        {
            var directory = await GetFileShareDirectoryAsync(folder, false);
            if (directory == null)
            {
                return null;
            }
            var file = directory.GetFileClient(fileName);
            if (!file.Exists())
            {
                return null;
            }
            return file.Download().Value.Content;
        }

        public async Task UploadFile(string localFilePath, string folder, string fileName)
        {
            var directory = await GetFileShareDirectoryAsync(folder);
            ShareFileClient file = directory.GetFileClient(fileName);
            using FileStream stream = File.OpenRead(localFilePath);
            file.Create(stream.Length);
            file.UploadRange(new HttpRange(0, stream.Length), stream);
        }

        public async Task DeleteDataFolder(string folder)
        {
            var directory = await GetFileShareDirectoryAsync(folder, false);
            if (directory != null)
            {
                await RemoveDirectoryRecursively(directory);
            }
        }

        public void StoreData()
        {
        }

        public void RemoveData()
        {
        }

        public void GetDataList()
        {
        }

        private async Task RemoveDirectoryRecursively(ShareDirectoryClient directory)
        {
            foreach (var item in directory.GetFilesAndDirectories())
            {
                if (item.IsDirectory)
                {
                    var subDirectory = directory.GetSubdirectoryClient(item.Name);
                    await RemoveDirectoryRecursively(subDirectory);
                }
                else
                {
                    await directory.DeleteFileAsync(item.Name);
                }
            }
            await directory.DeleteAsync();
        }

        private async Task<bool> FileShareDirectoryPathIsUnique(string path, string separator = "/")
        {
            ShareDirectoryClient directory = null;
            foreach (var piece in path.Split(separator))
            {
                if (directory == null)
                {
                    directory = ShareClient.GetDirectoryClient(piece);
                }
                else
                {
                    directory = directory.GetSubdirectoryClient(piece);
                }
                if (!(await directory.ExistsAsync()).Value)
                {
                    return true;
                }
            }
            return false;
        }

        private async Task<DateTimeOffset> GetCreationDateOfFileShareDiretory(ShareDirectoryClient directory)
        {
            var directoryProps = (await directory.GetPropertiesAsync()).Value;
            return directoryProps.LastModified;
        }

        private long GetTotalFileSizeInFileShareDiretory(ShareDirectoryClient directory, long total = 0)
        {
            foreach (var item in directory.GetFilesAndDirectories())
            {
                if (item.IsDirectory)
                {
                    var subDirectory = directory.GetSubdirectoryClient(item.Name);
                    total += GetTotalFileSizeInFileShareDiretory(subDirectory, total);
                }
                if (item.FileSize.HasValue)
                {
                    total += item.FileSize.Value;
                }
            }
            return total;
        }

        private async Task<ShareDirectoryClient> GetFileShareDirectoryAsync(string folder, bool createIfNotExist=true, string separator = "/")
        {
            ShareDirectoryClient directory = null;
            foreach (var piece in folder.Split(separator))
            {
                if (directory == null)
                {
                    directory = ShareClient.GetDirectoryClient(piece);
                }
                else
                {
                    directory = directory.GetSubdirectoryClient(piece);
                }
                if (!directory.Exists())
                {
                    if (!createIfNotExist)
                    {
                        return null;
                    }
                    await directory.CreateIfNotExistsAsync();              
                }
            }
            return directory;
        }
    }
}
