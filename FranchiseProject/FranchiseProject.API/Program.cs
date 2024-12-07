using FranchiseProject.API;
using FranchiseProject.API.Services;
using FranchiseProject.API.Middlewares;
using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Hubs; // Thêm dòng này để sử dụng NotificationHub
using FranchiseProject.Infrastructures;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using FranchiseProject.Domain.Entity;
using Microsoft.Extensions.Configuration;
using Quartzmin;
using FranchiseProject.Application.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var configuration = builder.Configuration.Get<AppConfiguration>();
builder.Services.AddInfrastructuresService(configuration.DatabaseConnection);
builder.Services.Configure<VnPayConfig>(options =>
{
    options.TmnCode = configuration.VnPay.TmnCode;
    options.HashSecret = configuration.VnPay.HashSecret;
    options.PaymentUrl = configuration.VnPay.PaymentUrl;
    options.ReturnUrl = configuration.VnPay.ReturnUrl;
});
// Thêm đoạn code này sau các service khác và trước builder.Build()
builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.AddConfiguration(builder.Configuration.GetSection("Logging"));
    loggingBuilder.AddConsole();
    loggingBuilder.AddDebug();
});
builder.Services.AddScoped<VnPayService>();
builder.Services.AddQuarztJobsService();
builder.Services.AddWebAPIService();
builder.Services.AddAuthenticationServices(configuration);
builder.Services.AddStackExchangeRedisCache(options => options.Configuration = configuration.RedisConfiguration);
builder.Services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();

// Đăng ký SignalR
builder.Services.AddSignalR();

// Cấu hình CORS cho phép kết nối SignalR từ frontend


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
app.UseCors("CorsPolicy"); // Sử dụng CORS policy

app.UseHttpsRedirection();
app.UseMiddleware<PerformanceMiddleware>();
/*app.UseMiddleware<RedisAuthenticationMiddleware>();*/

app.UseAuthorization();

// Định tuyến cho SignalR Hub
app.MapHub<NotificationHub>("/notificationHub").RequireCors("CorsPolicy"); // Định tuyến Hub

app.MapControllers();

app.Run();
