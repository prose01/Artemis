using Artemis.Interfaces;
using Artemis.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Artemis
{
    public class ImageUtil : IImageUtil
    {
        private readonly IAzureBlobStorage _azureBlobStorage;
        private readonly ICurrentUserRepository _profileRepository;
        private readonly long _fileSizeLimit;

        public ImageUtil(IConfiguration config, IAzureBlobStorage azureBlobStorage, ICurrentUserRepository profileRepository)
        {
            _fileSizeLimit = config.GetValue<long>("FileSizeLimit");
            _azureBlobStorage = azureBlobStorage;
            _profileRepository = profileRepository;
        }

        // TODO: Check this website for more info on this - https://docs.microsoft.com/en-us/aspnet/core/mvc/models/file-uploads?view=aspnetcore-3.1


        /// <summary>Adds the image to current user.</summary>
        /// <param name="currentUser">The current user.</param>
        /// <param name="image">The image.</param>
        /// <param name="title">The title.</param>
        /// <exception cref="Exception"></exception>
        public async Task AddImageToCurrentUser(CurrentUser currentUser, IFormFile image, string title)
        {
            try
            {
                //if (image.Length < 0 || image.Length > _fileSizeLimit)
                //{
                //    // TODO: Find på noget bedre end en exception når den fejler fx. pga. file size.
                //    throw new Exception();
                //}

                // TODO: Scan files for virus!!!!!

                //var randomFileName = Path.GetRandomFileName();
                //var fileName = randomFileName.Split('.');

                await _azureBlobStorage.UploadAsync(currentUser.ProfileId, title, new System.IO.MemoryStream());

                //using (var stream = image.OpenReadStream())
                //{
                //    await _azureBlobStorage.UploadAsync(currentUser.ProfileId, "testng", stream);
                //}

                // Save image reference to database. Most come after save to disk/filestream or it will save empty image because of async call.
                //await _profileRepository.AddImageToCurrentUser(currentUser, "testng", title);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>Deletes images from current user.</summary>
        /// <param name="currentUser">The current user.</param>
        /// <param name="imageIds">The image identifier.</param>
        public async Task DeleteImagesFromCurrentUser(CurrentUser currentUser, string[] imageIds)
        {
            try
            {
                foreach (var imageId in imageIds)
                {
                    var imageModel = currentUser.Images.Find(i => i.ImageId == imageId);

                    if (imageModel != null)
                    {
                        await _azureBlobStorage.DeleteImageByFileNameAsync(currentUser.ProfileId, imageModel.FileName);

                        // Remove image reference in database.
                        await _profileRepository.RemoveImageFromCurrentUser(currentUser, imageId);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>Gets all images from specified profileId.</summary>
        /// <param name="profileId">The profile identifier.</param>
        /// <returns></returns>
        public async Task<List<byte[]>> GetImagesAsync(string profileId)
        {
            List<byte[]> images = new List<byte[]>();

            try
            {
                List<Stream> streams = await _azureBlobStorage.DownloadAllImagesAsync(profileId);

                foreach (var stream in streams)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        stream.CopyTo(ms);
                        images.Add(ms.ToArray());
                    }
                }

                return images;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>Gets an images from CurrentUser by Image fileName.</summary>
        /// <param name="currentUser">The current user.</param>
        /// <param name="fileName">The image fileName.</param>
        /// <returns></returns>
        public async Task<byte[]> GetImageByFileName(string profileId, string fileName)
        {
            try
            {
                Stream stream = await _azureBlobStorage.DownloadImageByFileNameAsync(profileId, fileName);

                using (MemoryStream ms = new MemoryStream())
                {
                    stream.CopyTo(ms);
                    return ms.ToArray();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>Deletes all images for profile. There is no going back!</summary>
        /// <param name="currentUser">The CurrentUser.</param>
        /// <param name="profileId">The profile identifier.</param>
        /// <exception cref="Exception">You don't have admin rights to delete other people's images.</exception>
        public void DeleteAllImagesForProfile(CurrentUser currentUser, string profileId)
        {
            if (!currentUser.Admin) throw new Exception("You don't have admin rights to delete other people's images.");

            try
            {
                _azureBlobStorage.DeleteAllImagesAsync(profileId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>Deletes all images for CurrentUser. There is no going back!</summary>
        /// <param name="currentUser">The CurrentUser.</param>
        public void DeleteAllImagesForCurrentUser(CurrentUser currentUser)
        {
            try
            {
                _azureBlobStorage.DeleteAllImagesAsync(currentUser.ProfileId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
