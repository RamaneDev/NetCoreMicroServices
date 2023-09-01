using FluentAssertions;
using Integration.Tests.BasketApi.Fixtures;
using Integration.Tests.BasketApi.Models;
using Microsoft.OpenApi.Validations;
using RESTFulSense.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Schema;
using Xunit;

namespace Integration.Tests.BasketApi
{
    public class Basket_Api_Tests : IClassFixture<WebApplicationFactoryFixture>
    {
        private readonly WebApplicationFactoryFixture _factory;

        public Basket_Api_Tests(WebApplicationFactoryFixture factory)
        {
            _factory = factory;
        }

        [Fact]
        public async void BasketAPI_Integration_Test()
        {
            ShoppingCart basket = new ShoppingCart()
            {

                UserName = "swn",
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

            ShoppingCart basketAfterDiscount = new ShoppingCart()
            {

                UserName = "swn",
                Items = new List<ShoppingCartItem>()
                {
                    new()
                    {
                        Quantity = 3,
                        Color = "Green",
                        Price = 490,
                        ProductId = "60210c2a1556459e153f0554",
                        ProductName = "IPhone X"
                    },
                    new()
                    {
                        Quantity = 1,
                        Color = "Blue",
                        Price = 490,
                        ProductId = "60210c2a1556459e153f0555",
                        ProductName = "Samsung 10"
                    }
                }
            };



            await _factory.PostBasketAsync(basket);

            var resultFromRedis = await _factory.GetBasketAsync(basket.UserName);

            resultFromRedis.Should().BeEquivalentTo<ShoppingCart>(basketAfterDiscount);

            await _factory.DeleteBasketAsync(basket.UserName);

            Func<Task> asyncGet = async () => await _factory.GetBasketAsync(basket.UserName);

            await asyncGet.Should().ThrowAsync<HttpResponseNotFoundException>();

        }

    }
}