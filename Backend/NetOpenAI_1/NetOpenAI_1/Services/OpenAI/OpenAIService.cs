using System.Net.Http.Headers;
using System.Text.Json;

namespace NetOpenAI_1.Services.OpenAI
{
    public class OpenAIService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _model;

        public OpenAIService(IConfiguration configuration)
        {
            _apiKey = configuration["AppSettings:OpenAI:ApiKey"];
            _model = configuration["AppSettings:OpenAI:Model"];
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://api.openai.com/v1/")
            };
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
        }

        public async Task<(string, int, int)> GenerarRespuestaAsync(string pregunta)
        {
            var requestBody = new
            {
                model = _model,
                messages = new[]
                {
                new { role = "system", content = "Eres un asistente útil." },
                new { role = "user", content = pregunta }
            }
            };

            var requestContent = new StringContent(JsonSerializer.Serialize(requestBody), System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("chat/completions", requestContent);
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error en la API de OpenAI: {responseString}");
            }

            using var jsonDoc = JsonDocument.Parse(responseString);
            var root = jsonDoc.RootElement;

            string respuesta = root.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();
            int promptTokens = root.GetProperty("usage").GetProperty("prompt_tokens").GetInt32();
            int completionTokens = root.GetProperty("usage").GetProperty("completion_tokens").GetInt32();

            return (respuesta, promptTokens, completionTokens);
        }
    }
}
