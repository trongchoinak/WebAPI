using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http;
using MVC.Models;
using System.Net.Mail;
using System.Net;
namespace MVC.Controllers
{
    public class MenuController : Controller
    {
        private readonly HttpClient _httpClient;
        public MenuController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new System.Uri("https://localhost:7069/");
        }
        public async Task<IActionResult> dienthoai(string sortOrder, int pageNumber = 1, int pageSize = 6)
        {
            var token = HttpContext.Session.GetString("JWTToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            if (string.IsNullOrEmpty(sortOrder))
            {
                sortOrder = "price_asc"; // Thứ tự sắp xếp mặc định
            }

            var response = await _httpClient.GetAsync($"api/Phones/sorted?sortOrder={sortOrder}&pageNumber={pageNumber}&pageSize={pageSize}");
            response.EnsureSuccessStatusCode();

            var data = await response.Content.ReadAsStringAsync();
            var phones = JsonConvert.DeserializeObject<List<Phone>>(data);

            // Truyền các tham số thứ tự sắp xếp và phân trang đến view
            ViewBag.SortOrder = sortOrder;
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;

            return View(phones);
        }


        public IActionResult baiviet()
        {
            return View();
        }
        public IActionResult thongtin()
        {
            return View();
        }
      
        public IActionResult contact()
        {
            return View();
        }

        

        [HttpPost]
        public async Task<IActionResult> Contact(ContactModel model)
        {
            var token = HttpContext.Session.GetString("JWTToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            if (ModelState.IsValid)
            {
                var response = await _httpClient.PostAsJsonAsync("https://localhost:7069/api/Contact/Contact", model);

                if (response.IsSuccessStatusCode)
                {
                    ViewBag.Message = "Your message has been sent!";
                    return RedirectToAction("mailthanhcong");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "An error occurred while sending the message. Please try again later.");
                }
            }

            return View(model);
        }
        
        public IActionResult mailthanhcong()
        {

            var token = HttpContext.Session.GetString("JWTToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            ViewBag.Message = TempData["Message"];
            Console.WriteLine("ContactSuccess action called");
            return View();
        }
    }
}
