using ShopifySharp;
namespace Application.Interfaces
{
    public interface IShopifyRepository
    {
        public Task<GiftCard> CreateGiftCard(GiftCard card, long customerId);
    }
}
