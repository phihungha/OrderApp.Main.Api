using System.Text.Json.Serialization;
using FluentResults.Extensions.AspNetCore;
using OrderApp.Main.Api.Application;
using OrderApp.Main.Api.Infrastructure;
using OrderApp.Main.Api.WebApi.ResultEndpointProfiles;
using OrderApp.Main.Api.WebApi.SqsHandlers;

var builder = WebApplication.CreateBuilder(args);

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

builder.AddInfrastructureServices();
builder.AddApplicationServices();
builder.AddSqsHandlers();

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
