using Microsoft.EntityFrameworkCore;
using PaymentApi.Data;
using PaymentApi.DTOs;
using PaymentApi.Entities;
using PaymentApi.Repositories;
using PaymentApi.Services.Interfaces;

namespace PaymentApi.Services.Implementations;

public class PaymentService : IPaymentService
{
    private readonly AppDbContext _db;
    private readonly IUserRepository _users;
    private readonly IPaymentRepository _payments;

    private const decimal ChargeAmount = 1.10m;

    public PaymentService(AppDbContext db, IUserRepository users, IPaymentRepository payments)
    {
        _db = db;
        _users = users;
        _payments = payments;
    }

    public async Task<PaymentResult> PayAsync(Guid userId)
    {
        await using var tx = await _db.Database.BeginTransactionAsync();

        try
        {
            var user = await _users.GetByIdAsync(userId);
            if (user is null)
                return new PaymentResult(false, "User not found", 0);

            if (user.Balance < ChargeAmount)
                return new PaymentResult(false, "Insufficient funds", user.Balance);

            user.Balance -= ChargeAmount;
            await _users.SaveAsync(user);

            var payment = new PaymentRecord
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Amount = ChargeAmount,
                CreatedAt = DateTime.UtcNow
            };

            await _payments.AddAsync(payment);

            await tx.CommitAsync();

            return new PaymentResult(true, "Payment successful", user.Balance);
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync();
            return new PaymentResult(false, $"Error: {ex.Message}", 0);
        }
    }
}

