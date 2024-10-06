using FranchiseProject.API;
using FranchiseProject.API.Middlewares;
using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Hubs; // Thêm dòng này để sử dụng NotificationHub
using FranchiseProject.Infrastructures;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var configuration = builder.Configuration.Get<AppConfiguration>();
builder.Services.AddInfrastructuresService(configuration.DatabaseConnection);
builder.Services.AddWebAPIService();
builder.Services.AddAuthenticationServices(configuration);
builder.Services.AddStackExchangeRedisCache(options => options.Configuration = configuration.RedisConfiguration);

// Đăng ký SignalR
builder.Services.AddSignalR();

// Cấu hình CORS cho phép kết nối SignalR từ frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", builder =>
    {
        builder.WithOrigins("http://localhost:5173") // Thay bằng URL của frontend
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials();
    });
});

builder.Services.AddSingleton(configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("AllowSpecificOrigin"); // Sử dụng CORS policy

app.UseHttpsRedirection();
app.UseMiddleware<PerformanceMiddleware>();
/*app.UseMiddleware<RedisAuthenticationMiddleware>();*/

app.UseAuthorization();

// Định tuyến cho SignalR Hub
app.MapHub<NotificationHub>("/notificationHub").RequireCors("AllowSpecificOrigin"); // Định tuyến Hub

app.MapControllers();

app.Run();
