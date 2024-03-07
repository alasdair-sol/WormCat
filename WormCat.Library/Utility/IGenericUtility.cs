using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace WormCat.Library.Utility
{
    public interface IGenericUtility
    {
        IEnumerable<SelectListItem> GetSelectList<T>(DbSet<T> dbSet, int? targetId, Func<T, string> textFunc, Func<T, string> idFunc) where T : class;
    }
}