using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Catalog_API;
using System.Net.Http;
using RESTFulSense.Clients;
using Integration.Tests.Models;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Builders;
using ZstdSharp.Unsafe;

namespace Integration.Tests.Fixtures
{
    public class WebApplicationFactoryFixture : IAsyncLifetime
    {
        private readonly WebApplicationFactory<Program> _webApplicationFactory;
        private readonly HttpClient _httpClient;
        private readonly RESTFulApiFactoryClient _apiFactoryClient;

        private const string catalogurl = "api/v1/Catalog";

        private readonly IContainer _mongoContainer;

        public WebApplicationFactoryFixture()
        {
            _webApplicationFactory = new WebApplicationFactory<Program>();
            _httpClient = _webApplicationFactory.CreateClient();
            _apiFactoryClient = new RESTFulApiFactoryClient(_httpClient);

            _mongoContainer = new ContainerBuilder()
             .WithImage("mongo")
             .WithPortBinding(27017)
             .Build();

        }   
        

        public async ValueTask<Product> PostProductAsync(Product product) => 
             await this._apiFactoryClient.PostContentAsync(catalogurl, product);

        public async ValueTask<Product> GetProductAsync(string id) =>
            await this._apiFactoryClient.GetContentAsync<Product>($"{catalogurl}/{id}");

        public async ValueTask<bool> DeleteProductAsync(string id) =>
            await this._apiFactoryClient.DeleteContentAsync<bool>($"{catalogurl}/{id}");

        public async Task InitializeAsync()
        {
            await _mongoContainer.StartAsync();
           
        }


        public async Task DisposeAsync()
        {
           await _mongoContainer.StopAsync();
        }
    }
}
