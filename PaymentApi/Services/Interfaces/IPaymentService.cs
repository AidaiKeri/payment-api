using PaymentApi.DTOs;

namespace PaymentApi.Services.Interfaces;

public interface IPaymentService
{
    Task<PaymentResult> PayAsync(Guid userId);
}

