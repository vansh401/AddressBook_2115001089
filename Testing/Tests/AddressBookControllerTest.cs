using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AddressBookAPI.Controllers;
using BusinessLayer.Interface;
using BusinessLayer.Service;
using CacheLayer.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using RepositoryLayer.Context;
using RepositoryLayer.Entity;
using RepositoryLayer.Service;
using StackExchange.Redis;

namespace Testing.Tests
{
    public class AddressBookControllerTest
    {
        private AddressBookAPIController _controller;
        private AppDbContext _context;
        private AddressBookRL _repo;
        private AddressBookService _service;
        private IRedisCacheService _redisCacheService;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
            _context = new AppDbContext(options);

            _repo = new AddressBookRL(_context);
            _service = new AddressBookService(_repo);

            var redisMock = ConnectionMultiplexer.Connect("localhost:6379");
            _redisCacheService = new RedisCacheService(redisMock);

            var logger = new LoggerFactory().CreateLogger<AddressBookAPIController>();
            _controller = new AddressBookAPIController(logger, _service, _redisCacheService);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim("userId", "2")
            }));
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            _context.AddressBookContacts.Add(new AddressBookEntity
            {
                Id = 1,
                UserId = 2,
                Name = "Andre",
                Number = "123456",
                Email = "Andre@mail.com",
                Address = "TestAddress"
            });
            _context.SaveChanges();
        }

        [Test]
        public async Task GetAllContactsTest()
        {
            var result = await _controller.GetAllContacts() as ObjectResult;
            Assert.That(result.StatusCode, Is.EqualTo(200));
        }

        [Test]
        public async Task GetContactByIdTest()
        {
            var result=await _controller.GetContactById(1) as ObjectResult;
            Assert.That(result.StatusCode, Is.EqualTo(200));
        }

        [Test]
        public void AddContactTest()
        {
            var result = _controller.AddContact("John","9999999999","john@gmail.com","newAddress") as ObjectResult;
            Console.WriteLine(result);
            Assert.That(result.StatusCode,Is.EqualTo(200));
        }

        [Test]
        public async Task UpdateContactByIdTest()
        {
            var result = await _controller.UpdateContactById(1, "UpdatedName", "0000000000", "updated@gmail.com","updatedAddress") as ObjectResult;
            Assert.That(result.StatusCode, Is.EqualTo(200));
        }
        [Test]
        public async Task DeleteContactByIdTest()
        {
            var result = await _controller.DeleteContactById(1) as OkObjectResult;
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
