using Microsoft.Extensions.Logging;
using PTTDigital.Email.Data.Service;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using PTTDigital.Email.Common.Configuration.AppSetting;

namespace PTTDigital.Email.Application.Services;

public class EmailTriggerService : IEmailTriggerService
{
    private readonly ILogger<EmailTriggerService> _logger;
    private readonly IEmailDataService _emailDataService;
    private readonly IAppSetting _appSetting;

    public EmailTriggerService(ILogger<EmailTriggerService> logger, IEmailDataService emailDataService, IAppSetting appSetting)
    {
        _logger = logger;
        _emailDataService = emailDataService;
        _appSetting = appSetting;
    }

    public List<MailAddress> ConvertToMailAddresses(string data)
    {
        var result = new List<MailAddress>();

        if (string.IsNullOrEmpty(data)) return result;

        var emails = data.Split('|');
        foreach (var email in emails)
        {
            if (!string.IsNullOrEmpty(email))
                result.Add(new MailAddress(email));
        }

        return result;
    }

    //เพื่อยืนยันว่ามี Attribute เพียงพอต่อการส่งงาน
    public bool Validate(IList<MailAddress> toAddresses, IList<MailAddress> cCAddresses, string from, string subject, string body)
    {
        if (string.IsNullOrWhiteSpace(subject))
            return false;
        if (string.IsNullOrWhiteSpace(body))
            return false;
        if (string.IsNullOrWhiteSpace(from))
            return false;

        return toAddresses?.Count > 0 | cCAddresses?.Count > 0;
    }

    public bool IsValidEmailAddressFormat(string emailAddress)
    {
        //ไม่แน่ใจว่าต้องถอด const ออกมาข้างนอกไหมเพราะมันไม่มีทางเปลี่ยนแน่ ๆ ในอนาคต
        const string matchEmailPattern = @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
                                         + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
				                                    [0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
                                         + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
				                                    [0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
                                         + @"([a-zA-Z0-9]+[\w-]+\.)+[a-zA-Z]{1}[a-zA-Z0-9-]{1,23})$";
        return !string.IsNullOrWhiteSpace(emailAddress) && Regex.IsMatch(emailAddress, matchEmailPattern);
    }

    public void SendMail(string from, string display, string subject, string body, bool isHtml, IList<Attachment>? attachments, IList<MailAddress> toAddresses, IList<MailAddress> ccAddresses)
    {
        ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

        using var smtpClient = new SmtpClient(_appSetting.SMTPServer);
        using var mailMessage = new MailMessage();

        foreach (var item in toAddresses)
        {
            mailMessage.To.Add(item);
        }
        foreach (var item in ccAddresses)
        {
            mailMessage.CC.Add(item);
        }

        mailMessage.From = new MailAddress(from, display);
        mailMessage.Subject = subject;
        mailMessage.IsBodyHtml = isHtml;
        mailMessage.Body = body.Replace("\\n", "\n").Replace("\\t", "\t");

        if (attachments != null)
            foreach (var attachment in attachments)
            {
                mailMessage.Attachments.Add(attachment);
            }

        smtpClient.Send(mailMessage);
    }
}