using FluentResults.Extensions.AspNetCore;
using OrderApp.Main.Api.Application;
using OrderApp.Main.Api.Infrastructure;
using OrderApp.Main.Api.WebApi.ResultProfiles;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

AspNetCoreResult.Setup(config => config.DefaultProfile = new GlobalResultProfile());

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
