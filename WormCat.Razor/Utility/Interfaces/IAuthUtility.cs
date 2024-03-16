using Microsoft.AspNetCore.Identity;
using WormCat.Razor.Areas.Identity.Data;

namespace WormCat.Razor.Utility
{
    public interface IAuthUtility
    {
        Task<bool> CustomUsernameTakenAsync(UserManager<WormCatUser> userManager, string customUsername);
        Task<string?> GetCustomUsernameByIdAsync(string userId);
    }
}
