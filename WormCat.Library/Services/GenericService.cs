using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WormCat.Library.Services.Interfaces;


namespace WormCat.Library.Services
{
    public class GenericService : IGenericService
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

        public IEnumerable<SelectListItem> GetSelectList<T>(List<T> dbSet, int? targetId, Func<T, string> textFunc, Func<T, string> idFunc) where T : class
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
