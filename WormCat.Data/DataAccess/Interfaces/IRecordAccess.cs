using WormCat.Library.Models.Dbo;

namespace WormCat.Data.DataAccess.Interfaces
{
    public interface IRecordAccess : IAccessSaveable
    {
        Task<Record?> CreateNewAsync(string userId, Record record);
        Task<List<Record>> GetAllForUserAsync(string userId, bool includeGroups = false);
        Task<Record?> GetAsync(int? id);
    }
}