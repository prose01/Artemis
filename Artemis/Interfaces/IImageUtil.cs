using Artemis.Model;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Artemis.Interfaces
{
    public interface IImageUtil
    {
        Task AddImageToCurrentUser(CurrentUser currentUser, IFormFile image, string title);
        Task DeleteImagesForCurrentUser(CurrentUser currentUser, string[] imageIds);
        Task DeleteAllImagesForProfile(CurrentUser currentUser, string profileId);
        Task DeleteAllImagesForCurrentUser(CurrentUser currentUser);
        Task CopyImageFromRandomFolderToProfileId(string sourceImage, string profileId);
    }
}
