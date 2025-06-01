namespace BackendHtml.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using BackendHtml.Models;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using System.Security.Claims;


    [Authorize]  // Ensure that the user is authenticated to access this controller
    public class AIController : Controller
    {
        private AIRepository _aiRepository;
        private CategoryRepository _categoryRepository;

        public AIController(IConfiguration configuration)
        {
            _aiRepository = new AIRepository(configuration);
            _categoryRepository = new CategoryRepository(configuration);

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
        public async Task<IActionResult> Edit(int id, AIContent obj, IFormFile? imageFile, String categoryContent)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null)
                {
                    // Kiểm tra người dùng có upload ảnh không
                    string ext = Path.GetExtension(imageFile.FileName); //Lấy tên đuôi file ảnh
                    obj.ImageUrl = Helper.RandomString(32 - ext.Length) + ext;
                    string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwwroot", "image_ai", obj.ImageUrl);

                    using (Stream stream = new FileStream(path, FileMode.Create))
                    {
                        imageFile.CopyTo(stream);
                    }
                }
                else
                {
                    // Nếu không có ảnh thì giữ nguyên ảnh cũ
                    var existingContent = await _aiRepository.GetAIContentById(id);
                    obj.ImageUrl = existingContent.ImageUrl;
                }
                obj.Id = id;
                obj.CategoryContent = categoryContent;
                int ret = await _aiRepository.Edit(obj);
                string url = "/ai/usercontent/";
                if (ret > 0)
                {
                    return Redirect(url);
                }
            }
            return Redirect("/product/error");
        }

    }
}