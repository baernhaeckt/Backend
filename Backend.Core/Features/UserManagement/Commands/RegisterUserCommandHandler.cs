﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Backend.Core.Entities;
using Backend.Core.Events;
using Backend.Core.Features.UserManagement.Security;
using Backend.Core.Features.UserManagement.Security.Abstraction;
using Backend.Infrastructure.Persistence.Abstraction;
using Silverback.Messaging.Publishing;
using Silverback.Messaging.Subscribers;

namespace Backend.Core.Features.UserManagement.Commands
{
    internal class RegisterUserCommandHandler : ISubscriber
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly IPasswordGenerator _passwordGenerator;

        private readonly IPasswordStorage _passwordStorage;

        private readonly IEventPublisher _eventPublisher;

        public RegisterUserCommandHandler(IUnitOfWork unitOfWork, IPasswordGenerator passwordGenerator, IPasswordStorage passwordStorage, IEventPublisher eventPublisher)
        {
            _unitOfWork = unitOfWork;
            _passwordGenerator = passwordGenerator;
            _passwordStorage = passwordStorage;
            _eventPublisher = eventPublisher;
        }

        public async Task ExecuteAsync(RegisterUserCommand command)
        {
            string newPassword = _passwordGenerator.Generate();
            var newUser = new User
            {
                Email = command.Email, // TODO: Validate E-Mail Address
                Password = _passwordStorage.Create(newPassword),
                DisplayName = "Newby",
                Roles = new List<string> { Roles.User },
                Location = new Location
                {
                    City = "Bern",
                    Zip = "3011",
                    Latitude = 46.944699,
                    Longitude = 7.443788
                }
            };
            newUser = await _unitOfWork.InsertAsync(newUser);

            await _eventPublisher.PublishAsync(new UserRegisteredEvent(newUser, newPassword));
        }
    }
}