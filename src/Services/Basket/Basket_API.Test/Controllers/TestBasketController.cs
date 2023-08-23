using AutoMapper;
using Basket_API.Controllers;
using Basket_API.Entities;
using Basket_API.GRPC_Services;
using Basket_API.Repositories;
using FluentAssertions;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Basket_API.Test.Controllers
{
    public class TestBasketController
    {
        [Fact]
        public async Task Get_OnNoBasketFound_Reurns404()     // success
        {
            var mockBasketRepo = new Mock<IBasketRepository>();      
            string basketUsername = "notFound";
            var emptyshoppingCart = new ShoppingCart()
            {
                UserName = "",
                Items = new List<ShoppingCartItem>(),
            };

            mockBasketRepo.Setup(service => service.GetBasket(basketUsername)).ReturnsAsync((ShoppingCart)null);

            var sut = new BasketController(mockBasketRepo.Object,
                null,
                null,
                null);

            var actionResult = await sut.GetBasket(basketUsername);

            var IactionRsult = ((IConvertToActionResult)actionResult).Convert();

            IactionRsult.Should().BeOfType<NotFoundObjectResult>();

            var objectResult = (ObjectResult)IactionRsult;
            objectResult.Value.Should().BeOfType<ShoppingCart>();
            objectResult.Value.Should().BeEquivalentTo(emptyshoppingCart);
            objectResult.StatusCode.Should().Be(404);
            
        }
    }
}
