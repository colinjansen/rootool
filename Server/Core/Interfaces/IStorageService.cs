using Replication.RooTool.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Replication.RooTool.Core.Interfaces
{
    public interface IStorageService
    {
        void StoreData();
        void RemoveData();
        void GetDataList();

        /// <summary>
        /// Get a listing of data folders
        /// </summary>
        /// <param name="folder">The folder to search (probably the account)</param>
        Task<List<DataFolder>> GetListingOfDataFolders(string folder);

        /// <summary>
        /// Upload a file to the Azure file storage
        /// </summary>
        /// <param name="localFilePath">path to the local file (probably in a temp folder)</param>
        /// <param name="folder">the folder to save the file in</param>
        /// <param name="fileName">the name to write the file as</param>
        Task UploadFile(string localFilePath, string folder, string fileName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        Task<Stream> GetDataFile(string folder, string fileName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        Task<string> AssertUniqueDirectoryName(string path);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        Task DeleteDataFolder(string path);
    }
}
