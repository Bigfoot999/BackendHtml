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
            return View(_categoryRepository.GetCategories());
        }
        public IActionResult Add()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Add(Category obj)
        {
            if (ModelState.IsValid)
            {
                return Json(_categoryRepository.Add(obj));
            }
            return Json(-1);
        }
    }
}
