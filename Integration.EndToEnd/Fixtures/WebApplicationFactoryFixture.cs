using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using OcelotApiGw;
using Docker.DotNet.Models;
using System.Net.Http;
using RESTFulSense.Clients;
using Catalog_API.Entities;
using Microsoft.AspNetCore.Hosting;
using static Org.BouncyCastle.Math.EC.ECCurve;
using Microsoft.Extensions.Configuration;
using Basket_API.Entities;

namespace Integration.EndToEnd.Fixtures
{
    public class WebApplicationFactoryFixture : IAsyncLifetime
    {
        private readonly WebApplicationFactory<Program> _ocelot_webApplicationFactory;
        private readonly HttpClient _httpClient;
        private readonly RESTFulApiFactoryClient _apiFactoryClient;
        private const string catalogUrl = "/Catalog";
        private const string basketUrl = "/Basket";

        // apis containers
        private readonly IContainer _catalogapiContainer;
        private readonly IContainer _basket_apiContainer;
        private readonly IContainer _discount_apiContainer;
        private readonly IContainer _discount_GrpcContainer;
        private readonly IContainer _ordering_apiContainer;
      

        // database containers
        private readonly IContainer _mongoContainer;
        private readonly IContainer _redisContainer;
        private readonly IContainer _postgreSQLContainer;
        private readonly IContainer _SQLserverContainer;
        private readonly IContainer _rabbitmqContainer;


        static void CreateClient()
        {

        }

        public WebApplicationFactoryFixture()
        {
            _ocelot_webApplicationFactory = new WebApplicationFactory<Program>();       
            _httpClient = _ocelot_webApplicationFactory.CreateClient();
            _apiFactoryClient = new RESTFulApiFactoryClient(_httpClient);

            // docker custom network to connect containers
            var micro_services_net = new NetworkBuilder().WithName("micro_services_net").Build();           
            
            // apis containers
            #region build api container  

            _catalogapiContainer = new ContainerBuilder()
           .WithImage("ramane/catalog_api")
           .WithNetwork(micro_services_net)
           .WithName("catalog_api")
           .WithPortBinding(8000, 80)
           .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
           .WithEnvironment("DatabaseSettings:ConnectionString", "mongodb://catalogdb:27017")
           .Build();

            _basket_apiContainer = new ContainerBuilder()
           .WithImage("ramane/basket_api")
           .WithNetwork(micro_services_net)
           .WithName("basket_api")
           .WithPortBinding(8001, 80)
           .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
           .WithEnvironment("CacheSettings:ConnectionString", "basketdb:6379")
           .WithEnvironment("GrpcSettings:DiscountUrl", "http://discount_grpc")
           .WithEnvironment("EventBusSettings:HostAddress", "amqp://guest:guest@rabbitmq:5672")
           .Build();

            _discount_apiContainer = new ContainerBuilder()
           .WithImage("ramane/discount_api")
           .WithNetwork(micro_services_net)
           .WithName("discount_api")
           .WithPortBinding(8002, 80)
           .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
           .WithEnvironment("DatabaseSettings:ConnectionString", "Server=discountdb;Port=5432;Database=DiscountDb;User Id=admin;Password=admin1234;")
           .Build();

            _discount_GrpcContainer = new ContainerBuilder()
           .WithImage("ramane/discount_grpc")
           .WithNetwork(micro_services_net)
           .WithName("discount_grpc")
           .WithPortBinding(8003, 80)
           .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
           .WithEnvironment("DatabaseSettings:ConnectionString", "Server=discountdb;Port=5432;Database=DiscountDb;User Id=admin;Password=admin1234;")
           .Build();

            _ordering_apiContainer = new ContainerBuilder()
           .WithImage("ramane/ordering_api")
           .WithNetwork(micro_services_net)
           .WithName("ordering_api")
           .WithPortBinding(8004, 80)
           .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
           .WithEnvironment("ConnectionStrings:OrderingConnectionString", "Server=orderdb;Database=OrderDb;User Id=sa;Password=Pa$$w0rd;TrustServerCertificate=True")
           .WithEnvironment("EventBusSettings:HostAddress", "amqp://guest:guest@rabbitmq:5672")
           .Build();

            #endregion

            // database containers
            #region build databases containers
           
            _mongoContainer = new ContainerBuilder()
           .WithImage("mongo")
           .WithNetwork(micro_services_net)
           .WithName("catalogdb")
           .WithExposedPort(27017)
           .WithPortBinding(27017, 27017)
           .Build();

            _redisContainer = new ContainerBuilder()
           .WithImage("redis:alpine")
           .WithNetwork(micro_services_net)
           .WithName("basketdb")
           .WithPortBinding(6379, 6379)
           .Build();

            _postgreSQLContainer = new ContainerBuilder()
           .WithImage("postgres")
           .WithNetwork(micro_services_net)
           .WithName("discountdb")
           .WithPortBinding(5432, 5432)
           .WithEnvironment("POSTGRES_USER", "admin")
           .WithEnvironment("POSTGRES_PASSWORD", "admin1234")
           .WithEnvironment("POSTGRES_DB", "DiscountDb")
           .Build();

            _SQLserverContainer = new ContainerBuilder()
           .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
           .WithName("orderdb")
           .WithNetwork(micro_services_net)
           .WithPortBinding(1433, 1433)
           .WithEnvironment("MSSQL_SA_PASSWORD", "Pa$$w0rd")
           .WithEnvironment("ACCEPT_EULA", "Y")
           .Build();

           _rabbitmqContainer = new ContainerBuilder()
           .WithImage("rabbitmq:3-management-alpine")
           .WithName("rabbitmq")
           .WithNetwork(micro_services_net)
           .WithPortBinding(5672, 5672)
           .WithPortBinding(15672, 15672)            
           .Build();
            #endregion 
        }

