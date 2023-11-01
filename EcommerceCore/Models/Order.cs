using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Numerics;
using EcommerceCore.Models ;
using EcommerceCore.Models;

namespace EcommerceCore.Models
{
    public class BaseOrder : BaseClass
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }
        [Required]
        [MaxLength(255)]
        public string Code { get; set; }
        [Required]
        public int OrderStatus { get; set; }
        [Required]
        public int PaymentStatus { get; set; }
        [Required]
        public int ShippingStatus { get; set; }
        public string Note { get; set; }
        public decimal Total { get; set; }
        public string Address { get; set; }
        public int? CreateByUserId { get; set; }
    }

    public class Order : BaseOrder
    {
        [ForeignKey(nameof(CreateByUserId))]
        public User User { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
