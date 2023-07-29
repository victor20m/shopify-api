
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using ShopifySharp;

namespace Domain
{
    public class CustomerEntity : Customer
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public new string? Id { get; set; }

        public List<Order> Orders { get; set; }

        public List<GiftCard> GiftCards { get; set; }

        public int RewardPoints { get; set; }

        public CustomerEntity() { 
            Orders = new List<Order>();
            GiftCards = new List<GiftCard>();
            RewardPoints = 0;
        }
    }
}