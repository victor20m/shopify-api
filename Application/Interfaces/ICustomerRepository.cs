using Domain.Entities;

namespace Application.Interfaces
{
    public interface ICustomerRepository
    {
        public Task<CustomerEntity> Get(long? customerId);

        public Task Create(CustomerEntity customer);

        public Task Update(CustomerEntity customer);

    }
}
