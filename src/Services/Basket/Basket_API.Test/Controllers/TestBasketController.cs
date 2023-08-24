using AutoMapper;
using Basket_API.Controllers;
using Basket_API.Entities;
using Basket_API.GRPC_Services;
using Basket_API.Repositories;
using Dicount_GRPC;
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

        [Fact]
        public async Task UpdateBasket_OnBasketFound_InvokeGrpcServiceForEachElements()   // sucess
        {
            //Arrange
            var mockBasketRepo = new Mock<IBasketRepository>();
            var mokDiscountGrpc = new Mock<DiscountGrpcService>();
            var basketToUpdate = new ShoppingCart()
            {
                UserName = "username",
                Items = new List<ShoppingCartItem>()
                {
                    new ShoppingCartItem() {ProductName="p1", Color="red", Price=15, ProductId="p1", Quantity=3},
                    new ShoppingCartItem() {ProductName="p2", Color="green", Price=15, ProductId="p2", Quantity=3},
                    new ShoppingCartItem() {ProductName="p3", Color="black", Price=15, ProductId="p3", Quantity=3}
                }
            };

            var updatedBasket = new ShoppingCart()
            {
                UserName = "username",
                Items = new List<ShoppingCartItem>()
                {
                    new ShoppingCartItem() {ProductName="p1", Color="red", Price=5, ProductId="p1", Quantity=3},
                    new ShoppingCartItem() {ProductName="p2", Color="green", Price=5, ProductId="p2", Quantity=3},
                    new ShoppingCartItem() {ProductName="p3", Color="black", Price=5, ProductId="p3", Quantity=3}
                }
            };
         
            mockBasketRepo.Setup(service => service.UpdateBasket(basketToUpdate)).ReturnsAsync(updatedBasket);
            mokDiscountGrpc.Setup(service => service.GetDiscount(It.IsAny<string>())).ReturnsAsync(new CouponModel() {});
           

            //Act
            var sut = new BasketController(mockBasketRepo.Object,
                mokDiscountGrpc.Object,
                null,
                null);

            var result = await sut.UpdateBasket(basketToUpdate);

            //Assert
            mokDiscountGrpc.Verify(service => service.GetDiscount(It.IsAny<string>()), Times.Exactly(basketToUpdate.Items.Count));


        }



        [Fact]
        public async Task UpdateBasket_OnCall_InvokeBasketRepositoryServiceExactlyOnce()   // sucess
        {
            //Arrange
            var mockBasketRepo = new Mock<IBasketRepository>();
            var mokDiscountGrpc = new Mock<DiscountGrpcService>();
            var basketToUpdate = new ShoppingCart()
            {
                UserName = "username",
                Items = new List<ShoppingCartItem>()
                {
                    new ShoppingCartItem() {ProductName="p1", Color="red", Price=15, ProductId="p1", Quantity=3},
                    new ShoppingCartItem() {ProductName="p2", Color="green", Price=15, ProductId="p2", Quantity=3},
                    new ShoppingCartItem() {ProductName="p3", Color="black", Price=15, ProductId="p3", Quantity=3}
                }
            };

            var updatedBasket = new ShoppingCart()
            {
                UserName = "username",
                Items = new List<ShoppingCartItem>()
                {
                    new ShoppingCartItem() {ProductName="p1", Color="red", Price=5, ProductId="p1", Quantity=3},
                    new ShoppingCartItem() {ProductName="p2", Color="green", Price=5, ProductId="p2", Quantity=3},
                    new ShoppingCartItem() {ProductName="p3", Color="black", Price=5, ProductId="p3", Quantity=3}
                }
            };

            mockBasketRepo.Setup(service => service.UpdateBasket(basketToUpdate)).ReturnsAsync(updatedBasket);
            mokDiscountGrpc.Setup(service => service.GetDiscount(It.IsAny<string>())).ReturnsAsync(new CouponModel() { });


            //Act
            var sut = new BasketController(mockBasketRepo.Object,
                mokDiscountGrpc.Object,
                null,
                null);

            var result = await sut.UpdateBasket(basketToUpdate);

            //Assert
            mockBasketRepo.Verify(service => service.UpdateBasket(basketToUpdate), Times.Once);


        }


       



    }
}
