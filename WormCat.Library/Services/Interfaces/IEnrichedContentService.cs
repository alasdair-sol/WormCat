using WormCat.Library.Models;

namespace WormCat.Library.Services.Interfaces
{
    public interface IEnrichedContentService
    {
        public string DisplayName { get; }
        public Task<EnrichedContentModel?> GetEnrichedContentAsync(string isbn);
    }
}
