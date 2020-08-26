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
        private readonly ICurrentUserRepository _profileRepository;
        private readonly long _fileSizeLimit;

        public ImageUtil(IConfiguration config, ICurrentUserRepository profileRepository)
        {
            _fileSizeLimit = config.GetValue<long>("FileSizeLimit");
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
                if (image.Length < 0 || image.Length > _fileSizeLimit)
                {
                    // TODO: Find på noget bedre end en exception når den fejler fx. pga. file size.
                    throw new Exception();
                }

                // TODO: Scan files for virus!!!!!

                var randomFileName = Path.GetRandomFileName();
                var fileName = randomFileName.Split('.');

                // TODO: Find a place for you files!
                if (!Directory.Exists($"C:/Peter Rose - Private/Photos/{currentUser.ProfileId}"))
                {
                    Directory.CreateDirectory($"C:/Peter Rose - Private/Photos/{currentUser.ProfileId}");
                }

                using (var filestream = File.Create($"C:/Peter Rose - Private/Photos/{currentUser.ProfileId}/{fileName[0]}.png"))
                {
                    await image.CopyToAsync(filestream);
                    filestream.Flush();
                }

                // Save image reference to database. Most come after save to disk/filestream or it will save empty image because of async call.
                await _profileRepository.AddImageToCurrentUser(currentUser, fileName[0], title);
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
                        // TODO: Find a place for you files!
                        if (File.Exists($"C:/Peter Rose - Private/Photos/{currentUser.ProfileId}/{imageModel.FileName}.png"))
                        {
                            File.Delete($"C:/Peter Rose - Private/Photos/{currentUser.ProfileId}/{imageModel.FileName}.png");
                        }

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
                byte[] result;

                // TODO: Find a place for you files!
                if (Directory.Exists($"C:/Peter Rose - Private/Photos/123"))
                {
                    var files = Directory.GetFiles($"C:/Peter Rose - Private/Photos/123/");

                    foreach (var file in files)
                    {
                        using (FileStream stream = File.Open(file, FileMode.Open))
                        {
                            result = new byte[stream.Length];
                            await stream.ReadAsync(result, 0, (int)stream.Length);
                        }

                        images.Add(result);
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
                // TODO: Find a place for you files!
                if (File.Exists($"C:/Peter Rose - Private/Photos/{profileId}/{fileName}.png"))
                {
                    byte[] result;

                    using (FileStream stream = File.Open($"C:/Peter Rose - Private/Photos/{profileId}/{fileName}.png", FileMode.Open))
                    {
                        result = new byte[stream.Length];
                        await stream.ReadAsync(result, 0, (int)stream.Length);
                    }

                    return result;
                }

                return null; // TODO: Should return exception or error
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
                // TODO: Find a place for you files!
                //if (Directory.Exists($"C:/Peter Rose - Private/Photos/{profileId}"))
                //{
                //    Directory.Delete($"C:/Peter Rose - Private/Photos/{profileId}", true);
                //}
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
                // TODO: Find a place for you files!
                //if (Directory.Exists($"C:/Peter Rose - Private/Photos/{currentUser.ProfileId}"))
                //{
                //    Directory.Delete($"C:/Peter Rose - Private/Photos/{currentUser.ProfileId}", true);
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
