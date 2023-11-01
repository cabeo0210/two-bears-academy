using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Numerics;
using EcommerceCore.Models ;

namespace EcommerceCore.Models
{
    public class BaseOrderItem : BaseClass
    {
        [Key]
        public int Id { get; set; }
        public decimal Price { get; set; }
        public decimal PriceTotal { get; set; }
        public int Quantity { get; set; }
        public int? ProductId { get; set; }
        public int? OrderId { get; set; }
    }

    public class OrderItem : BaseOrderItem
    {
        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; }
        [ForeignKey(nameof(OrderId))]
        public Order Order { get; set; }
    }
}
