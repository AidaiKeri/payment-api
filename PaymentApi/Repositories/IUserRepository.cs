using PaymentApi.Entities;

namespace PaymentApi.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task SaveAsync(User user);
}


