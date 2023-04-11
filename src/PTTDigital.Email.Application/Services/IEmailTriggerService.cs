using System.Net.Mail;

namespace PTTDigital.Email.Application.Services
{
    public interface IEmailTriggerService
    {
        List<MailAddress> ConvertToMailAddresses(string data);
        bool Validate(IList<MailAddress> toAddresses, IList<MailAddress> cCAddresses, string @from, string subject, string body);
        bool IsValidEmailAddressFormat(string emailAddress);
        Task SendMail(string from, string display, string subject, string body, bool isHtml, IList<Attachment>? attachments, IList<MailAddress> toAddresses, IList<MailAddress> ccAddresses);
    }
}
