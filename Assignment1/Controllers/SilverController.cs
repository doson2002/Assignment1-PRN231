using BOs.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.OData.Formatter;
using REPOs;
using BAL.Authentications;
using BOs;

namespace Assignment1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SilverController : ControllerBase
    {
        private readonly ISilverRepo silverRepo;
        private readonly IAccountRepo accountRepo;
        private readonly ICategoryRepo categoryRepo;
        private IOptions<JwtAuth> jwtAuthOptions;

        public SilverController(ISilverRepo silverRepo, IAccountRepo accountRepo, ICategoryRepo categoryRepo, IOptions<JwtAuth> jwtAuthOptions)
        {
            this.silverRepo = silverRepo;
            this.accountRepo = accountRepo;
            this.categoryRepo = categoryRepo;
            this.jwtAuthOptions = jwtAuthOptions;
        }
        [HttpGet("/category/get_all")]
        [EnableQuery]
        [PermissionAuthorize(1,2)]
        public IActionResult GetAllCategory()
        {
            return Ok(categoryRepo.GetCategories());
        }

        [HttpGet("/category/get_category_by_id")]
        [EnableQuery]
        [PermissionAuthorize(1,2)]
        public IActionResult GetCategoryById([FromODataUri] string id)
        {
            var entity = categoryRepo.GetCategory(id);
            if (entity == null)
            {
                return NotFound();
            }
            return Ok(entity);
        }

        [HttpPost("/account/login")]
        [EnableQuery]
        public IActionResult Post([FromBody] AccountLoginDTO accountLoginDTO)
        {
            try
            {
               
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                AccountDTO getAccount = accountRepo.GetBranchAccount(accountLoginDTO.AccountEmail, accountLoginDTO.AccountPassword, jwtAuthOptions.Value);
                return Ok(getAccount);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Message = ex.Message,
                });

            }
        }

        [HttpPost("/silver/create")]
        [EnableQuery]
        [PermissionAuthorize(1)]
        public IActionResult CreateSilver([FromBody] SilverJewelry createSilver)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Model is not valid",
                        Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                    });
                }

                bool isAdded = this.silverRepo.AddJewelry(createSilver);

                if (isAdded)
                {
                    return Ok(new
                    {
                        Success = true,
                        Message = "Jewelry added successfully",
                        Data = createSilver
                    });
                }
                else
                {
                    return Conflict(new
                    {
                        Success = false,
                        Message = "Jewelry with this ID already exists."
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "An error occurred while adding the jewelry.",
                    Error = ex.Message
                });
            }
        }

        [HttpPut("/silver/update")]
        [EnableQuery]
        [PermissionAuthorize(1)]
        public IActionResult UpdateSilver([FromBody] SilverJewelry silverJewelry)
        {
            try
            {
                // Kiểm tra Model có hợp lệ không
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Gọi hàm cập nhật
                var result = silverRepo.UpdateJewelry(silverJewelry);

                // Kiểm tra kết quả cập nhật
                if (result)
                {
                    return Ok(new { Message = "Jewelry updated successfully." });
                }
                else
                {
                    return NotFound(new { Message = "Jewelry not found." });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
        [HttpDelete("/silver/delete")]
        [EnableQuery]
        [PermissionAuthorize(1)]
        public IActionResult deleteSilver([FromODataUri] string id)
        {
            try
            {
                var entity = silverRepo.GetSilver(id);
                if (entity == null)
                {
                    return NotFound();
                }

                // Gọi hàm cập nhật
                var result = silverRepo.RemoveJewelry(id);

                // Kiểm tra kết quả cập nhật
                if (result)
                {
                    return Ok(new { Message = "Jewelry deleted successfully." });
                }
                else
                {
                    return NotFound(new { Message = "Jewelry not found." });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
        [HttpGet("/silver/get_all")]
        [EnableQuery]
        [PermissionAuthorize(1,2)]
        public IActionResult GetAllSilver()
        {
            return Ok(silverRepo.GetSilvers());
        }

        [HttpGet("/silver/get_silver_by_id")]
        [EnableQuery]
        [PermissionAuthorize(1,2)]
        public IActionResult GetById([FromODataUri] string id)
        {
            var entity = silverRepo.GetSilver(id);
            if (entity == null)
            {
                return NotFound();
            }
            return Ok(entity);
        }

    }
}
