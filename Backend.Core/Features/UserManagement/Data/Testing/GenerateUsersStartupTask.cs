﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Backend.Core.Abstraction;
using Backend.Core.Features.UserManagement.Security;
using Backend.Core.Features.UserManagement.Security.Abstraction;
using Backend.Database.Abstraction;
using Backend.Database.Entities;
using Bogus;
using Bogus.Locations;

namespace Backend.Core.Features.UserManagement.Data.Testing
{
    public class GenerateUsersStartupTask : IStartupTask
    {
        private const int SeedCount = 20;

        private readonly IPasswordStorage _passwordStorage;

        private readonly IPasswordGenerator _paswordGenerator;

        private readonly IUnitOfWork _unitOfWork;

        public GenerateUsersStartupTask(IUnitOfWork unitOfWork, IPasswordStorage passwordStorage, IPasswordGenerator paswordGenerator)
        {
            _unitOfWork = unitOfWork;
            _passwordStorage = passwordStorage;
            _paswordGenerator = paswordGenerator;
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            if (await _unitOfWork.CountAsync<User>() >= SeedCount)
            {
                return;
            }

            IList<string> zips = new[] { "3001", "3006", "3010", "3013", "3018", "3027", "3004", "3007", "3011", "3014" };

            Faker<Location> locationFaker = new Faker<Location>()
                .RuleFor(u => u.Zip, f => f.PickRandom(zips))
                .RuleFor(u => u.City, "Bern")
                .RuleFor(u => u.Latitude, f => f.Location().AreaCircle(46.944699, 7.443788, 10000).Latitude)
                .RuleFor(u => u.Longitude, f => f.Location().AreaCircle(46.944699, 7.443788, 10000).Longitude);

            List<Location> locations = locationFaker.Generate(100).ToList();

            Faker<User> faker = new Faker<User>()
                .RuleFor(u => u.Id, f => Guid.NewGuid())
                .RuleFor(u => u.Email, f => f.Internet.Email())
                .RuleFor(u => u.Password, _passwordStorage.Create(_paswordGenerator.Generate()))
                .RuleFor(u => u.DisplayName, f => f.Name.FirstName())
                .RuleFor(u => u.Roles, new List<string> { Roles.User })
                .RuleFor(u => u.Location, f => f.PickRandom(locations));

            List<User> users = faker.Generate(SeedCount);

            // Random generate friendships
            var random = new Random();
            foreach (User user in users)
            {
                for (var i = 0; i < 12; i++)
                {
                    int index = random.Next(users.Count);
                    user.Friends.Add(users[index].Id);
                }
            }

            // This users are for the automated tests.
            users.Add(new User
            {
                Email = "user@leaf.ch",
                Roles = new List<string> { Roles.User },
                Password = _passwordStorage.Create("user")
            });
            users.Add(new User
            {
                Email = "partner@leaf.ch",
                Roles = new List<string> { Roles.Partner },
                Password = _passwordStorage.Create("partner")
            });

            await _unitOfWork.InsertManyAsync(users);
        }
    }
}