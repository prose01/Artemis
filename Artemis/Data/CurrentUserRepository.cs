using Artemis.Interfaces;
using Artemis.Model;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Artemis.Data
{
    public class CurrentUserRepository : ICurrentUserRepository
    {
        private readonly ProfileContext _context = null;

        public CurrentUserRepository(IOptions<Settings> settings)
        {
            _context = new ProfileContext(settings);
        }

        /// <summary>Gets the current profile by Auth0Id.</summary>
        /// <param name="auth0Id">The Auth0Id.</param>
        /// <returns></returns>
        public async Task<CurrentUser> GetCurrentProfileByAuth0Id(string auth0Id)
        {
            try
            {
                var filter = Builders<CurrentUser>.Filter.Eq("Auth0Id", auth0Id);

                var update = Builders<CurrentUser>
                                .Update.Set(e => e.LastActive, DateTime.Now);

                var options = new FindOneAndUpdateOptions<CurrentUser>
                {
                    ReturnDocument = ReturnDocument.After
                };

                return await _context.CurrentUser.FindOneAndUpdateAsync(filter, update, options);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>Adds the image to profile.</summary>
        /// <param name="currentUser">The current user.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="title">The title.</param>
        /// <returns></returns>
        public async Task<CurrentUser> AddImageToCurrentUser(CurrentUser currentUser, string fileName, string title)
        {
            try
            {
                var imageModel = new ImageModel() { ImageId = Guid.NewGuid().ToString(), FileName = fileName, Title = title };

                var filter = Builders<CurrentUser>
                                .Filter.Eq(e => e.ProfileId, currentUser.ProfileId);

                var update = Builders<CurrentUser>
                                .Update.Push(e => e.Images, imageModel)
                                .Set(e => e.UpdatedOn, DateTime.Now);

                var options = new FindOneAndUpdateOptions<CurrentUser>
                {
                    ReturnDocument = ReturnDocument.After
                };

                return await _context.CurrentUser.FindOneAndUpdateAsync(filter, update, options);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>Removes the image from profile.</summary>
        /// <param name="currentUser">The current user.</param>
        /// <param name="imageId">The image identifier.</param>
        /// <returns></returns>
        public async Task<CurrentUser> RemoveImageFromCurrentUser(CurrentUser currentUser, string imageId)
        {
            try
            {
                var images = currentUser.Images.Where(i => i.ImageId == imageId).ToList();

                var filter = Builders<CurrentUser>
                                .Filter.Eq(e => e.ProfileId, currentUser.ProfileId);

                var update = Builders<CurrentUser>
                                .Update.PullAll(e => e.Images, images)
                                .Set(e => e.UpdatedOn, DateTime.Now);

                var options = new FindOneAndUpdateOptions<CurrentUser>
                {
                    ReturnDocument = ReturnDocument.After
                };

                return await _context.CurrentUser.FindOneAndUpdateAsync(filter, update, options);
            }
            catch
            {
                throw;
            }
        }
    }
}
