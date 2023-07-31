using Application.Interfaces;
using Domain.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ShopifySharp;

namespace Infrastructure.Repository
{
    public class ShopifyRepository : IShopifyRepository
    {
        private readonly GiftCardService _giftCardService;
        private readonly IConfiguration _config;
        private readonly ILogger<ShopifyRepository> _logger;
        public ShopifyRepository(IConfiguration config, ILogger<ShopifyRepository> logger)
        {
            _config = config;
            _logger = logger;
            _giftCardService = new GiftCardService(_config["shopify:store_url"], _config["shopify:api_secret"]);
            _giftCardService.SetExecutionPolicy(new LeakyBucketExecutionPolicy());
        }

        public async Task<GiftCard> CreateGiftCard(GiftCard giftCard, long customerId)
        {
            try
            {
                GiftCard createdCard = await _giftCardService.CreateAsync(giftCard);
                createdCard.CustomerId = customerId;
                return await _giftCardService.UpdateAsync((long)createdCard.Id, createdCard);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                throw new CustomHttpException("Error creating gift card", 500);
            }
        }
    }
}
