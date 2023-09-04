using Basket_API.Entities;
using Catalog_API.Data;
using FluentAssertions;
using Integration.EndToEnd.Fixtures;
using System.Collections.Generic;
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

            // posting a basket 
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

            ShoppingCart espectedbasket = new ShoppingCart()
            {
                UserName = "swn",
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


            var basketFromRepo = await _factory.GetBasketAsync("swn");
            basketFromRepo.Should().BeOfType<ShoppingCart>()
                                   .And.BeEquivalentTo(espectedbasket);

        }
    }
}