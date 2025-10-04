using PaymentApi.DTOs;
using PaymentApi.Entities;

namespace PaymentApi.Services.Interfaces;

public interface IAuthService
{
    Task<LoginResult> LoginAsync(LoginRequest request);
    Task<bool> LogoutAsync(string token);
}

