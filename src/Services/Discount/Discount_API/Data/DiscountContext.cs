using Discount_API.Entities;
using Microsoft.EntityFrameworkCore;

namespace Discount_API.Data
{
    public class DiscountContext : DbContext
    {
        public DiscountContext(DbContextOptions<DiscountContext> options) : base(options)
        {        
        }

        public DbSet<Coupon> Coupons { get; set; }
    }
}
