using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using WormCat.Library.Models.GoogleBooks;

namespace WormCat.Library.Services
{
    public class HttpServiceGoogleBooks
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        private string queryFormat = "?q={0}&key={1}&fields=items(volumeInfo/title),items(volumeInfo/authors),items(volumeInfo/publishedDate),items(volumeInfo/pageCount),items(searchInfo/textSnippet),items(volumeInfo/imageLinks/thumbnail)";

        public HttpServiceGoogleBooks(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;

            _httpClient.BaseAddress = new Uri(configuration["Endpoints:GoogleBooksApi"] ?? string.Empty);
        }

        public string GetFullQueryString(string query)
        {
            return string.Format(queryFormat, query, _configuration["ApiKeys:GoogleBooks"]);
        }

        public async Task<GoogleBooksVolumeCollectionModel> GetGoogleBooksVolume(string query)
        {
            var result = await _httpClient.GetFromJsonAsync<GoogleBooksVolumeCollectionModel>(GetFullQueryString(query)) ?? new GoogleBooksVolumeCollectionModel();
            return result;
        }
    }
}
