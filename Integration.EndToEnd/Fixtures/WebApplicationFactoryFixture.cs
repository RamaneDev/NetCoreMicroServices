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

namespace Integration.EndToEnd.Fixtures
{
    public class WebApplicationFactoryFixture : IAsyncLifetime
    {
        private readonly WebApplicationFactory<Program> _ocelot_webApplicationFactory;
        //private readonly HttpClient _httpClient;
        //private readonly RESTFulApiFactoryClient _apiFactoryClient;

        // apis containers
        private readonly IContainer _catalogContainer;
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

        public WebApplicationFactoryFixture()
        {
            _ocelot_webApplicationFactory = new WebApplicationFactory<Program>();
            
            var micro_services_net = new NetworkBuilder().WithName("customNetwork").Build();

            // apis containers
            _catalogContainer = new ContainerBuilder()
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
           .WithEnvironment("GrpcSettings:DiscountUrl", "http://dicount_grpc")
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





            // database containers
            _mongoContainer = new ContainerBuilder()
           .WithImage("mongo")
           .WithNetwork(micro_services_net)
           .WithName("catalogdb")
           //.WithNetwork("bridge")
           //.WithNetworkAliases("catalogdb")
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
        }

        public async Task InitializeAsync()
        {
            var mongoTask      = _mongoContainer.StartAsync();
            var redisTask      = _redisContainer.StartAsync();
            var postgreSQLTask = _postgreSQLContainer.StartAsync();
            var SQLserverTask  = _SQLserverContainer.StartAsync();
            var rabbitmqTask   = _rabbitmqContainer.StartAsync();

            await Task.WhenAll(mongoTask, redisTask, postgreSQLTask, SQLserverTask, rabbitmqTask);

            await _catalogContainer.StartAsync();
        }



        public async Task DisposeAsync()
        {
            
            var mongoTask = _mongoContainer.StopAsync();
            var redisTask = _redisContainer.StopAsync();
            var postgreSQLTask = _postgreSQLContainer.StopAsync();
            var SQLserverTask = _SQLserverContainer.StopAsync();
            var rabbitmqTask = _rabbitmqContainer.StopAsync();

            await Task.WhenAll(mongoTask, redisTask, postgreSQLTask, SQLserverTask, rabbitmqTask);

            await _catalogContainer.StopAsync();
        }

      
    }
}
