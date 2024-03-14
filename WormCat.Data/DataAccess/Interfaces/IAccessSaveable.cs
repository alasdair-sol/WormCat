namespace WormCat.Data.DataAccess.Interfaces
{
    public interface IAccessSaveable
    {
        Task<int?> SaveContextAsync();
    }
}