using Microsoft.AspNetCore.Mvc;
using Leave.Core.Interfaces;
using Leave.Core.Models;

namespace LeaveManagement.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;

        public AccountController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            try
            {
                var user = await _authService.AuthenticateAsync(username, password);
                if (user == null)
                {
                    ViewBag.Error = "Invalid username or password";
                    return View();
                }

                HttpContext.Session.SetInt32("UserId", user.UserId);
                HttpContext.Session.SetString("Username", user.Username);
                HttpContext.Session.SetInt32("RoleId", user.RoleId);
                HttpContext.Session.SetInt32("EmployeeId", user.EmployeeId ?? 0);

                if (user.RoleId == 3)
                {
                    return RedirectToAction("Index", "Profile");
                }

                return RedirectToAction("Index", "Dashboard");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(User user, string password)
        {
            try
            {
                await _authService.RegisterAsync(user, password);
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }

        public IActionResult AccessDenied()
        {
            ViewBag.Message = "You don't have permission to access this page.";
            return View();
        }
    }
}
