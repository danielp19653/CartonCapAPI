using CartonCapAPI.Models.Enums;
using System;

namespace CartonCapAPI.Models
{
    public class ReferralModel
    {
        public int ReferralId { get; set; }
        public int OriginatingUserId { get; set; }
        public int RefereeUserId { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
        public string Code { get; set; }
        public DateTime? LastSent { get; set; }
        public bool IsSent { get; set; }
        public bool IsUserCreated { get; set; }
        public ReferralStatusesEnum Status { get; set; }
    }
}
