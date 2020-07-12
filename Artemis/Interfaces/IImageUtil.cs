using Artemis.Model;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Artemis.Interfaces
{
    public interface IImageUtil
    {
        Task AddImageToCurrentUser(CurrentUser currentUser, IFormFile photo, string title);
        Task DeleteImagesFromCurrentUser(CurrentUser currentUser, string[] imageIds);
        //Task<List<byte[]>> GetImagesAsync(string profileId);
        Task<byte[]> GetImageByFileName(string profileId, string fileName);
        void DeleteAllImagesForProfile(CurrentUser currentUser, string profileId);
        void DeleteAllImagesForCurrentUser(CurrentUser currentUser);
    }
}
