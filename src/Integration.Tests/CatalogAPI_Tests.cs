
using FluentAssertions;
using Integration.Tests.Fixtures;
using Integration.Tests.Models;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Xunit;
using System;
using RESTFulSense.Exceptions;

namespace Integration.Tests
{
    public class CatalogAPI_Tests : IClassFixture<WebApplicationFactoryFixture>
    {
        private readonly WebApplicationFactoryFixture _factory;

        public CatalogAPI_Tests(WebApplicationFactoryFixture factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task PostAndGetProduct()
        {
            Product pro = new Product()
            {
                Id = "abcdefabcdefabcdefabcdef",
                Name = "Asus Laptop",
                Category = "Computers",
                Summary = "Summary",
                Description = "Description",
                ImageFile = "ImageFile",
                Price = 54
            };


                await _factory.PostProductAsync(pro);

                var getresult = await _factory.GetProductAsync(pro.Id);

                 getresult.Should().BeOfType<Product>()
                               .And.BeEquivalentTo<Product>(pro);

                 var deletresult = await _factory.DeleteProductAsync(pro.Id);
                  deletresult.Should().BeTrue();

                 Func<Task> asyncGet =  async () => await _factory.GetProductAsync(pro.Id);


                 await asyncGet.Should().ThrowAsync<HttpResponseNotFoundException>();          
        }


     }
}