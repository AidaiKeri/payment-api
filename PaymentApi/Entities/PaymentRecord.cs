using System.ComponentModel.DataAnnotations;

namespace PaymentApi.Entities
{
    public class PaymentRecord
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public User? User { get; set; }

        [Required]
        public decimal Amount { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
