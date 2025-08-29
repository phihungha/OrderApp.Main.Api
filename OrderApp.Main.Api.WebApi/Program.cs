using System.Configuration;
using System.Text;
using System.Text.Json.Serialization;
using FluentResults.Extensions.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using OrderApp.Main.Api.Application;
using OrderApp.Main.Api.Domain.Entities.UserEntities;
using OrderApp.Main.Api.Infrastructure;
using OrderApp.Main.Api.Infrastructure.Persistence;
using OrderApp.Main.Api.Jobs;
using OrderApp.Main.Api.WebApi.ResultEndpointProfiles;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("email-settings.json");
builder.Services.Configure<AppConfig>(builder.Configuration);
var appConfig =
    builder.Configuration.Get<AppConfig>()
    ?? throw new ConfigurationErrorsException("App config is null.");

builder
    .Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

AspNetCoreResult.Setup(config => config.DefaultProfile = new GlobalResultEndpointProfile());

builder
    .Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder
    .Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = appConfig.Auth.Jwt.Issuer,
            ValidAudience = appConfig.Auth.Jwt.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(appConfig.Auth.Jwt.Key)
            ),
        };
    });

builder.AddInfrastructureServices();
builder.AddApplicationServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
