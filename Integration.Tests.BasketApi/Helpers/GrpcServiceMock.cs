using Basket_API.GRPC_Services;
using Dicount_GRPC;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integration.Tests.BasketApi.Helpers
{
    public static class GrpcServiceMock
    {
        public static DiscountGrpcService getGrpcService()         
        {
            var mokDiscountGrpc = new Mock<DiscountGrpcService>();

            //_service = mokDiscountGrpc.Object;
            mokDiscountGrpc.Setup(service => service.GetDiscount(It.IsAny<string>())).ReturnsAsync(new CouponModel() { Amount = 10 });

            return mokDiscountGrpc.Object;
        }
    }
}
