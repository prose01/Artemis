using Artemis.Interfaces;
using Artemis.Model;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Artemis.Helpers
{
    public class HelperMethods : IHelperMethods
    {
        private readonly ICurrentUserRepository _profileRepository;
        private readonly string _nameidentifier;

        public HelperMethods(IOptions<Settings> settings, ICurrentUserRepository profileRepository)
        {
            _nameidentifier = settings.Value.Auth0Id;
            _profileRepository = profileRepository;
        }

        /// <summary>Gets the current user profile.</summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<CurrentUser> GetCurrentUserProfile(ClaimsPrincipal user)
        {
            var auth0Id = user.Claims.FirstOrDefault(c => c.Type == _nameidentifier)?.Value;

            return await _profileRepository.GetCurrentProfileByAuth0Id(auth0Id);
        }
    }
}
