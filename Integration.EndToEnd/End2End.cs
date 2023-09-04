using Basket_API.Entities;
using Catalog_API.Data;
using FluentAssertions;
using Integration.EndToEnd.Fixtures;
using Ordering_Application.Features.Orders.Queries.GetOrdersList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Integration.EndToEnd
{
    public class End2End : IClassFixture<WebApplicationFactoryFixture>
    {
        private readonly WebApplicationFactoryFixture _factory;

        public End2End(WebApplicationFactoryFixture factory)
        {
            _factory = factory;
        }

        [Fact]
        public async void End2EndTest()
        {
            var expectedProductList = CatalogContextSeed.GetPreconfiguredProducts();
            Task.Delay(3000).Wait();
            var listProduct = await _factory.GetCatalogAsync();
            listProduct.Should().BeEquivalentTo(expectedProductList);

            Guid guid = Guid.NewGuid();

            // posting a basket 
            ShoppingCart basket = new ShoppingCart()
            {
                UserName = guid.ToString(),
                Items = new List<ShoppingCartItem>()
                 {
                     new()
                     {
                         Quantity = 3,
                         Color = "Green",
                         Price = 500,
                         ProductId = "60210c2a1556459e153f0554",
                         ProductName = "IPhone X"
                     },
                     new()
                     {
                         Quantity = 1,
                         Color = "Blue",
                         Price = 500,
                         ProductId = "60210c2a1556459e153f0555",
                         ProductName = "Samsung 10"
                     }
                 }
            };

            ShoppingCart espectedbasket = new ShoppingCart()
            {
                UserName = guid.ToString(),
                Items = new List<ShoppingCartItem>()
                 {
                     new()
                     {
                         Quantity = 3,
                         Color = "Green",
                         Price = 350,
                         ProductId = "60210c2a1556459e153f0554",
                         ProductName = "IPhone X"
                     },
                     new()
                     {
                         Quantity = 1,
                         Color = "Blue",
                         Price = 400,
                         ProductId = "60210c2a1556459e153f0555",
                         ProductName = "Samsung 10"
                     }
                 }
            };

            await _factory.PostBasketAsync(basket);


            var basketFromRepo = await _factory.GetBasketAsync(guid.ToString());
            basketFromRepo.Should().BeOfType<ShoppingCart>()
                                   .And.BeEquivalentTo(espectedbasket);


            // checkout basket
            BasketCheckout basketCheckout = new BasketCheckout()
            {
                  UserName = guid.ToString(),
	              TotalPrice = 0,
	              FirstName = "swn",
	              LastName = "swn",
	              EmailAddress = "string",
	              AddressLine = "string",
	              Country = "string",
	              State = "string",
	              ZipCode = "string",
	              CardName = "string",
	              CardNumber = "string",
	              Expiration = "string",
	              CVV = "string",
	              PaymentMethod = 1
            };

            await _factory.CheckoutBasketAsync(basketCheckout);

            var expectedorder = new OrderDto()
            {
                UserName = guid.ToString(),
                TotalPrice = 1450,
                FirstName = "swn",
                LastName = "swn",
                EmailAddress = "string",
                AddressLine = "string",
                Country = "string",
                State = "string",
                ZipCode = "string",
                CardName = "string",
                CardNumber = "string",
                Expiration = "string",
                CVV = "string",
                PaymentMethod = 1
            };

            Task.Delay(3000).Wait();

            var orderFromRepo = await _factory.GetOrderAsync(guid.ToString());

            orderFromRepo.First().Should().BeOfType<OrderDto>()
                                  .And.BeEquivalentTo(expectedorder, option => option.Excluding(x => x.Id)) ;




        }
    }
}