using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Drawing;
using System.Net;
using System.Text;
using System.Web;
using WormCat.Library.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace WormCat.Library.Utility
{
    public class EnrichedContentProviderGoogle : IEnrichedContentProvider
    {
        private readonly ILogger<EnrichedContentProviderGoogle> _logger;
        private readonly IConfiguration _config;
        private readonly IRecordUtility _recordUtility;

        public string DisplayName => "Google Books";

        public EnrichedContentProviderGoogle(ILogger<EnrichedContentProviderGoogle> logger, IConfiguration config, IRecordUtility recordUtility)
        {
            _logger = logger;
            _config = config;
            _recordUtility = recordUtility;
        }

        private EnrichedContentModel? PullEnrichedContent(string query)
        {
            try
            {
                EnrichedContentModel model = new EnrichedContentModel();
                string uri = $"https://www.googleapis.com/books/v1/volumes?q={query}&key={_config["ApiKeys:GoogleBooks"]}&fields=items(volumeInfo/title),items(volumeInfo/authors),items(volumeInfo/publishedDate),items(volumeInfo/pageCount),items(searchInfo/textSnippet),items(volumeInfo/imageLinks/thumbnail)";
                string result = string.Empty;

                HttpWebRequest request = WebRequest.CreateHttp(uri);
                request.Accept = "application/json";
                request.Method = "GET";

                _logger.LogInformation($"HttpWebRequest -\nRequest\n{JsonConvert.SerializeObject(request, Formatting.Indented)}\n\nResult\n{result}");


                using (HttpWebResponse httpResponse = (HttpWebResponse)request.GetResponse())
                {
                    // Read the response to a string
                    using (StreamReader sr = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        result = sr.ReadToEnd();

                        dynamic dynamicResult = JsonConvert.DeserializeObject<dynamic>(result);

                        JArray array = dynamicResult?.items as JArray ?? new JArray();

                        foreach (JToken item in array)
                        {
                            JToken volumeInfo = item["volumeInfo"];
                            JToken searchInfo = item["searchInfo"];

                            model.Title = volumeInfo?["title"]?.Value<string>() ?? string.Empty;

                            foreach (JToken author in volumeInfo?["authors"] ?? new JArray())
                            {
                                model.Author = author.Value<string>() ?? string.Empty;

                                // Break after first author, as we only store one.
                                if (string.IsNullOrWhiteSpace(model.Author) == false)
                                    break;
                            }

                            model.PublicationDate = volumeInfo?["publishedDate"]?.Value<string>() ?? string.Empty;
                            model.PageCount = volumeInfo?["pageCount"]?.Value<int>() ?? 0;
                            model.Synopsis = HttpUtility.HtmlDecode(searchInfo?["textSnippet"]?.Value<string>() ?? string.Empty);
                            model.Image = volumeInfo?["imageLinks"]?["thumbnail"].Value<string>() ?? string.Empty;

                            // Break after first item, as google books api returns all volumes available
                            break;
                        }
                    }

                    _logger.LogInformation($"HttpWebRequest -\nRequest\n{request}\n\nResponse\n{httpResponse}\n\nResult\n{result}");
                    return model;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }

        private List<EnrichedContentModel> PullAllEnrichedContent(string query)
        {
            List<EnrichedContentModel> models = new List<EnrichedContentModel>();

            try
            {
                string uri = $"https://www.googleapis.com/books/v1/volumes?q={query}&key={_config["ApiKeys:GoogleBooks"]}&fields=items(volumeInfo/title),items(volumeInfo/authors),items(volumeInfo/publishedDate),items(volumeInfo/pageCount),items(searchInfo/textSnippet),items(volumeInfo/imageLinks/thumbnail)";
                string result = string.Empty;

                HttpWebRequest request = WebRequest.CreateHttp(uri);
                request.Accept = "application/json";
                request.Method = "GET";

                _logger.LogInformation($"HttpWebRequest -\nRequest\n{JsonConvert.SerializeObject(request, Formatting.Indented)}\n\nResult\n{result}");


                using (HttpWebResponse httpResponse = (HttpWebResponse)request.GetResponse())
                {
                    // Read the response to a string
                    using (StreamReader sr = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        result = sr.ReadToEnd();

                        dynamic dynamicResult = JsonConvert.DeserializeObject<dynamic>(result);

                        JArray array = dynamicResult?.items as JArray ?? new JArray();

                        foreach (JToken item in array)
                        {
                            EnrichedContentModel model = new EnrichedContentModel();

                            JToken volumeInfo = item["volumeInfo"];
                            JToken searchInfo = item["searchInfo"];

                            model.Title = volumeInfo?["title"]?.Value<string>() ?? string.Empty;

                            foreach (JToken author in volumeInfo?["authors"] ?? new JArray())
                            {
                                model.Author = author.Value<string>() ?? string.Empty;

                                // Break after first author, as we only store one.
                                if (string.IsNullOrWhiteSpace(model.Author) == false)
                                    break;
                            }

                            model.PublicationDate = volumeInfo?["publishedDate"]?.Value<string>() ?? string.Empty;
                            model.PageCount = volumeInfo?["pageCount"]?.Value<int>() ?? 0;
                            model.Synopsis = HttpUtility.HtmlDecode(searchInfo?["textSnippet"]?.Value<string>() ?? string.Empty);
                            model.Image = volumeInfo?["imageLinks"]?["thumbnail"].Value<string>() ?? string.Empty;

                            models.Add(model);
                        }
                    }

                    _logger.LogInformation($"HttpWebRequest -\nRequest\n{request}\n\nResponse\n{httpResponse}\n\nResult\n{result}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }

            return models;
        }

        private byte[]? DownloadImageUrl(string url)
        {
            try
            {
                byte[]? coverImage = null;

                WebClient webClient = new WebClient();
                coverImage = webClient.DownloadData(url);

                return coverImage;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }

            return null;
        }

        public EnrichedContentModel? GetEnrichedContent(string isbn)
        {
            try
            {
                EnrichedContentModel? model = PullEnrichedContent($"isbn:{isbn}");

                if (model == null) throw new Exception($"No book found with isbn {isbn}");

                string initialImage = model.Image;
                model.Image = string.Empty;

                string query = "";

                if (string.IsNullOrWhiteSpace(model.Title) == false)
                    query += $"intitle:{model.Title}+";

                if (string.IsNullOrWhiteSpace(model.Author) == false)
                    query += $"inauthor:{model.Author}+";

                List<EnrichedContentModel> tmpModels = PullAllEnrichedContent(query);

                if (string.IsNullOrWhiteSpace(initialImage) == false)
                    tmpModels.Add(new EnrichedContentModel { Image = initialImage });

                foreach (EnrichedContentModel tmpModel in tmpModels)
                {
                    // Break once we have downloaded an image
                    if (string.IsNullOrWhiteSpace(model.Image) == false) break;

                    // Iterate if no image found in tmp model
                    if (string.IsNullOrWhiteSpace(tmpModel.Image)) continue;

                    tmpModel.Image = tmpModel.Image.Replace("zoom=1", "zoom=2").Replace("&edge=curl", "");

                    // Download the tmp image url
                    byte[]? bytes = DownloadImageUrl(tmpModel.Image);

                    // If the image is considered "valid", store it in the response model image as a Base64 string
                    if (bytes != null && bytes.Length > 100)
                    {
                        model.Image = System.Convert.ToBase64String(bytes);

                        _logger.LogInformation($"Found image link for isbn {isbn}\n{tmpModel.Image}");
                    }
                }

                return model;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }
    }
}
