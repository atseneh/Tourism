using System.Collections.Generic;

namespace Ministry_of_Tourism_pro.Models
{
    public class MessageResponse
    {
        public bool? isSent { get; set; }
        public string? messageId { get; set; }  
        public string? to { get; set; }
        public string? code { get; set; }
        public string? verificationId { get; set; }
        public List<string?>? errors { get; set; }
    }

    public class OtpVerificationRequest
    {
        public string PhoneNumber { get; set; }
        public string Vc { get; set; }
        public string Code { get; set; }
        public string MessageId { get; set; }
    }
}
