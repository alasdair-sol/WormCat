using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WormCat.Razor.Areas.Identity.Data;

namespace WormCat.Razor.Utility
{
    public static class AuthUtility
    {
        public static async Task<bool> CustomUsernameTaken(UserManager<WormCatUser> userManager, string customUsername)
        {
            try
            {
                WormCatUser? wormCatUser = await userManager.Users.FirstOrDefaultAsync(x => x.CustomUsername == customUsername);

                if (wormCatUser == null) return false;
            }
            catch { }

            return true;
        }
    }
}
