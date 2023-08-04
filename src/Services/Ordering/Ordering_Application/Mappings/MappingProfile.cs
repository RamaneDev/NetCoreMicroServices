using AutoMapper;
using Ordering_Application.Features.Orders.Commands.CheckoutOrder;
using Ordering_Application.Features.Orders.Commands.UpdateOrder;
using Ordering_Application.Features.Orders.Queries.GetOrdersList;
using Ordering_Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering_Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Order, OrderDto>().ReverseMap();
            CreateMap<Order, CheckoutOrderCommand>().ReverseMap();
            CreateMap<Order, UpdateOrderCommand>().ReverseMap();
        }
    }
}
