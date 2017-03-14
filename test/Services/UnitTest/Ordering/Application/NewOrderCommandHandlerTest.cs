﻿using Microsoft.eShopOnContainers.Services.Ordering.API.Application.Commands;
using Microsoft.eShopOnContainers.Services.Ordering.API.Infrastructure.Services;
using Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel.BuyerAggregate;
using Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel.OrderAggregate;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;


namespace UnitTest.Ordering.Application
{
    using MediatR;
    using System.Collections;
    using System.Collections.Generic;
    using Xunit;
    public class NewOrderRequestHandlerTest
    {
        private readonly Mock<IOrderRepository<Order>> _orderRepositoryMock;
        private readonly Mock<IIdentityService> _identityServiceMock;
        private readonly Mock<IMediator> _mediator;

        public NewOrderRequestHandlerTest()
        {

            _orderRepositoryMock = new Mock<IOrderRepository<Order>>();
            _identityServiceMock = new Mock<IIdentityService>();
            _mediator = new Mock<IMediator>();
        }

        [Fact]
        public async Task Handle_returns_true_when_order_is_persisted_succesfully()
        {

            var buyerId = "1234";

            var fakeOrderCmd = FakeOrderRequestWithBuyer(new Dictionary<string, object>
            { ["cardExpiration"] = DateTime.Now.AddYears(1) });

            // Arrange
            _orderRepositoryMock.Setup(orderRepo => orderRepo.GetAsync(It.IsAny<int>()))
               .Returns(Task.FromResult<Order>(FakeOrder()));

            _orderRepositoryMock.Setup(buyerRepo => buyerRepo.UnitOfWork.SaveChangesAsync(default(CancellationToken)))
                .Returns(Task.FromResult(1));

            _identityServiceMock.Setup(svc => svc.GetUserIdentity()).Returns(buyerId);
            //Act
            var handler = new CreateOrderCommandHandler(_mediator.Object, _orderRepositoryMock.Object, _identityServiceMock.Object);
            var result = await handler.Handle(fakeOrderCmd);

            //Assert
            Assert.True(result);
        }        

        [Fact]
        public async Task Handle_return_false_if_order_is_not_persisted()
        {
            var buyerId = "1234";

            var fakeOrderCmd = FakeOrderRequestWithBuyer(new Dictionary<string, object>
            { ["cardExpiration"] = DateTime.Now.AddYears(1) });

            _orderRepositoryMock.Setup(orderRepo => orderRepo.GetAsync(It.IsAny<int>()))
               .Returns(Task.FromResult<Order>(FakeOrder()));

            _orderRepositoryMock.Setup(buyerRepo => buyerRepo.UnitOfWork.SaveChangesAsync(default(CancellationToken)))
                .Returns(Task.FromResult(1));

            _identityServiceMock.Setup(svc => svc.GetUserIdentity()).Returns(buyerId);

            //Act
            var handler = new CreateOrderCommandHandler(_mediator.Object, _orderRepositoryMock.Object, _identityServiceMock.Object);
            var result = await handler.Handle(fakeOrderCmd);

            //Assert
            Assert.False(result);
        }

        [Fact]
        public async Task Handle_throws_exception_when_order_expired()
        {

            var buyerId = "1234";

            var fakeOrderCmd = FakeOrderRequestWithBuyer(new Dictionary<string, object>
            { ["cardExpiration"] = DateTime.Now.AddYears(-1) });

            // Arrange
            _orderRepositoryMock.Setup(orderRepo => orderRepo.GetAsync(It.IsAny<int>()))
               .Returns(Task.FromResult<Order>(FakeOrder()));

            _orderRepositoryMock.Setup(buyerRepo => buyerRepo.UnitOfWork.SaveChangesAsync(default(CancellationToken)))
                .Returns(Task.FromResult(1));

            _identityServiceMock.Setup(svc => svc.GetUserIdentity()).Returns(buyerId);
            //Act
            var handler = new CreateOrderCommandHandler(_mediator.Object, _orderRepositoryMock.Object, _identityServiceMock.Object);

            //Assert
            await Assert.ThrowsAsync<ArgumentException>(async () => await handler.Handle(fakeOrderCmd));
        }        

        [Fact]
        public async Task Handle_throws_exception_when_no_holdername()
        {

            var buyerId = "1234";

            var fakeOrderCmd = FakeOrderRequestWithBuyer(new Dictionary<string, object>
            {
                ["cardExpiration"] = DateTime.Now.AddYears(1),
                ["cardHolderName"] = string.Empty,
            });

            // Arrange
            _orderRepositoryMock.Setup(orderRepo => orderRepo.GetAsync(It.IsAny<int>()))
               .Returns(Task.FromResult<Order>(FakeOrder()));

            _orderRepositoryMock.Setup(buyerRepo => buyerRepo.UnitOfWork.SaveChangesAsync(default(CancellationToken)))
                .Returns(Task.FromResult(1));

            _identityServiceMock.Setup(svc => svc.GetUserIdentity()).Returns(buyerId);
            //Act
            var handler = new CreateOrderCommandHandler(_mediator.Object, _orderRepositoryMock.Object, _identityServiceMock.Object);

            //Assert
            await Assert.ThrowsAsync<ArgumentException>(async () => await handler.Handle(fakeOrderCmd));
        }

