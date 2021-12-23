using Artemis.Model;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Artemis.Interfaces
{
    public interface IImageUtil
    {
        Task AddImageToCurrentUser(CurrentUser currentUser, IFormFile image, string title);
        Task DeleteImagesForCurrentUser(CurrentUser currentUser, string[] imageIds);
        Task<List<byte[]>> GetImagesAsync(string profileId, ImageSizeEnum imageSize);
        Task<byte[]> GetImageByFileName(string profileId, string fileName, ImageSizeEnum imageSize);
        void DeleteAllImagesForProfile(CurrentUser currentUser, string profileId);
        void DeleteAllImagesForCurrentUser(CurrentUser currentUser);
    }
}
