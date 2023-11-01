using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Numerics;

namespace EcommerceCore.Models
{
    public class BaseCartItem : BaseClass
    {
        [Key]
        public int Id { get; set; }
        public int Quantity { get; set; }
        public int? ProductId { get; set; }
        public int? CartId { get; set; }
    }

    public class CartItem : BaseCartItem
    {
        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; }
        [ForeignKey(nameof(CartId))]
        public Cart Cart { get; set; }
    }
}
