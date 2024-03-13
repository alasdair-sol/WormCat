using WormCat.Library.Models;

namespace WormCat.Library.Services
{
    public interface IEnrichedContentService
    {
        public string DisplayName { get; }
        public EnrichedContentModel? GetEnrichedContent(string isbn);
    }
}
