using System.Security.Claims;
using System.Security.Principal;

namespace WormCat.Library.Services
{
    public interface IAuthDisplayService
    {
        string CSSAuthColour { get; }
        string CSSAuthDisabled { get; }
        string HTMLAuthIcon { get; }

        ClaimsPrincipal? User { get; }
        IIdentity? Identity { get; }
        bool IsAuthenticated { get; }

        string GetOAuthDisplayIcon(string idp);
    }
}
