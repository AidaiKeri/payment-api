using PaymentApi.DTOs;
using PaymentApi.Entities;

namespace PaymentApi.Services.Interfaces;

public interface IAuthService
{
    Task<LoginResult> LoginAsync(LoginRequest request);
    Task LogoutAsync(string token);
}

