using System.Security.Claims;

namespace WormCat.Razor.Utility
{
    public static class AuthExtensions
    {
        public static T GetUserId<T>(this ClaimsPrincipal principal)
        {
            try
            {
                if (principal == null)
                    throw new ArgumentNullException(nameof(principal));

                string? userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);

                if (userId == null)
                    throw new Exception($"User with claim NameIdentifier not found");

                if (typeof(T) == typeof(string))
                {
                    return (T)Convert.ChangeType(userId, typeof(T))!;
                }
                else if (typeof(T) == typeof(int) || typeof(T) == typeof(long))
                {
                    return userId != null ? (T)Convert.ChangeType(userId, typeof(T)) : (T)Convert.ChangeType(0, typeof(T));
                }
                else
                {
                    throw new Exception("Invalid type provided");
                }
            }
            catch(Exception ex)
            {
                return default(T);
            }
        }
    }
}
