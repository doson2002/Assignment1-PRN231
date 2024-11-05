using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using BOs;
using DAOs;
using System.Text.Json;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;

namespace PageSilver.Pages.SilverPage
{
    public class CreateModel : PageModel
    {
        private readonly HttpClient httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CreateModel(IHttpContextAccessor httpContextAccessor)
        {
            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://localhost:7016/");
            _httpContextAccessor = httpContextAccessor; 
        }

        [BindProperty]
        public List<Category> CategoryList { get; set; } = default!;
        public async Task<IActionResult> OnGet()
        {
            // Lấy token từ session
            var token = _httpContextAccessor.HttpContext.Session.GetString("accessToken");

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Index");
            }

            // Kiểm tra role trong token
            var handler = new JwtSecurityTokenHandler();
            if (handler.ReadToken(token) is JwtSecurityToken jwtToken)
            {
                // Lấy giá trị của claim "role"
                var role = jwtToken.Claims.FirstOrDefault(c => c.Type == "role")?.Value;

                // Kiểm tra nếu role = 1 hoặc role = 2
                if (role == "1")
                {
                    // Tạo yêu cầu HTTP với Bearer Token
                    using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, "https://localhost:7016/category/get_all"))
                    {
                        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                        // Gọi API để lấy danh sách category
                        var categoryResponse = await httpClient.SendAsync(requestMessage);

                        if (categoryResponse.IsSuccessStatusCode)
                        {
                            var options = new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true,
                            };
                            var categoryData = await categoryResponse.Content.ReadAsStringAsync();
                            CategoryList = JsonSerializer.Deserialize<List<Category>>(categoryData, options);

                            // Chuyển đổi danh sách category thành SelectList
                            ViewData["CategoryId"] = new SelectList(CategoryList, "CategoryId", "CategoryName");
                        }
                        else
                        {
                            ViewData["CategoryId"] = new SelectList(new List<Category>(), "CategoryId", "CategoryName");
                        }
                    }
                }
                else
                {
                    return RedirectToPage("/Error");
                }
            }
            else
            {
                return RedirectToPage("/Index");
            }

            return Page();
        }


        [BindProperty]
        public SilverJewelry SilverJewelry { get; set; } = default!;


        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await OnGet();
                return Page();
            }

            // Serialize SilverJewelry object to JSON
            var content = new StringContent(JsonSerializer.Serialize(SilverJewelry), Encoding.UTF8, "application/json");

            // Lấy Bearer Token từ session
            var token = _httpContextAccessor.HttpContext.Session.GetString("accessToken");

            // Tạo yêu cầu HTTP với Bearer Token
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, "https://localhost:7016/silver/create"))
            {
                if (!string.IsNullOrEmpty(token))
                {
                    requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                requestMessage.Content = content;

                // Call API to create SilverJewelry
                var response = await httpClient.SendAsync(requestMessage);

                if (response.IsSuccessStatusCode)
                {
                    // Set success message in TempData
                    TempData["SuccessMessage"] = "Create successful!";
                    return RedirectToPage("./Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Unable to create SilverJewelry.");
                    await OnGet();
                    return Page();
                }
            }
        }

    }

}
