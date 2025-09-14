namespace OrderApp.Main.Api.Domain.Entities.UserEntities
{
    public class User
    {
        public required string FullName { get; set; }

        public PaymentMethod? PaymentMethod { get; set; }
    }
}
