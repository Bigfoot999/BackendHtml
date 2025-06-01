
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using Humanizer;
public class Gemini
{
    private static readonly HttpClient client = new HttpClient();
    private static readonly API a = new API();


    public static async Task<string> CallGeminiApi(string prompt, string apiUrl, string apiKey)
    {

        try
        {
            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                }
            };

            string jsonRequest = System.Text.Json.JsonSerializer.Serialize(requestBody);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            string url = $"{apiUrl}?key={apiKey}";
            HttpResponseMessage response = await client.PostAsync(url, content);
            string responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return $"HTTP Error: {response.StatusCode}\nDetails:\n{responseBody}";
            }

            // Phân tích JSON và lấy văn bản trả về
            using (JsonDocument doc = JsonDocument.Parse(responseBody))
            {
                var root = doc.RootElement;
                // Tìm phần "text" trong "content" và "parts"
                var text = root
                    .GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text")
                    .GetString();

                return text ?? "Không có văn bản trả về.";
            }
        }
        catch (HttpRequestException ex)
        {
            return $"HTTP Error: {ex.Message}";
        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}";
        }
    }
}