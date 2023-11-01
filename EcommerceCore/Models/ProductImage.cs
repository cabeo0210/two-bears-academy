using EcommerceCore.Models ;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Numerics;
using EcommerceCore.Models ;
using EcommerceCore.Models;

namespace EcommerceCore.Models
{
    public class BaseProductImage
    {
        [Key]
        public int Id { get; set; }
        public int? ProductId { get; set; }
        public int? ImageId { get; set; }
    }

    public class ProductImage : BaseProductImage
    {
        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; }
        [ForeignKey(nameof(ImageId))]
        public Image Image { get; set; }
    }
}
