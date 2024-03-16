using WormCat.Library.Models.Dbo;

namespace WormCat.Data.DataAccess.Interfaces
{
    public interface IUserAccess
    {
        Task<User?> TryCreateNewAsync(string id, string customUsername, string email);
        Task<User?> GetUserById(string id);
        Task<User?> FindUserByCustomUsernameOrEmailAddressOrId(string? value);
        Task<string?> GetUsernameByUserId(string id);
    }
}