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

namespace AuthorizationTest
{
    public class Tests
    {


        Mock<ITokenRepository> tokenRepositoryMock;
        Mock<IConfiguration> configMock;

        CustomerData context;
        TokenRepository tokenRepository;

        [SetUp]
        public void Setup()
        {            
            tokenRepositoryMock = new Mock<ITokenRepository>();
            configMock = new Mock<IConfiguration>();

            configMock.Setup(p => p["Jwt:Key"]).Returns("ThisIsMySecretKey");
            configMock.Setup(p => p["Jwt:Issuer"]).Returns("https://localhost:1959");
            configMock.Setup(p => p["Jwt:Expires"]).Returns("15");

            context = new Authorization.Context.CustomerData();
            tokenRepository = new TokenRepository();
            context = new CustomerData();
        }

        #region Token Repository testing
        [Test]
        public void CheckCredential_ValidCredential_ReturnsCustomerDetail()
        {
            //Arrange
            LoginModel loginModel = new LoginModel()
            {
                Username = "t1",
                Password = "123",
            };

            CustomerDetail customerDetail = tokenRepository.CheckCredential(loginModel);
            Assert.IsNotNull(customerDetail);
            Assert.AreEqual(customerDetail.Username, loginModel.Username);
        }

        [Test]
        public void CheckCredential_InvalidCredential_ReturnsNull()
        {
            //Arrange
            LoginModel loginModel = new LoginModel()
            {
                Username = "Ravi",
                Password = "123",
            };

            CustomerDetail customerDetail = tokenRepository.CheckCredential(loginModel);
            Assert.IsNull(customerDetail);
        }

        [Test]
        public void CheckCredential_InvalidLoginModel_ReturnsNull()
        {
            //Arrange
            LoginModel loginModel = null;

            Assert.Throws<NullReferenceException>(() => { tokenRepository.CheckCredential(loginModel); }) ;
        }

        [Test]
        public void GenerateToken_ValidCustomerDetail_ReturnsToken()
        {
            //Arrange
            CustomerDetail customerDetail = context.GetDetailByUsername("t1");
            
            //Act
            var data = tokenRepository.GenerateToken(configMock.Object, customerDetail);
            
            //Assert
            Assert.IsNotNull(data);
            Assert.AreEqual("string".GetType(), data.GetType());
        }

        [Test]
        public void GenerateToken_InvalidCustomerDetail_ThrowsException()
        {
            //Arrange
            CustomerDetail customerDetail = null;

            var exceptionMessage = Assert.Throws<NullReferenceException>(() => tokenRepository.GenerateToken(configMock.Object, customerDetail));

            Assert.AreEqual("Object reference not set to an instance of an object.", exceptionMessage.Message);
        }

        #endregion

        #region Auth Controller testing

        [Test]
        public void LoginAction_ValidCredentials_ReturnsLoginResponse() {
            Customer customer = context.GetByUsername("t1");
            CustomerDetail customerDetail = context.GetDetailByUsername("t1");
            LoginModel loginModel = new LoginModel()
            {
                Username = customer.Username,
                Password = customer.Password
            };

            tokenRepositoryMock.Setup(p => p.CheckCredential(loginModel)).Returns(customerDetail);

            AuthController loginController = new AuthController(configMock.Object, tokenRepositoryMock.Object);

            var response = loginController.Login(loginModel);
            Assert.IsNotNull(response);
            var result = response as OkObjectResult;
            Assert.AreEqual(200, result.StatusCode);
        }

        [Test]
        public void LoginAction_MiscError_ReturnsInternalServerError() {
            LoginModel loginModel = new LoginModel()
            {
                Username = "t1",
                Password = "123"
            };

            tokenRepositoryMock.Setup(p => p.CheckCredential(loginModel)).Throws<Exception>();

            AuthController loginController = new AuthController(configMock.Object, tokenRepositoryMock.Object);
            var response = loginController.Login(loginModel);

            Assert.IsNotNull(response);
            var result = response as StatusCodeResult ;
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

            tokenRepositoryMock.Setup(p => p.CheckCredential(loginModel)).Returns(customerDetail);

            AuthController loginController = new AuthController(configMock.Object, tokenRepositoryMock.Object);
            var response = loginController.Login(loginModel);

            Assert.IsNotNull(response);
            var result = response as UnauthorizedObjectResult;
            Assert.AreEqual(401, result.StatusCode);
        }
        #endregion
    }
}