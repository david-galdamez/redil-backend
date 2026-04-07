using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using redil_backend.Dtos.Auth;
using redil_backend.Dtos.Classes;
using redil_backend.Dtos.Groups;
using redil_backend.Dtos.Redil;
using redil_backend.Dtos.Student;
using redil_backend.Dtos.Teacher;
using redil_backend.Middlewares;
using redil_backend.Models;
using redil_backend.Repository.Auth;
using redil_backend.Repository.ClassDetails;
using redil_backend.Repository.Classes;
using redil_backend.Repository.Groups;
using redil_backend.Repository.Redil;
using redil_backend.Repository.StudentRediles;
using redil_backend.Repository.Students;
using redil_backend.Services;
using redil_backend.Services.Auth;
using redil_backend.Services.Classes;
using redil_backend.Services.Groups;
using redil_backend.Services.Redil;
using redil_backend.Services.Students;
using redil_backend.Services.Teacher;
using redil_backend.Validators.Auth;
using redil_backend.Validators.Classes;
using redil_backend.Validators.Group;
using redil_backend.Validators.Redil;
using redil_backend.Validators.Student;
using redil_backend.Validators.Teacher;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Auth
builder.Services.AddScoped<IAuthService<ServiceResult<UserDto>,AuthRegisterDto, AuthLoginDto>, AuthService>();
builder.Services.AddScoped<IAuthRepository<User>, AuthRepository > ();
builder.Services.AddScoped<IValidator<AuthRegisterDto>, AuthRegisterValidator>();
builder.Services.AddScoped<IValidator<AuthLoginDto>, AuthLoginValidator>();
builder.Services.AddScoped<IValidator<UserProfileUpdateDto>, AuthUpdateProfileValidator>();
builder.Services.AddScoped<IValidator<UserPasswordChangeDto>, AuthUpdatePasswordValidator>();

// Redil
builder.Services.AddScoped<IRedilRepository<Redile>, RedilRepository>();
builder.Services.AddScoped<IRedilService<ServiceResult<RedilDto>, RegisterRedilDto>, RedilService>();
builder.Services.AddScoped<IValidator<RegisterRedilDto>, RegisterRedilValidator>();

// Teacher
builder.Services.AddScoped<ITeacherService<ServiceResult<TeacherDto>, RegisterTeacherDto, UpdateTeacherDto>, TeacherService>();
builder.Services.AddScoped<IValidator<RegisterTeacherDto>, RegisterTeacherValidator>();
builder.Services.AddScoped<IValidator<UpdateTeacherDto>, UpdateTeacherValidator>();

// Classes
builder.Services.AddScoped<IClassRepository<Class>, ClassRepository>();
builder.Services.AddScoped<IClassService<ServiceResult<ClassDto>, RegisterClassDto>, ClassService>();
builder.Services.AddScoped<IValidator<RegisterClassDto>, RegisterClassValidator>();
builder.Services.AddScoped<IValidator<RegisterAttendanceDto>, RegisterAssistValidator>();
builder.Services.AddScoped<IValidator<ClassStatsRequestDto>, StatClassRequestValidator>();
builder.Services.AddScoped<IClassDetailsRepository<ClassDetail>, ClassDetailsRepository>();

// Students
builder.Services.AddScoped<IStudentService<ServiceResult<int>, RegisterStudentDto>, StudentService>();
builder.Services.AddScoped<IStudentRepository<Student>, StudentRepository>();
builder.Services.AddScoped<IStudentRedilRepository<StudentRedil>, StudentRedilRepository>();
builder.Services.AddScoped<IValidator<RegisterStudentDto>, RegisterStudentValidator>();

// Groups
builder.Services.AddScoped<IGroupService<ServiceResult<int>>, GroupService>();
builder.Services.AddScoped<IGroupRepository<Group>, GroupRepository>();
builder.Services.AddScoped<IValidator<RegisterGroupDto>, RegisterGroupValidator>();

// Password Hasher
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

// Jwt
builder.Services.AddSingleton<TokenProvider>();

// Entity Framework
builder.Services.AddDbContext<RedilDbContext>(options =>
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

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:4321", "https://redil-frontend.vercel.app")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

app.UseMiddleware<ErrorHandlerMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
