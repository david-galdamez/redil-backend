using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using redil_backend.Dtos.Auth;
using redil_backend.Dtos.Classes;
using redil_backend.Dtos.Redil;
using redil_backend.Dtos.Teacher;
using redil_backend.Models;
using redil_backend.Repository.Auth;
using redil_backend.Repository.Classes;
using redil_backend.Repository.Redil;
using redil_backend.Services;
using redil_backend.Services.Auth;
using redil_backend.Services.Classes;
using redil_backend.Services.Redil;
using redil_backend.Services.Teacher;
using redil_backend.Validators.Auth;
using redil_backend.Validators.Classes;
using redil_backend.Validators.Redil;
using redil_backend.Validators.Teacher;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IAuthService<ServiceResult<UserDto>,AuthRegisterDto, AuthLoginDto>, AuthService>();
builder.Services.AddScoped<IAuthRepository<users>, AuthRepository > ();

builder.Services.AddScoped<IRedilRepository<rediles>, RedilRepository>();
builder.Services.AddScoped<IRedilService<ServiceResult<RedilDto>, RegisterRedilDto>, RedilService>();

builder.Services.AddScoped<ITeacherService<ServiceResult<TeacherDto>, RegisterTeacherDto>, TeacherService>();

builder.Services.AddScoped<IClassRepository<classes>, ClassRepository>();
builder.Services.AddScoped<IClassService<ServiceResult<ClassDto>, RegisterClassDto>, ClassService>();

// Validators
builder.Services.AddScoped<IValidator<AuthRegisterDto>, AuthRegisterValidator>();
builder.Services.AddScoped<IValidator<AuthLoginDto>, AuthLoginValidator>();
builder.Services.AddScoped<IValidator<RegisterRedilDto>, RegisterRedilValidator>();
builder.Services.AddScoped<IValidator<RegisterTeacherDto>, RegisterTeacherValidator>();
builder.Services.AddScoped<IValidator<RegisterClassDto>, RegisterClassValidator>();

// Password Hasher
builder.Services.AddScoped<IPasswordHasher<users>, PasswordHasher<users>>();

// Jwt
builder.Services.AddSingleton<TokenProvider>();

// Entity Framework
builder.Services.AddDbContext<RedilDBContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default"));
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(o =>
        {
            o.RequireHttpsMetadata = false;

            o.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Cookies["access_token"];
                    if (!string.IsNullOrEmpty(accessToken))
                    {
                        context.Token = accessToken;
                    }
                    return Task.CompletedTask;
                }
            };

            o.TokenValidationParameters = new TokenValidationParameters
            {
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!)),
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                RoleClaimType = ClaimTypes.Role,
                NameClaimType = ClaimTypes.NameIdentifier,
                ClockSkew = TimeSpan.Zero
            };
        });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
