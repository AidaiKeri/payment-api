using Microsoft.EntityFrameworkCore;
using PaymentApi.Data;
using PaymentApi.Entities;

namespace PaymentApi.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;
    public UserRepository(AppDbContext db) => _db = db;

    public Task<User?> GetByIdAsync(Guid id) =>
        _db.Users.SingleOrDefaultAsync(u => u.Id == id);

    public async Task SaveAsync(User user)
    {
        _db.Users.Update(user);
        await _db.SaveChangesAsync();
    }
}
