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
                    Projection = this.GetProjection(),
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
        public async Task AddImageToCurrentUser(CurrentUser currentUser, string fileName, string title)
        {
            try
            {
                var imageModel = new ImageModel() { ImageId = Guid.NewGuid().ToString(), FileName = fileName, Title = title == "null" ? null : title  };

                var filter = Builders<CurrentUser>
                                .Filter.Eq(e => e.ProfileId, currentUser.ProfileId);

                var update = Builders<CurrentUser>
                                .Update.Push(e => e.Images, imageModel)
                                .Set(e => e.UpdatedOn, DateTime.Now);

                await _context.CurrentUser.UpdateOneAsync(filter, update);
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
        public async Task RemoveImageFromCurrentUser(CurrentUser currentUser, string imageId)
        {
            try
            {
                var images = currentUser.Images.Where(i => i.ImageId == imageId).ToList();

                var filter = Builders<CurrentUser>
                                .Filter.Eq(e => e.ProfileId, currentUser.ProfileId);

                var update = Builders<CurrentUser>
                                .Update.PullAll(e => e.Images, images)
                                .Set(e => e.UpdatedOn, DateTime.Now);

                await _context.CurrentUser.UpdateOneAsync(filter, update);
            }
            catch
            {
                throw;
            }
        }

        private ProjectionDefinition<CurrentUser> GetProjection()
        {
            ProjectionDefinition<CurrentUser> projection = "{ " +
                "_id: 0, " +
                "Auth0Id: 0, " +
                "Avatar: 0, " +
                "CreatedOn: 0, " +
                "Countrycode: 0, " +
                "Age: 0, " +
                "Height: 0, " +
                "Contactable: 0, " +
                "Description: 0, " +
                "Tags: 0, " +
                "Body: 0, " +
                "SmokingHabits: 0, " +
                "HasChildren: 0, " +
                "WantChildren: 0, " +
                "HasPets: 0, " +
                "LivesIn: 0, " +
                "Education: 0, " +
                "EducationStatus: 0, " +
                "EmploymentStatus: 0, " +
                "SportsActivity: 0, " +
                "EatingHabits: 0, " +
                "ClotheStyle: 0, " +
                "BodyArt: 0, " +
                "Gender: 0, " +
                "Seeking: 0, " +
                "Bookmarks: 0, " +
                "ProfileFilter: 0, " +
                "Languagecode: 0, " +
                "Visited: 0, " +
                "Likes: 0, " +
                "Groups: 0, " +
                "Complains: 0, " +
                "ChatMemberslist: 0, " +
                "}";

            return projection;
        }
    }
}
