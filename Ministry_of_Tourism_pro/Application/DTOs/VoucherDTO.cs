using System;

namespace CNET_V7_Domain.Domain.TransactionSchema
{
    public class VoucherDTO
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public int Type { get; set; }

        public int Definition { get; set; }

        public int? OriginConsigneeUnit { get; set; }

        public int? DestinationConsigneeUnit { get; set; }

        public int? Period { get; set; }

        public int? Shift { get; set; }

        public int? Consignee1 { get; set; }

        public int? Consignee2 { get; set; }

        public int? Consignee3 { get; set; }

        public int? Consignee4 { get; set; }

        public int? Consignee5 { get; set; }

        public int? Consignee6 { get; set; }

        public int? ConsigneeUnit1 { get; set; }

        public int? ConsigneeUnit2 { get; set; }

        public int? ConsigneeUnit3 { get; set; }

        public int? ConsigneeUnit4 { get; set; }

        public int? ConsigneeUnit5 { get; set; }

        public int? ConsigneeUnit6 { get; set; }

        public int? Article { get; set; }

        public DateTime IssuedDate { get; set; }

        public bool IsIssued { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime LastModified { get; set; }

        public bool IsVoid { get; set; }

        public int? Day { get; set; }

        public int? Month { get; set; }

        public int? Year { get; set; }

        public decimal SubTotal { get; set; }

        public decimal Discount { get; set; }

        public decimal AddCharge { get; set; }

        public decimal GrandTotal { get; set; }

        public int? PaymentMethod { get; set; }

        public int? PaymentProcessor { get; set; }

        public int? Payer { get; set; }

        public bool? IsIncoming { get; set; }

        public decimal? PaymentAmount { get; set; }

        public DateTime? PaymentIssueDate { get; set; }

        public DateTime? PaymentMaturityDate { get; set; }

        public string? PaymentRefNumber { get; set; }

        public int? PaymentStatus { get; set; }

        public int? Currency { get; set; }

        public decimal? ExchangeRate { get; set; }

        public decimal? Tender { get; set; }

        public string? Note { get; set; }

        public int? Purpose { get; set; }

        public string? FsNumber { get; set; }

        public string? Mrc { get; set; }

        public int? Cart { get; set; }

        public string? Extension1 { get; set; }

        public string? Extension2 { get; set; }

        public string? Extension3 { get; set; }

        public string? Extension4 { get; set; }

        public string? Extension5 { get; set; }

        public string? Extension6 { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int? SourceStore { get; set; }

        public int? DestinationStore { get; set; }

        public bool? HasEffect { get; set; }

        public int? SourceBankAccount { get; set; }

        public int? DestinationBankAccount { get; set; }

        public int? LastActivity { get; set; }

        public int? DeliveryMethod { get; set; }

        public int? Count { get; set; }

        public string? Space { get; set; }

        public int? ContactPerson { get; set; }

        public int LastUser { get; set; }

        public int? LastDevice { get; set; }

        public int LastState { get; set; }

        public decimal? Latitiude { get; set; }

        public decimal? Longitude { get; set; }

        public bool? Locked { get; set; }

        public string? DefaultImageUrl { get; set; }

        public string? Remark { get; set; }
    }
}
