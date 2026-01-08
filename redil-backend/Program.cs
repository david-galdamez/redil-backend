using Microsoft.EntityFrameworkCore;
using redil_backend.Dtos.Auth;
using redil_backend.Models;
using redil_backend.Repository.Auth;
using redil_backend.Services.Auth;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IAuthService<UserDto, AuthRegisterDto, AuthLoginDto>, AuthService>();
builder.Services.AddScoped<IAuthRepository<users>, AuthRepository > ();

// Entity Framework
builder.Services.AddDbContext<RedilDBContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default"));
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
