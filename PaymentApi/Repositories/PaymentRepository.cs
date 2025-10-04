using Microsoft.EntityFrameworkCore;
using PaymentApi.Data;
using PaymentApi.Entities;

namespace PaymentApi.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private readonly AppDbContext _db;
    public PaymentRepository(AppDbContext db) => _db = db;

    public async Task AddAsync(PaymentRecord payment)
    {
        _db.PaymentRecords.Add(payment);
        await _db.SaveChangesAsync();
    }
}

