using Application.Interfaces;
using Application.Utility;
using Domain.Entities;
using Domain.Exceptions;
using Microsoft.Extensions.Logging;
using Models.In;
using Models.Out;
using ShopifySharp;
namespace Application.Implementations
{
    public class CustomerLocalService : ICustomerLocalService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IShopifyRepository _shopifyRepository;
        private readonly ILogger<CustomerLocalService> _logger;
        public CustomerLocalService(ICustomerRepository repository, IShopifyRepository shopifyRepository, ILogger<CustomerLocalService> logger)
        {
            _customerRepository = repository;
            _shopifyRepository = shopifyRepository;
            _logger = logger;
        }

        public async Task Create(Customer customer)
        {
            CustomerEntity newCustomer = new(customer);
            CustomerEntity customerEntity = await _customerRepository.Get(customer.Id);
            if (customerEntity is null)
            {
                await _customerRepository.Create(newCustomer);
            }
            else
            {
                throw new CustomHttpException($"The customer already exists.", 409);
            }
        }

        public async Task<CustomerEntity> Get(long? customerId)
        {
            CustomerEntity customer = await _customerRepository.Get(customerId);
            return customer is null ? throw new CustomHttpException($"The customer does not exist.", 404) : customer;
        }

        public async Task Update(CustomerEntity customer)
        {
            CustomerEntity customerEntity = await _customerRepository.Get(customer.Id);
            if (customerEntity is null)
            {
                throw new CustomHttpException($"The customer does not exist.", 404);
            }
            else
            {
                await _customerRepository.Update(customer);
            }
        }

        public async Task<AvailablePointsModel> GetPoints(long customerId)
        {
            CustomerEntity customerEntity = await _customerRepository.Get(customerId);
            if (customerEntity is null)
            {
                throw new CustomHttpException($"The customer does not exist.", 404);
            }
            else
            {
                return new AvailablePointsModel()
                {
                    CustomerId = customerEntity.Id.ToString(),
                    AvailablePoints = customerEntity.RewardPoints.ToString()
                };
            }
        }

        public async Task<GiftCard> Redeem(CustomerRedeemModel redeemRequest)
        {
            CustomerEntity customerEntity = await _customerRepository.Get(redeemRequest.CustomerId);
            if (customerEntity is null)
            {
                throw new CustomHttpException($"The customer does not exist.", 404);
            }
            if (redeemRequest.Points < 10)
            {
                throw new CustomHttpException($"Reward points should be at least 10.", 400);
            }
            if (customerEntity.RewardPoints < redeemRequest.Points)
            {
                throw new CustomHttpException($"The customer does not have enough points to redeem a card. Available points: {customerEntity.RewardPoints}", 400);
            }
            customerEntity.RewardPoints -= redeemRequest.Points;
            await _customerRepository.Update(customerEntity);
            GiftCard giftCard = new();
            int rewardAmount;
            try
            {
                rewardAmount = GiftPointsFormula.CalculateRewardAmount(redeemRequest.Points);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                throw new CustomHttpException($"Error while calculating reward amount.", 500);
            }
            giftCard.InitialValue = rewardAmount;
            return await _shopifyRepository.CreateGiftCard(giftCard, redeemRequest.CustomerId);
        }
    }
}