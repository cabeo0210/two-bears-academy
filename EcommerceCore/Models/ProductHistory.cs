using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Numerics;

namespace EcommerceCore.Models
{
    public class BaseProductHistory : BaseClass
    {
        [Key]
        public int Id { get; set; }
        public int? UserId { get; set; }
        public int? ProductId { get; set; }
    }

    public class ProductHistory : BaseProductHistory
    {
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }

        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; }
    }
}
