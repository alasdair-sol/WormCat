namespace WormCat.Library.Services.Interfaces
{
    /// <summary>
    /// An interface that assists in error code handling
    /// Helpful to stop passing error messages as query strings when syncing data between razor pages
    /// </summary>
    public interface IErrorCodeService
    {
        string GetErrorMessage(int? errorCode);
    }
}
