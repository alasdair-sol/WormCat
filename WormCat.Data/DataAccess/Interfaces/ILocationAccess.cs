using WormCat.Library.Models;
using WormCat.Library.Models.Dbo;

namespace WormCat.Data.DataAccess.Interfaces
{
    public interface ILocationAccess : IAccessSaveable
    {
        Task<TaskResponseErrorCode<Location?>> CreateNewAsync(string? userId, Location location);
        Task<TaskResponseErrorCode<bool>> DeleteLocationAsync(string? userId, int? id);
        Task<List<Location>> GetAllForUserAsync(string userId, bool includeGroups);
        Task<Location?> GetAsync(int? id);
    }
}