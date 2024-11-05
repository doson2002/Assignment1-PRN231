using BOs;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NuGet.Common;
using REPOs;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace PageSilver.Pages
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public string ErrorMessage = string.Empty;

        public string Token = string.Empty;

        public IndexModel(IHttpContextAccessor httpContextAccessor)
        {

            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7016/");
            _httpContextAccessor = httpContextAccessor;

        }

        public void OnGet()
        {
            // Code to handle GET request
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
            {
                ErrorMessage = "Email and Password cannot be empty.";
                return Page();
            }

            var loginModel = new
            {
                accountEmail = Email,
                accountPassword = Password
            };

            var jsonContent = new StringContent(JsonSerializer.Serialize(loginModel), Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync("/account/login", jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var jsonResponse = JsonDocument.Parse(responseContent);
                    Token = jsonResponse.RootElement.GetProperty("accessToken").GetString();

                    var handler = new JwtSecurityTokenHandler();

                    if (handler.ReadToken(Token) is not JwtSecurityToken jsonToken)
                        return RedirectToPage("SilverPage/Index");
                    var accountEmail = jsonToken.Claims.First(claim => claim.Type == "email").Value;
                    var accountId = jsonToken.Claims.First(claim => claim.Type == "sub").Value;
                    var role = jsonToken.Claims.First(claim => claim.Type == "role").Value;

                    if(role == "1" || role == "2")
                    {
                        _httpContextAccessor.HttpContext.Session.SetString("accessToken", Token);

                        AddRoleClaim(role, accountId, accountEmail);

                        return RedirectToPage("SilverPage/Index");
                    }
                    else
                    {
                        // Set success message in TempData
                        TempData["Message"] = "You dont have permission to access this page!";
                        return Page();
                    }
                  
                }

                ErrorMessage = "Case login unsuccessfully, display: You are not allowed to access this function!";
            }
            catch (HttpRequestException e)
            {
                ErrorMessage = $"Request error: {e.Message}";
            }

            return Page();
        }

        private void AddRoleClaim(string role, string userId, string email)
        {
            var claims = new List<Claim>
        {
            new(ClaimTypes.Role, role),
            new(ClaimTypes.NameIdentifier, userId),
            new(JwtRegisteredClaimNames.Email, email)
        };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }
    }
}