        public async ValueTask<IEnumerable<Product>> GetCatalogAsync() =>
           await this._apiFactoryClient.GetContentAsync<IEnumerable<Product>>(catalogUrl);

        public async ValueTask<ShoppingCart> PostBasketAsync(ShoppingCart basket) =>
          await this._apiFactoryClient.PostContentAsync<ShoppingCart>(basketUrl, basket);

        public async ValueTask<ShoppingCart> GetBasketAsync(string username) =>
          await this._apiFactoryClient.GetContentAsync<ShoppingCart>($"{basketUrl}/{username}");


        public async Task InitializeAsync()
        {
            var mongoTask      = _mongoContainer.StartAsync();
            var redisTask      = _redisContainer.StartAsync();
            var postgreSQLTask = _postgreSQLContainer.StartAsync();
            var SQLserverTask  = _SQLserverContainer.StartAsync();
            var rabbitmqTask   = _rabbitmqContainer.StartAsync();

            await Task.WhenAll(mongoTask, redisTask, postgreSQLTask, SQLserverTask, rabbitmqTask);

            Task.Delay(5000).Wait();
            
            var catalogapiTask   = _catalogapiContainer.StartAsync();
            var basketapiTask    = _basket_apiContainer.StartAsync() ;
            var discountapiTask  = _discount_apiContainer.StartAsync();
            var discountgrpcTask = _discount_GrpcContainer.StartAsync();
            var orderingTask     = _ordering_apiContainer.StartAsync();

            await Task.WhenAll(catalogapiTask, basketapiTask, discountapiTask, discountgrpcTask, orderingTask);

     
        }



        public async Task DisposeAsync()
        {
            
            var mongoTask = _mongoContainer.StopAsync();
            var redisTask = _redisContainer.StopAsync();
            var postgreSQLTask = _postgreSQLContainer.StopAsync();
            var SQLserverTask = _SQLserverContainer.StopAsync();
            var rabbitmqTask = _rabbitmqContainer.StopAsync();

            await Task.WhenAll(mongoTask, redisTask, postgreSQLTask, SQLserverTask, rabbitmqTask);

            var catalogapiTask = _catalogapiContainer.StopAsync();
            var basketapiTask = _basket_apiContainer.StopAsync();
            var discountapiTask = _discount_apiContainer.StopAsync();
            var discountgrpcTask = _discount_GrpcContainer.StopAsync();
            var orderingTask = _ordering_apiContainer.StopAsync();

            await Task.WhenAll(catalogapiTask, basketapiTask, discountapiTask, discountgrpcTask, orderingTask);
        }

      
    }
}
