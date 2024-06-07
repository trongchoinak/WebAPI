
 using Microsoft.AspNetCore.Mvc;
using MVC.Models;
using System.Text;
using System.Text.Json;
using MVC.Models.DTO;
using System.Net.Http.Headers;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
namespace MVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly HttpClient _httpClient;

        public AccountController(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:7069/");
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Thêm thông báo lỗi cho người dùng biết
                ModelState.AddModelError(string.Empty, "Vui lòng điền đầy đủ thông tin.");
                return View(model);
            }

            var registerRequest = new
            {
                Username = model.Username,
                Password = model.Password,
                Roles = model.Roles
            };

            var jsonContent = new StringContent(JsonSerializer.Serialize(registerRequest), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/User/Register", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Login", "Account");
            }

            // Lấy nội dung phản hồi từ API để biết thêm chi tiết về lỗi
            var errorMessage = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, $"Registration failed. {errorMessage}");
            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var loginRequest = new
            {
                Username = model.Username,
                Password = model.Password
            };

            var jsonContent = new StringContent(JsonSerializer.Serialize(loginRequest), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/User/Login", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                var loginResponse = JsonSerializer.Deserialize<LoginResponseDTO>(responseData, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                // Thiết lập ClaimsPrincipal cho người dùng
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, model.Username)
            // Thêm các claims khác nếu cần
        };

                var claimsIdentity = new ClaimsIdentity(claims, "CookieAuthentication");
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                await HttpContext.SignInAsync("CookieAuthentication", claimsPrincipal);

                // Lưu JWT Token vào session
                HttpContext.Session.SetString("JWTToken", loginResponse.JwtToken);

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            var token = HttpContext.Session.GetString("JWTToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.PostAsync("https://localhost:7069/api/User/Logout", null);

                if (response.IsSuccessStatusCode)
                {
                    HttpContext.Session.Remove("JWTToken");
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    // Handle error if needed
                    ModelState.AddModelError(string.Empty, "Logout failed.");
                    return View("Error");
                }
            }
            await HttpContext.SignOutAsync("CookieAuthentication");
            return RedirectToAction("Login", "Account");
        }

    }
}
