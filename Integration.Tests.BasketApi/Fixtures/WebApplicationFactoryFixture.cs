using RESTFulSense.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using Basket_API;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Builders;
using Integration.Tests.BasketApi.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Basket_API.GRPC_Services;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Dicount_GRPC;
using CouponModel = Dicount_GRPC.CouponModel;
using Org.BouncyCastle.Asn1.X509.Qualified;
using Integration.Tests.BasketApi.Helpers;
using Basket_API.Repositories;
using Integration.Tests.BasketApi.fakeServices;

namespace Integration.Tests.BasketApi.Fixtures
{
    public class WebApplicationFactoryFixture : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private readonly WebApplicationFactory<Program> _webApplicationFactory;
        private readonly HttpClient _httpClient;
        private readonly RESTFulApiFactoryClient _apiFactoryClient;
  

        private const string basketurl = "api/v1/Basket";

        private readonly IContainer _redisContainer;

      

            
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
          
            
            base.ConfigureWebHost(builder);

            builder.ConfigureServices(services =>
            {
                services.RemoveAll<IDiscountGrpcService>();
                services.AddScoped<IDiscountGrpcService, FakeGrpcService>();
               
            });
        }

        public WebApplicationFactoryFixture()
        {
            _webApplicationFactory = this;
            _httpClient = _webApplicationFactory.CreateClient();
            _apiFactoryClient = new RESTFulApiFactoryClient(_httpClient);

              _redisContainer = new ContainerBuilder()
             .WithImage("redis:alpine")
             .WithPortBinding(6379)
             .Build();
        }



        public async ValueTask<ShoppingCart> PostBasketAsync(ShoppingCart basket) =>
           await this._apiFactoryClient.PostContentAsync(basketurl, basket);

        public async ValueTask<ShoppingCart> GetBasketAsync(string id) =>
            await this._apiFactoryClient.GetContentAsync<ShoppingCart>($"{basketurl}/{id}");

        public async ValueTask DeleteBasketAsync(string id) =>
            await this._apiFactoryClient.DeleteContentAsync($"{basketurl}/{id}");





      

        public async Task InitializeAsync()
        {
            await _redisContainer.StartAsync();            
        }

        public async new Task DisposeAsync()
        {
            await _redisContainer.StopAsync();
        }
    }

    
}
