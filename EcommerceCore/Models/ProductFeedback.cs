using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Numerics;
using EcommerceCore.Models ;
using EcommerceCore.Models;

namespace EcommerceCore.Models
{
    public class BaseProductFeedback : BaseClass
    {
        [Key]
        public int Id { get; set; }
        public string Content { get; set; }
        public int Type { get; set; }
        public int? ProductId { get; set; }
        public int? CreateByUserId { get; set; }
    }

    public class ProductFeedback : BaseProductFeedback
    {
        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; }
        [ForeignKey(nameof(CreateByUserId))]
        public User User { get; set; }
    }
}
