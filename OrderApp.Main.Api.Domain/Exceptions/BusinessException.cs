namespace OrderApp.Main.Api.Domain.Exceptions
{
    public class BusinessException(string? message = null) : ApplicationException(message) { }
}
