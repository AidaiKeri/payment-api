using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PaymentApi.Data;
using PaymentApi.Entities;
using PaymentApi.Repositories;
using PaymentApi.Services.Implementations;
using PaymentApi.Services.Interfaces;
using System.Text;
var builder = WebApplication.CreateBuilder(args);

// ����������, ������� �� ������ � Docker
bool isDocker = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";

// �������� ������ �����������
var connectionString = isDocker
    ? builder.Configuration.GetConnectionString("DefaultConnection")
      ?? "Host=db;Port=5432;Database=paymentapi;Username=postgres;Password=postgres"
    : builder.Configuration.GetConnectionString("DefaultConnection")
      ?? "Host=localhost;Port=5432;Database=paymentapi;Username=postgres;Password=postgres"; // <-- ����� ����� ������� ������ ��������� ��

// ��������� DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// ������������ �������
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ��������� JWT
var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// ----------------------------
// �������������� ���������� �������� � �������� ��������� ������������
// ----------------------------
try
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // ������� ��������� �������� ������ ���� ���� �����������
        if (db.Database.CanConnect())
        {
            db.Database.Migrate();

            if (!db.Users.Any())
            {
                var passwordHash = Convert.ToBase64String(
                    System.Security.Cryptography.SHA256.HashData(
                        System.Text.Encoding.UTF8.GetBytes("123456")
                    )
                );

                db.Users.Add(new User
                {
                    Id = Guid.NewGuid(),
                    Username = "test",
                    PasswordHash = passwordHash,
                    Balance = 8m
                });

                db.SaveChanges();
            }
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine("�� ������� ������������ � ����: " + ex.Message);
}

app.Run();

