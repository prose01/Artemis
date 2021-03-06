﻿using Artemis.Interfaces;
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
        /// <exception cref="ArgumentException">ModelState is not valid {ModelState.IsValid}. - image</exception>
        /// <exception cref="ArgumentException">Image length is < 1 {image.Length}. - image</exception>
        [HttpPost("~/UploadImage")]
        public async Task<IActionResult> UploadImage([FromForm] IFormFile image, [FromForm] string title)
        {
            if (image.Length < 0) throw new ArgumentException($"Image length is < 1 {image.Length}.", nameof(image));
            if (string.IsNullOrEmpty(title)) throw new ArgumentException($"Image must have a title.", nameof(title));

            try
            {
                var currentUser = await _helper.GetCurrentUserProfile(User);

                if (currentUser.Images.Count >= _maxImageNumber) return BadRequest();

                return Ok(_imageUtil.AddImageToCurrentUser(currentUser, image, title));
            }
            catch (Exception ex)
            {
                return Problem(ex.ToString());
            }
        }

        /// <summary>Deletes the image from current user.</summary>
        /// <param name="imageId">The image identifier.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">ModelState is not valid {ModelState.IsValid}. - imageId</exception>
        [HttpPost("~/DeleteImage")]
        public async Task<IActionResult> DeleteImage([FromBody] string[] imageIds)
        {
            //if (!ModelState.IsValid) throw new ArgumentException($"ModelState is not valid {ModelState.IsValid}.", nameof(imageIds)); unnecessary 

            try
            {
                var currentUser = await _helper.GetCurrentUserProfile(User);

                foreach (var imageId in imageIds)
                {
                    if (!currentUser.Images.Any(i => i.ImageId != imageId)) return BadRequest();
                }

                return Ok(_imageUtil.DeleteImagesFromCurrentUser(currentUser, imageIds));
            }
            catch (Exception ex)
            {
                return Problem(ex.ToString());
            }
        }

        /// <summary>Gets an images from CurrentUser by Image fileName.</summary>
        /// <param name="fileName">The image fileName.</param>
        /// <returns></returns>
        [HttpGet("~/GetImageByFileName/{fileName}")]
        public async Task<IActionResult> GetImageByFileName(string fileName)
        {
            try
            {
                var currentUser = await _helper.GetCurrentUserProfile(User);

                if (!currentUser.Images.Any(i => i.FileName == fileName)) return BadRequest();

                return Ok(await _imageUtil.GetImageByFileName(currentUser.ProfileId, fileName));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Profile

        /// <summary>Gets all images from specified profileId.</summary>
        /// <param name="profileId">The profile identifier.</param>
        /// <returns></returns>
        [HttpGet("~/GetProfileImages/{profileId}")]
        public async Task<IActionResult> GetProfileImages(string profileId)
        {
            try
            {
                if (string.IsNullOrEmpty(profileId)) return BadRequest();

                return Ok(await _imageUtil.GetImagesAsync(profileId));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>Gets an images from Profile by Image fileName.</summary>
        /// <param name="profileId">The profile identifier.</param>
        /// <param name="fileName">The image fileName.</param>
        /// <returns></returns>
        [HttpGet("~/GetProfileImageByFileName/{profileId},{fileName}")]
        public async Task<IActionResult> GetProfileImageByFileName(string profileId, string fileName)
        {
            try
            {
                if (string.IsNullOrEmpty(profileId)) return BadRequest();
                if (string.IsNullOrEmpty(fileName)) return BadRequest();

                return Ok(await _imageUtil.GetImageByFileName(profileId, fileName));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Admin methods.

        /// <summary>Deletes all images for profile. There is no going back!</summary>
        /// <param name="profileIds">The profile identifiers.</param>
        /// <exception cref="Exception">You don't have admin rights to delete other people's images.</exception>
        [HttpPost("~/DeleteAllImagesForProfile")]
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

                return Ok();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>Deletes all images for CurrentUser. There is no going back!</summary>
        [HttpPost("~/DeleteAllImagesForCurrentUser")]
        public async Task<IActionResult> DeleteAllImagesForCurrentUser()
        {
            try
            {
                var currentUser = await _helper.GetCurrentUserProfile(User);

                if (currentUser.Admin) return BadRequest(); // Admins cannot delete themseleves.

                _imageUtil.DeleteAllImagesForCurrentUser(currentUser);
                return Ok();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}
