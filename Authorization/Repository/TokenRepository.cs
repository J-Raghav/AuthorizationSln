using Authorization.Context;
using Authorization.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Authorization.Repository
{

    public class TokenRepository : ITokenRepository
    {

        protected CustomerData _context; 
        
        public TokenRepository() {
            _context = new CustomerData();
        }

        public CustomerDetail CheckCredential(LoginModel login)
        {

            string username = login.Username;
            string password = login.Password;
            CustomerDetail customerDetail = null;

            Customer customer = _context.Customers.Where(customer =>
                customer.Username == username
                && customer.Password == password)
                .FirstOrDefault();

            if (customer != null)
            {
                customerDetail = new CustomerDetail()
                {
                    Username = customer.Username,
                    PortfolioId = customer.PortfolioId
                };
            }
            return customerDetail;
        }

        public string GenerateToken(IConfiguration _config, CustomerDetail customerDetail)
        {
/*            try
            {*/
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                List<Claim> claims = new List<Claim>() {
                    new Claim(ClaimTypes.Name, customerDetail.Username),
                    new Claim("PortfolioId", customerDetail.PortfolioId.ToString())
                };
                SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Issuer = _config["Jwt:Issuer"],
                    Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_config["Jwt:Expires"])),
                    SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature)
                };
                JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
                JwtSecurityToken token = handler.CreateJwtSecurityToken(descriptor);
                return handler.WriteToken(token);

/*            }
            catch (Exception e)
            {
                throw e;
            }*/

        }

    }
}
