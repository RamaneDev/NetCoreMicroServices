using Dicount_GRPC;
using System.Threading.Tasks;

namespace Basket_API.GRPC_Services
{
    public interface IDiscountGrpcService
    {
        Task<CouponModel> GetDiscount(string productName);
    }
}
