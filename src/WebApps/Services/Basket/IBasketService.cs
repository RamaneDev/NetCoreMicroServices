using AspnetRunBasics.Models;
using System.Threading.Tasks;

namespace AspnetRunBasics.Services.Basket
{
    public interface IBasketService
    {
        Task<BasketModel> GetBasket(string userName);
        Task<BasketModel> UpdateBasket(BasketModel model);
        Task CheckoutBasket(BasketCheckoutModel model);
    }
}
