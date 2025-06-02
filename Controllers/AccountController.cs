using Microsoft.AspNetCore.Mvc;
using BackendHtml.Context;
using LoginRegisterExample.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using NuGet.Protocol.Plugins;
using Microsoft.AspNetCore.Identity.UI.Services;
using BackendHtml.Models;

namespace BackendHtml.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        IEmailSender sender;
        private int otpCode;
        private AccountRepository accountRepository;
        public AccountController(ApplicationDbContext context, IEmailSender sender, IConfiguration configuration)
        {
            _context = context;
            this.sender = sender;
            accountRepository = new AccountRepository(configuration);
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


            User? user = _context.Users.FirstOrDefault(u => u.Email == email);

            if (user != null)
            {
                if (PasswordHasher.VerifyPassword(password, user.PasswordHash))
                {
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
                    return Redirect("/Account/denied");
                }
            }
            else
            {

                ViewBag.Message = "Đăng nhập thất bại. Sai tên hoặc mật khẩu.";
                return Redirect("/Account/denied");
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
        public IActionResult otppassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> otppassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                ViewBag.Message = "Vui lòng nhập địa chỉ email";
                return View();
            }

            User? user = _context.Users.FirstOrDefault(u => u.Email == email);

            if (user != null)
            {

                // user.OtpCode = otpCode;
                otpCode = Random.Shared.Next(100000, 999999); // Tạo mã OTP ngẫu nhiên 6 chữ số
                _context.SaveChanges();
                User newUser = new User
                {
                    Id = user.Id,
                    Fullname = user.Fullname,
                    Email = user.Email,
                    PasswordHash = PasswordHasher.HashPassword(otpCode.ToString())

                };
                await accountRepository.UpdateUser(newUser);
                string bodyMail = "Dear " + Convert.ToString(user.Fullname) + ",\n\n" +
                "Your OTP code is: " + otpCode.ToString() + "\n" +
                "\n\nThank you,\n" + "Your AI Bep Team \n";

                await sender.SendEmailAsync(user.Email, "AI Bep OTP Code", bodyMail);

                return Redirect("/Account/ChangePassword");
            }
            else
            {
                ViewBag.Message = "Email không tồn tại trong hệ thống.";
                return Redirect("/Account/Denied");
            }
        }
        public IActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword(string email, string otp, string password, string repassword)
        {
            System.Console.WriteLine("----------------------------------");
            System.Console.WriteLine($"Email: {email}, OTP Code: {otp}, Password: {password}, Repassword: {repassword}");
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(otp) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(repassword))
            {
                ViewBag.Message = "Vui lòng nhập đầy đủ thông tin";
                return View();
            }

            if (password != repassword)
            {
                ViewBag.Message = "Mật khẩu và xác nhận mật khẩu không khớp.";
                return View();
            }

            User? user = _context.Users.FirstOrDefault(u => u.Email == email);

            if (user != null)
            {
                System.Console.WriteLine("----------------------------------vao dayyyyy");
                if (PasswordHasher.VerifyPassword(otp, user.PasswordHash))
                {
                    System.Console.WriteLine("----------------------------------vao day");
                    System.Console.WriteLine($"User ID: {user.Id}, User Name: {user.Fullname}");
                    user.PasswordHash = PasswordHasher.HashPassword(password);
                    await accountRepository.UpdateUser(user);

                    ViewBag.Message = "Mật khẩu đã được thay đổi thành công!";
                    return Redirect("/Account/Login");
                }
                return Redirect("/Account/Denied");
            }
            else
            {
                ViewBag.Message = "Mã OTP không hợp lệ hoặc email không đúng.";
                return Redirect("/Account/Denied");
            }
        }
        // {
        //     if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(otpCode) || string.IsNullOrEmpty(newPassword))
        //     {
        //         ViewBag.Message = "Vui lòng nhập đầy đủ thông tin";
        //         return View();
        //     }

        //     User? user = _context.Users.FirstOrDefault(u => u.Email == email);

        //     if (user != null && user.PasswordHash == PasswordHasher.HashPassword(otpCode))
        //     {
        //         user.PasswordHash = PasswordHasher.HashPassword(newPassword);
        //         _context.SaveChanges();

        //         ViewBag.Message = "Mật khẩu đã được thay đổi thành công!";
        //         return Redirect("/Account/Login");
        //     }
        //     else
        //     {
        //         ViewBag.Message = "Mã OTP không hợp lệ hoặc email không đúng.";
        //         return Redirect("/Account/Denied");
        //     }
        // }
    }
}
