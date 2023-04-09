using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTTDigital.Email.Application.ViewModels.Requests
{
    public class EmailQueueRequest
    {
        public string EmailFrom { get; set; } //"email1@ptt.com;email2@ptt.com;"
        public string EmailTo { get; set; }
        public string? EmailCc { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool IsHtmlFormat { get; set; } = true;
    }
}
