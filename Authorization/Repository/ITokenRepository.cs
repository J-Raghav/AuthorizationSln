using Authorization.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Authorization.Repository
{
    public interface ITokenRepository
    {
        public CustomerDetail CheckCredential(LoginModel loginModel);

        public string GenerateToken(IConfiguration _config, CustomerDetail customerDetail);

    }
}
