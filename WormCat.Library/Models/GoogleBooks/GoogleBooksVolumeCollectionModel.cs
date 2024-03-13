namespace WormCat.Library.Models.GoogleBooks
{
    public class GoogleBooksVolumeCollectionModel
    {
        public IEnumerable<GoogleBooksVolumeModel>? Items { get; set; } = new List<GoogleBooksVolumeModel>();
    }
}
