using ShopifySharp;

namespace Application.Interfaces
{
    public interface IOrderLocalService
    {
        public Task Create(Order order);
    }
}
