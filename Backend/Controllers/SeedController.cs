﻿using System.Collections.Generic;
using AspNetCore.MongoDB;
using Backend.Core.Security;
using Backend.Models.Database;
using Bogus;
using Bogus.Locations;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeedController : ControllerBase
    {
        private readonly IPasswordStorage _passwordStorage;
        private readonly IMongoOperation<User> _operation;

        public SeedController(IPasswordStorage passwordStorage, IMongoOperation<User> operation)
        {
            _passwordStorage = passwordStorage;
            _operation = operation;
        }

        [HttpPost("users")]
        public void SeedUsers(int count)
        {
            IList<string> zips = new[] { "3001", "3006", "3010", "3013", "3018", "3027", "3004", "3007", "3011", "3014" };

            Faker<User> faker = new Faker<User>()
                .RuleFor(u => u.Email, f => f.Internet.Email())
                .RuleFor(u => u.Password, _passwordStorage.Create("1234"))
                .RuleFor(u => u.Zip, f => f.PickRandom(zips))
                .RuleFor(u => u.City, "Bern")
                .RuleFor(u => u.Latitude, f => f.Location().AreaCircle(46.944699, 7.443788, 10).Latitude)
                .RuleFor(u => u.Longitude, f => f.Location().AreaCircle(46.944699, 7.443788, 10).Longitude);

            List<User> users = faker.Generate(count);
            _operation.InsertMany(users);
        }
    }
}