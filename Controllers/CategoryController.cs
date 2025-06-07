using System.Threading.Tasks;
using BackendHtml.Models;
using Microsoft.AspNetCore.Mvc;

namespace BackendHtml.Controllers
{
    public class CategoryController : Controller
    {
        private CategoryRepository _categoryRepository;
        public CategoryController(IConfiguration configuration)
        {
            _categoryRepository = new CategoryRepository(configuration);
        }
        public IActionResult Index()
        {
            if (User.Identity == null || !User.Identity.IsAuthenticated || User.Identity.Name != "admin")
            {
                return Redirect("/account/denied");
            }
            else if (User.Identity.Name == "admin")
            {
                return View(_categoryRepository.GetCategories());
            }
            else
            {
                return Redirect("/account/login");
            }

        }
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(Category obj)
        {
            if (ModelState.IsValid)
            {
                await _categoryRepository.AddAsync(obj);
            }
            return Redirect("/category");
        }
        public async Task<IActionResult> Delete(int id, string? categoryName)
        {

            return View(await _categoryRepository.GetCategoryById(id));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (ModelState.IsValid)
            {
                await _categoryRepository.Delete(id);
            }
            return Redirect("/category");
        }
    }
}
