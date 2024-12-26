using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace Web.ControllersApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpeechToTextController : ControllerBase
    {
        
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public SpeechToTextController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        [HttpPost("upload-audio")]
        public async Task<IActionResult> UploadAudio([FromForm] AudioUploadModel model)
        {
            if (model.Audio == null || model.Audio.Length == 0)
            {
                return BadRequest("未收到音頻文件。");
            }

            // 保存臨時音頻文件
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName = $"{Guid.NewGuid()}_{model.Audio.FileName}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await model.Audio.CopyToAsync(stream);
            }

            try
            {
                // 調用 OpenAI Whisper API 進行轉錄
                var transcription = await TranscribeAudioAsync(filePath);
                return Ok(new { transcript = transcription });
            }
            catch (Exception ex)
            {
            
                return StatusCode(500, $"處理音頻文件時出錯: {ex.Message}");
            }
            finally
            {
                // 刪除臨時音頻文件
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }
        }

        private async Task<string> TranscribeAudioAsync(string filePath)
        {
            var apiKey = _configuration["OpenAI:ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
            {
                throw new Exception("OpenAI API 金鑰未配置。");
            }

            var apiUrl = "https://api.openai.com/v1/audio/transcriptions";

            using var httpClient = _httpClientFactory.CreateClient();

    
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            using var form = new MultipartFormDataContent();

      
            using var fileStream = System.IO.File.OpenRead(filePath);
            var fileContent = new StreamContent(fileStream);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("audio/wav"); 
            form.Add(fileContent, "file", Path.GetFileName(filePath));

     
            form.Add(new StringContent("whisper-1"), "model");

         
            form.Add(new StringContent("zh"), "language");

          
            form.Add(new StringContent("ZyntriQix, Digique Plus, CynapseFive, VortiQore V8, EchoNix Array, OrbitalLink Seven, DigiFractal Matrix, PULSE, RAPT, B.R.I.C.K., Q.U.A.R.T.Z., F.L.I.N.T."), "prompt");

            var response = await httpClient.PostAsync(apiUrl, form);

            if (response.IsSuccessStatusCode)
            {
                var transcriptionResponse = await response.Content.ReadFromJsonAsync<TranscriptionResponse>();
                return transcriptionResponse?.Text ?? "沒有轉錄結果。";
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"OpenAI API 錯誤: {response.StatusCode}\n{errorContent}");
            }
        }

  
        public class AudioUploadModel
        {
            public IFormFile Audio { get; set; }
        }

     
        public class TranscriptionResponse
        {
            [System.Text.Json.Serialization.JsonPropertyName("text")]
            public string Text { get; set; }
        
        }
    }
}

