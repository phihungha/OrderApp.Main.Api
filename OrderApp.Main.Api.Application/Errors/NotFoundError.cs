using FluentResults;

namespace OrderApp.Main.Api.Application.Errors
{
    public class NotFoundError(string? message = "Entity not found.") : Error(message) { }
}
