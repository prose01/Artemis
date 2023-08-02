using Artemis.Interfaces;
using Artemis.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Artemis.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class ImageUtilController : Controller
    {
        private readonly IHelperMethods _helper;
        private readonly IImageUtil _imageUtil;
        private readonly long _maxImageNumber;
        private readonly long _fileSizeLimit;

        public ImageUtilController(IConfiguration config, IHelperMethods helperMethods, IImageUtil imageUtil)
        {
            _fileSizeLimit = config.GetValue<long>("FileSizeLimit");
            _maxImageNumber = config.GetValue<long>("MaxImageNumber");
            _helper = helperMethods;
            _imageUtil = imageUtil;
        }

        #region CurrentUser

        /// <summary>Upload images to the profile image folder.</summary>
        /// <param name="image"></param>
        /// <param name="title"></param>
        /// <exception cref="ArgumentException">User has exceeded maximum number of images. {currentUser.Images.Count}</exception>
        [HttpPost("~/UploadImage")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> UploadImage([FromForm] IFormFile image, [FromForm] string title)
        {
            try
            {
                var currentUser = await _helper.GetCurrentUserProfile(User);

                if (currentUser == null || currentUser.Name == null)
                {
                    return NotFound();
                }

                if (currentUser.Images.Count >= _maxImageNumber) throw new ArgumentException($"User has exceeded maximum number of images.", nameof(currentUser.Images.Count));

                if (image.Length == 0)
                {
                    return BadRequest($"Image is empty.");
                }

                if (image.Length < 0 || image.Length > _fileSizeLimit)
                {
                    var megabyteSizeLimit = (_fileSizeLimit / 1048576);
                    return BadRequest($"Image has exceeded the maximum size of {megabyteSizeLimit:N1} MB.");
                }

                //if (!IsValidFileExtensionAndSignature) // TODO: Add checks for file extensions https://learn.microsoft.com/en-us/azure/security/develop/threat-modeling-tool-input-validation#controls-users & https://github.com/dotnet/AspNetCore.Docs/blob/main/aspnetcore/mvc/models/file-uploads/samples/3.x/SampleApp/Utilities/FileHelpers.cs
                //{
                //    return BadRequest($"Image type isn't permitted or the file's signature doesn't match the file's extension.");
                //}

                await _imageUtil.AddImageToCurrentUser(currentUser, image, title);

                return NoContent();
            }
            catch (Exception ex)
            {
                return Problem(ex.ToString());
            }
        }

        /// <summary>Deletes the image for current user.</summary>
        /// <param name="imageId">The image identifier.</param>
        /// <returns></returns>
        [HttpPost("~/DeleteImagesForCurrentUser")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> DeleteImagesForCurrentUser([FromBody] string[] imageIds)
        {
            try
            {
                var currentUser = await _helper.GetCurrentUserProfile(User);

                if (currentUser == null || currentUser.Name == null)
                {
                    return NotFound();
                }

                foreach (var imageId in imageIds)
                {
                    if (!currentUser.Images.Any(i => i.ImageId == imageId)) return BadRequest();
                }

                await _imageUtil.DeleteImagesForCurrentUser(currentUser, imageIds);

                return NoContent();
            }
            catch (Exception ex)
            {
                return Problem(ex.ToString());
            }
        }

        ///// <summary>Gets an images from CurrentUser by Image fileName.</summary>
        ///// <param name="fileName">The image fileName.</param>
        ///// <returns></returns>
        //[HttpGet("~/GetImageByFileName/{fileName}")]
        //public async Task<IActionResult> GetImageByFileName(string fileName)
        //{
        //    try
        //    {
        //        var currentUser = await _helper.GetCurrentUserProfile(User);

        //        if (!currentUser.Images.Any(i => i.FileName == fileName)) return BadRequest();

        //        return Ok(await _imageUtil.GetImageByFileName(currentUser.ProfileId, fileName));
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        #endregion

        #region Profile

        ///// <summary>Gets all images from specified profileId.</summary>
        ///// <param name="profileId">The profile identifier.</param>
        ///// <param name="imageSize">The size of image.</param>
        ///// <returns></returns>
        //[HttpGet("~/GetProfileImages/{profileId},{imageSize}")]
        //[ProducesResponseType((int)HttpStatusCode.OK)]
        //[ProducesResponseType((int)HttpStatusCode.BadRequest)]
        //[ProducesResponseType((int)HttpStatusCode.NotFound)]
        //public async Task<IActionResult> GetProfileImages(string profileId, ImageSizeEnum imageSize)
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(profileId)) return BadRequest();

        //        return Ok(await _imageUtil.GetImagesAsync(profileId, imageSize));
        //    }
        //    catch (Exception ex)
        //    {
        //        return Problem(ex.ToString());
        //    }
        //}

        ///// <summary>Gets an images from Profile by Image fileName.</summary>
        ///// <param name="profileId">The profile identifier.</param>
        ///// <param name="fileName">The image fileName.</param>
        ///// <param name="imageSize">The size of image.</param>
        ///// <returns></returns>
        //[HttpGet("~/GetProfileImageByFileName/{profileId},{fileName},{imageSize}")]
        //[ProducesResponseType((int)HttpStatusCode.OK)]
        //[ProducesResponseType((int)HttpStatusCode.BadRequest)]
        //[ProducesResponseType((int)HttpStatusCode.NotFound)]
        //public async Task<IActionResult> GetProfileImageByFileName(string profileId, string fileName, ImageSizeEnum imageSize)
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(profileId)) return BadRequest();
        //        if (string.IsNullOrEmpty(fileName)) return BadRequest();

        //        return Ok(await _imageUtil.GetImageByFileName(profileId, fileName, imageSize));
        //    }
        //    catch (Exception ex)
        //    {
        //        return Problem(ex.ToString());
        //    }
        //}

        #endregion

        #region Admin methods.

        /// <summary>Deletes all images for profile. There is no going back!</summary>
        /// <param name="profileIds">The profile identifiers.</param>
        /// <exception cref="Exception">You don't have admin rights to delete other people's images.</exception>
        [HttpPost("~/DeleteAllImagesForProfile")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> DeleteAllImagesForProfile([FromBody] string[] profileIds)
        {
            try
            {
                var currentUser = await _helper.GetCurrentUserProfile(User);

                if (currentUser == null || currentUser.Name == null)
                {
                    return NotFound();
                }

                if (!currentUser.Admin) throw new Exception("You don't have admin rights to delete other people's images.");

                foreach (var profileId in profileIds)
                {
                    await _imageUtil.DeleteAllImagesForProfile(currentUser, profileId);
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return Problem(ex.ToString());
            }
        }

        /// <summary>Deletes all images for CurrentUser. There is no going back!</summary>
        [HttpPost("~/DeleteAllImagesForCurrentUser")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> DeleteAllImagesForCurrentUser()
        {
            try
            {
                var currentUser = await _helper.GetCurrentUserProfile(User);

                if (currentUser == null || currentUser.Name == null)
                {
                    return NotFound();
                }

                if (currentUser.Admin) return BadRequest(); // Admins cannot delete themseleves.

                if (currentUser.ProfileId == null) return BadRequest();

                await _imageUtil.DeleteAllImagesForCurrentUser(currentUser);

                return NoContent();
            }
            catch (Exception ex)
            {
                return Problem(ex.ToString());
            }
        }

        #endregion
    }
}
