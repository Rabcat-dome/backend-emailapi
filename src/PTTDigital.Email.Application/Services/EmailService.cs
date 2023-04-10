using PTTDigital.Email.Application.Repositories;
using PTTDigital.Email.Application.ViewModels.Requests;
using PTTDigital.Email.Application.ViewModels.Responses;
using PTTDigital.Email.Data.Context;
using PTTDigital.Email.Data.SqlServer.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTTDigital.Email.Application.Services
{
    public class EmailService
    {
        private readonly IGenerator _generator;


        public EmailService(IGenerator generator)
        {
            this._generator = generator;

        }

        public async Task<List<EmailQueueResponse>> InsertQueue(List<EmailQueueRequest> queues)
        {
            var queueId = _generator.GenerateUlid();
            var response = new List<EmailQueueResponse>();


            return new List<EmailQueueResponse>();
        }

        public async Task RemoveQueue(List<string> QueueIds)
        { }
    }
}
