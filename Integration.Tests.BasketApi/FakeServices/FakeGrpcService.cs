using Basket_API.GRPC_Services;
using Dicount_GRPC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integration.Tests.BasketApi.fakeServices
{
    public class FakeGrpcService : IDiscountGrpcService
    {
        
        public async Task<CouponModel> GetDiscount(string productName)
        {
            await Task.Delay(100);
            
            return new CouponModel { Amount = 10 };
        }
    }
}
