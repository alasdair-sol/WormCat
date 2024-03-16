using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WormCat.Razor.Areas.Identity.Data;

namespace WormCat.Razor.Utility
{
    public class AuthUtility : IAuthUtility
    {
        private readonly UserManager<WormCatUser> _userManager;

        public AuthUtility(UserManager<WormCatUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<bool> CustomUsernameTakenAsync(UserManager<WormCatUser> userManager, string customUsername)
        {
            try
            {
                WormCatUser? wormCatUser = await userManager.Users.FirstOrDefaultAsync(x => x.CustomUsername == customUsername);

                if (wormCatUser == null) return false;
            }
            catch { }

            return true;
        }

        public async Task<string?> GetCustomUsernameByIdAsync(string userId)
        {
            WormCatUser? wormCatUser = await _userManager.FindByIdAsync(userId);

            if (wormCatUser == null)
                return null;

            return wormCatUser.CustomUsername;
        }
    }
}
