using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOs.DTO
{
    public class AccountLoginDTO
    {
        [Required(ErrorMessage = "Email is Required")]
        public string AccountEmail { get; set; }
        [Required(ErrorMessage = "Password is Required")]
        public string AccountPassword { get; set; } = null!;
    }
}
