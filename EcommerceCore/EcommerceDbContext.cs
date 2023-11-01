using EcommerceCore.Configurations;
using EcommerceCore.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace Ecommerce
{
    public class EcommerceDbContext : DbContext
    {
        public EcommerceDbContext()
        {

        }

        public EcommerceDbContext(DbContextOptions<EcommerceDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder
                .ApplyConfiguration(new UserConfiguration());
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductFeedback> ProductFeedbacks { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<CartHistory> CartHistories { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<CouponHistory> CouponHistories { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<New> News { get; set; }
        public DbSet<ProductHistory> ProductHistories { get; set; }
    }
}
