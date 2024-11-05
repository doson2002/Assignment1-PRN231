using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BOs;
using DAOs;
using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;

namespace PageSilver.Pages.SilverPage
{
    public class EditModel : PageModel
    {
        private readonly HttpClient httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EditModel(IHttpContextAccessor httpContextAccessor)
        {
            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://localhost:7016/");
            _httpContextAccessor = httpContextAccessor;
        }

        [BindProperty]
        public SilverJewelry SilverJewelry { get; set; } = default!;

        [BindProperty]
        public List<Category> CategoryList { get; set; } = default!;

        private String ErrorMessage = string.Empty;


        public async Task<IActionResult> OnGetAsync(string id)
        {
            // Lấy token từ session
            var token = _httpContextAccessor.HttpContext.Session.GetString("accessToken");

            if (string.IsNullOrEmpty(token))
            {
                ErrorMessage = "Token not found. Please log in again.";
                return RedirectToPage("/Index");
            }

            // Kiểm tra role trong token
            var handler = new JwtSecurityTokenHandler();
            if (handler.ReadToken(token) is JwtSecurityToken jwtToken)
            {
                // Lấy giá trị của claim "role"
                var role = jwtToken.Claims.FirstOrDefault(c => c.Type == "role")?.Value;

                // Kiểm tra nếu role = 1
                if (role == "1")
                {
                    // Người dùng có role = 1, tiếp tục xử lý như mong muốn
                    if (string.IsNullOrEmpty(id))
                    {
                        return NotFound();
                    }

                    // Tạo yêu cầu HTTP với Bearer Token để lấy SilverJewelry theo ID
                    using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"https://localhost:7016/silver/get_silver_by_id?id={id}"))
                    {
                        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                        // Gọi API
                        var silverJewelryResponse = await httpClient.SendAsync(requestMessage);

                        if (silverJewelryResponse.IsSuccessStatusCode)
                        {
                            var options = new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true,
                            };

                            var silverJewelryData = await silverJewelryResponse.Content.ReadAsStringAsync();
                            SilverJewelry = JsonSerializer.Deserialize<SilverJewelry>(silverJewelryData, options);
                        }
                        else
                        {
                            return NotFound("SilverJewelry not found.");
                        }

                        // Call API to get Category List với Bearer Token
                        using (var categoryRequestMessage = new HttpRequestMessage(HttpMethod.Get, "https://localhost:7016/category/get_all"))
                        {
                            categoryRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                            var categoryResponse = await httpClient.SendAsync(categoryRequestMessage);

                            if (categoryResponse.IsSuccessStatusCode)
                            {
                                var options = new JsonSerializerOptions
                                {
                                    PropertyNameCaseInsensitive = true,
                                };
                                var categoryData = await categoryResponse.Content.ReadAsStringAsync();
                                CategoryList = JsonSerializer.Deserialize<List<Category>>(categoryData, options);
                                // Convert categories to SelectList
                                ViewData["CategoryId"] = new SelectList(CategoryList, "CategoryId", "CategoryName");
                            }
                            else
                            {
                                ViewData["CategoryId"] = new SelectList(new List<Category>(), "CategoryId", "CategoryName");
                            }
                        }

                        return Page();
                    }
                }
                else
                {
                    ErrorMessage = "You do not have the required permissions to access this page.";
                    return RedirectToPage("/Error");
                }
            }
            else
            {
                ErrorMessage = "Invalid token. Please log in again.";
                return RedirectToPage("/Account/Login");
            }
        }


        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // Gọi lại OnGetAsync để nạp lại danh sách category
                await OnGetAsync(SilverJewelry.SilverJewelryId); // Sử dụng ID của SilverJewelry để gọi lại
                return Page();
            }

            // Serialize SilverJewelry object to JSON
            var content = new StringContent(JsonSerializer.Serialize(SilverJewelry), Encoding.UTF8, "application/json");

            // Lấy Bearer Token từ session
            var token = HttpContext.Session.GetString("accessToken");

            // Tạo một yêu cầu HTTP với Bearer Token
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Put, "silver/update"))
            {
                if (!string.IsNullOrEmpty(token))
                {
                    requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                requestMessage.Content = content;

                // Gọi API để cập nhật SilverJewelry
                var response = await httpClient.SendAsync(requestMessage);

                if (response.IsSuccessStatusCode)
                {
                    // Set success message in TempData
                    TempData["SuccessMessage"] = "Update successful!";
                    return RedirectToPage("./Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Unable to update SilverJewelry.");
                    // Gọi lại OnGetAsync để nạp lại danh sách category
                    await OnGetAsync(SilverJewelry.SilverJewelryId); // Sử dụng ID của SilverJewelry để gọi lại
                    return Page();
                }
            }
        }



    }
}
