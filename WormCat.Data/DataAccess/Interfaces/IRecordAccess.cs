using Microsoft.AspNetCore.Mvc.Rendering;
using WormCat.Library.Models;
using WormCat.Library.Models.Dbo;

namespace WormCat.Data.DataAccess.Interfaces
{
    public interface IRecordAccess : IAccessSaveable
    {
        Task<TaskResponseErrorCode<Record?>> CreateNewAsyncWithDefaultBook(int containerIdForDefaultBook, Record record);
        Task<TaskResponseErrorCode<Record?>> CreateNewAsyncWithoutDefaultBook(string? userId, Record record);
        Task<List<Record>> Search(string userId, string? query, string? sort = null, string? groupFilter = null);
        Task<Record?> GetAsync(int? id);
        Task<List<SelectListItem>> GetSortOptions();
        Task<List<SelectListItem>> GetGroupFilterOptions(string userId);
        Task<Record?> SearchForIsbn(string? userId, string query);
    }
}