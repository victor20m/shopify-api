using Application.Implementations;
using Application.Interfaces;
using Domain.Entities;
using Domain.Exceptions;
using Microsoft.Extensions.Logging;
using Models.In;
using Moq;
using ShopifySharp;

namespace Application.Tests
{
    [TestFixture]
    public class CustomerLocalServiceTests
    {
        private Mock<ICustomerRepository> _customerRepositoryMock;
        private Mock<IShopifyRepository> _shopifyRepositoryMock;
        private Mock<ILogger<CustomerLocalService>> _loggerMock;
        private CustomerLocalService _service;

        [SetUp]
        public void Setup()
        {
            _customerRepositoryMock = new Mock<ICustomerRepository>();
            _shopifyRepositoryMock = new Mock<IShopifyRepository>();
            _loggerMock = new Mock<ILogger<CustomerLocalService>>();
            _service = new CustomerLocalService(_customerRepositoryMock.Object, _shopifyRepositoryMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task CreateCustomer_WhenCustomerDoesNotExist_ShouldCreateCustomer()
        {
            var customer = new Customer { Id = 1 };
            _customerRepositoryMock.Setup(x => x.Get(It.IsAny<long>())).Returns(Task.FromResult<CustomerEntity>(null));
            _customerRepositoryMock.Setup(x => x.Create(It.IsAny<CustomerEntity>())).Returns(Task.CompletedTask);

            await _service.Create(customer);

            _customerRepositoryMock.Verify(x => x.Create(It.IsAny<CustomerEntity>()), Times.Once);
        }

        [Test]
        public void CreateCustomer_WhenCustomerExists_ShouldThrowException()
        {
            var customer = new Customer { Id = 1 };
            _customerRepositoryMock.Setup(x => x.Get(It.IsAny<long>())).Returns(Task.FromResult(new CustomerEntity(customer)));

            Assert.ThrowsAsync<CustomHttpException>(() => _service.Create(customer));

            _customerRepositoryMock.Verify(x => x.Create(It.IsAny<CustomerEntity>()), Times.Never);
        }

        [Test]
        public async Task GetCustomer_WhenCustomerExists_ShouldReturnCustomer()
        {
            var customerId = 1;
            var expectedCustomerEntity = new CustomerEntity(new Customer { Id = customerId });
            _customerRepositoryMock.Setup(x => x.Get(It.IsAny<long>())).Returns(Task.FromResult(expectedCustomerEntity));

            var actualCustomerEntity = await _service.Get(customerId);

            Assert.That(actualCustomerEntity, Is.EqualTo(expectedCustomerEntity));
        }

        [Test]
        public void GetCustomer_WhenCustomerDoesNotExist_ShouldThrowException()
        {
            var customerId = 1;
            _customerRepositoryMock.Setup(x => x.Get(It.IsAny<long>())).Returns(Task.FromResult<CustomerEntity>(null));

            Assert.ThrowsAsync<CustomHttpException>(() => _service.Get(customerId));
        }

        [Test]
        public async Task GetPoints_WhenCustomerExists_ShouldReturnAvailablePointsModel()
        {
            var customerId = 1;
            var expectedCustomerEntity = new CustomerEntity(new Customer { Id = customerId })
            {
                RewardPoints = 50
            };
            _customerRepositoryMock.Setup(x => x.Get(It.IsAny<long>())).Returns(Task.FromResult(expectedCustomerEntity));

            var actualPointsModel = await _service.GetPoints(customerId);

            Assert.That(actualPointsModel.CustomerId, Is.EqualTo(expectedCustomerEntity.Id.ToString()));
            Assert.That(actualPointsModel.AvailablePoints, Is.EqualTo(expectedCustomerEntity.RewardPoints.ToString()));
        }

        [Test]
        public void GetPoints_WhenCustomerDoesNotExist_ShouldThrowException()
        {
            var customerId = 1;
            _customerRepositoryMock.Setup(x => x.Get(It.IsAny<long>())).Returns(Task.FromResult<CustomerEntity>(null));

            Assert.ThrowsAsync<CustomHttpException>(() => _service.GetPoints(customerId));
        }

        [Test]
        public async Task Redeem_WhenCustomerExistsAndHasEnoughPoints_ShouldRedeemPoints()
        {
            var customerId = 1;
            var redeemRequest = new CustomerRedeemModel { CustomerId = customerId, Points = 10 };
            var customerEntity = new CustomerEntity(new Customer { Id = customerId })
            {
                RewardPoints = 50
            };
            var expectedGiftCard = new GiftCard { InitialValue = 100 };

            _customerRepositoryMock.Setup(x => x.Get(It.IsAny<long>())).Returns(Task.FromResult(customerEntity));
            _customerRepositoryMock.Setup(x => x.Update(It.IsAny<CustomerEntity>())).Callback<CustomerEntity>((c) =>
            {
                Assert.That(c.Id, Is.EqualTo(customerId));
                Assert.That(c.RewardPoints, Is.EqualTo(customerEntity.RewardPoints));
            }).Returns(Task.CompletedTask);

            _shopifyRepositoryMock.Setup(x => x.CreateGiftCard(It.IsAny<GiftCard>(), It.IsAny<long>())).Returns(Task.FromResult(expectedGiftCard));

            var actualGiftCard = await _service.Redeem(redeemRequest);

            Assert.That(actualGiftCard.InitialValue, Is.EqualTo(expectedGiftCard.InitialValue));
            _customerRepositoryMock.Verify(x => x.Update(It.IsAny<CustomerEntity>()), Times.Once);
        }


        [Test]
        public void Redeem_WhenCustomerDoesNotExist_ShouldThrowException()
        {
            var redeemRequest = new CustomerRedeemModel { CustomerId = 1, Points = 10 };
            _customerRepositoryMock.Setup(x => x.Get(It.IsAny<long>())).Returns(Task.FromResult<CustomerEntity>(null));

            Assert.ThrowsAsync<CustomHttpException>(() => _service.Redeem(redeemRequest));
        }

        [Test]
        public void Redeem_WhenCustomerExistsButPointsAreLessThan10_ShouldThrowException()
        {
            var customerId = 1;
            var redeemRequest = new CustomerRedeemModel { CustomerId = customerId, Points = 5 };
            var customerEntity = new CustomerEntity(new Customer { Id = customerId })
            {
                RewardPoints = 50
            };

            _customerRepositoryMock.Setup(x => x.Get(It.IsAny<long>())).Returns(Task.FromResult(customerEntity));

            Assert.ThrowsAsync<CustomHttpException>(() => _service.Redeem(redeemRequest));
        }

        [Test]
        public void Redeem_WhenCustomerExistsButDoesNotHaveEnoughPoints_ShouldThrowException()
        {
            var customerId = 1;
            var redeemRequest = new CustomerRedeemModel { CustomerId = customerId, Points = 60 };
            var customerEntity = new CustomerEntity(new Customer { Id = customerId })
            {
                RewardPoints = 50
            };
            _customerRepositoryMock.Setup(x => x.Get(It.IsAny<long>())).Returns(Task.FromResult(customerEntity));

            Assert.ThrowsAsync<CustomHttpException>(() => _service.Redeem(redeemRequest));
        }

    }
}
