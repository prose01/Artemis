using System.IO;
using System.Threading.Tasks;

namespace Artemis.Interfaces
{
    public interface IAzureBlobStorage
    {
        Task UploadAsync(string profileIdPath, string fileName, Stream fileStream);
        Task DeleteImageByFileNameAsync(string fileName);
        Task DeleteAllImagesAsync(string profileId);
    }
}
