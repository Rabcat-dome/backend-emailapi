using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTTDigital.Email.Data.Models
{
    [Index(nameof(Status))]
    public class EmailQueue
    {
        [Key]
        [Required]
        [MaxLength(36)]
        public string QueueId { get; set; }
        [MaxLength(4000)]
        public string EmailFrom { get; set; }
        [MaxLength(4000)]
        public string EmailTo { get; set; }
        [MaxLength(4000)]
        public string EmailCc { get; set; }

        public DateTime Initiated { get; set; } = DateTime.Now;
        public DateTime? Sent { get; set; }
        public bool IsHtmlFormat { get; set; } = true;
        public int RetryCount { get; set; } = 0;  //ใช้ในเงื่อนไขกวาดส่งกรณีที่มี Failed แต่ไม่เกี่ยวกับเงื่อนไข Transaction เคส 15 วันแจ้งเตือนเพราะเป็นเคสยิง Schedule อีกแบบในการ create Job
        public QueueStatus Status { get; set; } = QueueStatus.New;

        public int? RefAccPolicyId { get; set; } //เผื่อเคสที่เป็น bypass จะ allowNull
        public bool IsTest { get; set; } = false;

        [ForeignKey(nameof(Message))]
        [MaxLength(36)]
        public string MessageId { get; set; }
        public virtual Message? Message { get; set; }
    }

    public enum QueueStatus
    {
        New = 0,
        Queueing = 1,
        Sending = 2,
        Completed = 3,
        Canceled = 4,
        Failed = 5
    }
}
