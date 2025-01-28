using Microsoft.AspNetCore.Mvc;

namespace NetOpenAI_1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        [HttpGet("models")]
        [Produces("application/json")]
        public async Task<IActionResult> GetModels()
        {
            try
            {
                var modelsAvailable = new string[]{ "gpt-4o-mini", "gpt-4o", "o1", "o1-mini" };

                List<GptModel> models = new List<GptModel>();
                foreach (var model in modelsAvailable)
                {
                    GptModel gptModel = new GptModel
                    {
                        name = model
                    };
                    models.Add(gptModel);
                }

                return Ok(new { result = models });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Ocurrió un error al ejecutar el comando: {ex.Message}" });
            }
        }
    }

    public class GptModel
    {
        public string name { get; set; }
    }
}
