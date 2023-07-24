using Artemis.Interfaces;
using Artemis.Model;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Transforms;
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

        public ImageUtil(IAzureBlobStorage azureBlobStorage, ICurrentUserRepository profileRepository)
        {
            _azureBlobStorage = azureBlobStorage;
            _profileRepository = profileRepository;
        }

        // Check this website for more info on this - https://docs.microsoft.com/en-us/aspnet/core/mvc/models/file-uploads?view=aspnetcore-3.1


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

                //// Resize image to small and save 
                //var small = this.ConvertImageToByteArray(image, 150, 150);

                //using (var stream = new MemoryStream(small))
                //{
                //    await _azureBlobStorage.UploadAsync(currentUser.ProfileId, Path.Combine(ImageSizeEnum.small.ToString(), fileName[0] + '.' + format[1]), stream);
                //}

                // Resize image to medium and save 
                //var medium = this.ConvertImageToByteArray(image, 300, 300);

                //using (var stream = new MemoryStream(medium))
                //{
                //    await _azureBlobStorage.UploadAsync(currentUser.ProfileId, Path.Combine(ImageSizeEnum.medium.ToString(), fileName[0]), stream);
                //}

                // Save image reference to database. Most come after save to disk/filestream or it will save empty image because of async call.
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

                        foreach (var size in Enum.GetNames(typeof(ImageSizeEnum)))
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

        ///// <summary>Gets all images from specified profileId.</summary>
        ///// <param name="profileId">The profile identifier.</param>
        ///// <returns></returns>
        //public async Task<List<byte[]>> GetImagesAsync(string profileId, ImageSizeEnum imageSize)
        //{
        //    List<byte[]> images = new List<byte[]>();

        //    try
        //    {
        //        List<Stream> streams = await _azureBlobStorage.DownloadAllImagesAsync(profileId, imageSize);

        //        foreach (var stream in streams)
        //        {
        //            using (MemoryStream ms = new MemoryStream())
        //            {
        //                stream.CopyTo(ms);
        //                images.Add(ms.ToArray());
        //            }
        //        }

        //        return images;
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}

        ///// <summary>Gets an images from CurrentUser by Image fileName.</summary>
        ///// <param name="currentUser">The current user.</param>
        ///// <param name="fileName">The image fileName.</param>
        ///// <returns></returns>
        //public async Task<byte[]> GetImageByFileName(string profileId, string fileName, ImageSizeEnum imageSize)
        //{
        //    try
        //    {
        //        Stream stream = await _azureBlobStorage.DownloadImageByFileNameAsync(profileId, Path.Combine(imageSize.ToString(), fileName));

        //        using (MemoryStream ms = new MemoryStream())
        //        {
        //            stream.CopyTo(ms);
        //            return ms.ToArray();
        //        }
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}

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

        ///// Taken from https://github.com/SixLabors/ImageSharp
        ///// https://docs.sixlabors.com/articles/imagesharp/index.html?tabs=tabid-1
        ///// <summary>Converts the image to byte array.</summary>
        ///// <param name="inputImage">The input image.</param>
        ///// <param name="maxWidth">The maximum width.</param>
        ///// <param name="maxHeight">The maximum height.</param>
        ///// <returns></returns>
        //private byte[] ConvertImageToByteArray(IFormFile inputImage, int maxWidth = 50, int maxHeight = 100)
        //{
        //    try
        //    {
        //        byte[] result = null;

        //        // memory stream
        //        using (var memoryStream = new MemoryStream())

        //        // filestream
        //        using (var image = Image.Load(inputImage.OpenReadStream())) // IFormFile inputImage
        //        {
        //            //var before = memoryStream.Length; Removed this, assuming you are using for debugging?
        //            //var beforeMutations = image.Size();

        //            var width = image.Width;
        //            var height = image.Height;

        //            if (width >= maxWidth && height >= maxHeight)
        //            {

        //                int newWidth;
        //                int newHeight;

        //                if (width > height)
        //                {
        //                    newHeight = height * (maxWidth / width);
        //                    newWidth = maxWidth;
        //                }
        //                else
        //                {
        //                    newWidth = width * (maxHeight / height);
        //                    newHeight = maxHeight;
        //                }

        //                // dummy resize options
        //                //int width = 50;
        //                //int height = 100;
        //                IResampler sampler = KnownResamplers.MitchellNetravali;
        //                bool compand = true;
        //                ResizeMode mode = ResizeMode.BoxPad;

        //                // init resize object
        //                var resizeOptions = new ResizeOptions
        //                {
        //                    Size = new Size(newWidth, newHeight),
        //                    Sampler = sampler,
        //                    Compand = compand,
        //                    Mode = mode
        //                };

        //                // mutate image
        //                image.Mutate(x => x
        //                     .Resize(resizeOptions));

        //                //var afterMutations = image.Size();
        //            }

        //            //Encode here for quality
        //            var encoder = new JpegEncoder()
        //            {
        //                Quality = 75 //Use variable to set between 5-30 based on your requirements
        //            };

        //            //This saves to the memoryStream with encoder
        //            image.Save(memoryStream, encoder);
        //            memoryStream.Position = 0; // The position needs to be reset.

        //            // prepare result to byte[]
        //            result = memoryStream.ToArray();

        //            //var after = memoryStream.Length; // kind of not needed.

        //            return result;
        //        }

        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}
    }
}
