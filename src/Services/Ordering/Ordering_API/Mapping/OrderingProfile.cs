using AutoMapper;
using EventBus_Message.Events;
using Ordering_Application.Features.Orders.Commands.CheckoutOrder;

namespace Ordering_API.Mapping
{
    public class OrderingProfile : Profile
    {
        public OrderingProfile()
        {
            CreateMap<CheckoutOrderCommand, BasketCheckoutEvent>().ReverseMap();
        }
    }
}
