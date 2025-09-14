using FluentResults;

namespace OrderApp.Main.Api.Domain.Errors
{
    public class NotFoundError(string? message = "Entity not found.") : Error(message) { }
}
