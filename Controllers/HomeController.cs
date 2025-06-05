using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BackendHtml.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BackendHtml.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    public AIRepository _aiRepository;
    private readonly IConfiguration _configuration;
    private readonly CategoryRepository _categoryRepository;
    private readonly AccountRepository _accountRepository;
    public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
        _aiRepository = new AIRepository(_configuration);
        _categoryRepository = new CategoryRepository(_configuration);
        _accountRepository = new AccountRepository(_configuration);
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
    public IActionResult Index()
    {
        return View();
    }
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Index(String ingredients, string servingSize, string cuisine, string difficulty, string diet)
    {
        // String aiContent = $"Create a recipe with the following details: Ingredients: {ingredients}, Serving Size: {servingSize}, Cuisine: {cuisine}, Difficulty: {difficulty}, Diet: {diet}";
        String aiContent = createPrompt(ingredients, servingSize, cuisine, difficulty, diet);

        string? userId;
        string? username;
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            username = User.FindFirst(ClaimTypes.Name)?.Value;
        }
        else
        {
            userId = "UnknownUser"; // or handle unauthenticated users as needed
            username = "UnknownUser"; // or handle unauthenticated users as needed
        }
        AIContent temp = new AIContent();
        String response = await Gemini.CallGeminiApi(aiContent,
        "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent", "AIzaSyA9EM4-R8YeI6YwkDJ8qvK84sOeKh1Cl4Y");
        temp.Content = response;
        temp.UserID = userId;
        temp.Username = username;
        temp.Title = GetTitle(response);
        System.Console.WriteLine("-------------------------------------------------");
        System.Console.WriteLine(GetTitle(response));
        AIContent result = await _aiRepository.InsertAIContent(temp);
        //return CreatedAtAction(nameof(GetAIContentById), new { id = result.Id }, result);
        return Redirect("/AI/Usercontent");
    }



    public string GetTitle(string input)
    {
        // Biểu thức chính quy tìm chuỗi giữa cặp ** đầu tiên
        string pattern = @"\*\*(.*?)\*\*";
        Match match = Regex.Match(input, pattern);

        if (match.Success)
        {
            // Trả về nhóm 1 (phần chuỗi nằm giữa **)
            return match.Groups[1].Value.Trim();
        }

        // Nếu không tìm thấy, trả về chuỗi rỗng hoặc ném ngoại lệ tùy yêu cầu
        return string.Empty;
        // Hoặc: throw new InvalidOperationException("Không tìm thấy tên món ăn trong chuỗi.");
    }


    private String createPrompt(String ingredients, string servingSize, string cuisine, string difficulty, string diet)
    {
        string temppromt = string.Format(
        @"Hãy phân tích tất cả các nguyên liệu {0}, khẩu phần ăn cho {1}, ẩm thực theo nước {2} , độ khó cách nấu : {3}, sở thích ăn uống: {4} tôi muốn nấu một món ăn và viết lại bằng tiếng việt chính xác theo cấu trúc bên dưới:
       ** [Viết tên món ăn tại đây]** 
        * Mô tả món ăn:  *
        [Viết mô tả món ăn tại đây] 
        * Âm thực của nước: [Viết thông tin nước tại đây] *
        * Nguyên liệu:  *
         [Viết nguyên liệu tại đây] 
        * Hướng dẫn chế biến:  *
        [Trình bày hướng dẫn chế biến tại đây]
        * Trình Bày:  *
        [Trình bày món ăn tại đây]
        * Lưu ý:  *
        [Viết lưu ý tại đây]
        
        prompt, ingredients, servingSize, cuisine, difficulty, diet);
        return temppromt;   
        ", ingredients, servingSize, cuisine, difficulty, diet);
        //string temppromt = "giúp tôi viết hướng dẫn bằng tiếng Việt: " + prompt;
        //string temppromt = $"Hãy phân tích tất cả các nguyên liệu và yêu cầu để thực nấu một món ăn và viết lại chính xác bằng tiếng việt theo cấu trúc bên dưới: {prompt}";
        //string temppromt = $"Hãy phân tích tất cả các nguyên liệu và yêu cầu để thực nấu một món ăn và viết lại chính xác bằng tiếng việt theo cấu trúc bên dưới: {prompt}";  
        return temppromt;
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
    public IActionResult Privacy()
    {
        return View();
    }
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = HttpContext.TraceIdentifier });
    }
    public IActionResult About()
    {
        return View();
    }
    public IActionResult Contact()
    {
        return View();
    }
    public async Task<IActionResult> Forum()
    {
        var result = await _aiRepository.GetAllAIContents();
        ViewBag.CategoryContent = _categoryRepository.GetCategories();
        return View(result);
    }
    [HttpPost]
    public async Task<IActionResult> Forum(string category)
    {
        ViewBag.CategoryContent = _categoryRepository.GetCategories();
        ViewBag.CategoryCurrent = category;
        if (category == "All")
        {
            var result = await _aiRepository.GetAllAIContents();

            return View(result);
        }
        else
        {
            var result = await _aiRepository.GetAllAIContentsByCategory(category);
            return View(result);

        }
    }

}

