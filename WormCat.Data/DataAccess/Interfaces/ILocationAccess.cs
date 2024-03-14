using WormCat.Library.Models.Dbo;

namespace WormCat.Data.DataAccess.Interfaces
{
    public interface ILocationAccess : IAccessSaveable
    {
        Task<Location?> CreateNewAsync(User? user, string locationName);
        Task<List<Location>> GetAllForUserAsync(string userId);
        Task<Location?> GetAsync(int? id);
    }
}