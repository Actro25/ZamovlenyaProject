using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Додаємо політику CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyOrigin()    // Дозволяє будь-яке походження (Origin)
                   .AllowAnyMethod()    // Дозволяє будь-які методи (GET, POST, PUT, DELETE тощо)
                   .AllowAnyHeader();   // Дозволяє будь-які заголовки
        });
});

// Налаштування JWT
var key = "Key1111111111111111111111111111111111"; // Ключ для підпису токена
var issuer = "MyLocalGenerate"; // Видавець
var audience = "MyGroupUser";   // Аудиторія

// Налаштування Minimal API
var app = builder.Build();


app.UseCors("AllowAllOrigins");

// Ендпойнт для логіну та генерації JWT токена
app.MapPost("/login", (UserLogin userLogin) =>
{
    // Проста перевірка логіна (у реальному додатку логіка буде складнішою)
    if (userLogin.Username == "testuser" && userLogin.Password == "testpassword")
    {
        var token = GenerateJwtToken(userLogin.Username);
        return Results.Ok(new { token });
    }

    return Results.Unauthorized();
});

// Метод генерації JWT токена
string GenerateJwtToken(string username)
{
    var tokenHandler = new JwtSecurityTokenHandler();
    var keyBytes = Encoding.UTF8.GetBytes(key);

    var tokenDescriptor = new SecurityTokenDescriptor
    {
        Subject = new ClaimsIdentity(new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        }),
        Expires = DateTime.UtcNow.AddMinutes(30), // Термін дії токена
        Issuer = issuer,
        Audience = audience,
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature)
    };

    var token = tokenHandler.CreateToken(tokenDescriptor);
    return tokenHandler.WriteToken(token);
}

app.Run();

// Модель для логіну
public record UserLogin(string Username, string Password);
