using System.Configuration;
using Hangfire;
using Hangfire.PostgreSql;
using OrderApp.Main.Api.Application;
using OrderApp.Main.Api.Infrastructure;
using OrderApp.Main.Api.Jobs;
using OrderApp.Main.Api.Jobs.SqsHandlers;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddJsonFile("email-settings.json");
builder.Services.Configure<AppConfig>(builder.Configuration);

var connectionString =
    builder.Configuration.GetConnectionString("Jobs")
    ?? throw new ConfigurationErrorsException("ConnectionStrings:Jobs is not set");

builder.Services.AddHangfire(provider =>
    provider.UsePostgreSqlStorage(c => c.UseNpgsqlConnection(connectionString))
);
builder.Services.AddHangfireServer();

builder.AddInfrastructureServices();
builder.AddApplicationServices();
builder.AddSqsHandlers();

var app = builder.Build();

app.Run();
