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

        public async Task<IActionResult> Index()
        {
            List<Contact> contacts = await contactRepository.GetContacts();
            return View(contacts);
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
        [Authorize]
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