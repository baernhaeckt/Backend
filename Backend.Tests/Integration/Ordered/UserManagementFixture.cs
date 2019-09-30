﻿using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Backend.Core.Features.UserManagement.Models;
using Backend.Tests.Integration.Utilities;
using Backend.Tests.Integration.Utilities.Extensions;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.Ordering;

namespace Backend.Tests.Integration
{
    [Order(1)]
    [Collection("IntegrationTests")]
    [Trait("Category", "Integration")]
    public class UserManagementFixture
    {
        private readonly OrderedTestContext _context;

        private readonly ITestOutputHelper _output;

        public UserManagementFixture(OrderedTestContext context, ITestOutputHelper output)
        {
            _context = context;
            _output = output;
        }

        [Fact]
        [Order(0)]
        public async Task UsersRegister_SuccessfulAndSendsEmail()
        {
            string newUserEmail = DataGenerator.RandomEmail();
            _output.WriteLine("Register new user: " + newUserEmail);

            _context.NewTestUser = newUserEmail;

            var url = new Uri("api/users/Register?email=" + newUserEmail, UriKind.Relative);
            HttpResponseMessage response = await _context.AnonymousHttpClient.PostAsync(url, null);
            response.EnsureSuccessStatusCode();

            Assert.Single(_context.EmailService.Messages);
            Assert.Equal(newUserEmail, _context.EmailService.Messages.Single().Receiver);
        }

        [Fact]
        [Order(1)]
        public async Task UsersLogin_Successful()
        {
            _output.WriteLine("Sign in with the user: " + _context.NewTestUser);

            await _context.NewTestUserHttpClient.SignIn(_context.NewTestUser, "1234");
        }

        [Fact]
        [Order(2)]
        public async Task Profile_RetrieveSuccessful()
        {
            _output.WriteLine("Get profile of user: " + _context.NewTestUser);
            var url = new Uri("api/profile", UriKind.Relative);
            HttpResponseMessage response = await _context.NewTestUserHttpClient.GetAsync(url);
            var userResponse = await response.OnSuccessDeserialize<PrivateUserResponse>();
            Assert.Equal(_context.NewTestUser, userResponse.Email);
        }

        [Fact]
        [Order(3)]
        public async Task Profile_UpdateSuccessful()
        {
            _output.WriteLine("Update profile of user: " + _context.NewTestUser);
            var content = new StringContent(JsonConvert.SerializeObject(new UserUpdateRequest { DisplayName = "abc" }), Encoding.UTF8, "application/json");
            var url = new Uri("api/profile", UriKind.Relative);
            HttpResponseMessage response = await _context.NewTestUserHttpClient.PatchAsync(url, content);
            response.EnsureSuccessStatusCode();
        }
    }
}