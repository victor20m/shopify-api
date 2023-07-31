using Application.Interfaces;
using Application.Utility;
using Domain.Entities;
using Domain.Exceptions;
using Microsoft.Extensions.Logging;
using ShopifySharp;

namespace Application.Implementations
{
    public class OrderLocalService : IOrderLocalService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ILogger<OrderLocalService> _logger;
        public OrderLocalService(ICustomerRepository customerRepository, ILogger<OrderLocalService> logger)
        {
            _customerRepository = customerRepository;
            _logger = logger;
        }

        public async Task Create(Order order)
        {
            try
            {
                CustomerEntity customer = await _customerRepository.Get(order.Customer.Id);
                decimal orderAmount = order.TotalPrice ?? 0;
                int rewardPoints = GiftPointsFormula.CalculateRewardPoints(orderAmount);

                if (customer is null)
                {
                    customer = new CustomerEntity(order.Customer)
                    {
                        Orders = new List<Order> { order },
                        RewardPoints = rewardPoints
                    };
                    await _customerRepository.Create(customer);
                }
                else
                {
                    customer.Orders.Add(order);
                    customer.RewardPoints += rewardPoints;
                    await _customerRepository.Update(customer);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while creating order for Customer Id: {order.Customer.Id}");
                if (ex is CustomHttpException customHttpException) throw customHttpException;
                else throw new CustomHttpException($"Error while creating order for customer {order.Customer.FirstName}", 500);

            }
        }
    }
}