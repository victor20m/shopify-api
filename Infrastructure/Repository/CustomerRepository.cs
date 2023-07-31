using Application.Interfaces;
using Domain.Entities;
using Domain.Exceptions;
using Infrastructure.Context;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Infrastructure.Repository
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly IMongoCollection<CustomerEntity> _customerCollection;
        private readonly ILogger<CustomerRepository> _logger;

        public CustomerRepository(IOptions<ShopifyDbContext> shopifyDatabaseSettings, ILogger<CustomerRepository> logger)
        {
            var mongoClient = new MongoClient(
            shopifyDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                shopifyDatabaseSettings.Value.DatabaseName);

            _customerCollection = mongoDatabase.GetCollection<CustomerEntity>(
                shopifyDatabaseSettings.Value.CustomersCollection);
            _logger = logger;
        }
        public async Task Create(CustomerEntity customer)
        {
            try
            {
                await _customerCollection.InsertOneAsync(customer);
            }
            catch (Exception ex)
            {
                string errorMessage = "Error creating customer in database";
                _logger.LogError($"{errorMessage} {ex.Message}");
                throw new CustomHttpException(errorMessage, 500);
            }
        }

        public async Task<CustomerEntity> Get(long? customerId = -1)
        {
            try
            {
                CustomerEntity customer = await _customerCollection.Find<CustomerEntity>(customer => customer.Id.Equals(customerId)).FirstOrDefaultAsync();
                return customer;
            }
            catch (Exception ex)
            {
                string errorMessage = "Error retrieving customer from database";
                _logger.LogError($"{errorMessage} {ex.Message}");
                throw new CustomHttpException(errorMessage, 500);
            }
        }

        public async Task Update(CustomerEntity customer)
        {
            try
            {
                await _customerCollection.ReplaceOneAsync(x => x.Id == customer.Id, customer);
            }
            catch (Exception ex)
            {
                string errorMessage = "Error updating customer on database";
                _logger.LogError($"{errorMessage} {ex.Message}");
                throw new CustomHttpException(errorMessage, 500);
            }
        }
    }
}
