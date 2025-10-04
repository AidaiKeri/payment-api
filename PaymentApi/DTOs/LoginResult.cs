namespace PaymentApi.DTOs
{
    public record LoginResult(string Token, DateTime ExpiresAt);
}
