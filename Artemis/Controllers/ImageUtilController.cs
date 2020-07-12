﻿using Artemis.Interfaces;
using Artemis.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Artemis.Controllers
{
    [Route("api/[controller]")]
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

        /// <summary>Upload images to the profile image folder.</summary>
        /// <param name="image"></param>
        /// <param name="title"></param>
        /// <exception cref="ArgumentException">ModelState is not valid {ModelState.IsValid}. - image</exception>
        /// <exception cref="ArgumentException">Image length is < 1 {image.Length}. - image</exception>
        [HttpPost("~/UploadImage")]
        public async Task<IActionResult> UploadImage([FromForm] IFormFile image, [FromForm] string title)
        {
            if (!ModelState.IsValid) throw new ArgumentException($"ModelState is not valid {ModelState.IsValid}.", nameof(image));
            if (image.Length < 0) throw new ArgumentException($"Image length is < 1 {image.Length}.", nameof(image));

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
            if (!ModelState.IsValid) throw new ArgumentException($"ModelState is not valid {ModelState.IsValid}.", nameof(imageIds));

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

        #region Admin methods.

        /// <summary>Deletes all images for profile. There is no going back!</summary>
        /// <param name="currentUser">The CurrentUser.</param>
        /// <param name="profileId">The profile identifier.</param>
        /// <exception cref="Exception">You don't have admin rights to delete other people's images.</exception>
        public IActionResult DeleteAllImagesForProfile(CurrentUser currentUser, string profileId)
        {
            if (!currentUser.Admin) throw new Exception("You don't have admin rights to delete other people's images.");

            try
            {
                _imageUtil.DeleteAllImagesForProfile(currentUser, profileId);
                return Ok();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>Deletes all images for CurrentUser. There is no going back!</summary>
        /// <param name="currentUser">The CurrentUser.</param>
        public IActionResult DeleteAllImagesForCurrentUser(CurrentUser currentUser)
        {
            try
            {
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