        [Fact]
        public async Task Handle_throws_exception_when_no_securityNumber()
        {

            var buyerId = "1234";

            var fakeOrderCmd = FakeOrderRequestWithBuyer(new Dictionary<string, object>
            {
                ["cardExpiration"] = DateTime.Now.AddYears(1),
                ["cardSecurityNumber"] = string.Empty,
            });

            // Arrange
            _orderRepositoryMock.Setup(orderRepo => orderRepo.GetAsync(It.IsAny<int>()))
               .Returns(Task.FromResult<Order>(FakeOrder()));

            _orderRepositoryMock.Setup(buyerRepo => buyerRepo.UnitOfWork.SaveChangesAsync(default(CancellationToken)))
                .Returns(Task.FromResult(1));

            _identityServiceMock.Setup(svc => svc.GetUserIdentity()).Returns(buyerId);
            //Act
            var handler = new CreateOrderCommandHandler(_mediator.Object, _orderRepositoryMock.Object, _identityServiceMock.Object);

            //Assert
            await Assert.ThrowsAsync<ArgumentException>(async () => await handler.Handle(fakeOrderCmd));
        }

        [Fact]
        public async Task Handle_throws_exception_when_no_cardNumber()
        {

            var orderId = "1234";

            var fakeOrderCmd = FakeOrderRequestWithBuyer(new Dictionary<string, object>
            {
                ["cardExpiration"] = DateTime.Now.AddYears(1),
                ["cardNumber"] = string.Empty,
            });

            // Arrange
            _orderRepositoryMock.Setup(orderRepo => orderRepo.GetAsync(It.IsAny<int>()))
               .Returns(Task.FromResult<Order>(FakeOrder()));

            _orderRepositoryMock.Setup(orderRepo => orderRepo.UnitOfWork.SaveChangesAsync(default(CancellationToken)))
                .Returns(Task.FromResult(1));

            _identityServiceMock.Setup(svc => svc.GetUserIdentity()).Returns(orderId);
            //Act
            var handler = new CreateOrderCommandHandler(_mediator.Object, _orderRepositoryMock.Object, _identityServiceMock.Object);

            //Assert
            await Assert.ThrowsAsync<ArgumentException>(async () => await handler.Handle(fakeOrderCmd));
        }

        [Fact]
        public void Handle_throws_exception_when_no_buyerId()
        {
            //Assert
            Assert.Throws<ArgumentNullException>(() => new Buyer(string.Empty));
        }

        private Buyer FakeBuyer()
        {
            return new Buyer(Guid.NewGuid().ToString());
        }

        private Order FakeOrder()
        {
            return new Order(new Address("street", "city", "state", "country", "zipcode"), 1, "12", "111", "fakeName", DateTime.Now.AddYears(1));
        }

        private CreateOrderCommand FakeOrderRequestWithBuyer(Dictionary<string, object> args = null)
        {
            return new CreateOrderCommand(
                city: args != null && args.ContainsKey("city") ? (string)args["city"] : null,
                street: args != null && args.ContainsKey("street") ? (string)args["street"] : null,
                state: args != null && args.ContainsKey("state") ? (string)args["state"] : null,
                country: args != null && args.ContainsKey("country") ? (string)args["country"] : null,
                zipcode: args != null && args.ContainsKey("zipcode") ? (string)args["zipcode"] : null,
                cardNumber: args != null && args.ContainsKey("cardNumber") ? (string)args["cardNumber"] : "1234",
                cardExpiration: args != null && args.ContainsKey("cardExpiration") ? (DateTime)args["cardExpiration"] : DateTime.MinValue,
                cardSecurityNumber: args != null && args.ContainsKey("cardSecurityNumber") ? (string)args["cardSecurityNumber"] : "123",
                cardHolderName: args != null && args.ContainsKey("cardHolderName") ? (string)args["cardHolderName"] : "XXX",
                cardTypeId: args != null && args.ContainsKey("cardTypeId") ? (int)args["cardTypeId"] : 0,
                paymentId: args != null && args.ContainsKey("paymentId") ? (int)args["paymentId"] : 0,
                buyerId: args != null && args.ContainsKey("buyerId") ? (int)args["buyerId"] : 0);                
        }
    }
}
