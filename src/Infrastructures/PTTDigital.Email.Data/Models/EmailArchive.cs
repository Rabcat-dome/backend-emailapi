using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTTDigital.Email.Data.Models
{
    public class EmailArchive
    {
        [Key]
        [Required]
        [MaxLength(36)]
        public string ArchiveId { get; set; }

        [MaxLength(36)]
        public string QueueId { get; set; }
        [MaxLength(4000)]
        public string EmailFrom { get; set; }
        [MaxLength(4000)]
        public string EmailTo { get; set; }
        [MaxLength(4000)]
        public string EmailCc { get; set; }

        public DateTime Initiated { get; set; }
        public DateTime? Sent { get; set; }
        public bool IsHtmlFormat { get; set; }
        public int RetryCount { get; set; }
        public QueueStatus Status { get; set; }

        public int? RefAccPolicyId { get; set; }
        public bool IsTest { get; set; }

        [ForeignKey(nameof(Message))]
        [MaxLength(36)]
        public string MessageId { get; set; }
        public virtual Message? Message { get; set; }
    }
}
