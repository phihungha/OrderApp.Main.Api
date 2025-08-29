using Microsoft.AspNetCore.Identity;

namespace OrderApp.Main.Api.Domain.Entities.UserEntities
{
    public class User : IdentityUser
    {
        public string? FullName { get; set; }
        public PaymentMethod? PaymentMethod { get; set; }
    }
}
