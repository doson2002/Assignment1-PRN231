using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BOs;
using DAOs;
using System.Text.Json;
using System.Net.Http.Headers;

namespace PageSilver.Pages.SilverPage
{
    public class IndexModel : PageModel

    {
        private readonly HttpClient httpClient;

        public IndexModel()
        {
            httpClient = new HttpClient();
        }

        public IList<SilverJewelry> SilverJewelry { get;set; } = default!;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 5;
        public bool HasNextPage { get; set; }
        public int TotalPages { get; set; }



        public async Task OnGetAsync(int pageNumber = 1)
        {
            PageNumber = pageNumber;
            int skip = (PageNumber - 1) * PageSize;

            // Lấy Bearer Token từ session
            var token = HttpContext.Session.GetString("accessToken");

            // Tạo một yêu cầu HTTP với Bearer Token
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"https://localhost:7016/silver/get_all?$top={PageSize}&$skip={skip}"))
            {
                if (!string.IsNullOrEmpty(token))
                {
                    requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                HttpResponseMessage response = await httpClient.SendAsync(requestMessage);

                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var data = await response.Content.ReadAsStringAsync();
                    SilverJewelry = JsonSerializer.Deserialize<List<SilverJewelry>>(data, options);

                    // Xác định xem có trang tiếp theo không
                    HasNextPage = SilverJewelry.Count == PageSize;
                }
                else
                {
                    // Xử lý lỗi nếu cần
                }
            }
        }
    }
}
