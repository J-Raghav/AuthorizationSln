using Authorization.Models;
using Authorization.Repository;
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
        private readonly ITokenRepository _tokenRepository;
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(AuthController));

        public AuthController(IConfiguration config, ITokenRepository tokenRepository){
            _config = config;
            _tokenRepository = tokenRepository;
        }

        [HttpPost]
        [Route("Login")]
        [AllowAnonymous]
        public IActionResult Login([FromBody] LoginModel model)
        {

            try
            {
                _log4net.Info(nameof(Login) + " method invoked, Username : " + model.Username);

                CustomerDetail customerDetail = _tokenRepository.CheckCredential(model);
                if (customerDetail != null)
                {
                    string Token = _tokenRepository.GenerateToken(_config, customerDetail);
                    var loginResponse = new LoginResponse(){
                        Username = customerDetail.Username,
                        PortfolioId = customerDetail.PortfolioId,
                        Token = Token
                    };
                    _log4net.Info("Login Successful for "+model.Username);
                    return Ok(loginResponse);
                }

                return Unauthorized("Invalid Credentials");
            }
            catch (Exception e)
            {
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
