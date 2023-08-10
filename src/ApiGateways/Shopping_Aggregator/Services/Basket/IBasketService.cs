using Shopping.Aggregator.Models;
using System.Threading.Tasks;

namespace Shopping_Aggregator.Services.Basket
{
    public interface IBasketService
    {
        Task<BasketModel> GetBasket(string userName);
    }
}
