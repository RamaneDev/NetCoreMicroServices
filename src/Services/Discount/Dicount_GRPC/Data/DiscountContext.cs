using Dicount_GRPC.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dicount_GRPC.Data
{
    public class DiscountContext : DbContext
    {
        public DiscountContext(DbContextOptions<DiscountContext> options) : base(options)
        {        
        }

        public DbSet<Coupon> Coupons { get; set; }
    }
}
