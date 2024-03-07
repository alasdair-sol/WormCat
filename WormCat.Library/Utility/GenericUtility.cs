using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;


namespace WormCat.Library.Utility
{
    public class GenericUtility : IGenericUtility
    {
        public IEnumerable<SelectListItem> GetSelectList<T>(DbSet<T> dbSet, int? targetId, Func<T, string> textFunc, Func<T, string> idFunc) where T : class
        {
            IEnumerable<SelectListItem> list = dbSet
                   .AsEnumerable()
                   .Select(s => new SelectListItem
                   {
                       Selected = false,
                       Text = textFunc(s),
                       Value = idFunc(s)
                   }).ToList();

            if (targetId == null) return list;

            foreach (var item in list)
            {
                if (item.Value == targetId.ToString())
                    item.Selected = true;
            }

            return list;
        }
    }
}
