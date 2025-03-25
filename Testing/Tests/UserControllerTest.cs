using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AddressBookAPI.Controllers;
using BusinessLayer.Interface;
using BusinessLayer.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Middleware.RabbitMQ.Interface;
using Middleware.RabbitMQ.Service;
using ModelLayer.Model;
using Moq;
using NUnit.Framework;
using RepositoryLayer.Context;
using RepositoryLayer.Entity;
using RepositoryLayer.Helper;
using RepositoryLayer.Interface;
using RepositoryLayer.Service;

namespace Testing.Tests
{
    [TestFixture]
    public class UserControllerTest
    {
        private UserController _controller;
        private IUserBL _userBL;
        private ILogger<UserController> _logger;
        private Mock<IRabbitMQProducer> _mockRabbitMQProducer;
        private AppDbContext _context;

        [SetUp]
        public void Setup()
        {


            var options = new DbContextOptionsBuilder<AppDbContext>()
                    .UseInMemoryDatabase(databaseName: "TestAddressBookDb")
                    .Options;

            _context = new AppDbContext(options);
            IUserRL userRL = new UserRL(_context);
            var inMemorySettings = new Dictionary<string, string> {
                    {"Jwt:Key", "aB3f56vhyu890f98gfsd6f7g6sd7f8g6fs8df7g67sd8fg7sd8fg9aB3f56v"},
                    {"Jwt:Issuer", "testIssuer"},
                    {"Jwt:Audience", "testAudience"},
                    {"Jwt:ResetSecret", "bhds2342424234h2jbj2k44h2jk42" }
                };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            JwtTokenGenerator jwtTokenHelper = new JwtTokenGenerator(configuration);
            _userBL = new UserBL(userRL, jwtTokenHelper);

            _logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<UserController>();
            _mockRabbitMQProducer = new Mock<IRabbitMQProducer>();
            _controller = new UserController(_logger, _userBL, _mockRabbitMQProducer.Object);

        }

        [Test]
        public void Register_ReturnsOk_WhenSuccessful()
        {
            var model = new RegisterReq
            {
                UserName = "TestUser",
                Email = "test@example.com",
                Password = "password123"
            };

            var result = _controller.Register(model) as OkObjectResult;

            Assert.That(result.StatusCode, Is.EqualTo(200));
        }

        [Test]
        public void Login_ReturnsOk_WhenSuccessful()
        {
            var model = new LoginReq
            {
                Email = "test@example.com",
                Password = "password123"
            };

            var registerModel = new RegisterReq
            {
                UserName = "TestUser",
                Email = model.Email,
                Password = model.Password
            };
            _controller.Register(registerModel);

            var result = _controller.Login(model) as OkObjectResult;

            Assert.That(result.StatusCode, Is.EqualTo(200));
        }

        [Test]
        public async Task ForgotPassword_ReturnsOk_WhenEmailIsValid()
        {
            var model = new RegisterReq
            {
                UserName = "TestUser",
                Email = "vansh.verma_cs21@gla.ac.in",
                Password = "password123",
                Role = "User"
            };

            // Perform Registration
            var registerResponse = _controller.Register(model) as OkObjectResult;
            Assert.That(registerResponse, Is.Not.Null, "Register API returned null response.");
            Assert.That(registerResponse.StatusCode, Is.EqualTo(200), "Registration failed.");


            var response = registerResponse.Value as ResponseModel<string>;
            Assert.That(response, Is.Not.Null, "Response from Register API is null.");
            Assert.That(response.Success, Is.True, "User registration failed.");

            // Perform ForgotPassword
            var forgetModel = new ForgotPasswordReq { Email = model.Email };
            var result = await _controller.ForgotPassword(forgetModel) as OkObjectResult;

            // Assert API Response
            Assert.That(result, Is.Not.Null, "ForgotPassword API returned null response.");
            Assert.That(result.StatusCode, Is.EqualTo(200), "ForgotPassword API did not return status 200.");

            var forgotPasswordResponse = result.Value as ResponseModel<string>;
            Assert.That(forgotPasswordResponse, Is.Not.Null, "ForgotPassword response model is null.");
            Assert.That(forgotPasswordResponse.Success, Is.True, "ForgotPassword API failed.");
        }


        [Test]
        public void GetUserProfile_ReturnsOk_WhenUserIdExists()
        {
            var registerModel = new RegisterReq
            {
                UserName = "ProfileUser",
                Email = "profile@example.com",
                Password = "password123"
            };
            _controller.Register(registerModel);

            var userId = _context.Users.FirstOrDefault(u => u.Email == registerModel.Email)?.Id;
            var identity = new ClaimsIdentity(new[] { new Claim("UserId", userId.ToString()) });
            var user = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            var result = _controller.GetUserProfile() as OkObjectResult;

            Assert.That(result.StatusCode, Is.EqualTo(200));
        }
        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
