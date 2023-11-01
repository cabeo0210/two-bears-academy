using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Numerics;
using EcommerceCore.Models ;

namespace EcommerceCore.Models
{
    public class BaseProduct : BaseClass
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
        public int Status { get; set; }
        public int Quantity { get; set; }
        public string Description { get; set; }
        public decimal BasePrice { get; set; }
        public decimal SellPrice { get; set; }
        public int? CategoryId { get; set; }
    }

    public class Product : BaseProduct
    {
        [ForeignKey(nameof(CategoryId))]
        public Category Category { get; set; }
        public ICollection<ProductImage> ProductImages { get; set; }
    }
}
