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
        public async Task GetBasket_OnCall_InvokeBasketRepositoryExactlyOnce()   // sucess
        {
            //Arrange
            var mockBasketRepo = new Mock<IBasketRepository>();
            string basketUsername = "Found";
            mockBasketRepo.Setup(service => service.GetBasket(basketUsername)).ReturnsAsync(new ShoppingCart(basketUsername));

            var sut = new BasketController(mockBasketRepo.Object,
                null,
                null,
                null);

            //Act
            var actionResult = await sut.GetBasket(basketUsername);

            var IactionRsult = ((IConvertToActionResult)actionResult).Convert();

            var objectResult = (ObjectResult)IactionRsult;

            //Assert
            mockBasketRepo.Verify(service => service.GetBasket(basketUsername), Times.Once);


        }

        [Fact]
        public async Task GetBasket_OnNoBasketFound_Returns_NotFound_404()   // sucess
        {

            //Arrange
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
            
            //Act
            var actionResult = await sut.GetBasket(basketUsername);
            var IactionRsult = ((IConvertToActionResult)actionResult).Convert();
            var objectResult = (ObjectResult)IactionRsult;

            //Assert
            IactionRsult.Should().BeOfType<NotFoundObjectResult>();       
            objectResult.StatusCode.Should().Be(404);
            objectResult.Value.Should().BeOfType<ShoppingCart>();
            objectResult.Value.Should().BeEquivalentTo(emptyshoppingCart);
            
            
        }

        [Fact]
        public async Task GetBasket_OnBasketFound_Returns_Sucess_200()   // sucess
        {
            //Arrange
            var mockBasketRepo = new Mock<IBasketRepository>();
            string basketUsername = "Found";
            mockBasketRepo.Setup(service => service.GetBasket(basketUsername)).ReturnsAsync(new ShoppingCart(basketUsername));

            //Act
            var sut = new BasketController(mockBasketRepo.Object,
                null,
                null,
                null);

            var actionResult = await sut.GetBasket(basketUsername);
            var IactionRsult = ((IConvertToActionResult)actionResult).Convert();
            var objectResult = (ObjectResult)IactionRsult;

            //Assert
            IactionRsult.Should().BeOfType<OkObjectResult>();            
            objectResult.StatusCode.Should().Be(200);
            objectResult.Value.Should().BeOfType<ShoppingCart>();
            objectResult.Value.Should().NotBeNull()
                                       .And.BeOfType<ShoppingCart>()
                                       .Which.UserName.Should().Be("Found");


        }



    }
}
