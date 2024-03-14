using WormCat.Library.Models.Dbo;

namespace WormCat.Data.DataAccess.Interfaces
{
    public interface IBookAccess : IAccessSaveable
    {
        Task<Book> CreateNewAsync(int recordId, int containerId);
        Task<Book?> CreateNewForRecordAsync(string userId, int recordId);
        Task<Book?> GetAsync(int? id);
    }
}
