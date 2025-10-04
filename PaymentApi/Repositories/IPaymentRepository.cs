using PaymentApi.Entities;

namespace PaymentApi.Repositories;

public interface IPaymentRepository
{
    Task AddAsync(PaymentRecord payment);
}

