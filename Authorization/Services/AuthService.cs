using Authorization.Context;
using Authorization.Models;
using Authorization.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Authorization.Services
{
    public class AuthService : IAuthService
    {

        protected readonly ICustomerRepository _customerRepository;

        public AuthService(ICustomerRepository customerRepository) {
            _customerRepository = customerRepository;
        }
        
        public CustomerDetail ValidateCredential(LoginModel login)
        {
        
            string username = login.Username;
            string password = login.Password;
            
            CustomerDetail customerDetail = null;
            List<Customer> customers = _customerRepository.GetCustomers();
            
            Customer customer = customers.FirstOrDefault(customer =>
                customer.Username == username
                && customer.Password == password);

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
            // Prepare credentials which will be used to sign the token
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Claims to be present in token
            List<Claim> claims = new List<Claim>() {
                new Claim(ClaimTypes.Name, customerDetail.Username),
                new Claim("PortfolioId", customerDetail.PortfolioId.ToString())
            };

            SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Issuer = _config["Jwt:Issuer"],
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_config["Jwt:Expires"])),
                SigningCredentials = credentials
            };

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            JwtSecurityToken token = handler.CreateJwtSecurityToken(descriptor);

            return handler.WriteToken(token);



            /*
            //Extracting out the issuer, key and expires from appsettings.json file.
            var issuer = _config["Jwt:Issuer"];
            var key = _config["Jwt:Key"];
            var expires = _config["Jwt:Expires"];

            //Step1- Creating the claims list.
            List<Claim> claims = new List<Claim>()
            {
                 //Adding my custom data.
                 new Claim("Username",customerDetail.Username),
                 new Claim("PortfolioId",value:customerDetail.PortfolioId.ToString())
            };

            //Step2- Creating a symmetric key from key.
            byte[] keybytes = Encoding.UTF8.GetBytes(key);
            var sm_key = new SymmetricSecurityKey(keybytes);

            //Step3- creds represents the key and the cryptographic algo whose combination is
            //used to produce the digital signature.
            var creds = new SigningCredentials(sm_key, SecurityAlgorithms.HmacSha256);

            //Step4- Creating the JWT token.
            var token = new JwtSecurityToken(issuer, null, claims, expires: DateTime.UtcNow.AddMinutes(double.Parse(expires)), signingCredentials: creds);

            //Step5- Return the serialized version of the token.
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            return handler.WriteToken(token);
            */
        }
    }
}
