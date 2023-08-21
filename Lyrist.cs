using LyristAPI.Model;
using System.Text.Json;
using System.Web;

namespace LyristAPI
{
    internal static class LyristUtils
    {
        public static Music TryParseResponse(string response)
        {
            try
            {
                Music result = JsonSerializer.Deserialize<Music>(response)!;

                if(result.Lyrics == null)
                    throw new Exception("Música näo encontrada");

                return result;
            }
            catch (JsonException ex)
            {
                throw new Exception("Erro de desserialização: " + ex.Message);
            }
        }

        public static string BuildEncodedURL(string title, string artist)
        {
            string baseUrl = "https://lyrist.vercel.app/api/";
            string encodedTitle = Uri.EscapeDataString(title);
            string encodedArtist = Uri.EscapeDataString(artist);

            return $"{baseUrl}/{encodedTitle}/{encodedArtist}";
        }
    }
    public class LyristAPIService
    {
        private readonly HttpClient _httpClient;
        public LyristAPIService()
        {
            _httpClient = new HttpClient();
        }

        public LyristAPIService(HttpClient client)
        {
            _httpClient = client;
        }

        public async Task<string> GetApiResponseAsync(string url)
        {
            HttpResponseMessage response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                return content;
            }
            else
            {
                throw new Exception("Houve um erro durante a solicitação");
            }
        }
    }
    public class Lyrist
    {
        private readonly LyristAPIService _service;

        public Lyrist(LyristAPIService apiService)
        {
            _service = apiService;   
        }
        public async Task<Music> Search(string title, string artist)
        {
            try
            {
                string address = LyristUtils.BuildEncodedURL(title, artist);
                string content = _service.GetApiResponseAsync(address).Result;
                Music result = LyristUtils.TryParseResponse(content);

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Ocorreu um erro: " + ex.Message);
            }
        }
    }
}