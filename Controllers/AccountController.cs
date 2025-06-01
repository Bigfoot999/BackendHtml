using Microsoft.AspNetCore.Mvc;
using BackendHtml.Context;
using LoginRegisterExample.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using NuGet.Protocol.Plugins;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace BackendHtml.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        IEmailSender sender;
        public AccountController(ApplicationDbContext context, IEmailSender sender)
        {
            _context = context;
            this.sender = sender;
        }

        // Trang đăng nhập
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // Xử lý đăng nhập
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewBag.Message = "Vui lòng nhập đầy đủ thông tin";
                return View();
            }

            // Kiểm tra tên đăng nhập và mật khẩu
            User? user = _context.Users.FirstOrDefault(u => u.Email == email);

            if (user != null && PasswordHasher.VerifyPassword(password, user.PasswordHash))
            {
                // Tạo claims
                //List<Claim> claims = new List<Claim>{
                //    new Claim(ClaimTypes.Name, user.Fullname),
                //    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
                //};
                List<Claim> claims = new List<Claim>{
                     new Claim(ClaimTypes.Name, user.Fullname),
                     new Claim(ClaimTypes.Email, user.Email),
                     new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
                 };

                ;

                // Tạo identity và principal
                ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                ClaimsPrincipal principal = new ClaimsPrincipal(identity);

                // Đăng nhập và lưu cookie
                await HttpContext.SignInAsync(principal, new AuthenticationProperties
                {
                    IsPersistent = true
                });


                TempData["Message"] = "Đăng nhập thành công!";

                return Redirect("/home");
            }
            else
            {

                ViewBag.Message = "Đăng nhập thất bại. Sai tên hoặc mật khẩu.";
                return Redirect("/Account/Login");
            }
        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/account/login");
        }

        // Trang đăng ký
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // Xử lý đăng ký
        [HttpPost]
        public IActionResult Register(string username, string fullname, string password, string email)
        {
            if (_context.Users.Any(u => u.Username == username))
            {
                ViewBag.Message = "Tên đăng nhập đã tồn tại!";
                return View();
            }

            User newUser = new User
            {
                Username = username,
                Fullname = fullname,
                Email = email,
                PasswordHash = PasswordHasher.HashPassword(password)
            };

            _context.Users.Add(newUser);
            _context.SaveChanges();

            ViewBag.Message = "Đăng ký thành công!";


            string bodyMail = "Dear " + Convert.ToString(newUser.Fullname) + ",\n\n" +
            "Thank you for registering an account for AI Bep Web. \n" +
            "Username for login: " + Convert.ToString(newUser.Username) + "\n" +
            "\n\nThank you,\n" + "Your AI Bep Team \n";

            sender.SendEmailAsync(newUser.Email, "AI Bep Account Created", bodyMail);

            return Redirect("/Account/Login");
        }
        public IActionResult Denied()
        {
            return View();
        }
    }
}
