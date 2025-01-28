using Microsoft.AspNetCore.SignalR;
using NetOpenAI_1.Entities;
using System.Net.Http.Headers;
using System.Text.Json;

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
            string completeResponse = "";
            int promptTokens = 0;
            int completionTokens = 0;
            int totalTokens = 0;

            if (string.IsNullOrEmpty(modelName))
            {
                modelName = _model;
            }

            var requestBody = new
            {
                model = modelName,
                //stream = true,
                stream = false,
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

            #region Stream
            //using var stream = await response.Content.ReadAsStreamAsync();
            //using var reader = new StreamReader(stream);

            //while (!reader.EndOfStream)
            //{
            //    var line = await reader.ReadLineAsync();
            //    if (string.IsNullOrWhiteSpace(line) || !line.StartsWith("data:")) continue;

            //    line = line.Replace("data:", "").Trim();
            //    if (line == "[DONE]") break;

            //    try
            //    {
            //        var json = JsonDocument.Parse(line);
            //        var root = json.RootElement;

            //        // 🔹 Capturar fragmentos de la respuesta en tiempo real
            //        if (root.TryGetProperty("choices", out var choices) && choices.GetArrayLength() > 0)
            //        {
            //            var choice = choices[0];

            //            if (choice.TryGetProperty("delta", out var delta) && delta.TryGetProperty("content", out var content))
            //            {
            //                string fragment = content.GetString();
            //                if (!string.IsNullOrEmpty(fragment))
            //                {
            //                    completeResponse += fragment;
            //                    await Clients.Caller.SendAsync("ReceiveMessage", fragment, mId);
            //                }
            //            }
            //        }

            //        // 🔹 Capturar tokens (esto normalmente solo aparece en la última respuesta del stream)
            //        if (root.TryGetProperty("usage", out var usage))
            //        {
            //            promptTokens = usage.GetProperty("prompt_tokens").GetInt32();
            //            completionTokens = usage.GetProperty("completion_tokens").GetInt32();
            //            totalTokens = promptTokens + completionTokens;
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine($"Error en el procesamiento de la respuesta: {ex.Message}");
            //    }
            //}
            #endregion Stream

            #region No Stream
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error en la API de OpenAI: {responseString}");
            }

            using var jsonDoc = JsonDocument.Parse(responseString);
            var root = jsonDoc.RootElement;

            completeResponse = root.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();
            promptTokens = root.GetProperty("usage").GetProperty("prompt_tokens").GetInt32();
            completionTokens = root.GetProperty("usage").GetProperty("completion_tokens").GetInt32();
            totalTokens = promptTokens + completionTokens;

            await Clients.Caller.SendAsync("ReceiveMessage", completeResponse, mId);
            #endregion No Stream

            // 🔹 Guardar la interacción en la base de datos con los tokens extraídos
            var interaccion = new Interaction
            {
                ChatId = 1, // Ajustar según el ID del chat del usuario
                Request = message,
                Response = completeResponse,
                PromptTokens = promptTokens,
                CompletionTokens = completionTokens,
                TotalTokens = totalTokens
            };

            Console.WriteLine($"Interacción: {interaccion.Request} -> {interaccion.Response}");
            Console.WriteLine($"Tokens: {interaccion.PromptTokens} + {interaccion.CompletionTokens} = {interaccion.TotalTokens}");
            Console.WriteLine(responseString);
        }
    }
}
