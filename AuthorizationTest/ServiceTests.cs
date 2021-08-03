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
    class ServiceTests
    {
        // Basic Dependency
        ICustomerRepository customerRepository;

        // Mock Dependencies
        Mock<ICustomerRepository> customerRepositoryMock;
        Mock<IConfiguration> configMock;

        // Object to be tested
        IAuthService authService;

        [SetUp]
        public void Setup()
        {
            customerRepository = new CustomerRepository();

            // Setup and initialize common mock dependencies
            customerRepositoryMock = new Mock<ICustomerRepository>();
            customerRepositoryMock.Setup(p => p.GetCustomers()).Returns(customerRepository.GetCustomers());

            // Setup and initialize common mock dependencies
            configMock = new Mock<IConfiguration>();
            configMock.Setup(p => p["Jwt:Key"]).Returns("ThisIsMySecretKey");
            configMock.Setup(p => p["Jwt:Issuer"]).Returns("https://localhost:1959");
            configMock.Setup(p => p["Jwt:Expires"]).Returns("15");

        }

        #region Auth Service testing
        [Test]
        public void ValidateCredential_ValidCredential_ReturnsCustomerDetail()
        {
            //Init Login model with correct login credentials
            LoginModel loginModel = new LoginModel()
            {
                Username = "t1",
                Password = "123",
            };

            // Init object to be tested with mock dependencies
            authService = new AuthService(customerRepositoryMock.Object);

            // Get the Reponse
            CustomerDetail customerDetail = authService.ValidateCredential(loginModel);
            
            // Validate the Response
            Assert.IsNotNull(customerDetail);
            Assert.AreEqual(customerDetail.Username, loginModel.Username);
        }

        [Test]
        public void ValidateCredential_InvalidCredential_ReturnsNull()
        {
            //Arrange
            LoginModel loginModel = new LoginModel()
            {
                Username = "Ravi",
                Password = "123",
            };

            // Init object to be tested with mock dependencies
            authService = new AuthService(customerRepositoryMock.Object);

            // Get the response
            CustomerDetail customerDetail = authService.ValidateCredential(loginModel);
            
            // Validate response
            Assert.IsNull(customerDetail);
        }

        [Test]
        public void ValidateCredential_InvalidLoginModel_ReturnsNull()
        {
            // Init Invalid Input
            LoginModel loginModel = null;

            authService = new AuthService(customerRepositoryMock.Object);

            Assert.Throws<NullReferenceException>(() => { authService.ValidateCredential(loginModel); });
        }

        [Test]
        public void GenerateToken_ValidCustomerDetail_ReturnsToken()
        {
            //Arrange
            CustomerDetail customerDetail = customerRepository.GetCustomerDetail("t1");

            authService = new AuthService(customerRepositoryMock.Object);

            //Act
            var token = authService.GenerateToken(configMock.Object, customerDetail);

            //Assert
            Assert.IsNotNull(token);
            Assert.AreEqual("string".GetType(), token.GetType());
        }

        [Test]
        public void GenerateToken_InvalidCustomerDetail_ThrowsException()
        {
            //Arrange
            CustomerDetail customerDetail = null;
            authService = new AuthService(customerRepositoryMock.Object);

            //Assert
            var exceptionMessage = Assert.Throws<NullReferenceException>(() => authService.GenerateToken(configMock.Object, customerDetail));
            Assert.AreEqual("Object reference not set to an instance of an object.", exceptionMessage.Message);
        }

        #endregion

    }
}
