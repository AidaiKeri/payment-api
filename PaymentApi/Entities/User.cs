using System.ComponentModel.DataAnnotations;

namespace PaymentApi.Entities
{
    public class User
    {
        public Guid Id { get; set; }

        [Required, MaxLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public decimal Balance { get; set; } = 8.00m;
    }
}
