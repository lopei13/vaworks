using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VaWorks.Web.Data.Entities
{
    public enum InvitationType
    {
        Customer,
        SalesRepresentive
    }

    [Table("Invitations")]
    public class Invitation
    {
        [Key]
        public int InvitationId { get; set; }

        [Required]
        public int OrganizationId { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        [StringLength(50)]
        public string Company { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string SalesPersonEmail { get; set; }

        [Required]
        public InvitationType Type { get; set; }

        [Required]
        public DateTimeOffset CreatedDate { get; set; }

        [Required]
        public DateTimeOffset SentDate { get; set; }

        public bool IsClaimed { get; set; }

        public DateTimeOffset ClaimedDate { get; set; }

        public string ClaimedEmail { get; set; }
    }

    public enum RequestStatus
    {
        New,
        Granted,
        Rejected
    }

    [Table("InvitationRequests")]
    public class InvitationRequest
    {
        [Key]
        public int InvitationRequestId { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Company { get; set; }

        [Required]
        public DateTimeOffset RequestDate { get; set; }

        [Required]
        public RequestStatus Status { get; set; }
    }
}