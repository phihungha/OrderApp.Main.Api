using FluentResults;

namespace OrderApp.Main.Api.Domain.Errors
{
    public class BusinessError(string message) : Error(message) { }
}
