namespace PaymentApi.DTOs
{
    public record PaymentResult(bool Success, string Message, decimal NewBalance);
}
