using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PTTDigital.Email.Data.Service;

namespace PTTDigital.Email.Application.Services
{
    public class EmailTriggerService : IEmailTriggerService
    {
        private readonly ILogger<EmailTriggerService> _logger;
        private readonly IEmailDataService _emailDataService;

        public EmailTriggerService(ILogger<EmailTriggerService> logger,IEmailDataService emailDataService)
        {
            _logger = logger;
            _emailDataService = emailDataService;
        }


    }
}
