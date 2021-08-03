using Authorization.Context;
using NUnit.Framework;
using Moq;
using Authorization.Repository;
using Microsoft.Extensions.Configuration;
using Authorization.Models;
using System.Collections.Generic;
using System;
using Authorization.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Authorization.Services;

namespace AuthorizationTest
{
    class ControllerTests
    {
        CustomerData context;
        ICustomerRepository customerRepository;

        Mock<IAuthService> authServiceMock;
        Mock<ICustomerRepository> customerRepositoryMock;
        Mock<IConfiguration> configMock;

        [SetUp]
        public void Setup()
        {
            context = new CustomerData();
            customerRepository = new CustomerRepository();

            authServiceMock = new Mock<IAuthService>();

            customerRepositoryMock = new Mock<ICustomerRepository>();
            customerRepositoryMock.Setup(p => p.GetCustomers()).Returns(customerRepository.GetCustomers());
            
            configMock = new Mock<IConfiguration>();
            configMock.Setup(p => p["Jwt:Key"]).Returns("ThisIsMySecretKey");
            configMock.Setup(p => p["Jwt:Issuer"]).Returns("https://localhost:1959");
            configMock.Setup(p => p["Jwt:Expires"]).Returns("15");

        }

        #region Auth Controller testing

        [Test]
        public void LoginAction_ValidCredentials_ReturnsLoginResponse()
        {
            Customer customer = customerRepository.GetCustomer("t1");
            CustomerDetail customerDetail = customerRepository.GetCustomerDetail("t1");
            LoginModel loginModel = new LoginModel()
            {
                Username = customer.Username,
                Password = customer.Password
            };

            authServiceMock.Setup(p => p.ValidateCredential(loginModel)).Returns(customerDetail);

            AuthController loginController = new AuthController(configMock.Object, authServiceMock.Object);

            var response = loginController.Login(loginModel);
            Assert.IsNotNull(response);
            var result = response as OkObjectResult;
            Assert.AreEqual(200, result.StatusCode);
        }

        [Test]
        public void LoginAction_MiscError_ReturnsInternalServerError()
        {
            LoginModel loginModel = new LoginModel()
            {
                Username = "t1",
                Password = "123"
            };

            authServiceMock.Setup(p => p.ValidateCredential(loginModel)).Throws<Exception>();

            AuthController loginController = new AuthController(configMock.Object, authServiceMock.Object);
            var response = loginController.Login(loginModel);

            Assert.IsNotNull(response);
            var result = response as StatusCodeResult;
            Assert.AreEqual(500, result.StatusCode);

        }

        [Test]
        public void LoginAction_InvalidCredentials_ReturnsUnauthorized()
        {
            CustomerDetail customerDetail = null;
            LoginModel loginModel = new LoginModel()
            {
                Username = "Ram",
                Password = "123"
            };

            authServiceMock.Setup(p => p.ValidateCredential(loginModel)).Returns(customerDetail);

            AuthController loginController = new AuthController(configMock.Object, authServiceMock.Object);
            var response = loginController.Login(loginModel);

            Assert.IsNotNull(response);
            var result = response as UnauthorizedObjectResult;
            Assert.AreEqual(401, result.StatusCode);
        }
        #endregion
    }
}
