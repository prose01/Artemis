using Artemis.Model;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Artemis.Interfaces
{
    public interface IHelperMethods
    {
        Task<CurrentUser> GetCurrentUserProfile(ClaimsPrincipal user);
    }
}
