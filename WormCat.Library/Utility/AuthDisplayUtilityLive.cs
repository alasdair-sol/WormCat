using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Security.Principal;

namespace WormCat.Library.Utility
{
    public class AuthDisplayUtilityLive : IAuthDisplayUtility
    {
        public AuthDisplayUtilityLive(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private IHttpContextAccessor _httpContextAccessor { get; }

        public ClaimsPrincipal? User => _httpContextAccessor?.HttpContext?.User ?? null;

        public IIdentity? Identity => User?.Identity ?? null;

        public bool IsAuthenticated => Identity?.IsAuthenticated ?? false;

        public string HTMLAuthIcon => IsAuthenticated == false ? "<i class=\"fa-solid fa-lock\"/>" : string.Empty;
        public string CSSAuthColour => IsAuthenticated == false ? "danger" : "primary";
        public string CSSAuthDisabled => IsAuthenticated == false ? "disabled" : string.Empty;

    }
}
