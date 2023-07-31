using Domain.Entities;
using Models.In;
using Models.Out;
using ShopifySharp;

namespace Application.Interfaces
{
    public interface ICustomerLocalService
    {
        public Task<CustomerEntity> Get(long? customerId);

        public Task Create(Customer customerId);

        public Task Update(CustomerEntity customer);

        public Task<AvailablePointsModel> GetPoints(long customerId);

        public Task<GiftCard> Redeem(CustomerRedeemModel redeemRequest);
    }
}
