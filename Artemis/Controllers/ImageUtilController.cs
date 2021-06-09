using Artemis.Interfaces;
using Artemis.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
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

        public ImageUtilController(IConfiguration config, IHelperMethods helperMethods, IImageUtil imageUtil)
        {
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
        [ProducesResponseType(204)]
        public async Task<IActionResult> UploadImage([FromForm] IFormFile image, [FromForm] string title)
        {
            try
            {
                var currentUser = await _helper.GetCurrentUserProfile(User);

                if (currentUser.Images.Count >= _maxImageNumber) throw new ArgumentException($"User has exceeded maximum number of images.", nameof(currentUser.Images.Count));

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
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> DeleteImagesForCurrentUser([FromBody] string[] imageIds)
        {
            try
            {
                var currentUser = await _helper.GetCurrentUserProfile(User);

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

        /// <summary>Gets an images from CurrentUser by Image fileName.</summary>
        /// <param name="fileName">The image fileName.</param>
        /// <returns></returns>
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

        /// <summary>Gets all images from specified profileId.</summary>
        /// <param name="profileId">The profile identifier.</param>
        /// <param name="imageSize">The size of image.</param>
        /// <returns></returns>
        [HttpGet("~/GetProfileImages/{profileId},{imageSize}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetProfileImages(string profileId, ImageSizeEnum imageSize)
        {
            try
            {
                if (string.IsNullOrEmpty(profileId)) return BadRequest();

                return Ok(await _imageUtil.GetImagesAsync(profileId, imageSize));
            }
            catch (Exception ex)
            {
                return Problem(ex.ToString());
            }
        }

        /// <summary>Gets an images from Profile by Image fileName.</summary>
        /// <param name="profileId">The profile identifier.</param>
        /// <param name="fileName">The image fileName.</param>
        /// <param name="imageSize">The size of image.</param>
        /// <returns></returns>
        [HttpGet("~/GetProfileImageByFileName/{profileId},{fileName},{imageSize}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetProfileImageByFileName(string profileId, string fileName, ImageSizeEnum imageSize)
        {
            try
            {
                if (string.IsNullOrEmpty(profileId)) return BadRequest();
                if (string.IsNullOrEmpty(fileName)) return BadRequest();

                return Ok(await _imageUtil.GetImageByFileName(profileId, fileName, imageSize));
            }
            catch (Exception ex)
            {
                return Problem(ex.ToString());
            }
        }

        #endregion

        #region Admin methods.

        /// <summary>Deletes all images for profile. There is no going back!</summary>
        /// <param name="profileIds">The profile identifiers.</param>
        /// <exception cref="Exception">You don't have admin rights to delete other people's images.</exception>
        [HttpPost("~/DeleteAllImagesForProfile")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> DeleteAllImagesForProfile([FromBody] string[] profileIds)
        {
            try
            {
                var currentUser = await _helper.GetCurrentUserProfile(User);

                if (!currentUser.Admin) throw new Exception("You don't have admin rights to delete other people's images.");

                foreach (var profileId in profileIds)
                {
                    _imageUtil.DeleteAllImagesForProfile(currentUser, profileId);
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
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> DeleteAllImagesForCurrentUser()
        {
            try
            {
                var currentUser = await _helper.GetCurrentUserProfile(User);

                if (currentUser.Admin) return BadRequest(); // Admins cannot delete themseleves.

                _imageUtil.DeleteAllImagesForCurrentUser(currentUser);

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
