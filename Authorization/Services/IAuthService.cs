using Authorization.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Authorization.Services
{
    public interface IAuthService
    {
        public CustomerDetail ValidateCredential(LoginModel loginModel);

        public string GenerateToken(IConfiguration _config, CustomerDetail customerDetail);
    }
}
