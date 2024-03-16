using WormCat.Library.Models.Dbo;

namespace WormCat.Data.DataAccess.Interfaces
{
    public interface IContainerAccess : IAccessSaveable
    {
        Task<Container?> GetAsync(int? id);
        Task<Container?> GetFirstOrDefaultForUserAsync(string userId);
        Task<List<Container>> GetAllForUserAsync(string userId, bool includeGroups);
        Task<Container?> CreateNewAsync(Container container);
    }
}