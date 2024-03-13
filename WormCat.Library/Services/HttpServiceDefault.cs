namespace WormCat.Library.Services
{
    public class HttpServiceDefault
    {
        private readonly HttpClient _httpClient;

        public HttpServiceDefault(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<HttpResponseMessage?> GetAsync(string uri)
        {
            HttpResponseMessage? result = await _httpClient.GetAsync(uri) ?? null;
            return result;
        }

        public async Task<string?> GetAsStringAsync(string uri)
        {
            string? result = await _httpClient.GetStringAsync(uri) ?? null;
            return result;
        }

        public async Task<byte[]?> GetAsByteArrayAsync(string uri)
        {
            byte[]? result = await _httpClient.GetByteArrayAsync(uri) ?? null;
            return result;
        }
    }
}
