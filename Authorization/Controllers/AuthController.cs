using Authorization.Models;
using Authorization.Repository;
using Authorization.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Authorization.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IAuthService _authService;
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(AuthController));

        public AuthController(IConfiguration config, IAuthService authService){
            _config = config;
            _authService = authService;
        }

        [HttpPost]
        [Route("Login")]
        [AllowAnonymous]
        public IActionResult Login([FromBody] LoginModel model)
        {

            try
            {
                _log4net.Info(nameof(Login) + " method invoked, Username : " + model.Username);

                // Verifying and storing user credentials
                CustomerDetail customerDetail = _authService.ValidateCredential(model);

                // false if user details are invalid or not present in data layer
                if (customerDetail != null)
                {
                    // Generates token with user's details
                    string Token = _authService.GenerateToken(_config, customerDetail);
                    
                    var loginResponse = new LoginResponse(){
                        Username = customerDetail.Username,
                        PortfolioId = customerDetail.PortfolioId,
                        Token = Token
                    };
                    
                    _log4net.Info("Login Successful for " + model.Username);
                    
                    //Returns details of user with jwt token
                    return Ok(loginResponse);
                }

                // Returns these if user details are null
                return Unauthorized("Invalid Credentials");
            }
            catch (Exception e)
            {
                // Catches and logs the exception happend during execution of controller logic
                _log4net.Error("Error Occured from " + nameof(Login) + "Error Message : " + e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet]
        [Route("ValidateToken")]
        public IActionResult ValidateToken() {
            try
            {
                List<Claim> claims = User.Claims.ToList();

                string Username = User.Identity.Name;
                int PortfolioId = Convert.ToInt32(claims.First(claim => claim.Type == "PortfolioId").Value);

                CustomerDetail customerDetail = new CustomerDetail()
                {
                    Username = Username,
                    PortfolioId = PortfolioId
                };

                return Ok(customerDetail);
            }
            catch (Exception e) {
                _log4net.Error("Error Occured from " + nameof(ValidateToken) + "Error Message : " + e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

    }
}
