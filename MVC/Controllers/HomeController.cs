using Microsoft.AspNetCore.Mvc;
using MVC.Models;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Collections.Generic;
using Web2API.Controllers;
using System.Text.Json;
using System.Text.Json;

namespace MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new System.Uri("https://localhost:7069/");
        }
        [HttpGet]
        public IActionResult GetJwtToken()
        {
            var token = HttpContext.Session.GetString("JWTToken");
            return Ok(new { jwtToken = token });
        }
        public async Task<IActionResult> Search(string query)
        {
            var token = HttpContext.Session.GetString("JWTToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            if (string.IsNullOrEmpty(query))
            {
                return View(new List<Phone>());
            }

            var response = await _httpClient.GetAsync($"https://localhost:7069/api/Phones/search?query={query}");

            if (response.IsSuccessStatusCode)
            {
                var phones = await response.Content.ReadFromJsonAsync<IEnumerable<Phone>>();
                return View(phones);
            }
           
            return View(new List<Phone>());
        }
        public async Task<IActionResult> Index()
        {
            var token = HttpContext.Session.GetString("JWTToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync("api/Phones");
            response.EnsureSuccessStatusCode();

            var data = await response.Content.ReadAsStringAsync();
            var phones = JsonConvert.DeserializeObject<List<Phone>>(data);
      
            return View(phones);
        }
        public async Task<IActionResult> Details(int id)
        {
            var token = HttpContext.Session.GetString("JWTToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync($"https://localhost:7069/api/Phones/{id}");
            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                var phone = System.Text.Json.JsonSerializer.Deserialize<Phone>(responseData, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return View(phone);
            }

            return NotFound();
        }

        [HttpPost]

        public async Task<IActionResult> Create(Phone phone)
        {
            var token = HttpContext.Session.GetString("JWTToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.PostAsJsonAsync("api/Phones", phone);
            response.EnsureSuccessStatusCode();
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var token = HttpContext.Session.GetString("JWTToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync($"api/Phones/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var data = await response.Content.ReadAsStringAsync();
            var phone = JsonConvert.DeserializeObject<Phone>(data);
            return View(phone);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Phone phone, IFormFile imageFile)
        {
            var token = HttpContext.Session.GetString("JWTToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            if (ModelState.IsValid)
            {
                using (var content = new MultipartFormDataContent())
                {
                    content.Add(new StringContent(phone.Id.ToString()), "Id");
                    content.Add(new StringContent(phone.Name), "Name");
                    content.Add(new StringContent(phone.Manufacturer), "Manufacturer");
                    content.Add(new StringContent(phone.Price.ToString()), "Price");

                    if (imageFile != null && imageFile.Length > 0)
                    {
                        var fileContent = new StreamContent(imageFile.OpenReadStream());
                        fileContent.Headers.ContentType = new MediaTypeHeaderValue(imageFile.ContentType);
                        content.Add(fileContent, "ImageURL", imageFile.FileName);
                    }

                    var response = await _httpClient.PutAsync($"api/Phones/{phone.Id}", content);

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                    }
                }
            }

            return View(phone);
        }
    
    [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var token = HttpContext.Session.GetString("JWTToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.DeleteAsync($"api/Phones/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, response.Content.ReadAsStringAsync().Result);
            }

            return RedirectToAction(nameof(Index));
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        // Model Phone
      
    }
}
