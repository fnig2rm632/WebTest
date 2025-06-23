using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebSockets;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using WebTest.Data;
using WebTest.Interfaces;
using WebTest.Interfaces.Manager;
using WebTest.Interfaces.Repository;
using WebTest.Interfaces.Service;
using WebTest.Models;
using WebTest.Repository;
using WebTest.Services;
using WebTest.Servises;
using WebSocketManager = WebTest.Services.WebSocketManager;
using WebSocketMiddleware = WebTest.Middleware.WebSocketMiddleware;

var builder = WebApplication.CreateBuilder(args);

// Добавление Сервисов

builder.Services.AddControllers();

/*builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Listen(System.Net.IPAddress.Parse("192.168.1.106"), 5160); // Локальный IP
    serverOptions.Listen(System.Net.IPAddress.Any, 5160); // Все доступные IP
});*/

// OpenAPI 
builder.Services.AddOpenApi();

// Подключение к базе
builder.Services.AddDbContext<ApplicationDbContext>(options => 
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Регистрация Identity
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
}).AddEntityFrameworkStores<ApplicationDbContext>();

// Jwt Токен
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme =
    options.DefaultChallengeScheme =
    options.DefaultForbidScheme =
    options.DefaultScheme =
    options.DefaultSignInScheme =
    options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:Audience"],
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JWT:SigningKey"])
        )
    };
});


// Регистрация Сервисов
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IFriendService, FriendService>();
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IMatchmakingService, MatchmakingService>();

builder.Services.AddSingleton<IWedSocketManager, WebSocketManager>();
builder.Services.AddSingleton<IGameLogicService, GameLogicService>();

// Регистрация Репозиториев
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IGameRepository, GameRepository>();
builder.Services.AddScoped<IFriendRepository, FriendRepository>();

/*builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(2);
    options.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(1);
});*/

// Настройка URL-адресов (до builder.Build())
// builder.WebHost.UseUrls("http://0.0.0.0:5160"); 

var app = builder.Build();

app.UseWebSockets();
app.UseMiddleware<WebSocketMiddleware>();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}


app.UseAuthorization();

app.MapControllers();

await app.RunAsync();

