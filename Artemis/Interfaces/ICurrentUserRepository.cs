using Artemis.Model;
using System.Threading.Tasks;

namespace Artemis.Interfaces
{
    public interface ICurrentUserRepository
    {
        Task<CurrentUser> GetCurrentProfileByAuth0Id(string auth0Id);
        Task<CurrentUser> AddImageToCurrentUser(CurrentUser currentUser, string fileName, string title);
        Task<CurrentUser> RemoveImageFromCurrentUser(CurrentUser currentUser, string id);
    }
}
