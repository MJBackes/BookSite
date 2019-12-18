using BookSite.Models.APIResponseModels;
using BookSite.Models.MiscModels;
using BookSite.Models.SiteModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BookSite.APIHandlers
{
    public class GoogleBooksAPIHandler
    {
        public static GoogleBooksSearchResponse NavBarSearch(string input)
        {
            string URI = $"https://www.googleapis.com/books/v1/volumes?q={input}&key={APIKeys.APIKey.GoogleBooks}";
            return VolumeSearch(URI).Result;
        }
        public static GoogleBooksSearchResponse FullSearch(Search input)
        {
            StringBuilder URI = new StringBuilder($"https://www.googleapis.com/books/v1/volumes?q=");
            if (input.other != null)
                URI.Append(input.other);
            if (input.intitle != null)
                URI.Append($"+intitle:{input.intitle}");
            if (input.inauthor != null)
                URI.Append($"+inauthor:{input.inauthor}");
            if (input.subject != null)
                URI.Append($"+subject:{input.subject}");
            if (input.isbn != null)
                URI.Append($"+isbn:{input.isbn}");
            URI.Append($"&key={APIKeys.APIKey.GoogleBooks}");
            return VolumeSearch(URI.ToString()).Result;
        }
        public async static Task<GoogleBooksSearchResponse> VolumeSearch(string uri)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                using (HttpResponseMessage message = await client.GetAsync(uri).ConfigureAwait(false))
                {
                    if (message.IsSuccessStatusCode)
                    {
                        string json = await message.Content.ReadAsStringAsync().ConfigureAwait(false);
                        return JsonConvert.DeserializeObject<GoogleBooksSearchResponse>(json);
                    }
                    else
                    {
                        throw new Exception(message.ReasonPhrase);
                    }
                }
            }
        }
        public async static Task<GoogleBookSingleResponse> SingleSearch(string GoogleETag)
        {
            string URI = "";
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                using (HttpResponseMessage message = await client.GetAsync(URI).ConfigureAwait(false))
                {
                    if (message.IsSuccessStatusCode)
                    {
                        string json = await message.Content.ReadAsStringAsync().ConfigureAwait(false);
                        return JsonConvert.DeserializeObject<GoogleBookSingleResponse>(json);
                    }
                    else
                    {
                        throw new Exception(message.ReasonPhrase);
                    }
                }
            }
        }
    }
}