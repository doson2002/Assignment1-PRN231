using BOs.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPOs
{
    public interface IAccountRepo
    {
        public AccountDTO GetBranchAccount(String email, String password, JwtAuth jwtAuth);

    }
}
