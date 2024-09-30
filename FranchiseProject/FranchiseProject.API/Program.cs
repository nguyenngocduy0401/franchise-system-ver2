using FranchiseProject.API;
using FranchiseProject.API.Middlewares;
using FranchiseProject.Application.Commons;
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
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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
app.UseCors("CorsPolicy");
app.UseHttpsRedirection();
app.UseMiddleware<PerformanceMiddleware>();
/*app.UseMiddleware<RedisAuthenticationMiddleware>();*/

app.UseAuthorization();

app.MapControllers();

app.Run();
