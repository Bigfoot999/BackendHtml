namespace BackendHtml.Controllers
{
    using BackendHtml.Context;
    using BackendHtml.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    public class ContactController : Controller
    {
        private ContactRepository contactRepository;

        public ContactController(IConfiguration configuration)
        {
            contactRepository = new ContactRepository(configuration);
        }
        [Authorize]
        public async Task<IActionResult> Index()
        {
            if (User.Identity == null || !User.Identity.IsAuthenticated || User.Identity.Name != "admin")
            {
                return Redirect("/account/denied");
            }
            else if (User.Identity.Name == "admin")
            {
                List<Contact> contacts = await contactRepository.GetContacts();
                return View(contacts);
            }
            else
            {
                return Redirect("/account/login");
            }

        }

        public IActionResult Contact()
        {
            return View();
        }
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Contact(Contact contact)
        {
            System.Console.WriteLine("Contact form submitted with data: " + contact.Name + ", " + contact.Email + ", " + contact.Phone + ", " + contact.Message);
            // Validate the contact object
            if (ModelState.IsValid)
            {
                int result = await contactRepository.Add(contact);
                if (result > 0)
                {
                    TempData["Message"] = "Contact added successfully!";
                    //return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Failed to add contact. Please try again.");
                }
            }
            return Redirect("/home");
        }
    }
}