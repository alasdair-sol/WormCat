using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace WormCat.Razor.Areas.Identity.Data;

// Add profile data for application users by adding properties to the WormCatUser class
[Index(nameof(CustomUsername), IsUnique = true)]
public class WormCatUser : IdentityUser
{
    /// <summary>
    /// A custom username for the user.
    /// NOTE: This is different from IdentityUser.UserName/IdentityUser.NormalizedUserName (which is usually the email address of this user)
    /// </summary>
    public string? CustomUsername { get; set; }
}

