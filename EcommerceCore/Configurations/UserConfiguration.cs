
using EcommerceCore.Const;
using EcommerceCore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcommerceCore.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> modelBuilder)
        {
            modelBuilder
                .ToTable("User");

            modelBuilder.HasData(new User
            {
                UserId = 1,
                Email = "admin@XProfile.com".ToLower(),
                Address = "21 Street 6",
                CreatedAt = new DateTime(2022, 1, 1, 0, 0, 0, 0, DateTimeKind.Local).AddTicks(0),
                UpdatedAt = new DateTime(2022, 1, 1, 0, 0, 0, 0, DateTimeKind.Local).AddTicks(0),
                IsActive = true,
                Avatar = "",
                CartId = null,
                IsDeleted = false,
                Role = (int)SysEnum.Role.Admin,
                Name = "XProfile Admin",
                Password = "123456".Hash(),
                Phone = "0123456789",
            });
        }
    }
}
