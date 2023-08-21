using LyristAPI.Model;
using System.Text.Json;
using System.Web;

namespace LyristAPI
{
    // Classe estática de utilidades.
    internal static class LyristUtils
    {
        // Transofrma a resposta do servidor em um objeto manipulável.
        public static Music TryParseResponse(string response)
        {
            try
            {
                Music result = JsonSerializer.Deserialize<Music>(response)!;

                if(result.Lyrics == null)
                    throw new Exception("Música näo encontrada"); // Lança um erro caso não exista retorno.

                return result;
            }
            catch (JsonException ex)
            {
                throw new Exception("Erro de desserialização: " + ex.Message);
            }
        }

        public static string BuildEncodedURL(string title, string artist)
        {
            // Monta uma URL no formato certo para buscar na API.
            string baseUrl = "https://lyrist.vercel.app/api/";
            string encodedTitle = Uri.EscapeDataString(title);
            string encodedArtist = Uri.EscapeDataString(artist);

            return $"{baseUrl}/{encodedTitle}/{encodedArtist}";
        }
    }
    // Classe responsável somente pelo contato com a API.
    public class LyristAPIService
    {
        private readonly HttpClient _httpClient;
        // Dois construtores
        // Caso haja um HTTPClient anterior, pode ser utilizado. Caso não, cria-se um novo.
        public LyristAPIService()
        {
            _httpClient = new HttpClient();
        }

        public LyristAPIService(HttpClient client)
        {
            _httpClient = client;
        }

        // Busca a música na API.
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
                // Qualquer problema a requisição vai ser pego aqui.
                throw new Exception("Houve um erro durante a solicitação");
            }
        }
    }
    // Classe principal
    public class Lyrist
    {
        private readonly LyristAPIService _service;
        // Recebe o serviço que interage com a API. Pode ser substituído por outro.
        public Lyrist(LyristAPIService apiService)
        {
            _service = apiService;   
        }

        // Método que coordena a busca.
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