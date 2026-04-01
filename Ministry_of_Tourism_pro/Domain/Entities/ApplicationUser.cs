namespace Ministry_of_Tourism_pro.Domain.Entities
{
    public class ApplicationUser
    {
        public string? Id { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public string? TIN { get; set; } 
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
