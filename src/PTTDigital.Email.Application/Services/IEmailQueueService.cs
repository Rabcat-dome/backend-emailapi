using PTTDigital.Email.Application.ViewModels.Requests;
using PTTDigital.Email.Application.ViewModels.Responses;

namespace PTTDigital.Email.Application.Services
{
    public interface IEmailQueueService
    {
        List<EmailQueueResponse> InsertQueue(List<EmailQueueRequest> queues);
        Task CancelQueue(List<CancelEmailQueueRequest> queueIds);
        Task TriggerMail();
        Task ArchiveMail();
    }
}
