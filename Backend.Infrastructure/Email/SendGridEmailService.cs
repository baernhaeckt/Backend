﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Threading.Tasks;
using Backend.Infrastructure.Abstraction.Email;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Backend.Infrastructure.Email
{
    [ExcludeFromCodeCoverage]
    public class SendGridEmailService : IEmailService
    {
        private readonly SendGridOptions _config;

        private readonly ILogger<SendGridEmailService> _logger;

        public SendGridEmailService(IOptions<SendGridOptions> configuration, ILogger<SendGridEmailService> logger)
        {
            _logger = logger;
            _config = configuration.Value;
        }

        public async Task Send(string subject, string text, string receiver)
        {
            var client = new SendGridClient(_config.ApiKey);
            var from = new EmailAddress(_config.SenderEmail, _config.SenderDisplayName);
            var receivers = new List<EmailAddress> { new EmailAddress(receiver) };
            SendGridMessage mail = MailHelper.CreateSingleEmailToMultipleRecipients(from, receivers, subject, text, text, false);

            _logger.SendEmail(subject, text, receiver);

            Response response = await client.SendEmailAsync(mail);
            if (response.StatusCode != HttpStatusCode.Accepted)
            {
                string result = await response.Body.ReadAsStringAsync();
                throw new InvalidOperationException("Email sending was not successful. Body: " + result);
            }
        }
    }
}