using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Drawing;
using System.Net;
using System.Text;
using System.Text.Encodings.Web;
using System.Web;
using WormCat.Library.Models;
using WormCat.Library.Models.GoogleBooks;
using WormCat.Library.Services.Interfaces;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace WormCat.Library.Services
{
    public class EnrichedContentServiceGoogle : IEnrichedContentService
    {
        private readonly ILogger<EnrichedContentServiceGoogle> _logger;
        private readonly HttpServiceDefault _httpServiceDefault;
        private readonly HttpServiceGoogleBooks _httpServiceGoogleBooks;

        public string DisplayName => "Google Books";

        public EnrichedContentServiceGoogle(ILogger<EnrichedContentServiceGoogle> logger, HttpServiceDefault httpServiceDefault, HttpServiceGoogleBooks httpServiceGoogleBooks)
        {
            _logger = logger;
            _httpServiceDefault = httpServiceDefault;
            _httpServiceGoogleBooks = httpServiceGoogleBooks;
        }

        /// <summary>
        /// Returns a list of enriched book content from Google Books Api
        /// </summary>
        /// <param name="isbn">The ISBN of the book to search for</param>
        public async Task<EnrichedContentModel?> GetEnrichedContentAsync(string isbn)
        {
            try
            {
                GoogleBooksVolumeCollectionModel googleBooksCollection = await _httpServiceGoogleBooks.GetGoogleBooksVolume($"isbn:{isbn}");

                EnrichedContentModel model = ExtractEnrichedContentModelFromVolumeInfo(googleBooksCollection);

                if (model == null) throw new Exception($"No book found with isbn {isbn}");

                model.Image = await ExtractImageFromEnrichedContent(model);

                return model;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Extracts all relevant information from a Google Books Api response, and populates an Enriched Content Model with the information.
        /// 
        /// Note: This takes in a collection of items, but only parses the first entry
        /// Note: This is because the Google Books Api will return fuzzy matches, and the further into the response you go, the less relevant the information is
        /// </summary>
        /// <param name="googleBooksCollection">A model depicting the response of a Google Books Api request</param>
        private EnrichedContentModel ExtractEnrichedContentModelFromVolumeInfo(GoogleBooksVolumeCollectionModel googleBooksCollection)
        {
            try
            {
                EnrichedContentModel model = new EnrichedContentModel();

                foreach (GoogleBooksVolumeModel volume in googleBooksCollection.Items!)
                {
                    if (volume == null)
                        continue;

                    model.Title = volume.VolumeInfo?.Title ?? string.Empty;
                    model.Author ??= volume.VolumeInfo?.Authors?.FirstOrDefault() ?? string.Empty;
                    model.PublicationDate ??= volume.VolumeInfo?.PublishedDate ?? string.Empty;
                    model.PageCount ??= volume.VolumeInfo?.PageCount ?? 0;
                    model.Synopsis ??= HttpUtility.HtmlDecode(volume.SearchInfo?.TextSnippet ?? string.Empty);
                    model.Image ??= volume.VolumeInfo?.ImageLinks?.Thumbnail ?? string.Empty;

                    // Break after first iteration as that will be the most relevant volume
                    // Other volumes after this may be incorrect books
                    break;
                }

                return model;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Tries to pull an image from an EnrichedContentModel object
        /// This method will first check the model.Image parameter
        /// If that does not return a valid response, it will then perform another request to Google Books Api based on author/title
        /// It will then check these for a valid image
        /// 
        /// Note: The model.Image should be a url (at this point) supplied by the Google Books Api        
        /// </summary>
        /// <param name="model">The model to base any future requests off</param>
        /// <returns>A Base64 encoded string of the first image that is found, or an empty string</returns>
        private async Task<string> ExtractImageFromEnrichedContent(EnrichedContentModel model)
        {
            try
            {
                byte[]? imageBytes = null;

                // Attempt to construct an image from the original value
                if (string.IsNullOrWhiteSpace(model.Image) == false)
                    imageBytes = await FetchImageBytes(model.Image);

                // If the original value is valid, return that
                if (imageBytes != null)
                    return Convert.ToBase64String(imageBytes);

                // Construct the query string to include book title
                string query = string.IsNullOrWhiteSpace(model.Title) ? string.Empty : $"intitle:{model.Title}+";

                // If available, include book author in query string
                if (string.IsNullOrWhiteSpace(model.Author) == false)
                    query += $"inauthor:{model.Author}+";

                // Fetch volumes that match query
                GoogleBooksVolumeCollectionModel googleBooksCollection = await _httpServiceGoogleBooks.GetGoogleBooksVolume(query);

                foreach (GoogleBooksVolumeModel item in googleBooksCollection.Items!)
                {
                    if (item == null)
                        continue;

                    if (string.IsNullOrWhiteSpace(item.VolumeInfo?.ImageLinks?.Thumbnail) == false)
                        imageBytes = await FetchImageBytes(item.VolumeInfo.ImageLinks.Thumbnail);

                    if (imageBytes != null)
                        return Convert.ToBase64String(imageBytes);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }

            return string.Empty;
        }

        /// <summary>
        /// Downloads an image at a given URL, and performs some data manipulation
        /// </summary>
        /// <param name="imageUrl">The URL to fetch the image from</param>
        /// <returns>A byte array containing the found image, or null</returns>
        private async Task<byte[]?> FetchImageBytes(string imageUrl)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(imageUrl))
                    throw new ArgumentNullException(nameof(imageUrl));

                imageUrl = imageUrl.Replace("zoom=1", "zoom=2").Replace("&edge=curl", "");

                byte[]? imageBytes = await _httpServiceDefault.GetAsByteArrayAsync(imageUrl);

                if (imageBytes != null && imageBytes.Length > 100)
                    return imageBytes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }

            return null;
        }
    }
}
