using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Numerics;

namespace EcommerceCore.Models
{
    public class BaseCart : BaseClass
    {
        [Key]
        public int Id { get; set; }
        public decimal Total { get; set; }
        public int? CouponId { get; set; }
    }

    public class Cart : BaseCart
    {
        [ForeignKey(nameof(CouponId))]
        public Coupon Coupon { get; set; }

        public ICollection<CartItem> CartItems { get; set; }
    }
}
