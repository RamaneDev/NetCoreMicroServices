using AutoMapper;
using Basket_API.Entities;
using EventBus_Message.Events;

namespace Basket_API.Mapping
{
    public class BasketProfile : Profile
    {
        public BasketProfile()
        {
            CreateMap<BasketCheckout, BasketCheckoutEvent>().ReverseMap();
        }
    }
}
