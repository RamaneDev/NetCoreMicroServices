using Microsoft.Extensions.Logging;
using Ordering_Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering_Infrastructure.Persistence
{
    public class OrderContextSeed
    {
        public static async Task SeedAsync(OrderContext orderContext, ILogger<OrderContextSeed> logger)
        {
            if (!orderContext.Orders.Any())
            {
                orderContext.Orders.AddRange(GetPreconfiguredOrders());
                await orderContext.SaveChangesAsync();
                logger.LogInformation("Seed database associated with context {DbContextName}", typeof(OrderContext).Name);
            }
        }

        private static IEnumerable<Order> GetPreconfiguredOrders()
        {
            return new List<Order>
            {
                new Order() {UserName = "swn", FirstName = "Mesri", LastName = "Abderrahmane", EmailAddress = "abde.mes.dciss@gmail.com", AddressLine = "Rue Emmanuel Arago", Country = "France", TotalPrice = 350 },
                new Order() {UserName = "swn1", FirstName = "Yousefi", LastName = "Adlen", EmailAddress = "abde_mes@yahoo.fr", AddressLine = "Rue Jean-Geores", Country = "France", TotalPrice = 350 }
            };
        }
    }
}
