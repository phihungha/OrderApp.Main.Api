using FluentResults;

namespace OrderApp.Main.Api.Application.Errors
{
    public class BusinessError(string message) : Error(message) { }
}
