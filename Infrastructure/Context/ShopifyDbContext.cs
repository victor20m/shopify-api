namespace Infrastructure.Context
{
    public class ShopifyDbContext
    {
        public string ConnectionString { get; set; } = null!;

        public string DatabaseName { get; set; } = null!;

        public string CustomersCollection { get; set; } = null!;
    }
}
