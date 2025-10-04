using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PaymentApi.Data;
using PaymentApi.DTOs;
using PaymentApi.Entities;
using PaymentApi.Services.Interfaces;

namespace PaymentApi.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        private static readonly Dictionary<string, int> FailedAttempts = new();
        private static readonly Dictionary<string, DateTime> LockedUntil = new();
        private const int MaxAttempts = 5;
        private static readonly TimeSpan LockoutTime = TimeSpan.FromMinutes(1);

        public AuthService(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<LoginResult> LoginAsync(LoginRequest request)
        {
            if (LockedUntil.TryGetValue(request.Username, out var until) && until > DateTime.UtcNow)
                throw new UnauthorizedAccessException("Account temporarily locked due to failed login attempts");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
            if (user == null || !VerifyPassword(request.Password, user.PasswordHash))
            {
                if (!FailedAttempts.ContainsKey(request.Username))
                    FailedAttempts[request.Username] = 0;

                FailedAttempts[request.Username]++;

                if (FailedAttempts[request.Username] >= MaxAttempts)
                {
                    LockedUntil[request.Username] = DateTime.UtcNow.Add(LockoutTime);
                    FailedAttempts[request.Username] = 0;
                }

                throw new UnauthorizedAccessException("Invalid username or password");
            }

            FailedAttempts.Remove(request.Username);
            LockedUntil.Remove(request.Username);

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]!);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            var session = new Session
            {
                UserId = user.Id,
                Token = tokenString,
                ExpiresAt = tokenDescriptor.Expires!.Value
            };

            _context.Sessions.Add(session);
            await _context.SaveChangesAsync();

            return new LoginResult(tokenString, session.ExpiresAt);
        }

        public async Task<bool> LogoutAsync(string token)
        {
            var session = await _context.Sessions.FirstOrDefaultAsync(s => s.Token == token);
            if (session == null)
                return false;

            _context.Sessions.Remove(session);
            await _context.SaveChangesAsync();
            return true;
        }

        private static bool VerifyPassword(string password, string storedHash)
        {
            var hash = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(password)));
            return hash == storedHash;
        }
    }
}

