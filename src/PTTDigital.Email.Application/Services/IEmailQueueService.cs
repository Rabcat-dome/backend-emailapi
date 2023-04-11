using PTTDigital.Email.Application.ViewModels.Requests;
using PTTDigital.Email.Application.ViewModels.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTTDigital.Email.Application.Services
{
    public interface IEmailQueueService
    {
        List<EmailQueueResponse> InsertQueue(List<EmailQueueRequest> queues);
        Task CancelQueue(List<CancelEmailQueueRequest> queueIds);
    }
}
