using System.Text.Json;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.SignalR;

namespace NetOpenAI_1.Hubs.OpenAI
{
    public class ChatHub : Hub
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _model;

        public ChatHub(IConfiguration configuration)
        {
            _apiKey = configuration["AppSettings:OpenAI:ApiKey"];
            _model = configuration["AppSettings:OpenAI:Model"];
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://api.openai.com/v1/")
            };
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
        }

        public async Task GetMessage(string message, string modelName)
        {
            string mId = Guid.NewGuid().ToString();

            var requestBody = new
            {
                model = _model,
                stream = true,
                messages = new[]
                {
                new { role = "system", content = "Eres un asistente útil." },
                new { role = "user", content = message }
            }
            };

            var requestContent = new StringContent(JsonSerializer.Serialize(requestBody), System.Text.Encoding.UTF8, "application/json");

            using var response = await _httpClient.PostAsync("chat/completions", requestContent);

            if (!response.IsSuccessStatusCode)
            {
                await Clients.Caller.SendAsync("ReceiveMessage", "Error en OpenAI", mId);
                return;
            }

            using var stream = await response.Content.ReadAsStreamAsync();
            using var reader = new StreamReader(stream);

            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();
                if (string.IsNullOrWhiteSpace(line) || !line.StartsWith("data:")) continue;

                line = line.Replace("data:", "").Trim();
                if (line == "[DONE]") break;

                try
                {
                    var json = JsonDocument.Parse(line);
                    var content = json.RootElement.GetProperty("choices")[0].GetProperty("delta").GetProperty("content").GetString();

                    if (!string.IsNullOrEmpty(content))
                    {
                        // Enviar fragmento al cliente en tiempo real
                        await Clients.Caller.SendAsync("ReceiveMessage", content, mId);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error en el procesamiento de la respuesta: {ex.Message}");
                }
            }
        }
    }
}
