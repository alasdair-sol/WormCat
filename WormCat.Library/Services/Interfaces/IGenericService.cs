using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace WormCat.Library.Services.Interfaces
{
    public interface IGenericService
    {
        IEnumerable<SelectListItem> GetSelectList<T>(DbSet<T> dbSet, int? targetId, Func<T, string> textFunc, Func<T, string> idFunc) where T : class;
        IEnumerable<SelectListItem> GetSelectList<T>(List<T> dbSet, int? targetId, Func<T, string> textFunc, Func<T, string> idFunc) where T : class;
    }
}