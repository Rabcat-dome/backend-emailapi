using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTTDigital.Email.Application.ViewModels.Responses
{
    public class EmailQueueResponse
    {
        //ใช้เพื่อตอบ Response สำหรับรับขา CancelQueue
        public string? QueueId { get; set; }
        public string Subject { get; set; }
        public string EmailTo { get; set; }
    }
}
