using Artemis.Interfaces;
using Artemis.Model;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Artemis
{
    public class ImageUtil : IImageUtil
    {
        private readonly IAzureBlobStorage _azureBlobStorage;
        private readonly ICurrentUserRepository _profileRepository;

        public ImageUtil(IAzureBlobStorage azureBlobStorage, ICurrentUserRepository profileRepository)
        {
            _azureBlobStorage = azureBlobStorage;
            _profileRepository = profileRepository;
        }

        /// <summary>Adds the image to current user.</summary>
        /// <param name="currentUser">The current user.</param>
        /// <param name="image">The image.</param>
        /// <param name="title">The title.</param>
        /// <exception cref="Exception"></exception>
        public async Task AddImageToCurrentUser(CurrentUser currentUser, IFormFile image, string title)
        {
            try
            {
                // TODO: Scan files for virus!!!!!

                var randomFileName = Path.GetRandomFileName();
                var fileName = randomFileName.Split('.');

                var format = image.ContentType.Split('/');

                // Save original image
                using (var stream = image.OpenReadStream())
                {
                    await _azureBlobStorage.UploadAsync(currentUser.ProfileId, Path.Combine(fileName[0] + '.' + format[1]), stream);
                }

                // Save image reference to database. Must come after save to disk/filestream or it will save empty image because of async call.
                await _profileRepository.AddImageToCurrentUser(currentUser, fileName[0] + '.' + format[1], title);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>Deletes images for current user.</summary>
        /// <param name="currentUser">The current user.</param>
        /// <param name="imageIds">The image identifier.</param>
        public async Task DeleteImagesForCurrentUser(CurrentUser currentUser, string[] imageIds)
        {
            try
            {
                foreach (var imageId in imageIds)
                {
                    var imageModel = currentUser.Images.Find(i => i.ImageId == imageId);

                    if (imageModel != null)
                    {
                        // Remove image reference in database.
                        await _profileRepository.RemoveImageFromCurrentUser(currentUser, imageId);

                        foreach (var size in Enum.GetNames(typeof(ImageSizeEnum)))              // TODO: Have a look if we shoould use ImageSizeEnum
                        {
                            // TODO: Temp condition to add jpeg to un-typed images.
                            if (!imageModel.FileName.Contains('.'))
                            {
                                imageModel.FileName += ".jpeg";
                            }

                            await _azureBlobStorage.DeleteImageByFileNameAsync(Path.Combine(currentUser.ProfileId, Path.Combine(size.ToString(), imageModel.FileName)));
                        }
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>Deletes all images for profile. There is no going back!</summary>
        /// <param name="currentUser">The CurrentUser.</param>
        /// <param name="profileId">The profile identifier.</param>
        /// <exception cref="Exception">You don't have admin rights to delete other people's images.</exception>
        /// <exception cref="ArgumentException">ProfileId is missing. {profileId}</exception>
        public async Task DeleteAllImagesForProfile(CurrentUser currentUser, string profileId)
        {
            if (!currentUser.Admin) throw new Exception("You don't have admin rights to delete other people's images.");

            if (profileId == null) throw new ArgumentException("ProfileId is missing.");

            try
            {
                await _azureBlobStorage.DeleteAllImagesAsync(profileId);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>Deletes all images for CurrentUser. There is no going back!</summary>
        /// <param name="currentUser">The CurrentUser.</param>
        /// <exception cref="ArgumentException">ProfileId is missing. {currentUser.ProfileId}</exception>
        public async Task DeleteAllImagesForCurrentUser(CurrentUser currentUser)
        {
            if (currentUser.ProfileId == null) throw new ArgumentException("ProfileId is missing.");

            try
            {
                await _azureBlobStorage.DeleteAllImagesAsync(currentUser.ProfileId);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>Copy image from the random imgae folder to ProfileId</summary>
        /// <param name="sourceImage">The sourceImage identifier.</param>
        /// <param name="profileId">The profile identifier.</param>
        /// <exception cref="ArgumentException">ProfileId is missing. {currentUser.ProfileId}</exception>
        public async Task CopyImageFromRandomFolderToProfileId(string sourceImage, string profileId)
        {
            if (string.IsNullOrEmpty(profileId)) throw new ArgumentException("ProfileId is missing.");

            try
            {
                await _azureBlobStorage.CopyImageFromRandomFolderToProfileId(sourceImage, profileId);
            }
            catch
            {
                throw;
            }
        }
    }
}
