﻿using System.Threading.Tasks;
using Backend.Core.Entities;
using Backend.Core.Framework.Cqrs;
using Backend.Infrastructure.Abstraction.Persistence;
using Microsoft.Extensions.Logging;

namespace Backend.Core.Features.UserManagement.Queries
{
    internal class EmailRegisteredQueryHandler : QueryHandler<EmailRegisteredQueryResult, EmailRegisteredQuery>
    {
        public EmailRegisteredQueryHandler(IReader reader, ILogger<EmailRegisteredQueryHandler> logger)
            : base(reader, logger)
        {
        }

        public override async Task<EmailRegisteredQueryResult> ExecuteAsync(EmailRegisteredQuery query)
        {
            Logger.RetrieveEmailAlreadyRegistered(query.Email);

            long count = await Reader.CountAsync<User>(u => u.Email == query.Email.ToLowerInvariant());
            bool isRegistered = count > 0;
            var result = new EmailRegisteredQueryResult(isRegistered);

            Logger.RetrieveEmailAlreadyRegisteredSuccessful(query.Email, result.IsRegistered);

            return result;
        }
    }
}