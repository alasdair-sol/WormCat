namespace WormCat.Library.Models.GoogleBooks
{
    public class GoogleBooksVolumeInfo
    {
        public string? Title { get; set; }
        public string[]? Authors { get; set; }
        public string? PublishedDate { get; set; }
        public int? PageCount { get; set; }
        public GoogleBooksImageLinks? ImageLinks { get; set; }
    }
}
