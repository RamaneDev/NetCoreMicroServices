using AutoMapper;
using Dicount_GRPC.Entities;

namespace Dicount_GRPC.Mapping
{
    public class DiscountProfile : Profile
    {
        public DiscountProfile()
        {
            CreateMap<Coupon, CouponModel>().ReverseMap();
        }
    }
}
