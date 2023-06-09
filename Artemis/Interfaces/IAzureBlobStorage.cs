using Artemis.Model;
using Azure;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Artemis.Interfaces
{
    public interface IAzureBlobStorage
    {
        Task UploadAsync(string profileIdPath, string fileName, Stream fileStream);
        Task<Stream> DownloadImageByFileNameAsync(string profileId, string fileName);
        Task DeleteImageByFileNameAsync(string fileName);
        //Task<List<Stream>> DownloadAllImagesAsync(string profileId, ImageSizeEnum imageSize);
        Task DeleteAllImagesAsync(string profileId);
    }
}
