using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BOs;
using DAOs;
using System.Net.Http;
using System.Text.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;

namespace PageSilver.Pages.SilverPage
{
    public class DeleteModel : PageModel
    {
        private readonly HttpClient httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public DeleteModel(IHttpContextAccessor httpContextAccessor)
        {
           httpClient = new HttpClient();
            _httpContextAccessor = httpContextAccessor;

        }

        [BindProperty]
      public SilverJewelry SilverJewelry { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
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
                    if (string.IsNullOrEmpty(id))
                    {
                        return BadRequest("ID cannot be null or empty.");
                    }

                    // Tạo yêu cầu HTTP với Bearer Token
                    using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"https://localhost:7016/silver/get_silver_by_id?id={id}"))
                    {
                        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                        // Gọi API để lấy SilverJewelry theo ID
                        HttpResponseMessage response = await httpClient.SendAsync(requestMessage);

                        if (response.IsSuccessStatusCode)
                        {
                            var options = new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true,
                            };
                            var data = await response.Content.ReadAsStringAsync();
                            SilverJewelry = JsonSerializer.Deserialize<SilverJewelry>(data, options);

                            return Page();
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "Error retrieving SilverJewelry data.");
                            return Page();
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
        }


        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("ID cannot be null or empty.");
            }

            // Lấy Bearer Token từ session
            var token = HttpContext.Session.GetString("accessToken");

            // Tạo yêu cầu HTTP với Bearer Token
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Delete, $"https://localhost:7016/silver/delete?id={id}"))
            {
                if (!string.IsNullOrEmpty(token))
                {
                    requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                // Gọi API để xóa SilverJewelry theo ID
                HttpResponseMessage deleteResponse = await httpClient.SendAsync(requestMessage);

                if (deleteResponse.IsSuccessStatusCode)
                {
                    // Set success message in TempData
                    TempData["SuccessMessage"] = "Delete successful!";
                    // Successfully deleted
                    return RedirectToPage("./Index");
                }
                else
                {
                    // Handle failure
                    ModelState.AddModelError(string.Empty, "Error deleting the item. Please try again.");
                    return Page();
                }
            }
        }

    }
}
