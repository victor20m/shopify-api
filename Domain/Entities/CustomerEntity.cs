using ShopifySharp;

namespace Domain.Entities
{
    public class CustomerEntity : Customer
    {
        public List<Order> Orders { get; set; }

        public List<GiftCard> GiftCards { get; set; }

        public int RewardPoints { get; set; }

        public CustomerEntity()
        {
            Orders = new List<Order>();
            GiftCards = new List<GiftCard>();
            RewardPoints = 0;
        }
        public CustomerEntity(Customer customer)
        {
            foreach (var field in typeof(Customer).GetProperties())
            {
                field.SetValue(this, field.GetValue(customer));
            }
            Orders = new List<Order>();
            GiftCards = new List<GiftCard>();
            RewardPoints = 0;
        }
    }
}