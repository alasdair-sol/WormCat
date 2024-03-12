using WormCat.Library.Models;

namespace WormCat.Library.Utility
{
    public interface IEnrichedContentProvider
    {
        public string DisplayName { get; }
        public EnrichedContentModel? GetEnrichedContent(string isbn);
    }
}
