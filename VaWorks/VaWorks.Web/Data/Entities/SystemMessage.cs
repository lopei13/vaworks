using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VaWorks.Web.Data.Entities
{
    [Table("SystemMessages")]
    public class SystemMessage
    {
        [Key]
        public int UserMessageId { get; set; }

        public string UserId { get; set; }

        public DateTimeOffset DateSent { get; set; }

        public bool IsRead { get; set; }

        public DateTimeOffset? DateRead { get; set; }

        [StringLength(100)]
        public string MessagePreview { get; set; }

        [DataType(DataType.MultilineText)]
        public string Message { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
    }
}