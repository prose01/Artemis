using Artemis.Model;
using System.Threading.Tasks;

namespace Artemis.Interfaces
{
    public interface ICurrentUserRepository
    {
        Task<CurrentUser> GetCurrentProfileByAuth0Id(string auth0Id);
        Task AddImageToCurrentUser(CurrentUser currentUser, string fileName, string title);
        Task RemoveImageFromCurrentUser(CurrentUser currentUser, string id);
    }
}
