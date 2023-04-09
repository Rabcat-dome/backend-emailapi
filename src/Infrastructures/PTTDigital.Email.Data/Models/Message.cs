using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTTDigital.Email.Data.Models
{
    public class Message
    {

        [Key]
        [Required]
        [MaxLength(36)]
        public string MessageId { get; set; }
        [Column(TypeName = "text")]
        public string EmailSubject { get; set; }
        [Column(TypeName = "text")]
        public string EmailBody { get; set; }
    }
}
