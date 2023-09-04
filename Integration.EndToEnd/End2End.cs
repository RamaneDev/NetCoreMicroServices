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

            

        }
    }
}