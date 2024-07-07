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
    }
}
