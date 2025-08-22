using Business.Interfaces;
using Entities;
using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ToDoList.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;

        public AccountController(IUserService userService)
        {
            _userService = userService;

        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {
            try
            {
                await _userService.AddAsync(user);


                return RedirectToAction("Login");
            }

            catch (FluentValidation.ValidationException ex)
            {
                ModelState.Clear();
                foreach (var error in ex.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                return View(user);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Kayıt işlemi sırasında bir hata oluştu: " + ex.Message);
                return View(user);
            }
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = await _userService.GetByUsernameAsync(username);

            if (user != null)
            {
                var hasher = new PasswordHasher<User>();
                var result = hasher.VerifyHashedPassword(user, user.Password, password);

                if (result == PasswordVerificationResult.Success)
                {
                    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                    return RedirectToAction("Index", "ToDoGroup");
                }
                else
                {
                    HttpContext.Items["Audit_LoginFailed"] = new { Username = username, Reason = "InvalidPassword" };
                    HttpContext.Items["Audit_Message"] = "Login failed: Invalid password";
                }
            }
            else
            {
                HttpContext.Items["Audit_LoginFailed"] = new { Username = username, Reason = "UserNotFound" };
                HttpContext.Items["Audit_Message"] = "Login failed: user not found";
            }

            Response.StatusCode = 401;
            ViewBag.Message = "Kullanıcı adı veya şifre hatalı.";
            return View();
        }



        [HttpGet("/test-error")]
        public IActionResult TestError()
        {
            throw new InvalidOperationException("Demo amaçlı atılan hata!");
        }

        [HttpPost("/test-post")]
        public IActionResult TestPost([FromBody] object body)
        {
            throw new Exception("Post test hatası");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

    }
}
