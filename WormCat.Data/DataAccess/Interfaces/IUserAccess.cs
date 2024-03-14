using WormCat.Library.Models.Dbo;

namespace WormCat.Data.DataAccess.Interfaces
{
    public interface IUserAccess
    {
        Task<User?> TryCreateNewAsync(string id);
        Task<User?> GetAsync(string id);
    }
}