namespace BackendHtml.Controllers
{
    using System.Security.Claims;
    using System.Text.Json;
    using System.Threading.Tasks;
    using BackendHtml.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using RestSharp;

    [Authorize]  // Ensure that the user is authenticated to access this controller
    public class AIController : Controller
    {
        private AIRepository _aiRepository;
        private CategoryRepository _categoryRepository;
        private readonly IConfiguration _configuration;
        public AIController(IConfiguration configuration)
        {
            _aiRepository = new AIRepository(configuration);
            _categoryRepository = new CategoryRepository(configuration);
            _configuration = configuration;
        }
        public IActionResult Index()
        {
            return View(_aiRepository.GetAllAIContents());
        }


        public IActionResult Add()
        {
            return View();
        }
        public async Task<IActionResult> Allcontent()
        {
            string? userId;
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userName = User.FindFirst(ClaimTypes.Name)?.Value;
                System.Console.WriteLine($"User ID: {userId}, User Name: {userName}");
            }
            else
            {
                userId = "UnknownUser"; // or handle unauthenticated users as needed
                return BadRequest("User ID is required.");
            }
            var result = (await _aiRepository.GetAIContentUserById(userId)).ToList();
            return View(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetAIContentById(int id)
        {
            var result = await _aiRepository.GetAIContentById(id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }


        [HttpGet]
        public async Task<IActionResult> GetAllAIContents()
        {
            var result = await _aiRepository.GetAllAIContents();
            return Ok(result);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Add(AIContent aiContent)
        {
            if (aiContent == null)
            {
                return BadRequest("Invalid AI content data.");
            }

            string? userId;
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userName = User.FindFirst(ClaimTypes.Name)?.Value;
                System.Console.WriteLine($"User ID: {userId}, User Name: {userName}");
            }
            else
            {
                userId = "UnknownUser"; // or handle unauthenticated users as needed
            }
            AIContent temp = new AIContent();
            String response = await Gemini.CallGeminiApi(aiContent.Content,
            "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent", "AIzaSyA9EM4-R8YeI6YwkDJ8qvK84sOeKh1Cl4Y");
            temp.Content = response;
            temp.UserID = userId;
            AIContent result = await _aiRepository.InsertAIContent(temp);
            return CreatedAtAction(nameof(GetAIContentById), new { id = result.Id }, result);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAIContent(int id)
        {
            var existingContent = await _aiRepository.GetAIContentById(id);
            if (existingContent == null)
            {
                return NotFound();
            }
            await _aiRepository.DeleteAIContent(id);
            return NoContent();
        }
        public async Task<IActionResult> UsercontentAsync()
        {
            string? userId;
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userName = User.FindFirst(ClaimTypes.Name)?.Value;
                System.Console.WriteLine($"User ID: {userId}, User Name: {userName}");
                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest("User ID is required.");
                }
            }
            else
            {
                return BadRequest("User ID is required.");
            }
            var result = (await _aiRepository.GetAIContentUserById(userId)).ToList();
            return View(result);
        }
        public async Task<IActionResult> ContentDetail(int id)
        {
            var result = await _aiRepository.GetAIContentById(id);
            if (result == null)
            {
                return NotFound();
            }
            return View(result);
        }

        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.Categorycontent = _categoryRepository.GetCategories();
            // var result = await _aiRepository.GetAIContentById(id);
            return View(await _aiRepository.GetAIContentById(id));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit(int id, AIContent aiContent, IFormFile? imageFile, string categoryContent)
        {
            if (ModelState.IsValid)
            {
                string imageUrl = null;
                if (imageFile != null)
                {
                    // Upload ảnh lên ImgBB
                    imageUrl = await UploadToImgBB(imageFile);
                    if (string.IsNullOrEmpty(imageUrl))
                    {
                        ModelState.AddModelError("", "Failed to upload image to ImgBB.");
                        return View("Edit", aiContent);
                    }
                    aiContent.ImageUrl = imageUrl;
                }
                else
                {
                    // Giữ nguyên URL ảnh cũ nếu không có file mới
                    var existingContent = await _aiRepository.GetAIContentById(id);
                    aiContent.ImageUrl = existingContent?.ImageUrl;
                }

                aiContent.Id = id;
                aiContent.CategoryContent = categoryContent;
                int ret = await _aiRepository.Edit(aiContent);
                if (ret > 0)
                {
                    return Redirect("/ai/usercontent/");
                }
            }
            return Redirect("/product/error");
        }

        private async Task<string> UploadToImgBB(IFormFile imageFile)
        {
            try
            {
                // Read API key from appsettings.json
                string apiKey = _configuration.GetSection("ImgBB:ApiKey").Value;
                if (string.IsNullOrEmpty(apiKey))
                {
                    Console.WriteLine("ImgBB API key not found in appsettings.json.");
                    return null;
                }

                // Validate file size (ImgBB limit: 32MB)
                if (imageFile.Length > 32 * 1024 * 1024)
                {
                    Console.WriteLine("File size exceeds 32MB limit.");
                    return null;
                }

                // Validate file format
                string ext = Path.GetExtension(imageFile.FileName).ToLower();
                if (!new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" }.Contains(ext))
                {
                    Console.WriteLine("Unsupported file format.");
                    return null;
                }

                // Create RestClient instance
                using var client = new RestClient("https://api.imgbb.com/1/upload");
                var request = new RestRequest
                {
                    Method = Method.Post,
                    AlwaysMultipartFormData = true
                };

                // Read image file into memory
                using var memoryStream = new MemoryStream();
                await imageFile.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                // Add image file and API key to request
                request.AddFile("image", memoryStream.ToArray(), imageFile.FileName, imageFile.ContentType);
                request.AddParameter("key", apiKey);

                // Execute request
                var response = await client.ExecuteAsync(request);
                if (!response.IsSuccessful || response.Content == null)
                {
                    Console.WriteLine($"ImgBB upload failed: {response.StatusCode}");
                    return null;
                }

                // Parse JSON response to get URL
                using var jsonDoc = JsonDocument.Parse(response.Content);
                var imageUrl = jsonDoc.RootElement.GetProperty("data").GetProperty("url").GetString();

                return imageUrl;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading to ImgBB: {ex.Message}");
                return null;
            }
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _aiRepository.DeleteAIContent(id);
            return Redirect("/ai/usercontent");
        }

    }
}