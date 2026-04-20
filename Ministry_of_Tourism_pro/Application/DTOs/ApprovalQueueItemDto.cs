namespace Ministry_of_Tourism_pro.Application.DTOs
{
    public class ApprovalQueueItemDto
    {
        public int Id { get; set; }
        public string PropertyName { get; set; } = string.Empty;
        public string Tin { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Subcity { get; set; } = string.Empty;
        public string SpecificAddress { get; set; } = string.Empty;
        public string StarRating { get; set; } = string.Empty;
        public string AddressLine1 { get; set; } = string.Empty;
        public bool? IsActive { get; set; }
        public string PreferenceDescription { get; set; } = string.Empty;
    }
}
