using Discount_API.Data;
using Discount_API.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Discount_API.Repositories
{
    public class DiscountRepository : IDiscountRepository
    {
        private readonly DiscountContext _context;

        public DiscountRepository(DiscountContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateDiscount(Coupon coupon)
        {
            _context.Coupons.Add(coupon);            
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteDiscount(string productName)
        {
            var coupon = await _context.Coupons.FirstOrDefaultAsync(c => c.ProductName.Equals(productName));
            _context.Coupons.Remove(coupon);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Coupon> GetDiscount(string productName)
        {
            return await _context.Coupons.FirstOrDefaultAsync(c => c.ProductName.Equals(productName));
        }

        public async Task<bool> UpdateDiscount(Coupon coupon)
        {
            var couponFromRepo = await _context.Coupons.FindAsync(coupon.Id);
            couponFromRepo.ProductName = coupon.ProductName;
            couponFromRepo.Amount = coupon.Amount;
            couponFromRepo.Description = coupon.Description;
            return await _context.SaveChangesAsync() > 0;            
        }
    }
}
